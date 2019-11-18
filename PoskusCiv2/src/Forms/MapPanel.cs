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
        public static int BoxNoX { get; set; }      //No of visible squares on map
        public static int BoxNoY { get; set; }
        public static int OffsetX { get; set; }     //Offset squares from (0,0) for showing map
        public static int OffsetY { get; set; }

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

            BoxNoX = (int)Math.Floor((double)DrawPanel.Width / 64);
            BoxNoY = (int)Math.Floor((double)DrawPanel.Height / 32);
            OffsetX = 0;
            OffsetY = 0;
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
            //Draw map
            for (int col = 0; col < BoxNoX; col++)
                for (int row = 0; row < 2 * BoxNoY; row++)
                    e.Graphics.DrawImage(Game.Map[col, row].Graphic, 64 * col + 32 * (row % 2) + 1, 16 * row + 1);
                        
            //Draw cities
            StringFormat sf = new StringFormat();
            sf.LineAlignment = StringAlignment.Center;
            sf.Alignment = StringAlignment.Center;
            foreach (City city in Game.Cities) { 
                e.Graphics.DrawImage(city.Graphic, 32 * (city.X2 - OffsetX), 16 * (city.Y2 - OffsetY) - 16);
                e.Graphics.DrawString(city.Name, new Font("Times New Roman", 14.0f), new SolidBrush(Color.Black), 32 * (city.X2 - OffsetX) + 32 + 2, 16 * (city.Y2 - OffsetY) + 32, sf);    //Draw shadow around font
                e.Graphics.DrawString(city.Name, new Font("Times New Roman", 14.0f), new SolidBrush(Color.Black), 32 * (city.X2 - OffsetX) + 32, 16 * (city.Y2 - OffsetY) + 32 + 2, sf);    //Draw shadow around font
                e.Graphics.DrawString(city.Name, new Font("Times New Roman", 14.0f), new SolidBrush(CivColors.Light[city.Owner]), 32 * (city.X2 - OffsetX) + 32, 16 * (city.Y2 - OffsetY) + 32, sf); }
            sf.Dispose();

            //Draw units
            foreach (IUnit unit in Game.Units)
                if (unit == Game.Instance.ActiveUnit)
                    e.Graphics.DrawImage(unit.GraphicMapPanel, 32 * (unit.X2 - OffsetX), 16 * (unit.Y2 - OffsetY) - 16);
                else if (unit.IsInCity || (unit.IsInStack && unit.IsLastInStackList))


                //if (unit != Game.Instance.ActiveUnit) // && unit != MovingUnit)
                //{
                //    //Determine if unit inside city
                //    bool unitOnTopOfCity = false;
                //    foreach (City city in Game.Cities) if (unit.X == city.X && unit.Y == city.Y) { unitOnTopOfCity = true; break; }

                //    if (!unitOnTopOfCity && (unit.X != Game.Instance.ActiveUnit.X || unit.Y != Game.Instance.ActiveUnit.Y))   //Draw only if unit NOT inside city AND if active unit is not on same square
                //    {
                //        List<IUnit> unitsInXY = ListOfUnitsIn(unit.X, unit.Y);    //make a list of units on this X-Y square
                //                                                                  //if units are stacked, draw only the last unit in the list. Otherwise draw normally.
                //        if (unitsInXY.Count > 1) e.Graphics.DrawImage(unit.Grap(unitsInXY.Last(), true, 1), 32 * (unit.X2 - OffsetX), 16 * (unit.Y2 - OffsetY) - 16);
                //        //if (unitsInXY.Count > 1) e.Graphics.DrawImage(unit.Grap(unitsInXY.Last(), true, 1), 32 * (unit.X2 - OffsetX), 16 * (unit.Y2 - OffsetY) - 16);
                //        else e.Graphics.DrawImage(Draw.DrawUnit(unit, false, 1), 32 * (unit.X2 - OffsetX), 16 * (unit.Y2 - OffsetY) - 16);
                //    }
                //}

        }

        private List<IUnit> ListOfUnitsIn(int x, int y) //Return list of units on a X-Y square
        {
            List<IUnit> unitsInXY = new List<IUnit>();
            foreach (IUnit unit in Game.Units) if (unit.X == x && unit.Y == y) unitsInXY.Add(unit);
            return unitsInXY;
        }
    }
}
