namespace HotelManagement.Localization
{
    /// <summary>
    /// Giao diện dành cho các Form hỗ trợ đa ngôn ngữ không cần khởi động lại (DCKLTTQ-86).
    /// </summary>
    public interface ILocalizableForm
    {
        /// <summary>
        /// Cập nhật toàn bộ Text của các control trên Form theo ngôn ngữ hiện hành.
        /// </summary>
        void CapNhatNgonNgu();
    }
}

