using HotelManagement.BLL.DTOs;
using HotelManagement.BLL.Services;
using Microsoft.Extensions.DependencyInjection;

namespace HotelManagement.Forms
{
    /// <summary>
    /// Hộp thoại chọn khách hàng đã có sẵn, dùng cho FormDatPhong.
    /// Trả về khách hàng qua property KhachHangDaChon khi nhấn "Chọn".
    /// </summary>
    public partial class FormChonKhachHang : Form
    {
        private readonly IServiceScope _scope;
        private readonly IKhachHangService _khachHangService;

        public KhachHangDto? KhachHangDaChon { get; private set; }

        public FormChonKhachHang()
        {
            _scope = Program.ServiceProvider.CreateScope();
            _khachHangService = _scope.ServiceProvider.GetRequiredService<IKhachHangService>();

            InitializeComponent();

            FormClosed += (_, _) => _scope.Dispose();
            Load += async (_, _) => await TaiDanhSachAsync();
        }

        private async Task TaiDanhSachAsync()
        {
            var danhSach = await _khachHangService.GetAllAsync();
            GanDuLieu(danhSach);
        }

        private async Task TimKiemAsync()
        {
            var ketQua = await _khachHangService.TimKiemAsync(_txtTimKiem.Text.Trim());
            GanDuLieu(ketQua);
        }

        private void GanDuLieu(List<KhachHangDto> danhSach)
        {
            _dgv.DataSource = null;
            _dgv.DataSource = danhSach;

            if (_dgv.Columns[nameof(KhachHangDto.IdKhachHang)] != null)
                _dgv.Columns[nameof(KhachHangDto.IdKhachHang)].Visible = false;
            if (_dgv.Columns[nameof(KhachHangDto.Ten)] != null)
                _dgv.Columns[nameof(KhachHangDto.Ten)].HeaderText = "Họ tên";
            if (_dgv.Columns[nameof(KhachHangDto.Cccd)] != null)
                _dgv.Columns[nameof(KhachHangDto.Cccd)].HeaderText = "CCCD";
            if (_dgv.Columns[nameof(KhachHangDto.Sdt)] != null)
                _dgv.Columns[nameof(KhachHangDto.Sdt)].HeaderText = "SĐT";
            if (_dgv.Columns[nameof(KhachHangDto.Email)] != null)
                _dgv.Columns[nameof(KhachHangDto.Email)].HeaderText = "Email";
            if (_dgv.Columns[nameof(KhachHangDto.GioiTinh)] != null)
                _dgv.Columns[nameof(KhachHangDto.GioiTinh)].HeaderText = "Giới tính";
            if (_dgv.Columns[nameof(KhachHangDto.NgaySinh)] != null)
                _dgv.Columns[nameof(KhachHangDto.NgaySinh)].Visible = false;
        }

        private void ChonKhachHang()
        {
            if (_dgv.CurrentRow?.DataBoundItem is not KhachHangDto dto) return;

            KhachHangDaChon = dto;
            DialogResult = DialogResult.OK;
            Close();
        }

        private async Task ThemKhachHangMoiAsync()
        {
            using var frmKhachHang = new FormKhachHang();
            frmKhachHang.ShowDialog(this);
            await TaiDanhSachAsync();
        }
    }
}
