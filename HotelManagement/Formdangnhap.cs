using System.Diagnostics;
using HotelManagement.BLL.Services;
using HotelManagement.Models.Entities;
using Microsoft.Extensions.DependencyInjection;

namespace HotelManagement
{
    public partial class Formdangnhap : Form
    {
        public Formdangnhap()
        {
            InitializeComponent();
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