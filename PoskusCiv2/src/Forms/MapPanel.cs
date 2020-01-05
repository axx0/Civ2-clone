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

        public delegate void SendCoords(int[] rectStartCoords, int[] rectSize);
        public event SendCoords SendCoordsEvent;

        public void CreateMapPanel(int width, int height)
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
            //TODO: Implement zoom

            //Uncomment this for help in drawing-logic
            //HelpLabel = new Label
            //{
            //    Location = new Point(1000, 50),
            //    AutoSize = true,
            //    BackColor = Color.White,
            //    Text = "OK"
            //};
            //DrawPanel.Controls.Add(HelpLabel);
        }

        private void MapPanel_Paint(object sender, PaintEventArgs e)
        {
            //Title
            StringFormat sf = new StringFormat();
            sf.LineAlignment = StringAlignment.Center;
            sf.Alignment = StringAlignment.Center;
            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;
            e.Graphics.DrawString($"{Game.Civs[Data.HumanPlayer].Adjective} Map", new Font("Times New Roman", 15, FontStyle.Bold), new SolidBrush(Color.Black), new Point(this.Width / 2 + 1, 20 + 1), sf);
            e.Graphics.DrawString($"{Game.Civs[Data.HumanPlayer].Adjective} Map", new Font("Times New Roman", 15, FontStyle.Bold), new SolidBrush(Color.FromArgb(135, 135, 135)), new Point(this.Width / 2, 20), sf);
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
        //Draw map
        StringFormat sf = new StringFormat();
        sf.LineAlignment = StringAlignment.Center;
        sf.Alignment = StringAlignment.Center;
        for (int row = 0; row < DrawingSqXY[1] - EdgeDrawOffsetXY[1] + EdgeDrawOffsetXY[3]; row++)
            for (int col = 0; col < DrawingSqXY[0] - EdgeDrawOffsetXY[0] + EdgeDrawOffsetXY[2]; col++)
                if (Math.Abs(row - col) % 2 == 0)   //choose which squares
                {
                    //TILES
                    int[] coords = Ext.Civ2xy(new int[] { col + StartingSqXY[0] + EdgeDrawOffsetXY[0], row + StartingSqXY[1] + EdgeDrawOffsetXY[1] });
                    e.Graphics.DrawImage(ModifyImage.ResizeImage(Game.Map[coords[0], coords[1]].Graphic, ZoomLvl * 8, ZoomLvl * 4), DrawingPxOffsetXY[0] + 32 * col, DrawingPxOffsetXY[1] + 16 * row);
                }

            //UNITS
            foreach (IUnit unit in Game.Units)
                if (UnitIsInView(unit) && unit != Game.Instance.ActiveUnit && !(unit.IsInCity || (unit.IsInStack && unit.IsLastInStack)))
                    e.Graphics.DrawImage(unit.GraphicMapPanel, 4 * ZoomLvl * (unit.X - StartingSqXY[0] - EdgeDrawOffsetXY[0]), 2 * ZoomLvl * (unit.Y - StartingSqXY[1] - EdgeDrawOffsetXY[1]) - 2 * ZoomLvl);

            //CITIES
            foreach (City city in Game.Cities)
                if (CityIsInView(city))
                {
                    e.Graphics.DrawImage(city.Graphic, 4 * ZoomLvl * (city.X - StartingSqXY[0] - EdgeDrawOffsetXY[0]), 2 * ZoomLvl * (city.Y - StartingSqXY[1] - EdgeDrawOffsetXY[1]) - 2 * ZoomLvl);
                    e.Graphics.DrawImage(city.TextGraphic, 4 * ZoomLvl * (city.X - StartingSqXY[0] - EdgeDrawOffsetXY[0]) + 4 * ZoomLvl - city.TextGraphic.Width / 2, 2 * ZoomLvl * (city.Y - StartingSqXY[1] - EdgeDrawOffsetXY[1]) - 2 * ZoomLvl + ZoomLvl * 5);
                }

            //ACTIVE UNIT
            if ((Game.Instance.ActiveUnit != null) & UnitIsInView(Game.Instance.ActiveUnit))
                e.Graphics.DrawImage(Game.Instance.ActiveUnit.GraphicMapPanel, 4 * ZoomLvl * (Game.Instance.ActiveUnit.X - StartingSqXY[0] - EdgeDrawOffsetXY[0]), 2 * ZoomLvl * (Game.Instance.ActiveUnit.Y - StartingSqXY[1] - EdgeDrawOffsetXY[1]) - 2 * ZoomLvl);

            //GRIDLINES
            if (Options.Grid)
            {
                Color brushColor;
                for (int row = 0; row < DrawingSqXY[1] - EdgeDrawOffsetXY[1] + EdgeDrawOffsetXY[3]; row++)
                    for (int col = 0; col < DrawingSqXY[0] - EdgeDrawOffsetXY[0] + EdgeDrawOffsetXY[2]; col++)
                        if (Math.Abs(row - col) % 2 == 0)   //choose which squares
                        {
                            brushColor = ((col + StartingSqXY[0] + EdgeDrawOffsetXY[0] == CenterSqXY[0]) && (row + StartingSqXY[1] + EdgeDrawOffsetXY[1] == CenterSqXY[1])) ? Color.Red : Color.Yellow; //color central tile red
                            if (MapGridVar > 0)
                                e.Graphics.DrawImage(Images.GridLines, DrawingPxOffsetXY[0] + 32 * col, DrawingPxOffsetXY[1] + 16 * row);
                            if (MapGridVar == 2)     //Map coords from SAVfile logic
                            {
                                int[] realCoords = Ext.Civ2xy(new int[] { col + StartingSqXY[0] + EdgeDrawOffsetXY[0], row + StartingSqXY[1] + EdgeDrawOffsetXY[1] });
                                e.Graphics.DrawString(String.Format($"({realCoords[0]},{realCoords[1]})"), new Font("Arial", 8), new SolidBrush(brushColor), DrawingPxOffsetXY[0] + 32 * col + 32, DrawingPxOffsetXY[1] + 16 * row + 16, sf);
                            }
                            if (MapGridVar == 3)    //Civ2-coords
                                e.Graphics.DrawString(String.Format($"({col + StartingSqXY[0] + EdgeDrawOffsetXY[0]},{row + StartingSqXY[1] + EdgeDrawOffsetXY[1]})"), new Font("Arial", 8), new SolidBrush(brushColor), DrawingPxOffsetXY[0] + 32 * col + 32, DrawingPxOffsetXY[1] + 16 * row + 16, sf);
                        }
            }

            //Uncomment this for help in drawing-logic
            //HelpLabel.Text = $"Panel size = ({DrawPanel.Width},{DrawPanel.Height})\n" +
            //    $"CenterDistanceXY = ({CenterDistanceXY[0]},{CenterDistanceXY[1]})\n" +
            //    $"DrawingSqXY = ({DrawingSqXY[0]},{DrawingSqXY[1]})\n" +
            //    $"DrawingPxOffsetXY = ({DrawingPxOffsetXY[0]},{DrawingPxOffsetXY[1]}) px\n" +
            //    $"StartingSqXY = ({StartingSqXY[0]},{StartingSqXY[1]})\n" +
            //    $"EdgeDrawOffsetXY = ({EdgeDrawOffsetXY[0]},{EdgeDrawOffsetXY[1]},{EdgeDrawOffsetXY[2]},{EdgeDrawOffsetXY[3]})\n" +
            //    $"CenterSqXY = ({CenterSqXY[0]},{CenterSqXY[1]})";

            e.Dispose();
            sf.Dispose();
        }

        private void DrawPanel_MouseClick(object sender, MouseEventArgs e)
        {
            ClickedXY = PxToCoords(e.Location.X, e.Location.Y); // (X,Y) coordinates of clicked square
            StartingSqXY = new int[] { StartingSqXY[0] + ClickedXY[0] - CenterDistanceXY[0], StartingSqXY[1] + ClickedXY[1] - CenterDistanceXY[1] };

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
                if (Game.Cities.Any(city => city.X == ClickedXY[0] && city.Y == ClickedXY[1]))    //if city is clicked => open form
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
            if (SendCoordsEvent != null) 
                SendCoordsEvent.Invoke(StartingSqXY, DrawingSqXY);  //send dimensions of current view
        }

        private int[] _startingSqXY;
        private int[] StartingSqXY
        {
            get 
            { 
                if (_startingSqXY == null) _startingSqXY = new int[] { 0, 0 };
                return _startingSqXY;
            }
            set 
            {
                //limit movement so that map limits are not exceeded
                if (value[0] < 0 && value[1] < 0)    //movement beyond upper & left edge
                    _startingSqXY = new int[] { 0, 0 };
                else if ((value[0] + DrawingSqXY[0] >= 2 * Data.MapXdim) && value[1] < 0)    //movement beyond upper & right edge
                    _startingSqXY = new int[] { 2 * Data.MapXdim - DrawingSqXY[0], 0 };
                else if (value[0] < 0 && (value[1] + DrawingSqXY[1] >= Data.MapYdim))    //movement beyond lower & left edge
                    _startingSqXY = new int[] { 0, Data.MapYdim - DrawingSqXY[1] };
                else if ((value[0] + DrawingSqXY[0] >= 2 * Data.MapXdim) && (value[1] + DrawingSqXY[1] >= Data.MapYdim))    //movement beyond lower & right edge
                    _startingSqXY = new int[] { 2 * Data.MapXdim - DrawingSqXY[0], Data.MapYdim - DrawingSqXY[1] };
                else if (value[0] < 0)     //movement beyond left edge
                    _startingSqXY = new int[] { value[1] % 2, value[1] };
                else if (value[1] < 0)     //movement beyond upper edge
                    _startingSqXY = new int[] { value[0], value[0] % 2 };
                else if (value[0] + DrawingSqXY[0] >= 2 * Data.MapXdim)     //movement beyond right edge
                    _startingSqXY = new int[] { 2 * Data.MapXdim - DrawingSqXY[0] - value[1] % 2, value[1] };
                else if (value[1] + DrawingSqXY[1] >= Data.MapYdim)     //movement beyond bottom edge
                    _startingSqXY = new int[] { value[0], Data.MapYdim - DrawingSqXY[1] - value[0] % 2 };
                else 
                    _startingSqXY = value;
            }
        }

        private int[] _edgeDrawOffsetXY;
        private int[] EdgeDrawOffsetXY  //determines offset to StartingSqXY for drawing of squares on panel edge { left, up, right, down }
        {
            get
            {
                _edgeDrawOffsetXY = new int[] { -2, -2, 2, 2 }; //by default draw 2 squares more in each direction
                if (StartingSqXY[0] == 0 || StartingSqXY[1] == 0)   //starting on edge
                {
                    _edgeDrawOffsetXY[0] = -Math.Max(Math.Min(StartingSqXY[0], 2), 0);
                    _edgeDrawOffsetXY[1] = -Math.Max(Math.Min(StartingSqXY[1], 2), 0);
                }
                if (StartingSqXY[0] == 1 || StartingSqXY[1] == 1)  //starting in 1st row/column
                {
                    _edgeDrawOffsetXY[0] = -1;
                    _edgeDrawOffsetXY[1] = -1;
                }
                if (StartingSqXY[0] + DrawingSqXY[0] == 2 * Data.MapXdim)  //on right edge
                {
                    _edgeDrawOffsetXY[2] = 0;
                    _edgeDrawOffsetXY[3] = Math.Min(Data.MapYdim - DrawingSqXY[1] - StartingSqXY[1], 2);
                }
                if (StartingSqXY[1] + DrawingSqXY[1] == Data.MapYdim)  //on bottom edge
                {
                    _edgeDrawOffsetXY[2] = Math.Min(2 * Data.MapXdim - DrawingSqXY[0] - StartingSqXY[0], 2);
                    _edgeDrawOffsetXY[3] = 0;
                }
                if (StartingSqXY[0] + DrawingSqXY[0] == 2 * Data.MapXdim - 1)  //1 column left of right edge
                {
                    _edgeDrawOffsetXY[2] = 1;
                    _edgeDrawOffsetXY[3] = Math.Min(Data.MapYdim - DrawingSqXY[1] - StartingSqXY[1], 2);
                }
                if (StartingSqXY[1] + DrawingSqXY[1] == Data.MapYdim - 1)  //1 column up of bottom edge
                {
                    _edgeDrawOffsetXY[2] = Math.Min(2 * Data.MapXdim - DrawingSqXY[0] - StartingSqXY[0], 2);
                    _edgeDrawOffsetXY[3] = 1;
                }
                return _edgeDrawOffsetXY; 
            }
        }

        private int[] _drawingPxOffsetXY;
        private int[] DrawingPxOffsetXY   //in px
        {
            get 
            {
                _drawingPxOffsetXY = new int[] { 32 * EdgeDrawOffsetXY[0], 16 * EdgeDrawOffsetXY[1] };
                if (StartingSqXY[0] + DrawingSqXY[0] == 2 * Data.MapXdim) _drawingPxOffsetXY[0] = DrawPanel.Width - (32 + 32 * DrawingSqXY[0] - 32 * EdgeDrawOffsetXY[0]);
                if (StartingSqXY[1] + DrawingSqXY[1] == Data.MapYdim) _drawingPxOffsetXY[1] = DrawPanel.Height - (16 + 16 * DrawingSqXY[1] - 16 * EdgeDrawOffsetXY[1]);
                return _drawingPxOffsetXY; 
            }
        }

        private int[] _drawingSqXY;
        private int[] DrawingSqXY  //Squares to be drawn on the panel
        {
            //get { return new int[] { 2 * (int)Math.Ceiling((double)DrawPanel.Width / (8 * ZoomLvl)), 2 * (int)Math.Ceiling((double)DrawPanel.Height / (4 * ZoomLvl)) }; }
            get 
            {
                _drawingSqXY = new int[] { (int)Math.Floor(((double)DrawPanel.Width - 32) / 32), (int)Math.Floor(((double)DrawPanel.Height - 16) / 16) };
                return _drawingSqXY;
            }
        }

        private int[] CenterSqXY
        {
            get { return new int[] { CenterDistanceXY[0] + StartingSqXY[0], CenterDistanceXY[1] + StartingSqXY[1] }; }
        }

        private int[] CenterDistanceXY  //offset of central tile from panel NW corner (civ2 coords)
        {
            get { return PxToCoords(DrawPanel.Width / 2, DrawPanel.Height / 2); }            
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
            Options.Grid = (MapGridVar != 0) ? true : false;
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

        private bool UnitIsInView(IUnit unit)   //Determine if unit can be seen in current map view
        {
            bool isInView;
            if ((unit.X >= StartingSqXY[0] + EdgeDrawOffsetXY[0]) && 
                (unit.X <= StartingSqXY[0] + DrawingSqXY[0] + EdgeDrawOffsetXY[0] + EdgeDrawOffsetXY[2]) &&
                (unit.Y >= StartingSqXY[1] + EdgeDrawOffsetXY[1]) &&
                (unit.Y <= StartingSqXY[1] + DrawingSqXY[1] + EdgeDrawOffsetXY[1] + EdgeDrawOffsetXY[3])) isInView = true;
            else isInView = false;
            return isInView;
        }

        private bool CityIsInView(City city)   //Determine if city can be seen in current map view
        {
            bool isInView;
            if ((city.X >= StartingSqXY[0] + EdgeDrawOffsetXY[0]) &&
                (city.X <= StartingSqXY[0] + DrawingSqXY[0] + EdgeDrawOffsetXY[0] + EdgeDrawOffsetXY[2]) &&
                (city.Y >= StartingSqXY[1] + EdgeDrawOffsetXY[1]) &&
                (city.Y <= StartingSqXY[1] + DrawingSqXY[1] + EdgeDrawOffsetXY[1] + EdgeDrawOffsetXY[3])) isInView = true;
            else isInView = false;
            return isInView;
        }
    }
}
