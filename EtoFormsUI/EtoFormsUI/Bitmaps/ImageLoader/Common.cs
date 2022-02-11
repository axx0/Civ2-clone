using System.Collections.Generic;
using Civ2engine;
using Eto.Drawing;

namespace EtoFormsUI.ImageLoader
{
    public static class Common
    {
        /// <summary>
        //  Read file in local directory. If it doesn't exist there, read it in root civ2 directory.
        /// </summary>
        /// <param name="name">the filename to load</param>
        /// <param name="path">the local directory to load from</param>
        /// <returns></returns>
        internal static Bitmap LoadBitmapFrom(string name, IEnumerable<string> paths)
        {
            var filePath = Utils.GetFilePath(name, paths, "bmp", "gif");

            return filePath == null ? new Bitmap(640, 480, PixelFormat.Format32bppRgba) : new Bitmap(filePath);
        }


    }
}