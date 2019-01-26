using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PoskusCiv2.Enums;
using PoskusCiv2.Units;
using PoskusCiv2.Imagery;

namespace PoskusCiv2.Forms
{
    public partial class MapForm : Civ2form
    {
        public MainCiv2Window mainCiv2Window;
        CreateUnitForm createUnitForm = new CreateUnitForm();
        DoubleBufferedPanel MapPanel;
        Random randomNo = new Random();
        Timer t_VP = new Timer();   //viewing pieces timer
        Timer t_Unit = new Timer(); //unit animation timer
        Draw Draw = new Draw();
        Pen pulsatingRectPen = new Pen(Color.White, 1);

        public static int BoxNoX { get; set; }      //No of visible squares on map
        public static int BoxNoY { get; set; }
        public static int CenterBoxX { get; set; }  //Square in the center of figure
        public static int CenterBoxY { get; set; }
        public static int OffsetX { get; set; }     //Offset squares from (0,0) for showing map
        public static int OffsetY { get; set; }
        public static int ClickedBoxX { get; set; } //Currently clicked box with mouse
        public static int ClickedBoxY { get; set; }
        public static int ActiveBoxX { get; set; }  //Currently active box (active unit or viewing piece)
        public static int ActiveBoxY { get; set; }
        public static bool ViewingPiecesMode { get; set; }
        public bool UnitIsMoving { get; set; }
        public int TimerVPCount { get; set; }
        public int TimerUnitCount { get; set; }
        public int AnimationStartX { get; set; }
        public int AnimationStartY { get; set; }
        public bool CreateUnit { get; set; }
        public bool GridIsChecked { get; set; }
        public bool DrawXYnumbers { get; set; }
        public IUnit MovingUnit { get; set; }

        public MapForm(MainCiv2Window _mainCiv2Window)
        {
            InitializeComponent();
            mainCiv2Window = _mainCiv2Window;
            Size = new Size((int)((_mainCiv2Window.ClientSize.Width) * 0.8625 - 6), _mainCiv2Window.ClientSize.Height - 30);    //-4 is experience setting
            Paint += new PaintEventHandler(MapForm_Paint);

            //Panel for map
            MapPanel = new DoubleBufferedPanel
            {
                Location = new Point(8 + 2, 38 + 2),    //2 because of panel border
                Size = new Size(this.ClientSize.Width - 19 - 4, this.ClientSize.Height - 47 - 4),   //4 because of panel border
                BackColor = Color.Black
            };
            Controls.Add(MapPanel);
            MapPanel.Paint += new PaintEventHandler(MapPanel_Paint);
            MapPanel.MouseClick += new MouseEventHandler(MapPanel_MouseClick);

            //Initialize variables
            GridIsChecked = false;
            DrawXYnumbers = false;
            ViewingPiecesMode = false;
            CreateUnit = false;
            UnitIsMoving = false;
            TimerVPCount = 0;   //records no of timer ticks
            TimerUnitCount = 0;
            AnimationStartX = 0;    //starting tile for animating unit movement
            AnimationStartY = 0;
            MovingUnit = null;
            //for calculation of moving with mouse in MapForm   
            BoxNoX = (int)Math.Floor((double)MapPanel.Width / 64);   //No of squares in X and Y direction
            BoxNoY = (int)Math.Floor((double)MapPanel.Height / 32);
            CenterBoxX = (int)Math.Ceiling((double)BoxNoX / 2); //Determine the square in the center of figure
            CenterBoxY = (int)Math.Ceiling((double)BoxNoY / 2);
            OffsetX = 0; //starting offset from (0,0)
            OffsetY = 0;
            ActiveBoxX = Game.Instance.ActiveUnit.X2;   //set active box coords to active unit coords
            ActiveBoxY = Game.Instance.ActiveUnit.Y2;

            //timer for viewing pieces
            t_VP.Interval = 200; // specify interval time as you want (ms)
            t_VP.Tick += new EventHandler(TimerVP_Tick);
            t_VP.Start();

            //timer for animating units (don't start it until the unit has to move)
            t_Unit.Interval = 40;
            t_Unit.Tick += new EventHandler(TimerUnit_Tick);
        }

        private void MapForm_Load(object sender, EventArgs e) { }

        private void MapForm_Paint(object sender, PaintEventArgs e)
        {
            StringFormat sf = new StringFormat();
            sf.LineAlignment = StringAlignment.Center;
            sf.Alignment = StringAlignment.Center;
            Civilization humanPlayer = Game.Civs.Find(civ => civ.Id == Game.Data.HumanPlayerUsed);
            e.Graphics.DrawString(humanPlayer.Adjective + " Map", new Font("Times New Roman", 19), new SolidBrush(Color.Black), new Point(this.Width / 2 + 1, 20 + 1), sf);
            e.Graphics.DrawString(humanPlayer.Adjective + " Map", new Font("Times New Roman", 19), new SolidBrush(Color.FromArgb(135, 135, 135)), new Point(this.Width / 2, 20), sf);
            sf.Dispose();
            //Draw line borders of panel
            e.Graphics.DrawLine(new Pen(Color.FromArgb(67, 67, 67)), 8, 38, 8 + MapPanel.Width + 3, 38);   //1st layer of border
            e.Graphics.DrawLine(new Pen(Color.FromArgb(67, 67, 67)), 8, 38, 8, 38 + MapPanel.Height + 3);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(223, 223, 223)), 8, 38 + MapPanel.Height + 3, 8 + MapPanel.Width + 3, 38 + MapPanel.Height + 3);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(223, 223, 223)), 8 + MapPanel.Width + 3, 38, 8 + MapPanel.Width + 3, 38 + MapPanel.Height + 3);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(67, 67, 67)), 9, 39, 9 + MapPanel.Width + 1, 39);   //2nd layer of border
            e.Graphics.DrawLine(new Pen(Color.FromArgb(67, 67, 67)), 9, 39, 9, 39 + MapPanel.Height + 1);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(223, 223, 223)), 9, 38 + MapPanel.Height + 2, 9 + MapPanel.Width + 2, 38 + MapPanel.Height + 2);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(223, 223, 223)), 8 + MapPanel.Width + 2, 39, 8 + MapPanel.Width + 2, 39 + MapPanel.Height + 2);
        }

        private void MapPanel_Paint(object sender, PaintEventArgs e)
        {
            //Draw map
            e.Graphics.DrawImage(
                Game.Map,
                0,
                0,
                new Rectangle(OffsetX * 32, OffsetY * 16, (BoxNoX + 1) * 64, (BoxNoY + 1) * 32),
                GraphicsUnit.Pixel);

            //Draw all units
            foreach (IUnit unit in Game.Units)
            {
                if (unit != Game.Instance.ActiveUnit && unit != MovingUnit)
                {
                    //Determine if unit inside city
                    bool unitOnTopOfCity = false;
                    foreach (City city in Game.Cities) if (unit.X == city.X && unit.Y == city.Y) { unitOnTopOfCity = true; break; }

                    if (!unitOnTopOfCity && (unit.X != Game.Instance.ActiveUnit.X || unit.Y != Game.Instance.ActiveUnit.Y))   //Draw only if unit NOT inside city AND if active unit is not on same square
                    {
                        List<IUnit> unitsInXY = ListOfUnitsIn(unit.X, unit.Y);    //make a list of units on this X-Y square
                        //if units are stacked, draw only the last unit in the list. Otherwise draw normally.
                        if (unitsInXY.Count > 1) e.Graphics.DrawImage(Draw.DrawUnit(unitsInXY.Last(), true, 1), 32 * (unit.X2 - OffsetX), 16 * (unit.Y2 - OffsetY) - 16);
                        else e.Graphics.DrawImage(Draw.DrawUnit(unit, false, 1), 32 * (unit.X2 - OffsetX), 16 * (unit.Y2 - OffsetY) - 16);
                    }
                }
            }

            //Draw cities
            foreach (City city in Game.Cities)
            {
                e.Graphics.DrawImage(Draw.DrawCity(city, true), 32 * (city.X2 - OffsetX), 16 * (city.Y2 - OffsetY) - 16);

                StringFormat sf = new StringFormat();
                sf.LineAlignment = StringAlignment.Center;
                sf.Alignment = StringAlignment.Center;
                //Draw city name
                e.Graphics.DrawString(city.Name, new Font("Times New Roman", 15.0f), new SolidBrush(Color.Black), 32 * (city.X2 - OffsetX) + 32 + 2, 16 * (city.Y2 - OffsetY) + 32, sf);    //Draw shadow around font
                e.Graphics.DrawString(city.Name, new Font("Times New Roman", 15.0f), new SolidBrush(Color.Black), 32 * (city.X2 - OffsetX) + 32, 16 * (city.Y2 - OffsetY) + 32 + 2, sf);    //Draw shadow around font
                e.Graphics.DrawString(city.Name, new Font("Times New Roman", 15.0f), new SolidBrush(CivColors.Light[city.Owner]), 32 * (city.X2 - OffsetX) + 32, 16 * (city.Y2 - OffsetY) + 32, sf);
                sf.Dispose();
            }

            //Draw active unit
            if (UnitIsMoving)
            {
                int moveToX = MovingUnit.X2 - AnimationStartX;
                int moveToY = MovingUnit.Y2 - AnimationStartY;
                e.Graphics.DrawImage(Draw.DrawUnit(MovingUnit, false, 1), 32 * (AnimationStartX - OffsetX) + TimerUnitCount * moveToX * 8, 16 * (AnimationStartY - OffsetY) + TimerUnitCount * moveToY * 4 - 16);

                if (TimerUnitCount == 4) { UnitIsMoving = false; t_Unit.Stop(); TimerUnitCount = 0; MovingUnit = null; }   //stop the animation when unit moved
            }
            else if (TimerVPCount % 2 == 1 || ViewingPiecesMode)
            {
                //Determine if active unit is stacked
                bool stacked = false;
                List<IUnit> unitsInXY = ListOfUnitsIn(Game.Instance.ActiveUnit.X, Game.Instance.ActiveUnit.Y);
                if (unitsInXY.Count > 1) stacked = true;

                e.Graphics.DrawImage(Draw.DrawUnit(Game.Instance.ActiveUnit, stacked, 1), 32 * (Game.Instance.ActiveUnit.X2 - OffsetX), 16 * (Game.Instance.ActiveUnit.Y2 - OffsetY) - 16);
            }

            //Draw gridlines
            if (GridIsChecked)
            {
                for (int col = 0; col < Game.Data.MapXdim / 2; col++)
                {
                    for (int row = 0; row < Game.Data.MapYdim; row++)
                    {
                        e.Graphics.DrawImage(Images.GridLines, 64 * col + 32 * (row % 2), 16 * row);
                    }
                }
            }

            //Draw (x,y) locations on grid
            int x, y;
            if (DrawXYnumbers)
            {
                for (int i = 0; i < BoxNoX; i++)
                {
                    for (int j = 0; j < BoxNoY; j++)
                    {
                        x = i * 64 + 12;
                        y = j * 32 + 8;
                        e.Graphics.DrawString(String.Format("({0},{1})", 2 * i + OffsetX, 2 * j + OffsetY), new Font("Arial", 8), new SolidBrush(Color.Yellow), x, y, new StringFormat()); //for first horizontal line
                        e.Graphics.DrawString(String.Format("({0},{1})", 2 * i + 1 + OffsetX, 2 * j + 1 + OffsetY), new Font("Arial", 8), new SolidBrush(Color.Yellow), x + 32, y + 16, new StringFormat()); //for second horizontal line
                    }
                }
            }

            //Draw viewing pieces
            if (ViewingPiecesMode && TimerVPCount % 2 == 1)
            {
                e.Graphics.DrawImage(Images.ViewingPieces, 32 * (ClickedBoxX - OffsetX), 16 * (ClickedBoxY - OffsetY), 64, 32);
            }

        }

        private void MapPanel_MouseClick(object sender, MouseEventArgs e)
        {
            //Calculate (X,Y) coordinates of clicked square
            double nx = e.Location.X - 2 * e.Location.Y;  //crossing at x-axis
            double ny = -(-e.Location.Y - 0.5 * e.Location.X);   //crossing at y-axis
            int nX = Convert.ToInt32(Math.Floor((nx + 32) / 64));   //converting crossing to int
            int nY = Convert.ToInt32(Math.Floor((ny - 16) / 32));   //converting crossing to int
            ClickedBoxX = nX + nY + OffsetX;
            ClickedBoxY = nY - nX + OffsetY;
            OffsetX = ClickedBoxX - 2 * CenterBoxX + 2; //calculate offset of shown map from (0,0)
            OffsetY = ClickedBoxY - 2 * CenterBoxY + 2;
            CheckOffset();

            MapPanel.Refresh();

            if (e.Button == MouseButtons.Right)
            {
                ViewingPiecesMode = true;   //with right-click you activate viewing pieces mode in status form
                Application.OpenForms.OfType<MainCiv2Window>().First().UpdateOrdersMenu();  //update orders menu in main screen
                mainCiv2Window.statusForm.ReceiveMousePositionFromMapForm();  //send mouse click location to status form
            }
            else
            {
                mainCiv2Window.statusForm.ReceiveMousePositionFromMapForm();   //send mouse click location to status form
                if (Game.Cities.Any(city => city.X2 == ClickedBoxX && city.Y2 == ClickedBoxY))    //if city is clicked => open form
                {
                    CityForm cityForm = new CityForm(this, Game.Cities.Find(city => city.X2 == ClickedBoxX && city.Y2 == ClickedBoxY));
                    cityForm.Show();
                }
            }

            //Update active box coordinates. If viewing pieces mode is off, the active unit determines coords.
            if (ViewingPiecesMode)
            {
                ActiveBoxX = ClickedBoxX;
                ActiveBoxY = ClickedBoxY;
            }
            else
            {
                ActiveBoxX = Game.Instance.ActiveUnit.X2;
                ActiveBoxY = Game.Instance.ActiveUnit.Y2;
            }
        }

        private void TimerVP_Tick(object sender, EventArgs e)
        {
            TimerVPCount += 1;
            //MapPanel.Invalidate(new Rectangle(64 * (CenterBoxX - 1), 32 * (CenterBoxY - 1), 64, 32));
            MapPanel.Refresh();
        }

        private void TimerUnit_Tick(object sender, EventArgs e)
        {
            TimerUnitCount += 1;
            MapPanel.Refresh();
        }

        public void RefreshMapForm()
        {
            TimerVPCount = 1; //set this so that when this method is called everything in form is refreshed immediately
            MapPanel.Refresh();
        }

        public void AnimateUnit(IUnit movingUnit, int startX, int startY)
        {
            UnitIsMoving = true;
            MovingUnit = movingUnit;
            AnimationStartX = startX;
            AnimationStartY = startY;
            t_Unit.Start();
            MapPanel.Refresh();
        }

        //Center view on new unit only if it is on the edge of current view or further away
        public void MoveMapViewIfNecessary()
        {
            if (Game.Instance.ActiveUnit.X2 <= OffsetX ||
                Game.Instance.ActiveUnit.Y2 <= OffsetY ||
                Game.Instance.ActiveUnit.X2 >= OffsetX + 2 * BoxNoX ||
                Game.Instance.ActiveUnit.Y2 >= OffsetY + 2 * BoxNoY)
            {
                OffsetX = Game.Instance.ActiveUnit.X2 - 2 * (CenterBoxX - 1);
                OffsetY = Game.Instance.ActiveUnit.Y2 - 2 * (CenterBoxY - 1);
                CheckOffset();
            }
        }

        //Call this to make sure offset is correct after movement with mouse or when new unit is chosen
        public void CheckOffset()
        {
            //Do not allow to move out of map bounds by limiting offset
            if (OffsetX < 0) OffsetX = 0;
            if (OffsetX >= 2 * Game.Data.MapXdim - 2 * BoxNoX) OffsetX = 2 * Game.Data.MapXdim - 2 * BoxNoX;
            if (OffsetY < 0) OffsetY = 0;
            if (OffsetY >= Game.Data.MapYdim - 2 * BoxNoY) OffsetY = Game.Data.MapYdim - 2 * BoxNoY;

            //After limiting offset, do not allow some combinations, e.g. (2,1)
            if (Math.Abs((OffsetX - OffsetY) % 2) == 1)
            {
                if (OffsetX + 1 < Game.Data.MapXdim) OffsetX += 1;
                else if (OffsetY + 1 < Game.Data.MapYdim) OffsetY += 1;
                else if (OffsetX - 1 > 0) OffsetX -= 1;
                else OffsetY -= 1;
            }
        }

        //Return list of units on a X-Y square
        private List<IUnit> ListOfUnitsIn(int x, int y)
        {
            List<IUnit> unitsInXY = new List<IUnit>();
            foreach (IUnit unit in Game.Units) if (unit.X == x && unit.Y == y) unitsInXY.Add(unit);
            return unitsInXY;
        }
    }

}
