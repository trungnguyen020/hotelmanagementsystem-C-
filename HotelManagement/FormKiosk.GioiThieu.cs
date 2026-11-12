using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using HotelManagement.DAL.Repositories;
using HotelManagement.Localization;
using HotelManagement.Models.Entities;
using Microsoft.Extensions.DependencyInjection;

namespace HotelManagement.Forms
{
    /// <summary>
    /// Phân đoạn tab "Giới thiệu khách sạn" của FormKiosk (DCKLTTQ-103).
    /// Danh sách dịch vụ chỉ tải một lần duy nhất ở lần mở tab đầu tiên.
    /// </summary>
    public partial class FormKiosk
    {
        private bool _daTaiDanhSachDichVu;
        private bool _dangTaiDanhSachDichVu;

        private void KhoiTaoGioiThieu()
        {
            _tabMain.SelectedIndexChanged += async (_, _) =>
            {
                if (_tabMain.SelectedTab == _tabGioiThieu)
                {
                    await TaiDanhSachDichVuMotLanAsync();
                }
            };

            if (_tabMain.SelectedTab == _tabGioiThieu)
            {
                Shown += async (_, _) => await TaiDanhSachDichVuMotLanAsync();
            }
        }

        private async Task TaiDanhSachDichVuMotLanAsync()
        {
            if (_daTaiDanhSachDichVu || _dangTaiDanhSachDichVu)
                return;

            _dangTaiDanhSachDichVu = true;
            _lblDichVuTrangThai.Text = LanguageManager.GetString("FormKiosk_Intro_ServicesLoading", "Đang tải danh sách dịch vụ...");
            _lblDichVuTrangThai.Visible = true;

            try
            {
                var unitOfWork = _scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                var danhSach = await unitOfWork.DichVuRepository.GetAllAsync();

                // Sắp xếp để danh sách ổn định giữa các lần hiển thị
                var sapXep = new List<DichVu>(danhSach);
                sapXep.Sort((a, b) =>
                {
                    int soSanh = a.Mucgia.CompareTo(b.Mucgia);
                    return soSanh != 0 ? soSanh : string.CompareOrdinal(a.TenDichvu, b.TenDichvu);
                });

                HienThiDanhSachDichVu(sapXep);
                _daTaiDanhSachDichVu = true;
                _lblDichVuTrangThai.Visible = false;
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "[FormKiosk] Không tải được danh sách dịch vụ cho tab giới thiệu.");
                _lblDichVuTrangThai.Text = LanguageManager.GetString("FormKiosk_Intro_ServicesError", "Không tải được danh sách dịch vụ. Vui lòng liên hệ lễ tân.");
                _lblDichVuTrangThai.Visible = true;
            }
            finally
            {
                _dangTaiDanhSachDichVu = false;
            }
        }

        private void HienThiDanhSachDichVu(IReadOnlyList<DichVu> danhSach)
        {
            _flpDichVu.SuspendLayout();
            _flpDichVu.Controls.Clear();

            if (danhSach.Count == 0)
            {
                _lblDichVuTrangThai.Text = LanguageManager.GetString("FormKiosk_Intro_ServicesEmpty", "Chưa có dữ liệu dịch vụ.");
                _lblDichVuTrangThai.Visible = true;
                _flpDichVu.ResumeLayout();
                return;
            }

            int chieuRong = _flpDichVu.ClientSize.Width - 26;
            if (chieuRong < 320) chieuRong = 320;

            foreach (var dichVu in danhSach)
            {
                _flpDichVu.Controls.Add(TaoTheDichVu(dichVu, chieuRong));
            }

            _flpDichVu.ResumeLayout();
        }

        private Control TaoTheDichVu(DichVu dichVu, int chieuRong)
        {
            var the = new Panel
            {
                Width = chieuRong,
                Height = 62,
                BackColor = AppTheme.SurfaceSecondary,
                Margin = new Padding(0, 0, 0, 8)
            };

            var vachMau = new Panel
            {
                Dock = DockStyle.Left,
                Width = 4,
                BackColor = AppTheme.AccentGold
            };

            var lblTen = new Label
            {
                Text = dichVu.TenDichvu,
                Font = AppTheme.FontBodyBold,
                ForeColor = AppTheme.TextPrimary,
                Location = new Point(16, 9),
                AutoSize = true,
                UseMnemonic = false
            };

            var lblGia = new Label
            {
                Text = $"{dichVu.Mucgia:N0} đ",
                Font = AppTheme.FontSmallBold,
                ForeColor = AppTheme.Primary,
                Location = new Point(16, 33),
                AutoSize = true,
                UseMnemonic = false
            };

            the.Controls.AddRange(new Control[] { lblGia, lblTen, vachMau });
            return the;
        }
    }
}
