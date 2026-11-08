using System.Drawing;
using System.Reflection;

namespace HotelManagement.Common
{
    public static class EmbeddedImages
    {
        private const string LogoResource = "HotelManagement.Resources.logo.png";
        private const string BackgroundResource = "HotelManagement.Resources.background.png";

        private static readonly Lazy<Bitmap> _logo = new(() => Load(LogoResource));
        private static readonly Lazy<Bitmap> _background = new(() => Load(BackgroundResource));

        public static Image Logo => _logo.Value;

        public static Image Background => _background.Value;

        private static Bitmap Load(string resourceName)
        {
            using Stream? stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
            if (stream == null)
                throw new InvalidOperationException($"Không tìm thấy ảnh nhúng: {resourceName}");

            using var image = Image.FromStream(stream);
            return new Bitmap(image);
        }
    }
}
