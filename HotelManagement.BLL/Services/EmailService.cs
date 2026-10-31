using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HotelManagement.BLL.DTOs;
using HotelManagement.DAL.Repositories;
using HotelManagement.Models.Entities;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;

namespace HotelManagement.BLL.Services
{
    /// <summary>
    /// Triển khai dịch vụ gửi email thông báo qua Gmail SMTP bằng MailKit (DCKLTTQ-81).
    /// Đọc thông tin kết nối và Gmail App Password từ IConfiguration (User Secrets / appsettings).
    /// Mọi luồng gửi đều có try/catch nội bộ, trả về EmailResult mà không bao giờ ném ngoại lệ làm hỏng nghiệp vụ chính.
    /// </summary>
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHoaDonService _hoaDonService;
        private readonly IHoaDonPdfService _hoaDonPdfService;

        public EmailService(
            IConfiguration configuration,
            IUnitOfWork unitOfWork,
            IHoaDonService hoaDonService,
            IHoaDonPdfService hoaDonPdfService)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _hoaDonService = hoaDonService ?? throw new ArgumentNullException(nameof(hoaDonService));
            _hoaDonPdfService = hoaDonPdfService ?? throw new ArgumentNullException(nameof(hoaDonPdfService));
        }

        #region Helper Đọc Cấu Hình

        private string LaySmtpHost() =>
            _configuration["Email:SmtpServer"]
            ?? _configuration["Smtp:Server"]
            ?? _configuration["SmtpServer"]
            ?? "smtp.gmail.com";

        private int LaySmtpPort()
        {
            string? portStr = _configuration["Email:Port"]
                ?? _configuration["Smtp:Port"]
                ?? _configuration["Port"];

            return int.TryParse(portStr, out int port) ? port : 587;
        }

        private string LaySenderEmail() =>
            _configuration["Email:SenderEmail"]
            ?? _configuration["Email:FromEmail"]
            ?? _configuration["Email:Username"]
            ?? _configuration["Smtp:Username"]
            ?? _configuration["Gmail:Email"]
            ?? _configuration["SenderEmail"]
            ?? string.Empty;

        private string LaySenderName() =>
            _configuration["Email:SenderName"]
            ?? _configuration["Email:FromName"]
            ?? "Khách sạn DCKLTTQ";

        private string LayAppPassword() =>
            _configuration["Email:AppPassword"]
            ?? _configuration["Email:Password"]
            ?? _configuration["Smtp:Password"]
            ?? _configuration["Gmail:AppPassword"]
            ?? _configuration["AppPassword"]
            ?? string.Empty;

        #endregion

        #region IEmailService Implementation

        public async Task<EmailResult> GuiEmailAsync(
            string nguoiNhan,
            string tieuDe,
            string noiDungHtml,
            byte[]? tepDinhKem = null,
            string? tenTepDinhKem = null,
            CancellationToken cancellationToken = default)
        {
            var thoiGian = DateTime.Now;

            if (string.IsNullOrWhiteSpace(nguoiNhan))
            {
                return new EmailResult(false, "Địa chỉ email người nhận trống.", nguoiNhan, tieuDe, thoiGian);
            }

            string senderEmail = LaySenderEmail();
            string senderName = LaySenderName();
            string appPassword = LayAppPassword();
            string smtpHost = LaySmtpHost();
            int smtpPort = LaySmtpPort();

            if (string.IsNullOrWhiteSpace(senderEmail))
            {
                return new EmailResult(
                    false,
                    "Chưa cấu hình địa chỉ email gửi (Email:SenderEmail) trong cấu hình hoặc User Secrets.",
                    nguoiNhan,
                    tieuDe,
                    thoiGian);
            }

            if (string.IsNullOrWhiteSpace(appPassword))
            {
                return new EmailResult(
                    false,
                    "Chưa cấu hình Gmail App Password (Email:AppPassword) trong User Secrets hoặc cấu hình.",
                    nguoiNhan,
                    tieuDe,
                    thoiGian);
            }

            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(senderName, senderEmail));
                message.To.Add(new MailboxAddress(nguoiNhan, nguoiNhan));
                message.Subject = tieuDe;

                var builder = new BodyBuilder
                {
                    HtmlBody = noiDungHtml
                };

                if (tepDinhKem != null && tepDinhKem.Length > 0 && !string.IsNullOrWhiteSpace(tenTepDinhKem))
                {
                    builder.Attachments.Add(tenTepDinhKem, tepDinhKem, new ContentType("application", "pdf"));
                }

                message.Body = builder.ToMessageBody();

                using var client = new SmtpClient();
                // Giới hạn timeout 15s để không treo UI nếu mất kết nối mạng
                client.Timeout = 15000;

                // SMTP Gmail: smtp.gmail.com cổng 587 StartTls
                await client.ConnectAsync(smtpHost, smtpPort, SecureSocketOptions.StartTls, cancellationToken);
                await client.AuthenticateAsync(senderEmail, appPassword.Replace(" ", string.Empty), cancellationToken);
                await client.SendAsync(message, cancellationToken);
                await client.DisconnectAsync(true, cancellationToken);

                return new EmailResult(true, null, nguoiNhan, tieuDe, thoiGian);
            }
            catch (Exception ex)
            {
                // Bắt toàn bộ lỗi nội bộ (SMTP lỗi, timeout, sai mật khẩu, rớt mạng)
                // Tuyệt đối không throw để không làm hỏng luồng đặt phòng hoặc thanh toán
                return new EmailResult(false, $"Lỗi khi gửi email qua MailKit: {ex.Message}", nguoiNhan, tieuDe, thoiGian);
            }
        }

        public async Task<EmailResult> GuiEmailXacNhanDatPhongAsync(
            string maBooking,
            CancellationToken cancellationToken = default)
        {
            var thoiGian = DateTime.Now;

            if (string.IsNullOrWhiteSpace(maBooking))
            {
                return new EmailResult(false, "Mã booking không được để trống.", null, null, thoiGian);
            }

            try
            {
                var dp = await _unitOfWork.DatPhongRepository.GetByMaBookingAsync(maBooking.Trim());
                if (dp == null)
                {
                    return new EmailResult(false, $"Không tìm thấy đơn đặt phòng với mã booking '{maBooking}'.", null, null, thoiGian);
                }

                var datPhong = await _unitOfWork.DatPhongRepository.GetChiTietDayDuAsync(dp.IdDatphong);
                if (datPhong == null)
                {
                    return new EmailResult(false, $"Không tải được chi tiết đặt phòng cho mã booking '{maBooking}'.", null, null, thoiGian);
                }

                string emailKhachHang = datPhong.IdKhachhangNavigation?.Email?.Trim() ?? string.Empty;
                string tenKhachHang = datPhong.IdKhachhangNavigation?.Ten?.Trim() ?? "Quý khách";

                if (string.IsNullOrWhiteSpace(emailKhachHang))
                {
                    return new EmailResult(false, $"Khách hàng '{tenKhachHang}' không có địa chỉ email trong hệ thống.", null, null, thoiGian);
                }

                string tieuDe = $"[DCKLTTQ Hotel] Xác nhận đặt phòng thành công - Mã booking: {datPhong.Mabooking}";
                string htmlBody = TaoHtmlXacNhanDatPhong(datPhong);

                return await GuiEmailAsync(emailKhachHang, tieuDe, htmlBody, null, null, cancellationToken);
            }
            catch (Exception ex)
            {
                return new EmailResult(false, $"Lỗi khi xử lý email đặt phòng: {ex.Message}", null, null, thoiGian);
            }
        }

        public async Task<EmailResult> GuiEmailHoaDonAsync(
            string maBooking,
            CancellationToken cancellationToken = default)
        {
            var thoiGian = DateTime.Now;

            if (string.IsNullOrWhiteSpace(maBooking))
            {
                return new EmailResult(false, "Mã booking không được để trống.", null, null, thoiGian);
            }

            try
            {
                var hoaDon = await _hoaDonService.LayHoaDonTheoMaBookingAsync(maBooking.Trim());
                if (hoaDon == null)
                {
                    return new EmailResult(false, $"Không tìm thấy thông tin hóa đơn cho mã booking '{maBooking}'.", null, null, thoiGian);
                }

                string emailKhachHang = hoaDon.EmailKhachHang?.Trim() ?? string.Empty;
                if (string.IsNullOrWhiteSpace(emailKhachHang))
                {
                    return new EmailResult(false, $"Khách hàng '{hoaDon.TenKhachHang}' không có địa chỉ email trong hệ thống.", null, null, thoiGian);
                }

                // Sinh tệp PDF hóa đơn từ QuestPDF (DCKLTTQ-77)
                byte[] pdfBytes = await _hoaDonPdfService.SinhPdfTheoMaBookingAsync(maBooking.Trim());
                string tenTepPdf = $"HoaDon_{maBooking.Trim()}.pdf";

                string tieuDe = $"[DCKLTTQ Hotel] Hóa đơn thanh toán - Mã booking: {maBooking.Trim()}";
                string htmlBody = TaoHtmlHoaDonThanhToan(hoaDon);

                return await GuiEmailAsync(emailKhachHang, tieuDe, htmlBody, pdfBytes, tenTepPdf, cancellationToken);
            }
            catch (Exception ex)
            {
                return new EmailResult(false, $"Lỗi khi xử lý email hóa đơn: {ex.Message}", null, null, thoiGian);
            }
        }

        #endregion

        #region Mẫu Giao Diện Email HTML

        private string TaoHtmlXacNhanDatPhong(DatPhong datPhong)
        {
            string tenKhach = datPhong.IdKhachhangNavigation?.Ten ?? "Quý khách";
            string sdtKhach = datPhong.IdKhachhangNavigation?.Sdt ?? "-";
            string cccdKhach = datPhong.IdKhachhangNavigation?.Cccd ?? "-";

            var sbPhongs = new StringBuilder();
            if (datPhong.ChiTietDatPhongs != null && datPhong.ChiTietDatPhongs.Any())
            {
                foreach (var ct in datPhong.ChiTietDatPhongs)
                {
                    string soPhong = ct.IdPhongNavigation?.Sophong ?? "-";
                    string loaiPhong = ct.IdPhongNavigation?.IdLoaiphongNavigation?.Ten ?? "-";
                    decimal donGia = ct.Dongia;

                    sbPhongs.Append($@"
                    <tr style='border-bottom: 1px solid #e2e8f0;'>
                        <td style='padding: 10px 12px; font-weight: 600; color: #1e293b;'>Phòng {soPhong}</td>
                        <td style='padding: 10px 12px; color: #475569;'>{loaiPhong}</td>
                        <td style='padding: 10px 12px; text-align: right; color: #0d9488; font-weight: 600;'>{donGia:N0} đ/đêm</td>
                    </tr>");
                }
            }
            else
            {
                sbPhongs.Append("<tr><td colspan='3' style='padding: 12px; text-align: center; color: #64748b;'>(Chưa phân phòng cụ thể)</td></tr>");
            }

            return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <style>
        body {{ font-family: 'Segoe UI', Arial, sans-serif; background-color: #f1f5f9; margin: 0; padding: 24px; color: #334155; }}
        .card {{ max-width: 620px; margin: 0 auto; background: #ffffff; border-radius: 10px; overflow: hidden; box-shadow: 0 4px 6px -1px rgba(0,0,0,0.1); }}
        .header {{ background: #003366; color: #ffffff; padding: 28px 24px; text-align: center; }}
        .header h1 {{ margin: 0; font-size: 22px; letter-spacing: 0.5px; text-transform: uppercase; }}
        .header p {{ margin: 6px 0 0 0; font-size: 13px; opacity: 0.85; }}
        .body {{ padding: 28px 24px; }}
        .greeting {{ font-size: 15px; margin-bottom: 20px; line-height: 1.5; }}
        .badge-box {{ background: #f8fafc; border: 2px dashed #0d9488; border-radius: 8px; padding: 16px; text-align: center; margin: 20px 0; }}
        .badge-title {{ font-size: 12px; text-transform: uppercase; letter-spacing: 1px; color: #64748b; font-weight: 600; margin-bottom: 6px; }}
        .booking-code {{ font-size: 26px; font-weight: bold; color: #003366; letter-spacing: 2px; }}
        .info-table {{ width: 100%; border-collapse: collapse; margin-top: 18px; }}
        .info-table td {{ padding: 8px 0; font-size: 14px; vertical-align: top; }}
        .info-label {{ width: 38%; color: #64748b; font-weight: 500; }}
        .info-value {{ width: 62%; color: #0f172a; font-weight: 600; }}
        .rooms-table {{ width: 100%; border-collapse: collapse; margin-top: 14px; font-size: 13px; }}
        .rooms-table th {{ background: #f8fafc; padding: 10px 12px; text-align: left; font-size: 12px; text-transform: uppercase; color: #475569; border-bottom: 2px solid #cbd5e1; }}
        .footer {{ background: #f8fafc; padding: 20px 24px; text-align: center; font-size: 12px; color: #64748b; border-top: 1px solid #e2e8f0; }}
    </style>
</head>
<body>
    <div class='card'>
        <div class='header'>
            <h1>Khách sạn DCKLTTQ</h1>
            <p>XÁC NHẬN ĐẶT PHÒNG TRỰC TUYẾN / LỄ TÂN</p>
        </div>
        <div class='body'>
            <div class='greeting'>
                Kính gửi <strong>{tenKhach}</strong>,<br>
                Đơn đặt phòng của quý khách đã được tiếp nhận và xác nhận thành công trên hệ thống.
            </div>

            <div class='badge-box'>
                <div class='badge-title'>MÃ BOOKING CỦA BẠN</div>
                <div class='booking-code'>{datPhong.Mabooking}</div>
                <div style='font-size: 12px; color: #64748b; margin-top: 6px;'>
                    Quý khách có thể sử dụng mã này để làm thủ tục nhận phòng hoặc tra cứu tại màn hình Kiosk.
                </div>
            </div>

            <h3 style='color: #003366; margin: 24px 0 10px 0; font-size: 15px; border-bottom: 2px solid #003366; padding-bottom: 6px;'>
                THÔNG TIN CHI TIẾT
            </h3>

            <table class='info-table'>
                <tr>
                    <td class='info-label'>Khách hàng:</td>
                    <td class='info-value'>{tenKhach}</td>
                </tr>
                <tr>
                    <td class='info-label'>Số điện thoại:</td>
                    <td class='info-value'>{sdtKhach}</td>
                </tr>
                <tr>
                    <td class='info-label'>Số CCCD:</td>
                    <td class='info-value'>{cccdKhach}</td>
                </tr>
                <tr>
                    <td class='info-label'>Thời gian lưu trú:</td>
                    <td class='info-value'>{datPhong.NgaynhanDukien:dd/MM/yyyy} &rarr; {datPhong.NgaytraDukien:dd/MM/yyyy}</td>
                </tr>
                <tr>
                    <td class='info-label'>Số lượng khách:</td>
                    <td class='info-value'>{datPhong.Songuoi} người</td>
                </tr>
                <tr>
                    <td class='info-label'>Trạng thái:</td>
                    <td class='info-value' style='color: #0d9488;'>{datPhong.Trangthai}</td>
                </tr>
            </table>

            <h3 style='color: #003366; margin: 24px 0 10px 0; font-size: 15px; border-bottom: 2px solid #003366; padding-bottom: 6px;'>
                DANH SÁCH PHÒNG
            </h3>

            <table class='rooms-table'>
                <thead>
                    <tr>
                        <th>Số phòng</th>
                        <th>Loại phòng</th>
                        <th style='text-align: right;'>Đơn giá</th>
                    </tr>
                </thead>
                <tbody>
                    {sbPhongs}
                </tbody>
            </table>
        </div>
        <div class='footer'>
            Khách sạn DCKLTTQ &bull; Hotline: 1900-1234 &bull; Email: contact@hotelmanagement.vn<br>
            Cảm ơn quý khách đã tin tưởng và lựa chọn dịch vụ của chúng tôi!
        </div>
    </div>
</body>
</html>";
        }

        private string TaoHtmlHoaDonThanhToan(HoaDonDto hoaDon)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <style>
        body {{ font-family: 'Segoe UI', Arial, sans-serif; background-color: #f1f5f9; margin: 0; padding: 24px; color: #334155; }}
        .card {{ max-width: 620px; margin: 0 auto; background: #ffffff; border-radius: 10px; overflow: hidden; box-shadow: 0 4px 6px -1px rgba(0,0,0,0.1); }}
        .header {{ background: #003366; color: #ffffff; padding: 28px 24px; text-align: center; }}
        .header h1 {{ margin: 0; font-size: 22px; letter-spacing: 0.5px; text-transform: uppercase; }}
        .header p {{ margin: 6px 0 0 0; font-size: 13px; opacity: 0.85; }}
        .body {{ padding: 28px 24px; }}
        .greeting {{ font-size: 15px; margin-bottom: 20px; line-height: 1.5; }}
        .info-table {{ width: 100%; border-collapse: collapse; margin-top: 14px; }}
        .info-table td {{ padding: 8px 0; font-size: 14px; vertical-align: top; }}
        .info-label {{ width: 42%; color: #64748b; font-weight: 500; }}
        .info-value {{ width: 58%; color: #0f172a; font-weight: 600; }}
        .amount-card {{ background: #f0fdf4; border: 1px solid #86efac; border-radius: 8px; padding: 18px; margin: 20px 0; }}
        .amount-row {{ display: flex; justify-content: space-between; margin-bottom: 6px; font-size: 13px; color: #475569; }}
        .total-row {{ display: flex; justify-content: space-between; font-size: 18px; font-weight: bold; color: #15803d; border-top: 1px dashed #86efac; padding-top: 10px; margin-top: 8px; }}
        .attachment-note {{ background: #eff6ff; border-left: 4px solid #3b82f6; padding: 12px 16px; border-radius: 4px; font-size: 13px; color: #1e40af; margin-top: 20px; }}
        .footer {{ background: #f8fafc; padding: 20px 24px; text-align: center; font-size: 12px; color: #64748b; border-top: 1px solid #e2e8f0; }}
    </style>
</head>
<body>
    <div class='card'>
        <div class='header'>
            <h1>Khách sạn DCKLTTQ</h1>
            <p>HÓA ĐƠN THANH TOÁN DỊCH VỤ</p>
        </div>
        <div class='body'>
            <div class='greeting'>
                Kính gửi <strong>{hoaDon.TenKhachHang}</strong>,<br>
                Hóa đơn dịch vụ cho đợt lưu trú của quý khách đã được thanh toán thành công.
            </div>

            <table class='info-table'>
                <tr>
                    <td class='info-label'>Mã booking:</td>
                    <td class='info-value' style='color: #003366; font-size: 16px;'>{hoaDon.MaBooking}</td>
                </tr>
                <tr>
                    <td class='info-label'>Ngày lập hóa đơn:</td>
                    <td class='info-value'>{hoaDon.NgayLap:dd/MM/yyyy HH:mm}</td>
                </tr>
                <tr>
                    <td class='info-label'>Hình thức thanh toán:</td>
                    <td class='info-value'>{hoaDon.PhuongThucThanhtoan ?? "Tiền mặt"}</td>
                </tr>
                <tr>
                    <td class='info-label'>Trạng thái thanh toán:</td>
                    <td class='info-value' style='color: #16a34a;'>{hoaDon.TrangThaiThanhtoan}</td>
                </tr>
                <tr>
                    <td class='info-label'>Số đêm lưu trú:</td>
                    <td class='info-value'>{hoaDon.SoDemLuuTru} đêm</td>
                </tr>
            </table>

            <div class='amount-card'>
                <table style='width: 100%; font-size: 14px;'>
                    <tr>
                        <td style='color: #475569; padding: 4px 0;'>Tiền phòng:</td>
                        <td style='text-align: right; font-weight: 600; color: #1e293b;'>{hoaDon.TienPhong:N0} đ</td>
                    </tr>
                    <tr>
                        <td style='color: #475569; padding: 4px 0;'>Tiền dịch vụ phát sinh:</td>
                        <td style='text-align: right; font-weight: 600; color: #1e293b;'>{hoaDon.TienDichVu:N0} đ</td>
                    </tr>
                    <tr>
                        <td style='color: #475569; padding: 4px 0;'>Chiết khấu / Giảm giá:</td>
                        <td style='text-align: right; font-weight: 600; color: #dc2626;'>-{hoaDon.GiamGia:N0} đ</td>
                    </tr>
                    <tr style='border-top: 1px dashed #86efac;'>
                        <td style='font-size: 16px; font-weight: bold; color: #15803d; padding-top: 10px;'>TỔNG THANH TOÁN:</td>
                        <td style='font-size: 18px; font-weight: bold; color: #15803d; text-align: right; padding-top: 10px;'>{hoaDon.TongGia:N0} đ</td>
                    </tr>
                </table>
            </div>

            <div class='attachment-note'>
                &#128206; <strong>Tệp đính kèm:</strong> Tệp hóa đơn điện tử định dạng PDF chi tiết (<strong>HoaDon_{hoaDon.MaBooking}.pdf</strong>) đã được đính kèm theo thư này để quý khách lưu trữ và tra cứu.
            </div>
        </div>
        <div class='footer'>
            Khách sạn DCKLTTQ &bull; Hotline: 1900-1234 &bull; Email: contact@hotelmanagement.vn<br>
            Hân hạnh được phục vụ quý khách và hẹn gặp lại trong những kỳ nghỉ tiếp theo!
        </div>
    </div>
</body>
</html>";
        }

        #endregion
    }
}

