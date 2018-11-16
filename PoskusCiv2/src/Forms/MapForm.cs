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
    public partial class MapForm : Form
    {
        public MainCiv2Window mainCiv2Window;

        public static int offsetX, offsetY, CenterBoxX, CenterBoxY;
        public int BoxNoX, BoxNoY;
        public static int ClickedBoxX, ClickedBoxY;
        Random randomNo = new Random();

        public bool GridIsChecked = false;
        public bool DrawXYnumbers = false;
        public static bool viewingPiecesMode = false;
        public bool CreateUnit;
        
        //timer
        Timer t = new Timer();
        int stej = 0;   //records no of timer ticks

        ImportSavegame importMap = new ImportSavegame();
        Draw Draw = new Draw();
        Bitmap Map;
        
        CreateUnitForm createUnitForm = new CreateUnitForm();

        DoubleBufferedPanel MapPanel;

        Pen pulsatingRectPen = new Pen(Color.White, 1);

        public MapForm(MainCiv2Window _mainCiv2Window)
        {
            InitializeComponent();
            Size = new Size(1280, 810);
            BackColor = Color.Black;
            FormBorderStyle = FormBorderStyle.None;
            BackgroundImage = Images.WallpaperMapForm;
            DoubleBuffered = true;
            mainCiv2Window = _mainCiv2Window;

            //Panel for map
            MapPanel = new DoubleBufferedPanel
            {
                Location = new Point(5, 35),
                Size = new Size(1255, 730),          
                BorderStyle = BorderStyle.Fixed3D
            };
            Controls.Add(MapPanel);
            MapPanel.Paint += MapPanel_Paint;
            MapPanel.MouseClick += MapPanel_MouseClick;

            Map = Draw.DrawMap(); //prepare whole game map
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
            Civilization myCiv = Game.Civs.Find(civ => civ.Id == Game.Data.PlayersCivilizationNumberUsed);
            e.Graphics.DrawString(myCiv.Adjective + " Map", new Font("Times New Roman", 19), new SolidBrush(Color.Black), new Point(this.Width / 2 + 1, 20 + 1), sf);
            e.Graphics.DrawString(myCiv.Adjective + " Map", new Font("Times New Roman", 19), new SolidBrush(Color.FromArgb(135, 135, 135)), new Point(this.Width / 2, 20), sf);
            sf.Dispose();
        }

        private void MapPanel_Paint(object sender, PaintEventArgs e)
        {
            Console.WriteLine("OFFSETXY={0},{1}", offsetX, offsetY);
            Console.WriteLine("CenterboxXY={0},{1}", CenterBoxX, CenterBoxY);
            e.Graphics.DrawImage(
                Map,
                0,
                0,
                new Rectangle(offsetX * 32, offsetY * 16, (BoxNoX + 1) * 64, (BoxNoY + 1) * 32),
                GraphicsUnit.Pixel);

            //Draw all units
            int x, y;
            foreach (IUnit unit in Game.Units)
            {
                if (unit != Game.Instance.ActiveUnit)
                {
                    x = 2 * unit.X + unit.Y % 2;    //convert XY to civ2-style
                    y = unit.Y;

                    //Determine if unit inside city
                    bool unitOnTopOfCity = false;
                    foreach (City city in Game.Cities) { if (unit.X == city.X && unit.Y == city.Y) { unitOnTopOfCity = true; break; } }

                    if (!unitOnTopOfCity)   //Draw only if unit NOT inside city
                    {
                        e.Graphics.DrawImage(Draw.DrawUnit(unit), 32 * (x - offsetX), 16 * (y - offsetY) - 16);
                    }
                }
            }

            //Draw cities
            foreach (City city in Game.Cities)
            {
                x = 2 * city.X + city.Y % 2;    //convert XY to civ2-style
                y = city.Y;

                e.Graphics.DrawImage(Draw.DrawCity(city, true), 32 * (x - offsetX), 16 * (y - offsetY) - 16);

                StringFormat sf = new StringFormat();
                sf.LineAlignment = StringAlignment.Center;
                sf.Alignment = StringAlignment.Center;
                //Draw city name
                e.Graphics.DrawString(city.Name, new Font("Times New Roman", 15.0f), new SolidBrush(Color.Black), 32 * (x - offsetX) + 32 + 2, 16 * (y - offsetY) + 32, sf);    //Draw shadow around font
                e.Graphics.DrawString(city.Name, new Font("Times New Roman", 15.0f), new SolidBrush(Color.Black), 32 * (x - offsetX) + 32, 16 * (y - offsetY) + 32 + 2, sf);    //Draw shadow around font
                e.Graphics.DrawString(city.Name, new Font("Times New Roman", 15.0f), new SolidBrush(Images.CivColors[city.Owner]), 32 * (x - offsetX) + 32, 16 * (y - offsetY) + 32, sf);

                sf.Dispose();
            }

            //Draw active unit
            x = 2 * Game.Instance.ActiveUnit.X + Game.Instance.ActiveUnit.Y % 2;    //convert XY to civ2-style
            y = Game.Instance.ActiveUnit.Y;
            if (stej % 2 == 1)
            {
                e.Graphics.DrawImage(Draw.DrawUnit(Game.Instance.ActiveUnit), 32 * (x - offsetX), 16 * (y - offsetY) - 16);
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
            if (viewingPiecesMode & stej % 2 == 1)
            {
                e.Graphics.DrawImage(Images.ViewingPieces, 64 * (CenterBoxX - 1), 32 * (CenterBoxY - 1), 64, 32);
            }

        }

        //click with a mouse --> center MapForm on the square
        private void MapForm_MouseClick(object sender, MouseEventArgs e)
        { }

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
            MapPanel.Invalidate();

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


            //Convert coordinates from Civ-2 style to real coordinates (only x, y is OK)
            ClickedBoxX = (ClickedBoxX - (ClickedBoxY % 2)) / 2;

            if (e.Button == MouseButtons.Right)
            {
                viewingPiecesMode = true;   //with right-click you activate viewing pieces mode in status form
                //mainCiv2Window.statusForm.UpdateUnitLabels(Game.unitInLine);    //update status form

               //send mouse click location to status form
                mainCiv2Window.statusForm.ReceiveMousePositionFromMapForm(ClickedBoxX, ClickedBoxY);
            }
            else
            {
                //send mouse click location to status form
                mainCiv2Window.statusForm.ReceiveMousePositionFromMapForm(ClickedBoxX, ClickedBoxY);

                if (Game.Cities.Any(city => city.X == ClickedBoxX && city.Y == ClickedBoxY))    //if city is clicked => open form
                {
                    CityForm cityForm = new CityForm(this, Game.Cities.Find(city => city.X == ClickedBoxX && city.Y == ClickedBoxY));
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

    }

}
