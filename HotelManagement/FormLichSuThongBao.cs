using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using HotelManagement.DAL.Repositories;
using HotelManagement.Models.Entities;
using Microsoft.Extensions.DependencyInjection;

namespace HotelManagement.Forms
{
    /// <summary>
    /// Màn hình Xem lịch sử gửi thông báo email (DCKLTTQ-82).
    /// Hỗ trợ xem danh sách, lọc theo loại thông báo, trạng thái và tìm kiếm theo mã booking/email.
    /// </summary>
    public partial class FormLichSuThongBao : Form
    {
        private readonly IServiceScope _scope;
        private readonly IUnitOfWork _unitOfWork;

        private List<LichSuGuiThongBao> _danhSachGoc = new();

        public FormLichSuThongBao()
        {
            _scope = Program.ServiceProvider.CreateScope();
            _unitOfWork = _scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            InitializeComponent();

            FormClosed += (_, _) => _scope.Dispose();

            Load += async (_, _) => await TaiDuLieuAsync();

            _btnLoc.Click += (_, _) => ApDungBoLoc();
            _btnLamMoi.Click += async (_, _) =>
            {
                _cboLoaiThongBao.SelectedIndex = 0;
                _cboTrangThai.SelectedIndex = 0;
                _txtTimKiem.Clear();
                await TaiDuLieuAsync();
            };

            _txtTimKiem.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    ApDungBoLoc();
                }
            };

            _dgvLichSu.SelectionChanged += (_, _) => HienThiChiTietDongChon();
            _dgvLichSu.CellFormatting += DgvLichSu_CellFormatting;

            _btnDong.Click += (_, _) => Close();
        }

        private async Task TaiDuLieuAsync()
        {
            try
            {
                _btnLamMoi.Enabled = false;
                _btnLoc.Enabled = false;

                var data = await _unitOfWork.LichSuGuiThongBaoRepository.LayDanhSachMoiNhatAsync(250);
                _danhSachGoc = data.ToList();

                ApDungBoLoc();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải lịch sử gửi thông báo: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _btnLamMoi.Enabled = true;
                _btnLoc.Enabled = true;
            }
        }

        private void ApDungBoLoc()
        {
            var query = _danhSachGoc.AsEnumerable();

            // 1. Lọc theo Loại thông báo
            if (_cboLoaiThongBao.SelectedIndex > 0)
            {
                string loaiChon = _cboLoaiThongBao.SelectedItem?.ToString() ?? string.Empty;
                query = query.Where(x => x.LoaiThongbao != null && x.LoaiThongbao.Equals(loaiChon, StringComparison.OrdinalIgnoreCase));
            }

            // 2. Lọc theo Trạng thái (hỗ trợ cả tiếng Việt lẫn mã không dấu)
            if (_cboTrangThai.SelectedIndex > 0)
            {
                int ttIndex = _cboTrangThai.SelectedIndex;
                if (ttIndex == 1) // Thành công
                {
                    query = query.Where(x => x.TrangThai != null && (
                        x.TrangThai.Equals("Thành công", StringComparison.OrdinalIgnoreCase) ||
                        x.TrangThai.Equals("ThanhCong", StringComparison.OrdinalIgnoreCase) ||
                        x.TrangThai.Equals("Success", StringComparison.OrdinalIgnoreCase)));
                }
                else if (ttIndex == 2) // Thất bại
                {
                    query = query.Where(x => x.TrangThai != null && (
                        x.TrangThai.Equals("Thất bại", StringComparison.OrdinalIgnoreCase) ||
                        x.TrangThai.Equals("ThatBai", StringComparison.OrdinalIgnoreCase) ||
                        x.TrangThai.Equals("Failed", StringComparison.OrdinalIgnoreCase) ||
                        x.TrangThai.Equals("Error", StringComparison.OrdinalIgnoreCase)));
                }
            }

            // 3. Lọc theo từ khóa tìm kiếm (Mã booking hoặc Email hoặc Tiêu đề)
            string tuKhoa = _txtTimKiem.Text.Trim();
            if (!string.IsNullOrEmpty(tuKhoa))
            {
                query = query.Where(x =>
                    (x.NguoiNhan != null && x.NguoiNhan.Contains(tuKhoa, StringComparison.OrdinalIgnoreCase)) ||
                    (x.IdDatphongNavigation != null && x.IdDatphongNavigation.Mabooking.Contains(tuKhoa, StringComparison.OrdinalIgnoreCase)) ||
                    (x.IdHoadonNavigation != null && x.IdHoadonNavigation.IdDatphongNavigation != null &&
                     x.IdHoadonNavigation.IdDatphongNavigation.Mabooking.Contains(tuKhoa, StringComparison.OrdinalIgnoreCase)) ||
                    (x.TieuDe != null && x.TieuDe.Contains(tuKhoa, StringComparison.OrdinalIgnoreCase))
                );
            }

            var ketQua = query.ToList();

            // Cập nhật thống kê
            int tong = ketQua.Count;
            int thanhCong = ketQua.Count(x => x.TrangThai != null && (
                x.TrangThai.Equals("Thành công", StringComparison.OrdinalIgnoreCase) ||
                x.TrangThai.Equals("ThanhCong", StringComparison.OrdinalIgnoreCase) ||
                x.TrangThai.Equals("Success", StringComparison.OrdinalIgnoreCase)));
            int thatBai = tong - thanhCong;

            _lblBadgeTong.Text = $"Tổng: {tong}";
            _lblBadgeThanhCong.Text = $"✓ Thành công: {thanhCong}";
            _lblBadgeThatBai.Text = $"⚠ Thất bại: {thatBai}";

            // Nạp dữ liệu vào DataGridView
            _dgvLichSu.Rows.Clear();
            foreach (var item in ketQua)
            {
                string maBooking = item.IdDatphongNavigation?.Mabooking
                    ?? item.IdHoadonNavigation?.IdDatphongNavigation?.Mabooking
                    ?? "-";

                int rowIndex = _dgvLichSu.Rows.Add(
                    item.IdLichsu,
                    item.ThoiGianGui.ToString("dd/MM/yyyy HH:mm:ss"),
                    item.LoaiThongbao,
                    maBooking,
                    item.NguoiNhan,
                    item.TieuDe ?? "-",
                    item.TrangThai,
                    item.LyDoLoi ?? "-"
                );

                _dgvLichSu.Rows[rowIndex].Tag = item;
            }

            if (ketQua.Count == 0)
            {
                if (_danhSachGoc.Count == 0)
                {
                    _lblChiTietTieuDe.Text = "ℹ Chưa có lịch sử gửi email nào trong CSDL:";
                    _lblChiTietTieuDe.ForeColor = AppTheme.TextSecondary;
                    _txtChiTietLoi.Text = "Hệ thống chưa ghi nhận thông báo nào. Khi bạn tạo 'Đặt phòng mới' hoặc 'Thanh toán hóa đơn', lịch sử gửi email sẽ tự động xuất hiện tại đây.";
                }
                else
                {
                    _lblChiTietTieuDe.Text = "ℹ Không có kết quả phù hợp với bộ lọc:";
                    _lblChiTietTieuDe.ForeColor = AppTheme.StatusWarningText;
                    _txtChiTietLoi.Text = $"Không tìm thấy bản ghi nào với Loại = '{_cboLoaiThongBao.SelectedItem}', Trạng thái = '{_cboTrangThai.SelectedItem}', Từ khóa = '{tuKhoa}'.\r\nNhấn 'Làm mới' hoặc chỉnh điều kiện lọc để xem toàn bộ danh sách.";
                }
            }
            else
            {
                if (_dgvLichSu.Rows.Count > 0)
                {
                    _dgvLichSu.Rows[0].Selected = true;
                }
                HienThiChiTietDongChon();
            }
        }

        private void HienThiChiTietDongChon()
        {
            if (_dgvLichSu.CurrentRow?.Tag is LichSuGuiThongBao item)
            {
                bool isSuccess = item.TrangThai.Equals("Thành công", StringComparison.OrdinalIgnoreCase) ||
                                 item.TrangThai.Equals("ThanhCong", StringComparison.OrdinalIgnoreCase) ||
                                 item.TrangThai.Equals("Success", StringComparison.OrdinalIgnoreCase);

                if (isSuccess)
                {
                    _lblChiTietTieuDe.Text = $"Chi tiết thông báo [#{item.IdLichsu} - {item.LoaiThongbao}]:";
                    _lblChiTietTieuDe.ForeColor = AppTheme.StatusSuccessText;
                    _txtChiTietLoi.Text = $"Email đã được gửi thành công đến '{item.NguoiNhan}' lúc {item.ThoiGianGui:dd/MM/yyyy HH:mm:ss}.\r\nTiêu đề: {item.TieuDe}";
                }
                else
                {
                    _lblChiTietTieuDe.Text = $"Chi tiết thông báo lỗi [#{item.IdLichsu} - {item.LoaiThongbao}]:";
                    _lblChiTietTieuDe.ForeColor = AppTheme.StatusDangerText;
                    _txtChiTietLoi.Text = $"Gửi thất bại tới '{item.NguoiNhan}' lúc {item.ThoiGianGui:dd/MM/yyyy HH:mm:ss}.\r\nTiêu đề: {item.TieuDe}\r\nLý do lỗi: {item.LyDoLoi ?? "Không rõ nguyên nhân"}";
                }
            }
            else if (_dgvLichSu.Rows.Count > 0)
            {
                _lblChiTietTieuDe.Text = "Chi tiết thông báo / Lỗi:";
                _lblChiTietTieuDe.ForeColor = AppTheme.TextSecondary;
                _txtChiTietLoi.Text = "(Chọn một dòng trong danh sách để xem chi tiết đầy đủ)";
            }
        }

        private void DgvLichSu_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            string colName = _dgvLichSu.Columns[e.ColumnIndex].Name;
            if (colName == "TrangThai" && e.Value != null)
            {
                string val = e.Value.ToString() ?? string.Empty;
                bool isSuccess = val.Equals("Thành công", StringComparison.OrdinalIgnoreCase) ||
                                 val.Equals("ThanhCong", StringComparison.OrdinalIgnoreCase) ||
                                 val.Equals("Success", StringComparison.OrdinalIgnoreCase);

                if (isSuccess)
                {
                    e.CellStyle!.ForeColor = AppTheme.StatusSuccessText;
                    e.CellStyle!.BackColor = AppTheme.StatusSuccessBg;
                    e.CellStyle!.Font = AppTheme.FontSmallBold;
                    e.CellStyle!.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }
                else
                {
                    e.CellStyle!.ForeColor = AppTheme.StatusDangerText;
                    e.CellStyle!.BackColor = AppTheme.StatusDangerBg;
                    e.CellStyle!.Font = AppTheme.FontSmallBold;
                    e.CellStyle!.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }
            }
        }
    }
}
