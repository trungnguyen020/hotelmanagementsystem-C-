using HotelManagement.BLL.DTOs;
using HotelManagement.BLL.Services;
using Microsoft.Extensions.DependencyInjection;

namespace HotelManagement.Forms
{
    /// <summary>
    /// Form CRUD Loại phòng — mở từ menu "Quản lý phòng" của FormChinh.
    /// Giao diện khai báo trong Formloaiphong.Designer.cs.
    /// Tự tạo scope DI riêng, dispose khi đóng Form.
    /// </summary>
    public partial class FormLoaiPhong : Form
    {
        private readonly IServiceScope _scope;
        private readonly ILoaiPhongService _loaiPhongService;

        private int? _idDangChon;

        public FormLoaiPhong()
        {
            _scope = Program.ServiceProvider.CreateScope();
            _loaiPhongService = _scope.ServiceProvider.GetRequiredService<ILoaiPhongService>();

            InitializeComponent();

            Load += async (_, _) => await TaiDanhSachAsync();
            FormClosed += (_, _) => _scope.Dispose();
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
    }
}