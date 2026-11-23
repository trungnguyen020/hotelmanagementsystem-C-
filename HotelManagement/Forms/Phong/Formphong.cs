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
    /// Form CRUD Phòng — mở từ menu "Quản lý phòng" của FormChinh.
    /// Thiết kế hiện đại với thanh công cụ trên cùng và thao tác sửa/xóa trực tiếp trong dòng.
    /// </summary>
    public partial class FormPhong : Form
    {
        private readonly IServiceScope _scope;
        private readonly IPhongService _phongService;
        private readonly ILoaiPhongService _loaiPhongService;
        private readonly HotelManagement.Models.Entities.TaiKhoan? _taiKhoanDangNhap;
        private readonly System.Windows.Forms.Timer _debounceTimer = new();

        private List<LoaiPhongDto> _dsLoaiPhong = new();

        public FormPhong(HotelManagement.Models.Entities.TaiKhoan? taiKhoanDangNhap = null)
        {
            _taiKhoanDangNhap = taiKhoanDangNhap;
            _scope = Program.ServiceProvider.CreateScope();
            _phongService = _scope.ServiceProvider.GetRequiredService<IPhongService>();
            _loaiPhongService = _scope.ServiceProvider.GetRequiredService<ILoaiPhongService>();

            InitializeComponent();

            // Phân quyền: nút Import Excel chỉ hiển thị cho Admin
            if (_taiKhoanDangNhap != null && _taiKhoanDangNhap.IdVaitroNavigation != null)
            {
                _btnImportExcel.Visible = _taiKhoanDangNhap.IdVaitroNavigation.TenVaitro == "Admin";
            }

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
            _btnXuatExcel.Click += async (_, _) => await ExcelExportHelper.XuatDataGridViewRaExcelAsync(_dgv, "Phòng", "DanhSachPhong", this);
            _btnImportExcel.Click += async (_, _) =>
            {
                using var form = new FormImportPhong();
                form.ShowDialog(this);
                await TaiDanhSachAsync();
            };

            _dgv.CellContentClick += Dgv_CellContentClick;

            Load += async (_, _) =>
            {
                await TaiDanhSachLoaiPhongAsync();
                await TaiDanhSachAsync();
            };

            FormClosed += (_, _) =>
            {
                _debounceTimer.Dispose();
                _scope.Dispose();
            };
        }

        private async Task TaiDanhSachLoaiPhongAsync()
        {
            try
            {
                _dsLoaiPhong = await _loaiPhongService.LayDanhSachAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải loại phòng: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task TaiDanhSachAsync()
        {
            try
            {
                var danhSach = await _phongService.LayDanhSachAsync();
                _dgv.DataSource = danhSach;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải danh sách phòng: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task TimKiemAsync()
        {
            try
            {
                string tuKhoa = _txtTimKiem.Text.Trim();
                var ketQua = await _phongService.TimKiemAsync(tuKhoa);
                _dgv.DataSource = ketQua;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tìm kiếm phòng: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task ThemMoiAsync()
        {
            if (_dsLoaiPhong.Count == 0)
                await TaiDanhSachLoaiPhongAsync();

            using var dlg = new FormChiTietPhong(null, _dsLoaiPhong);
            if (dlg.ShowDialog(this) == DialogResult.OK)
            {
                try
                {
                    await _phongService.ThemAsync(dlg.DtoKetQua);
                    MessageBox.Show("Thêm phòng thành công.", "Thông báo",
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

            if (_dgv.Rows[e.RowIndex].DataBoundItem is not PhongDto dto) return;

            // 1. Nhấn Sửa
            if (e.ColumnIndex == _colEdit.Index)
            {
                if (_dsLoaiPhong.Count == 0)
                    await TaiDanhSachLoaiPhongAsync();

                using var dlg = new FormChiTietPhong(dto, _dsLoaiPhong);
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    try
                    {
                        await _phongService.CapNhatAsync(dlg.DtoKetQua);
                        MessageBox.Show("Cập nhật phòng thành công.", "Thông báo",
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
            // 2. Nhấn Xóa
            else if (e.ColumnIndex == _colDelete.Index)
            {
                var xacNhan = MessageBox.Show(
                    $"Bạn có chắc chắn muốn xóa phòng {dto.SoPhong}?", "Xác nhận xóa",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (xacNhan != DialogResult.Yes) return;

                try
                {
                    await _phongService.XoaAsync(dto.Id);
                    MessageBox.Show("Xóa phòng thành công.", "Thông báo",
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
