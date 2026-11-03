using System;
using System.Drawing;
using HotelManagement.Localization;

namespace HotelManagement.Forms
{
    /// <summary>
    /// Phân đoạn xử lý đa ngôn ngữ cho FormKiosk (DCKLTTQ-86).
    /// </summary>
    public partial class FormKiosk : ILocalizableForm
    {
        public void CapNhatNgonNgu()
        {
            // 1. Tiêu đề Form
            this.Text = LanguageManager.GetString("FormKiosk_Title");

            // 2. Top Header
            if (_lblHeaderHotel != null) _lblHeaderHotel.Text = LanguageManager.GetString("FormKiosk_HeaderHotel");
            if (_lblHeaderSub != null) _lblHeaderSub.Text = LanguageManager.GetString("FormKiosk_HeaderSub");

            // Highlight nút ngôn ngữ
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

            // 3. Cột trái: Thẻ nhập liệu tra cứu
            if (_lblSearchTitle != null) _lblSearchTitle.Text = LanguageManager.GetString("FormKiosk_SearchTitle");
            if (_lblSearchSub != null) _lblSearchSub.Text = LanguageManager.GetString("FormKiosk_SearchSub");
            if (_lblMaBookingTitle != null) _lblMaBookingTitle.Text = LanguageManager.GetString("FormKiosk_MaBookingTitle");
            if (_txtMaBooking != null) _txtMaBooking.PlaceholderText = LanguageManager.GetString("FormKiosk_MaBookingPlaceholder");
            if (_lblSdtTitle != null) _lblSdtTitle.Text = LanguageManager.GetString("FormKiosk_SdtTitle");
            if (_txtSdt != null) _txtSdt.PlaceholderText = LanguageManager.GetString("FormKiosk_SdtPlaceholder");
            if (_btnTraCuu != null) _btnTraCuu.Text = LanguageManager.GetString("FormKiosk_BtnTraCuu");
            if (_btnXoaTrang != null) _btnXoaTrang.Text = LanguageManager.GetString("FormKiosk_BtnXoaTrang");

            // 4. Cột phải: Thẻ kết quả
            if (_lblResultTitle != null) _lblResultTitle.Text = LanguageManager.GetString("FormKiosk_ResultTitle");
            if (_lblPlaceholderText != null) _lblPlaceholderText.Text = LanguageManager.GetString("FormKiosk_PlaceholderText");
            if (_btnXoaKetQua != null) _btnXoaKetQua.Text = LanguageManager.GetString("FormKiosk_BtnXoaKetQua");
            if (_lblAutoClearCount != null)
            {
                _lblAutoClearCount.Text = string.Format(LanguageManager.GetString("FormKiosk_AutoClearCount"), _secondsRemaining);
            }

            // 5. Bottom Footer
            if (_lblHelpText != null) _lblHelpText.Text = LanguageManager.GetString("FormKiosk_HelpText");

            // 6. Nếu đang có kết quả hiển thị, cập nhật lại nhãn kết quả
            if (_currentBooking != null)
            {
                HienThiKetQuaBooking(_currentBooking);
            }
        }
    }
}

