using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;
using HotelManagement.BLL.DTOs;
using HotelManagement.BLL.Services;
using Microsoft.Extensions.DependencyInjection;

namespace HotelManagement.Forms
{
    /// <summary>
    /// Form Quản lý danh mục Dịch vụ trong hệ thống khách sạn.
    /// Cho phép xem danh sách, tìm kiếm, thêm, sửa, xóa dịch vụ phát sinh.
    /// </summary>
    public partial class FormDichVu : Form
    {
        private readonly IServiceScope _scope;
        private readonly IDichVuService _dichVuService;

        private int? _idDangChon;
        private readonly System.Windows.Forms.Timer _debounceTimer = new();

        public FormDichVu()
        {
            _scope = Program.ServiceProvider.CreateScope();
            _dichVuService = _scope.ServiceProvider.GetRequiredService<IDichVuService>();

            InitializeComponent();

            _debounceTimer.Interval = 300;
            _debounceTimer.Tick += async (s, e) =>
            {
                _debounceTimer.Stop();
                await TimKiemAsync();
            };

            _txtTimKiem.TextChanged += (s, e) =>
            {
                _debounceTimer.Stop();
                _debounceTimer.Start();
            };

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
                var danhSach = await _dichVuService.LayDanhSachAsync();
                GanDuLieuLuoi(danhSach);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải danh sách dịch vụ: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task TimKiemAsync()
        {
            try
            {
                var ketQua = await _dichVuService.TimKiemAsync(_txtTimKiem.Text);
                GanDuLieuLuoi(ketQua);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tìm kiếm dịch vụ: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void GanDuLieuLuoi(List<DichVuDto> danhSach)
        {
            _dgv.DataSource = null;
            _dgv.DataSource = danhSach;

            if (_dgv.Columns["IdDichVu"] != null)
            {
                _dgv.Columns["IdDichVu"].HeaderText = "Mã DV";
                _dgv.Columns["IdDichVu"].Width = 90;
            }
            if (_dgv.Columns["TenDichVu"] != null)
            {
                _dgv.Columns["TenDichVu"].HeaderText = "Tên dịch vụ";
            }
            if (_dgv.Columns["MucGia"] != null)
            {
                _dgv.Columns["MucGia"].HeaderText = "Đơn giá (VNĐ)";
                _dgv.Columns["MucGia"].DefaultCellStyle.Format = "N0";
                _dgv.Columns["MucGia"].Width = 180;
            }
        }

        private void Dgv_SelectionChanged(object? sender, EventArgs e)
        {
            if (_dgv.CurrentRow?.DataBoundItem is not DichVuDto dto) return;

            _idDangChon = dto.IdDichVu;
            _txtTen.Text = dto.TenDichVu;
            _numGia.Value = Math.Min(dto.MucGia, _numGia.Maximum);
        }

        private async Task ThemAsync()
        {
            try
            {
                var dto = new DichVuDto
                {
                    TenDichVu = _txtTen.Text.Trim(),
                    MucGia = _numGia.Value
                };

                await _dichVuService.ThemAsync(dto);
                MessageBox.Show("Thêm dịch vụ thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LamMoiForm();
                await TaiDanhSachAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi thêm", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private async Task SuaAsync()
        {
            if (_idDangChon is null)
            {
                MessageBox.Show("Vui lòng chọn dịch vụ cần sửa từ danh sách.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                var dto = new DichVuDto
                {
                    IdDichVu = _idDangChon.Value,
                    TenDichVu = _txtTen.Text.Trim(),
                    MucGia = _numGia.Value
                };

                await _dichVuService.CapNhatAsync(dto);
                MessageBox.Show("Cập nhật dịch vụ thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                await TaiDanhSachAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi sửa", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private async Task XoaAsync()
        {
            if (_idDangChon is null)
            {
                MessageBox.Show("Vui lòng chọn dịch vụ cần xóa từ danh sách.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var hoi = MessageBox.Show($"Bạn có chắc chắn muốn xóa dịch vụ '{_txtTen.Text}'?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (hoi != DialogResult.Yes) return;

            try
            {
                await _dichVuService.XoaAsync(_idDangChon.Value);
                MessageBox.Show("Đã xóa dịch vụ thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LamMoiForm();
                await TaiDanhSachAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi xóa", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void LamMoiForm()
        {
            _idDangChon = null;
            _txtTen.Clear();
            _numGia.Value = 0;
            _dgv.ClearSelection();
            _txtTen.Focus();
        }
    }
}
