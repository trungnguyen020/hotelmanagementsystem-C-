using HotelManagement.BLL.DTOs;
using HotelManagement.BLL.Services;
using Microsoft.Extensions.DependencyInjection;

namespace HotelManagement.Forms
{
    /// <summary>
    /// Form CRUD Nhân sự (DCKLTTQ-68) — mở từ menu "Quản lý nhân sự" của FormChinh.
    /// Giao diện khai báo trong FormNhanSu.Designer.cs.
    /// Tự tạo scope DI riêng, dispose khi đóng Form.
    /// Tính năng bổ sung: checkbox "Tạo tài khoản Lễ tân" — khi tick sẽ hiện thêm
    /// ô Tên đăng nhập và Mật khẩu, Service sẽ tạo TaiKhoan atomic cùng NhanSu.
    /// </summary>
    public partial class FormNhanSu : Form
    {
        private readonly IServiceScope _scope;
        private readonly INhanSuService _nhanSuService;

        private int? _idDangChon;

        public FormNhanSu()
        {
            _scope = Program.ServiceProvider.CreateScope();
            _nhanSuService = _scope.ServiceProvider.GetRequiredService<INhanSuService>();

            InitializeComponent();

            Load += async (_, _) => await TaiDanhSachAsync();
            FormClosed += (_, _) => _scope.Dispose();
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
    }
}
