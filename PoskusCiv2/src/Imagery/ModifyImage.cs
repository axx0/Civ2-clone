using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace RTciv2.Imagery
{
    class ModifyImage
    {
        // <summary>
        // Resize the image to the specified width and height.
        // </summary>
        // <param name="image">The image to resize.</param>
        // <param name="width">The width to resize to.</param>
        // <param name="height">The height to resize to.</param>
        // <returns>The resized image.</returns>
        public static Bitmap ResizeImage(Image image, int width, int height)
        {
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

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

        //Crop image
        public static Bitmap CropImage(Bitmap img, Rectangle cropArea)
        {
            Bitmap bmpImage = new Bitmap(img);
            return bmpImage.Clone(cropArea, bmpImage.PixelFormat);
        }

        //Merge 2 images
        public static Bitmap MergedBitmaps(Bitmap original, Bitmap layer, int x, int y)
        {
            //Bitmap result = new Bitmap(original.Width, original.Height);
            Bitmap result = original;

            using (Graphics g = Graphics.FromImage(result))
            {
                //g.DrawImage(bmp2, Point.Empty);
                g.DrawImage(layer, x, y);
            }
            return result;
        }

        //Grey out an image
        public static ImageAttributes ConvertToGray()
        {
            ImageAttributes imageAttributes = new ImageAttributes();

            float[][] colorMatrixElements = {
                new float[] { 0, 0, 0, 0, 0},        // red
                new float[] { 0, 0, 0,  0, 0},        // green
                new float[] { 0, 0, 0, 0, 0},        // blue
                new float[] { 0,  0,  0,  1, 0},        // alpha scaling
                new float[] { 0.529f, 0.529f, 0.529f,  0, 1}};    // translations
            
            ColorMatrix colorMatrix = new ColorMatrix(colorMatrixElements);

            imageAttributes.SetColorMatrix(
               colorMatrix,
               ColorMatrixFlag.Default,
               ColorAdjustType.Bitmap);

            return imageAttributes;
        }
    }
}
