using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace HotelManagement.Forms
{
    partial class FormSoDoPhong
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        // Panels
        private Panel _pnlTop = null!;
        private FlowLayoutPanel _pnlThongKe = null!;
        private FlowLayoutPanel _pnlBoLoc = null!;
        private Panel _pnlRight = null!;
        private Panel _pnlCanvasContainer = null!;
        private RoomMapCanvas _mapCanvas = null!;

        // Controls Thống kê
        private Label _lblStatTong = null!;
        private Label _lblStatTrong = null!;
        private Label _lblStatDangO = null!;
        private Label _lblStatDangDon = null!;
        private Label _lblStatBaoTri = null!;
        private Label _lblStatTyLe = null!;

        // Controls Bộ lọc
        private ComboBox _cboLocTrangThai = null!;
        private ComboBox _cboLocLoaiPhong = null!;
        private ComboBox _cboCheDoXem = null!;
        private TextBox _txtTimKiem = null!;
        private Button _btnLamMoi = null!;

        // Controls Sidebar Chi tiết & Thao tác nhanh
        private Label _lblChiTietHeader = null!;
        private Panel _pnlCardPhong = null!;
        private Label _lblPhongSoPhong = null!;
        private Label _lblPhongLoaiPhong = null!;
        private Label _lblPhongTrangThaiBadge = null!;
        private Label _lblThongTinPhu = null!;
        private Button _btnThaoTacChinh = null!;
        private Button _btnDoiSangTrong = null!;
        private Button _btnDoiSangDangDon = null!;
        private Button _btnDoiSangBaoTri = null!;
        private Button _btnMoFormPhong = null!;
        private GroupBox _grpChuThich = null!;
        private Label _lblHuongDanThaoTac = null!;

        // Context Menu
        private ContextMenuStrip _ctxMenuPhong = null!;
        private ToolStripMenuItem _mnuCtxCheckOut = null!;
        private ToolStripMenuItem _mnuCtxDatPhong = null!;
        private ToolStripSeparator _mnuCtxSep1 = null!;
        private ToolStripMenuItem _mnuCtxDoiTrangThai = null!;
        private ToolStripMenuItem _mnuCtxSangTrong = null!;
        private ToolStripMenuItem _mnuCtxSangDangDon = null!;
        private ToolStripMenuItem _mnuCtxSangBaoTri = null!;
        private ToolStripSeparator _mnuCtxSep2 = null!;
        private ToolStripMenuItem _mnuCtxQuanLyPhong = null!;

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();

            // Form cấu hình
            this.Text = "Sơ đồ phòng trực quan - Khách sạn";
            this.Size = new Size(1360, 800);
            this.MinimumSize = new Size(1100, 650);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Font = new Font("Segoe UI", 9.5f, FontStyle.Regular);
            this.BackColor = Color.FromArgb(248, 249, 250);

            // ================= 1. Top Panel: Thống kê & Bộ lọc =================
            _pnlTop = new Panel
            {
                Dock = DockStyle.Top,
                Height = 125,
                BackColor = Color.White,
                Padding = new Padding(18, 10, 18, 10)
            };

            // Thước kẻ phân cách dưới panel top
            _pnlTop.Paint += (s, e) =>
            {
                using var pen = new Pen(Color.FromArgb(226, 232, 240), 1);
                e.Graphics.DrawLine(pen, 0, _pnlTop.Height - 1, _pnlTop.Width, _pnlTop.Height - 1);
            };

            // 1.1 Panel Thống kê (Hàng trên) dùng FlowLayoutPanel tự giãn cách rộng rãi
            _pnlThongKe = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 44,
                BackColor = Color.Transparent,
                WrapContents = false,
                AutoScroll = false,
                Padding = new Padding(0)
            };

            _lblStatTong = TaoLabelThongKe("Tổng: 0 phòng", Color.FromArgb(71, 85, 105), Color.FromArgb(241, 245, 249));
            _lblStatTrong = TaoLabelThongKe("● Trống: 0", Color.FromArgb(22, 101, 52), Color.FromArgb(220, 252, 231));
            _lblStatDangO = TaoLabelThongKe("● Đang ở: 0", Color.FromArgb(153, 27, 27), Color.FromArgb(254, 226, 226));
            _lblStatDangDon = TaoLabelThongKe("● Đang dọn: 0", Color.FromArgb(146, 64, 14), Color.FromArgb(254, 243, 199));
            _lblStatBaoTri = TaoLabelThongKe("● Bảo trì: 0", Color.FromArgb(51, 65, 85), Color.FromArgb(241, 245, 249));
            _lblStatTyLe = TaoLabelThongKe("● Lấp đầy: 0%", Color.FromArgb(30, 64, 175), Color.FromArgb(224, 231, 255));

            _lblStatTong.Margin = new Padding(0, 4, 16, 4);
            _lblStatTrong.Margin = new Padding(0, 4, 16, 4);
            _lblStatDangO.Margin = new Padding(0, 4, 16, 4);
            _lblStatDangDon.Margin = new Padding(0, 4, 16, 4);
            _lblStatBaoTri.Margin = new Padding(0, 4, 16, 4);
            _lblStatTyLe.Margin = new Padding(0, 4, 0, 4);

            _pnlThongKe.Controls.AddRange(new Control[] {
                _lblStatTong, _lblStatTrong, _lblStatDangO, _lblStatDangDon, _lblStatBaoTri, _lblStatTyLe
            });

            // 1.2 Panel Bộ lọc & Tìm kiếm (Hàng dưới) dùng FlowLayoutPanel với khoảng cách lớn không bị đè
            _pnlBoLoc = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom,
                Height = 50,
                BackColor = Color.Transparent,
                WrapContents = false,
                AutoScroll = false,
                Padding = new Padding(0)
            };

            var lblLocTrangThai = new Label
            {
                Text = "Trạng thái:",
                AutoSize = true,
                Margin = new Padding(0, 10, 8, 0),
                ForeColor = Color.FromArgb(71, 85, 105)
            };
            _cboLocTrangThai = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Width = 155,
                Margin = new Padding(0, 6, 24, 0),
                Font = new Font("Segoe UI", 9.5f)
            };
            _cboLocTrangThai.Items.AddRange(new object[] { "Tất cả trạng thái", "Trống", "Đang ở", "Đang dọn", "Bảo trì" });
            _cboLocTrangThai.SelectedIndex = 0;

            var lblLocLoaiPhong = new Label
            {
                Text = "Loại phòng:",
                AutoSize = true,
                Margin = new Padding(0, 10, 8, 0),
                ForeColor = Color.FromArgb(71, 85, 105)
            };
            _cboLocLoaiPhong = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Width = 175,
                Margin = new Padding(0, 6, 24, 0),
                Font = new Font("Segoe UI", 9.5f)
            };

            var lblCheDo = new Label
            {
                Text = "Chế độ xem:",
                AutoSize = true,
                Margin = new Padding(0, 10, 8, 0),
                ForeColor = Color.FromArgb(71, 85, 105)
            };
            _cboCheDoXem = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Width = 185,
                Margin = new Padding(0, 6, 24, 0),
                Font = new Font("Segoe UI", 9.5f)
            };
            _cboCheDoXem.Items.AddRange(new object[] { "Gom nhóm theo Tầng", "Lưới phẳng tổng thể" });
            _cboCheDoXem.SelectedIndex = 0;

            var lblTimKiem = new Label
            {
                Text = "Tìm số phòng:",
                AutoSize = true,
                Margin = new Padding(0, 10, 8, 0),
                ForeColor = Color.FromArgb(71, 85, 105)
            };
            _txtTimKiem = new TextBox
            {
                Width = 145,
                Margin = new Padding(0, 6, 20, 0),
                Font = new Font("Segoe UI", 9.5f),
                PlaceholderText = "Nhập số phòng..."
            };

            _btnLamMoi = new Button
            {
                Text = "🔄 Làm mới (F5)",
                Width = 135,
                Height = 31,
                Margin = new Padding(0, 4, 0, 0),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(241, 245, 249),
                ForeColor = Color.FromArgb(30, 41, 59),
                Cursor = Cursors.Hand
            };
            _btnLamMoi.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);

            _pnlBoLoc.Controls.AddRange(new Control[] {
                lblLocTrangThai, _cboLocTrangThai,
                lblLocLoaiPhong, _cboLocLoaiPhong,
                lblCheDo, _cboCheDoXem,
                lblTimKiem, _txtTimKiem,
                _btnLamMoi
            });

            _pnlTop.Controls.Add(_pnlBoLoc);
            _pnlTop.Controls.Add(_pnlThongKe);

            // ================= 2. Right Sidebar Panel: Thông tin phòng & Thao tác nhanh =================
            _pnlRight = new Panel
            {
                Dock = DockStyle.Right,
                Width = 340,
                BackColor = Color.White,
                Padding = new Padding(15)
            };

            // Thước kẻ phân cách trái panel right
            _pnlRight.Paint += (s, e) =>
            {
                using var pen = new Pen(Color.FromArgb(226, 232, 240), 1);
                e.Graphics.DrawLine(pen, 0, 0, 0, _pnlRight.Height);
            };

            _lblChiTietHeader = new Label
            {
                Text = "THÔNG TIN & THAO TÁC",
                UseMnemonic = false,
                Font = new Font("Segoe UI", 10.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 41, 59),
                Location = new Point(15, 15),
                AutoSize = true
            };

            // Card chi tiết phòng
            _pnlCardPhong = new Panel
            {
                Location = new Point(15, 45),
                Width = 300,
                Height = 140,
                BackColor = Color.FromArgb(248, 250, 252)
            };
            _pnlCardPhong.Paint += (s, e) =>
            {
                using var pen = new Pen(Color.FromArgb(226, 232, 240), 1);
                e.Graphics.DrawRectangle(pen, 0, 0, _pnlCardPhong.Width - 1, _pnlCardPhong.Height - 1);
            };

            _lblPhongSoPhong = new Label
            {
                Text = "Chưa chọn phòng",
                Font = new Font("Segoe UI", 16f, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                Location = new Point(12, 10),
                AutoSize = true
            };

            _lblPhongTrangThaiBadge = new Label
            {
                Text = "--",
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(100, 116, 139),
                BackColor = Color.FromArgb(241, 245, 249),
                Location = new Point(15, 52),
                AutoSize = false,
                Size = new Size(110, 24),
                TextAlign = ContentAlignment.MiddleCenter
            };

            _lblPhongLoaiPhong = new Label
            {
                Text = "Loại phòng: --",
                Font = new Font("Segoe UI", 9.5f),
                ForeColor = Color.FromArgb(71, 85, 105),
                Location = new Point(15, 83),
                AutoSize = true
            };

            _lblThongTinPhu = new Label
            {
                Text = "Booking hiện tại: --",
                Font = new Font("Segoe UI", 9f, FontStyle.Italic),
                ForeColor = Color.FromArgb(100, 116, 139),
                Location = new Point(15, 108),
                AutoSize = true
            };

            _pnlCardPhong.Controls.AddRange(new Control[] {
                _lblPhongSoPhong, _lblPhongTrangThaiBadge, _lblPhongLoaiPhong, _lblThongTinPhu
            });

            // Nút thao tác chính (Context-aware button)
            _btnThaoTacChinh = new Button
            {
                Text = "🛎️ Thực hiện Check-out",
                Location = new Point(15, 200),
                Width = 300,
                Height = 44,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(220, 53, 69),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10.5f, FontStyle.Bold),
                Cursor = Cursors.Hand,
                Visible = false
            };
            _btnThaoTacChinh.FlatAppearance.BorderSize = 0;

            // Nút chuyển trạng thái nhanh
            var lblNhanh = new Label
            {
                Text = "Chuyển nhanh trạng thái phòng:",
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                ForeColor = Color.FromArgb(71, 85, 105),
                Location = new Point(15, 258),
                AutoSize = true
            };

            _btnDoiSangTrong = TaoButtonPhu("● Đánh dấu đã dọn / Sẵn sàng (Trống)", 285, Color.FromArgb(22, 101, 52), Color.FromArgb(240, 253, 244), Color.FromArgb(187, 247, 208));
            _btnDoiSangDangDon = TaoButtonPhu("● Chuyển sang Đang dọn", 328, Color.FromArgb(146, 64, 14), Color.FromArgb(254, 243, 199), Color.FromArgb(253, 230, 138));
            _btnDoiSangBaoTri = TaoButtonPhu("● Chuyển sang Bảo trì phòng", 371, Color.FromArgb(51, 65, 85), Color.FromArgb(241, 245, 249), Color.FromArgb(203, 213, 225));

            _btnMoFormPhong = new Button
            {
                Text = "⚙️ Mở danh mục Quản lý phòng",
                Location = new Point(15, 422),
                Width = 300,
                Height = 35,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(248, 250, 252),
                ForeColor = Color.FromArgb(51, 65, 85),
                Font = new Font("Segoe UI", 9f),
                Cursor = Cursors.Hand
            };
            _btnMoFormPhong.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);

            // GroupBox chú thích màu trạng thái (CK_Phong_TrangThai)
            _grpChuThich = new GroupBox
            {
                Text = "Chú thích 4 trạng thái (CK_Phong_TrangThai)",
                Location = new Point(15, 470),
                Width = 300,
                Height = 160,
                Font = new Font("Segoe UI", 8.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(71, 85, 105)
            };

            var lblLegTrong = TaoItemChuThich("● Trống", "Phòng sẵn sàng đón khách mới", 24, Color.FromArgb(22, 101, 52));
            var lblLegDangO = TaoItemChuThich("● Đang ở", "Đang có khách lưu trú", 54, Color.FromArgb(153, 27, 27));
            var lblLegDangDon = TaoItemChuThich("● Đang dọn", "Vừa check-out, đang vệ sinh", 84, Color.FromArgb(146, 64, 14));
            var lblLegBaoTri = TaoItemChuThich("● Bảo trì", "Sửa chữa hoặc dừng phục vụ", 114, Color.FromArgb(71, 85, 105));

            _grpChuThich.Controls.AddRange(new Control[] { lblLegTrong, lblLegDangO, lblLegDangDon, lblLegBaoTri });

            _lblHuongDanThaoTac = new Label
            {
                Text = "💡 Mẹo: Nhấp chuột đúp vào phòng để mở thao tác nhanh (Check-out hoặc Đặt phòng). Chuột phải để mở menu tác vụ.",
                Font = new Font("Segoe UI", 8f, FontStyle.Italic),
                ForeColor = Color.FromArgb(100, 116, 139),
                Location = new Point(15, 640),
                Width = 300,
                Height = 45
            };

            _pnlRight.Controls.AddRange(new Control[] {
                _lblChiTietHeader, _pnlCardPhong,
                _btnThaoTacChinh, lblNhanh,
                _btnDoiSangTrong, _btnDoiSangDangDon, _btnDoiSangBaoTri,
                _btnMoFormPhong, _grpChuThich, _lblHuongDanThaoTac
            });

            // ================= 3. Center: GDI+ Room Map Canvas =================
            _pnlCanvasContainer = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(248, 249, 250),
                Padding = new Padding(15)
            };

            _mapCanvas = new RoomMapCanvas
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(248, 249, 250)
            };

            _pnlCanvasContainer.Controls.Add(_mapCanvas);

            // ================= 4. Context Menu Chuột Phải =================
            _ctxMenuPhong = new ContextMenuStrip(this.components);

            _mnuCtxCheckOut = new ToolStripMenuItem("🛎️ Thực hiện Check-out...");
            _mnuCtxDatPhong = new ToolStripMenuItem("📝 Tạo đặt phòng cho phòng này...");
            _mnuCtxSep1 = new ToolStripSeparator();

            _mnuCtxDoiTrangThai = new ToolStripMenuItem("🔄 Đổi trạng thái phòng");
            _mnuCtxSangTrong = new ToolStripMenuItem("🟢 Trống (Sẵn sàng)");
            _mnuCtxSangDangDon = new ToolStripMenuItem("🟠 Đang dọn");
            _mnuCtxSangBaoTri = new ToolStripMenuItem("⚪ Bảo trì");
            _mnuCtxDoiTrangThai.DropDownItems.AddRange(new ToolStripItem[] {
                _mnuCtxSangTrong, _mnuCtxSangDangDon, _mnuCtxSangBaoTri
            });

            _mnuCtxSep2 = new ToolStripSeparator();
            _mnuCtxQuanLyPhong = new ToolStripMenuItem("⚙️ Mở quản lý phòng");

            _ctxMenuPhong.Items.AddRange(new ToolStripItem[] {
                _mnuCtxCheckOut,
                _mnuCtxDatPhong,
                _mnuCtxSep1,
                _mnuCtxDoiTrangThai,
                _mnuCtxSep2,
                _mnuCtxQuanLyPhong
            });

            // Ghép toàn bộ vào Form
            this.Controls.Add(_pnlCanvasContainer);
            this.Controls.Add(_pnlRight);
            this.Controls.Add(_pnlTop);
        }

        private static Label TaoLabelThongKe(string text, Color foreColor, Color backColor)
        {
            var lbl = new Label
            {
                Text = text,
                AutoSize = true,
                Padding = new Padding(14, 7, 14, 7),
                Font = new Font("Segoe UI", 9.2f, FontStyle.Bold),
                ForeColor = foreColor,
                BackColor = backColor
            };
            return lbl;
        }

        private static Button TaoButtonPhu(string text, int top, Color foreColor, Color backColor, Color borderColor)
        {
            var btn = new Button
            {
                Text = text,
                Location = new Point(15, top),
                Width = 300,
                Height = 35,
                FlatStyle = FlatStyle.Flat,
                BackColor = backColor,
                ForeColor = foreColor,
                Font = new Font("Segoe UI", 8.8f, FontStyle.Bold),
                Cursor = Cursors.Hand,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(8, 0, 0, 0),
                Visible = false
            };
            btn.FlatAppearance.BorderColor = borderColor;
            return btn;
        }

        private static Label TaoItemChuThich(string title, string desc, int top, Color color)
        {
            var lbl = new Label
            {
                Text = $"{title}: {desc}",
                Location = new Point(12, top),
                AutoSize = true,
                Font = new Font("Segoe UI", 8.5f),
                ForeColor = color
            };
            return lbl;
        }

        #endregion
    }
}
