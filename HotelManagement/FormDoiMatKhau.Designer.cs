using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace HotelManagement.Forms
{
    partial class FormDoiMatKhau
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private TextBox _txtMatKhauCu = null!;
        private TextBox _txtMatKhauMoi = null!;
        private TextBox _txtXacNhanMatKhau = null!;

        private Button _btnLuu = null!;
        private Button _btnHuy = null!;

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();

            Text = "Đổi mật khẩu";
            Width = 500;
            Height = 300;
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            int lblWidth = 120;
            int y = 30;

            var lblMatKhauCu = new Label { Text = "Mật khẩu cũ:", Location = new Point(30, y), AutoSize = true };
            _txtMatKhauCu = new TextBox { Location = new Point(lblWidth + 30, y - 3), Width = 220, UseSystemPasswordChar = true };

            y += 45;
            var lblMatKhauMoi = new Label { Text = "Mật khẩu mới:", Location = new Point(30, y), AutoSize = true };
            _txtMatKhauMoi = new TextBox { Location = new Point(lblWidth + 30, y - 3), Width = 220, UseSystemPasswordChar = true };

            y += 45;
            var lblXacNhan = new Label { Text = "Xác nhận MK:", Location = new Point(30, y), AutoSize = true };
            _txtXacNhanMatKhau = new TextBox { Location = new Point(lblWidth + 30, y - 3), Width = 220, UseSystemPasswordChar = true };

            y += 55;
            _btnLuu = new Button { Text = "Lưu", Location = new Point(140, y), Width = 110, Height = 35 };
            _btnLuu.Click += async (_, _) => await DoiMatKhauAsync();

            _btnHuy = new Button { Text = "Hủy", Location = new Point(270, y), Width = 110, Height = 35 };
            _btnHuy.Click += (_, _) => Close();

            Controls.AddRange(new Control[]
            {
                lblMatKhauCu, _txtMatKhauCu,
                lblMatKhauMoi, _txtMatKhauMoi,
                lblXacNhan, _txtXacNhanMatKhau,
                _btnLuu, _btnHuy
            });
        }

        #endregion
    }
}
