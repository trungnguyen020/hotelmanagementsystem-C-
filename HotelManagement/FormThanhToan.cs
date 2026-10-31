using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using HotelManagement.BLL.DTOs;
using HotelManagement.BLL.Services;
using HotelManagement.DAL.Repositories;
using HotelManagement.Models.Entities;
using Microsoft.Extensions.DependencyInjection;

namespace HotelManagement.Forms
{
    /// <summary>
    /// Màn hình Thanh toán hóa đơn (DCKLTTQ-76).
    /// Hỗ trợ chọn hình thức thanh toán: Tiền mặt hoặc Chuyển khoản (hiển thị mã QR ngân hàng tĩnh).
    /// Tái sử dụng IHoaDonService để cập nhật trạng thái thanh toán trên hóa đơn.
    /// </summary>
    public partial class FormThanhToan : Form
    {
        private string? _maBookingKhoiTao;
        private readonly IServiceScope _scope;
        private readonly IHoaDonService _hoaDonService;
        private readonly IHoaDonPdfService _hoaDonPdfService;
        private readonly IEmailService _emailService;
        private readonly IUnitOfWork _unitOfWork;

        private HoaDonDto? _hoaDonHienTai;
        private decimal _tongTienSauGiamGia = 0;

        public FormThanhToan(string? maBooking = null)
        {
            _maBookingKhoiTao = maBooking;
            _scope = Program.ServiceProvider.CreateScope();
            _hoaDonService = _scope.ServiceProvider.GetRequiredService<IHoaDonService>();
            _hoaDonPdfService = _scope.ServiceProvider.GetRequiredService<IHoaDonPdfService>();
            _emailService = _scope.ServiceProvider.GetRequiredService<IEmailService>();
            _unitOfWork = _scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            InitializeComponent();

            FormClosed += (_, _) => _scope.Dispose();
            Load += async (_, _) =>
            {
                TaiAnhQrTinh();

                if (!string.IsNullOrWhiteSpace(_maBookingKhoiTao))
                {
                    _txtMaBookingTraCuu.Text = _maBookingKhoiTao.Trim();
                    await TaiDuLieuHoaDonAsync(_maBookingKhoiTao.Trim());
                }
            };
        }

        private void TaiAnhQrTinh()
        {
            try
            {
                // Kiểm tra file ảnh QR tĩnh đặt tại thư mục Resources của ứng dụng
                string thuMucResources = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources");
                string duongDanAnh = Path.Combine(thuMucResources, "qr_chuyenkhoan.png");

                if (File.Exists(duongDanAnh))
                {
                    _picQr.Image = Image.FromFile(duongDanAnh);
                }
                else
                {
                    // Nếu chưa có file ảnh thực tế trên ổ đĩa, tự động vẽ khung QR tĩnh mô phỏng phục vụ demo
                    _picQr.Image = TaoAnhQrMoPhong();
                }
            }
            catch
            {
                _picQr.Image = TaoAnhQrMoPhong();
            }
        }

        private static Bitmap TaoAnhQrMoPhong()
        {
            var bmp = new Bitmap(165, 165);
            using var g = Graphics.FromImage(bmp);
            g.Clear(Color.White);

            using var pen = new Pen(Color.FromArgb(0, 51, 102), 4);
            using var brush = new SolidBrush(Color.FromArgb(0, 51, 102));

            // Vẽ viền ngoài
            g.DrawRectangle(pen, 6, 6, 153, 153);

            // 3 mắt định vị chuẩn của QR Code
            void VeMatDinhVi(int x, int y)
            {
                g.FillRectangle(brush, x, y, 32, 32);
                g.FillRectangle(Brushes.White, x + 6, y + 6, 20, 20);
                g.FillRectangle(brush, x + 10, y + 10, 12, 12);
            }

            VeMatDinhVi(16, 16);
            VeMatDinhVi(117, 16);
            VeMatDinhVi(16, 117);

            // Vẽ các khối pixel mô phỏng QR
            var rnd = new Random(12345);
            for (int r = 0; r < 9; r++)
            {
                for (int c = 0; c < 9; c++)
                {
                    int px = 55 + c * 6;
                    int py = 55 + r * 6;
                    if (rnd.Next(2) == 1)
                    {
                        g.FillRectangle(brush, px, py, 5, 5);
                    }
                }
            }

            // Dòng chữ VietQR ở giữa dưới
            using var font = new Font("Arial", 8f, FontStyle.Bold);
            using var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            g.DrawString("VietQR — VCB", font, brush, new RectangleF(0, 140, 165, 20), sf);

            return bmp;
        }

        private async Task TaiDuLieuHoaDonAsync(string maBooking)
        {
            if (string.IsNullOrWhiteSpace(maBooking))
            {
                MessageBox.Show("Vui lòng nhập mã booking cần thanh toán.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                _txtMaBookingTraCuu.Focus();
                return;
            }

            try
            {
                _btnTraCuu.Enabled = false;
                _btnTraCuu.Text = "Đang tìm...";

                var dto = await _hoaDonService.LayHoaDonTheoMaBookingAsync(maBooking.Trim());
                if (dto == null)
                {
                    MessageBox.Show($"Không tìm thấy thông tin đặt phòng cho mã booking '{maBooking}'. Vui lòng kiểm tra lại.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    XoaTrangThongTin();
                    return;
                }

                _hoaDonHienTai = dto;

                _lblMaBooking.Text = dto.MaBooking;
                _lblTrangThaiHienTai.Text = dto.TrangThaiThanhtoan;
                _lblTrangThaiHienTai.ForeColor = dto.TrangThaiThanhtoan == "Đã thanh toán" ? Color.DarkGreen : Color.Crimson;

                _lblKhachHang.Text = dto.TenKhachHang;
                _lblSdt.Text = dto.SdtKhachHang;

                string thoiGianNhan = dto.NgayNhanThucTe?.ToString("dd/MM/yyyy HH:mm") ?? dto.NgayNhanDuKien.ToString("dd/MM/yyyy");
                string thoiGianTra = dto.NgayTraThucTe?.ToString("dd/MM/yyyy HH:mm") ?? dto.NgayTraDuKien.ToString("dd/MM/yyyy");
                _lblThoiGianLuuTru.Text = $"{thoiGianNhan} → {thoiGianTra} ({dto.SoDemLuuTru} đêm)";

                _lblTienPhongVal.Text = $"{dto.TienPhong:N0} đ";
                _lblTienDichVuVal.Text = $"{dto.TienDichVu:N0} đ";

                _numGiamGia.Maximum = dto.TienPhong + dto.TienDichVu;
                _numGiamGia.Value = dto.GiamGia;

                _lblNoiDungChuyenKhoanVal.Text = dto.MaBooking;

                CapNhatTongTien();

                if (dto.TrangThaiThanhtoan == "Đã thanh toán")
                {
                    _btnXacNhan.Enabled = false;
                    _btnXacNhan.Text = "Hóa đơn đã thanh toán";
                    _btnXacNhan.BackColor = Color.Gray;
                    _numGiamGia.Enabled = false;
                    _pnlTienMat.Enabled = false;
                    _pnlChuyenKhoan.Enabled = false;
                }
                else
                {
                    _btnXacNhan.Enabled = true;
                    _btnXacNhan.Text = "Xác nhận thanh toán";
                    _btnXacNhan.BackColor = Color.FromArgb(39, 174, 96);
                    _numGiamGia.Enabled = true;
                    _pnlTienMat.Enabled = true;
                    _pnlChuyenKhoan.Enabled = true;
                }

                _btnXuatPdf.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải thông tin hóa đơn: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _btnTraCuu.Enabled = true;
                _btnTraCuu.Text = "Tra cứu";
            }
        }

        private void CapNhatTongTien()
        {
            if (_hoaDonHienTai == null) return;

            decimal tongGoc = _hoaDonHienTai.TienPhong + _hoaDonHienTai.TienDichVu;
            decimal giamGia = _numGiamGia.Value;

            _tongTienSauGiamGia = Math.Max(0, tongGoc - giamGia);
            _lblTongCongVal.Text = $"{_tongTienSauGiamGia:N0} đ";
            _lblSoTienChuyenKhoanVal.Text = $"{_tongTienSauGiamGia:N0} đ";

            if (_rdoTienMat.Checked && _numTienKhachDua.Value < _tongTienSauGiamGia)
            {
                _numTienKhachDua.Value = _tongTienSauGiamGia;
            }

            TinhTienThua();
        }

        private void HinhThucThanhToanChanged()
        {
            _pnlTienMat.Visible = _rdoTienMat.Checked;
            _pnlChuyenKhoan.Visible = _rdoChuyenKhoan.Checked;

            if (_rdoTienMat.Checked)
            {
                TinhTienThua();
            }
        }

        private void TinhTienThua()
        {
            decimal tienKhach = _numTienKhachDua.Value;
            decimal tienThua = Math.Max(0, tienKhach - _tongTienSauGiamGia);
            _lblTienThuaVal.Text = $"{tienThua:N0} đ";

            if (tienKhach < _tongTienSauGiamGia)
            {
                _lblTienThuaVal.Text = $"Còn thiếu {(_tongTienSauGiamGia - tienKhach):N0} đ";
                _lblTienThuaVal.ForeColor = Color.Crimson;
            }
            else
            {
                _lblTienThuaVal.ForeColor = Color.DarkGreen;
            }
        }

        private async Task XacNhanThanhToanAsync()
        {
            if (_hoaDonHienTai == null)
            {
                MessageBox.Show("Vui lòng tra cứu thông tin hóa đơn hợp lệ trước khi xác nhận thanh toán.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string phuongThuc = _rdoTienMat.Checked ? "Tiền mặt" : "Chuyển khoản";

            // Kiểm tra theo từng hình thức
            if (_rdoTienMat.Checked)
            {
                if (_numTienKhachDua.Value < _tongTienSauGiamGia)
                {
                    var chon = MessageBox.Show(
                        "Số tiền khách đưa chưa đủ với tổng tiền hóa đơn. Bạn có chắc chắn muốn tiếp tục xác nhận thanh toán?",
                        "Cảnh báo số tiền",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning);

                    if (chon != DialogResult.Yes)
                        return;
                }
            }
            else
            {
                var xacNhanNganHang = MessageBox.Show(
                    $"Xác nhận lễ tân đã kiểm tra tài khoản ngân hàng và nhận đủ số tiền {_tongTienSauGiamGia:N0} đ từ khách hàng qua chuyển khoản (Nội dung: {_hoaDonHienTai.MaBooking})?",
                    "Xác nhận đã nhận chuyển khoản",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (xacNhanNganHang != DialogResult.Yes)
                    return;
            }

            var xacNhan = MessageBox.Show(
                $"Xác nhận hoàn tất thanh toán cho booking [{_hoaDonHienTai.MaBooking}]?\n\n" +
                $"- Khách hàng: {_hoaDonHienTai.TenKhachHang}\n" +
                $"- Hình thức: {phuongThuc}\n" +
                $"- Số tiền thanh toán: {_tongTienSauGiamGia:N0} đ",
                "Xác nhận thanh toán",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (xacNhan != DialogResult.Yes) return;

            try
            {
                _btnXacNhan.Enabled = false;
                _btnXacNhan.Text = "Đang xử lý...";

                // 1. Tính toán và lưu hóa đơn (gồm cả giảm giá nếu có)
                var (successTinh, msgTinh, dataTinh) = await _hoaDonService.TinhVaLuuHoaDonAsync(_hoaDonHienTai.MaBooking, _numGiamGia.Value);
                if (!successTinh)
                {
                    MessageBox.Show(msgTinh, "Lỗi tính hóa đơn", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (dataTinh != null)
                {
                    _hoaDonHienTai = dataTinh;
                }

                // 2. Cập nhật phương thức ("Tiền mặt" hoặc "Chuyển khoản") và trạng thái "Đã thanh toán"
                var (successTT, msgTT) = await _hoaDonService.CapNhatThanhToanAsync(_hoaDonHienTai.MaBooking, phuongThuc, "Đã thanh toán");
                if (!successTT)
                {
                    MessageBox.Show(msgTT, "Lỗi cập nhật thanh toán", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // 3. Luồng 2 (DCKLTTQ-81): Gửi email kèm hóa đơn PDF đính kèm sau khi thanh toán xong
                _btnXacNhan.Text = "Đang gửi email...";
                var emailResult = await _emailService.GuiEmailHoaDonAsync(_hoaDonHienTai.MaBooking);

                // DCKLTTQ-82: Ghi nhận lịch sử gửi thông báo
                await GhiLichSuThongBaoAsync(emailResult, "HoaDon", _hoaDonHienTai.IdDatPhong, _hoaDonHienTai.IdHoaDon);

                string thongBaoEmail = emailResult.ThanhCong
                    ? $"\n- Email hóa đơn PDF đã được gửi đến: {emailResult.NguoiNhan}"
                    : $"\n- (Lưu ý: Không thể gửi email hóa đơn: {emailResult.LyDoLoi})";

                var dialogResult = MessageBox.Show(
                    $"Thanh toán thành công cho booking '{_hoaDonHienTai.MaBooking}'!\n" +
                    $"Hình thức: {phuongThuc}\n" +
                    $"Hóa đơn đã được lưu trữ và cập nhật trạng thái 'Đã thanh toán'.{thongBaoEmail}\n\n" +
                    "Bạn có muốn xuất hóa đơn PDF ngay bây giờ không?",
                    "Thanh toán thành công",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Information);

                if (dialogResult == DialogResult.Yes)
                {
                    await XuatHoaDonPdfAsync();
                }

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Có lỗi xảy ra trong quá trình thanh toán: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _btnXacNhan.Enabled = true;
                _btnXacNhan.Text = "Xác nhận thanh toán";
            }
        }

        private void XoaTrangThongTin()
        {
            _hoaDonHienTai = null;
            _lblMaBooking.Text = "-";
            _lblTrangThaiHienTai.Text = "-";
            _lblTrangThaiHienTai.ForeColor = Color.Black;
            _lblKhachHang.Text = "-";
            _lblSdt.Text = "-";
            _lblThoiGianLuuTru.Text = "-";
            _lblTienPhongVal.Text = "0 đ";
            _lblTienDichVuVal.Text = "0 đ";
            _numGiamGia.Value = 0;
            _lblTongCongVal.Text = "0 đ";
            _lblSoTienChuyenKhoanVal.Text = "0 đ";
            _lblNoiDungChuyenKhoanVal.Text = "BK...";
            _numTienKhachDua.Value = 0;
            _lblTienThuaVal.Text = "0 đ";
            _btnXacNhan.Enabled = false;
            _btnXuatPdf.Enabled = false;
        }

        private async Task XuatHoaDonPdfAsync()
        {
            if (_hoaDonHienTai == null)
            {
                MessageBox.Show("Vui lòng tra cứu thông tin hóa đơn hợp lệ trước khi xuất PDF.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using var sfd = new SaveFileDialog
                {
                    Title = "Lưu hóa đơn thanh toán PDF",
                    Filter = "Tệp PDF (*.pdf)|*.pdf",
                    FileName = $"HoaDon_{_hoaDonHienTai.MaBooking}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf"
                };

                if (sfd.ShowDialog(this) == DialogResult.OK)
                {
                    _btnXuatPdf.Enabled = false;
                    _btnXuatPdf.Text = "Đang xuất PDF...";

                    // Nạp lại dữ liệu hóa đơn mới nhất từ DB để đảm bảo tính nhất quán
                    var hoaDonMoiNhat = await _hoaDonService.LayHoaDonTheoMaBookingAsync(_hoaDonHienTai.MaBooking) ?? _hoaDonHienTai;

                    await _hoaDonPdfService.XuatFilePdfAsync(hoaDonMoiNhat, sfd.FileName);

                    var xemNgay = MessageBox.Show(
                        $"Xuất hóa đơn PDF thành công!\n\nĐường dẫn: {sfd.FileName}\n\nBạn có muốn mở xem hóa đơn ngay bây giờ không?",
                        "Xuất PDF thành công",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);

                    if (xemNgay == DialogResult.Yes)
                    {
                        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                        {
                            FileName = sfd.FileName,
                            UseShellExecute = true
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Có lỗi xảy ra khi xuất hóa đơn PDF: " + ex.Message, "Lỗi xuất PDF", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _btnXuatPdf.Enabled = _hoaDonHienTai != null;
                _btnXuatPdf.Text = "📄 Xuất hóa đơn PDF";
            }
        }

        private async Task GhiLichSuThongBaoAsync(EmailResult result, string loaiThongBao, int? idDatPhong, int? idHoaDon)
        {
            try
            {
                var lichSu = new LichSuGuiThongBao
                {
                    IdDatphong = idDatPhong,
                    IdHoadon = idHoaDon,
                    LoaiThongbao = loaiThongBao,
                    NguoiNhan = result.NguoiNhan ?? string.Empty,
                    TieuDe = result.TieuDe,
                    TrangThai = result.ThanhCong ? "Thành công" : "Thất bại",
                    LyDoLoi = result.LyDoLoi,
                    ThoiGianGui = result.ThoiGianGui ?? DateTime.Now
                };

                await _unitOfWork.LichSuGuiThongBaoRepository.AddAsync(lichSu);
                await _unitOfWork.SaveChangesAsync();
            }
            catch
            {
                // DCKLTTQ-82: Ghi log không được phép làm gián đoạn luồng thanh toán nếu xảy ra lỗi
            }
        }
    }
}
