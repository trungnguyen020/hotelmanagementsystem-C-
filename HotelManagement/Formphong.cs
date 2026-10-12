using HotelManagement.BLL.DTOs;
using HotelManagement.BLL.Services;
using Microsoft.Extensions.DependencyInjection;

namespace HotelManagement.Forms
{
    /// <summary>
    /// Form CRUD Phòng — mở từ menu "Quản lý phòng" của FormChinh.
    /// Giao diện khai báo trong Formphong.Designer.cs, tự tạo scope DI riêng,
    /// dispose khi đóng Form.
    /// </summary>
    public partial class FormPhong : Form
    {
        private readonly IServiceScope _scope;
        private readonly IPhongService _phongService;
        private readonly ILoaiPhongService _loaiPhongService;

        private int? _idDangChon;

        public FormPhong()
        {
            _scope = Program.ServiceProvider.CreateScope();
            _phongService = _scope.ServiceProvider.GetRequiredService<IPhongService>();
            _loaiPhongService = _scope.ServiceProvider.GetRequiredService<ILoaiPhongService>();

            InitializeComponent();

            Load += async (_, _) =>
            {
                await TaiDanhSachLoaiPhongAsync();
                await TaiDanhSachAsync();
            };
            FormClosed += (_, _) => _scope.Dispose();
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
    }
}