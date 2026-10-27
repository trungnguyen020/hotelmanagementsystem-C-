using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ClosedXML.Excel;
using HotelManagement.BLL.DTOs;
using HotelManagement.DAL.Repositories;
using HotelManagement.Models.Entities;

namespace HotelManagement.BLL.Services
{
    /// <summary>
    /// Triển khai dịch vụ Import phòng từ file Excel bằng ClosedXML (DCKLTTQ-79).
    /// Tuân thủ quy trình 3 bước: Đọc file -> Validate toàn bộ danh sách (gom lỗi theo dòng) -> Lưu trong 1 transaction.
    /// </summary>
    public class PhongImportService : IPhongImportService
    {
        private readonly IUnitOfWork _unitOfWork;

        private static readonly XLColor PrimaryNavy = XLColor.FromHtml("#0D3B66");
        private static readonly XLColor SecondaryBlue = XLColor.FromHtml("#1E5F8A");
        private static readonly XLColor TableAlternateRow = XLColor.FromHtml("#F8FAFC");
        private static readonly XLColor BorderGray = XLColor.FromHtml("#CBD5E1");

        public PhongImportService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<PhongImportDto>> DocVaValidateAsync(string duongDanFile)
        {
            if (string.IsNullOrWhiteSpace(duongDanFile))
                throw new ArgumentException("Đường dẫn file Excel không được để trống.", nameof(duongDanFile));

            if (!File.Exists(duongDanFile))
                throw new FileNotFoundException($"Không tìm thấy file Excel tại đường dẫn: {duongDanFile}");

            using var stream = new FileStream(duongDanFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            return await DocVaValidateAsync(stream);
        }

        public async Task<List<PhongImportDto>> DocVaValidateAsync(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream), "Dữ liệu file không được để trống.");

            var danhSachTho = new List<PhongImportDto>();

            using (var workbook = new XLWorkbook(stream))
            {
                var worksheet = workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                    throw new InvalidOperationException("File Excel không chứa bất kỳ trang tính (worksheet) nào.");

                // 1. Xác định dòng tiêu đề (Header row) và vị trí các cột
                int headerRowIndex = 1;
                int colSoPhong = -1;
                int colLoaiPhong = -1;
                int colTrangThai = -1;

                // Quét tối đa 10 dòng đầu để tìm dòng chứa tiêu đề
                int maxScanRow = Math.Min(10, worksheet.LastRowUsed()?.RowNumber() ?? 1);
                for (int r = 1; r <= maxScanRow; r++)
                {
                    var row = worksheet.Row(r);
                    if (row.IsEmpty()) continue;

                    int cSoPhong = TimCotTheoTuKhoa(row, "số phòng", "so phong", "phòng", "room");
                    int cLoaiPhong = TimCotTheoTuKhoa(row, "loại phòng", "loai phong", "hạng phòng", "room type");
                    int cTrangThai = TimCotTheoTuKhoa(row, "trạng thái", "trang thai", "tình trạng", "status");

                    if (cSoPhong > 0 || cLoaiPhong > 0 || cTrangThai > 0)
                    {
                        headerRowIndex = r;
                        colSoPhong = cSoPhong;
                        colLoaiPhong = cLoaiPhong;
                        colTrangThai = cTrangThai;
                        break;
                    }
                }

                // Nếu không tự động nhận diện được tên cột qua từ khóa, dùng thứ tự mặc định:
                // Cột 1: STT hoặc Số phòng; nếu có cột STT thì Cột 2: Số phòng, Cột 3: Loại phòng, Cột 4: Trạng thái
                if (colSoPhong <= 0 || colLoaiPhong <= 0 || colTrangThai <= 0)
                {
                    int lastColUsed = worksheet.LastColumnUsed()?.ColumnNumber() ?? 3;
                    if (lastColUsed >= 4)
                    {
                        colSoPhong = colSoPhong > 0 ? colSoPhong : 2;
                        colLoaiPhong = colLoaiPhong > 0 ? colLoaiPhong : 3;
                        colTrangThai = colTrangThai > 0 ? colTrangThai : 4;
                    }
                    else
                    {
                        colSoPhong = colSoPhong > 0 ? colSoPhong : 1;
                        colLoaiPhong = colLoaiPhong > 0 ? colLoaiPhong : 2;
                        colTrangThai = colTrangThai > 0 ? colTrangThai : 3;
                    }
                }

                // 2. Đọc từng dòng dữ liệu từ dòng kế tiếp của header
                int lastRowNumber = worksheet.LastRowUsed()?.RowNumber() ?? 0;
                for (int r = headerRowIndex + 1; r <= lastRowNumber; r++)
                {
                    var row = worksheet.Row(r);
                    if (row.IsEmpty()) continue;

                    string soPhongVal = LayGiaTriO(row.Cell(colSoPhong));
                    string loaiPhongVal = LayGiaTriO(row.Cell(colLoaiPhong));
                    string trangThaiVal = LayGiaTriO(row.Cell(colTrangThai));

                    // Nếu cả 3 ô chính đều rỗng thì bỏ qua dòng này (tránh dòng trắng ở cuối file)
                    if (string.IsNullOrWhiteSpace(soPhongVal) &&
                        string.IsNullOrWhiteSpace(loaiPhongVal) &&
                        string.IsNullOrWhiteSpace(trangThaiVal))
                    {
                        continue;
                    }

                    danhSachTho.Add(new PhongImportDto
                    {
                        DongThuTu = r,
                        SoPhong = soPhongVal,
                        TenLoaiPhong = loaiPhongVal,
                        TrangThai = trangThaiVal
                    });
                }
            }

            // 3. Thực hiện Validate toàn diện danh sách và gom lỗi theo dòng
            await ValidateDanhSachPhongAsync(danhSachTho);

            return danhSachTho;
        }

        public async Task<int> LuuDanhSachHopLeAsync(IEnumerable<PhongImportDto> danhSach)
        {
            if (danhSach == null)
                throw new ArgumentNullException(nameof(danhSach), "Danh sách dữ liệu không được null.");

            var danhSachHopLe = danhSach.Where(x => x.HopLe && x.IdLoaiPhong.HasValue).ToList();
            if (danhSachHopLe.Count == 0)
                return 0;

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                foreach (var item in danhSachHopLe)
                {
                    var phong = new Phong
                    {
                        Sophong = item.SoPhong.Trim(),
                        IdLoaiphong = item.IdLoaiPhong!.Value,
                        Trangthai = item.TrangThai.Trim()
                    };

                    await _unitOfWork.PhongRepository.AddAsync(phong);
                }

                await _unitOfWork.CommitTransactionAsync();
                return danhSachHopLe.Count;
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        public byte[] TaoFileMauExcel()
        {
            using var workbook = new XLWorkbook();

            // Sheet 1: Mẫu nhập liệu
            var wsData = workbook.Worksheets.Add("Danh sách phòng");
            wsData.Style.Font.FontName = "Segoe UI";
            wsData.Style.Font.FontSize = 10;
            wsData.ShowGridLines = true;

            // Tiêu đề bảng
            wsData.Cell("A1").Value = "BIỂU MẪU IMPORT DANH SÁCH PHÒNG";
            wsData.Range("A1:E1").Merge();
            wsData.Range("A1:E1").Style.Font.FontSize = 13;
            wsData.Range("A1:E1").Style.Font.Bold = true;
            wsData.Range("A1:E1").Style.Font.FontColor = PrimaryNavy;
            wsData.Range("A1:E1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            wsData.Cell("A2").Value = "Hệ thống quản lý khách sạn — Các cột có dấu (*) là bắt buộc";
            wsData.Range("A2:E2").Merge();
            wsData.Range("A2:E2").Style.Font.FontSize = 9.5;
            wsData.Range("A2:E2").Style.Font.Italic = true;
            wsData.Range("A2:E2").Style.Font.FontColor = XLColor.Gray;
            wsData.Range("A2:E2").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            // Header hàng 4
            string[] headers = { "STT", "Số phòng (*)", "Loại phòng (*)", "Trạng thái (*)", "Ghi chú" };
            for (int i = 0; i < headers.Length; i++)
            {
                var cell = wsData.Cell(4, i + 1);
                cell.Value = headers[i];
                cell.Style.Font.Bold = true;
                cell.Style.Font.FontColor = XLColor.White;
                cell.Style.Fill.BackgroundColor = PrimaryNavy;
                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                cell.Style.Border.OutsideBorderColor = BorderGray;
            }
            wsData.Row(4).Height = 26;

            // Dữ liệu mẫu (3 dòng)
            var mauData = new[]
            {
                new { STT = 1, SoPhong = "501", LoaiPhong = "Standard", TrangThai = "Trống", GhiChu = "Phòng tiêu chuẩn tầng 5" },
                new { STT = 2, SoPhong = "502", LoaiPhong = "Deluxe", TrangThai = "Trống", GhiChu = "Phòng cao cấp hướng biển tầng 5" },
                new { STT = 3, SoPhong = "503", LoaiPhong = "VIP", TrangThai = "Bảo trì", GhiChu = "Đang lắp đặt thêm smart TV" }
            };

            for (int i = 0; i < mauData.Length; i++)
            {
                int r = 5 + i;
                wsData.Cell(r, 1).Value = mauData[i].STT;
                wsData.Cell(r, 2).Value = mauData[i].SoPhong;
                wsData.Cell(r, 3).Value = mauData[i].LoaiPhong;
                wsData.Cell(r, 4).Value = mauData[i].TrangThai;
                wsData.Cell(r, 5).Value = mauData[i].GhiChu;

                wsData.Cell(r, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                wsData.Cell(r, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                wsData.Cell(r, 4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                for (int c = 1; c <= 5; c++)
                {
                    wsData.Cell(r, c).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    wsData.Cell(r, c).Style.Border.OutsideBorderColor = BorderGray;
                    if (i % 2 == 1)
                    {
                        wsData.Cell(r, c).Style.Fill.BackgroundColor = TableAlternateRow;
                    }
                }
                wsData.Row(r).Height = 22;
            }

            wsData.Columns().AdjustToContents(15, 45);

            // Sheet 2: Hướng dẫn quy định dữ liệu
            var wsHuongDan = workbook.Worksheets.Add("Quy định dữ liệu");
            wsHuongDan.Style.Font.FontName = "Segoe UI";
            wsHuongDan.Style.Font.FontSize = 10;
            wsHuongDan.ShowGridLines = true;

            wsHuongDan.Cell("A1").Value = "QUY ĐỊNH VALIDATE IMPORT PHÒNG";
            wsHuongDan.Range("A1:C1").Merge();
            wsHuongDan.Range("A1:C1").Style.Font.FontSize = 12;
            wsHuongDan.Range("A1:C1").Style.Font.Bold = true;
            wsHuongDan.Range("A1:C1").Style.Font.FontColor = PrimaryNavy;

            wsHuongDan.Cell("A3").Value = "Cột";
            wsHuongDan.Cell("B3").Value = "Quy định validate";
            wsHuongDan.Cell("C3").Value = "Ví dụ hợp lệ";
            wsHuongDan.Range("A3:C3").Style.Font.Bold = true;
            wsHuongDan.Range("A3:C3").Style.Fill.BackgroundColor = SecondaryBlue;
            wsHuongDan.Range("A3:C3").Style.Font.FontColor = XLColor.White;

            var quyDinh = new[]
            {
                new { Cot = "Số phòng (*)", QuyDinh = "Không được để trống. Không được trùng lặp trong file và không được trùng với phòng đã có trong hệ thống.", ViDu = "101, 202, P301" },
                new { Cot = "Loại phòng (*)", QuyDinh = "Bắt buộc phải khớp chính xác với một trong các tên Loại phòng đã khai báo trong danh mục (không phân biệt hoa thường).", ViDu = "Standard, Deluxe, VIP" },
                new { Cot = "Trạng thái (*)", QuyDinh = "Chỉ chấp nhận 1 trong 4 trạng thái: Trống, Đang ở, Đang dọn, Bảo trì.", ViDu = "Trống, Bảo trì" },
                new { Cot = "Ghi chú", QuyDinh = "Không bắt buộc (chỉ phục vụ người dùng theo dõi nội bộ).", ViDu = "Tầng 5 view biển" }
            };

            for (int i = 0; i < quyDinh.Length; i++)
            {
                int r = 4 + i;
                wsHuongDan.Cell(r, 1).Value = quyDinh[i].Cot;
                wsHuongDan.Cell(r, 2).Value = quyDinh[i].QuyDinh;
                wsHuongDan.Cell(r, 3).Value = quyDinh[i].ViDu;
                wsHuongDan.Cell(r, 1).Style.Font.Bold = true;

                for (int c = 1; c <= 3; c++)
                {
                    wsHuongDan.Cell(r, c).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    wsHuongDan.Cell(r, c).Style.Border.OutsideBorderColor = BorderGray;
                }
            }

            wsHuongDan.Columns().AdjustToContents(20, 60);

            using var memStream = new MemoryStream();
            workbook.SaveAs(memStream);
            return memStream.ToArray();
        }

        private async Task ValidateDanhSachPhongAsync(List<PhongImportDto> danhSach)
        {
            if (danhSach.Count == 0) return;

            // Tải danh mục Loại phòng từ cơ sở dữ liệu
            var dsLoaiPhong = await _unitOfWork.LoaiPhongRepository.GetAllAsync();
            var dictLoaiPhong = dsLoaiPhong.ToDictionary(
                lp => lp.Ten.Trim(),
                lp => lp,
                StringComparer.OrdinalIgnoreCase);

            // Tải danh sách phòng hiện có trong cơ sở dữ liệu để kiểm tra trùng
            var dsPhongHienCo = await _unitOfWork.PhongRepository.GetAllAsync();
            var setSoPhongDb = new HashSet<string>(
                dsPhongHienCo.Select(p => p.Sophong.Trim()),
                StringComparer.OrdinalIgnoreCase);

            // Kiểm tra trùng lặp số phòng trong chính file Excel
            var nhomTrungTrongFile = danhSach
                .Where(x => !string.IsNullOrWhiteSpace(x.SoPhong))
                .GroupBy(x => x.SoPhong.Trim(), StringComparer.OrdinalIgnoreCase)
                .Where(g => g.Count() > 1)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(r => r.DongThuTu).ToList(),
                    StringComparer.OrdinalIgnoreCase);

            // Validate từng dòng (gom toàn bộ lỗi, không ngắt giữa chừng)
            foreach (var item in danhSach)
            {
                var loi = new List<string>();

                // 1. Kiểm tra Số phòng
                if (string.IsNullOrWhiteSpace(item.SoPhong))
                {
                    loi.Add("Số phòng không được để trống.");
                }
                else
                {
                    string soChuan = item.SoPhong.Trim();

                    // Kiểm tra trùng với database
                    if (setSoPhongDb.Contains(soChuan))
                    {
                        loi.Add($"Số phòng '{soChuan}' đã tồn tại trong cơ sở dữ liệu.");
                    }

                    // Kiểm tra trùng trong file Excel
                    if (nhomTrungTrongFile.TryGetValue(soChuan, out var cacDongTrung))
                    {
                        loi.Add($"Số phòng '{soChuan}' bị trùng lặp trong file (xuất hiện tại dòng: {string.Join(", ", cacDongTrung)}).");
                    }
                }

                // 2. Kiểm tra Loại phòng
                if (string.IsNullOrWhiteSpace(item.TenLoaiPhong))
                {
                    loi.Add("Loại phòng không được để trống.");
                }
                else
                {
                    string tenLoaiChuan = item.TenLoaiPhong.Trim();
                    if (dictLoaiPhong.TryGetValue(tenLoaiChuan, out var loaiPhongEntity))
                    {
                        item.IdLoaiPhong = loaiPhongEntity.IdLoaiphong;
                        item.TenLoaiPhong = loaiPhongEntity.Ten; // Chuẩn hóa hoa thường theo tên trong DB
                    }
                    else
                    {
                        loi.Add($"Loại phòng '{tenLoaiChuan}' không tồn tại trong danh mục hệ thống.");
                    }
                }

                // 3. Kiểm tra Trạng thái phòng
                if (string.IsNullOrWhiteSpace(item.TrangThai))
                {
                    loi.Add("Trạng thái phòng không được để trống.");
                }
                else
                {
                    string trangThaiChuan = item.TrangThai.Trim();
                    var trangThaiHopLe = IPhongService.DanhSachTrangThai
                        .FirstOrDefault(t => string.Equals(t, trangThaiChuan, StringComparison.OrdinalIgnoreCase));

                    if (trangThaiHopLe != null)
                    {
                        item.TrangThai = trangThaiHopLe; // Chuẩn hóa hoa thường theo đúng quy ước
                    }
                    else
                    {
                        loi.Add($"Trạng thái '{trangThaiChuan}' không hợp lệ (chỉ chấp nhận: {string.Join(", ", IPhongService.DanhSachTrangThai)}).");
                    }
                }

                item.DanhSachLoi = loi;
                item.HopLe = (loi.Count == 0);
            }
        }

        private static int TimCotTheoTuKhoa(IXLRow row, params string[] tuKhoas)
        {
            foreach (var cell in row.CellsUsed())
            {
                string text = LayGiaTriO(cell).ToLowerInvariant();
                foreach (var kw in tuKhoas)
                {
                    if (text.Contains(kw.ToLowerInvariant()))
                    {
                        return cell.Address.ColumnNumber;
                    }
                }
            }
            return -1;
        }

        private static string LayGiaTriO(IXLCell? cell)
        {
            if (cell == null || cell.IsEmpty()) return string.Empty;

            try
            {
                return cell.GetFormattedString().Trim();
            }
            catch
            {
                return cell.ToString()?.Trim() ?? string.Empty;
            }
        }
    }
}
