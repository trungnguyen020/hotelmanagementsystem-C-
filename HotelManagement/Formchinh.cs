using System.Diagnostics;
using HotelManagement.BLL.Services;
using HotelManagement.Models.Entities;
using Microsoft.Extensions.DependencyInjection;

namespace HotelManagement
{
    public partial class Formchinh : Form
    {
        /// <summary>
        /// Báo cho FormDangNhap biết người dùng chủ động đăng xuất - để phân biệt với việc
        /// đóng ứng dụng bằng nút X (hai trường hợp cần xử lý khác nhau).
        /// </summary>
        public event EventHandler? YeuCauDangXuat;

        private MenuStrip menuChinh = null!;
        private ToolStripMenuItem mnuDatPhong = null!;
        private ToolStripMenuItem mnuQuanLyNhanSu = null!;   // chỉ Admin
        private ToolStripMenuItem mnuBaoCao = null!;         // chỉ Admin
        private ToolStripMenuItem mnuDoiMatKhau = null!;
        private ToolStripMenuItem mnuDangXuat = null!;

        private readonly TaiKhoan _taiKhoanDangNhap;

        public Formchinh(TaiKhoan taiKhoanDangNhap)
        {
            _taiKhoanDangNhap = taiKhoanDangNhap;
            InitializeComponent();
            KhoiTaoGiaoDien();
        }

        private void KhoiTaoGiaoDien()
        {
            StartPosition = FormStartPosition.CenterScreen;
            ClientSize = new Size(1000, 650);
            WindowState = FormWindowState.Maximized;

            menuChinh = new MenuStrip { Dock = DockStyle.Top };

            var mnuQuanLyPhong = new ToolStripMenuItem("Quản lý phòng");
            var mnuLoaiPhong = new ToolStripMenuItem("Loại phòng");
            mnuLoaiPhong.Click += (_, _) =>
            {
                using var form = new HotelManagement.Forms.FormLoaiPhong();
                form.ShowDialog(this);
            };
            mnuQuanLyPhong.DropDownItems.Add(mnuLoaiPhong);

            menuChinh.Items.Add(mnuQuanLyPhong);

            var mnuPhong = new ToolStripMenuItem("Phòng");
            mnuPhong.Click += (_, _) =>
            {
                using var form = new HotelManagement.Forms.FormPhong();
                form.ShowDialog(this);
            };
            mnuQuanLyPhong.DropDownItems.Add(mnuPhong);

            mnuDatPhong = new ToolStripMenuItem("Đặt phòng");
            mnuDatPhong.Click += (_, _) =>
            {
                using var form = new HotelManagement.Forms.FormDatPhong(_taiKhoanDangNhap);
                form.ShowDialog(this);
            };

            // --- Quản lý khách hàng (tất cả vai trò đều thấy) ---
            var mnuKhachHang = new ToolStripMenuItem("Quản lý khách hàng");
            mnuKhachHang.Click += (_, _) =>
            {
                using var form = new HotelManagement.Forms.FormKhachHang();
                form.ShowDialog(this);
            };

            // --- Quản lý nhân sự (chỉ Admin) ---
            mnuQuanLyNhanSu = new ToolStripMenuItem("Quản lý nhân sự");
            mnuQuanLyNhanSu.Click += (_, _) =>
            {
                using var form = new HotelManagement.Forms.FormNhanSu();
                form.ShowDialog(this);
            };

            mnuBaoCao = new ToolStripMenuItem("Báo cáo doanh thu");
            
            mnuDoiMatKhau = new ToolStripMenuItem("Đổi mật khẩu");
            mnuDoiMatKhau.Click += (_, _) =>
            {
                using var form = new HotelManagement.Forms.FormDoiMatKhau(_taiKhoanDangNhap.IdTaikhoan);
                form.ShowDialog(this);
            };
            
            mnuDangXuat = new ToolStripMenuItem("Đăng xuất");
            mnuDangXuat.Click += MnuDangXuat_Click;

            menuChinh.Items.Add(mnuDatPhong);
            menuChinh.Items.Add(mnuKhachHang);
            menuChinh.Items.Add(mnuQuanLyNhanSu);
            menuChinh.Items.Add(mnuBaoCao);
            menuChinh.Items.Add(mnuDoiMatKhau);
            menuChinh.Items.Add(mnuDangXuat);

            MainMenuStrip = menuChinh;
            Controls.Add(menuChinh);

            // TODO: đối chiếu lại nếu sau này chuyển sang MDI - hiện đang là Form độc lập (non-MDI)
            bool laAdmin = _taiKhoanDangNhap.IdVaitroNavigation.TenVaitro == "Admin";
            mnuQuanLyNhanSu.Visible = laAdmin;
            mnuBaoCao.Visible = laAdmin;
            mnuDoiMatKhau.Visible = !laAdmin; // Không cho Admin đổi mật khẩu

            Text = $"Hệ thống quản lý khách sạn - Xin chào {_taiKhoanDangNhap.IdNhansuNavigation.Hoten} " +
                   $"({_taiKhoanDangNhap.IdVaitroNavigation.TenVaitro})";
        }

        private async void MnuDangXuat_Click(object? sender, EventArgs e)
        {
            try
            {
                using var scope = Program.ServiceProvider.CreateScope();
                var authService = scope.ServiceProvider.GetRequiredService<IAuthService>();
                await authService.DangXuatAsync(_taiKhoanDangNhap.IdTaikhoan);
            }
            catch (Exception ex)
            {
                // Lỗi ghi log không nên chặn người dùng đăng xuất - chỉ ghi nhận để debug sau.
                Debug.WriteLine(ex);
            }

            YeuCauDangXuat?.Invoke(this, EventArgs.Empty);
            Close();
        }
    }
}