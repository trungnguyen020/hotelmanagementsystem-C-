using System;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Windows.Forms;

namespace HotelManagement.Localization
{
    /// <summary>
    /// Bộ quản lý đa ngôn ngữ tập trung theo cơ chế Không Restart (DCKLTTQ-86).
    /// Đọc chuỗi hiển thị từ Resources.resx (Tiếng Việt) và Resources.en.resx (Tiếng Anh).
    /// </summary>
    public static class LanguageManager
    {
        private static readonly ResourceManager _resourceManager =
            new ResourceManager("HotelManagement.Resources.Resources", typeof(LanguageManager).Assembly);

        private static AppLanguage _currentLanguage = AppLanguage.Vietnamese;

        /// <summary>
        /// Ngôn ngữ hiện tại của ứng dụng.
        /// </summary>
        public static AppLanguage CurrentLanguage => _currentLanguage;

        /// <summary>
        /// Sự kiện phát ra khi ngôn ngữ ứng dụng thay đổi.
        /// </summary>
        public static event Action? LanguageChanged;

        static LanguageManager()
        {
            ApplyCulture(_currentLanguage);
        }

        /// <summary>
        /// Chuyển đổi ngôn ngữ và cập nhật tức thời toàn bộ các Form đang hiển thị.
        /// </summary>
        public static void SetLanguage(AppLanguage language)
        {
            if (_currentLanguage == language)
                return;

            _currentLanguage = language;
            ApplyCulture(_currentLanguage);

            // Duyệt và gọi cập nhật giao diện trên toàn bộ Form đang mở mà không cần restart
            var openForms = Application.OpenForms.Cast<Form>().ToList();
            foreach (var form in openForms)
            {
                if (form is ILocalizableForm localizableForm)
                {
                    try
                    {
                        localizableForm.CapNhatNgonNgu();
                    }
                    catch
                    {
                        // Đảm bảo lỗi ở một Form không làm ngắt cập nhật các Form khác
                    }
                }
            }

            LanguageChanged?.Invoke();
        }

        /// <summary>
        /// Lấy chuỗi bản dịch theo khóa từ file resource tương ứng với ngôn ngữ đang chọn.
        /// </summary>
        public static string GetString(string key, string? defaultValue = null)
        {
            try
            {
                var culture = GetCulture(_currentLanguage);
                string? value = _resourceManager.GetString(key, culture);
                return !string.IsNullOrEmpty(value) ? value : (defaultValue ?? key);
            }
            catch
            {
                return defaultValue ?? key;
            }
        }

        public static CultureInfo GetCulture(AppLanguage language)
        {
            return language switch
            {
                AppLanguage.English => new CultureInfo("en-US"),
                _ => new CultureInfo("vi-VN")
            };
        }

        private static void ApplyCulture(AppLanguage language)
        {
            var culture = GetCulture(language);
            CultureInfo.CurrentCulture = culture;
            CultureInfo.CurrentUICulture = culture;
        }
    }
}

