using HotelManagement.BLL.DTOs;
using HotelManagement.BLL.Services;
using Microsoft.Extensions.DependencyInjection;

namespace HotelManagement.Forms
{
    /// <summary>
    /// Form CRUD Phòng — mở từ menu "Quản lý phòng" của FormChinh.
    /// Cùng khuôn mẫu với FormLoaiPhong: dựng control bằng code, tự tạo scope DI riêng.
    /// </summary>
    public class FormPhong : Form
    {
        private readonly IServiceScope _scope;
        private readonly IPhongService _phongService;
        private readonly ILoaiPhongService _loaiPhongService;

        private DataGridView _dgv = null!;
        private TextBox _txtTimKiem = null!;
        private Button _btnTimKiem = null!;

        private TextBox _txtSoPhong = null!;
        private ComboBox _cboLoaiPhong = null!;
        private ComboBox _cboTrangThai = null!;

        private Button _btnThem = null!;
        private Button _btnSua = null!;
        private Button _btnXoa = null!;
        private Button _btnLamMoi = null!;

        private int? _idDangChon;

        public FormPhong()
        {
            _scope = Program.ServiceProvider.CreateScope();
            _phongService = _scope.ServiceProvider.GetRequiredService<IPhongService>();
            _loaiPhongService = _scope.ServiceProvider.GetRequiredService<ILoaiPhongService>();

            KhoiTaoGiaoDien();

            Load += async (_, _) =>
            {
                await TaiDanhSachLoaiPhongAsync();
                await TaiDanhSachAsync();
            };
            FormClosed += (_, _) => _scope.Dispose();
        }

        private void KhoiTaoGiaoDien()
        {
            Text = "Quản lý phòng";
            Width = 900;
            Height = 600;
            StartPosition = FormStartPosition.CenterParent;
            MinimumSize = new Size(820, 520);

            var lblTimKiem = new Label { Text = "Tìm kiếm:", Location = new Point(12, 15), AutoSize = true };
            _txtTimKiem = new TextBox { Location = new Point(80, 12), Width = 220 };
            _btnTimKiem = new Button { Text = "Tìm", Location = new Point(310, 10), Width = 100, Height = 30 };
            _btnTimKiem.Click += async (_, _) => await TimKiemAsync();

            _dgv = new DataGridView
            {
                Location = new Point(12, 45),
                Width = 860,
                Height = 340,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            _dgv.SelectionChanged += Dgv_SelectionChanged;

            int yNhap = 400;
            var lblSoPhong = new Label { Text = "Số phòng:", Location = new Point(12, yNhap), AutoSize = true };
            _txtSoPhong = new TextBox { Location = new Point(100, yNhap - 3), Width = 150 };

            var lblLoaiPhong = new Label { Text = "Loại phòng:", Location = new Point(280, yNhap), AutoSize = true };
            _cboLoaiPhong = new ComboBox
            {
                Location = new Point(370, yNhap - 3),
                Width = 220,
                DropDownStyle = ComboBoxStyle.DropDownList,
                DisplayMember = nameof(LoaiPhongDto.TenLoaiPhong),
                ValueMember = nameof(LoaiPhongDto.Id)
            };

            var lblTrangThai = new Label { Text = "Trạng thái:", Location = new Point(620, yNhap), AutoSize = true };
            _cboTrangThai = new ComboBox
            {
                Location = new Point(700, yNhap - 3),
                Width = 150,
                DropDownStyle = ComboBoxStyle.DropDownList,
                DataSource = IPhongService.DanhSachTrangThai
            };

            int yNut = yNhap + 50;
            _btnThem = new Button { Text = "Thêm", Location = new Point(12, yNut), Width = 100, Height = 30 };
            _btnSua = new Button { Text = "Sửa", Location = new Point(122, yNut), Width = 100, Height = 30 };
            _btnXoa = new Button { Text = "Xóa", Location = new Point(232, yNut), Width = 100 , Height = 30 };
            _btnLamMoi = new Button { Text = "Làm mới", Location = new Point(342, yNut), Width = 100, Height = 30 };

            _btnThem.Click += async (_, _) => await ThemAsync();
            _btnSua.Click += async (_, _) => await SuaAsync();
            _btnXoa.Click += async (_, _) => await XoaAsync();
            _btnLamMoi.Click += (_, _) => LamMoiForm();

            Controls.AddRange(new Control[]
            {
                lblTimKiem, _txtTimKiem, _btnTimKiem,
                _dgv,
                lblSoPhong, _txtSoPhong,
                lblLoaiPhong, _cboLoaiPhong,
                lblTrangThai, _cboTrangThai,
                _btnThem, _btnSua, _btnXoa, _btnLamMoi
            });
        }

        private async Task TaiDanhSachLoaiPhongAsync()
        {
            var dsLoaiPhong = await _loaiPhongService.LayDanhSachAsync();
            _cboLoaiPhong.DataSource = dsLoaiPhong;
        }

        private async Task TaiDanhSachAsync()
        {
            var danhSach = await _phongService.LayDanhSachAsync();
            GanDuLieuLuoi(danhSach);
        }

        private async Task TimKiemAsync()
        {
            var ketQua = await _phongService.TimKiemAsync(_txtTimKiem.Text);
            GanDuLieuLuoi(ketQua);
        }

        private void GanDuLieuLuoi(List<PhongDto> danhSach)
        {
            _dgv.DataSource = null;
            _dgv.DataSource = danhSach;

            if (_dgv.Columns["Id"] != null) _dgv.Columns["Id"].Visible = false;
            if (_dgv.Columns["IdLoaiPhong"] != null) _dgv.Columns["IdLoaiPhong"].Visible = false;
            if (_dgv.Columns["SoPhong"] != null) _dgv.Columns["SoPhong"].HeaderText = "Số phòng";
            if (_dgv.Columns["TenLoaiPhong"] != null) _dgv.Columns["TenLoaiPhong"].HeaderText = "Loại phòng";
            if (_dgv.Columns["TrangThai"] != null) _dgv.Columns["TrangThai"].HeaderText = "Trạng thái";
        }

        private void Dgv_SelectionChanged(object? sender, EventArgs e)
        {
            if (_dgv.CurrentRow?.DataBoundItem is not PhongDto dto) return;

            _idDangChon = dto.Id;
            _txtSoPhong.Text = dto.SoPhong;
            _cboLoaiPhong.SelectedValue = dto.IdLoaiPhong;
            _cboTrangThai.SelectedItem = dto.TrangThai;
        }

        private async Task ThemAsync()
        {
            try
            {
                await _phongService.ThemAsync(LayDtoTuForm(0));
                MessageBox.Show("Thêm phòng thành công.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                LamMoiForm();
                await TaiDanhSachAsync();
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private async Task SuaAsync()
        {
            if (_idDangChon is null)
            {
                MessageBox.Show("Vui lòng chọn một phòng trong danh sách để sửa.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                await _phongService.CapNhatAsync(LayDtoTuForm(_idDangChon.Value));
                MessageBox.Show("Cập nhật phòng thành công.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                LamMoiForm();
                await TaiDanhSachAsync();
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private async Task XoaAsync()
        {
            if (_idDangChon is null)
            {
                MessageBox.Show("Vui lòng chọn một phòng trong danh sách để xóa.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var xacNhan = MessageBox.Show(
                "Bạn có chắc chắn muốn xóa phòng đã chọn?", "Xác nhận xóa",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (xacNhan != DialogResult.Yes) return;

            try
            {
                await _phongService.XoaAsync(_idDangChon.Value);
                MessageBox.Show("Xóa phòng thành công.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                LamMoiForm();
                await TaiDanhSachAsync();
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private PhongDto LayDtoTuForm(int id) => new()
        {
            Id = id,
            SoPhong = _txtSoPhong.Text,
            IdLoaiPhong = _cboLoaiPhong.SelectedValue is int idLoaiPhong ? idLoaiPhong : 0,
            TrangThai = _cboTrangThai.SelectedItem as string ?? IPhongService.DanhSachTrangThai[0]
        };

        private void LamMoiForm()
        {
            _idDangChon = null;
            _txtSoPhong.Clear();
            if (_cboLoaiPhong.Items.Count > 0) _cboLoaiPhong.SelectedIndex = 0;
            _cboTrangThai.SelectedIndex = 0;
            _dgv.ClearSelection();
        }

        private void InitializeComponent()
        {

        }
    }
}