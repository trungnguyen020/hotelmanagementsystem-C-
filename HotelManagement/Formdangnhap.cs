using System.Diagnostics;
using HotelManagement.BLL.Services;
using HotelManagement.Models.Entities;
using Microsoft.Extensions.DependencyInjection;

namespace HotelManagement
{
    public partial class Formdangnhap : Form
    {
        private Panel pnlDangNhap = null!;
        private TextBox txtTenDangNhap = null!;
        private TextBox txtMatKhau = null!;
        private Button btnDangNhap = null!;
        private Label lblThongBao = null!;

        public Formdangnhap()
        {
            InitializeComponent();
            KhoiTaoGiaoDien();
        }

        private void KhoiTaoGiaoDien()
        {
            Text = "Hệ thống quản lý khách sạn - Đăng nhập";
            StartPosition = FormStartPosition.CenterScreen;
            ClientSize = new Size(400, 260);

            pnlDangNhap = new Panel { Dock = DockStyle.Fill };

            var lblTieuDe = new Label
            {
                Text = "ĐĂNG NHẬP HỆ THỐNG",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Top,
                Height = 50
            };

            var lblTenDangNhap = new Label { Text = "Tên đăng nhập:", Location = new Point(30, 80), AutoSize = true };
            txtTenDangNhap = new TextBox { Location = new Point(150, 77), Width = 200 };

            var lblMatKhau = new Label { Text = "Mật khẩu:", Location = new Point(30, 120), AutoSize = true };
            txtMatKhau = new TextBox { Location = new Point(150, 117), Width = 200, UseSystemPasswordChar = true };

            btnDangNhap = new Button { Text = "Đăng nhập", Location = new Point(150, 160), Width = 100, Height = 32 };
            btnDangNhap.Click += BtnDangNhap_Click;

            lblThongBao = new Label
            {
                Location = new Point(30, 205),
                Width = 340,
                Height = 40,
                ForeColor = Color.Red,
                Text = string.Empty
            };

            pnlDangNhap.Controls.Add(lblTieuDe);
            pnlDangNhap.Controls.Add(lblTenDangNhap);
            pnlDangNhap.Controls.Add(txtTenDangNhap);
            pnlDangNhap.Controls.Add(lblMatKhau);
            pnlDangNhap.Controls.Add(txtMatKhau);
            pnlDangNhap.Controls.Add(btnDangNhap);
            pnlDangNhap.Controls.Add(lblThongBao);

            AcceptButton = btnDangNhap;

            Controls.Add(pnlDangNhap);
        }

        private async void BtnDangNhap_Click(object? sender, EventArgs e)
        {
            lblThongBao.Text = string.Empty;
            btnDangNhap.Enabled = false;

            try
            {
                using var scope = Program.ServiceProvider.CreateScope();
                var authService = scope.ServiceProvider.GetRequiredService<IAuthService>();

                var ketQua = await authService.DangNhapAsync(txtTenDangNhap.Text, txtMatKhau.Text);

                if (ketQua.TrangThai != TrangThaiDangNhap.ThanhCong)
                {
                    lblThongBao.Text = ketQua.ThongBao;
                    return;
                }

                MoFormChinh(ketQua.TaiKhoan!);
            }
            catch (Exception ex)
            {
                // TODO: thay bằng cơ chế ghi log lỗi thực tế ở giai đoạn hoàn thiện
                lblThongBao.Text = "Có lỗi xảy ra khi đăng nhập. Vui lòng thử lại.";
                Debug.WriteLine(ex);
            }
            finally
            {
                btnDangNhap.Enabled = true;
            }
        }

        private void MoFormChinh(TaiKhoan taiKhoan)
        {
            txtTenDangNhap.Text = string.Empty;
            txtMatKhau.Text = string.Empty;
            lblThongBao.Text = string.Empty;
            Hide();

            var formChinh = new Formchinh(taiKhoan);

            // Đăng xuất từ FormChinh: hiện lại màn hình đăng nhập, KHÔNG thoát ứng dụng.
            formChinh.YeuCauDangXuat += (s, e) => Show();

            // FormChinh bị đóng mà KHÔNG qua đăng xuất (bấm nút X) => thoát hẳn ứng dụng.
            // Nếu đã đăng xuất, FormDangNhap đã được Show() lại ở trên nên Visible sẽ là true, bỏ qua.
            formChinh.FormClosed += (s, e) =>
            {
                if (!Visible)
                    Close();
            };

            formChinh.Show();
        }
    }
}