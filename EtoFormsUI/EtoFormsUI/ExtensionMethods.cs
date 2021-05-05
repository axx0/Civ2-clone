using System;
using Eto.Drawing;

namespace EtoFormsUIExtensionMethods
{
    public static class ExtensionMethods
    {
        // Resize image (according to zoom)
        public static Bitmap Resize(this Bitmap image, int zoom)
        {
            if (zoom == 0) return image.Clone();

            int width = image.Width.ZoomScale(zoom);
            int height = image.Height.ZoomScale(zoom);

            //var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height, PixelFormat.Format32bppRgba);
            //var destImage = new Bitmap(image, width, height, ImageInterpolation.Default);

            //destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            //using (var graphics = Graphics.FromImage(destImage))
            //{
            //graphics.CompositingMode = CompositingMode.SourceCopy;
            ////graphics.CompositingQuality = CompositingQuality.HighQuality;
            ////graphics.CompositingQuality = CompositingQuality.AssumeLinear;
            //graphics.CompositingQuality = CompositingQuality.HighSpeed;
            ////graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            //graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
            ////graphics.SmoothingMode = SmoothingMode.HighQuality;
            //graphics.SmoothingMode = SmoothingMode.HighSpeed;
            ////graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            //graphics.PixelOffsetMode = PixelOffsetMode.None;

            //using var wrapMode = new ImageAttributes();
            //wrapMode.SetWrapMode(WrapMode.TileFlipXY);
            //graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);

            using var g = new Graphics(destImage) {AntiAlias = false, ImageInterpolation = ImageInterpolation.None};
            g.DrawImage(image, 0, 0, width, height);

            return destImage;
        }

        // Resize image (in both directions)
        public static Bitmap Resize(this Bitmap image, double scaleX, double scaleY)
        {
            int width = (int)(image.Width * scaleX);
            int height = (int)(image.Height * scaleY);

            var destImage = new Bitmap(width, height, PixelFormat.Format32bppRgba);

            using (var g = new Graphics(destImage))
            {
                g.AntiAlias = false;
                g.ImageInterpolation = ImageInterpolation.None;
                g.DrawImage(image, 0, 0, width, height);
            }

            return destImage;
        }

        // Crop image
        public static Bitmap CropImage(this Bitmap img, Rectangle cropArea)
        {
            using var bmpImage = new Bitmap(img);
            return bmpImage.Clone(cropArea);
        }

        // Make transparent
        public static void ReplaceColors(this Bitmap orig, Color origColor, Color replacementColor)
        {
            for (int col = 0; col < orig.Width; col++)
            {
                for (int row = 0; row < orig.Height; row++)
                {
                    if (orig.GetPixel(col, row) == origColor)
                    {
                        orig.SetPixel(col, row, replacementColor);
                    }
                }
            }
        }

        // Return position of "i" relative to zoom
        public static int ZoomScale(this int i, int zoom)
        {
            return (int)((8.0 + (float)zoom) / 8.0 * i);
        }
    }
}
