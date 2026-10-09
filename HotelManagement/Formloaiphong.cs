using HotelManagement.BLL.DTOs;
using HotelManagement.BLL.Services;
using Microsoft.Extensions.DependencyInjection;

namespace HotelManagement.Forms
{
    /// <summary>
    /// Form CRUD Loại phòng — mở từ menu "Quản lý phòng" của FormChinh.
    /// Dựng control bằng code trong constructor, theo đúng khuôn mẫu
    /// đã thống nhất từ FormDangNhap/FormChinh (không dùng Designer).
    /// Tự tạo scope DI riêng, dispose khi đóng Form.
    /// </summary>
    public class FormLoaiPhong : Form
    {
        private readonly IServiceScope _scope;
        private readonly ILoaiPhongService _loaiPhongService;

        private DataGridView _dgv = null!;
        private TextBox _txtTimKiem = null!;
        private Button _btnTimKiem = null!;

        private TextBox _txtTen = null!;
        private NumericUpDown _numGia = null!;
        private NumericUpDown _numSucChua = null!;
        private TextBox _txtMoTa = null!;

        private Button _btnThem = null!;
        private Button _btnSua = null!;
        private Button _btnXoa = null!;
        private Button _btnLamMoi = null!;

        private int? _idDangChon;

        public FormLoaiPhong()
        {
            _scope = Program.ServiceProvider.CreateScope();
            _loaiPhongService = _scope.ServiceProvider.GetRequiredService<ILoaiPhongService>();

            KhoiTaoGiaoDien();

            Load += async (_, _) => await TaiDanhSachAsync();
            FormClosed += (_, _) => _scope.Dispose();
        }

        private void KhoiTaoGiaoDien()
        {
            Text = "Quản lý loại phòng";
            Width = 900;
            Height = 680;
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
                Height = 320,
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
            var lblTen = new Label { Text = "Tên loại phòng:", Location = new Point(12, yNhap), AutoSize = true };
            _txtTen = new TextBox { Location = new Point(130, yNhap - 3), Width = 220 };

            var lblGia = new Label { Text = "Giá (VNĐ):", Location = new Point(370, yNhap), AutoSize = true };
            _numGia = new NumericUpDown
            {
                Location = new Point(460, yNhap - 3),
                Width = 150,
                Maximum = 1_000_000_000,
                ThousandsSeparator = true
            };

            var lblSucChua = new Label { Text = "Sức chứa (người):", Location = new Point(12, yNhap + 35), AutoSize = true };
            _numSucChua = new NumericUpDown
            {
                Location = new Point(150, yNhap + 32),
                Width = 80,
                Minimum = 1,
                Maximum = 20
            };

            var lblMoTa = new Label { Text = "Mô tả:", Location = new Point(12, yNhap + 70), AutoSize = true };
            _txtMoTa = new TextBox
            {
                Location = new Point(130, yNhap + 67),
                Width = 718,
                Height = 50,
                Multiline = true
            };

            int yNut = yNhap + 130;
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
                lblTen, _txtTen, lblGia, _numGia,
                lblSucChua, _numSucChua,
                lblMoTa, _txtMoTa,
                _btnThem, _btnSua, _btnXoa, _btnLamMoi
            });
        }

        private async Task TaiDanhSachAsync()
        {
            var danhSach = await _loaiPhongService.LayDanhSachAsync();
            GanDuLieuLuoi(danhSach);
        }

        private async Task TimKiemAsync()
        {
            var ketQua = await _loaiPhongService.TimKiemAsync(_txtTimKiem.Text);
            GanDuLieuLuoi(ketQua);
        }

        private void GanDuLieuLuoi(List<LoaiPhongDto> danhSach)
        {
            _dgv.DataSource = null;
            _dgv.DataSource = danhSach;

            if (_dgv.Columns["Id"] != null) _dgv.Columns["Id"].Visible = false;
            if (_dgv.Columns["TenLoaiPhong"] != null) _dgv.Columns["TenLoaiPhong"].HeaderText = "Tên loại phòng";
            if (_dgv.Columns["Gia"] != null)
            {
                _dgv.Columns["Gia"].HeaderText = "Giá (VNĐ)";
                _dgv.Columns["Gia"].DefaultCellStyle.Format = "N0";
            }
            if (_dgv.Columns["SucChua"] != null) _dgv.Columns["SucChua"].HeaderText = "Sức chứa";
            if (_dgv.Columns["MoTa"] != null) _dgv.Columns["MoTa"].HeaderText = "Mô tả";
        }

        private void Dgv_SelectionChanged(object? sender, EventArgs e)
        {
            if (_dgv.CurrentRow?.DataBoundItem is not LoaiPhongDto dto) return;

            _idDangChon = dto.Id;
            _txtTen.Text = dto.TenLoaiPhong;
            _numGia.Value = dto.Gia;
            _numSucChua.Value = dto.SucChua;
            _txtMoTa.Text = dto.MoTa;
        }

        private async Task ThemAsync()
        {
            try
            {
                await _loaiPhongService.ThemAsync(LayDtoTuForm(0));
                MessageBox.Show("Thêm loại phòng thành công.", "Thông báo",
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
                MessageBox.Show("Vui lòng chọn một loại phòng trong danh sách để sửa.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                await _loaiPhongService.CapNhatAsync(LayDtoTuForm(_idDangChon.Value));
                MessageBox.Show("Cập nhật loại phòng thành công.", "Thông báo",
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
                MessageBox.Show("Vui lòng chọn một loại phòng trong danh sách để xóa.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var xacNhan = MessageBox.Show(
                "Bạn có chắc chắn muốn xóa loại phòng đã chọn?", "Xác nhận xóa",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (xacNhan != DialogResult.Yes) return;

            try
            {
                await _loaiPhongService.XoaAsync(_idDangChon.Value);
                MessageBox.Show("Xóa loại phòng thành công.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                LamMoiForm();
                await TaiDanhSachAsync();
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private LoaiPhongDto LayDtoTuForm(int id) => new()
        {
            Id = id,
            TenLoaiPhong = _txtTen.Text,
            Gia = _numGia.Value,
            SucChua = (int)_numSucChua.Value,
            MoTa = _txtMoTa.Text
        };

        private void LamMoiForm()
        {
            _idDangChon = null;
            _txtTen.Clear();
            _numGia.Value = 0;
            _numSucChua.Value = _numSucChua.Minimum;
            _txtMoTa.Clear();
            _dgv.ClearSelection();
        }

        private void InitializeComponent()
        {

        }
    }
}