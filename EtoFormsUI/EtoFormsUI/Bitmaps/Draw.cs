using EtoFormsUIExtensionMethods;
using Eto.Drawing;
using Civ2engine;
using Civ2engine.MapObjects;

namespace EtoFormsUI
{
    public static partial class Draw
    {
        private static Game Game => Game.Instance;
        private static Map Map => Game.CurrentMap;

        public static void ViewPiece(Graphics g, int zoom, Point dest)
        {
            using var piecePic = MapImages.ViewPiece.Resize(zoom);
            g.DrawImage(piecePic, dest);
        }

        public static void Grid(Graphics g, int zoom, Point dest)
        {
            using var gridPic = MapImages.GridLines.Resize(zoom);
            g.DrawImage(gridPic, dest);
        }

        public static void Dither(Graphics g, int tileX, int tileY, int zoom, Point dest)
        {
            using var ditherPic = MapImages.Terrains[Map.MapIndex].DitherMask[tileX *2 + tileY].Resize(zoom);
            g.DrawImage(ditherPic, dest);
        }

        public static void BattleAnim(Graphics g, int frame, int zoom, Point dest)
        {
            using var battlePic = MapImages.BattleAnim[frame].Resize(zoom);
            g.DrawImage(battlePic, dest);
        }

        public static void Checkbox(Graphics g, bool check, Point dest)
        {
            g.AntiAlias = false;
            using var _brush2 = new SolidBrush(Colors.White);
            using var _brush3 = new SolidBrush(Color.FromArgb(128, 128, 128));
            using var _penB = new Pen(Colors.Black);
            using var _penW = new Pen(Colors.White);
            using var _penG = new Pen(Color.FromArgb(192, 192, 192));
            g.FillRectangle(_brush2, new Rectangle(dest.X + 3, dest.Y + 2, 15, 17));
            g.FillRectangle(_brush2, new Rectangle(dest.X + 2, dest.Y + 3, 17, 15));
            g.FillRectangle(_brush3, new Rectangle(dest.X + 4, dest.Y + 3, 13, 15));
            g.FillRectangle(_brush3, new Rectangle(dest.X + 3, dest.Y + 4, 15, 13));
            g.DrawLine(_penB, dest.X + 4, dest.Y + 3, dest.X + 16, dest.Y + 3);
            g.DrawLine(_penB, dest.X + 3, dest.Y + 4, dest.X + 3, dest.Y + 16);
            g.DrawLine(_penB, dest.X + 3, dest.Y + 4, dest.X + 4, dest.Y + 4);
            g.DrawLine(_penB, dest.X + 4, dest.Y + 19, dest.X + 18, dest.Y + 19);
            g.DrawLine(_penB, dest.X + 18, dest.Y + 18, dest.X + 19, dest.Y + 18);
            g.DrawLine(_penB, dest.X + 19, dest.Y + 4, dest.X + 19, dest.Y + 17);

            if (check)
            {
                g.DrawLine(_penB, dest.X + 21, dest.Y + 3, dest.X + 25, dest.Y + 3);
                g.DrawLine(_penB, dest.X + 20, dest.Y + 4, dest.X + 23, dest.Y + 4);
                g.DrawLine(_penB, dest.X + 19, dest.Y + 5, dest.X + 21, dest.Y + 5);
                g.DrawLine(_penB, dest.X + 18, dest.Y + 6, dest.X + 20, dest.Y + 6);
                g.DrawLine(_penB, dest.X + 17, dest.Y + 7, dest.X + 19, dest.Y + 7);
                g.DrawLine(_penB, dest.X + 16, dest.Y + 8, dest.X + 18, dest.Y + 8);
                g.DrawLine(_penB, dest.X + 15, dest.Y + 9, dest.X + 17, dest.Y + 9);
                g.DrawLine(_penB, dest.X + 5, dest.Y + 10, dest.X + 6, dest.Y + 10);
                g.DrawLine(_penB, dest.X + 14, dest.Y + 10, dest.X + 16, dest.Y + 10);
                g.DrawLine(_penB, dest.X + 6, dest.Y + 11, dest.X + 7, dest.Y + 11);
                g.DrawLine(_penB, dest.X + 14, dest.Y + 11, dest.X + 16, dest.Y + 11);
                g.DrawLine(_penB, dest.X + 7, dest.Y + 12, dest.X + 8, dest.Y + 12);
                g.DrawLine(_penB, dest.X + 13, dest.Y + 12, dest.X + 15, dest.Y + 12);
                g.DrawLine(_penB, dest.X + 8, dest.Y + 13, dest.X + 14, dest.Y + 13);
                g.DrawLine(_penB, dest.X + 12, dest.Y + 13, dest.X + 15, dest.Y + 13);
                g.DrawLine(_penB, dest.X + 12, dest.Y + 14, dest.X + 14, dest.Y + 14);
                g.DrawLine(_penB, dest.X + 9, dest.Y + 15, dest.X + 12, dest.Y + 15);
                g.DrawLine(_penB, dest.X + 10, dest.Y + 16, dest.X + 12, dest.Y + 16);
                g.DrawLine(_penB, dest.X + 11, dest.Y + 16, dest.X + 11, dest.Y + 17);
                g.DrawLine(_penW, dest.X + 20, dest.Y + 1, dest.X + 22, dest.Y + 1);
                g.DrawLine(_penW, dest.X + 19, dest.Y + 2, dest.X + 20, dest.Y + 2);
                g.DrawLine(_penG, dest.X + 20, dest.Y + 2, dest.X + 22, dest.Y + 2);
                g.DrawLine(_penW, dest.X + 18, dest.Y + 3, dest.X + 19, dest.Y + 3);
                g.DrawLine(_penG, dest.X + 19, dest.Y + 3, dest.X + 20, dest.Y + 3);
                g.DrawLine(_penW, dest.X + 17, dest.Y + 4, dest.X + 18, dest.Y + 4);
                g.DrawLine(_penG, dest.X + 18, dest.Y + 4, dest.X + 19, dest.Y + 4);
                g.DrawLine(_penW, dest.X + 16, dest.Y + 5, dest.X + 17, dest.Y + 5);
                g.DrawLine(_penG, dest.X + 17, dest.Y + 5, dest.X + 18, dest.Y + 5);
                g.DrawLine(_penW, dest.X + 15, dest.Y + 6, dest.X + 16, dest.Y + 6);
                g.DrawLine(_penG, dest.X + 16, dest.Y + 6, dest.X + 17, dest.Y + 6);
                g.DrawLine(_penW, dest.X + 14, dest.Y + 7, dest.X + 15, dest.Y + 7);
                g.DrawLine(_penG, dest.X + 15, dest.Y + 7, dest.X + 16, dest.Y + 7);
                g.DrawLine(_penW, dest.X + 4, dest.Y + 8, dest.X + 5, dest.Y + 8);
                g.DrawLine(_penG, dest.X + 5, dest.Y + 8, dest.X + 5, dest.Y + 9);
                g.DrawLine(_penW, dest.X + 13, dest.Y + 8, dest.X + 14, dest.Y + 8);
                g.DrawLine(_penG, dest.X + 14, dest.Y + 8, dest.X + 15, dest.Y + 8);
                g.DrawLine(_penG, dest.X + 6, dest.Y + 9, dest.X + 6, dest.Y + 10);
                g.DrawLine(_penG, dest.X + 13, dest.Y + 9, dest.X + 14, dest.Y + 9);
                g.DrawLine(_penG, dest.X + 7, dest.Y + 10, dest.X + 7, dest.Y + 11);
                g.DrawLine(_penW, dest.X + 12, dest.Y + 10, dest.X + 13, dest.Y + 10);
                g.DrawLine(_penG, dest.X + 13, dest.Y + 10, dest.X + 13, dest.Y + 11);
                g.DrawLine(_penG, dest.X + 8, dest.Y + 11, dest.X + 8, dest.Y + 12);
                g.DrawLine(_penW, dest.X + 11, dest.Y + 11, dest.X + 12, dest.Y + 11);
                g.DrawLine(_penG, dest.X + 12, dest.Y + 11, dest.X + 13, dest.Y + 11);
                g.DrawLine(_penG, dest.X + 9, dest.Y + 12, dest.X + 9, dest.Y + 13);
                g.DrawLine(_penG, dest.X + 11, dest.Y + 12, dest.X + 12, dest.Y + 12);
                g.DrawLine(_penG, dest.X + 9, dest.Y + 13, dest.X + 11, dest.Y + 13);
                g.DrawLine(_penG, dest.X + 9, dest.Y + 14, dest.X + 11, dest.Y + 14);
                g.DrawLine(_penG, dest.X + 10, dest.Y + 14, dest.X + 10, dest.Y + 15);
            }
        }

        public static void RadioBtn(Graphics g, bool selected, Point dest)
        {
            g.AntiAlias = false;
            g.FillEllipse(Color.FromArgb(128, 128, 128), new Rectangle(dest.X, dest.Y, 16, 16));
            g.DrawEllipse(Colors.Black, new Rectangle(dest.X + 1, dest.Y + 1, 16, 16));
            g.FillRectangle(new SolidBrush(Colors.Black), new Rectangle(dest.X + 1, dest.Y + 4, 2, 3));
            g.FillRectangle(new SolidBrush(Colors.Black), new Rectangle(dest.X + 3, dest.Y + 2, 2, 2));
            g.FillRectangle(new SolidBrush(Colors.Black), new Rectangle(dest.X + 6, dest.Y + 1, 1, 1));
            g.FillRectangle(new SolidBrush(Colors.Black), new Rectangle(dest.X + 11, dest.Y + 15, 3, 2));
            g.FillRectangle(new SolidBrush(Colors.Black), new Rectangle(dest.X + 14, dest.Y + 13, 2, 2));
            g.FillRectangle(new SolidBrush(Colors.Black), new Rectangle(dest.X + 16, dest.Y + 11, 1, 1));
            g.DrawEllipse(Colors.White, new Rectangle(dest.X, dest.Y, 16, 16));

            if (!selected)
            {
                g.FillRectangle(new SolidBrush(Color.FromArgb(192, 192, 192)), new Rectangle(dest.X + 6, dest.Y + 4, 5, 9));
                g.FillRectangle(new SolidBrush(Color.FromArgb(192, 192, 192)), new Rectangle(dest.X + 4, dest.Y + 6, 9, 5));
                g.FillRectangle(new SolidBrush(Colors.White), new Rectangle(dest.X + 5, dest.Y + 11, 1, 1));
                g.FillRectangle(new SolidBrush(Colors.White), new Rectangle(dest.X + 4, dest.Y + 6, 1, 5));
                g.FillRectangle(new SolidBrush(Colors.White), new Rectangle(dest.X + 5, dest.Y + 5, 1, 2));
                g.FillRectangle(new SolidBrush(Colors.White), new Rectangle(dest.X + 6, dest.Y + 4, 1, 2));
                g.FillRectangle(new SolidBrush(Colors.White), new Rectangle(dest.X + 7, dest.Y + 4, 4, 1));
                g.FillRectangle(new SolidBrush(Colors.White), new Rectangle(dest.X + 11, dest.Y + 5, 1, 1));
                g.FillRectangle(new SolidBrush(Color.FromArgb(192, 192, 192)), new Rectangle(dest.X + 11, dest.Y + 11, 1, 1));
                g.FillRectangle(new SolidBrush(Colors.Black), new Rectangle(dest.X + 7, dest.Y + 13, 4, 1));
                g.FillRectangle(new SolidBrush(Colors.Black), new Rectangle(dest.X + 11, dest.Y + 12, 1, 1));
                g.FillRectangle(new SolidBrush(Colors.Black), new Rectangle(dest.X + 12, dest.Y + 11, 1, 1));
                g.FillRectangle(new SolidBrush(Colors.Black), new Rectangle(dest.X + 13, dest.Y + 7, 1, 4));
            }
            else
            {
                g.FillRectangle(new SolidBrush(Color.FromArgb(192, 192, 192)), new Rectangle(dest.X + 7, dest.Y + 4, 4, 10));
                g.FillRectangle(new SolidBrush(Color.FromArgb(192, 192, 192)), new Rectangle(dest.X + 4, dest.Y + 7, 10, 4));
                g.FillRectangle(new SolidBrush(Color.FromArgb(192, 192, 192)), new Rectangle(dest.X + 6, dest.Y + 5, 6, 8));
                g.FillRectangle(new SolidBrush(Color.FromArgb(192, 192, 192)), new Rectangle(dest.X + 5, dest.Y + 6, 8, 6));
                g.FillRectangle(new SolidBrush(Colors.Black), new Rectangle(dest.X + 7, dest.Y + 6, 4, 6));
                g.FillRectangle(new SolidBrush(Colors.Black), new Rectangle(dest.X + 6, dest.Y + 7, 6, 4));
            }
        }
    }
}
