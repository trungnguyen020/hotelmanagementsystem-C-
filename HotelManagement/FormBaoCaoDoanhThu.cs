using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using HotelManagement.BLL.DTOs;
using HotelManagement.BLL.Services;
using HotelManagement.Models.Entities;
using Microsoft.Extensions.DependencyInjection;

namespace HotelManagement.Forms
{
    /// <summary>
    /// Màn hình Báo cáo doanh thu & Hiệu suất hoạt động phòng (DCKLTTQ-78).
    /// Hỗ trợ lọc theo khoảng ngày, lọc theo loại phòng, xem trước trên DataGridView và xuất file Excel bằng ClosedXML.
    /// Dựng control hoàn toàn bằng code trong constructor, tuân thủ DI scope.
    /// </summary>
    public partial class FormBaoCaoDoanhThu : Form
    {
        private readonly IServiceScope _scope;
        private readonly IBaoCaoService _baoCaoService;
        private readonly IBaoCaoExcelService _baoCaoExcelService;
        private readonly TaiKhoan? _taiKhoanDangNhap;

        private BaoCaoDoanhThuDto? _baoCaoHienTai;

        private class ComboBoxItem
        {
            public string Text { get; set; } = string.Empty;
            public int? Value { get; set; }

            public override string ToString() => Text;
        }

        public FormBaoCaoDoanhThu(TaiKhoan? taiKhoanDangNhap = null)
        {
            _taiKhoanDangNhap = taiKhoanDangNhap;
            _scope = Program.ServiceProvider.CreateScope();
            _baoCaoService = _scope.ServiceProvider.GetRequiredService<IBaoCaoService>();
            _baoCaoExcelService = _scope.ServiceProvider.GetRequiredService<IBaoCaoExcelService>();

            InitializeComponent();

            // Mặc định khoảng ngày: Từ ngày 1 đầu tháng hiện tại đến ngày hiện tại
            _dtpTuNgay.Value = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            _dtpDenNgay.Value = DateTime.Today;

            // Gán sự kiện
            Load += async (_, _) =>
            {
                // Đảm bảo window handle của các TabPage được khởi tạo đầy đủ tránh lỗi layout WinForms
                foreach (TabPage tab in _tabDuLieu.TabPages)
                {
                    var _ = tab.Handle;
                }

                await NapDanhMucLoaiPhongAsync();
                await TaiBaoCaoAsync();
            };

            _btnXemBaoCao.Click += async (_, _) => await TaiBaoCaoAsync();
            _btnXuatExcel.Click += async (_, _) => await XuatExcelAsync();
            FormClosed += (_, _) => _scope.Dispose();
        }

        private async Task NapDanhMucLoaiPhongAsync()
        {
            try
            {
                var danhSachLoai = await _baoCaoService.LayDanhSachLoaiPhongAsync();

                _cboLoaiPhong.Items.Clear();
                _cboLoaiPhong.Items.Add(new ComboBoxItem { Text = "--- Tất cả loại phòng ---", Value = null });

                foreach (var lp in danhSachLoai)
                {
                    _cboLoaiPhong.Items.Add(new ComboBoxItem
                    {
                        Text = lp.TenLoaiPhong,
                        Value = lp.Id
                    });
                }

                _cboLoaiPhong.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải danh mục loại phòng: " + ex.Message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private async Task TaiBaoCaoAsync()
        {
            if (_dtpTuNgay.Value.Date > _dtpDenNgay.Value.Date)
            {
                MessageBox.Show("Ngày bắt đầu không được lớn hơn ngày kết thúc.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                _btnXemBaoCao.Enabled = false;
                _btnXuatExcel.Enabled = false;
                _lblTrangThai.Text = "Đang tổng hợp dữ liệu báo cáo từ cơ sở dữ liệu...";
                Cursor = Cursors.WaitCursor;

                int? idLoaiPhongChon = null;
                string? tenLoaiPhongChon = null;

                if (_cboLoaiPhong.SelectedItem is ComboBoxItem item && item.Value.HasValue)
                {
                    idLoaiPhongChon = item.Value;
                    tenLoaiPhongChon = item.Text;
                }

                var filter = new BaoCaoFilterDto
                {
                    TuNgay = _dtpTuNgay.Value.Date,
                    DenNgay = _dtpDenNgay.Value.Date,
                    IdLoaiPhong = idLoaiPhongChon,
                    TenLoaiPhong = tenLoaiPhongChon
                };

                string nguoiLap = _taiKhoanDangNhap?.IdNhansuNavigation?.Hoten ?? "Admin";
                _baoCaoHienTai = await _baoCaoService.LayBaoCaoDoanhThuAsync(filter, nguoiLap);

                // Cập nhật giá trị các thẻ KPI
                _lblTongDoanhThuVal.Text = $"{_baoCaoHienTai.TongDoanhThu:N0} đ";
                _lblTienPhongVal.Text = $"{_baoCaoHienTai.TongTienPhong:N0} đ";
                _lblTienDichVuVal.Text = $"{_baoCaoHienTai.TongTienDichVu:N0} đ";
                _lblTyLeLapDayVal.Text = $"{_baoCaoHienTai.TyLeLapDayPhanTram:F1} %";
                _lblTongBookingVal.Text = $"{_baoCaoHienTai.TongSoBooking} đơn ({_baoCaoHienTai.TongSoDemLuuTru} đêm)";

                // Nạp dữ liệu vào bảng hóa đơn
                _dgvHoaDon.DataSource = null;
                _dgvHoaDon.DataSource = _baoCaoHienTai.DanhSachHoaDon.ToList();
                DinhDangBangHoaDon();
                _dgvHoaDon.ClearSelection();

                // Nạp dữ liệu vào bảng breakdown loại phòng
                _dgvLoaiPhong.DataSource = null;
                _dgvLoaiPhong.DataSource = _baoCaoHienTai.DanhSachLoaiPhong.ToList();
                DinhDangBangLoaiPhong();
                _dgvLoaiPhong.ClearSelection();

                // Cập nhật tiêu đề tab kèm số lượng
                _tabDuLieu.TabPages[0].Text = $"📋 Danh sách hóa đơn ({_baoCaoHienTai.DanhSachHoaDon.Count})";
                _tabDuLieu.TabPages[1].Text = $"🏢 Phân tích theo loại phòng ({_baoCaoHienTai.DanhSachLoaiPhong.Count})";

                _btnXuatExcel.Enabled = true;
                _lblTrangThai.Text = $"Đã tải {_baoCaoHienTai.DanhSachHoaDon.Count} hóa đơn. Tổng doanh thu thực: {_baoCaoHienTai.TongDoanhThu:N0} đ | Tỷ lệ lấp đầy: {_baoCaoHienTai.TyLeLapDayPhanTram:F1}%.";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Có lỗi xảy ra khi tổng hợp báo cáo: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _lblTrangThai.Text = "Tổng hợp báo cáo thất bại.";
            }
            finally
            {
                _btnXemBaoCao.Enabled = true;
                Cursor = Cursors.Default;
            }
        }

        private void DinhDangBangHoaDon()
        {
            if (_dgvHoaDon.Columns.Count == 0) return;

            // Đảm bảo handle tồn tại trước khi thao tác layout
            if (!_dgvHoaDon.IsHandleCreated)
                _ = _dgvHoaDon.Handle;

            _dgvHoaDon.SuspendLayout();
            try
            {
                // Trước tiên set tất cả cột về AutoSizeMode.None để tránh NullReferenceException
                foreach (DataGridViewColumn c in _dgvHoaDon.Columns)
                    c.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;

                void ConfigCol(string name, string header, int width, DataGridViewContentAlignment align, string? format = null, bool isFill = false)
                {
                    if (_dgvHoaDon.Columns[name] is DataGridViewColumn col)
                    {
                        col.HeaderText = header;
                        col.DefaultCellStyle.Alignment = align;
                        col.HeaderCell.Style.Alignment = align;
                        if (!string.IsNullOrEmpty(format))
                            col.DefaultCellStyle.Format = format;
                        if (isFill)
                        {
                            col.MinimumWidth = width;
                            col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                        }
                        else
                        {
                            col.Width = width;
                        }
                    }
                }

                ConfigCol(nameof(ChiTietHoaDonBaoCaoDto.IdHoaDon), "Số HĐ", 78, DataGridViewContentAlignment.MiddleCenter, "\\H\\D-00000");
                ConfigCol(nameof(ChiTietHoaDonBaoCaoDto.MaBooking), "Mã Booking", 115, DataGridViewContentAlignment.MiddleCenter);
                ConfigCol(nameof(ChiTietHoaDonBaoCaoDto.TenKhachHang), "Khách hàng", 130, DataGridViewContentAlignment.MiddleLeft, isFill: true);
                ConfigCol(nameof(ChiTietHoaDonBaoCaoDto.DanhSachPhong), "Phòng", 65, DataGridViewContentAlignment.MiddleCenter);
                ConfigCol(nameof(ChiTietHoaDonBaoCaoDto.NgayThanhToan), "Ngày thanh toán", 130, DataGridViewContentAlignment.MiddleCenter, "dd/MM/yyyy HH:mm");
                ConfigCol(nameof(ChiTietHoaDonBaoCaoDto.TienPhong), "Tiền phòng", 100, DataGridViewContentAlignment.MiddleRight, "#,##0 đ");
                ConfigCol(nameof(ChiTietHoaDonBaoCaoDto.TienDichVu), "Tiền dịch vụ", 95, DataGridViewContentAlignment.MiddleRight, "#,##0 đ");
                ConfigCol(nameof(ChiTietHoaDonBaoCaoDto.GiamGia), "Giảm giá", 75, DataGridViewContentAlignment.MiddleRight, "#,##0 đ");
                ConfigCol(nameof(ChiTietHoaDonBaoCaoDto.TongTien), "Tổng tiền", 115, DataGridViewContentAlignment.MiddleRight, "#,##0 đ");
                ConfigCol(nameof(ChiTietHoaDonBaoCaoDto.PhuongThucThanhToan), "Hình thức TT", 105, DataGridViewContentAlignment.MiddleCenter);

                if (_dgvHoaDon.Columns[nameof(ChiTietHoaDonBaoCaoDto.TongTien)] is DataGridViewColumn colTongTien)
                {
                    colTongTien.DefaultCellStyle.Font = new Font(_dgvHoaDon.Font, FontStyle.Bold);
                    colTongTien.DefaultCellStyle.ForeColor = Color.FromArgb(13, 59, 102);
                }
            }
            finally
            {
                _dgvHoaDon.ResumeLayout(true);
            }
        }

        private void DinhDangBangLoaiPhong()
        {
            if (_dgvLoaiPhong.Columns.Count == 0) return;

            // Đảm bảo handle tồn tại trước khi thao tác layout
            if (!_dgvLoaiPhong.IsHandleCreated)
                _ = _dgvLoaiPhong.Handle;

            _dgvLoaiPhong.SuspendLayout();
            try
            {
                // Trước tiên set tất cả cột về AutoSizeMode.None để tránh NullReferenceException
                foreach (DataGridViewColumn c in _dgvLoaiPhong.Columns)
                    c.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;

                if (_dgvLoaiPhong.Columns[nameof(DoanhThuTheoLoaiPhongDto.IdLoaiPhong)] is DataGridViewColumn colId)
                    colId.Visible = false;

                void ConfigCol(string name, string header, int width, DataGridViewContentAlignment align, string? format = null, bool isFill = false)
                {
                    if (_dgvLoaiPhong.Columns[name] is DataGridViewColumn col)
                    {
                        col.HeaderText = header;
                        col.DefaultCellStyle.Alignment = align;
                        col.HeaderCell.Style.Alignment = align;
                        if (!string.IsNullOrEmpty(format))
                            col.DefaultCellStyle.Format = format;
                        if (isFill)
                        {
                            col.MinimumWidth = width;
                            col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                        }
                        else
                        {
                            col.Width = width;
                        }
                    }
                }

                ConfigCol(nameof(DoanhThuTheoLoaiPhongDto.TenLoaiPhong), "Tên loại phòng", 160, DataGridViewContentAlignment.MiddleLeft, isFill: true);
                ConfigCol(nameof(DoanhThuTheoLoaiPhongDto.SoLuongPhong), "Số phòng hiện có", 120, DataGridViewContentAlignment.MiddleCenter);
                ConfigCol(nameof(DoanhThuTheoLoaiPhongDto.SoLuotDat), "Số lượt đặt phòng", 120, DataGridViewContentAlignment.MiddleCenter);
                ConfigCol(nameof(DoanhThuTheoLoaiPhongDto.TongSoDem), "Tổng số đêm lưu trú", 130, DataGridViewContentAlignment.MiddleCenter);
                ConfigCol(nameof(DoanhThuTheoLoaiPhongDto.DoanhThuPhong), "Doanh thu tiền phòng", 150, DataGridViewContentAlignment.MiddleRight, "#,##0 đ");
                ConfigCol(nameof(DoanhThuTheoLoaiPhongDto.TyTrongPhanTram), "Tỷ trọng (%)", 110, DataGridViewContentAlignment.MiddleCenter, "0.0'%'");

                if (_dgvLoaiPhong.Columns[nameof(DoanhThuTheoLoaiPhongDto.DoanhThuPhong)] is DataGridViewColumn colDt)
                {
                    colDt.DefaultCellStyle.Font = new Font(_dgvLoaiPhong.Font, FontStyle.Bold);
                    colDt.DefaultCellStyle.ForeColor = Color.FromArgb(13, 59, 102);
                }
            }
            finally
            {
                _dgvLoaiPhong.ResumeLayout(true);
            }
        }

        private async Task XuatExcelAsync()
        {
            if (_baoCaoHienTai == null)
            {
                MessageBox.Show("Vui lòng tải dữ liệu báo cáo trước khi xuất file Excel.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using var sfd = new SaveFileDialog
            {
                Title = "Lưu báo cáo doanh thu Excel",
                Filter = "Excel Workbook (*.xlsx)|*.xlsx",
                FileName = $"BaoCaoDoanhThu_{_baoCaoHienTai.TuNgay:yyyyMMdd}_{_baoCaoHienTai.DenNgay:yyyyMMdd}.xlsx"
            };

            if (sfd.ShowDialog(this) == DialogResult.OK)
            {
                try
                {
                    _btnXuatExcel.Enabled = false;
                    _btnXuatExcel.Text = "Đang xuất Excel...";
                    _lblTrangThai.Text = "Đang tạo tệp Excel theo định dạng chuẩn...";
                    Cursor = Cursors.WaitCursor;

                    await _baoCaoExcelService.XuatFileExcelAsync(_baoCaoHienTai, sfd.FileName);

                    _lblTrangThai.Text = $"Xuất file Excel thành công lúc {DateTime.Now:HH:mm:ss}!";

                    var result = MessageBox.Show(
                        $"Xuất file Excel thành công!\n\nĐường dẫn:\n{sfd.FileName}\n\nBạn có muốn mở file báo cáo ngay bây giờ không?",
                        "Xuất Excel thành công",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Information);

                    if (result == DialogResult.Yes)
                    {
                        Process.Start(new ProcessStartInfo(sfd.FileName) { UseShellExecute = true });
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Có lỗi xảy ra khi xuất file Excel: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    _lblTrangThai.Text = "Xuất file Excel thất bại.";
                }
                finally
                {
                    _btnXuatExcel.Enabled = true;
                    _btnXuatExcel.Text = "📊 Xuất Excel (.xlsx)";
                    Cursor = Cursors.Default;
                }
            }
        }
    }
}
