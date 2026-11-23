using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using ClosedXML.Excel;

namespace HotelManagement.Common
{
    /// <summary>
    /// Tiện ích xuất dữ liệu từ DataGridView ra tệp Excel (.xlsx) sử dụng ClosedXML.
    /// Bỏ qua các cột hình ảnh (Edit, Delete, Avatar) và định dạng tự động.
    /// </summary>
    public static class ExcelExportHelper
    {
        public static async Task XuatDataGridViewRaExcelAsync(DataGridView dgv, string tenTieuDe, string tenFileMacDinh, IWin32Window parent)
        {
            if (dgv.Rows.Count == 0)
            {
                MessageBox.Show("Không có dữ liệu trong danh sách để xuất file Excel.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using var sfd = new SaveFileDialog
            {
                Title = "Xuất dữ liệu Excel",
                Filter = "Excel Workbook (*.xlsx)|*.xlsx",
                FileName = $"{tenFileMacDinh}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx"
            };

            if (sfd.ShowDialog(parent) != DialogResult.OK)
                return;

            string duongDan = sfd.FileName;

            try
            {
                await Task.Run(() =>
                {
                    using var workbook = new XLWorkbook();
                    var ws = workbook.Worksheets.Add(tenTieuDe.Length > 25 ? tenTieuDe.Substring(0, 25) : tenTieuDe);

                    // 1. Tiêu đề chính
                    ws.Cell(1, 1).Value = $"DANH SÁCH {tenTieuDe.ToUpper()}";
                    ws.Cell(1, 1).Style.Font.Bold = true;
                    ws.Cell(1, 1).Style.Font.FontSize = 14;
                    ws.Cell(1, 1).Style.Font.FontColor = XLColor.FromHtml("#1B5E20");

                    ws.Cell(2, 1).Value = $"Ngày xuất: {DateTime.Now:dd/MM/yyyy HH:mm:ss}";
                    ws.Cell(2, 1).Style.Font.Italic = true;
                    ws.Cell(2, 1).Style.Font.FontSize = 10;
                    ws.Cell(2, 1).Style.Font.FontColor = XLColor.Gray;

                    // 2. Ghi Header cột
                    int colIndex = 1;
                    var colMap = new System.Collections.Generic.List<int>();

                    for (int i = 0; i < dgv.Columns.Count; i++)
                    {
                        var col = dgv.Columns[i];
                        if (!col.Visible || col is DataGridViewImageColumn)
                            continue;

                        ws.Cell(4, colIndex).Value = string.IsNullOrWhiteSpace(col.HeaderText) ? col.Name : col.HeaderText;
                        ws.Cell(4, colIndex).Style.Font.Bold = true;
                        ws.Cell(4, colIndex).Style.Fill.BackgroundColor = XLColor.FromHtml("#00838F");
                        ws.Cell(4, colIndex).Style.Font.FontColor = XLColor.White;
                        ws.Cell(4, colIndex).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        ws.Cell(4, colIndex).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                        colMap.Add(i);
                        colIndex++;
                    }

                    // 3. Ghi dữ liệu dòng
                    int rowIndex = 5;
                    for (int r = 0; r < dgv.Rows.Count; r++)
                    {
                        var row = dgv.Rows[r];
                        if (row.IsNewRow) continue;

                        for (int c = 0; c < colMap.Count; c++)
                        {
                            int originalCol = colMap[c];
                            var cellVal = row.Cells[originalCol].Value;
                            var xlCell = ws.Cell(rowIndex, c + 1);

                            if (cellVal != null)
                            {
                                if (cellVal is decimal decVal)
                                {
                                    xlCell.Value = (double)decVal;
                                    xlCell.Style.NumberFormat.Format = "#,##0";
                                    xlCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                                }
                                else if (cellVal is int intVal)
                                {
                                    xlCell.Value = intVal;
                                    xlCell.Style.NumberFormat.Format = "#,##0";
                                    xlCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                                }
                                else if (cellVal is DateTime dtVal)
                                {
                                    xlCell.Value = dtVal;
                                    xlCell.Style.NumberFormat.Format = "dd/MM/yyyy HH:mm";
                                    xlCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                                }
                                else
                                {
                                    xlCell.Value = cellVal.ToString();
                                }
                            }

                            // Viền nhẹ
                            xlCell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                            xlCell.Style.Border.OutsideBorderColor = XLColor.FromHtml("#E0E0E0");

                            if (rowIndex % 2 == 0)
                            {
                                xlCell.Style.Fill.BackgroundColor = XLColor.FromHtml("#F9FBFB");
                            }
                        }
                        rowIndex++;
                    }

                    // 4. Định dạng chung bảng
                    ws.Columns().AdjustToContents();
                    workbook.SaveAs(duongDan);
                });

                var xacNhan = MessageBox.Show(
                    $"Xuất file Excel thành công!\n\nĐường dẫn:\n{duongDan}\n\nBạn có muốn mở file ngay bây giờ không?",
                    "Thành công",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Information);

                if (xacNhan == DialogResult.Yes)
                {
                    Process.Start(new ProcessStartInfo(duongDan) { UseShellExecute = true });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Có lỗi xảy ra khi xuất file Excel: " + ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
