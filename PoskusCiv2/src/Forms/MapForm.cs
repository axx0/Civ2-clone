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

        public static int offsetX, offsetY, CenterBoxX, CenterBoxY, ClickedBoxX, ClickedBoxY, BoxNoX, BoxNoY;
        Random randomNo = new Random();

        public bool CreateUnit, GridIsChecked = false, DrawXYnumbers = false;
        public static bool viewingPiecesMode = false;

        //timer
        Timer t = new Timer();
        int stej = 0;   //records no of timer ticks

        Draw Draw = new Draw();

        CreateUnitForm createUnitForm = new CreateUnitForm();

        DoubleBufferedPanel MapPanel;

        Pen pulsatingRectPen = new Pen(Color.White, 1);

        public MapForm(MainCiv2Window _mainCiv2Window)
        {
            InitializeComponent();
            mainCiv2Window = _mainCiv2Window;
            Size = new Size((int)((_mainCiv2Window.ClientSize.Width) * 0.8625 - 4), _mainCiv2Window.ClientSize.Height - 80);    //-4 is experience setting
            Paint += new PaintEventHandler(MapForm_Paint);

            //Panel for map
            MapPanel = new DoubleBufferedPanel
            {
                Location = new Point(8, 34),
                Size = new Size(this.ClientSize.Width - 18, this.ClientSize.Height - 47),
                BackColor = Color.Black
            };
            Controls.Add(MapPanel);
            MapPanel.Paint += new PaintEventHandler(MapPanel_Paint);
            MapPanel.MouseClick += new MouseEventHandler(MapPanel_MouseClick);
        }

        private void MapForm_Load(object sender, EventArgs e)
        {
            //timer for animating units
            t.Interval = 200; // specify interval time as you want (ms)
            t.Tick += new EventHandler(Timer_Tick);
            t.Start();

            CreateUnit = false; //for start

            //for calculation of moving with mouse in MapForm   
            BoxNoX = (int)Math.Floor((double)MapPanel.Width / 64);   //No of squares in X and Y direction
            BoxNoY = (int)Math.Floor((double)MapPanel.Height / 32);
            CenterBoxX = (int)Math.Ceiling((double)BoxNoX / 2); //Determine the square in the center of figure
            CenterBoxY = (int)Math.Ceiling((double)BoxNoY / 2);
            offsetX = 0; //starting offset from (0,0)
            offsetY = 0;
        }

        private void MapForm_KeyPress(object sender, KeyPressEventArgs e)
        {
            //Play movement sound for unit
            //if (new char[] { '1', '2', '3', '4', '6', '7', '8', '9'}.Contains(e.KeyChar)) { player.Play(); };

            //Game.UserInput(e.KeyChar);
            Actions.UnitKeyboardAction(e.KeyChar);
        }

        private void MapForm_Paint(object sender, PaintEventArgs e)
        {
            StringFormat sf = new StringFormat();
            sf.LineAlignment = StringAlignment.Center;
            sf.Alignment = StringAlignment.Center;
            Civilization humanPlayer = Game.Civs.Find(civ => civ.Id == Game.Data.HumanPlayerUsed);

            e.Graphics.DrawString(humanPlayer.Adjective + " Map", new Font("Times New Roman", 19), new SolidBrush(Color.Black), new Point(this.Width / 2 + 1, 20 + 1), sf);
            e.Graphics.DrawString(humanPlayer.Adjective + " Map", new Font("Times New Roman", 19), new SolidBrush(Color.FromArgb(135, 135, 135)), new Point(this.Width / 2, 20), sf);
            sf.Dispose();
        }

        private void MapPanel_Paint(object sender, PaintEventArgs e)
        {
            //Draw line borders
            e.Graphics.DrawLine(new Pen(Color.FromArgb(67, 67, 67)), 0, 0, MapPanel.Width - 2, 0);   //1st layer of border
            e.Graphics.DrawLine(new Pen(Color.FromArgb(67, 67, 67)), 0, 0, 0, MapPanel.Height - 2);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(223, 223, 223)), MapPanel.Width - 1, 0, MapPanel.Width - 1, MapPanel.Height - 1);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(223, 223, 223)), 0, MapPanel.Height - 1, MapPanel.Width - 1, MapPanel.Height - 1);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(67, 67, 67)), 1, 1, MapPanel.Width - 3, 1);   //2nd layer of border
            e.Graphics.DrawLine(new Pen(Color.FromArgb(67, 67, 67)), 1, 1, 1, MapPanel.Height - 3);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(223, 223, 223)), MapPanel.Width - 2, 1, MapPanel.Width - 2, MapPanel.Height - 2);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(223, 223, 223)), 1, MapPanel.Height - 2, MapPanel.Width - 2, MapPanel.Height - 2);

            //Draw map
            e.Graphics.DrawImage(
                Game.Map,
                2,
                2,
                new Rectangle(offsetX * 32, offsetY * 16, (BoxNoX + 1) * 64, (BoxNoY + 1) * 32),
                GraphicsUnit.Pixel);

            //e.Graphics.DrawImage(
            //    Draw.GetMapPart(Game.Map, offsetX * 32, offsetY * 16, MapPanel.Size),
            //    2,
            //    2,
            //    new Rectangle(offsetX * 32, offsetY * 16, (BoxNoX + 1) * 64, (BoxNoY + 1) * 32),
            //    GraphicsUnit.Pixel);

            //Draw all units
            foreach (IUnit unit in Game.Units)
            {
                if (unit != Game.Instance.ActiveUnit)
                {
                    //Determine if unit inside city
                    bool unitOnTopOfCity = false;
                    foreach (City city in Game.Cities) { if (unit.X == city.X && unit.Y == city.Y) { unitOnTopOfCity = true; break; } }

                    if (!unitOnTopOfCity && (unit.X != Game.Instance.ActiveUnit.X || unit.Y != Game.Instance.ActiveUnit.Y))   //Draw only if unit NOT inside city AND if active unit is not on same square
                    {
                        List<IUnit> unitsInXY = ListOfUnitsIn(unit.X, unit.Y);    //make a list of units on this X-Y square
                        if (unitsInXY.Count > 1)    //if units are stacked, draw only the last unit in the list
                        {
                            e.Graphics.DrawImage(Draw.DrawUnit(unitsInXY.Last(), true, 1), 32 * (unit.X2 - offsetX), 16 * (unit.Y2 - offsetY) - 16);
                        }
                        else    //if units aren't stacked, draw normally
                        {
                            e.Graphics.DrawImage(Draw.DrawUnit(unit, false, 1), 32 * (unit.X2 - offsetX), 16 * (unit.Y2 - offsetY) - 16);
                        }
                    }
                }
            }

            //Draw cities
            foreach (City city in Game.Cities)
            {
                e.Graphics.DrawImage(Draw.DrawCity(city, true), 32 * (city.X2 - offsetX), 16 * (city.Y2 - offsetY) - 16);

                StringFormat sf = new StringFormat();
                sf.LineAlignment = StringAlignment.Center;
                sf.Alignment = StringAlignment.Center;
                //Draw city name
                e.Graphics.DrawString(city.Name, new Font("Times New Roman", 15.0f), new SolidBrush(Color.Black), 32 * (city.X2 - offsetX) + 32 + 2, 16 * (city.Y2 - offsetY) + 32, sf);    //Draw shadow around font
                e.Graphics.DrawString(city.Name, new Font("Times New Roman", 15.0f), new SolidBrush(Color.Black), 32 * (city.X2 - offsetX) + 32, 16 * (city.Y2 - offsetY) + 32 + 2, sf);    //Draw shadow around font
                e.Graphics.DrawString(city.Name, new Font("Times New Roman", 15.0f), new SolidBrush(CivColors.Light[city.Owner]), 32 * (city.X2 - offsetX) + 32, 16 * (city.Y2 - offsetY) + 32, sf);

                sf.Dispose();
            }

            //Draw active unit
            if (stej % 2 == 1)
            {
                //Determine if active unit is stacked
                bool stacked = false;
                List<IUnit> unitsInXY = ListOfUnitsIn(Game.Instance.ActiveUnit.X, Game.Instance.ActiveUnit.Y);
                if (unitsInXY.Count > 1) { stacked = true; }

                e.Graphics.DrawImage(Draw.DrawUnit(Game.Instance.ActiveUnit, stacked, 1), 32 * (Game.Instance.ActiveUnit.X2 - offsetX), 16 * (Game.Instance.ActiveUnit.Y2 - offsetY) - 16);
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
                        e.Graphics.DrawString(String.Format("({0},{1})", 2 * i + offsetX, 2 * j + offsetY), new Font("Arial", 8), new SolidBrush(Color.Yellow), x, y, new StringFormat()); //for first horizontal line
                        e.Graphics.DrawString(String.Format("({0},{1})", 2 * i + 1 + offsetX, 2 * j + 1 + offsetY), new Font("Arial", 8), new SolidBrush(Color.Yellow), x + 32, y + 16, new StringFormat()); //for second horizontal line
                    }
                }
            }

            //Draw viewing pieces
            if (viewingPiecesMode && stej % 2 == 1)
            {
                e.Graphics.DrawImage(Images.ViewingPieces, 32 * (ClickedBoxX - offsetX), 16 * (ClickedBoxY - offsetY), 64, 32);
            }

        }

        private void MapPanel_MouseClick(object sender, MouseEventArgs e)
        {
            BoxNoX = (int)Math.Floor((double)MapPanel.Width / 64);//Calculate No of squares in the form in X and Y
            BoxNoY = (int)Math.Floor((double)MapPanel.Height / 32);
            CenterBoxX = (int)Math.Ceiling((double)BoxNoX / 2);//Determine the square in the center of figure
            CenterBoxY = (int)Math.Ceiling((double)BoxNoY / 2);

            //Calculate (X,Y) coordinates of clicked square
            double nx = e.Location.X - 2 * e.Location.Y;  //crossing at x-axis
            double ny = -(-e.Location.Y - 0.5 * e.Location.X);   //crossing at y-axis
            int nX = Convert.ToInt32(Math.Floor((nx + 32) / 64));   //converting crossing to int
            int nY = Convert.ToInt32(Math.Floor((ny - 16) / 32));   //converting crossing to int
            ClickedBoxX = nX + nY + offsetX;
            ClickedBoxY = nY - nX + offsetY;
            offsetX = ClickedBoxX - 2 * CenterBoxX + 2; //calculate offset of shown map from (0,0)
            offsetY = ClickedBoxY - 2 * CenterBoxY + 2;

            //Do not allow to move out of map bounds by limiting offset
            if (offsetX < 0) { offsetX = 0; }
            if (offsetX >= 2 * Game.Data.MapXdim - 2 * BoxNoX) { offsetX = 2 * Game.Data.MapXdim - 2 * BoxNoX; }
            if (offsetY < 0) { offsetY = 0; }
            if (offsetY >= Game.Data.MapYdim - 2 * BoxNoY) { offsetY = Game.Data.MapYdim - 2 * BoxNoY; }

            //After limiting offset, do not allow some combinations, e.g. (2,1)
            if (Math.Abs((offsetX - offsetY) % 2) == 1)
            {
                if (offsetX + 1 < Game.Data.MapXdim) { offsetX += 1; }
                else if (offsetY + 1 < Game.Data.MapYdim) { offsetY += 1; }
                else if (offsetX - 1 > 0) { offsetX -= 1; }
                else { offsetY -= 1; }
            }
            MapPanel.Invalidate();

            if (e.Button == MouseButtons.Right)
            {
                viewingPiecesMode = true;   //with right-click you activate viewing pieces mode in status form                             
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
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            stej += 1;
            //update viewing pieces
            //MapPanel.Invalidate(new Rectangle(64 * (CenterBoxX - 1), 32 * (CenterBoxY - 1), 64, 32));
            InvalidatePanel();
        }

        public void InvalidatePanel()
        {
            MapPanel.Invalidate();
        }

        //Return list of units on a X-Y square
        private List<IUnit> ListOfUnitsIn(int x, int y)
        {
            List<IUnit> unitsInXY = new List<IUnit>();
            foreach (IUnit unit in Game.Units)
            {
                if (unit.X == x && unit.Y == y) { unitsInXY.Add(unit); }
            }
            return unitsInXY;
        }

    }

}
