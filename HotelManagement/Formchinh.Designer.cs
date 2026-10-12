namespace HotelManagement
{
    partial class Formchinh
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

        private MenuStrip menuChinh = null!;
        private ToolStripMenuItem mnuDatPhong = null!;
        private ToolStripMenuItem mnuCheckIn = null!;
        private ToolStripMenuItem mnuCheckOut = null!;
        private ToolStripMenuItem mnuQuanLyNhanSu = null!;   // chỉ Admin
        private ToolStripMenuItem mnuBaoCao = null!;         // chỉ Admin
        private ToolStripMenuItem mnuDoiMatKhau = null!;
        private ToolStripMenuItem mnuDangXuat = null!;

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;

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

            mnuCheckIn = new ToolStripMenuItem("Check-in");
            mnuCheckIn.Click += (_, _) =>
            {
                using var form = new HotelManagement.Forms.FormCheckIn(_taiKhoanDangNhap);
                form.ShowDialog(this);
            };

            mnuCheckOut = new ToolStripMenuItem("Check-out");
            mnuCheckOut.Click += (_, _) =>
            {
                using var form = new HotelManagement.Forms.FormCheckOut(_taiKhoanDangNhap);
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
            menuChinh.Items.Add(mnuCheckIn);
            menuChinh.Items.Add(mnuCheckOut);
            menuChinh.Items.Add(mnuKhachHang);
            menuChinh.Items.Add(mnuQuanLyNhanSu);
            menuChinh.Items.Add(mnuBaoCao);
            menuChinh.Items.Add(mnuDoiMatKhau);
            menuChinh.Items.Add(mnuDangXuat);

            MainMenuStrip = menuChinh;
            Controls.Add(menuChinh);
        }

        #endregion
    }
}
