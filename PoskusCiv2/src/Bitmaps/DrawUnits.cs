using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;

namespace PoskusCiv2.Imagery
{
    class DrawUnits
    {
        Bitmap unitsBitmap, unitShieldBitmap_;
        public Bitmap[] unitShieldBitmap = new Bitmap[8];    //for 8 civs
        public Bitmap[] unitBitmap = new Bitmap[63];
        public int[,] unitShieldLocation = new int[63,2];

        ImageAttributes imageAttributes = new ImageAttributes();
        ColorMap colorMap = new ColorMap();

        public DrawUnits()
        {
            //Read large bitmap UNITS.GIF
            unitsBitmap = new Bitmap(@"C:\DOS\CIV 2\Civ2\UNITS.GIF");

            //Extract individual unit bitmaps from large unit bitmap file
            for (int i = 0; i < 7; i++) //7 rows
            {
                for (int j = 0; j < 9; j++) //9 columns
                {
                    unitBitmap[i * 9 + j] = (Bitmap)unitsBitmap.Clone(new Rectangle(1 + 65 * j, 1 + 49 * i, 64, 48), unitsBitmap.PixelFormat);

                    //define transparent back colors
                    unitBitmap[i * 9 + j].MakeTransparent(Color.FromArgb(255, 0, 255));  //pink
                    unitBitmap[i * 9 + j].MakeTransparent(Color.FromArgb(135, 83, 135));  //gray

                    //determine where the unit shield is located (x-y)
                    for (int ix = 0; ix < 64; ix++) //in x-direction
                    {
                        if (unitsBitmap.GetPixel(65 * j + ix, 49 * i) == Color.FromArgb(0, 0, 255)) { unitShieldLocation[i * 9 + j, 0] = ix; }  //if pixel on border is blue
                    }
                    for (int iy = 0; iy < 48; iy++) //in y-direction
                    {
                        if (unitsBitmap.GetPixel(65 * j, 49 * i + iy) == Color.FromArgb(0, 0, 255)) { unitShieldLocation[i * 9 + j, 1] = iy; }
                    }
                }
            }

            //unitBitmap[1] = LoadGraphics.unitBitmap;

            //Extract unit shield
            unitShieldBitmap_ = (Bitmap)unitsBitmap.Clone(new Rectangle(597, 30, 12, 20), unitsBitmap.PixelFormat);
            //Make shields of different colors for 8 different civs
            unitShieldBitmap[0] = CreateNonIndexedImage(unitShieldBitmap_); //convert GIF to non-indexed picture
            unitShieldBitmap[1] = CreateNonIndexedImage(unitShieldBitmap_);
            unitShieldBitmap[2] = CreateNonIndexedImage(unitShieldBitmap_);
            unitShieldBitmap[3] = CreateNonIndexedImage(unitShieldBitmap_);
            unitShieldBitmap[4] = CreateNonIndexedImage(unitShieldBitmap_);
            unitShieldBitmap[5] = CreateNonIndexedImage(unitShieldBitmap_);
            unitShieldBitmap[6] = CreateNonIndexedImage(unitShieldBitmap_);
            unitShieldBitmap[7] = CreateNonIndexedImage(unitShieldBitmap_);
            //Replace colors
            for (int x = 0; x < 12; x++)
            {
                for (int y = 0; y < 20; y++)
                {
                    if (unitShieldBitmap_.GetPixel(x, y) == Color.FromArgb(255, 0, 255))    //pink
                    {
                        unitShieldBitmap[0].SetPixel(x, y, Color.FromArgb(243, 0, 0));  //red
                        unitShieldBitmap[1].SetPixel(x, y, Color.FromArgb(239, 239, 239));  //white
                        unitShieldBitmap[2].SetPixel(x, y, Color.FromArgb(87, 171, 39));    //green
                        unitShieldBitmap[3].SetPixel(x, y, Color.FromArgb(75, 95, 183));    //blue
                        unitShieldBitmap[4].SetPixel(x, y, Color.FromArgb(255, 255, 0));    //yellow
                        unitShieldBitmap[5].SetPixel(x, y, Color.FromArgb(55, 175, 191));   //cyan
                        unitShieldBitmap[6].SetPixel(x, y, Color.FromArgb(235, 131, 11));   //orange
                        unitShieldBitmap[7].SetPixel(x, y, Color.FromArgb(131, 103, 179));   //violet
                    }
                }
            }
   
        }

        //Converting GIFs to non-indexed images (required for SetPixel method)
        public Bitmap CreateNonIndexedImage(Image src)
        {
            Bitmap newBmp = new Bitmap(src.Width, src.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            using (Graphics gfx = Graphics.FromImage(newBmp))
            {
                gfx.DrawImage(src, 0, 0);
            }

            return newBmp;
        }
    }
}
