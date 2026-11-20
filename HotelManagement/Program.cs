using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using HotelManagement.BLL.Services;
using HotelManagement.DAL;
using HotelManagement.DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace HotelManagement;

internal static class Program
{
    public static IServiceProvider ServiceProvider { get; private set; } = null!;

    [STAThread]
    static void Main()
    {
        // 1. Khởi tạo Serilog ghi log ra file theo ngày (DCKLTTQ-94 - Rubric H2)
        string logDirectory = Path.Combine(AppContext.BaseDirectory, "logs");
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.File(
                path: Path.Combine(logDirectory, "log-.txt"),
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 30,
                outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff} {Level:u3}] {Message:lj}{NewLine}{Exception}")
            .CreateLogger();

        Log.Information("=== Ứng dụng Hotel Management khởi động ===");

        try
        {
            // 2. Thiết lập Global Exception Handler toàn cục (DCKLTTQ-94)
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);

            // 2.1. Bắt lỗi trên UI Thread
            Application.ThreadException += (sender, e) =>
            {
                Log.Error(e.Exception, "Lỗi chưa được xử lý trên UI Thread: {Message}", e.Exception.Message);
                HienThiThongBaoLoiChoNguoiDung("Đã xảy ra lỗi trong quá trình thao tác trên giao diện. Vui lòng thử lại hoặc liên hệ quản trị viên.");
            };

            // 2.2. Bắt lỗi trên các Thread nền / ThreadPool
            AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
            {
                if (e.ExceptionObject is Exception ex)
                {
                    Log.Fatal(ex, "Lỗi nghiêm trọng chưa được xử lý ngoài UI Thread (IsTerminating: {IsTerminating}): {Message}", e.IsTerminating, ex.Message);
                }
                else
                {
                    Log.Fatal("Lỗi nghiêm trọng không xác định ngoài UI Thread (IsTerminating: {IsTerminating}): {Object}", e.IsTerminating, e.ExceptionObject);
                }

                HienThiThongBaoLoiChoNguoiDung("Hệ thống gặp sự cố ngoài dự kiến. Chi tiết sự cố đã được ghi nhận vào nhật ký log an toàn.");
            };

            // 2.3. Bắt lỗi trong Task không được await đúng cách
            TaskScheduler.UnobservedTaskException += (sender, e) =>
            {
                Log.Error(e.Exception, "Lỗi tác vụ nền không được quan sát (Unobserved Task Exception): {Message}", e.Exception.Message);
                e.SetObserved(); // Ngăn ngừa việc đứt gãy tiến trình đột ngột
            };

            ApplicationConfiguration.Initialize();

            // 3. Khởi tạo cấu hình (appsettings + User Secrets)
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
                .AddUserSecrets(System.Reflection.Assembly.GetExecutingAssembly(), optional: true)
                .Build();

            var services = new ServiceCollection();

            services.AddSingleton<IConfiguration>(configuration);
            services.AddDbContext<HotelManagementDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("HotelManagementDB")));
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IDatPhongRepository, DatPhongRepository>();
            services.AddScoped<ITaiKhoanRepository, TaiKhoanRepository>();
            services.AddScoped<ILichSuGuiThongBaoRepository, LichSuGuiThongBaoRepository>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<HotelManagement.BLL.Services.ILoaiPhongService,
                        HotelManagement.BLL.Services.LoaiPhongService>();
            services.AddScoped<HotelManagement.BLL.Services.IPhongService,
                        HotelManagement.BLL.Services.PhongService>();
            services.AddScoped<HotelManagement.BLL.Services.IKhachHangService,
                        HotelManagement.BLL.Services.KhachHangService>();
            services.AddScoped<HotelManagement.BLL.Services.INhanSuService,
                        HotelManagement.BLL.Services.NhanSuService>();
            services.AddScoped<HotelManagement.BLL.Services.IDichVuService,
                        HotelManagement.BLL.Services.DichVuService>();
            services.AddScoped<IDatPhongService, DatPhongService>();
            services.AddScoped<IHoaDonService, HoaDonService>();
            services.AddScoped<IHoaDonPdfService, HoaDonPdfService>();
            services.AddScoped<IBaoCaoService, BaoCaoService>();
            services.AddScoped<IBaoCaoExcelService, BaoCaoExcelService>();
            services.AddScoped<IPhongImportService, PhongImportService>();
            services.AddScoped<IQrCodeService, QrCodeService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IDashboardService, DashboardService>();
            services.AddScoped<IKioskService, KioskService>();
            services.AddScoped<IChatbotService, ChatbotService>();

            ServiceProvider = services.BuildServiceProvider();

            // 4. Kiểm tra kết nối CSDL ngay khi khởi động
            using (var scope = ServiceProvider.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<HotelManagementDbContext>();
                if (!db.Database.CanConnect())
                {
                    Log.Fatal("Không thể kết nối cơ sở dữ liệu SQL Server. Vui lòng kiểm tra connection string trong User Secrets hoặc appsettings.");
                    MessageBox.Show("Không thể kết nối cơ sở dữ liệu. Vui lòng kiểm tra connection string trong User Secrets hoặc appsettings.",
                        "Lỗi kết nối", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                Log.Information("Kết nối cơ sở dữ liệu SQL Server thành công.");
            }

            Application.Run(new Formdangnhap());
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Lỗi nghiêm trọng làm dừng ứng dụng: {Message}", ex.Message);
        }
        finally
        {
            Log.Information("=== Ứng dụng Hotel Management kết thúc ===");
            Log.CloseAndFlush();
        }
    }

    private static void HienThiThongBaoLoiChoNguoiDung(string thongBao)
    {
        try
        {
            MessageBox.Show(
                thongBao,
                "Thông báo sự cố",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
        }
        catch
        {
            // Dự phòng an toàn nếu không thể hiển thị dialog
        }
    }
}
