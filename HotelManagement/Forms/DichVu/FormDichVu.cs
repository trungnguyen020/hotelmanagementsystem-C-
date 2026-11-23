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
    /// Form Quản lý danh mục Dịch vụ trong hệ thống khách sạn.
    /// Thiết kế hiện đại với thanh công cụ trên cùng và thao tác sửa/xóa trực tiếp trong dòng.
    /// </summary>
    public partial class FormDichVu : Form
    {
        private readonly IServiceScope _scope;
        private readonly IDichVuService _dichVuService;
        private readonly System.Windows.Forms.Timer _debounceTimer = new();

        public FormDichVu()
        {
            _scope = Program.ServiceProvider.CreateScope();
            _dichVuService = _scope.ServiceProvider.GetRequiredService<IDichVuService>();

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
            _btnXuatExcel.Click += async (_, _) => await ExcelExportHelper.XuatDataGridViewRaExcelAsync(_dgv, "Dịch vụ", "DanhSachDichVu", this);
            _dgv.CellContentClick += Dgv_CellContentClick;

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
                _dgv.DataSource = danhSach;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải danh sách dịch vụ: " + ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task TimKiemAsync()
        {
            try
            {
                string tuKhoa = _txtTimKiem.Text.Trim();
                var ketQua = await _dichVuService.TimKiemAsync(tuKhoa);
                _dgv.DataSource = ketQua;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tìm kiếm dịch vụ: " + ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task ThemMoiAsync()
        {
            using var dlg = new FormChiTietDichVu();
            if (dlg.ShowDialog(this) == DialogResult.OK)
            {
                try
                {
                    await _dichVuService.ThemAsync(dlg.DtoKetQua);
                    MessageBox.Show("Thêm dịch vụ thành công.", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    await TaiDanhSachAsync();
                }
                catch (InvalidOperationException ex)
                {
                    MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Có lỗi xảy ra: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private async void Dgv_CellContentClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= _dgv.Rows.Count) return;

            if (_dgv.Rows[e.RowIndex].DataBoundItem is not DichVuDto dto) return;

            // 1. Nhấn nút Sửa
            if (e.ColumnIndex == _colEdit.Index)
            {
                using var dlg = new FormChiTietDichVu(dto);
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    try
                    {
                        await _dichVuService.CapNhatAsync(dlg.DtoKetQua);
                        MessageBox.Show("Cập nhật dịch vụ thành công.", "Thông báo",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        await TaiDanhSachAsync();
                    }
                    catch (InvalidOperationException ex)
                    {
                        MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Có lỗi xảy ra: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            // 2. Nhấn nút Xóa
            else if (e.ColumnIndex == _colDelete.Index)
            {
                var xacNhan = MessageBox.Show(
                    $"Bạn có chắc chắn muốn xóa dịch vụ '{dto.TenDichVu}'?", "Xác nhận xóa",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (xacNhan != DialogResult.Yes) return;

                try
                {
                    await _dichVuService.XoaAsync(dto.IdDichVu);
                    MessageBox.Show("Xóa dịch vụ thành công.", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    await TaiDanhSachAsync();
                }
                catch (InvalidOperationException ex)
                {
                    MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Có lỗi xảy ra: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
