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
        private ToolStripMenuItem mnuQuanLyDatPhong = null!;
        private ToolStripMenuItem mnuDatPhong = null!;
        private ToolStripMenuItem mnuCheckIn = null!;
        private ToolStripMenuItem mnuCheckOut = null!;
        private ToolStripMenuItem mnuThanhToan = null!;
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

            // --- 1. Quản lý phòng (Phòng, Loại phòng) ---
            var mnuQuanLyPhong = new ToolStripMenuItem("Quản lý phòng");
            
            var mnuPhong = new ToolStripMenuItem("Danh sách phòng");
            mnuPhong.Click += (_, _) =>
            {
                using var form = new HotelManagement.Forms.FormPhong();
                form.ShowDialog(this);
            };
            mnuQuanLyPhong.DropDownItems.Add(mnuPhong);

            var mnuLoaiPhong = new ToolStripMenuItem("Loại phòng");
            mnuLoaiPhong.Click += (_, _) =>
            {
                using var form = new HotelManagement.Forms.FormLoaiPhong();
                form.ShowDialog(this);
            };
            mnuQuanLyPhong.DropDownItems.Add(mnuLoaiPhong);

            menuChinh.Items.Add(mnuQuanLyPhong);

            // --- 2. Sơ đồ phòng (trực quan hóa trạng thái phòng) ---
            var mnuSoDoPhongTop = new ToolStripMenuItem("Sơ đồ phòng");
            mnuSoDoPhongTop.Click += (_, _) =>
            {
                using var form = new HotelManagement.Forms.FormSoDoPhong(_taiKhoanDangNhap);
                form.ShowDialog(this);
            };
            menuChinh.Items.Add(mnuSoDoPhongTop);

            // --- 3. Quản lý đặt phòng (Đặt phòng, Check-in, Check-out, Thanh toán) ---
            mnuQuanLyDatPhong = new ToolStripMenuItem("Quản lý đặt phòng");

            mnuDatPhong = new ToolStripMenuItem("Đặt phòng mới");
            mnuDatPhong.Click += (_, _) =>
            {
                using var form = new HotelManagement.Forms.FormDatPhong(_taiKhoanDangNhap);
                form.ShowDialog(this);
            };
            mnuQuanLyDatPhong.DropDownItems.Add(mnuDatPhong);

            mnuCheckIn = new ToolStripMenuItem("Check-in (Nhận phòng)");
            mnuCheckIn.Click += (_, _) =>
            {
                using var form = new HotelManagement.Forms.FormCheckIn(_taiKhoanDangNhap);
                form.ShowDialog(this);
            };
            mnuQuanLyDatPhong.DropDownItems.Add(mnuCheckIn);

            mnuCheckOut = new ToolStripMenuItem("Check-out (Trả phòng)");
            mnuCheckOut.Click += (_, _) =>
            {
                using var form = new HotelManagement.Forms.FormCheckOut(_taiKhoanDangNhap);
                form.ShowDialog(this);
            };
            mnuQuanLyDatPhong.DropDownItems.Add(mnuCheckOut);

            mnuThanhToan = new ToolStripMenuItem("Thanh toán hóa đơn");
            mnuThanhToan.Click += (_, _) =>
            {
                using var form = new HotelManagement.Forms.FormThanhToan();
                form.ShowDialog(this);
            };
            mnuQuanLyDatPhong.DropDownItems.Add(mnuThanhToan);

            menuChinh.Items.Add(mnuQuanLyDatPhong);

            // --- 4. Quản lý khách hàng (tất cả vai trò đều thấy) ---
            var mnuKhachHang = new ToolStripMenuItem("Quản lý khách hàng");
            mnuKhachHang.Click += (_, _) =>
            {
                using var form = new HotelManagement.Forms.FormKhachHang();
                form.ShowDialog(this);
            };
            menuChinh.Items.Add(mnuKhachHang);

            // --- 5. Quản lý nhân sự (chỉ Admin) ---
            mnuQuanLyNhanSu = new ToolStripMenuItem("Quản lý nhân sự");
            mnuQuanLyNhanSu.Click += (_, _) =>
            {
                using var form = new HotelManagement.Forms.FormNhanSu();
                form.ShowDialog(this);
            };
            menuChinh.Items.Add(mnuQuanLyNhanSu);

            // --- 6. Báo cáo doanh thu (chỉ Admin) ---
            mnuBaoCao = new ToolStripMenuItem("Báo cáo doanh thu");
            mnuBaoCao.Click += (_, _) =>
            {
                using var form = new HotelManagement.Forms.FormBaoCaoDoanhThu(_taiKhoanDangNhap);
                form.ShowDialog(this);
            };
            menuChinh.Items.Add(mnuBaoCao);
            
            // --- 7. Đổi mật khẩu ---
            mnuDoiMatKhau = new ToolStripMenuItem("Đổi mật khẩu");
            mnuDoiMatKhau.Click += (_, _) =>
            {
                using var form = new HotelManagement.Forms.FormDoiMatKhau(_taiKhoanDangNhap.IdTaikhoan);
                form.ShowDialog(this);
            };
            menuChinh.Items.Add(mnuDoiMatKhau);
            
            // --- 8. Đăng xuất ---
            mnuDangXuat = new ToolStripMenuItem("Đăng xuất");
            mnuDangXuat.Click += MnuDangXuat_Click;
            menuChinh.Items.Add(mnuDangXuat);

            MainMenuStrip = menuChinh;
            Controls.Add(menuChinh);
        }

        #endregion
    }
}
