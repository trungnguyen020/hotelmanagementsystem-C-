using HotelManagement.BLL.DTOs;
using HotelManagement.BLL.Services;
using Microsoft.Extensions.DependencyInjection;

namespace HotelManagement.Forms
{
    /// <summary>
    /// Form CRUD Khách hàng (DCKLTTQ-67) — mở từ menu "Quản lý khách hàng" của FormChinh.
    /// Giao diện khai báo trong FormKhachHang.Designer.cs.
    /// Tự tạo scope DI riêng, dispose khi đóng Form.
    /// </summary>
    public partial class FormKhachHang : Form
    {
        private readonly IServiceScope _scope;
        private readonly IKhachHangService _khachHangService;

        private int? _idDangChon;

        public FormKhachHang()
        {
            _scope = Program.ServiceProvider.CreateScope();
            _khachHangService = _scope.ServiceProvider.GetRequiredService<IKhachHangService>();

            InitializeComponent();

            Load += async (_, _) => await TaiDanhSachAsync();
            FormClosed += (_, _) => _scope.Dispose();
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
    }
}
