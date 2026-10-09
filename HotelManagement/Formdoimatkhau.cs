using HotelManagement.BLL.Services;
using Microsoft.Extensions.DependencyInjection;

namespace HotelManagement.Forms
{
    public class FormDoiMatKhau : Form
    {
        private readonly int _idTaiKhoan;
        private readonly IServiceScope _scope;
        private readonly IAuthService _authService;

        private TextBox _txtMatKhauCu = null!;
        private TextBox _txtMatKhauMoi = null!;
        private TextBox _txtXacNhanMatKhau = null!;
        
        private Button _btnLuu = null!;
        private Button _btnHuy = null!;

        public FormDoiMatKhau(int idTaiKhoan)
        {
            _idTaiKhoan = idTaiKhoan;
            _scope = Program.ServiceProvider.CreateScope();
            _authService = _scope.ServiceProvider.GetRequiredService<IAuthService>();

            KhoiTaoGiaoDien();
            FormClosed += (_, _) => _scope.Dispose();
        }

        private void KhoiTaoGiaoDien()
        {
            Text = "Đổi mật khẩu";
            Width = 450;
            Height = 270;
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            int lblWidth = 120;
            int y = 20;

            var lblMatKhauCu = new Label { Text = "Mật khẩu cũ:", Location = new Point(20, y), AutoSize = true };
            _txtMatKhauCu = new TextBox { Location = new Point(lblWidth + 20, y - 3), Width = 150, UseSystemPasswordChar = true };

            y += 40;
            var lblMatKhauMoi = new Label { Text = "Mật khẩu mới:", Location = new Point(20, y), AutoSize = true };
            _txtMatKhauMoi = new TextBox { Location = new Point(lblWidth + 20, y - 3), Width = 150, UseSystemPasswordChar = true };

            y += 40;
            var lblXacNhan = new Label { Text = "Xác nhận MK:", Location = new Point(20, y), AutoSize = true };
            _txtXacNhanMatKhau = new TextBox { Location = new Point(lblWidth + 20, y - 3), Width = 150, UseSystemPasswordChar = true };

            y += 50;
            _btnLuu = new Button { Text = "Lưu", Location = new Point(110, y), Width = 100, Height = 35 };
            _btnLuu.Click += async (_, _) => await DoiMatKhauAsync();

            _btnHuy = new Button { Text = "Hủy", Location = new Point(230, y), Width = 100, Height = 35 };
            _btnHuy.Click += (_, _) => Close();

            Controls.AddRange(new Control[]
            {
                lblMatKhauCu, _txtMatKhauCu,
                lblMatKhauMoi, _txtMatKhauMoi,
                lblXacNhan, _txtXacNhanMatKhau,
                _btnLuu, _btnHuy
            });
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
        
        private void InitializeComponent()
        {
        }
    }
}
