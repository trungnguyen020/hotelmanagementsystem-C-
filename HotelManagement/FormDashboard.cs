using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HotelManagement.BLL.DTOs;
using HotelManagement.BLL.Services;
using LiveChartsCore;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using Microsoft.Extensions.DependencyInjection;
using SkiaSharp;

namespace HotelManagement.Forms
{
    /// <summary>
    /// Màn hình Dashboard phân tích biểu đồ doanh thu và hiệu suất phòng (DCKLTTQ-84 - Admin only).
    /// Sử dụng thư viện LiveCharts2 với 3 loại chart: Biểu đồ Cột, Biểu đồ Tròn và Biểu đồ Đường.
    /// </summary>
    public partial class FormDashboard : Form
    {
        private readonly IServiceScope _scope;
        private readonly IDashboardService _dashboardService;

        public FormDashboard()
        {
            _scope = Program.ServiceProvider.CreateScope();
            _dashboardService = _scope.ServiceProvider.GetRequiredService<IDashboardService>();

            InitializeComponent();

            FormClosed += (_, _) => _scope.Dispose();

            // Mặc định khoảng lọc 7 ngày gần nhất
            _dtpTuNgay.Value = DateTime.Today.AddDays(-6);
            _dtpDenNgay.Value = DateTime.Today;

            Load += async (_, _) => await TaiDuLieuAsync();

            // Gán sự kiện cho các nút chọn nhanh
            _btnHomNay.Click += async (_, _) =>
            {
                _dtpTuNgay.Value = DateTime.Today;
                _dtpDenNgay.Value = DateTime.Today;
                await TaiDuLieuAsync();
            };

            _btn7Ngay.Click += async (_, _) =>
            {
                _dtpTuNgay.Value = DateTime.Today.AddDays(-6);
                _dtpDenNgay.Value = DateTime.Today;
                await TaiDuLieuAsync();
            };

            _btn30Ngay.Click += async (_, _) =>
            {
                _dtpTuNgay.Value = DateTime.Today.AddDays(-29);
                _dtpDenNgay.Value = DateTime.Today;
                await TaiDuLieuAsync();
            };

            _btnThangNay.Click += async (_, _) =>
            {
                var now = DateTime.Today;
                _dtpTuNgay.Value = new DateTime(now.Year, now.Month, 1);
                _dtpDenNgay.Value = DateTime.Today;
                await TaiDuLieuAsync();
            };

            _btnLoc.Click += async (_, _) => await TaiDuLieuAsync();
            _btnLamMoi.Click += async (_, _) => await TaiDuLieuAsync();
        }

        private async Task TaiDuLieuAsync()
        {
            try
            {
                BatTatNutDieuKhien(false);

                var filter = new DashboardFilterDto
                {
                    TuNgay = _dtpTuNgay.Value.Date,
                    DenNgay = _dtpDenNgay.Value.Date
                };

                var data = await _dashboardService.LayDuLieuDashboardAsync(filter);

                CapNhatTheKpi(data.Kpi);
                VeBieuDoCotDoanhThu(data.DoanhThuTheoNgay);
                VeBieuDoTronTrangThaiPhong(data.CoCauTrangThaiPhong);
                VeBieuDoDuongLapDay(data.XuHuongLapDayTheoNgay);
                CapNhatTomTat(data);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải dữ liệu thống kê Dashboard: " + ex.Message,
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                BatTatNutDieuKhien(true);
            }
        }

        private void BatTatNutDieuKhien(bool enabled)
        {
            _btnLoc.Enabled = enabled;
            _btnLamMoi.Enabled = enabled;
            _btnHomNay.Enabled = enabled;
            _btn7Ngay.Enabled = enabled;
            _btn30Ngay.Enabled = enabled;
            _btnThangNay.Enabled = enabled;
        }

        private void CapNhatTheKpi(DashboardKpiDto kpi)
        {
            _lblValDoanhThu.Text = kpi.TongDoanhThu.ToString("N0") + " đ";
            _lblSubDoanhThu.Text = $"Phòng: {kpi.TongTienPhong:N0} đ | DV: {kpi.TongTienDichVu:N0} đ";

            _lblValLapDay.Text = $"{kpi.TyLeLapDayTrungBinh:F1}%";
            _lblSubLapDay.Text = kpi.TyLeLapDayTrungBinh >= 70 ? "✓ Đạt hiệu suất cao" : "Cần đẩy mạnh khai thác";

            _lblValLuotDat.Text = kpi.TongSoLuotDat.ToString("N0");
            _lblSubLuotDat.Text = $"Trong kỳ {_dtpTuNgay.Value:dd/MM} - {_dtpDenNgay.Value:dd/MM}";

            _lblValHienTrang.Text = $"{kpi.SoPhongDangO} / {kpi.TongSoPhong} phòng";
            _lblSubHienTrang.Text = $"Trống sẵn sàng: {kpi.SoPhongTrong} phòng";
        }

        /// <summary>
        /// Chart 1: Biểu đồ Cột doanh thu (Tiền phòng + Tiền dịch vụ) theo ngày.
        /// </summary>
        private void VeBieuDoCotDoanhThu(List<DoanhThuTheoNgayDto> data)
        {
            var labels = data.Select(x => x.NhanNgay).ToArray();
            var tienPhong = data.Select(x => (double)x.TienPhong).ToArray();
            var tienDichVu = data.Select(x => (double)x.TienDichVu).ToArray();

            _chartDoanhThu.Series = new ISeries[]
            {
                new ColumnSeries<double>
                {
                    Name = "Tiền phòng",
                    Values = tienPhong,
                    Fill = new SolidColorPaint(new SKColor(140, 98, 57)), // Bronze
                    MaxBarWidth = 26
                },
                new ColumnSeries<double>
                {
                    Name = "Tiền dịch vụ",
                    Values = tienDichVu,
                    Fill = new SolidColorPaint(new SKColor(176, 141, 87)), // Gold
                    MaxBarWidth = 26
                }
            };

            _chartDoanhThu.XAxes = new Axis[]
            {
                new Axis
                {
                    Labels = labels,
                    TextSize = 11,
                    LabelsRotation = labels.Length > 10 ? 25 : 0
                }
            };

            _chartDoanhThu.YAxes = new Axis[]
            {
                new Axis
                {
                    Labeler = val => val >= 1_000_000 ? (val / 1_000_000.0).ToString("0.#") + " Tr" : val.ToString("N0") + " đ",
                    TextSize = 11
                }
            };

            _chartDoanhThu.LegendPosition = LegendPosition.Top;
        }

        /// <summary>
        /// Chart 2: Biểu đồ Tròn cơ cấu trạng thái phòng hiện tại.
        /// </summary>
        private void VeBieuDoTronTrangThaiPhong(List<TrangThaiPhongThongKeDto> coCau)
        {
            var seriesList = new List<ISeries>();

            foreach (var item in coCau)
            {
                SKColor mau = LayMauTrangThai(item.TrangThai);
                seriesList.Add(new PieSeries<int>
                {
                    Name = $"{item.TrangThai}: {item.SoLuong} ({item.TyLePhanTram:F1}%)",
                    Values = new int[] { item.SoLuong },
                    Fill = new SolidColorPaint(mau),
                    Pushout = 3
                });
            }

            _chartTrangThaiPhong.Series = seriesList;
            _chartTrangThaiPhong.LegendPosition = LegendPosition.Bottom;
        }

        /// <summary>
        /// Chart 3: Biểu đồ Đường tỷ lệ lấp đầy phòng (%) qua từng ngày.
        /// </summary>
        private void VeBieuDoDuongLapDay(List<TyLeLapDayTheoNgayDto> data)
        {
            var labels = data.Select(x => x.NhanNgay).ToArray();
            var tyLe = data.Select(x => x.TyLeLapDayPhanTram).ToArray();

            _chartLapDay.Series = new ISeries[]
            {
                new LineSeries<double>
                {
                    Name = "Tỷ lệ lấp đầy (%)",
                    Values = tyLe,
                    Fill = new SolidColorPaint(new SKColor(46, 204, 113, 35)),
                    Stroke = new SolidColorPaint(new SKColor(46, 204, 113)) { StrokeThickness = 3 },
                    GeometrySize = 7,
                    GeometryStroke = new SolidColorPaint(new SKColor(46, 204, 113)) { StrokeThickness = 2 },
                    GeometryFill = new SolidColorPaint(SKColors.White)
                }
            };

            _chartLapDay.XAxes = new Axis[]
            {
                new Axis
                {
                    Labels = labels,
                    TextSize = 11,
                    LabelsRotation = labels.Length > 10 ? 25 : 0
                }
            };

            _chartLapDay.YAxes = new Axis[]
            {
                new Axis
                {
                    MinLimit = 0,
                    MaxLimit = 100,
                    Labeler = val => val.ToString("0") + "%",
                    TextSize = 11
                }
            };

            _chartLapDay.LegendPosition = LegendPosition.Top;
        }

        private void CapNhatTomTat(DashboardDataDto data)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"• Khoảng thời gian: {_dtpTuNgay.Value:dd/MM/yyyy} ➔ {_dtpDenNgay.Value:dd/MM/yyyy} ({(data.Filter.DenNgay - data.Filter.TuNgay).Days + 1} ngày)");
            sb.AppendLine($"• Tổng doanh thu: {data.Kpi.TongDoanhThu:N0} đ (Tiền phòng: {data.Kpi.TongTienPhong:N0} đ | Dịch vụ: {data.Kpi.TongTienDichVu:N0} đ)");
            sb.AppendLine($"• Tỷ lệ lấp đầy bình quân: {data.Kpi.TyLeLapDayTrungBinh:F1}%");
            sb.AppendLine($"• Số đơn booking hoàn tất: {data.Kpi.TongSoLuotDat} lượt đặt");
            sb.AppendLine($"• Tình trạng phòng hiện tại: {data.Kpi.SoPhongDangO} phòng đang có khách, {data.Kpi.SoPhongTrong} phòng trống sẵn sàng.");

            // Đánh giá hiệu suất
            if (data.Kpi.TyLeLapDayTrungBinh >= 80)
            {
                sb.AppendLine("⭐ Đánh giá: Khách sạn đang vận hành với công suất rất cao (trên 80%). Nên chuẩn bị kế hoạch dọn dẹp và phân bổ nhân sự lễ tân tăng cường.");
            }
            else if (data.Kpi.TyLeLapDayTrungBinh >= 50)
            {
                sb.AppendLine("✓ Đánh giá: Công suất phòng ở mức ổn định (50% - 80%).");
            }
            else
            {
                sb.AppendLine("ℹ Đánh giá: Công suất phòng đang dưới 50%. Có thể áp dụng các chương trình ưu đãi hoặc giảm giá dịch vụ để thu hút thêm khách lưu trú.");
            }

            _lblSummaryNoiDung.Text = sb.ToString();
        }

        private SKColor LayMauTrangThai(string trangThai)
        {
            string s = trangThai.ToLowerInvariant();
            if (s.Contains("trống") || s.Contains("trong"))
                return new SKColor(46, 204, 113); // Xanh lá
            if (s.Contains("đang ở") || s.Contains("dang o"))
                return new SKColor(52, 152, 219); // Xanh dương
            if (s.Contains("dọn") || s.Contains("don"))
                return new SKColor(241, 196, 15); // Vàng
            if (s.Contains("trì") || s.Contains("tri") || s.Contains("hỏng"))
                return new SKColor(231, 76, 60);  // Đỏ

            return new SKColor(149, 165, 166);   // Xám
        }
    }
}
