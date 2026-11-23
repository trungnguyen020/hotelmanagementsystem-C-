using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;
using HotelManagement.BLL.DTOs;
using HotelManagement.BLL.Services;
using HotelManagement.Common;
using HotelManagement.Models.Entities;
using Microsoft.Extensions.DependencyInjection;

namespace HotelManagement.Forms
{
    /// <summary>
    /// Form CRUD Khách hàng (DCKLTTQ-67) — mở từ menu "Quản lý khách hàng" của FormChinh.
    /// Thiết kế hiện đại với thanh công cụ trên cùng và thao tác sửa/xóa trực tiếp trong dòng.
    /// </summary>
    public partial class FormKhachHang : Form
    {
        private readonly TaiKhoan? _taiKhoanDangNhap;
        private readonly IServiceScope _scope;
        private readonly IKhachHangService _khachHangService;
        private readonly System.Windows.Forms.Timer _debounceTimer = new();

        public FormKhachHang(TaiKhoan? taiKhoanDangNhap = null)
        {
            _taiKhoanDangNhap = taiKhoanDangNhap;
            _scope = Program.ServiceProvider.CreateScope();
            _khachHangService = _scope.ServiceProvider.GetRequiredService<IKhachHangService>();

            InitializeComponent();

            _debounceTimer.Interval = 300;
            _debounceTimer.Tick += async (_, _) =>
            {
                _debounceTimer.Stop();
                await TimKiemAsync();
            };

            _txtTimKiem.TextChanged += (_, _) =>
            {
                _debounceTimer.Stop();
                _debounceTimer.Start();
            };

            _btnThemMoi.Click += async (_, _) => await ThemMoiAsync();
            _btnXuatExcel.Click += async (_, _) => await ExcelExportHelper.XuatDataGridViewRaExcelAsync(_dgv, "Khách hàng", "DanhSachKhachHang", this);
            _btnDatPhong.Click += (_, _) => MoDatPhongChoKhachHang();

            _mnuDatPhong.Click += (_, _) => MoDatPhongChoKhachHang();
            _mnuDatLaiMk.Click += async (_, _) => await DatLaiMatKhauAsync();
            _mnuKhoaMoKhoa.Click += async (_, _) => await ChuyenTrangThaiTaiKhoanAsync();

            _dgv.CellContentClick += Dgv_CellContentClick;
            _dgv.CellDoubleClick += (_, _) => MoDatPhongChoKhachHang();

            Load += async (_, _) => await TaiDanhSachAsync();
            FormClosed += (_, _) =>
            {
                _debounceTimer.Dispose();
                _scope.Dispose();
            };
        }

        private async Task TaiDanhSachAsync()
        {
            try
            {
                var danhSach = await _khachHangService.GetAllAsync();
                _dgv.DataSource = danhSach;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải danh sách khách hàng: " + ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task TimKiemAsync()
        {
            try
            {
                string tuKhoa = _txtTimKiem.Text.Trim();
                var ketQua = await _khachHangService.TimKiemAsync(tuKhoa);
                _dgv.DataSource = ketQua;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tìm kiếm khách hàng: " + ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task ThemMoiAsync()
        {
            using var dlg = new FormChiTietKhachHang();
            if (dlg.ShowDialog(this) == DialogResult.OK)
            {
                var (success, message) = await _khachHangService.AddAsync(dlg.DtoKetQua);
                MessageBox.Show(message, success ? "Thông báo" : "Lỗi",
                    MessageBoxButtons.OK, success ? MessageBoxIcon.Information : MessageBoxIcon.Warning);

                if (success)
                    await TaiDanhSachAsync();
            }
        }

        private async void Dgv_CellContentClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= _dgv.Rows.Count) return;

            if (_dgv.Rows[e.RowIndex].DataBoundItem is not KhachHangDto dto) return;

            // 1. Nhấn Sửa
            if (e.ColumnIndex == _colEdit.Index)
            {
                using var dlg = new FormChiTietKhachHang(dto);
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    var (success, message) = await _khachHangService.UpdateAsync(dlg.DtoKetQua);
                    MessageBox.Show(message, success ? "Thông báo" : "Lỗi",
                        MessageBoxButtons.OK, success ? MessageBoxIcon.Information : MessageBoxIcon.Warning);

                    if (success)
                        await TaiDanhSachAsync();
                }
            }
            // 2. Nhấn Xóa
            else if (e.ColumnIndex == _colDelete.Index)
            {
                var xacNhan = MessageBox.Show(
                    $"Bạn có chắc chắn muốn xóa khách hàng '{dto.Ten}'?", "Xác nhận xóa",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (xacNhan != DialogResult.Yes) return;

                var (success, message) = await _khachHangService.DeleteAsync(dto.IdKhachHang);
                MessageBox.Show(message, success ? "Thông báo" : "Lỗi",
                    MessageBoxButtons.OK, success ? MessageBoxIcon.Information : MessageBoxIcon.Warning);

                if (success)
                    await TaiDanhSachAsync();
            }
        }

        private async Task DatLaiMatKhauAsync()
        {
            if (_dgv.CurrentRow?.DataBoundItem is not KhachHangDto dto) return;

            var xacNhan = MessageBox.Show(
                $"Bạn có chắc chắn muốn đặt lại mật khẩu của khách hàng '{dto.Ten}' về mặc định '123456'?",
                "Xác nhận đặt lại mật khẩu",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (xacNhan != DialogResult.Yes) return;

            var (success, message) = await _khachHangService.ResetMatKhauKhachHangAsync(dto.IdKhachHang);
            MessageBox.Show(message, success ? "Thành công" : "Lỗi",
                MessageBoxButtons.OK, success ? MessageBoxIcon.Information : MessageBoxIcon.Warning);

            if (success)
                await TaiDanhSachAsync();
        }

        private async Task ChuyenTrangThaiTaiKhoanAsync()
        {
            if (_dgv.CurrentRow?.DataBoundItem is not KhachHangDto dto) return;

            var (success, message) = await _khachHangService.ChuyenTrangThaiTaiKhoanAsync(dto.IdKhachHang);
            MessageBox.Show(message, success ? "Thông báo" : "Lỗi",
                MessageBoxButtons.OK, success ? MessageBoxIcon.Information : MessageBoxIcon.Warning);

            if (success)
                await TaiDanhSachAsync();
        }

        private void MoDatPhongChoKhachHang()
        {
            if (_dgv.CurrentRow?.DataBoundItem is not KhachHangDto khachHang)
            {
                MessageBox.Show("Vui lòng chọn một khách hàng từ danh sách để đặt phòng.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using var frmDatPhong = new FormDatPhong(_taiKhoanDangNhap, khachHang);
            frmDatPhong.ShowDialog(this);
        }
    }
}
