using HotelManagement.BLL.Services;
using Microsoft.Extensions.DependencyInjection;

namespace HotelManagement.Forms
{
    public partial class FormDoiMatKhau : Form
    {
        private readonly int _idTaiKhoan;
        private readonly IServiceScope _scope;
        private readonly IAuthService _authService;

        public FormDoiMatKhau(int idTaiKhoan)
        {
            _idTaiKhoan = idTaiKhoan;
            _scope = Program.ServiceProvider.CreateScope();
            _authService = _scope.ServiceProvider.GetRequiredService<IAuthService>();

            InitializeComponent();
            FormClosed += (_, _) => _scope.Dispose();
        }

        private async Task DoiMatKhauAsync()
        {
            if (string.IsNullOrWhiteSpace(_txtMatKhauCu.Text) || 
                string.IsNullOrWhiteSpace(_txtMatKhauMoi.Text) || 
                string.IsNullOrWhiteSpace(_txtXacNhanMatKhau.Text))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_txtMatKhauMoi.Text != _txtXacNhanMatKhau.Text)
            {
                MessageBox.Show("Xác nhận mật khẩu mới không khớp.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_txtMatKhauMoi.Text.Length < 6)
            {
                MessageBox.Show("Mật khẩu mới phải có ít nhất 6 ký tự.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var (success, message) = await _authService.DoiMatKhauAsync(_idTaiKhoan, _txtMatKhauCu.Text, _txtMatKhauMoi.Text);
                
                if (success)
                {
                    MessageBox.Show(message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Close();
                }
                else
                {
                    MessageBox.Show(message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                string errorMsg = ex.Message;
                if (ex.InnerException != null)
                {
                    errorMsg += "\n\nChi tiết (Inner): " + ex.InnerException.Message;
                }
                MessageBox.Show($"Có lỗi xảy ra khi lưu: {errorMsg}", "Lỗi hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
