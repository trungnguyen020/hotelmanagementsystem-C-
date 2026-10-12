namespace HotelManagement
{
    partial class Formdangnhap
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

        private Panel pnlDangNhap = null!;
        private TextBox txtTenDangNhap = null!;
        private TextBox txtMatKhau = null!;
        private Button btnDangNhap = null!;
        private Label lblThongBao = null!;

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;

            this.Text = "Hệ thống quản lý khách sạn - Đăng nhập";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.ClientSize = new Size(460, 300);

            pnlDangNhap = new Panel { Dock = DockStyle.Fill };

            var lblTieuDe = new Label
            {
                Text = "ĐĂNG NHẬP HỆ THỐNG",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Top,
                Height = 60
            };

            var lblTenDangNhap = new Label { Text = "Tên đăng nhập:", Location = new Point(40, 95), AutoSize = true };
            txtTenDangNhap = new TextBox { Location = new Point(170, 92), Width = 220 };

            var lblMatKhau = new Label { Text = "Mật khẩu:", Location = new Point(40, 140), AutoSize = true };
            txtMatKhau = new TextBox { Location = new Point(170, 137), Width = 220, UseSystemPasswordChar = true };

            btnDangNhap = new Button { Text = "Đăng nhập", Location = new Point(170, 185), Width = 120, Height = 38 };
            btnDangNhap.Click += BtnDangNhap_Click;

            lblThongBao = new Label
            {
                Location = new Point(40, 235),
                Width = 380,
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

            this.AcceptButton = btnDangNhap;

            this.Controls.Add(pnlDangNhap);
        }

        #endregion
    }
}
