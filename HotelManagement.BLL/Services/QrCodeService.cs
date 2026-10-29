using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using HotelManagement.DAL;
using HotelManagement.DAL.Repositories;
using ZXing;
using ZXing.Common;
using ZXing.QrCode;
using ZXing.Windows.Compatibility;

namespace HotelManagement.BLL.Services
{
    /// <summary>
    /// Triển khai dịch vụ sinh và giải mã QR bằng ZXing.Net (DCKLTTQ-80).
    /// Sinh mã QR động chứa mã booking phục vụ tra cứu nhanh tại Kiosk và in trên hóa đơn PDF/phiếu xác nhận.
    /// Tái sử dụng IDatPhongRepository.GetByMaBookingAsync từ IUnitOfWork để kiểm tra tính hợp lệ dữ liệu.
    /// </summary>
    [SupportedOSPlatform("windows")]
    public class QrCodeService : IQrCodeService
    {
        private readonly IUnitOfWork _unitOfWork;

        public QrCodeService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public byte[] SinhMaQrByte(string noiDung, int kichThuoc = 300)
        {
            using var bitmap = SinhMaQrBitmap(noiDung, kichThuoc);
            using var ms = new MemoryStream();
            bitmap.Save(ms, ImageFormat.Png);
            return ms.ToArray();
        }

        public Bitmap SinhMaQrBitmap(string noiDung, int kichThuoc = 300)
        {
            if (string.IsNullOrWhiteSpace(noiDung))
                throw new ArgumentException("Nội dung tạo mã QR không được để trống.", nameof(noiDung));

            if (kichThuoc <= 0)
                kichThuoc = 300;

            var writer = new BarcodeWriter
            {
                Format = BarcodeFormat.QR_CODE,
                Options = new QrCodeEncodingOptions
                {
                    Height = kichThuoc,
                    Width = kichThuoc,
                    Margin = 1,
                    CharacterSet = "UTF-8",
                    ErrorCorrection = ZXing.QrCode.Internal.ErrorCorrectionLevel.M
                }
            };

            return writer.Write(noiDung);
        }

        public async Task<byte[]?> SinhMaQrChoBookingAsync(string maBooking, int kichThuoc = 300)
        {
            if (string.IsNullOrWhiteSpace(maBooking))
                return null;

            var datPhong = await _unitOfWork.DatPhongRepository.GetByMaBookingAsync(maBooking.Trim());
            if (datPhong == null)
                return null;

            // Chỉ chứa mã booking ngắn gọn để quét tra cứu nhanh tại Kiosk
            return SinhMaQrByte(datPhong.Mabooking, kichThuoc);
        }

        public async Task<Bitmap?> SinhMaQrChoBookingBitmapAsync(string maBooking, int kichThuoc = 300)
        {
            if (string.IsNullOrWhiteSpace(maBooking))
                return null;

            var datPhong = await _unitOfWork.DatPhongRepository.GetByMaBookingAsync(maBooking.Trim());
            if (datPhong == null)
                return null;

            return SinhMaQrBitmap(datPhong.Mabooking, kichThuoc);
        }

        public string? DocMaQrTuBitmap(Bitmap bitmap)
        {
            if (bitmap == null)
                return null;

            try
            {
                var reader = new BarcodeReader
                {
                    AutoRotate = true,
                    Options = new DecodingOptions
                    {
                        TryHarder = true,
                        PossibleFormats = new List<BarcodeFormat> { BarcodeFormat.QR_CODE }
                    }
                };

                var result = reader.Decode(bitmap);
                return result?.Text?.Trim();
            }
            catch
            {
                return null;
            }
        }

        public string? DocMaQrTuBytes(byte[] imageBytes)
        {
            if (imageBytes == null || imageBytes.Length == 0)
                return null;

            try
            {
                using var ms = new MemoryStream(imageBytes);
                using var bitmap = new Bitmap(ms);
                return DocMaQrTuBitmap(bitmap);
            }
            catch
            {
                return null;
            }
        }
    }
}
