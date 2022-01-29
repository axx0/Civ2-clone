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

        // Return position of "i" relative to zoom
        public static int ZoomScale(this int i, int zoom)
        {
            return (int)((8.0 + (float)zoom) / 8.0 * i);
        }

        /// <summary>
        /// Replace colors in bitmap
        /// </summary>
        /// <param name="bmp">Bitmap</param>
        /// <param name="origColor">Color to be replaced</param>
        /// <param name="replacementColor">Replacement color</param>
        unsafe public static void ReplaceColors(this Bitmap bmp, Color origColor, Color replacementColor)
        {
            using var bmpData = bmp.Lock();
            byte* scan0 = (byte*)bmpData.Data;
            var bitsPP = bmpData.BitsPerPixel;
            var height = bmp.Height;
            var width = bmp.Width;
            for (int i = 0; i < height; ++i)
            {
                for (int j = 0; j < width; ++j)
                {
                    byte* data = scan0 + i * bmpData.ScanWidth + j * bitsPP / 8;
                    if (data[2] == origColor.Rb && data[1] == origColor.Gb && data[0] == origColor.Bb)
                    {
                        data[0] = (byte)replacementColor.Bb; // B
                        data[1] = (byte)replacementColor.Gb; // G
                        data[2] = (byte)replacementColor.Rb; // R
                    }
                }
            }
        }

        /// <summary>
        /// Convert bitmap to single color
        /// </summary>
        /// <param name="bmp">Bitmap</param>
        /// <param name="targetColor">Target color</param>
        unsafe public static void ToSingleColor(this Bitmap bmp, Color targetColor)
        {
            using var bmpData = bmp.Lock();
            byte* scan0 = (byte*)bmpData.Data;
            var bitsPP = bmpData.BitsPerPixel;
            var height = bmp.Height;
            var width = bmp.Width;
            for (int i = 0; i < height; ++i)
            {
                for (int j = 0; j < width; ++j)
                {
                    byte* data = scan0 + i * bmpData.ScanWidth + j * bitsPP / 8;
                    if (!(data[3] == Colors.Transparent.Ab && data[2] == Colors.Transparent.Rb && data[1] == Colors.Transparent.Gb && data[0] == Colors.Transparent.Bb))
                    {
                        data[0] = (byte)targetColor.Bb; // B
                        data[1] = (byte)targetColor.Gb; // G
                        data[2] = (byte)targetColor.Rb; // R
                    }
                }
            }
        }

        /// <summary>
        /// Set transparency of colors in bitmap
        /// </summary>
        /// <param name="bmp">Bitmap</param>
        /// <param name="colors">Colors to make transparent</param>
        unsafe public static void SetTransparent(this Bitmap bmp, Color[] colors)
        {
            using var bmpData = bmp.Lock();
            byte* scan0 = (byte*)bmpData.Data;
            var bitsPP = bmpData.BitsPerPixel;
            var height = bmp.Height;
            var width = bmp.Width;
            for (int i = 0; i < height; ++i)
            {
                for (int j = 0; j < width; ++j)
                {
                    byte* data = scan0 + i * bmpData.ScanWidth + j * bitsPP / 8;
                    for (int c = 0; c < colors.Length; c++)
                    {
                        if (data[2] == colors[c].Rb && data[1] == colors[c].Gb && data[0] == colors[c].Bb)
                        {
                            data[0] = 0x00; // B
                            data[1] = 0x00; // G
                            data[2] = 0x00; // R
                            data[3] = 0x00; // alpha
                        }
                    }
                }
            }
        }
    }
}
