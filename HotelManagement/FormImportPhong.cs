using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using HotelManagement.BLL.DTOs;
using HotelManagement.BLL.Services;
using Microsoft.Extensions.DependencyInjection;

namespace HotelManagement.Forms
{
    /// <summary>
    /// Màn hình Import danh sách phòng từ file Excel có validate toàn diện (DCKLTTQ-79).
    /// Chỉ dành cho vai trò Quản trị viên (Admin).
    /// </summary>
    public partial class FormImportPhong : Form
    {
        private readonly IServiceScope _scope;
        private readonly IPhongImportService _phongImportService;

        private List<PhongImportDto> _danhSachKetQua = new();
        private string? _duongDanFileHienTai;

        public FormImportPhong()
        {
            _scope = Program.ServiceProvider.CreateScope();
            _phongImportService = _scope.ServiceProvider.GetRequiredService<IPhongImportService>();

            InitializeComponent();
            DangKySuKien();
        }

        private void DangKySuKien()
        {
            FormClosed += (_, _) => _scope.Dispose();

            _btnChonFile.Click += async (_, _) => await ChonFileVaKiemTraAsync();
            _btnKiemTra.Click += async (_, _) => await KiemTraFileHienTaiAsync();
            _btnTaiMau.Click += TaiFileMauExcel;
            _chkChiHienThiLoi.CheckedChanged += (_, _) => CapNhatDuLieuLuoi();
            _dgvKetQua.CellFormatting += DgvKetQua_CellFormatting;
            _btnImport.Click += async (_, _) => await ThucHienImportAsync();
            _btnDong.Click += (_, _) => Close();
        }

        private async Task ChonFileVaKiemTraAsync()
        {
            using var ofd = new OpenFileDialog
            {
                Title = "Chọn file danh sách phòng Excel để import",
                Filter = "File Excel (*.xlsx;*.xls)|*.xlsx;*.xls|Tất cả các file (*.*)|*.*",
                Multiselect = false
            };

            if (ofd.ShowDialog(this) == DialogResult.OK)
            {
                _duongDanFileHienTai = ofd.FileName;
                _txtDuongDan.Text = _duongDanFileHienTai;
                _btnKiemTra.Enabled = true;

                await KiemTraVaHienThiDuLieuAsync(_duongDanFileHienTai);
            }
        }

        private async Task KiemTraFileHienTaiAsync()
        {
            if (string.IsNullOrWhiteSpace(_duongDanFileHienTai) || !File.Exists(_duongDanFileHienTai))
            {
                MessageBox.Show("Vui lòng chọn file Excel hợp lệ trước khi kiểm tra.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            await KiemTraVaHienThiDuLieuAsync(_duongDanFileHienTai);
        }

        private async Task KiemTraVaHienThiDuLieuAsync(string duongDanFile)
        {
            BậtTrạngTháiĐangTải(true, "Đang đọc và kiểm tra dữ liệu từ file Excel...");

            try
            {
                _danhSachKetQua = await _phongImportService.DocVaValidateAsync(duongDanFile);

                int tongSo = _danhSachKetQua.Count;
                int soHopLe = _danhSachKetQua.Count(x => x.HopLe);
                int soLoi = _danhSachKetQua.Count(x => !x.HopLe);

                _lblTongSoDong.Text = tongSo.ToString();
                _lblSoDongHopLe.Text = soHopLe.ToString();
                _lblSoDongLoi.Text = soLoi.ToString();

                CapNhatDuLieuLuoi();

                _btnImport.Enabled = soHopLe > 0;
                _btnImport.Text = soHopLe > 0
                    ? $"⚡  Tiến hành Import ({soHopLe} phòng hợp lệ)"
                    : "⚡  Tiến hành Import vào hệ thống";

                if (tongSo == 0)
                {
                    _lblTrangThaiXuLy.Text = "Không tìm thấy dòng dữ liệu nào trong file Excel.";
                    _lblTrangThaiXuLy.ForeColor = Color.FromArgb(153, 27, 27);
                }
                else if (soLoi == 0)
                {
                    _lblTrangThaiXuLy.Text = $"Hoàn tất kiểm tra: Toàn bộ {tongSo} dòng đều hợp lệ! Có thể import ngay.";
                    _lblTrangThaiXuLy.ForeColor = Color.FromArgb(22, 101, 52);
                }
                else
                {
                    _lblTrangThaiXuLy.Text = $"Kiểm tra xong: Có {soHopLe} dòng hợp lệ, {soLoi} dòng bị lỗi cần chỉnh sửa.";
                    _lblTrangThaiXuLy.ForeColor = Color.FromArgb(180, 83, 9);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Đã xảy ra lỗi khi đọc file Excel:\n{ex.Message}", "Lỗi đọc file",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);

                _lblTrangThaiXuLy.Text = "Lỗi khi xử lý file Excel.";
                _lblTrangThaiXuLy.ForeColor = Color.FromArgb(153, 27, 27);
                _btnImport.Enabled = false;
            }
            finally
            {
                BậtTrạngTháiĐangTải(false);
            }
        }

        private void CapNhatDuLieuLuoi()
        {
            var nguonDuLieu = _chkChiHienThiLoi.Checked
                ? _danhSachKetQua.Where(x => !x.HopLe).ToList()
                : _danhSachKetQua;

            _dgvKetQua.DataSource = null;
            _dgvKetQua.DataSource = nguonDuLieu;
            _dgvKetQua.ClearSelection();
        }

        private void DgvKetQua_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= _dgvKetQua.Rows.Count) return;

            var row = _dgvKetQua.Rows[e.RowIndex];
            if (row.DataBoundItem is not PhongImportDto dto) return;

            // Tô màu nền cho cả dòng
            if (!dto.HopLe)
            {
                row.DefaultCellStyle.BackColor = Color.FromArgb(254, 242, 242);
                row.DefaultCellStyle.SelectionBackColor = Color.FromArgb(254, 202, 202);
                row.DefaultCellStyle.SelectionForeColor = Color.FromArgb(153, 27, 27);
            }
            else
            {
                row.DefaultCellStyle.BackColor = Color.FromArgb(240, 253, 244);
                row.DefaultCellStyle.SelectionBackColor = Color.FromArgb(187, 247, 208);
                row.DefaultCellStyle.SelectionForeColor = Color.FromArgb(22, 101, 52);
            }

            // Định dạng riêng cho cột "Kết quả"
            if (_dgvKetQua.Columns[e.ColumnIndex].DataPropertyName == "KetQuaKiemTra")
            {
                e.CellStyle.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
                e.CellStyle.ForeColor = dto.HopLe
                    ? Color.FromArgb(22, 101, 52)
                    : Color.FromArgb(153, 27, 27);
            }

            // Định dạng riêng cho cột "Chi tiết lỗi"
            if (_dgvKetQua.Columns[e.ColumnIndex].DataPropertyName == "ChiTietLoi")
            {
                if (!dto.HopLe)
                {
                    e.CellStyle.ForeColor = Color.FromArgb(185, 28, 28);
                }
            }
        }

        private async Task ThucHienImportAsync()
        {
            var dsHopLe = _danhSachKetQua.Where(x => x.HopLe).ToList();
            if (dsHopLe.Count == 0)
            {
                MessageBox.Show("Không có dòng hợp lệ nào để import.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int soLoi = _danhSachKetQua.Count - dsHopLe.Count;
            string thongBaoXacNhan = soLoi > 0
                ? $"Hệ thống tìm thấy {dsHopLe.Count} dòng hợp lệ sẵn sàng import vào cơ sở dữ liệu.\n" +
                  $"({soLoi} dòng bị lỗi sẽ được bỏ qua không lưu).\n\n" +
                  $"Bạn có chắc chắn muốn tiến hành lưu {dsHopLe.Count} phòng này không?"
                : $"Toàn bộ {dsHopLe.Count} dòng đều hợp lệ.\n\n" +
                  $"Bạn có chắc chắn muốn tiến hành lưu {dsHopLe.Count} phòng này vào cơ sở dữ liệu không?";

            var xacNhan = MessageBox.Show(thongBaoXacNhan, "Xác nhận Import phòng",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (xacNhan != DialogResult.Yes) return;

            BậtTrạngTháiĐangTải(true, $"Đang lưu {dsHopLe.Count} phòng vào cơ sở dữ liệu qua transaction...");

            try
            {
                int soLuongDaLuu = await _phongImportService.LuuDanhSachHopLeAsync(_danhSachKetQua);

                MessageBox.Show(
                    $"Import thành công {soLuongDaLuu} phòng vào hệ thống!",
                    "Import thành công",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                _btnImport.Enabled = false;
                _btnImport.Text = "Đã import thành công";
                _lblTrangThaiXuLy.Text = $"Đã lưu thành công {soLuongDaLuu} phòng vào cơ sở dữ liệu.";
                _lblTrangThaiXuLy.ForeColor = Color.FromArgb(22, 101, 52);

                // Sau khi lưu xong, nếu người dùng muốn kiểm tra lại chính file này,
                // các phòng vừa lưu sẽ tự động bị đánh dấu là "đã tồn tại trong DB"
                if (!string.IsNullOrEmpty(_duongDanFileHienTai))
                {
                    await KiemTraVaHienThiDuLieuAsync(_duongDanFileHienTai);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Lỗi khi lưu dữ liệu vào cơ sở dữ liệu (Transaction đã được hoàn tác):\n{ex.Message}",
                    "Lỗi lưu dữ liệu",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                _lblTrangThaiXuLy.Text = "Lưu dữ liệu thất bại.";
                _lblTrangThaiXuLy.ForeColor = Color.FromArgb(153, 27, 27);
            }
            finally
            {
                BậtTrạngTháiĐangTải(false);
            }
        }

        private void TaiFileMauExcel(object? sender, EventArgs e)
        {
            try
            {
                using var sfd = new SaveFileDialog
                {
                    Title = "Lưu file biểu mẫu Excel nhập danh sách phòng",
                    Filter = "File Excel (*.xlsx)|*.xlsx",
                    FileName = "Mau_Import_Phong.xlsx"
                };

                if (sfd.ShowDialog(this) == DialogResult.OK)
                {
                    byte[] fileBytes = _phongImportService.TaoFileMauExcel();
                    File.WriteAllBytes(sfd.FileName, fileBytes);

                    var hoiMoFile = MessageBox.Show(
                        $"Đã xuất file mẫu thành công tại:\n{sfd.FileName}\n\nBạn có muốn mở file ngay bây giờ không?",
                        "Tải file mẫu thành công",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);

                    if (hoiMoFile == DialogResult.Yes)
                    {
                        Process.Start(new ProcessStartInfo
                        {
                            FileName = sfd.FileName,
                            UseShellExecute = true
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải file mẫu Excel:\n{ex.Message}", "Lỗi xuất file",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BậtTrạngTháiĐangTải(bool dangTai, string? thongBao = null)
        {
            UseWaitCursor = dangTai;
            _btnChonFile.Enabled = !dangTai;
            _btnKiemTra.Enabled = !dangTai && !string.IsNullOrEmpty(_duongDanFileHienTai);
            _btnTaiMau.Enabled = !dangTai;
            _btnDong.Enabled = !dangTai;

            if (dangTai)
            {
                _btnImport.Enabled = false;
                if (!string.IsNullOrEmpty(thongBao))
                {
                    _lblTrangThaiXuLy.Text = thongBao;
                    _lblTrangThaiXuLy.ForeColor = Color.FromArgb(30, 95, 138);
                }
            }
            Application.DoEvents();
        }
    }
}
