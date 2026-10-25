using System;
using HotelManagement.BLL.DTOs;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace HotelManagement.BLL.Pdf
{
    /// <summary>
    /// Bản dựng bố cục hóa đơn khách sạn theo chuẩn Fluent API của QuestPDF (DCKLTTQ-77).
    /// Dữ liệu nạp từ HoaDonDto (được sinh từ IHoaDonService, tổng giá lấy từ computed column SQL Server).
    /// </summary>
    public class HoaDonPdfDocument : IDocument
    {
        public HoaDonDto Model { get; }
        public HoaDonPdfTheme Theme { get; }

        public HoaDonPdfDocument(HoaDonDto model, HoaDonPdfTheme? theme = null)
        {
            Model = model ?? throw new ArgumentNullException(nameof(model));
            Theme = theme ?? HoaDonPdfTheme.MacDinh;
        }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;
        public DocumentSettings GetSettings() => DocumentSettings.Default;

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(Theme.PageMargin);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontFamily(Theme.FontFamily).FontSize(Theme.BodyFontSize).FontColor(Theme.TextColor));

                page.Header().Element(ComposeHeader);
                page.Content().Element(ComposeContent);
                page.Footer().Element(ComposeFooter);
            });
        }

        private void ComposeHeader(IContainer container)
        {
            container.Column(column =>
            {
                column.Item().Row(row =>
                {
                    // Thông tin khách sạn bên trái
                    row.RelativeItem(6).Column(col =>
                    {
                        col.Item().Text(Theme.TenKhachSan)
                            .FontFamily(Theme.FontFamily)
                            .FontSize(Theme.SectionHeaderFontSize + 2)
                            .Bold()
                            .FontColor(Theme.PrimaryColor);

                        col.Item().Text(Theme.Slogan)
                            .FontSize(Theme.SmallFontSize)
                            .Italic()
                            .FontColor(Theme.MutedTextColor);

                        col.Item().PaddingTop(3).Text($"Địa chỉ: {Theme.DiaChi}").FontSize(Theme.SmallFontSize);
                        col.Item().Text($"Hotline: {Theme.Hotline}  |  Email: {Theme.Email}").FontSize(Theme.SmallFontSize);
                    });

                    // Tiêu đề hóa đơn và mã số bên phải
                    row.RelativeItem(4).AlignRight().Column(col =>
                    {
                        col.Item().Text("HÓA ĐƠN THANH TOÁN")
                            .FontFamily(Theme.FontFamily)
                            .FontSize(Theme.TitleFontSize)
                            .Bold()
                            .FontColor(Theme.PrimaryColor);

                        col.Item().Text("HOTEL INVOICE")
                            .FontSize(Theme.SmallFontSize)
                            .Bold()
                            .FontColor(Theme.MutedTextColor);

                        col.Item().PaddingTop(4).Text(text =>
                        {
                            text.Span("Mã Booking: ").FontSize(Theme.BodyFontSize);
                            text.Span(Model.MaBooking).FontSize(Theme.BodyFontSize).Bold().FontColor(Theme.PrimaryColor);
                        });

                        col.Item().Text(text =>
                        {
                            text.Span("Số HĐ: ").FontSize(Theme.SmallFontSize);
                            text.Span(Model.IdHoaDon > 0 ? $"HD-{Model.IdHoaDon:D6}" : "TẠM TÍNH")
                                .FontSize(Theme.SmallFontSize)
                                .Bold();
                        });

                        col.Item().Text($"Ngày lập: {Model.NgayLap:dd/MM/yyyy HH:mm}").FontSize(Theme.SmallFontSize);

                        // Trạng thái thanh toán (Badge)
                        bool daThanhToan = Model.TrangThaiThanhtoan.Equals("Đã thanh toán", StringComparison.OrdinalIgnoreCase);
                        string badgeBg = daThanhToan ? Theme.SuccessColor : Theme.WarningColor;
                        string badgeText = daThanhToan ? "ĐÃ THANH TOÁN" : "CHƯA THANH TOÁN";

                        col.Item().PaddingTop(4).AlignRight().Background(badgeBg).PaddingVertical(3).PaddingHorizontal(8).Text(badgeText)
                            .FontSize(Theme.SmallFontSize)
                            .Bold()
                            .FontColor(Colors.White);
                    });
                });

                // Đường kẻ ngăn cách Header
                column.Item().PaddingTop(10).PaddingBottom(10).LineHorizontal(1.5f).LineColor(Theme.BorderColor);
            });
        }

        private void ComposeContent(IContainer container)
        {
            container.Column(col =>
            {
                // 1. Khối thông tin khách hàng & booking (Card view 2 cột)
                col.Item().Element(ComposeCustomerAndBookingInfo);

                col.Item().PaddingVertical(8);

                // 2. Bảng chi tiết tiền phòng
                col.Item().Element(ComposeRoomTable);

                col.Item().PaddingVertical(6);

                // 3. Bảng dịch vụ phát sinh (nếu có)
                if (Model.DanhSachDichVu != null && Model.DanhSachDichVu.Count > 0)
                {
                    col.Item().Element(ComposeServiceTable);
                    col.Item().PaddingVertical(6);
                }

                // 4. Bảng tổng kết chi phí thanh toán
                col.Item().Element(ComposeSummaryBox);

                col.Item().PaddingTop(18);

                // 5. Khu vực ký xác nhận
                col.Item().Element(ComposeSignatureSection);
            });
        }

        private void ComposeCustomerAndBookingInfo(IContainer container)
        {
            container.Background(Theme.BackgroundColor)
                .Border(1)
                .BorderColor(Theme.BorderColor)
                .Padding(10)
                .Row(row =>
                {
                    // Thông tin khách hàng
                    row.RelativeItem().Column(c =>
                    {
                        c.Item().Text("THÔNG TIN KHÁCH HÀNG")
                            .FontSize(Theme.SmallFontSize + 0.5f)
                            .Bold()
                            .FontColor(Theme.PrimaryColor);

                        c.Item().PaddingTop(3).Text(t =>
                        {
                            t.Span("Họ và tên: ").SemiBold();
                            t.Span(!string.IsNullOrWhiteSpace(Model.TenKhachHang) ? Model.TenKhachHang : "-");
                        });

                        c.Item().Text(t =>
                        {
                            t.Span("Số điện thoại: ").SemiBold();
                            t.Span(!string.IsNullOrWhiteSpace(Model.SdtKhachHang) ? Model.SdtKhachHang : "-");
                        });

                        c.Item().Text(t =>
                        {
                            t.Span("CCCD/Hộ chiếu: ").SemiBold();
                            t.Span(!string.IsNullOrWhiteSpace(Model.CccdKhachHang) ? Model.CccdKhachHang : "-");
                        });

                        c.Item().Text(t =>
                        {
                            t.Span("Email: ").SemiBold();
                            t.Span(!string.IsNullOrWhiteSpace(Model.EmailKhachHang) ? Model.EmailKhachHang : "-");
                        });
                    });

                    // Chi tiết thời gian lưu trú
                    row.RelativeItem().Column(c =>
                    {
                        c.Item().Text("THỜI GIAN LƯU TRÚ")
                            .FontSize(Theme.SmallFontSize + 0.5f)
                            .Bold()
                            .FontColor(Theme.PrimaryColor);

                        c.Item().PaddingTop(3).Text(t =>
                        {
                            t.Span("Nhận phòng dự kiến: ").SemiBold();
                            t.Span(Model.NgayNhanDuKien.ToString("dd/MM/yyyy"));
                        });

                        c.Item().Text(t =>
                        {
                            t.Span("Trả phòng dự kiến: ").SemiBold();
                            t.Span(Model.NgayTraDuKien.ToString("dd/MM/yyyy"));
                        });

                        if (Model.NgayNhanThucTe.HasValue || Model.NgayTraThucTe.HasValue)
                        {
                            string nhanThucTe = Model.NgayNhanThucTe.HasValue ? Model.NgayNhanThucTe.Value.ToString("dd/MM/yyyy HH:mm") : "-";
                            string traThucTe = Model.NgayTraThucTe.HasValue ? Model.NgayTraThucTe.Value.ToString("dd/MM/yyyy HH:mm") : "-";

                            c.Item().Text(t =>
                            {
                                t.Span("Thực tế: ").SemiBold();
                                t.Span($"{nhanThucTe} → {traThucTe}").FontSize(Theme.SmallFontSize);
                            });
                        }

                        c.Item().Text(t =>
                        {
                            t.Span("Tổng số đêm: ").SemiBold();
                            t.Span($"{Model.SoDemLuuTru} đêm").Bold().FontColor(Theme.PrimaryColor);
                        });
                    });
                });
        }

        private void ComposeRoomTable(IContainer container)
        {
            container.Column(col =>
            {
                col.Item().PaddingBottom(4).Text("I. CHI TIẾT TIỀN PHÒNG")
                    .FontSize(Theme.SectionHeaderFontSize)
                    .Bold()
                    .FontColor(Theme.PrimaryColor);

                col.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(30);   // STT
                        columns.RelativeColumn(2);    // Số phòng
                        columns.RelativeColumn(3.5f); // Loại phòng
                        columns.RelativeColumn(2.5f); // Đơn giá / đêm
                        columns.RelativeColumn(1.8f); // Số đêm
                        columns.RelativeColumn(2.5f); // Thành tiền
                    });

                    // Tiêu đề bảng
                    table.Header(header =>
                    {
                        header.Cell().Background(Theme.TableHeaderBgColor).Padding(5).AlignCenter().Text("STT").FontColor(Colors.White).Bold();
                        header.Cell().Background(Theme.TableHeaderBgColor).Padding(5).Text("Số phòng").FontColor(Colors.White).Bold();
                        header.Cell().Background(Theme.TableHeaderBgColor).Padding(5).Text("Loại phòng").FontColor(Colors.White).Bold();
                        header.Cell().Background(Theme.TableHeaderBgColor).Padding(5).AlignRight().Text("Đơn giá/đêm").FontColor(Colors.White).Bold();
                        header.Cell().Background(Theme.TableHeaderBgColor).Padding(5).AlignCenter().Text("Số đêm").FontColor(Colors.White).Bold();
                        header.Cell().Background(Theme.TableHeaderBgColor).Padding(5).AlignRight().Text("Thành tiền").FontColor(Colors.White).Bold();
                    });

                    // Dòng dữ liệu
                    int stt = 1;
                    foreach (var p in Model.DanhSachPhong)
                    {
                        string bg = (stt % 2 == 0) ? Theme.TableAlternateBgColor : Colors.White;
                        decimal thanhTienPhong = p.DonGia * Model.SoDemLuuTru;

                        table.Cell().Background(bg).BorderBottom(1).BorderColor(Theme.BorderColor).Padding(5).AlignCenter().Text(stt.ToString());
                        table.Cell().Background(bg).BorderBottom(1).BorderColor(Theme.BorderColor).Padding(5).Text(p.SoPhong).Bold();
                        table.Cell().Background(bg).BorderBottom(1).BorderColor(Theme.BorderColor).Padding(5).Text(p.TenLoaiPhong);
                        table.Cell().Background(bg).BorderBottom(1).BorderColor(Theme.BorderColor).Padding(5).AlignRight().Text($"{p.DonGia:N0} đ");
                        table.Cell().Background(bg).BorderBottom(1).BorderColor(Theme.BorderColor).Padding(5).AlignCenter().Text(Model.SoDemLuuTru.ToString());
                        table.Cell().Background(bg).BorderBottom(1).BorderColor(Theme.BorderColor).Padding(5).AlignRight().Text($"{thanhTienPhong:N0} đ").SemiBold();

                        stt++;
                    }

                    if (Model.DanhSachPhong.Count == 0)
                    {
                        table.Cell().ColumnSpan(6).Padding(8).AlignCenter().Text("Không có thông tin phòng.").Italic();
                    }
                });
            });
        }

        private void ComposeServiceTable(IContainer container)
        {
            container.Column(col =>
            {
                col.Item().PaddingBottom(4).Text("II. DỊCH VỤ PHÁT SINH")
                    .FontSize(Theme.SectionHeaderFontSize)
                    .Bold()
                    .FontColor(Theme.PrimaryColor);

                col.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(30);   // STT
                        columns.RelativeColumn(3.5f); // Tên dịch vụ
                        columns.RelativeColumn(2.5f); // Thời gian sử dụng
                        columns.RelativeColumn(1.8f); // Số lượng
                        columns.RelativeColumn(2.5f); // Đơn giá
                        columns.RelativeColumn(2.5f); // Thành tiền
                    });

                    table.Header(header =>
                    {
                        header.Cell().Background(Theme.TableHeaderBgColor).Padding(5).AlignCenter().Text("STT").FontColor(Colors.White).Bold();
                        header.Cell().Background(Theme.TableHeaderBgColor).Padding(5).Text("Tên dịch vụ").FontColor(Colors.White).Bold();
                        header.Cell().Background(Theme.TableHeaderBgColor).Padding(5).Text("Thời gian").FontColor(Colors.White).Bold();
                        header.Cell().Background(Theme.TableHeaderBgColor).Padding(5).AlignCenter().Text("SL").FontColor(Colors.White).Bold();
                        header.Cell().Background(Theme.TableHeaderBgColor).Padding(5).AlignRight().Text("Đơn giá").FontColor(Colors.White).Bold();
                        header.Cell().Background(Theme.TableHeaderBgColor).Padding(5).AlignRight().Text("Thành tiền").FontColor(Colors.White).Bold();
                    });

                    int stt = 1;
                    foreach (var dv in Model.DanhSachDichVu)
                    {
                        string bg = (stt % 2 == 0) ? Theme.TableAlternateBgColor : Colors.White;
                        decimal thanhTienDv = dv.DonGia * dv.SoLuong;

                        table.Cell().Background(bg).BorderBottom(1).BorderColor(Theme.BorderColor).Padding(5).AlignCenter().Text(stt.ToString());
                        table.Cell().Background(bg).BorderBottom(1).BorderColor(Theme.BorderColor).Padding(5).Text(dv.TenDichVu).SemiBold();
                        table.Cell().Background(bg).BorderBottom(1).BorderColor(Theme.BorderColor).Padding(5).Text(dv.ThoiGian.ToString("dd/MM/yyyy HH:mm")).FontSize(Theme.SmallFontSize);
                        table.Cell().Background(bg).BorderBottom(1).BorderColor(Theme.BorderColor).Padding(5).AlignCenter().Text(dv.SoLuong.ToString());
                        table.Cell().Background(bg).BorderBottom(1).BorderColor(Theme.BorderColor).Padding(5).AlignRight().Text($"{dv.DonGia:N0} đ");
                        table.Cell().Background(bg).BorderBottom(1).BorderColor(Theme.BorderColor).Padding(5).AlignRight().Text($"{thanhTienDv:N0} đ").SemiBold();

                        stt++;
                    }
                });
            });
        }

        private void ComposeSummaryBox(IContainer container)
        {
            container.Row(row =>
            {
                // Cột ghi chú & phương thức bên trái
                row.RelativeItem(5).Column(col =>
                {
                    col.Item().Text("GHI CHÚ THANH TOÁN").Bold().FontSize(Theme.SmallFontSize).FontColor(Theme.PrimaryColor);
                    
                    string phuongThuc = !string.IsNullOrWhiteSpace(Model.PhuongThucThanhtoan)
                        ? Model.PhuongThucThanhtoan
                        : "Chưa xác định";

                    col.Item().PaddingTop(2).Text(t =>
                    {
                        t.Span("Hình thức: ").SemiBold();
                        t.Span(phuongThuc);
                    });

                    col.Item().Text(t =>
                    {
                        t.Span("Trạng thái: ").SemiBold();
                        t.Span(Model.TrangThaiThanhtoan);
                    });

                    col.Item().PaddingTop(5).Text("(*) Hóa đơn đã bao gồm phí phục vụ và thuế GTGT theo quy định.")
                        .FontSize(Theme.SmallFontSize)
                        .Italic()
                        .FontColor(Theme.MutedTextColor);
                });

                // Khối tính tổng chi phí bên phải
                // Lưu ý nguyên tắc: Giá trị tổng tiền lấy trực tiếp từ Model.TongGia (Computed Column của SQL Server)
                row.RelativeItem(5).Border(1).BorderColor(Theme.BorderColor).Background(Theme.BackgroundColor).Padding(8).Column(col =>
                {
                    void AddSummaryLine(string label, decimal amount, bool isDiscount = false)
                    {
                        col.Item().Row(r =>
                        {
                            r.RelativeItem().Text(label);
                            string prefix = isDiscount && amount > 0 ? "- " : "";
                            r.RelativeItem().AlignRight().Text($"{prefix}{amount:N0} đ");
                        });
                    }

                    AddSummaryLine("Tiền phòng:", Model.TienPhong);
                    AddSummaryLine("Tiền dịch vụ:", Model.TienDichVu);

                    if (Model.GiamGia > 0)
                    {
                        AddSummaryLine("Giảm giá / Chiết khấu:", Model.GiamGia, isDiscount: true);
                    }

                    col.Item().PaddingVertical(4).LineHorizontal(1).LineColor(Theme.BorderColor);

                    // TỔNG TIỀN PHẢI THANH TOÁN (lấy từ Computed Column Model.TongGia)
                    col.Item().Row(r =>
                    {
                        r.RelativeItem().Text("TỔNG CỘNG:").Bold().FontSize(Theme.SubTitleFontSize).FontColor(Theme.PrimaryColor);
                        r.RelativeItem().AlignRight().Text($"{Model.TongGia:N0} đ")
                            .Bold()
                            .FontSize(Theme.SubTitleFontSize)
                            .FontColor(Theme.PrimaryColor);
                    });
                });
            });
        }

        private void ComposeSignatureSection(IContainer container)
        {
            container.Row(row =>
            {
                row.RelativeItem().AlignCenter().Column(col =>
                {
                    col.Item().Text("KHÁCH HÀNG").Bold().FontSize(Theme.BodyFontSize);
                    col.Item().Text("(Ký và ghi rõ họ tên)").FontSize(Theme.SmallFontSize).Italic().FontColor(Theme.MutedTextColor);
                    col.Item().Height(45); // Khoảng trống ký tên
                    col.Item().Text(!string.IsNullOrWhiteSpace(Model.TenKhachHang) ? Model.TenKhachHang : "").SemiBold();
                });

                row.RelativeItem().AlignCenter().Column(col =>
                {
                    col.Item().Text("LỄ TÂN THU NGÂN").Bold().FontSize(Theme.BodyFontSize);
                    col.Item().Text("(Ký và ghi rõ họ tên)").FontSize(Theme.SmallFontSize).Italic().FontColor(Theme.MutedTextColor);
                    col.Item().Height(45); // Khoảng trống ký tên
                    col.Item().Text("Người lập phiếu").SemiBold();
                });
            });
        }

        private void ComposeFooter(IContainer container)
        {
            container.Column(col =>
            {
                col.Item().LineHorizontal(1).LineColor(Theme.BorderColor);
                col.Item().PaddingTop(6).Row(row =>
                {
                    row.RelativeItem(7).Text(text =>
                    {
                        text.Span("Cảm ơn Quý khách đã lựa chọn ").FontSize(Theme.SmallFontSize);
                        text.Span(Theme.TenKhachSan).FontSize(Theme.SmallFontSize).SemiBold();
                        text.Span("! Hẹn gặp lại Quý khách.").FontSize(Theme.SmallFontSize);
                    });

                    row.RelativeItem(3).AlignRight().Text(text =>
                    {
                        text.Span("Trang ").FontSize(Theme.SmallFontSize);
                        text.CurrentPageNumber().FontSize(Theme.SmallFontSize);
                        text.Span(" / ").FontSize(Theme.SmallFontSize);
                        text.TotalPages().FontSize(Theme.SmallFontSize);
                    });
                });
            });
        }
    }
}
