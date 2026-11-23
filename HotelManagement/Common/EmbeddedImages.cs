using System.Drawing;
using System.Reflection;

namespace HotelManagement.Common
{
    public static class EmbeddedImages
    {
        private const string LogoResource = "HotelManagement.Resources.logo.png";
        private const string BackgroundResource = "HotelManagement.Resources.background.png";
        private const string AvatarResource = "HotelManagement.Resources.avatar.png";
        private const string EditResource = "HotelManagement.Resources.edit.png";
        private const string DeleteResource = "HotelManagement.Resources.delete.png";

        private static readonly Lazy<Bitmap> _logo = new(() => Load(LogoResource));
        private static readonly Lazy<Bitmap> _background = new(() => Load(BackgroundResource));
        private static readonly Lazy<Bitmap> _avatar = new(() => Load(AvatarResource));
        private static readonly Lazy<Bitmap> _edit = new(() => Load(EditResource));
        private static readonly Lazy<Bitmap> _delete = new(() => Load(DeleteResource));

        private static readonly Lazy<Bitmap> _editSmall = new(() => ScaleImage(_edit.Value, 20, 20));
        private static readonly Lazy<Bitmap> _deleteSmall = new(() => ScaleImage(_delete.Value, 20, 20));

        public static Image Logo => _logo.Value;

        public static Image Background => _background.Value;

        public static Image Avatar => _avatar.Value;

        public static Image Edit => _edit.Value;

        public static Image Delete => _delete.Value;

        public static Image EditSmall => _editSmall.Value;

        public static Image DeleteSmall => _deleteSmall.Value;

        public static Bitmap ScaleImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using var graphics = Graphics.FromImage(destImage);
            graphics.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;
            graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

            using var wrapMode = new System.Drawing.Imaging.ImageAttributes();
            wrapMode.SetWrapMode(System.Drawing.Drawing2D.WrapMode.TileFlipXY);
            graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);

            return destImage;
        }

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
