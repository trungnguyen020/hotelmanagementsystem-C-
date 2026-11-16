using System.Data;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Text.RegularExpressions;
using HotelManagement.BLL.DTOs;
using HotelManagement.BLL.Services;
using HotelManagement.Models.Entities;
using Microsoft.Extensions.DependencyInjection;

namespace HotelManagement.Forms
{
    /// <summary>
    /// DCKLTTQ-73 — Sơ đồ phòng trực quan vẽ bằng GDI+ (OnPaint / Graphics).
    /// Bố trí phòng theo tầng hoặc dạng lưới, tô màu theo 4 trạng thái chuẩn của ràng buộc CK_Phong_TrangThai:
    /// Trống / Đang ở / Đang dọn / Bảo trì.
    /// Cho phép click xem thông tin chi tiết và thao tác nhanh (Check-out, Đặt phòng, Đổi trạng thái).
    /// </summary>
    public partial class FormSoDoPhong : Form
    {
        private readonly TaiKhoan? _taiKhoanDangNhap;
        private readonly IServiceScope _scope;
        private readonly IPhongService _phongService;
        private readonly IDatPhongService _datPhongService;
        private readonly ILoaiPhongService _loaiPhongService;

        private List<PhongDto> _danhSachGoc = new();
        private PhongDto? _phongDangChon;
        private string? _maBookingCuaPhongDangChon;

        public FormSoDoPhong(TaiKhoan? taiKhoanDangNhap = null)
        {
            _taiKhoanDangNhap = taiKhoanDangNhap;
            _scope = Program.ServiceProvider.CreateScope();
            _phongService = _scope.ServiceProvider.GetRequiredService<IPhongService>();
            _datPhongService = _scope.ServiceProvider.GetRequiredService<IDatPhongService>();
            _loaiPhongService = _scope.ServiceProvider.GetRequiredService<ILoaiPhongService>();

            InitializeComponent();
            DangKySuKien();

            FormClosed += (_, _) => _scope.Dispose();
            Load += async (_, _) =>
            {
                await TaiDanhSachLoaiPhongAsync();
                await TaiDuLieuSoDoAsync();
            };
        }

        private void DangKySuKien()
        {
            // Bắt phím F5 để làm mới nhanh
            KeyPreview = true;
            KeyDown += async (s, e) =>
            {
                if (e.KeyCode == Keys.F5)
                {
                    await TaiDuLieuSoDoAsync();
                    e.Handled = true;
                }
            };

            _btnLamMoi.Click += async (s, e) => await TaiDuLieuSoDoAsync();

            _cboLocTrangThai.SelectedIndexChanged += (s, e) => ApDungBoLoc();
            _cboLocLoaiPhong.SelectedIndexChanged += (s, e) => ApDungBoLoc();
            _cboCheDoXem.SelectedIndexChanged += (s, e) => ApDungBoLoc();
            _txtTimKiem.TextChanged += (s, e) => ApDungBoLoc();

            // Sự kiện từ Canvas GDI+
            _mapCanvas.RoomClicked += async (s, phong) => await ChonPhongAsync(phong);
            _mapCanvas.RoomDoubleClicked += async (s, phong) => await XuLyDoubleClickPhongAsync(phong);
            _mapCanvas.RoomRightClicked += (s, e) => HienThiContextMenu(e.Phong, e.Location);

            // Nút thao tác chính
            _btnThaoTacChinh.Click += async (s, e) =>
            {
                if (_phongDangChon != null)
                {
                    await XuLyDoubleClickPhongAsync(_phongDangChon);
                }
            };

            // Các nút chuyển trạng thái nhanh
            _btnDoiSangTrong.Click += async (s, e) => await DoiTrangThaiPhongAsync("Trống");
            _btnDoiSangDangDon.Click += async (s, e) => await DoiTrangThaiPhongAsync("Đang dọn");
            _btnDoiSangBaoTri.Click += async (s, e) => await DoiTrangThaiPhongAsync("Bảo trì");

            // Nút mở form Quản lý phòng CRUD
            _btnMoFormPhong.Click += (s, e) =>
            {
                using var frm = new FormPhong();
                frm.ShowDialog(this);
                _ = TaiDuLieuSoDoAsync();
            };

            // Context Menu Items
            _mnuCtxCheckOut.Click += async (s, e) =>
            {
                if (_phongDangChon != null) await MoCheckOutChoPhongAsync(_phongDangChon);
            };

            _mnuCtxDatPhong.Click += (s, e) =>
            {
                if (_phongDangChon != null) MoDatPhong();
            };

            _mnuCtxSangTrong.Click += async (s, e) => await DoiTrangThaiPhongAsync("Trống");
            _mnuCtxSangDangDon.Click += async (s, e) => await DoiTrangThaiPhongAsync("Đang dọn");
            _mnuCtxSangBaoTri.Click += async (s, e) => await DoiTrangThaiPhongAsync("Bảo trì");

            _mnuCtxQuanLyPhong.Click += (s, e) =>
            {
                using var frm = new FormPhong();
                frm.ShowDialog(this);
                _ = TaiDuLieuSoDoAsync();
            };
        }

        private async Task TaiDanhSachLoaiPhongAsync()
        {
            try
            {
                var dsLoai = await _loaiPhongService.LayDanhSachAsync();
                _cboLocLoaiPhong.Items.Clear();
                _cboLocLoaiPhong.Items.Add("Tất cả loại phòng");
                foreach (var lp in dsLoai)
                {
                    _cboLocLoaiPhong.Items.Add(lp.TenLoaiPhong);
                }
                _cboLocLoaiPhong.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể tải danh sách loại phòng: " + ex.Message,
                    "Lỗi dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private async Task TaiDuLieuSoDoAsync()
        {
            try
            {
                _btnLamMoi.Enabled = false;
                _btnLamMoi.Text = "Đang tải...";

                _danhSachGoc = await _phongService.LayDanhSachAsync();

                CapNhatThanhThongKe(_danhSachGoc);
                ApDungBoLoc();

                // Nếu đang chọn một phòng, cập nhật lại trạng thái mới nhất của phòng đó
                if (_phongDangChon != null)
                {
                    var phongMoi = _danhSachGoc.FirstOrDefault(p => p.Id == _phongDangChon.Id);
                    if (phongMoi != null)
                    {
                        await ChonPhongAsync(phongMoi);
                    }
                    else
                    {
                        BoChonPhong();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải dữ liệu sơ đồ phòng: " + ex.Message,
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _btnLamMoi.Enabled = true;
                _btnLamMoi.Text = "🔄 Làm mới (F5)";
            }
        }

        private void CapNhatThanhThongKe(List<PhongDto> danhSach)
        {
            int tong = danhSach.Count;
            int trong = danhSach.Count(p => p.TrangThai == "Trống");
            int dangO = danhSach.Count(p => p.TrangThai == "Đang ở");
            int dangDon = danhSach.Count(p => p.TrangThai == "Đang dọn");
            int baoTri = danhSach.Count(p => p.TrangThai == "Bảo trì");

            double tyLe = tong > 0 ? (double)dangO / tong * 100 : 0;

            _lblStatTong.Text = $"Tổng: {tong} phòng";
            _lblStatTrong.Text = $"● Trống: {trong}";
            _lblStatDangO.Text = $"● Đang ở: {dangO}";
            _lblStatDangDon.Text = $"● Đang dọn: {dangDon}";
            _lblStatBaoTri.Text = $"● Bảo trì: {baoTri}";
            _lblStatTyLe.Text = $"● Lấp đầy: {tyLe:0.#}%";
        }

        private void ApDungBoLoc()
        {
            var ketQua = _danhSachGoc.AsEnumerable();

            // 1. Lọc theo trạng thái
            if (_cboLocTrangThai.SelectedIndex > 0)
            {
                string tt = _cboLocTrangThai.SelectedItem?.ToString() ?? "";
                ketQua = ketQua.Where(p => p.TrangThai == tt);
            }

            // 2. Lọc theo loại phòng
            if (_cboLocLoaiPhong.SelectedIndex > 0)
            {
                string lp = _cboLocLoaiPhong.SelectedItem?.ToString() ?? "";
                ketQua = ketQua.Where(p => p.TenLoaiPhong.Equals(lp, StringComparison.OrdinalIgnoreCase));
            }

            // 3. Tìm theo số phòng
            string tuKhoa = _txtTimKiem.Text.Trim();
            if (!string.IsNullOrEmpty(tuKhoa))
            {
                ketQua = ketQua.Where(p => p.SoPhong.Contains(tuKhoa, StringComparison.OrdinalIgnoreCase));
            }

            bool gomTheoTang = _cboCheDoXem.SelectedIndex == 0;
            _mapCanvas.SetData(ketQua.ToList(), gomTheoTang);
        }

        private async Task ChonPhongAsync(PhongDto phong)
        {
            _phongDangChon = phong;
            _mapCanvas.SelectedRoomId = phong.Id;
            _maBookingCuaPhongDangChon = null;

            _lblPhongSoPhong.Text = $"Phòng {phong.SoPhong}";
            _lblPhongLoaiPhong.Text = $"Loại: {(string.IsNullOrEmpty(phong.TenLoaiPhong) ? "Tiêu chuẩn" : phong.TenLoaiPhong)}";

            // Định dạng badge trạng thái
            _lblPhongTrangThaiBadge.Text = phong.TrangThai;
            switch (phong.TrangThai)
            {
                case "Trống":
                    _lblPhongTrangThaiBadge.BackColor = Color.FromArgb(220, 252, 231);
                    _lblPhongTrangThaiBadge.ForeColor = Color.FromArgb(22, 101, 52);
                    break;
                case "Đang ở":
                    _lblPhongTrangThaiBadge.BackColor = Color.FromArgb(254, 226, 226);
                    _lblPhongTrangThaiBadge.ForeColor = Color.FromArgb(153, 27, 27);
                    break;
                case "Đang dọn":
                    _lblPhongTrangThaiBadge.BackColor = Color.FromArgb(254, 243, 199);
                    _lblPhongTrangThaiBadge.ForeColor = Color.FromArgb(146, 64, 14);
                    break;
                case "Bảo trì":
                    _lblPhongTrangThaiBadge.BackColor = Color.FromArgb(241, 245, 249);
                    _lblPhongTrangThaiBadge.ForeColor = Color.FromArgb(71, 85, 105);
                    break;
            }

            // Thiết lập nút thao tác chính và thông tin phụ
            _btnDoiSangTrong.Visible = false;
            _btnDoiSangDangDon.Visible = false;
            _btnDoiSangBaoTri.Visible = false;

            if (phong.TrangThai == "Đang ở")
            {
                _lblThongTinPhu.Text = "Đang tra cứu booking...";
                _btnThaoTacChinh.Text = "🛎️ Thực hiện Check-out (Trả phòng)";
                _btnThaoTacChinh.BackColor = Color.FromArgb(220, 53, 69);
                _btnThaoTacChinh.Visible = true;

                _btnDoiSangDangDon.Visible = true;
                _btnDoiSangBaoTri.Visible = true;

                // Tìm mã booking hiện tại
                _maBookingCuaPhongDangChon = await _datPhongService.LayMaBookingHienTaiCuaPhongAsync(phong.Id);
                if (!string.IsNullOrEmpty(_maBookingCuaPhongDangChon))
                {
                    _lblThongTinPhu.Text = $"Mã booking: {_maBookingCuaPhongDangChon}";
                }
                else
                {
                    _lblThongTinPhu.Text = "Không tìm thấy mã booking tương ứng.";
                }
            }
            else if (phong.TrangThai == "Trống")
            {
                _lblThongTinPhu.Text = "Phòng sẵn sàng đón khách.";
                _btnThaoTacChinh.Text = "📝 Tạo đặt phòng mới";
                _btnThaoTacChinh.BackColor = Color.FromArgb(40, 167, 69);
                _btnThaoTacChinh.Visible = true;

                _btnDoiSangDangDon.Visible = true;
                _btnDoiSangBaoTri.Visible = true;
            }
            else if (phong.TrangThai == "Đang dọn")
            {
                _lblThongTinPhu.Text = "Đang làm vệ sinh sau khi khách trả phòng.";
                _btnThaoTacChinh.Text = "✅ Đã dọn xong -> Chuyển sang Trống";
                _btnThaoTacChinh.BackColor = Color.FromArgb(230, 126, 34);
                _btnThaoTacChinh.Visible = true;

                _btnDoiSangTrong.Visible = true;
                _btnDoiSangBaoTri.Visible = true;
            }
            else if (phong.TrangThai == "Bảo trì")
            {
                _lblThongTinPhu.Text = "Phòng đang tạm ngưng phục vụ để bảo trì.";
                _btnThaoTacChinh.Text = "✅ Hoàn tất bảo trì -> Chuyển sang Trống";
                _btnThaoTacChinh.BackColor = Color.FromArgb(108, 117, 125);
                _btnThaoTacChinh.Visible = true;

                _btnDoiSangTrong.Visible = true;
                _btnDoiSangDangDon.Visible = true;
            }
        }

        private void BoChonPhong()
        {
            _phongDangChon = null;
            _maBookingCuaPhongDangChon = null;
            _mapCanvas.SelectedRoomId = null;

            _lblPhongSoPhong.Text = "Chưa chọn phòng";
            _lblPhongLoaiPhong.Text = "Loại phòng: --";
            _lblPhongTrangThaiBadge.Text = "--";
            _lblPhongTrangThaiBadge.BackColor = Color.FromArgb(241, 245, 249);
            _lblPhongTrangThaiBadge.ForeColor = Color.FromArgb(100, 116, 139);
            _lblThongTinPhu.Text = "Booking hiện tại: --";

            _btnThaoTacChinh.Visible = false;
            _btnDoiSangTrong.Visible = false;
            _btnDoiSangDangDon.Visible = false;
            _btnDoiSangBaoTri.Visible = false;
        }

        private async Task XuLyDoubleClickPhongAsync(PhongDto phong)
        {
            await ChonPhongAsync(phong);

            switch (phong.TrangThai)
            {
                case "Đang ở":
                    await MoCheckOutChoPhongAsync(phong);
                    break;
                case "Trống":
                    MoDatPhong();
                    break;
                case "Đang dọn":
                    var hoiDon = MessageBox.Show(
                        $"Bạn có muốn hoàn tất dọn dẹp và chuyển phòng {phong.SoPhong} sang trạng thái 'Trống'?",
                        "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (hoiDon == DialogResult.Yes)
                    {
                        await DoiTrangThaiPhongAsync("Trống");
                    }
                    break;
                case "Bảo trì":
                    var hoiBaoTri = MessageBox.Show(
                        $"Bạn có muốn kết thúc bảo trì và chuyển phòng {phong.SoPhong} sang trạng thái 'Trống'?",
                        "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (hoiBaoTri == DialogResult.Yes)
                    {
                        await DoiTrangThaiPhongAsync("Trống");
                    }
                    break;
            }
        }

        private async Task MoCheckOutChoPhongAsync(PhongDto phong)
        {
            if (_taiKhoanDangNhap == null)
            {
                MessageBox.Show("Không có thông tin tài khoản đăng nhập.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string? maBooking = _maBookingCuaPhongDangChon;
            if (string.IsNullOrEmpty(maBooking))
            {
                maBooking = await _datPhongService.LayMaBookingHienTaiCuaPhongAsync(phong.Id);
            }

            using var frm = new FormCheckOut(_taiKhoanDangNhap, maBooking);
            frm.ShowDialog(this);

            await TaiDuLieuSoDoAsync();
        }

        private void MoDatPhong()
        {
            if (_taiKhoanDangNhap == null)
            {
                MessageBox.Show("Không có thông tin tài khoản đăng nhập.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using var frm = new FormDatPhong(_taiKhoanDangNhap);
            frm.ShowDialog(this);

            _ = TaiDuLieuSoDoAsync();
        }

        private async Task DoiTrangThaiPhongAsync(string trangThaiMoi)
        {
            if (_phongDangChon == null)
            {
                MessageBox.Show("Vui lòng chọn một phòng cần đổi trạng thái.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (_phongDangChon.TrangThai == trangThaiMoi)
            {
                MessageBox.Show($"Phòng {_phongDangChon.SoPhong} hiện đã ở trạng thái '{trangThaiMoi}'.",
                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (_phongDangChon.TrangThai == "Đang ở" && trangThaiMoi != "Đang ở")
            {
                var xacNhan = MessageBox.Show(
                    $"Phòng {_phongDangChon.SoPhong} đang có khách ở! " +
                    $"Chuyển trạng thái sang '{trangThaiMoi}' thủ công mà không qua Check-out có thể làm lệch dữ liệu lưu trú. Bạn có chắc chắn muốn tiếp tục?",
                    "Cảnh báo nghiệp vụ", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (xacNhan != DialogResult.Yes) return;
            }

            try
            {
                await _phongService.CapNhatTrangThaiAsync(_phongDangChon.Id, trangThaiMoi);
                await TaiDuLieuSoDoAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể cập nhật trạng thái phòng: " + ex.Message,
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void HienThiContextMenu(PhongDto phong, Point screenLocation)
        {
            _ = ChonPhongAsync(phong);

            _mnuCtxCheckOut.Visible = (phong.TrangThai == "Đang ở");
            _mnuCtxDatPhong.Visible = (phong.TrangThai == "Trống");

            _mnuCtxSangTrong.Enabled = (phong.TrangThai != "Trống");
            _mnuCtxSangDangDon.Enabled = (phong.TrangThai != "Đang dọn");
            _mnuCtxSangBaoTri.Enabled = (phong.TrangThai != "Bảo trì");

            _ctxMenuPhong.Show(screenLocation);
        }
    }

    /// <summary>
    /// UserControl/Canvas vẽ sơ đồ phòng bằng GDI+ (OnPaint / Graphics).
    /// </summary>
    public class RoomMapCanvas : Panel
    {
        public event EventHandler<PhongDto>? RoomClicked;
        public event EventHandler<PhongDto>? RoomDoubleClicked;
        public event EventHandler<(PhongDto Phong, Point Location)>? RoomRightClicked;

        private List<PhongDto> _rooms = new();
        private bool _groupByFloor = true;
        private int? _hoveredRoomId;
        private int? _selectedRoomId;

        // Lưu thông tin vị trí các card để hit-test
        private readonly List<RoomRenderBounds> _renderList = new();

        [System.ComponentModel.Browsable(false)]
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public int? SelectedRoomId
        {
            get => _selectedRoomId;
            set
            {
                if (_selectedRoomId != value)
                {
                    _selectedRoomId = value;
                    Invalidate();
                }
            }
        }

        public RoomMapCanvas()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw, true);

            AutoScroll = true;
            DoubleBuffered = true;
        }

        public void SetData(List<PhongDto> rooms, bool groupByFloor)
        {
            _rooms = rooms ?? new();
            _groupByFloor = groupByFloor;
            _hoveredRoomId = null;
            RecomputeLayoutAndScroll();
            Invalidate();
        }

        protected override void OnResize(EventArgs eventargs)
        {
            base.OnResize(eventargs);
            RecomputeLayoutAndScroll();
            Invalidate();
        }

        private void RecomputeLayoutAndScroll()
        {
            _renderList.Clear();

            int clientW = Math.Max(ClientSize.Width - 30, 400);
            int cardW = 145;
            int cardH = 110;
            int gapX = 16;
            int gapY = 16;
            int startX = 15;
            int currentY = 15;

            if (_groupByFloor)
            {
                // Gom nhóm theo Tầng phân tích từ số phòng
                var floorGroups = _rooms
                    .GroupBy(p => TrichXuatTenTang(p.SoPhong))
                    .OrderBy(g => SapXepTang(g.Key))
                    .ToList();

                foreach (var group in floorGroups)
                {
                    int headerH = 38;
                    var headerBounds = new Rectangle(startX, currentY, clientW, headerH);

                    // Lưu header nếu cần vẽ
                    currentY += headerH + 12;

                    int x = startX;
                    foreach (var phong in group.OrderBy(p => p.SoPhong))
                    {
                        if (x + cardW > clientW + startX)
                        {
                            x = startX;
                            currentY += cardH + gapY;
                        }

                        var cardBounds = new Rectangle(x, currentY, cardW, cardH);
                        _renderList.Add(new RoomRenderBounds(phong, cardBounds, group.Key, headerBounds));
                        x += cardW + gapX;
                    }

                    currentY += cardH + 28;
                }
            }
            else
            {
                // Lưới phẳng tổng thể
                int x = startX;
                foreach (var phong in _rooms.OrderBy(p => p.SoPhong))
                {
                    if (x + cardW > clientW + startX)
                    {
                        x = startX;
                        currentY += cardH + gapY;
                    }

                    var cardBounds = new Rectangle(x, currentY, cardW, cardH);
                    _renderList.Add(new RoomRenderBounds(phong, cardBounds, "Tất cả", Rectangle.Empty));
                    x += cardW + gapX;
                }

                currentY += cardH + 20;
            }

            AutoScrollMinSize = new Size(clientW + 30, currentY);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            // Xử lý cuộn trang
            g.TranslateTransform(AutoScrollPosition.X, AutoScrollPosition.Y);

            if (_rooms.Count == 0)
            {
                using var brush = new SolidBrush(Color.FromArgb(148, 163, 184));
                using var font = new Font("Segoe UI", 12f, FontStyle.Italic);
                var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString("Không có phòng nào phù hợp với bộ lọc hiện tại.", font, brush,
                    new Rectangle(0, 50, Math.Max(Width, 400), 100), sf);
                return;
            }

            // Vẽ headers của các tầng
            if (_groupByFloor)
            {
                var drawnHeaders = new HashSet<string>();
                foreach (var item in _renderList)
                {
                    if (item.HeaderBounds != Rectangle.Empty && drawnHeaders.Add(item.FloorName))
                    {
                        VeHeaderTang(g, item.FloorName, item.HeaderBounds);
                    }
                }
            }

            // Vẽ từng ô phòng (Room Card)
            foreach (var item in _renderList)
            {
                bool isHovered = (_hoveredRoomId == item.Phong.Id);
                bool isSelected = (_selectedRoomId == item.Phong.Id);

                VeCardPhong(g, item.Phong, item.Bounds, isHovered, isSelected);
            }
        }

        private void VeHeaderTang(Graphics g, string floorName, Rectangle bounds)
        {
            // Nền header tầng
            using (var path = TaoDuongDanBoTron(bounds, 6))
            using (var brush = new SolidBrush(Color.FromArgb(241, 245, 249)))
            using (var pen = new Pen(Color.FromArgb(226, 232, 240), 1))
            {
                g.FillPath(brush, path);
                g.DrawPath(pen, path);
            }

            // Dải màu nhấn bên trái header
            var accentBar = new Rectangle(bounds.X + 2, bounds.Y + 6, 4, bounds.Height - 12);
            using (var brush = new SolidBrush(Color.FromArgb(37, 99, 235)))
            {
                g.FillRectangle(brush, accentBar);
            }

            // Thống kê sơ bộ phòng thuộc tầng này
            var roomsInFloor = _rooms.Where(p => TrichXuatTenTang(p.SoPhong) == floorName).ToList();
            int cTrong = roomsInFloor.Count(p => p.TrangThai == "Trống");
            int cDangO = roomsInFloor.Count(p => p.TrangThai == "Đang ở");
            int cDangDon = roomsInFloor.Count(p => p.TrangThai == "Đang dọn");
            int cBaoTri = roomsInFloor.Count(p => p.TrangThai == "Bảo trì");

            string textTitle = $"{floorName.ToUpper()}  ({roomsInFloor.Count} phòng)";
            string textStats = $"● {cTrong} Trống   •   ● {cDangO} Đang ở   •   ● {cDangDon} Đang dọn   •   ● {cBaoTri} Bảo trì";

            using (var fontTitle = new Font("Segoe UI", 10.5f, FontStyle.Bold))
            using (var brushTitle = new SolidBrush(Color.FromArgb(30, 41, 59)))
            {
                g.DrawString(textTitle, fontTitle, brushTitle, bounds.X + 15, bounds.Y + 9);
            }

            using (var fontStats = new Font("Segoe UI", 9f, FontStyle.Regular))
            using (var brushStats = new SolidBrush(Color.FromArgb(71, 85, 105)))
            {
                var sfRight = new StringFormat
                {
                    Alignment = StringAlignment.Far,
                    LineAlignment = StringAlignment.Center,
                    FormatFlags = StringFormatFlags.NoWrap
                };
                g.DrawString(textStats, fontStats, brushStats,
                    new Rectangle(bounds.Right - 550, bounds.Y, 530, bounds.Height), sfRight);
            }
        }

        private void VeCardPhong(Graphics g, PhongDto phong, Rectangle rect, bool isHovered, bool isSelected)
        {
            // Thiết lập bảng màu chuẩn xác theo 4 giá trị của ràng buộc CK_Phong_TrangThai:
            // "Trống", "Đang ở", "Đang dọn", "Bảo trì"
            Color clrCardBg;
            Color clrBorder;
            Color clrBadgeBg;
            Color clrBadgeText;
            Color clrAccentDot;

            switch (phong.TrangThai)
            {
                case "Trống":
                    clrCardBg = isHovered ? Color.FromArgb(236, 253, 245) : Color.FromArgb(240, 253, 244);
                    clrBorder = isSelected ? Color.FromArgb(16, 185, 129) : Color.FromArgb(187, 247, 208);
                    clrBadgeBg = Color.FromArgb(220, 252, 231);
                    clrBadgeText = Color.FromArgb(22, 101, 52);
                    clrAccentDot = Color.FromArgb(34, 197, 94);
                    break;
                case "Đang ở":
                    clrCardBg = isHovered ? Color.FromArgb(254, 242, 242) : Color.FromArgb(255, 245, 245);
                    clrBorder = isSelected ? Color.FromArgb(239, 68, 68) : Color.FromArgb(254, 202, 202);
                    clrBadgeBg = Color.FromArgb(254, 226, 226);
                    clrBadgeText = Color.FromArgb(153, 27, 27);
                    clrAccentDot = Color.FromArgb(239, 68, 68);
                    break;
                case "Đang dọn":
                    clrCardBg = isHovered ? Color.FromArgb(255, 251, 235) : Color.FromArgb(255, 253, 242);
                    clrBorder = isSelected ? Color.FromArgb(245, 158, 11) : Color.FromArgb(253, 230, 138);
                    clrBadgeBg = Color.FromArgb(254, 243, 199);
                    clrBadgeText = Color.FromArgb(146, 64, 14);
                    clrAccentDot = Color.FromArgb(245, 158, 11);
                    break;
                case "Bảo trì":
                default:
                    clrCardBg = isHovered ? Color.FromArgb(243, 244, 246) : Color.FromArgb(248, 250, 252);
                    clrBorder = isSelected ? Color.FromArgb(100, 116, 139) : Color.FromArgb(226, 232, 240);
                    clrBadgeBg = Color.FromArgb(241, 245, 249);
                    clrBadgeText = Color.FromArgb(71, 85, 105);
                    clrAccentDot = Color.FromArgb(148, 163, 184);
                    break;
            }

            // 1. Vẽ bóng (Drop shadow nhẹ khi hover / selected)
            if (isHovered || isSelected)
            {
                var shadowRect = new Rectangle(rect.X + 2, rect.Y + 3, rect.Width, rect.Height);
                using var shadowPath = TaoDuongDanBoTron(shadowRect, 8);
                using var shadowBrush = new SolidBrush(Color.FromArgb(25, 0, 0, 0));
                g.FillPath(shadowBrush, shadowPath);
            }

            // 2. Vẽ nền Card bo tròn
            using (var cardPath = TaoDuongDanBoTron(rect, 8))
            {
                using (var brush = new SolidBrush(clrCardBg))
                {
                    g.FillPath(brush, cardPath);
                }

                // 3. Vẽ đường viền
                int borderWidth = isSelected ? 3 : (isHovered ? 2 : 1);
                using (var pen = new Pen(isSelected ? Color.FromArgb(13, 110, 253) : clrBorder, borderWidth))
                {
                    g.DrawPath(pen, cardPath);
                }
            }

            // 4. Vẽ Chấm tròn & Huy hiệu trạng thái (Badge pill) ở góc trên
            var badgeRect = new Rectangle(rect.X + 10, rect.Y + 9, rect.Width - 20, 22);
            using (var badgePath = TaoDuongDanBoTron(badgeRect, 5))
            using (var badgeBrush = new SolidBrush(clrBadgeBg))
            {
                g.FillPath(badgeBrush, badgePath);
            }

            // Chấm tròn trạng thái
            using (var dotBrush = new SolidBrush(clrAccentDot))
            {
                g.FillEllipse(dotBrush, badgeRect.X + 7, badgeRect.Y + 7, 8, 8);
            }

            // Chữ trạng thái trên badge
            using (var badgeFont = new Font("Segoe UI", 8.2f, FontStyle.Bold))
            using (var badgeTextBrush = new SolidBrush(clrBadgeText))
            {
                var sfBadge = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };
                g.DrawString(phong.TrangThai, badgeFont, badgeTextBrush,
                    new Rectangle(badgeRect.X + 12, badgeRect.Y, badgeRect.Width - 12, badgeRect.Height), sfBadge);
            }

            // 5. Vẽ Số phòng (SoPhong) thật to, rõ nét ở giữa
            using (var roomFont = new Font("Segoe UI", 16f, FontStyle.Bold))
            using (var roomBrush = new SolidBrush(Color.FromArgb(15, 23, 42)))
            {
                var sfCenter = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };
                g.DrawString(phong.SoPhong, roomFont, roomBrush,
                    new Rectangle(rect.X, rect.Y + 34, rect.Width, 38), sfCenter);
            }

            // 6. Vẽ Tên loại phòng (TenLoaiPhong) ở phía dưới
            string tenLoai = string.IsNullOrEmpty(phong.TenLoaiPhong) ? "Tiêu chuẩn" : phong.TenLoaiPhong;
            using (var typeFont = new Font("Segoe UI", 8.5f, FontStyle.Regular))
            using (var typeBrush = new SolidBrush(Color.FromArgb(100, 116, 139)))
            {
                var sfBottom = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center,
                    Trimming = StringTrimming.EllipsisCharacter
                };
                g.DrawString(tenLoai, typeFont, typeBrush,
                    new Rectangle(rect.X + 6, rect.Bottom - 30, rect.Width - 12, 22), sfBottom);
            }

            // 7. Huy hiệu chọn (Checked icon / ring) nếu đang được chọn
            if (isSelected)
            {
                using var selPen = new Pen(Color.FromArgb(13, 110, 253), 2);
                g.DrawEllipse(selPen, rect.Right - 18, rect.Top + 6, 10, 10);
                using var selBrush = new SolidBrush(Color.FromArgb(13, 110, 253));
                g.FillEllipse(selBrush, rect.Right - 16, rect.Top + 8, 6, 6);
            }
        }

        private static GraphicsPath TaoDuongDanBoTron(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            int d = radius * 2;
            if (d > rect.Width) d = rect.Width;
            if (d > rect.Height) d = rect.Height;

            var arc = new Rectangle(rect.Location, new Size(d, d));

            // Góc trên-trái
            path.AddArc(arc, 180, 90);
            // Góc trên-phải
            arc.X = rect.Right - d;
            path.AddArc(arc, 270, 90);
            // Góc dưới-phải
            arc.Y = rect.Bottom - d;
            path.AddArc(arc, 0, 90);
            // Góc dưới-trái
            arc.X = rect.Left;
            path.AddArc(arc, 90, 90);

            path.CloseFigure();
            return path;
        }

        // Tương tác chuột
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            var virtualPt = new Point(e.X - AutoScrollPosition.X, e.Y - AutoScrollPosition.Y);

            var item = _renderList.FirstOrDefault(r => r.Bounds.Contains(virtualPt));
            int? newHoverId = item?.Phong.Id;

            if (newHoverId != _hoveredRoomId)
            {
                _hoveredRoomId = newHoverId;
                Cursor = (newHoverId.HasValue) ? Cursors.Hand : Cursors.Default;
                Invalidate();
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            if (_hoveredRoomId.HasValue)
            {
                _hoveredRoomId = null;
                Cursor = Cursors.Default;
                Invalidate();
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            var virtualPt = new Point(e.X - AutoScrollPosition.X, e.Y - AutoScrollPosition.Y);
            var item = _renderList.FirstOrDefault(r => r.Bounds.Contains(virtualPt));

            if (item != null)
            {
                if (e.Button == MouseButtons.Left)
                {
                    RoomClicked?.Invoke(this, item.Phong);
                }
                else if (e.Button == MouseButtons.Right)
                {
                    RoomRightClicked?.Invoke(this, (item.Phong, PointToScreen(e.Location)));
                }
            }
        }

        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            base.OnMouseDoubleClick(e);
            if (e.Button == MouseButtons.Left)
            {
                var virtualPt = new Point(e.X - AutoScrollPosition.X, e.Y - AutoScrollPosition.Y);
                var item = _renderList.FirstOrDefault(r => r.Bounds.Contains(virtualPt));
                if (item != null)
                {
                    RoomDoubleClicked?.Invoke(this, item.Phong);
                }
            }
        }

        /// <summary>
        /// Tách số tầng từ số phòng (ví dụ 101, 102 -> Tầng 1; 201 -> Tầng 2; 1001 -> Tầng 10; P201 -> Tầng 2).
        /// </summary>
        private static string TrichXuatTenTang(string soPhong)
        {
            if (string.IsNullOrWhiteSpace(soPhong)) return "Khu vực khác";

            var match = Regex.Match(soPhong, @"\d+");
            if (match.Success)
            {
                string digits = match.Value;
                if (digits.Length >= 3)
                {
                    // Lấy tất cả ký tự trừ 2 số cuối (vd 101 -> 1; 205 -> 2; 1002 -> 10)
                    string floorNum = digits.Substring(0, digits.Length - 2);
                    return $"Tầng {floorNum}";
                }
                else if (digits.Length == 1 || digits.Length == 2)
                {
                    return $"Tầng {digits[0]}";
                }
            }

            return "Khu vực chung";
        }

        private static int SapXepTang(string floorName)
        {
            var match = Regex.Match(floorName, @"\d+");
            if (match.Success && int.TryParse(match.Value, out int n))
            {
                return n;
            }
            return 999;
        }

        private sealed record RoomRenderBounds(PhongDto Phong, Rectangle Bounds, string FloorName, Rectangle HeaderBounds);
    }
}
