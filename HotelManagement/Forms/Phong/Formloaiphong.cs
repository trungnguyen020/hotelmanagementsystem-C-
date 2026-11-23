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
    /// Form CRUD Loại phòng — thiết kế hiện đại theo chuẩn bảng và hành động trực tiếp trong dòng.
    /// Nút "+ Thêm mới" và "Xuất file Excel" nằm trên thanh công cụ.
    /// </summary>
    public partial class FormLoaiPhong : Form
    {
        private readonly IServiceScope _scope;
        private readonly ILoaiPhongService _loaiPhongService;
        private readonly System.Windows.Forms.Timer _debounceTimer = new();

        public FormLoaiPhong()
        {
            _scope = Program.ServiceProvider.CreateScope();
            _loaiPhongService = _scope.ServiceProvider.GetRequiredService<ILoaiPhongService>();

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
            _btnXuatExcel.Click += async (_, _) => await ExcelExportHelper.XuatDataGridViewRaExcelAsync(_dgv, "Loại phòng", "DanhSachLoaiPhong", this);
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
                var danhSach = await _loaiPhongService.LayDanhSachAsync();
                _dgv.DataSource = danhSach;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải danh sách loại phòng: " + ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task TimKiemAsync()
        {
            try
            {
                string tuKhoa = _txtTimKiem.Text.Trim();
                var ketQua = await _loaiPhongService.TimKiemAsync(tuKhoa);
                _dgv.DataSource = ketQua;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tìm kiếm loại phòng: " + ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task ThemMoiAsync()
        {
            using var dlg = new FormChiTietLoaiPhong();
            if (dlg.ShowDialog(this) == DialogResult.OK)
            {
                try
                {
                    await _loaiPhongService.ThemAsync(dlg.DtoKetQua);
                    MessageBox.Show("Thêm loại phòng thành công.", "Thông báo",
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

            if (_dgv.Rows[e.RowIndex].DataBoundItem is not LoaiPhongDto dto) return;

            // 1. Nhấn nút Sửa
            if (e.ColumnIndex == _colEdit.Index)
            {
                using var dlg = new FormChiTietLoaiPhong(dto);
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    try
                    {
                        await _loaiPhongService.CapNhatAsync(dlg.DtoKetQua);
                        MessageBox.Show("Cập nhật loại phòng thành công.", "Thông báo",
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
                    $"Bạn có chắc chắn muốn xóa loại phòng '{dto.TenLoaiPhong}'?", "Xác nhận xóa",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (xacNhan != DialogResult.Yes) return;

                try
                {
                    await _loaiPhongService.XoaAsync(dto.Id);
                    MessageBox.Show("Xóa loại phòng thành công.", "Thông báo",
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
