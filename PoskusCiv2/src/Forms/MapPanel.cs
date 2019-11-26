using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using RTciv2.Imagery;
using RTciv2.Units;

namespace RTciv2.Forms
{
    public partial class MapPanel : Civ2panel
    {
        DoubleBufferedPanel DrawPanel;
        public static int[] MapVisSqXY { get; set; }      //Visible map squares shown on the panel
        public static int[] MapOffsetXY { get; set; }     //Starting map coordinates
        private int MapGridVar { get; set; }
        private int ZoomLvl { get; set; } //Needs to be read from SAV !!!

        public MapPanel(int width, int height)
        {
            Size = new Size(width, height);
            this.Paint += new PaintEventHandler(MapPanel_Paint);

            DrawPanel = new DoubleBufferedPanel()
            {
                Location = new Point(11, 38),
                Size = new Size(Width - 22, Height - 49),
                BackColor = Color.Black
            };
            Controls.Add(DrawPanel);
            DrawPanel.Paint += DrawPanel_Paint;

            Button ZoomOUTButton = new Button
            {
                Location = new Point(11, 9),
                Size = new Size(23, 23),
                FlatStyle = FlatStyle.Flat,
                BackgroundImage = ModifyImage.ResizeImage(Images.ZoomOUT, 23, 23)
            };
            ZoomOUTButton.FlatAppearance.BorderSize = 0;
            Controls.Add(ZoomOUTButton);
            ZoomOUTButton.Click += ZoomOUTclicked;

            Button ZoomINButton = new Button
            {
                Location = new Point(36, 9),
                Size = new Size(23, 23),
                FlatStyle = FlatStyle.Flat,
                BackgroundImage = ModifyImage.ResizeImage(Images.ZoomIN, 23, 23)
            };
            ZoomINButton.FlatAppearance.BorderSize = 0;
            Controls.Add(ZoomINButton);
            ZoomINButton.Click += ZoomINclicked;

            MapVisSqXY = new int[] { (int)Math.Ceiling((double)DrawPanel.Width / 64), (int)Math.Ceiling((double)DrawPanel.Height / 16) };
            MapOffsetXY = new int[] { 0, 0 };
            MapGridVar = 0;

            ZoomLvl = 8;  // TODO: zoom needs to be read from SAV
        }

        private void MapPanel_Paint(object sender, PaintEventArgs e)
        {
            //Title
            StringFormat sf = new StringFormat();
            sf.LineAlignment = StringAlignment.Center;
            sf.Alignment = StringAlignment.Center;
            //Civilization humanPlayer = Game.Civs.Find(civ => civ.Id == Game.Data.HumanPlayerUsed);
            e.Graphics.DrawString("Roman Map", new Font("Times New Roman", 18), new SolidBrush(Color.Black), new Point(this.Width / 2 + 1, 20 + 1), sf);
            e.Graphics.DrawString("Roman Map", new Font("Times New Roman", 18), new SolidBrush(Color.FromArgb(135, 135, 135)), new Point(this.Width / 2, 20), sf);
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
        }

        private void DrawPanel_Paint(object sender, PaintEventArgs e)
        {
            Console.WriteLine("MapVisSqXY={0},{1}", MapVisSqXY[0], MapVisSqXY[1]);
            Console.WriteLine("Panel={0}", DrawPanel.Size);
            Console.WriteLine("ZoomLvl={0}", ZoomLvl);
            //Draw map
            for (int col = MapOffsetXY[0]; col < Math.Min(MapVisSqXY[0], Game.Map.GetLength(0)); col++)
                for (int row = MapOffsetXY[1]; row < Math.Min(MapVisSqXY[1], Game.Map.GetLength(1)); row++)
                    e.Graphics.DrawImage(ModifyImage.ResizeImage(Game.Map[col, row].Graphic, ZoomLvl * 8, ZoomLvl * 4), ZoomLvl * 8 * col + ZoomLvl * 4 * (row % 2), ZoomLvl * 2 * row);

            //Draw cities
            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;  //make text not blurry
            StringFormat sf = new StringFormat();
            sf.LineAlignment = StringAlignment.Center;
            sf.Alignment = StringAlignment.Center;
            foreach (City city in Game.Cities.Where(n => n.IsInView))
            { // TODO: move this to Cities
                //e.Graphics.DrawImage(ModifyImage.ResizeImage(city.Graphic, ZoomLvl * 8, ZoomLvl * 4), 32 * (city.X2 - MapOffsetXY[0]), 16 * (city.Y2 - MapOffsetXY[1]) - 16);
                e.Graphics.DrawImage(city.Graphic, 32 * (city.X2 - MapOffsetXY[0]), 16 * (city.Y2 - MapOffsetXY[1]) - 16);
                //e.Graphics.DrawString(city.Name, new Font("Times New Roman", 14.0f), new SolidBrush(Color.Black), 32 * (city.X2 - MapOffsetXY[0]) + 32 + 2, 16 * (city.Y2 - MapOffsetXY[1]) + 32, sf);    //Draw shadow around font
                //e.Graphics.DrawString(city.Name, new Font("Times New Roman", 14.0f), new SolidBrush(Color.Black), 32 * (city.X2 - MapOffsetXY[0]) + 32, 16 * (city.Y2 - MapOffsetXY[1]) + 32 + 2, sf);    //Draw shadow around font
                e.Graphics.DrawString(city.Name, new Font("Times New Roman", 1), new SolidBrush(Color.Cyan), 32 * (city.X2 - MapOffsetXY[0]) + 32, 16 * (city.Y2 - MapOffsetXY[1]) + 32, sf);
            }
            sf.Dispose();

            //Draw units
            foreach (IUnit unit in Game.Units.Where(n => n.IsInView))
                if (unit == Game.Instance.ActiveUnit) e.Graphics.DrawImage(unit.GraphicMapPanel, 32 * (unit.X2 - MapOffsetXY[0]), 16 * (unit.Y2 - MapOffsetXY[1]) - 16);
                else if (!(unit.IsInCity || (unit.IsInStack && unit.IsLastInStack))) e.Graphics.DrawImage(unit.GraphicMapPanel, 32 * (unit.X2 - MapOffsetXY[0]), 16 * (unit.Y2 - MapOffsetXY[1]) - 16);

            //Draw gridlines
            if (Options.Grid)
            {
                for (int col = 0; col < MapVisSqXY[0]; col++)
                    for (int row = 0; row < MapVisSqXY[1]; row++)
                    {
                        if (MapGridVar > 0) e.Graphics.DrawImage(Images.GridLines, 64 * col + 32 * (row % 2), 16 * row);
                        if (MapGridVar == 2)    //XY coords
                        {
                            int x = col * 64 + 12;
                            int y = row * 32 + 8;
                            e.Graphics.DrawString(String.Format("({0},{1})", col + MapOffsetXY[0], row + MapOffsetXY[1]), new Font("Arial", 8), new SolidBrush(Color.Yellow), x, y, new StringFormat()); //for first horizontal line
                            e.Graphics.DrawString(String.Format("({0},{1})", col + MapOffsetXY[0] + 1, row + MapOffsetXY[1] + 1), new Font("Arial", 8), new SolidBrush(Color.Yellow), x + 32, y + 16, new StringFormat()); //for second horizontal line
                        }
                        if (MapGridVar == 3)    //civXY coords
                        {
                            int x = col * 64 + 12;
                            int y = row * 32 + 8;
                            e.Graphics.DrawString(String.Format("({0},{1})", 2 * col + MapOffsetXY[0], 2 * row + MapOffsetXY[1]), new Font("Arial", 8), new SolidBrush(Color.Yellow), x, y, new StringFormat()); //for first horizontal line
                            e.Graphics.DrawString(String.Format("({0},{1})", 2 * col + 1 + MapOffsetXY[0], 2 * row + 1 + MapOffsetXY[1]), new Font("Arial", 8), new SolidBrush(Color.Yellow), x + 32, y + 16, new StringFormat()); //for second horizontal line
                        }
                    }
            }

        }

        public int ToggleMapGrid()
        {
            MapGridVar++;
            if (MapGridVar > 3) MapGridVar = 0;
            if (MapGridVar != 0) Options.Grid = true;
            else Options.Grid = false;
            Refresh();
            return MapGridVar;
        }

        private void ZoomOUTclicked(Object sender, EventArgs e)
        {
            ZoomLvl = Math.Max(ZoomLvl - 1, 1);
            MapVisSqXY = new int[] { (int)Math.Ceiling((double)DrawPanel.Width / (ZoomLvl * 8)), (int)Math.Ceiling((double)DrawPanel.Height / (ZoomLvl * 2)) };
            Refresh();
        }

        private void ZoomINclicked(Object sender, EventArgs e)
        {
            ZoomLvl = Math.Min(ZoomLvl + 1, 16);
            MapVisSqXY = new int[] { (int)Math.Ceiling((double)DrawPanel.Width / (ZoomLvl * 8)), (int)Math.Ceiling((double)DrawPanel.Height / (ZoomLvl * 2)) };
            Refresh();
        }
    }
}
