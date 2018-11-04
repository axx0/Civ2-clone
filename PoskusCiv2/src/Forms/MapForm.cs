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

        public int offsetX, offsetY, BoxNoX, BoxNoY;
        public static int CenterBoxX, CenterBoxY, ClickedBoxX, ClickedBoxY;
        Random randomNo = new Random();

        public bool GridIsChecked = false;
        public bool DrawXYnumbers = false;
        public static bool viewingPiecesMode = false;
        public bool CreateUnit;
        
        //timer
        Timer t = new Timer();
        int stej = 0;   //records no of timer ticks

        ImportSavegame importMap = new ImportSavegame();
        DrawMap MapBitmap = new DrawMap();
        
        CreateUnitForm createUnitForm = new CreateUnitForm();

        //a helpful label
        Label label1 = new Label();

        Pen pulsatingRectPen = new Pen(Color.White, 1);

        public MapForm(MainCiv2Window _mainCiv2Window)
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            mainCiv2Window = _mainCiv2Window;
        }

        private void MapForm_Load(object sender, EventArgs e)
        {
            
            ////TESTING...
            //foreach (Civilization civ in Game.Civs)
            //{
            //    Console.WriteLine("Civ " + civ.TribeName + ", " + civ.LeaderName + ", " + civ.Adjective);
            //}
            ////TESTING...
            //foreach (City city in Game.Cities)
            //{
            //    Console.WriteLine("City " + city.Name + " X=" + city.X.ToString() + " Y=" + city.Y.ToString());
            //}
            //Console.WriteLine("The following are units:");
            //foreach (IUnit unit in Game.Units)
            //{
            //    Console.WriteLine(unit.Type + " X=" + unit.X.ToString() + " Y=" + unit.Y.ToString());
            //}
            //////TESTING...Importing savegame....
            //Console.WriteLine("Bloodlust= " + importMap.Bloodlust);
            //Console.WriteLine("Simplified combat= " + importMap.SimplifiedCombat);
            //Console.WriteLine("Flat earth= " + importMap.FlatEarth);
            //Console.WriteLine("Turn number= {0}", importMap.TurnNumber);
            //Console.WriteLine("No of units= {0}", importMap.NumberOfUnits);
            //Console.WriteLine("No of cities= {0}", importMap.NumberOfCities);

            //timer for animating units
            t.Interval = 200; // specify interval time as you want (ms)
            t.Tick += new EventHandler(timer_Tick);
            t.Start();

            //a helpful label
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(1100, 700);
            label1.ForeColor = Color.White;
            label1.Text = "WAITING...";
            this.Controls.Add(label1);

            CreateUnit = false; //for start
            
            //for calculation of moving with mouse in MapForm
            BoxNoX = (int)Math.Floor((double)this.ClientSize.Width / 64);   //No of squares in X and Y direction
            BoxNoY = (int)Math.Floor((double)this.ClientSize.Height / 32);            
            CenterBoxX = (int)Math.Ceiling((double)BoxNoX / 2); //Determine the square in the center of figure
            CenterBoxY = (int)Math.Ceiling((double)BoxNoY / 2);
            offsetX = 0; //starting offset from (0,0)
            offsetY = 0;
        }

        //At press enter update game turn+year in status form
        private void MapForm_KeyPress(object sender, KeyPressEventArgs e)
        {
            //Play movement sound for unit
            //if (new char[] { '1', '2', '3', '4', '6', '7', '8', '9'}.Contains(e.KeyChar)) { player.Play(); };

            Game.UserInput(e.KeyChar);
        }

        private void MapForm_Paint(object sender, PaintEventArgs e)
        {
            //e.Graphics.DrawImage(Images.Ocean[3], 0, 0);
            e.Graphics.DrawImage(
                MapBitmap.Map,
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
                        e.Graphics.DrawImage(Images.BlackUnitShield, 32 * (x - offsetX) + Images.unitShieldLocation[(int)unit.Type, 0] - 1, 16 * (y - offsetY) - 16 + Images.unitShieldLocation[(int)unit.Type, 1]); //draw border shield (offset for 1 pixel to left) 
                        e.Graphics.DrawImage(Images.UnitShield[(int)unit.Civ], 32 * (x - offsetX) + Images.unitShieldLocation[(int)unit.Type, 0], 16 * (y - offsetY) - 16 + Images.unitShieldLocation[(int)unit.Type, 1]); //draw shield
                        e.Graphics.DrawImage(Images.Units[(int)unit.Type], 32 * (x - offsetX), 16 * (y - offsetY) - 16);    //draw other units not pulsating
                    }
                }
            }

            //Draw cities
            foreach (City city in Game.Cities)
            {
                x = 2 * city.X + city.Y % 2;    //convert XY to civ2-style
                y = city.Y;

                int cityStyle = Game.Civs[city.Owner].CityStyle;

                int sizeStyle = 0;
                //Determine city bitmap
                //For everything not modern or industrial => 4 city size styles (0=sizes 1...3, 1=sizes 4...5, 2=sizes 6...7, 3=sizes >= 8)
                //If city is capital => 3 size styles (1=sizes 1...3, 2=sizes 4...5, 3=sizes >= 6)
                if (cityStyle < 4)
                {
                    if (Array.Exists(city.Improvements, element => element.Type == ImprovementType.Palace)) //palace exists
                    {
                        if (city.Size <= 3) { sizeStyle = 1; }
                        else if (city.Size > 3 && city.Size <= 5) { sizeStyle = 2; }
                        else { sizeStyle = 3; }

                    }
                    else
                    {
                        if (city.Size <= 3) { sizeStyle = 0; }
                        else if (city.Size > 3 && city.Size <= 5) { sizeStyle = 1; }
                        else if (city.Size > 5 && city.Size <= 7) { sizeStyle = 2; }
                        else { sizeStyle = 3; }
                    }
                }
                //If city is industrial => 4 city size styles (0=sizes 1...4, 1=sizes 5...7, 2=sizes 8...10, 3=sizes >= 11)
                //If city is capital => 3 size styles (1=sizes 1...4, 2=sizes 5...7, 3=sizes >= 8)
                else if (cityStyle == 4)
                {
                    if (Array.Exists(city.Improvements, element => element.Type == ImprovementType.Palace)) //palace exists
                    {
                        if (city.Size <= 4) { sizeStyle = 1; }
                        else if (city.Size > 4 && city.Size <= 7) { sizeStyle = 2; }
                        else { sizeStyle = 3; }

                    }
                    else
                    {
                        if (city.Size <= 4) { sizeStyle = 0; }
                        else if (city.Size > 4 && city.Size <= 7) { sizeStyle = 1; }
                        else if (city.Size > 7 && city.Size <= 10) { sizeStyle = 2; }
                        else { sizeStyle = 3; }
                    }
                }
                //If city is modern => 4 city size styles (0=sizes 1...4, 1=sizes 5...10, 2=sizes 11...18, 3=sizes >= 19)
                //If city is capital => 3 size styles (1=sizes 1...4, 2=sizes 5...10, 3=sizes >= 11)
                else
                {
                    if (Array.Exists(city.Improvements, element => element.Type == ImprovementType.Palace)) //palace exists
                    {
                        if (city.Size <= 4) { sizeStyle = 1; }
                        else if (city.Size > 4 && city.Size <= 10) { sizeStyle = 2; }
                        else { sizeStyle = 3; }

                    }
                    else
                    {
                        if (city.Size <= 4) { sizeStyle = 0; }
                        else if (city.Size > 4 && city.Size <= 10) { sizeStyle = 1; }
                        else if (city.Size > 10 && city.Size <= 18) { sizeStyle = 2; }
                        else { sizeStyle = 3; }
                    }
                }

                StringFormat sf = new StringFormat();
                sf.LineAlignment = StringAlignment.Center;
                sf.Alignment = StringAlignment.Center;

                //Draw city
                if (!Array.Exists(city.Improvements, element => element.Type == ImprovementType.CityWalls))  //no city walls
                {
                    e.Graphics.DrawImage(Images.City[cityStyle, sizeStyle], 32 * (x - offsetX), 16 * (y - offsetY) - 16);
                    //Draw city size window
                    e.Graphics.DrawRectangle(new Pen(Color.Black), 32 * (x - offsetX) - 1 + Images.citySizeWindowLoc[cityStyle, sizeStyle, 0], 16 * (y - offsetY) - 16 + Images.citySizeWindowLoc[cityStyle, sizeStyle, 1] - 1, 9, 13);
                    e.Graphics.FillRectangle(new SolidBrush(Images.CivColors[city.Owner]), 32 * (x - offsetX) + Images.citySizeWindowLoc[cityStyle, sizeStyle, 0], 16 * (y - offsetY) - 16 + Images.citySizeWindowLoc[cityStyle, sizeStyle, 1], 8, 12); //filling of rectangle
                    e.Graphics.DrawString(city.Size.ToString(), new Font("Times New Roman", 10.0f, FontStyle.Bold), new SolidBrush(Color.Black), 32 * (x - offsetX) + Images.citySizeWindowLoc[cityStyle, sizeStyle, 0] + 4, 16 * (y - offsetY) - 16 + Images.citySizeWindowLoc[cityStyle, sizeStyle, 1] + 6, sf);    //Size text
                    //Draw city flag
                    e.Graphics.DrawImage(Images.CityFlag[city.Owner], 32 * (x - offsetX) + Images.cityFlagLoc[cityStyle, sizeStyle, 0] - 3, 16 * (y - offsetY) - 16 + Images.cityFlagLoc[cityStyle, sizeStyle, 1] - 17);
                }
                else
                {
                    e.Graphics.DrawImage(Images.CityWall[cityStyle, sizeStyle], 32 * (x - offsetX), 16 * (y - offsetY) - 16);
                    //Draw city (+Wall) size window
                    e.Graphics.DrawRectangle(new Pen(Color.Black), 32 * (x - offsetX) - 1 + Images.cityWallSizeWindowLoc[cityStyle, sizeStyle, 0], 16 * (y - offsetY) - 16 + Images.cityWallSizeWindowLoc[cityStyle, sizeStyle, 1] - 1, 9, 13);
                    e.Graphics.FillRectangle(new SolidBrush(Images.CivColors[city.Owner]), 32 * (x - offsetX) + Images.cityWallSizeWindowLoc[cityStyle, sizeStyle, 0], 16 * (y - offsetY) - 16 + Images.cityWallSizeWindowLoc[cityStyle, sizeStyle, 1], 8, 12); //filling of rectangle
                    e.Graphics.DrawString(city.Size.ToString(), new Font("Times New Roman", 10.0f, FontStyle.Bold), new SolidBrush(Color.Black), 32 * (x - offsetX) + Images.cityWallSizeWindowLoc[cityStyle, sizeStyle, 0] + 4, 16 * (y - offsetY) - 16 + Images.cityWallSizeWindowLoc[cityStyle, sizeStyle, 1] + 6, sf);    //Size text
                    //Draw city flag
                    e.Graphics.DrawImage(Images.CityFlag[city.Owner], 32 * (x - offsetX) + Images.cityWallFlagLoc[cityStyle, sizeStyle, 0] - 3, 16 * (y - offsetY) - 16 + Images.cityWallFlagLoc[cityStyle, sizeStyle, 1] - 17);
                }

                //Draw city name
                e.Graphics.DrawString(city.Name, new Font("Times New Roman", 15.0f), new SolidBrush(Color.Black), 32 * (x - offsetX) + 32 + 1, 16 * (y - offsetY) + 32, sf);    //Draw shadow around font
                e.Graphics.DrawString(city.Name, new Font("Times New Roman", 15.0f), new SolidBrush(Color.Black), 32 * (x - offsetX) + 32, 16 * (y - offsetY) + 32 + 1, sf);    //Draw shadow around font
                e.Graphics.DrawString(city.Name, new Font("Times New Roman", 15.0f), new SolidBrush(Images.CivColors[city.Owner]), 32 * (x - offsetX) + 32, 16 * (y - offsetY) + 32, sf);

                sf.Dispose();
            }

            //Draw active unit
            x = 2 * Game.Instance.ActiveUnit.X + Game.Instance.ActiveUnit.Y % 2;    //convert XY to civ2-style
            y = Game.Instance.ActiveUnit.Y;
            if (stej % 2 == 1)
            {
                e.Graphics.DrawImage(Images.BlackUnitShield, 32 * (x - offsetX) + Images.unitShieldLocation[(int)Game.Instance.ActiveUnit.Type, 0] - 1, 16 * (y - offsetY) - 16 + Images.unitShieldLocation[(int)Game.Instance.ActiveUnit.Type, 1]); //draw black shield border
                e.Graphics.DrawImage(Images.UnitShield[(int)Game.Instance.ActiveUnit.Civ], 32 * (x - offsetX) + Images.unitShieldLocation[(int)Game.Instance.ActiveUnit.Type, 0], 16 * (y - offsetY) - 16 + Images.unitShieldLocation[(int)Game.Instance.ActiveUnit.Type, 1]); //draw shield
                e.Graphics.DrawImage(Images.Units[(int)Game.Instance.ActiveUnit.Type], 32 * (x - offsetX), 16 * (y - offsetY) - 16);    //draw unit pulsating
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
                Graphics formGraphics = this.CreateGraphics();
                Font drawFont = new Font("Arial", 8);
                SolidBrush drawBrush = new SolidBrush(Color.Yellow);
                StringFormat drawFormat = new StringFormat();
                for (int i = 0; i < BoxNoX; i++)
                {
                    for (int j = 0; j < BoxNoY; j++)
                    {
                        x = i * 64 + 12;
                        y = j * 32 + 8;
                        e.Graphics.DrawString(String.Format("({0},{1})", 2 * i + offsetX, 2 * j + offsetY), drawFont, drawBrush, x, y, drawFormat); //for first horizontal line
                        e.Graphics.DrawString(String.Format("({0},{1})", 2 * i + 1 + offsetX, 2 * j + 1 + offsetY), drawFont, drawBrush, x + 32, y + 16, drawFormat); //for second horizontal line
                    }
                }
                drawFont.Dispose();
                drawBrush.Dispose();
                formGraphics.Dispose();
            }
      
            //Draw viewing pieces
            if (viewingPiecesMode & stej % 2 == 1)
            {
                e.Graphics.DrawImage(Images.ViewingPieces, 64 * (CenterBoxX - 1), 32 * (CenterBoxY - 1), 64, 32);
            }

        }

        //click with a mouse --> center MapForm on the square
        private void MapForm_MouseClick(object sender, MouseEventArgs e)
        {
            BoxNoX = (int)Math.Floor((double)this.ClientSize.Width / 64);//Calculate No of squares in the form in X and Y
            BoxNoY = (int)Math.Floor((double)this.ClientSize.Height / 32);            
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
            Invalidate();

            if (e.Button == MouseButtons.Right)
            {
                viewingPiecesMode = true;   //with right-click you activate viewing pieces mode in status form
                mainCiv2Window.statusForm.UpdateUnitLabels(Game.unitInLine);    //update status form

               //send mouse click location to status form
                mainCiv2Window.statusForm.ReceiveMousePositionFromMapForm(ClickedBoxX, ClickedBoxY);
            }
            else
            {
                //send mouse click location to status form
                mainCiv2Window.statusForm.ReceiveMousePositionFromMapForm(ClickedBoxX, ClickedBoxY);

                if (Game.Cities.Any(city => city.X == ClickedBoxX && city.Y == ClickedBoxY))
                {
                    CityForm cityForm = new CityForm();
                    cityForm.Show();
                }
            }
        }

        void timer_Tick(object sender, EventArgs e)
        {
            //helpful label
            stej += 1;
            this.label1.Text = Convert.ToString(stej/5) + " sec";
            label1.Refresh();

            //update viewing pieces
            //this.Invalidate(new Rectangle(64 * (CenterBoxX - 1), 32 * (CenterBoxY - 1), 64, 32));
            this.Invalidate();
        }

    }

}
