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
using HotelManagement.Models.Entities;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace HotelManagement.Forms
{
    public partial class FormLichSuKhachHang : Form
    {
        private readonly TaiKhoan _taiKhoanDangNhap;
        private List<DatPhongLichSuDto> _danhSachLichSu = new();

        public FormLichSuKhachHang(TaiKhoan taiKhoanDangNhap)
        {
            _taiKhoanDangNhap = taiKhoanDangNhap ?? throw new ArgumentNullException(nameof(taiKhoanDangNhap));
            InitializeComponent();

            this.Load += async (_, _) => await TaiLichSuAsync();
        }

        public async Task TaiLichSuAsync()
        {
            if (!_taiKhoanDangNhap.IdKhachhang.HasValue)
            {
                lblTongSo.Text = "Tài khoản chưa liên kết thông tin khách hàng.";
                return;
            }

            try
            {
                btnLamMoi.Enabled = false;
                string? filterTrangThai = cboTrangThai.SelectedItem?.ToString();
                if (filterTrangThai == "Tất cả") filterTrangThai = null;

                using var scope = Program.ServiceProvider.CreateScope();
                var datPhongService = scope.ServiceProvider.GetRequiredService<IDatPhongService>();

                _danhSachLichSu = await datPhongService.LayLichSuKhachHangAsync(_taiKhoanDangNhap.IdKhachhang.Value, filterTrangThai);

                dgvLichSu.DataSource = null;
                dgvLichSu.DataSource = _danhSachLichSu;

                lblTongSo.Text = $"Tổng số: {_danhSachLichSu.Count} đơn đặt phòng";
                CapNhatNutHoaDon();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "[FormLichSuKhachHang] Lỗi khi tải lịch sử đặt phòng cho khách hàng ID {IdKhachHang}: {Message}",
                    _taiKhoanDangNhap.IdKhachhang, ex.Message);
                MessageBox.Show($"Không thể tải lịch sử đặt phòng: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnLamMoi.Enabled = true;
            }
        }

        private void DgvLichSu_SelectionChanged(object? sender, EventArgs e)
        {
            CapNhatNutHoaDon();
        }

        private DatPhongLichSuDto? LayDtoDangChon()
        {
            if (dgvLichSu.CurrentRow?.DataBoundItem is DatPhongLichSuDto dto)
                return dto;
            return null;
        }

        private void CapNhatNutHoaDon()
        {
            var dto = LayDtoDangChon();
            // Luôn bật nút khi có dòng được chọn để người dùng không cảm thấy nút bị liệt
            btnXemHoaDon.Enabled = (dto != null);
        }

        private async Task XuatHoaDonPdfAsync()
        {
            var dto = LayDtoDangChon();
            if (dto is null)
            {
                MessageBox.Show("Vui lòng chọn một đơn đặt phòng trong danh sách để xem hoặc tải hóa đơn.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using var sfd = new SaveFileDialog
            {
                Title = $"Xuất hóa đơn PDF cho đơn {dto.MaBooking}",
                Filter = "Tệp PDF (*.pdf)|*.pdf",
                FileName = $"HoaDon_{dto.MaBooking}.pdf",
                RestoreDirectory = true
            };

            if (sfd.ShowDialog(this) != DialogResult.OK)
                return;

            string duongDanTep = sfd.FileName;

            try
            {
                btnXemHoaDon.Enabled = false;
                this.Cursor = Cursors.WaitCursor;

                using var scope = Program.ServiceProvider.CreateScope();
                var hoaDonPdfService = scope.ServiceProvider.GetRequiredService<IHoaDonPdfService>();

                await hoaDonPdfService.XuatFilePdfTheoMaBookingAsync(dto.MaBooking, duongDanTep);

                var hoiMoFile = MessageBox.Show(
                    $"Đã xuất hóa đơn PDF thành công cho booking {dto.MaBooking}!\n\nĐường dẫn: {duongDanTep}\n\nBạn có muốn mở tệp hóa đơn xem ngay không?",
                    "Xuất hóa đơn thành công",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Information);

                if (hoiMoFile == DialogResult.Yes && File.Exists(duongDanTep))
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = duongDanTep,
                        UseShellExecute = true
                    });
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "[FormLichSuKhachHang] Lỗi khi xuất PDF hóa đơn cho mã booking {MaBooking}: {Message}",
                    dto.MaBooking, ex.Message);
                MessageBox.Show($"Có lỗi xảy ra khi tạo hóa đơn PDF: {ex.Message}", "Lỗi xuất hóa đơn",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.Cursor = Cursors.Default;
                CapNhatNutHoaDon();
            }
        }

        private void DgvLichSu_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            string colName = dgvLichSu.Columns[e.ColumnIndex].Name;

            if (colName == "TrangThaiBooking" && e.Value is string ttBooking)
            {
                switch (ttBooking)
                {
                    case "Đã xác nhận":
                        e.CellStyle!.ForeColor = AppTheme.Primary;
                        e.CellStyle.Font = AppTheme.FontBodyBold;
                        break;
                    case "Đã check-in":
                        e.CellStyle!.ForeColor = Color.FromArgb(21, 101, 192); // Blue
                        e.CellStyle.Font = AppTheme.FontBodyBold;
                        break;
                    case "Đã check-out":
                        e.CellStyle!.ForeColor = Color.FromArgb(46, 125, 50); // Dark Green
                        e.CellStyle.Font = AppTheme.FontBodyBold;
                        break;
                    case "Đã hủy":
                        e.CellStyle!.ForeColor = AppTheme.StatusDangerText;
                        e.CellStyle.Font = AppTheme.FontBody;
                        break;
                }
            }
            else if (colName == "TrangThaiThanhToan" && e.Value is string ttThanhToan)
            {
                if (ttThanhToan.Equals("Đã thanh toán", StringComparison.OrdinalIgnoreCase))
                {
                    e.CellStyle!.ForeColor = Color.FromArgb(46, 125, 50);
                    e.CellStyle.Font = AppTheme.FontBodyBold;
                }
                else
                {
                    e.CellStyle!.ForeColor = AppTheme.TextMuted;
                }
            }
        }
    }
}
