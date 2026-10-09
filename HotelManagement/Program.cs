using HotelManagement.BLL.Services;
using HotelManagement.DAL;
using HotelManagement.DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HotelManagement;

internal static class Program
{
    public static IServiceProvider ServiceProvider { get; private set; } = null!;

    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();

        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        var services = new ServiceCollection();

        services.AddDbContext<HotelManagementDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("HotelManagementDB")));
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IDatPhongRepository, DatPhongRepository>();
        services.AddScoped<ITaiKhoanRepository, TaiKhoanRepository>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<HotelManagement.BLL.Services.ILoaiPhongService,
                    HotelManagement.BLL.Services.LoaiPhongService>();
        services.AddScoped<HotelManagement.BLL.Services.IPhongService,
                    HotelManagement.BLL.Services.PhongService>();
        services.AddScoped<HotelManagement.BLL.Services.IKhachHangService,
                    HotelManagement.BLL.Services.KhachHangService>();
        services.AddScoped<HotelManagement.BLL.Services.INhanSuService,
                    HotelManagement.BLL.Services.NhanSuService>();
        services.AddScoped<IDatPhongService, DatPhongService>();

        ServiceProvider = services.BuildServiceProvider();

        // Kiểm tra kết nối ngay khi khởi động
        using (var scope = ServiceProvider.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<HotelManagementDbContext>();
            if (!db.Database.CanConnect())
            {
                MessageBox.Show("Không thể kết nối cơ sở dữ liệu. Vui lòng kiểm tra connection string.",
                    "Lỗi kết nối", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        Application.Run(new Formdangnhap());
    }
}