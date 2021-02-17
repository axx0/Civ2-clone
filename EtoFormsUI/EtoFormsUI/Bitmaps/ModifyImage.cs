using System.Drawing;
using System.Drawing.Imaging;

namespace EtoFormsUI
{
    public static class ModifyImage
    {
        // Merge 2 images
        public static Bitmap MergedBitmaps(Bitmap original, Bitmap layer, int x, int y)
        {
            // Bitmap result = new Bitmap(original.Width, original.Height);
            Bitmap result = original;

            using (Graphics g = Graphics.FromImage(result))
            {
                g.DrawImage(layer, x, y);
            }
            return result;
        }

        // Grey out an image
        public static ImageAttributes ConvertToGray()
        {
            var imageAttributes = new ImageAttributes();

            float[][] colorMatrixElements = {
                new float[] { 0, 0, 0, 0, 0},        // red
                new float[] { 0, 0, 0,  0, 0},        // green
                new float[] { 0, 0, 0, 0, 0},        // blue
                new float[] { 0,  0,  0,  1, 0},        // alpha scaling
                new float[] { 0.529f, 0.529f, 0.529f,  0, 1}};    // translations

            var colorMatrix = new ColorMatrix(colorMatrixElements);

            imageAttributes.SetColorMatrix(
               colorMatrix,
               ColorMatrixFlag.Default,
               ColorAdjustType.Bitmap);

            return imageAttributes;
        }
    }
}
