using System;
using System.Drawing;
using HotelManagement.Localization;

namespace HotelManagement
{
    /// <summary>
    /// Phân đoạn xử lý đa ngôn ngữ cho FormChinh (DCKLTTQ-86).
    /// </summary>
    public partial class Formchinh : ILocalizableForm
    {
        private bool _isUpdatingLanguage;

        public void CapNhatNgonNgu()
        {
            _isUpdatingLanguage = true;
            try
            {
                // 1. Tiêu đề cửa sổ
                this.Text = $"{LanguageManager.GetString("FormChinh_Title")} - " +
                            string.Format(LanguageManager.GetString("FormChinh_UserBadge"),
                                _taiKhoanDangNhap.IdNhansuNavigation.Hoten,
                                _taiKhoanDangNhap.IdVaitroNavigation.TenVaitro);

                // 2. Header TopBar
                if (_lblHotelTitle != null)
                {
                    _lblHotelTitle.Text = LanguageManager.GetString("FormChinh_HeaderTitle");
                }

                if (_lblUserInfoBadge != null)
                {
                    _lblUserInfoBadge.Text = string.Format(LanguageManager.GetString("FormChinh_UserBadge"),
                        _taiKhoanDangNhap.IdNhansuNavigation.Hoten,
                        _taiKhoanDangNhap.IdVaitroNavigation.TenVaitro);
                }

                if (_cboLanguage != null)
                {
                    int expectedIndex = LanguageManager.CurrentLanguage == AppLanguage.English ? 1 : 0;
                    if (_cboLanguage.SelectedIndex != expectedIndex)
                    {
                        _cboLanguage.SelectedIndex = expectedIndex;
                    }
                }

                // Căn chỉnh lại vị trí User Badge khi Text thay đổi
                if (_pnlTopBar != null && _cboLanguage != null && _lblUserInfoBadge != null)
                {
                    _cboLanguage.Location = new Point(_pnlTopBar.ClientSize.Width - _cboLanguage.Width - 24, 18);
                    _lblUserInfoBadge.Location = new Point(_cboLanguage.Left - _lblUserInfoBadge.Width - 16, 16);
                }

                // 3. Hệ thống MenuStrip
                if (mnuQuanLyPhong != null) mnuQuanLyPhong.Text = LanguageManager.GetString("FormChinh_Menu_QuanLyPhong");
                if (mnuPhong != null) mnuPhong.Text = LanguageManager.GetString("FormChinh_Menu_DanhSachPhong");
                if (mnuLoaiPhong != null) mnuLoaiPhong.Text = LanguageManager.GetString("FormChinh_Menu_LoaiPhong");
                if (mnuImportPhong != null) mnuImportPhong.Text = LanguageManager.GetString("FormChinh_Menu_ImportPhong", "Import phòng từ Excel");
                if (mnuSoDoPhongTop != null) mnuSoDoPhongTop.Text = LanguageManager.GetString("FormChinh_Menu_SoDoPhong");
                if (mnuQuanLyDatPhong != null) mnuQuanLyDatPhong.Text = LanguageManager.GetString("FormChinh_Menu_QuanLyDatPhong");
                if (mnuDatPhong != null) mnuDatPhong.Text = LanguageManager.GetString("FormChinh_Menu_DatPhongMoi");
                if (mnuCheckIn != null) mnuCheckIn.Text = LanguageManager.GetString("FormChinh_Menu_CheckIn");
                if (mnuCheckOut != null) mnuCheckOut.Text = LanguageManager.GetString("FormChinh_Menu_CheckOut");
                if (mnuThanhToan != null) mnuThanhToan.Text = LanguageManager.GetString("FormChinh_Menu_ThanhToan");
                if (mnuKhachHang != null) mnuKhachHang.Text = LanguageManager.GetString("FormChinh_Menu_KhachHang");
                if (mnuQuanLyNhanSu != null) mnuQuanLyNhanSu.Text = LanguageManager.GetString("FormChinh_Menu_NhanSu");
                if (mnuBaoCao != null) mnuBaoCao.Text = LanguageManager.GetString("FormChinh_Menu_BaoCao");
                if (mnuDashboard != null) mnuDashboard.Text = LanguageManager.GetString("FormChinh_Menu_Dashboard");
                if (mnuLichSuThongBao != null) mnuLichSuThongBao.Text = LanguageManager.GetString("FormChinh_Menu_LichSuThongBao");
                if (mnuDoiMatKhau != null) mnuDoiMatKhau.Text = LanguageManager.GetString("FormChinh_Menu_DoiMatKhau");
                if (mnuDangXuat != null) mnuDangXuat.Text = LanguageManager.GetString("FormChinh_Menu_DangXuat");

                // 4. Nội dung Dashboard & Tác vụ nhanh
                if (_lblDashboardTitle != null) _lblDashboardTitle.Text = LanguageManager.GetString("FormChinh_DashboardTitle");
                if (_lblDashboardSub != null) _lblDashboardSub.Text = LanguageManager.GetString("FormChinh_DashboardSub");

                // 5. Status Strip
                if (_lblStatusUser != null)
                {
                    _lblStatusUser.Text = string.Format(LanguageManager.GetString("FormChinh_StatusReady"),
                        _taiKhoanDangNhap.TenDangnhap,
                        _taiKhoanDangNhap.IdVaitroNavigation.TenVaitro);
                }

                // 6. Nạp lại thẻ tác vụ nhanh với các chuỗi dịch mới
                TaiLaiTheTacVuNhanh();
            }
            finally
            {
                _isUpdatingLanguage = false;
            }
        }
    }
}

