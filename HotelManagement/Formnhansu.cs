using HotelManagement.BLL.DTOs;
using HotelManagement.BLL.Services;
using Microsoft.Extensions.DependencyInjection;

namespace HotelManagement.Forms
{
    /// <summary>
    /// Form CRUD Nhân sự (DCKLTTQ-68) — mở từ menu "Quản lý nhân sự" của FormChinh.
    /// Dựng control bằng code trong constructor, theo đúng khuôn mẫu FormLoaiPhong/FormPhong.
    /// Tự tạo scope DI riêng, dispose khi đóng Form.
    /// Tính năng bổ sung: checkbox "Tạo tài khoản Lễ tân" — khi tick sẽ hiện thêm
    /// ô Tên đăng nhập và Mật khẩu, Service sẽ tạo TaiKhoan atomic cùng NhanSu.
    /// </summary>
    public class FormNhanSu : Form
    {
        private readonly IServiceScope _scope;
        private readonly INhanSuService _nhanSuService;

        private DataGridView _dgv = null!;
        private TextBox _txtTimKiem = null!;
        private Button _btnTimKiem = null!;

        private TextBox _txtHoTen = null!;
        private TextBox _txtCccd = null!;
        private TextBox _txtSdt = null!;
        private DateTimePicker _dtpNgaySinh = null!;

        // --- Phần tạo tài khoản lễ tân ---
        private CheckBox _chkTaoTaiKhoan = null!;
        private Label _lblTenDangNhap = null!;
        private TextBox _txtTenDangNhap = null!;
        private Label _lblMatKhau = null!;
        private TextBox _txtMatKhau = null!;
        private Label _lblTaiKhoanInfo = null!;

        private Button _btnThem = null!;
        private Button _btnSua = null!;
        private Button _btnXoa = null!;
        private Button _btnLamMoi = null!;
        
        // --- Nút quản lý tài khoản ---
        private Button _btnDatLaiMk = null!;

        private int? _idDangChon;

        public FormNhanSu()
        {
            _scope = Program.ServiceProvider.CreateScope();
            _nhanSuService = _scope.ServiceProvider.GetRequiredService<INhanSuService>();

            KhoiTaoGiaoDien();

            Load += async (_, _) => await TaiDanhSachAsync();
            FormClosed += (_, _) => _scope.Dispose();
        }

        private void KhoiTaoGiaoDien()
        {
            Text = "Quản lý nhân sự";
            Width = 950;
            Height = 780;
            StartPosition = FormStartPosition.CenterParent;
            MinimumSize = new Size(850, 650);

            // --- Thanh tìm kiếm ---
            var lblTimKiem = new Label { Text = "Tìm kiếm:", Location = new Point(12, 15), AutoSize = true };
            _txtTimKiem = new TextBox { Location = new Point(80, 12), Width = 220 };
            _btnTimKiem = new Button { Text = "Tìm", Location = new Point(310, 10), Width = 100, Height = 30 };
            _btnTimKiem.Click += async (_, _) => await TimKiemAsync();

            // --- DataGridView ---
            _dgv = new DataGridView
            {
                Location = new Point(12, 45),
                Width = 910,
                Height = 280,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            _dgv.SelectionChanged += Dgv_SelectionChanged;

            // --- Vùng nhập liệu nhân sự ---
            int yNhap = 350;
            int lblWidth = 120;

            var lblHoTen = new Label { Text = "Họ tên:", Location = new Point(12, yNhap), AutoSize = true };
            _txtHoTen = new TextBox { Location = new Point(lblWidth + 12, yNhap - 3), Width = 250 };

            var lblCccd = new Label { Text = "CCCD:", Location = new Point(400, yNhap), AutoSize = true };
            _txtCccd = new TextBox { Location = new Point(460, yNhap - 3), Width = 180 };

            var lblSdt = new Label { Text = "Số điện thoại:", Location = new Point(12, yNhap + 35), AutoSize = true };
            _txtSdt = new TextBox { Location = new Point(lblWidth + 12, yNhap + 32), Width = 180 };

            var lblNgaySinh = new Label { Text = "Ngày sinh:", Location = new Point(400, yNhap + 35), AutoSize = true };
            _dtpNgaySinh = new DateTimePicker
            {
                Location = new Point(460, yNhap + 32),
                Width = 160,
                Format = DateTimePickerFormat.Short
            };

            // --- Phần tạo tài khoản lễ tân ---
            int yTaiKhoan = yNhap + 75;

            // Label hiển thị thông tin tài khoản hiện có (khi chọn nhân sự đã có TK)
            _lblTaiKhoanInfo = new Label
            {
                Text = "",
                Location = new Point(12, yTaiKhoan),
                AutoSize = true,
                ForeColor = System.Drawing.Color.Blue
            };

            _chkTaoTaiKhoan = new CheckBox
            {
                Text = "Tạo tài khoản Lễ tân khi thêm nhân sự",
                Location = new Point(12, yTaiKhoan + 25),
                AutoSize = true,
                Checked = false
            };
            _chkTaoTaiKhoan.CheckedChanged += (_, _) => CapNhatHienThiTaiKhoan();

            _lblTenDangNhap = new Label
            {
                Text = "Tên đăng nhập:",
                Location = new Point(12, yTaiKhoan + 55),
                AutoSize = true,
                Visible = false
            };
            _txtTenDangNhap = new TextBox
            {
                Location = new Point(lblWidth + 12, yTaiKhoan + 52),
                Width = 200,
                Visible = false
            };

            _lblMatKhau = new Label
            {
                Text = "Mật khẩu:",
                Location = new Point(400, yTaiKhoan + 55),
                AutoSize = true,
                Visible = false
            };
            _txtMatKhau = new TextBox
            {
                Location = new Point(460, yTaiKhoan + 52),
                Width = 200,
                Visible = false,
                UseSystemPasswordChar = true
            };

            // --- Nút thao tác ---
            int yNut = yTaiKhoan + 95;
            _btnThem = new Button { Text = "Thêm", Location = new Point(12, yNut), Width = 90, Height = 30 };
            _btnSua = new Button { Text = "Sửa", Location = new Point(112, yNut), Width = 90, Height = 30 };
            _btnXoa = new Button { Text = "Xóa", Location = new Point(212, yNut), Width = 90, Height = 30 };
            _btnLamMoi = new Button { Text = "Làm mới", Location = new Point(312, yNut), Width = 90, Height = 30 };

            _btnDatLaiMk = new Button { Text = "Đặt lại MK", Location = new Point(412, yNut), Width = 100, Height = 30, Visible = false };

            _btnThem.Click += async (_, _) => await ThemAsync();
            _btnSua.Click += async (_, _) => await SuaAsync();
            _btnXoa.Click += async (_, _) => await XoaAsync();
            _btnLamMoi.Click += (_, _) => LamMoiForm();
            _btnDatLaiMk.Click += async (_, _) => await DatLaiMatKhauAsync();

            Controls.AddRange(new Control[]
            {
                lblTimKiem, _txtTimKiem, _btnTimKiem,
                _dgv,
                lblHoTen, _txtHoTen, lblCccd, _txtCccd,
                lblSdt, _txtSdt, lblNgaySinh, _dtpNgaySinh,
                _lblTaiKhoanInfo,
                _chkTaoTaiKhoan,
                _lblTenDangNhap, _txtTenDangNhap,
                _lblMatKhau, _txtMatKhau,
                _btnThem, _btnSua, _btnXoa, _btnLamMoi, _btnDatLaiMk
            });
        }

        private void CapNhatHienThiTaiKhoan()
        {
            bool hienThi = _chkTaoTaiKhoan.Checked;
            _lblTenDangNhap.Visible = hienThi;
            _txtTenDangNhap.Visible = hienThi;
            _lblMatKhau.Visible = hienThi;
            _txtMatKhau.Visible = hienThi;
        }

        private async Task TaiDanhSachAsync()
        {
            var danhSach = await _nhanSuService.GetAllAsync();
            GanDuLieuLuoi(danhSach);
        }

        private async Task TimKiemAsync()
        {
            var ketQua = await _nhanSuService.TimKiemAsync(_txtTimKiem.Text);
            GanDuLieuLuoi(ketQua);
        }

        private void GanDuLieuLuoi(List<NhanSuDto> danhSach)
        {
            _dgv.DataSource = null;
            _dgv.DataSource = danhSach;

            if (_dgv.Columns["IdNhanSu"] != null) _dgv.Columns["IdNhanSu"].Visible = false;
            if (_dgv.Columns["HoTen"] != null) _dgv.Columns["HoTen"].HeaderText = "Họ tên";
            if (_dgv.Columns["Cccd"] != null) _dgv.Columns["Cccd"].HeaderText = "CCCD";
            if (_dgv.Columns["Sdt"] != null) _dgv.Columns["Sdt"].HeaderText = "Số điện thoại";
            if (_dgv.Columns["NgaySinh"] != null) _dgv.Columns["NgaySinh"].HeaderText = "Ngày sinh";
            if (_dgv.Columns["TenTaiKhoanHienThi"] != null) _dgv.Columns["TenTaiKhoanHienThi"].HeaderText = "Tài khoản";
            if (_dgv.Columns["TrangThaiTaiKhoan"] != null) _dgv.Columns["TrangThaiTaiKhoan"].HeaderText = "TT Tài khoản";

            // Ẩn các cột không cần hiển thị trên grid
            if (_dgv.Columns["TaoTaiKhoanLeTan"] != null) _dgv.Columns["TaoTaiKhoanLeTan"].Visible = false;
            if (_dgv.Columns["TenDangNhap"] != null) _dgv.Columns["TenDangNhap"].Visible = false;
            if (_dgv.Columns["MatKhau"] != null) _dgv.Columns["MatKhau"].Visible = false;
        }

        private void Dgv_SelectionChanged(object? sender, EventArgs e)
        {
            if (_dgv.CurrentRow?.DataBoundItem is not NhanSuDto dto) return;

            _idDangChon = dto.IdNhanSu;
            _txtHoTen.Text = dto.HoTen;
            _txtCccd.Text = dto.Cccd;
            _txtSdt.Text = dto.Sdt;
            _dtpNgaySinh.Value = dto.NgaySinh.ToDateTime(TimeOnly.MinValue);

            // Hiển thị thông tin tài khoản nếu đã có
            if (!string.IsNullOrEmpty(dto.TenTaiKhoanHienThi))
            {
                _lblTaiKhoanInfo.Text = $"📋 Tài khoản: {dto.TenTaiKhoanHienThi} — Trạng thái: {dto.TrangThaiTaiKhoan}";
                _chkTaoTaiKhoan.Enabled = false; // Đã có TK → không cho tick thêm
                _chkTaoTaiKhoan.Checked = false;
                
                // Ẩn nút "Đặt lại MK" nếu tài khoản đang chọn chính là Admin
                if (dto.TenTaiKhoanHienThi?.ToLower() == "admin")
                {
                    _btnDatLaiMk.Visible = false;
                }
                else
                {
                    _btnDatLaiMk.Visible = true;
                }
            }
            else
            {
                _lblTaiKhoanInfo.Text = "(Chưa có tài khoản)";
                _chkTaoTaiKhoan.Enabled = true;
                _chkTaoTaiKhoan.Checked = false;
                
                _btnDatLaiMk.Visible = false;
            }

            // Xóa thông tin tài khoản khi chọn row (vì chỉ tạo TK lúc thêm mới)
            _txtTenDangNhap.Clear();
            _txtMatKhau.Clear();
        }

        private async Task ThemAsync()
        {
            var dto = LayDtoTuForm(0);
            var (success, message) = await _nhanSuService.AddAsync(dto);
            MessageBox.Show(message, success ? "Thông báo" : "Lỗi",
                MessageBoxButtons.OK, success ? MessageBoxIcon.Information : MessageBoxIcon.Warning);

            if (success)
            {
                LamMoiForm();
                await TaiDanhSachAsync();
            }
        }

        private async Task SuaAsync()
        {
            if (_idDangChon is null)
            {
                MessageBox.Show("Vui lòng chọn một nhân sự trong danh sách để sửa.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Khi sửa, không cho phép tạo TK (chỉ sửa thông tin nhân sự)
            var dto = LayDtoTuForm(_idDangChon.Value);
            dto.TaoTaiKhoanLeTan = false; // Bỏ qua, chỉ cập nhật thông tin nhân sự

            var (success, message) = await _nhanSuService.UpdateAsync(dto);
            MessageBox.Show(message, success ? "Thông báo" : "Lỗi",
                MessageBoxButtons.OK, success ? MessageBoxIcon.Information : MessageBoxIcon.Warning);

            if (success)
            {
                LamMoiForm();
                await TaiDanhSachAsync();
            }
        }

        private async Task XoaAsync()
        {
            if (_idDangChon is null)
            {
                MessageBox.Show("Vui lòng chọn một nhân sự trong danh sách để xóa.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var xacNhan = MessageBox.Show(
                "Bạn có chắc chắn muốn xóa nhân sự đã chọn?", "Xác nhận xóa",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (xacNhan != DialogResult.Yes) return;

            var (success, message) = await _nhanSuService.DeleteAsync(_idDangChon.Value);
            MessageBox.Show(message, success ? "Thông báo" : "Lỗi",
                MessageBoxButtons.OK, success ? MessageBoxIcon.Information : MessageBoxIcon.Warning);

            if (success)
            {
                LamMoiForm();
                await TaiDanhSachAsync();
            }
        }

        private async Task DatLaiMatKhauAsync()
        {
            if (_idDangChon is null) return;

            var xacNhan = MessageBox.Show(
                "Bạn có chắc chắn muốn đặt lại mật khẩu thành 'letan123'?", "Xác nhận",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                
            if (xacNhan != DialogResult.Yes) return;

            var (success, message) = await _nhanSuService.DatLaiMatKhauTaiKhoanAsync(_idDangChon.Value, "letan123");
            MessageBox.Show(message, success ? "Thông báo" : "Lỗi",
                MessageBoxButtons.OK, success ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
        }

        private NhanSuDto LayDtoTuForm(int id) => new()
        {
            IdNhanSu = id,
            HoTen = _txtHoTen.Text,
            Cccd = _txtCccd.Text,
            Sdt = _txtSdt.Text,
            NgaySinh = DateOnly.FromDateTime(_dtpNgaySinh.Value),
            TaoTaiKhoanLeTan = _chkTaoTaiKhoan.Checked,
            TenDangNhap = _txtTenDangNhap.Text,
            MatKhau = _txtMatKhau.Text
        };

        private void LamMoiForm()
        {
            _idDangChon = null;
            _txtHoTen.Clear();
            _txtCccd.Clear();
            _txtSdt.Clear();
            _dtpNgaySinh.Value = DateTime.Today;
            _chkTaoTaiKhoan.Checked = false;
            _chkTaoTaiKhoan.Enabled = true;
            _txtTenDangNhap.Clear();
            _txtMatKhau.Clear();
            _lblTaiKhoanInfo.Text = "";
            _btnDatLaiMk.Visible = false;
            _dgv.ClearSelection();
        }

        private void InitializeComponent()
        {

        }
    }
}
