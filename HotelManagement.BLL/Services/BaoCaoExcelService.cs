using System;
using System.IO;
using System.Threading.Tasks;
using ClosedXML.Excel;
using HotelManagement.BLL.DTOs;

namespace HotelManagement.BLL.Services
{
    /// <summary>
    /// Triển khai dịch vụ xuất báo cáo doanh thu Excel bằng ClosedXML (DCKLTTQ-78).
    /// </summary>
    public class BaoCaoExcelService : IBaoCaoExcelService
    {
        private static readonly XLColor PrimaryNavy = XLColor.FromHtml("#0D3B66");
        private static readonly XLColor SecondaryBlue = XLColor.FromHtml("#1E5F8A");
        private static readonly XLColor AccentGold = XLColor.FromHtml("#D4AF37");
        private static readonly XLColor TableAlternateRow = XLColor.FromHtml("#F9FBFC");
        private static readonly XLColor BorderGray = XLColor.FromHtml("#DEE2E6");
        private static readonly XLColor CardDarkBg = XLColor.FromHtml("#F1F5F9");

        private const string MainFont = "Segoe UI";

        public byte[] SinhBaoCaoExcel(BaoCaoDoanhThuDto baoCao)
        {
            if (baoCao == null)
                throw new ArgumentNullException(nameof(baoCao), "Dữ liệu báo cáo không được để trống.");

            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Báo cáo doanh thu");

            ws.Style.Font.FontName = MainFont;
            ws.Style.Font.FontSize = 10;
            ws.ShowGridLines = true;

            // 1. Tiêu đề chính của báo cáo
            ws.Cell("A1").Value = "KHÁCH SẠN HOTEL MANAGEMENT";
            ws.Range("A1:K1").Merge();
            ws.Range("A1:K1").Style.Font.FontSize = 15;
            ws.Range("A1:K1").Style.Font.Bold = true;
            ws.Range("A1:K1").Style.Font.FontColor = PrimaryNavy;
            ws.Range("A1:K1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            ws.Cell("A2").Value = "BÁO CÁO DOANH THU & HIỆU SUẤT HOẠT ĐỘNG PHÒNG";
            ws.Range("A2:K2").Merge();
            ws.Range("A2:K2").Style.Font.FontSize = 13;
            ws.Range("A2:K2").Style.Font.Bold = true;
            ws.Range("A2:K2").Style.Font.FontColor = SecondaryBlue;
            ws.Range("A2:K2").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            string loaiPhongStr = string.IsNullOrWhiteSpace(baoCao.TenLoaiPhongLoc) ? "Tất cả loại phòng" : baoCao.TenLoaiPhongLoc;
            ws.Cell("A3").Value = $"Khoảng thời gian: {baoCao.TuNgay:dd/MM/yyyy} - {baoCao.DenNgay:dd/MM/yyyy}  |  Loại phòng: {loaiPhongStr}  |  Người lập: {baoCao.NguoiLapBaoCao}  |  Ngày xuất: {baoCao.NgayXuatBaoCao:dd/MM/yyyy HH:mm}";
            ws.Range("A3:K3").Merge();
            ws.Range("A3:K3").Style.Font.FontSize = 9.5;
            ws.Range("A3:K3").Style.Font.Italic = true;
            ws.Range("A3:K3").Style.Font.FontColor = XLColor.Gray;
            ws.Range("A3:K3").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            // 2. Khối KPI Cards / Tổng quan hoạt động
            ws.Cell("A5").Value = "I. CHỈ SỐ TỔNG QUAN HIỆU SUẤT";
            ws.Cell("A5").Style.Font.FontSize = 11;
            ws.Cell("A5").Style.Font.Bold = true;
            ws.Cell("A5").Style.Font.FontColor = PrimaryNavy;

            void CreateKpiCard(string rangeHeader, string rangeVal, string title, object val, XLColor headerBg, XLColor headerText, string? numFormat = null)
            {
                ws.Range(rangeHeader).Merge();
                ws.Range(rangeHeader).Value = title;
                ws.Range(rangeHeader).Style.Fill.BackgroundColor = headerBg;
                ws.Range(rangeHeader).Style.Font.FontColor = headerText;
                ws.Range(rangeHeader).Style.Font.Bold = true;
                ws.Range(rangeHeader).Style.Font.FontSize = 9;
                ws.Range(rangeHeader).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                ws.Range(rangeVal).Merge();
                var cellVal = ws.Range(rangeVal).FirstCell();
                if (val is decimal dec)
                    cellVal.Value = dec;
                else if (val is int i)
                    cellVal.Value = i;
                else if (val is double d)
                    cellVal.Value = d;
                else
                    cellVal.Value = val.ToString();

                ws.Range(rangeVal).Style.Fill.BackgroundColor = CardDarkBg;
                ws.Range(rangeVal).Style.Font.Bold = true;
                ws.Range(rangeVal).Style.Font.FontSize = 12;
                ws.Range(rangeVal).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Range(rangeVal).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                ws.Range(rangeVal).Style.Border.OutsideBorderColor = BorderGray;

                if (!string.IsNullOrEmpty(numFormat))
                {
                    ws.Range(rangeVal).Style.NumberFormat.Format = numFormat;
                }
            }

            CreateKpiCard("A6:C6", "A7:C7", "TỔNG DOANH THU THỰC TẾ", baoCao.TongDoanhThu, PrimaryNavy, XLColor.White, "#,##0 \"đ\"");
            CreateKpiCard("D6:E6", "D7:E7", "DOANH THU TIỀN PHÒNG", baoCao.TongTienPhong, SecondaryBlue, XLColor.White, "#,##0 \"đ\"");
            CreateKpiCard("F6:G6", "F7:G7", "DOANH THU DỊCH VỤ", baoCao.TongTienDichVu, XLColor.FromHtml("#2B7A78"), XLColor.White, "#,##0 \"đ\"");
            CreateKpiCard("H6:I6", "H7:I7", "TỶ LỆ LẤP ĐẦY PHÒNG", baoCao.TyLeLapDayPhanTram / 100.0, AccentGold, XLColor.Black, "0.0%");
            CreateKpiCard("J6:K6", "J7:K7", "TỔNG BOOKING / ĐÊM Ở", $"{baoCao.TongSoBooking} đơn ({baoCao.TongSoDemLuuTru} đêm)", XLColor.FromHtml("#4A5568"), XLColor.White);

            // 3. Bảng 1: Phân rã doanh thu theo loại phòng (Breakdown)
            int currentRow = 9;
            ws.Cell(currentRow, 1).Value = "II. PHÂN TÍCH DOANH THU THEO LOẠI PHÒNG";
            ws.Cell(currentRow, 1).Style.Font.FontSize = 11;
            ws.Cell(currentRow, 1).Style.Font.Bold = true;
            ws.Cell(currentRow, 1).Style.Font.FontColor = PrimaryNavy;

            currentRow++;
            int table1HeaderRow = currentRow;
            string[] table1Headers = { "STT", "Tên loại phòng", "Số phòng hiện có", "Số lượt đặt", "Tổng số đêm", "Doanh thu phòng", "Tỷ trọng (%)" };
            for (int i = 0; i < table1Headers.Length; i++)
            {
                var cell = ws.Cell(table1HeaderRow, i + 1);
                cell.Value = table1Headers[i];
                cell.Style.Fill.BackgroundColor = PrimaryNavy;
                cell.Style.Font.FontColor = XLColor.White;
                cell.Style.Font.Bold = true;
                cell.Style.Alignment.Horizontal = (i == 0 || i == 2 || i == 3 || i == 4 || i == 6)
                    ? XLAlignmentHorizontalValues.Center
                    : (i == 5 ? XLAlignmentHorizontalValues.Right : XLAlignmentHorizontalValues.Left);
                cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                cell.Style.Border.OutsideBorderColor = BorderGray;
            }

            int sttLoai = 1;
            int table1DataStart = table1HeaderRow + 1;
            currentRow = table1DataStart;

            foreach (var lp in baoCao.DanhSachLoaiPhong)
            {
                XLColor rowBg = (sttLoai % 2 == 0) ? TableAlternateRow : XLColor.White;

                ws.Cell(currentRow, 1).Value = sttLoai;
                ws.Cell(currentRow, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                ws.Cell(currentRow, 2).Value = lp.TenLoaiPhong;
                ws.Cell(currentRow, 2).Style.Font.Bold = true;

                ws.Cell(currentRow, 3).Value = lp.SoLuongPhong;
                ws.Cell(currentRow, 3).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                ws.Cell(currentRow, 4).Value = lp.SoLuotDat;
                ws.Cell(currentRow, 4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                ws.Cell(currentRow, 5).Value = lp.TongSoDem;
                ws.Cell(currentRow, 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                ws.Cell(currentRow, 6).Value = lp.DoanhThuPhong;
                ws.Cell(currentRow, 6).Style.NumberFormat.Format = "#,##0 \"đ\"";
                ws.Cell(currentRow, 6).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                ws.Cell(currentRow, 7).Value = lp.TyTrongPhanTram / 100.0;
                ws.Cell(currentRow, 7).Style.NumberFormat.Format = "0.0%";
                ws.Cell(currentRow, 7).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                for (int col = 1; col <= 7; col++)
                {
                    ws.Cell(currentRow, col).Style.Fill.BackgroundColor = rowBg;
                    ws.Cell(currentRow, col).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    ws.Cell(currentRow, col).Style.Border.OutsideBorderColor = BorderGray;
                }

                sttLoai++;
                currentRow++;
            }

            // Dòng tổng cộng bảng 1
            int table1TotalRow = currentRow;
            ws.Range(table1TotalRow, 1, table1TotalRow, 5).Merge();
            ws.Cell(table1TotalRow, 1).Value = "TỔNG CỘNG TIỀN PHÒNG:";
            ws.Cell(table1TotalRow, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            ws.Cell(table1TotalRow, 1).Style.Font.Bold = true;

            if (baoCao.DanhSachLoaiPhong.Count > 0)
            {
                ws.Cell(table1TotalRow, 6).FormulaA1 = $"=SUM(F{table1DataStart}:F{table1TotalRow - 1})";
                ws.Cell(table1TotalRow, 7).FormulaA1 = $"=SUM(G{table1DataStart}:G{table1TotalRow - 1})";
            }
            else
            {
                ws.Cell(table1TotalRow, 6).Value = 0;
                ws.Cell(table1TotalRow, 7).Value = 0;
            }

            ws.Cell(table1TotalRow, 6).Style.NumberFormat.Format = "#,##0 \"đ\"";
            ws.Cell(table1TotalRow, 6).Style.Font.Bold = true;
            ws.Cell(table1TotalRow, 6).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

            ws.Cell(table1TotalRow, 7).Style.NumberFormat.Format = "0.0%";
            ws.Cell(table1TotalRow, 7).Style.Font.Bold = true;
            ws.Cell(table1TotalRow, 7).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            for (int col = 1; col <= 7; col++)
            {
                ws.Cell(table1TotalRow, col).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell(table1TotalRow, col).Style.Border.BottomBorder = XLBorderStyleValues.Double;
                ws.Cell(table1TotalRow, col).Style.Fill.BackgroundColor = CardDarkBg;
            }

            // 4. Bảng 2: Danh sách chi tiết các hóa đơn trong kỳ
            currentRow += 2;
            ws.Cell(currentRow, 1).Value = "III. DANH SÁCH HÓA ĐƠN ĐÃ THANH TOÁN";
            ws.Cell(currentRow, 1).Style.Font.FontSize = 11;
            ws.Cell(currentRow, 1).Style.Font.Bold = true;
            ws.Cell(currentRow, 1).Style.Font.FontColor = PrimaryNavy;

            currentRow++;
            int table2HeaderRow = currentRow;
            string[] table2Headers = { "STT", "Số HĐ", "Mã Booking", "Khách hàng", "Phòng", "Ngày thanh toán", "Tiền phòng", "Tiền dịch vụ", "Giảm giá", "Tổng cộng", "Hình thức TT" };

            for (int i = 0; i < table2Headers.Length; i++)
            {
                var cell = ws.Cell(table2HeaderRow, i + 1);
                cell.Value = table2Headers[i];
                cell.Style.Fill.BackgroundColor = SecondaryBlue;
                cell.Style.Font.FontColor = XLColor.White;
                cell.Style.Font.Bold = true;
                cell.Style.Alignment.Horizontal = (i == 0 || i == 1 || i == 2 || i == 4 || i == 5 || i == 10)
                    ? XLAlignmentHorizontalValues.Center
                    : (i >= 6 && i <= 9 ? XLAlignmentHorizontalValues.Right : XLAlignmentHorizontalValues.Left);
                cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                cell.Style.Border.OutsideBorderColor = BorderGray;
            }

            int sttHd = 1;
            int table2DataStart = table2HeaderRow + 1;
            currentRow = table2DataStart;

            foreach (var hd in baoCao.DanhSachHoaDon)
            {
                XLColor rowBg = (sttHd % 2 == 0) ? TableAlternateRow : XLColor.White;

                ws.Cell(currentRow, 1).Value = sttHd;
                ws.Cell(currentRow, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                ws.Cell(currentRow, 2).Value = $"HD-{hd.IdHoaDon:D5}";
                ws.Cell(currentRow, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                ws.Cell(currentRow, 3).Value = hd.MaBooking;
                ws.Cell(currentRow, 3).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Cell(currentRow, 3).Style.Font.Bold = true;

                ws.Cell(currentRow, 4).Value = hd.TenKhachHang;

                ws.Cell(currentRow, 5).Value = hd.DanhSachPhong;
                ws.Cell(currentRow, 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                ws.Cell(currentRow, 6).Value = hd.NgayThanhToan.ToString("dd/MM/yyyy HH:mm");
                ws.Cell(currentRow, 6).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                ws.Cell(currentRow, 7).Value = hd.TienPhong;
                ws.Cell(currentRow, 7).Style.NumberFormat.Format = "#,##0 \"đ\"";
                ws.Cell(currentRow, 7).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                ws.Cell(currentRow, 8).Value = hd.TienDichVu;
                ws.Cell(currentRow, 8).Style.NumberFormat.Format = "#,##0 \"đ\"";
                ws.Cell(currentRow, 8).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                ws.Cell(currentRow, 9).Value = hd.GiamGia;
                ws.Cell(currentRow, 9).Style.NumberFormat.Format = "#,##0 \"đ\"";
                ws.Cell(currentRow, 9).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                ws.Cell(currentRow, 10).Value = hd.TongTien;
                ws.Cell(currentRow, 10).Style.NumberFormat.Format = "#,##0 \"đ\"";
                ws.Cell(currentRow, 10).Style.Font.Bold = true;
                ws.Cell(currentRow, 10).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                ws.Cell(currentRow, 11).Value = hd.PhuongThucThanhToan;
                ws.Cell(currentRow, 11).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                for (int col = 1; col <= 11; col++)
                {
                    ws.Cell(currentRow, col).Style.Fill.BackgroundColor = rowBg;
                    ws.Cell(currentRow, col).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    ws.Cell(currentRow, col).Style.Border.OutsideBorderColor = BorderGray;
                }

                sttHd++;
                currentRow++;
            }

            // Dòng tổng cộng bảng 2
            int table2TotalRow = currentRow;
            ws.Range(table2TotalRow, 1, table2TotalRow, 6).Merge();
            ws.Cell(table2TotalRow, 1).Value = "TỔNG CỘNG DOANH THU:";
            ws.Cell(table2TotalRow, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            ws.Cell(table2TotalRow, 1).Style.Font.Bold = true;

            if (baoCao.DanhSachHoaDon.Count > 0)
            {
                ws.Cell(table2TotalRow, 7).FormulaA1 = $"=SUM(G{table2DataStart}:G{table2TotalRow - 1})";
                ws.Cell(table2TotalRow, 8).FormulaA1 = $"=SUM(H{table2DataStart}:H{table2TotalRow - 1})";
                ws.Cell(table2TotalRow, 9).FormulaA1 = $"=SUM(I{table2DataStart}:I{table2TotalRow - 1})";
                ws.Cell(table2TotalRow, 10).FormulaA1 = $"=SUM(J{table2DataStart}:J{table2TotalRow - 1})";
            }
            else
            {
                ws.Cell(table2TotalRow, 7).Value = 0;
                ws.Cell(table2TotalRow, 8).Value = 0;
                ws.Cell(table2TotalRow, 9).Value = 0;
                ws.Cell(table2TotalRow, 10).Value = 0;
            }

            for (int col = 7; col <= 10; col++)
            {
                ws.Cell(table2TotalRow, col).Style.NumberFormat.Format = "#,##0 \"đ\"";
                ws.Cell(table2TotalRow, col).Style.Font.Bold = true;
                ws.Cell(table2TotalRow, col).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            }

            for (int col = 1; col <= 11; col++)
            {
                ws.Cell(table2TotalRow, col).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell(table2TotalRow, col).Style.Border.BottomBorder = XLBorderStyleValues.Double;
                ws.Cell(table2TotalRow, col).Style.Fill.BackgroundColor = CardDarkBg;
            }

            // 5. Khối chữ ký phê duyệt cuối báo cáo
            currentRow += 2;
            ws.Cell(currentRow, 2).Value = "NGƯỜI LẬP BÁO CÁO";
            ws.Cell(currentRow, 2).Style.Font.Bold = true;
            ws.Cell(currentRow, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            ws.Cell(currentRow, 9).Value = "GIÁM ĐỐC / QUẢN LÝ";
            ws.Cell(currentRow, 9).Style.Font.Bold = true;
            ws.Cell(currentRow, 9).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            currentRow++;
            ws.Cell(currentRow, 2).Value = "(Ký và ghi rõ họ tên)";
            ws.Cell(currentRow, 2).Style.Font.Italic = true;
            ws.Cell(currentRow, 2).Style.Font.FontSize = 8.5;
            ws.Cell(currentRow, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            ws.Cell(currentRow, 9).Value = "(Ký và đóng dấu)";
            ws.Cell(currentRow, 9).Style.Font.Italic = true;
            ws.Cell(currentRow, 9).Style.Font.FontSize = 8.5;
            ws.Cell(currentRow, 9).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            currentRow += 4;
            ws.Cell(currentRow, 2).Value = baoCao.NguoiLapBaoCao;
            ws.Cell(currentRow, 2).Style.Font.Bold = true;
            ws.Cell(currentRow, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            // Tự động căn chỉnh độ rộng cột theo nội dung
            ws.Columns().AdjustToContents();

            using var memoryStream = new MemoryStream();
            workbook.SaveAs(memoryStream);
            return memoryStream.ToArray();
        }

        public async Task XuatFileExcelAsync(BaoCaoDoanhThuDto baoCao, string duongDanTep)
        {
            if (baoCao == null)
                throw new ArgumentNullException(nameof(baoCao), "Dữ liệu báo cáo không được để trống.");

            if (string.IsNullOrWhiteSpace(duongDanTep))
                throw new ArgumentException("Đường dẫn lưu tệp không được để trống.", nameof(duongDanTep));

            string? thuMuc = Path.GetDirectoryName(duongDanTep);
            if (!string.IsNullOrEmpty(thuMuc) && !Directory.Exists(thuMuc))
            {
                Directory.CreateDirectory(thuMuc);
            }

            byte[] bytes = SinhBaoCaoExcel(baoCao);
            await File.WriteAllBytesAsync(duongDanTep, bytes);
        }
    }
}
