using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using ExtensionMethods;
using RTciv2.Imagery;
using RTciv2.Units;

namespace RTciv2.Forms
{
    public partial class MapPanel : Civ2panel
    {
        private static DoubleBufferedPanel DrawPanel;
        private int MapGridVar { get; set; }    //style of map grid presentation        
        private bool ViewingPiecesMode { get; set; }
        Label HelpLabel;

        public MapPanel(int width, int height)
        {
            Size = new Size(width, height);
            this.Paint += new PaintEventHandler(MapPanel_Paint);

            DrawPanel = new DoubleBufferedPanel() 
            {
                Location = new Point(11, 38),
                Size = new Size(Width - 22, Height - 49),
                BackColor = Color.Green 
            };
            Controls.Add(DrawPanel);
            DrawPanel.Paint += DrawPanel_Paint;
            DrawPanel.MouseClick += DrawPanel_MouseClick;

            Button ZoomINButton = new Button
            {
                Location = new Point(11, 9),
                Size = new Size(23, 23),
                FlatStyle = FlatStyle.Flat,
                BackgroundImage = ModifyImage.ResizeImage(Images.ZoomIN, 23, 23)
            };
            ZoomINButton.FlatAppearance.BorderSize = 0;
            Controls.Add(ZoomINButton);
            ZoomINButton.Click += ZoomINclicked;

            Button ZoomOUTButton = new Button 
            {
                Location = new Point(36, 9),
                Size = new Size(23, 23),
                FlatStyle = FlatStyle.Flat,
                BackgroundImage = ModifyImage.ResizeImage(Images.ZoomOUT, 23, 23) 
            };
            ZoomOUTButton.FlatAppearance.BorderSize = 0;
            Controls.Add(ZoomOUTButton);
            ZoomOUTButton.Click += ZoomOUTclicked;

            //Initialize variables
            ZoomLvl = 8;  // TODO: zoom needs to be read from SAV
            ClickedXY = new int[] { 0, 0 };
            MapGridVar = 0;
            ViewingPiecesMode = false;

            //!!! HELP !!!
            HelpLabel = new Label
            {
                Location = new Point(1000, 50),
                AutoSize = true,
                BackColor = Color.White,
                Text = "OK"
            };
            DrawPanel.Controls.Add(HelpLabel);
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
            Console.WriteLine("Drawing: offset map={0},{1}", MapOffsetXY[0], MapOffsetXY[1]);
            Console.WriteLine("Drawing: vis sq={0},{1}", DrawingSqXY[0], DrawingSqXY[1]);
            //Draw map
            //int[] coords = Ext.Civ2xy(MapOffsetXY);  //convert civ2 to real coords
            //for (int col = 0; col < DrawingSqXY[0] / 2; col++)
            //    for (int row = 0; row < DrawingSqXY[1]; row++)
            //        e.Graphics.DrawImage(ModifyImage.ResizeImage(Game.Map[coords[0] + col, coords[1] + row].Graphic, ZoomLvl * 8, ZoomLvl * 4), ZoomLvl * 8 * col + ZoomLvl * 4 * (row % 2), ZoomLvl * 2 * row);

            //Draw units
            //foreach (IUnit unit in Game.Units.Where(n => n.IsInView))
            //    if (unit == Game.Instance.ActiveUnit) e.Graphics.DrawImage(unit.GraphicMapPanel, 4 * ZoomLvl * (unit.X2 - MapOffsetXY[0]), 2 * ZoomLvl * (unit.Y2 - MapOffsetXY[1]) - 2 * ZoomLvl);
            //    else if (!(unit.IsInCity || (unit.IsInStack && unit.IsLastInStack))) e.Graphics.DrawImage(unit.GraphicMapPanel, 4 * ZoomLvl * (unit.X2 - MapOffsetXY[0]), 2 * ZoomLvl * (unit.Y2 - MapOffsetXY[1]) - 2 * ZoomLvl);

            //Draw cities
            //foreach (City city in Game.Cities.Where(n => n.IsInView))
            //{
            //    e.Graphics.DrawImage(city.Graphic, 4 * ZoomLvl * (city.X2 - MapOffsetXY[0]), 2 * ZoomLvl * (city.Y2 - MapOffsetXY[1]) - 2 * ZoomLvl);
            //    e.Graphics.DrawImage(city.TextGraphic, 4 * ZoomLvl * (city.X2 - MapOffsetXY[0]) + 4 * ZoomLvl - city.TextGraphic.Width / 2, 2 * ZoomLvl * (city.Y2 - MapOffsetXY[1]) - 2 * ZoomLvl + ZoomLvl * 5);
            //}

            //Draw gridlines
            //if (Options.Grid)
            //{
            //    StringFormat sf = new StringFormat();
            //    sf.LineAlignment = StringAlignment.Center;
            //    sf.Alignment = StringAlignment.Center;
            //    for (int col = MapOffsetXY[0]; col < MapOffsetXY[0] + DrawingSqXY[0] / 2; col++)
            //        for (int row = MapOffsetXY[1]; row < MapOffsetXY[1] + DrawingSqXY[1]; row++)
            //        {
            //            if (MapGridVar > 0) e.Graphics.DrawImage(ModifyImage.ResizeImage(Images.GridLines, 8 * ZoomLvl, 4 * ZoomLvl), 8 * ZoomLvl * col + 4 * ZoomLvl * (row % 2), 2 * ZoomLvl * row);
            //            if (MapGridVar == 2)     //XY coords
            //                e.Graphics.DrawString(String.Format("({0},{1})", col + MapOffsetXY[0], row + MapOffsetXY[1]), new Font("Arial", ZoomLvl), new SolidBrush(Color.Yellow), 8 * ZoomLvl * col + 4 * ZoomLvl * (row % 2) + 4 * ZoomLvl, 2 * ZoomLvl * row + 2 * ZoomLvl, sf);
            //            if (MapGridVar == 3)    //civXY coords
            //            {
            //                coords = Ext.XYciv2(new int[] { col + MapOffsetXY[0], row + MapOffsetXY[1] });
            //                e.Graphics.DrawString(String.Format("({0},{1})", coords[0], coords[1]), new Font("Arial", ZoomLvl), new SolidBrush(Color.Yellow), 8 * ZoomLvl * col + 4 * ZoomLvl * (row % 2) + 4 * ZoomLvl, 2 * ZoomLvl * row + 2 * ZoomLvl, sf);
            //            }
            //        }
            //    sf.Dispose();
            //}

            StringFormat sf = new StringFormat();
            sf.LineAlignment = StringAlignment.Center;
            sf.Alignment = StringAlignment.Center;
            for (int col = StartingSqXY[0]; col < StartingSqXY[0] + DrawingSqXY[0]; col++)
                for (int row = StartingSqXY[1]; row < StartingSqXY[1] + DrawingSqXY[1]; row++)
                {
                    e.Graphics.DrawImage(Images.GridLines, StartingOffsetXY[0] + 64 * col + 32 * (row % 2), StartingOffsetXY[1] + 16 * row);
                    e.Graphics.DrawString(String.Format("({0},{1})", col, row), new Font("Arial", 8), new SolidBrush(Color.Yellow), StartingOffsetXY[0] + 64 * col + 32 * (row % 2) + 32, StartingOffsetXY[1] + 16 * row + 16, sf);
                }

            HelpLabel.Text = "Panel size = (" + DrawPanel.Width.ToString() + "," + DrawPanel.Height.ToString() + ")\n" +
                "CenterOffsetXY = (" + CenterOffsetXY[0].ToString() + "," + CenterOffsetXY[1].ToString() + ")\n" +
                "DrawingSqXY = (" + DrawingSqXY[0].ToString() + "," + DrawingSqXY[1].ToString() + ")\n" +
                "StartingOffsetXY = (" + StartingOffsetXY[0].ToString() + "," + StartingOffsetXY[1].ToString() + ") px\n" +
                "StartingSqXY = (" + StartingSqXY[0].ToString() + "," + StartingSqXY[1].ToString() + ")\n";
        }

        private void DrawPanel_MouseClick(object sender, MouseEventArgs e)
        {
            Console.WriteLine("before offset={0},{1}", MapOffsetXY[0], MapOffsetXY[1]);
            Console.WriteLine("center={0},{1}", CenterOffsetXY[0], CenterOffsetXY[1]);

            ClickedXY = PxToCoords(e.Location.X, e.Location.Y); // (X,Y) coordinates of clicked square
            Console.WriteLine("Offset if not limited={0},{1}", MapOffsetXY[0] + ClickedXY[0] - CenterOffsetXY[0], MapOffsetXY[1] + ClickedXY[1] - CenterOffsetXY[1]);
            MapOffsetXY = new int[] { MapOffsetXY[0] + ClickedXY[0] - CenterOffsetXY[0], MapOffsetXY[1] + ClickedXY[1] - CenterOffsetXY[1] }; // offset of shown map from (0,0)

            Console.WriteLine("Clicked={0},{1}", ClickedXY[0], ClickedXY[1]);
            Console.WriteLine("Offset after limit={0},{1}", MapOffsetXY[0], MapOffsetXY[1]);
            Console.WriteLine("Vis sq={0},{1}", DrawingSqXY[0], DrawingSqXY[1]);
            DrawPanel.Refresh();

            if (e.Button == MouseButtons.Right)
            {
                ViewingPiecesMode = true;   //with right-click you activate viewing pieces mode in status form
                //Application.OpenForms.OfType<MainCiv2Window>().First().UpdateOrdersMenu();  //update orders menu in main screen
                //mainCiv2Window.statusForm.ReceiveMousePositionFromMapForm();  //send mouse click location to status form
            }
            else
            {
                //mainCiv2Window.statusForm.ReceiveMousePositionFromMapForm();   //send mouse click location to status form
                if (Game.Cities.Any(city => city.X2 == ClickedXY[0] && city.Y2 == ClickedXY[1]))    //if city is clicked => open form
                {
                    //CityForm cityForm = new CityForm(this, Game.Cities.Find(city => city.X2 == ClickedXY[0] && city.Y2 == ClickedXY[1]));
                    //cityForm.Show();
                }
            }

            //Update active box coordinates. If viewing pieces mode is off, the active unit determines coords.
            //if (ViewingPiecesMode)
            //{
            //    ActiveXY[0] = ClickedXY[0];
            //    ActiveXY[1] = ClickedXY[1];
            //}
            //else
            //{
            //    ActiveXY[0] = Game.Instance.ActiveUnit.X2;
            //    ActiveXY[1] = Game.Instance.ActiveUnit.Y2;
            //}
        }

        private int[] StartingSqXY
        {
            get { return new int[] { 0, 0 }; }
        }
        private int[] StartingOffsetXY
        {
            get { return new int[] { 0, 0 }; }
        }

        public static int[] DrawingSqXY  //Squares to be drawn on the panel (must be multiple of 2)
        {
            //get { return new int[] { 2 * (int)Math.Ceiling((double)DrawPanel.Width / (8 * ZoomLvl)), 2 * (int)Math.Ceiling((double)DrawPanel.Height / (4 * ZoomLvl)) }; }
            get { return new int[] { 2 * (int)Math.Ceiling((double)DrawPanel.Width / 64), 2 * (int)Math.Ceiling((double)DrawPanel.Height / 32) }; }
        }

        private int[] CenterOffsetXY  //offset of central tile from panel NW corner (civ2 coords)
        {
            get { return PxToCoords(DrawPanel.Width /2, DrawPanel.Height / 2); }
        }

        private static int[] _mapOffsetXY;
        public static int[] MapOffsetXY //Starting map coordinates (civ2 coords)
        {
            get 
            {
                if (_mapOffsetXY == null) _mapOffsetXY = new int[] { 0, 0 };
                return _mapOffsetXY; 
            }
            set
            {
                //Do not allow to move out of map bounds by limiting offset
                Console.WriteLine("SET OFFSET BEF ={0},{1}", _mapOffsetXY[0], _mapOffsetXY[1]);
                _mapOffsetXY = value;
                Console.WriteLine("SET OFFSET AFT ={0},{1}", _mapOffsetXY[0], _mapOffsetXY[1]);
                if (value[0] < 0)
                {
                    Console.WriteLine("LIMIT#1"); 
                    _mapOffsetXY[0] = 0;
                }
                if (value[0] >= 2 * Game.Data.MapXdim - DrawingSqXY[0]) 
                {
                    Console.WriteLine("LIMIT#2"); 
                    _mapOffsetXY[0] = 2 * Game.Data.MapXdim - DrawingSqXY[0];
                }
                if (value[1] < 0) 
                {
                    Console.WriteLine("LIMIT#3"); 
                    _mapOffsetXY[1] = 0;
                }
                if (value[1] >= Game.Data.MapYdim - DrawingSqXY[1]) 
                {
                    Console.WriteLine("LIMIT#4"); 
                    _mapOffsetXY[1] = Game.Data.MapYdim - DrawingSqXY[1];
                }
                Console.WriteLine("SET OFFSET AFT LIM ={0},{1}", _mapOffsetXY[0], _mapOffsetXY[1]);
                //After limiting offset, do not allow XY combinations not in civ2 style (e.g. (2, 1))
                if (Math.Abs((_mapOffsetXY[0] - _mapOffsetXY[1]) % 2) == 1)
                {
                    if (_mapOffsetXY[0] + 1 < 2 * Game.Data.MapXdim) _mapOffsetXY[0]++;
                    else if (_mapOffsetXY[1] + 1 < Game.Data.MapYdim) _mapOffsetXY[1]++;
                    else if (_mapOffsetXY[0] - 1 > 0) _mapOffsetXY[0]--;
                    else _mapOffsetXY[1]--;
                }
                Console.WriteLine("SET OFFSET AFT LIMv2 ={0},{1}", _mapOffsetXY[0], _mapOffsetXY[1]);
            }
        }
        public int[] ClickedXY     //Civ2 coords of clicked tile
        { 
            get; 
            set; 
        }

        private int[] ActiveXY  //Currently active box (active unit or viewing piece), civ2 coords
        {
            get { return new int[] { Game.Instance.ActiveUnit.X, Game.Instance.ActiveUnit.Y }; }
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

        private static int _zoomLvl;
        public static int ZoomLvl 
        {
            get { return _zoomLvl; }
            set 
            { 
                _zoomLvl = Math.Max(Math.Min(value, 16), 1);
                DrawPanel.Refresh();
            }
        }

        public void ZoomOUTclicked(Object sender, EventArgs e) { ZoomLvl--; }
        public void ZoomINclicked(Object sender, EventArgs e) { ZoomLvl++; }
        public void MaxZoomINclicked(Object sender, EventArgs e) { ZoomLvl = 16; }
        public void MaxZoomOUTclicked(Object sender, EventArgs e) { ZoomLvl = 1; }
        public void StandardZOOMclicked(Object sender, EventArgs e) { ZoomLvl = 8; }
        public void MediumZoomOUTclicked(Object sender, EventArgs e) { ZoomLvl = 5; }

        private int[] PxToCoords(int x, int y)  //determine XY civ2 coords from x-y pixel location on panel
        {
            double[] nxy = new double[] { x - 2 * y, -(-y - 0.5 * x) };  //crossing at x,y-axis
            int[] nXY = new int[] { Convert.ToInt32(Math.Floor((nxy[0] + 4 * ZoomLvl) / (8 * ZoomLvl))), Convert.ToInt32(Math.Floor((nxy[1] - 2 * ZoomLvl) / (4 * ZoomLvl))) };   //converting crossing to int
            return new int[] { nXY[0] + nXY[1], nXY[1] - nXY[0] };
        }

    }
}
