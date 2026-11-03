using HotelManagement.Localization;

namespace HotelManagement
{
    /// <summary>
    /// Phân đoạn xử lý đa ngôn ngữ cho FormDangNhap (DCKLTTQ-86).
    /// </summary>
    public partial class Formdangnhap : ILocalizableForm
    {
        public void CapNhatNgonNgu()
        {
            // 1. Tiêu đề Form
            this.Text = LanguageManager.GetString("FormDangNhap_Title");

            // 2. Các nhãn và tiêu đề Card
            if (_lblBrand != null) _lblBrand.Text = LanguageManager.GetString("FormDangNhap_Brand");
            if (_lblTieuDe != null) _lblTieuDe.Text = LanguageManager.GetString("FormDangNhap_Header");
            if (_lblTenDangNhap != null) _lblTenDangNhap.Text = LanguageManager.GetString("FormDangNhap_Username");
            if (txtTenDangNhap != null) txtTenDangNhap.PlaceholderText = LanguageManager.GetString("FormDangNhap_UsernamePlaceholder");
            if (_lblMatKhau != null) _lblMatKhau.Text = LanguageManager.GetString("FormDangNhap_Password");
            if (txtMatKhau != null) txtMatKhau.PlaceholderText = LanguageManager.GetString("FormDangNhap_PasswordPlaceholder");

            // 3. Các nút bấm
            if (btnDangNhap != null) btnDangNhap.Text = LanguageManager.GetString("FormDangNhap_BtnLogin");
            if (_btnKiosk != null) _btnKiosk.Text = LanguageManager.GetString("FormDangNhap_BtnKiosk");
            if (_lblFooter != null) _lblFooter.Text = LanguageManager.GetString("FormDangNhap_Footer");

            // 4. Highlight nút ngôn ngữ
            if (_btnLangVi != null && _btnLangEn != null)
            {
                bool isEn = LanguageManager.CurrentLanguage == AppLanguage.English;
                if (isEn)
                {
                    AppTheme.ApplySecondaryButton(_btnLangVi);
                    AppTheme.ApplyPrimaryButton(_btnLangEn);
                }
                else
                {
                    AppTheme.ApplyPrimaryButton(_btnLangVi);
                    AppTheme.ApplySecondaryButton(_btnLangEn);
                }
            }
        }
    }
}

