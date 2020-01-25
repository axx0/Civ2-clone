using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using RTciv2.Enums;
using RTciv2.Imagery;

namespace RTciv2.Forms
{
    public partial class MinimapPanel : Civ2panel
    {
        DoubleBufferedPanel DrawPanel;

        //public void CreateMinimapPanel(int width, int height)
        public MinimapPanel(int width, int height)
        {
            Size = new Size(width, height);
            this.Paint += new PaintEventHandler(MinimapPanel_Paint);
            MapPanel.MapViewChangedEvent += ViewChangedInMapPanel;

            DrawPanel = new DoubleBufferedPanel()
            {
                Location = new Point(11, 38),
                Size = new Size(Width - 22, Height - 49),
                BackColor = Color.Black
            };
            Controls.Add(DrawPanel);
            DrawPanel.Paint += new PaintEventHandler(DrawPanel_Paint);
        }

        private void MinimapPanel_Paint(object sender, PaintEventArgs e)
        {
            //Title
            StringFormat sf = new StringFormat();
            sf.LineAlignment = StringAlignment.Center;
            sf.Alignment = StringAlignment.Center;
            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;
            e.Graphics.DrawString("World", new Font("Times New Roman", 14, FontStyle.Bold), new SolidBrush(Color.Black), new Point(this.Width / 2 + 1, 20 + 1), sf);
            e.Graphics.DrawString("World", new Font("Times New Roman", 14, FontStyle.Bold), new SolidBrush(Color.FromArgb(135, 135, 135)), new Point(this.Width / 2, 20), sf);
            sf.Dispose();
            //Draw line borders of panel
            e.Graphics.DrawLine(new Pen(Color.FromArgb(67, 67, 67)), 9, 36, 9 + (Width - 18 - 1), 36);   //1st layer of border
            e.Graphics.DrawLine(new Pen(Color.FromArgb(67, 67, 67)), 9, 36, 9, Height - 9 - 1);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(223, 223, 223)), Width - 9 - 1, 36, Width - 9 - 1, Height - 9 - 1);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(223, 223, 223)), 9, Height - 9 - 1, Width - 9 - 1, Height - 9 - 1);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(67, 67, 67)), 10, 37, 9 + (Width - 18 - 2), 37);   //2nd layer of border
            e.Graphics.DrawLine(new Pen(Color.FromArgb(67, 67, 67)), 10, 37, 10, Height - 9 - 2);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(223, 223, 223)), Width - 9 - 2, 37, Width - 9 - 2, Height - 9 - 2);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(223, 223, 223)), 10, Height - 9 - 2, Width - 9 - 2, Height - 9 - 2);
            e.Dispose();
        }

        private void DrawPanel_Paint(object sender, PaintEventArgs e)
        {
            //determine the offset of minimap from panel edges
            int[] offset = new int[] { (DrawPanel.Width - 2 * Data.MapXdim) / 2, (DrawPanel.Height - Data.MapYdim) / 2 };

            //draw map
            Color drawColor;
            for (int row = 0; row < Data.MapYdim; row++)
                for (int col = 0; col < Data.MapXdim; col++)
                {
                    drawColor = (Game.Map[col, row].Type == TerrainType.Ocean) ? Color.FromArgb(0, 0, 95) : Color.FromArgb(55, 123, 23);
                    e.Graphics.FillRectangle(new SolidBrush(drawColor), offset[0] + 2 * col + (row % 2), offset[1] + row, 2, 1);
                }

            //draw cities
            foreach (City city in Game.Cities)
                e.Graphics.FillRectangle(new SolidBrush(CivColors.CityTextColor[city.Owner]), offset[0] + city.X, offset[1] + city.Y, 2, 1);

            //draw current view rectangle
            e.Graphics.DrawRectangle(new Pen(Color.White), offset[0] + MapPanel.StartingSqXY[0], offset[1] + MapPanel.StartingSqXY[1], MapPanel.DrawingSqXY[0], MapPanel.DrawingSqXY[1]);
            e.Dispose();
        }
        //TODO: Make sure minimap rectangle is correct immediately after game loading

        private void ViewChangedInMapPanel()
        {
            DrawPanel.Refresh();
        }
    }
}
