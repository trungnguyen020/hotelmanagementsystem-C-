using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;
using HotelManagement.BLL.DTOs;
using HotelManagement.BLL.Services;
using HotelManagement.Common;
using Microsoft.Extensions.DependencyInjection;

namespace HotelManagement.Forms
{
    /// <summary>
    /// Form CRUD Nhân sự (DCKLTTQ-68) — mở từ menu "Quản lý nhân sự" của FormChinh.
    /// Thiết kế hiện đại với thanh công cụ trên cùng, thao tác sửa/xóa trực tiếp trong dòng
    /// và thanh quản lý tài khoản ở phía dưới.
    /// </summary>
    public partial class FormNhanSu : Form
    {
        private readonly IServiceScope _scope;
        private readonly INhanSuService _nhanSuService;
        private readonly System.Windows.Forms.Timer _debounceTimer = new();

        private int? _idDangChon;

        public FormNhanSu()
        {
            _scope = Program.ServiceProvider.CreateScope();
            _nhanSuService = _scope.ServiceProvider.GetRequiredService<INhanSuService>();

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
            _btnXuatExcel.Click += async (_, _) => await ExcelExportHelper.XuatDataGridViewRaExcelAsync(_dgv, "Nhân sự", "DanhSachNhanSu", this);
            _btnDatLaiMk.Click += async (_, _) => await DatLaiMatKhauAsync();
            _mnuDatLaiMk.Click += async (_, _) => await DatLaiMatKhauAsync();

            _dgv.CellContentClick += Dgv_CellContentClick;
            _dgv.SelectionChanged += Dgv_SelectionChanged;

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
                var danhSach = await _nhanSuService.GetAllAsync();
                _dgv.DataSource = danhSach;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải danh sách nhân sự: " + ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task TimKiemAsync()
        {
            try
            {
                string tuKhoa = _txtTimKiem.Text.Trim();
                var ketQua = await _nhanSuService.TimKiemAsync(tuKhoa);
                _dgv.DataSource = ketQua;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tìm kiếm nhân sự: " + ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Dgv_SelectionChanged(object? sender, EventArgs e)
        {
            if (_dgv.CurrentRow?.DataBoundItem is not NhanSuDto dto)
            {
                _idDangChon = null;
                _lblTaiKhoanInfo.Text = "Chưa chọn nhân sự";
                _btnDatLaiMk.Visible = false;
                return;
            }

            _idDangChon = dto.IdNhanSu;

            if (!string.IsNullOrEmpty(dto.TenTaiKhoanHienThi))
            {
                _lblTaiKhoanInfo.Text = $"📋 Tài khoản: {dto.TenTaiKhoanHienThi} — Trạng thái: {dto.TrangThaiTaiKhoan}";
                if (dto.TenTaiKhoanHienThi.ToLower() == "admin")
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
                _lblTaiKhoanInfo.Text = "(Nhân sự này chưa có tài khoản)";
                _btnDatLaiMk.Visible = false;
            }
        }

        private async Task ThemMoiAsync()
        {
            using var dlg = new FormChiTietNhanSu();
            if (dlg.ShowDialog(this) == DialogResult.OK)
            {
                var (success, message) = await _nhanSuService.AddAsync(dlg.DtoKetQua);
                MessageBox.Show(message, success ? "Thông báo" : "Lỗi",
                    MessageBoxButtons.OK, success ? MessageBoxIcon.Information : MessageBoxIcon.Warning);

                if (success)
                    await TaiDanhSachAsync();
            }
        }

        private async void Dgv_CellContentClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= _dgv.Rows.Count) return;

            if (_dgv.Rows[e.RowIndex].DataBoundItem is not NhanSuDto dto) return;

            // 1. Nhấn nút Sửa
            if (e.ColumnIndex == _colEdit.Index)
            {
                using var dlg = new FormChiTietNhanSu(dto);
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    var (success, message) = await _nhanSuService.UpdateAsync(dlg.DtoKetQua);
                    MessageBox.Show(message, success ? "Thông báo" : "Lỗi",
                        MessageBoxButtons.OK, success ? MessageBoxIcon.Information : MessageBoxIcon.Warning);

                    if (success)
                        await TaiDanhSachAsync();
                }
            }
            // 2. Nhấn nút Xóa
            else if (e.ColumnIndex == _colDelete.Index)
            {
                var xacNhan = MessageBox.Show(
                    $"Bạn có chắc chắn muốn xóa nhân sự '{dto.HoTen}'?", "Xác nhận xóa",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (xacNhan != DialogResult.Yes) return;

                var (success, message) = await _nhanSuService.DeleteAsync(dto.IdNhanSu);
                MessageBox.Show(message, success ? "Thông báo" : "Lỗi",
                    MessageBoxButtons.OK, success ? MessageBoxIcon.Information : MessageBoxIcon.Warning);

                if (success)
                    await TaiDanhSachAsync();
            }
        }

        private async Task DatLaiMatKhauAsync()
        {
            if (!_idDangChon.HasValue) return;

            var dto = _dgv.CurrentRow?.DataBoundItem as NhanSuDto;
            if (dto?.TenTaiKhoanHienThi?.ToLower() == "admin")
            {
                MessageBox.Show("Không thể đặt lại mật khẩu cho tài khoản Quản trị viên (Admin) từ đây.", "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string hoTen = dto?.HoTen ?? "này";
            var xacNhan = MessageBox.Show(
                $"Bạn có chắc chắn muốn đặt lại mật khẩu cho nhân sự '{hoTen}' thành 'letan123'?", "Xác nhận",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (xacNhan != DialogResult.Yes) return;

            var (success, message) = await _nhanSuService.DatLaiMatKhauTaiKhoanAsync(_idDangChon.Value, "letan123");
            MessageBox.Show(message, success ? "Thông báo" : "Lỗi",
                MessageBoxButtons.OK, success ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
        }
    }
}
