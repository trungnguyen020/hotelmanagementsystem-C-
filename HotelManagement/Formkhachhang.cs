using HotelManagement.BLL.DTOs;
using HotelManagement.BLL.Services;
using Microsoft.Extensions.DependencyInjection;

namespace HotelManagement.Forms
{
    /// <summary>
    /// Form CRUD Khách hàng (DCKLTTQ-67) — mở từ menu "Quản lý khách hàng" của FormChinh.
    /// Dựng control bằng code trong constructor, theo đúng khuôn mẫu
    /// đã thống nhất từ FormLoaiPhong/FormPhong (không dùng Designer).
    /// Tự tạo scope DI riêng, dispose khi đóng Form.
    /// </summary>
    public class FormKhachHang : Form
    {
        private readonly IServiceScope _scope;
        private readonly IKhachHangService _khachHangService;

        private DataGridView _dgv = null!;
        private TextBox _txtTimKiem = null!;
        private Button _btnTimKiem = null!;

        private TextBox _txtTen = null!;
        private TextBox _txtCccd = null!;
        private TextBox _txtEmail = null!;
        private TextBox _txtSdt = null!;
        private ComboBox _cboGioiTinh = null!;
        private DateTimePicker _dtpNgaySinh = null!;
        private CheckBox _chkCoNgaySinh = null!;

        private Button _btnThem = null!;
        private Button _btnSua = null!;
        private Button _btnXoa = null!;
        private Button _btnLamMoi = null!;

        private int? _idDangChon;

        public FormKhachHang()
        {
            _scope = Program.ServiceProvider.CreateScope();
            _khachHangService = _scope.ServiceProvider.GetRequiredService<IKhachHangService>();

            KhoiTaoGiaoDien();

            Load += async (_, _) => await TaiDanhSachAsync();
            FormClosed += (_, _) => _scope.Dispose();
        }

        private void KhoiTaoGiaoDien()
        {
            Text = "Quản lý khách hàng";
            Width = 950;
            Height = 720;
            StartPosition = FormStartPosition.CenterParent;
            MinimumSize = new Size(850, 600);

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
                Height = 300,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            _dgv.SelectionChanged += Dgv_SelectionChanged;

            // --- Vùng nhập liệu ---
            int yNhap = 370;
            int lblWidth = 120;

            var lblTen = new Label { Text = "Tên khách hàng:", Location = new Point(12, yNhap), AutoSize = true };
            _txtTen = new TextBox { Location = new Point(lblWidth + 12, yNhap - 3), Width = 250 };

            var lblCccd = new Label { Text = "CCCD:", Location = new Point(400, yNhap), AutoSize = true };
            _txtCccd = new TextBox { Location = new Point(460, yNhap - 3), Width = 180 };

            var lblSdt = new Label { Text = "Số điện thoại:", Location = new Point(12, yNhap + 35), AutoSize = true };
            _txtSdt = new TextBox { Location = new Point(lblWidth + 12, yNhap + 32), Width = 180 };

            var lblEmail = new Label { Text = "Email:", Location = new Point(400, yNhap + 35), AutoSize = true };
            _txtEmail = new TextBox { Location = new Point(460, yNhap + 32), Width = 250 };

            var lblGioiTinh = new Label { Text = "Giới tính:", Location = new Point(12, yNhap + 70), AutoSize = true };
            _cboGioiTinh = new ComboBox
            {
                Location = new Point(lblWidth + 12, yNhap + 67),
                Width = 120,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            _cboGioiTinh.Items.AddRange(new object[] { "", "Nam", "Nữ" });
            _cboGioiTinh.SelectedIndex = 0;

            _chkCoNgaySinh = new CheckBox
            {
                Text = "Ngày sinh:",
                Location = new Point(270, yNhap + 70),
                AutoSize = true,
                Checked = false
            };
            _dtpNgaySinh = new DateTimePicker
            {
                Location = new Point(370, yNhap + 67),
                Width = 160,
                Format = DateTimePickerFormat.Short,
                Enabled = false
            };
            _chkCoNgaySinh.CheckedChanged += (_, _) => _dtpNgaySinh.Enabled = _chkCoNgaySinh.Checked;

            // --- Nút thao tác ---
            int yNut = yNhap + 115;
            _btnThem = new Button { Text = "Thêm", Location = new Point(12, yNut), Width = 100, Height = 30 };
            _btnSua = new Button { Text = "Sửa", Location = new Point(122, yNut), Width = 100, Height = 30 };
            _btnXoa = new Button { Text = "Xóa", Location = new Point(232, yNut), Width = 100, Height = 30 };
            _btnLamMoi = new Button { Text = "Làm mới", Location = new Point(342, yNut), Width = 100, Height = 30 };

            _btnThem.Click += async (_, _) => await ThemAsync();
            _btnSua.Click += async (_, _) => await SuaAsync();
            _btnXoa.Click += async (_, _) => await XoaAsync();
            _btnLamMoi.Click += (_, _) => LamMoiForm();

            Controls.AddRange(new Control[]
            {
                lblTimKiem, _txtTimKiem, _btnTimKiem,
                _dgv,
                lblTen, _txtTen, lblCccd, _txtCccd,
                lblSdt, _txtSdt, lblEmail, _txtEmail,
                lblGioiTinh, _cboGioiTinh,
                _chkCoNgaySinh, _dtpNgaySinh,
                _btnThem, _btnSua, _btnXoa, _btnLamMoi
            });
        }

        private async Task TaiDanhSachAsync()
        {
            var danhSach = await _khachHangService.GetAllAsync();
            GanDuLieuLuoi(danhSach);
        }

        private async Task TimKiemAsync()
        {
            var ketQua = await _khachHangService.TimKiemAsync(_txtTimKiem.Text);
            GanDuLieuLuoi(ketQua);
        }

        private void GanDuLieuLuoi(List<KhachHangDto> danhSach)
        {
            _dgv.DataSource = null;
            _dgv.DataSource = danhSach;

            if (_dgv.Columns["IdKhachHang"] != null) _dgv.Columns["IdKhachHang"].Visible = false;
            if (_dgv.Columns["Ten"] != null) _dgv.Columns["Ten"].HeaderText = "Tên khách hàng";
            if (_dgv.Columns["Cccd"] != null) _dgv.Columns["Cccd"].HeaderText = "CCCD";
            if (_dgv.Columns["Email"] != null) _dgv.Columns["Email"].HeaderText = "Email";
            if (_dgv.Columns["Sdt"] != null) _dgv.Columns["Sdt"].HeaderText = "Số điện thoại";
            if (_dgv.Columns["GioiTinh"] != null) _dgv.Columns["GioiTinh"].HeaderText = "Giới tính";
            if (_dgv.Columns["NgaySinh"] != null) _dgv.Columns["NgaySinh"].HeaderText = "Ngày sinh";
        }

        private void Dgv_SelectionChanged(object? sender, EventArgs e)
        {
            if (_dgv.CurrentRow?.DataBoundItem is not KhachHangDto dto) return;

            _idDangChon = dto.IdKhachHang;
            _txtTen.Text = dto.Ten;
            _txtCccd.Text = dto.Cccd;
            _txtEmail.Text = dto.Email;
            _txtSdt.Text = dto.Sdt;

            // Giới tính
            int idx = _cboGioiTinh.Items.IndexOf(dto.GioiTinh ?? "");
            _cboGioiTinh.SelectedIndex = idx >= 0 ? idx : 0;

            // Ngày sinh
            if (dto.NgaySinh.HasValue)
            {
                _chkCoNgaySinh.Checked = true;
                _dtpNgaySinh.Value = dto.NgaySinh.Value.ToDateTime(TimeOnly.MinValue);
            }
            else
            {
                _chkCoNgaySinh.Checked = false;
            }
        }

        private async Task ThemAsync()
        {
            var (success, message) = await _khachHangService.AddAsync(LayDtoTuForm(0));
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
                MessageBox.Show("Vui lòng chọn một khách hàng trong danh sách để sửa.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var (success, message) = await _khachHangService.UpdateAsync(LayDtoTuForm(_idDangChon.Value));
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
                MessageBox.Show("Vui lòng chọn một khách hàng trong danh sách để xóa.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var xacNhan = MessageBox.Show(
                "Bạn có chắc chắn muốn xóa khách hàng đã chọn?", "Xác nhận xóa",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (xacNhan != DialogResult.Yes) return;

            var (success, message) = await _khachHangService.DeleteAsync(_idDangChon.Value);
            MessageBox.Show(message, success ? "Thông báo" : "Lỗi",
                MessageBoxButtons.OK, success ? MessageBoxIcon.Information : MessageBoxIcon.Warning);

            if (success)
            {
                LamMoiForm();
                await TaiDanhSachAsync();
            }
        }

        private KhachHangDto LayDtoTuForm(int id) => new()
        {
            IdKhachHang = id,
            Ten = _txtTen.Text,
            Cccd = _txtCccd.Text,
            Email = _txtEmail.Text,
            Sdt = _txtSdt.Text,
            GioiTinh = _cboGioiTinh.SelectedItem?.ToString(),
            NgaySinh = _chkCoNgaySinh.Checked
                ? DateOnly.FromDateTime(_dtpNgaySinh.Value)
                : null
        };

        private void LamMoiForm()
        {
            _idDangChon = null;
            _txtTen.Clear();
            _txtCccd.Clear();
            _txtEmail.Clear();
            _txtSdt.Clear();
            _cboGioiTinh.SelectedIndex = 0;
            _chkCoNgaySinh.Checked = false;
            _dgv.ClearSelection();
        }

        private void InitializeComponent()
        {

        }
    }
}
