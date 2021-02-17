using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System;

namespace WinFormsUIExtensionMethods
{
    public static class ExtensionMethods
    {
        // Resize image
        public static Bitmap Resize(this Bitmap image, int zoom)
        {
            if (zoom == 0) return (Bitmap)image.Clone();

            int width = image.Width.ZoomScale(zoom);
            int height = image.Height.ZoomScale(zoom);

            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                //graphics.CompositingQuality = CompositingQuality.HighQuality;
                //graphics.CompositingQuality = CompositingQuality.AssumeLinear;
                graphics.CompositingQuality = CompositingQuality.HighSpeed;
                //graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
                //graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.SmoothingMode = SmoothingMode.HighSpeed;
                //graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.None;

                using var wrapMode = new ImageAttributes();
                wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
            }

            return destImage;
        }

        // Crop image
        public static Bitmap CropImage(this Bitmap img, Rectangle cropArea)
        {
            using var bmpImage = new Bitmap(img);
            return bmpImage.Clone(cropArea, bmpImage.PixelFormat);
        }

        // Return position of "i" relative to zoom
        public static int ZoomScale(this int i, int zoom)
        {
            return (int)((8.0 + (float)zoom) / 8.0 * i);
        }
    }
}
