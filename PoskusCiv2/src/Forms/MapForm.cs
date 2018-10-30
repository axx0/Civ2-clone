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
        System.Windows.Forms.Timer t = new System.Windows.Forms.Timer();
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
            
            //TESTING...
            foreach (Civilization civ in Game.Civs)
            {
                Console.WriteLine("Civ " + civ.TribeName + ", " + civ.LeaderName + ", " + civ.Adjective);
            }
            //TESTING...
            foreach (City city in Game.Cities)
            {
                Console.WriteLine("City " + city.Name + " X=" + city.X.ToString() + " Y=" + city.Y.ToString());
            }
            Console.WriteLine("The following are units:");
            foreach (IUnit unit in Game.Units)
            {
                Console.WriteLine(unit.Type + " X=" + unit.X.ToString() + " Y=" + unit.Y.ToString());
            }
            ////TESTING...Importing savegame....
            Console.WriteLine("Bloodlust= " + importMap.Bloodlust);
            Console.WriteLine("Simplified combat= " + importMap.SimplifiedCombat);
            Console.WriteLine("Flat earth= " + importMap.FlatEarth);
            Console.WriteLine("Turn number= {0}", importMap.TurnNumber);
            Console.WriteLine("No of units= {0}", importMap.NumberOfUnits);
            Console.WriteLine("No of cities= {0}", importMap.NumberOfCities);

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
            foreach (IUnit unit in Game.Units)
            {
                if (unit == Game.Instance.ActiveUnit)
                {
                    if (stej % 2 == 1)
                    {
                        e.Graphics.DrawImage(Images.UnitShield, 32 * (unit.X - offsetX) + Images.unitShieldLocation[(int)unit.Type, 0], 16 * (unit.Y - offsetY) - 16 + Images.unitShieldLocation[(int)unit.Type, 1]); //draw shield
                        e.Graphics.DrawImage(Images.Units[(int)unit.Type], 32 * (unit.X - offsetX), 16 * (unit.Y - offsetY) - 16);    //draw unit pulsating
                    }
                }
                else
                {
                    e.Graphics.DrawImage(Images.UnitShield, 32 * (unit.X - offsetX) + Images.unitShieldLocation[(int)unit.Type, 0], 16 * (unit.Y - offsetY) - 16 + Images.unitShieldLocation[(int)unit.Type, 1]); //draw shield
                    e.Graphics.DrawImage(Images.Units[(int)unit.Type], 32 * (unit.X - offsetX), 16 * (unit.Y - offsetY) - 16);    //draw other units not pulsating
                }
            }

            //Draw grid lines
            Pen blackPen = new Pen(Color.White, 1);
            if (GridIsChecked) { blackPen.Color = Color.Black; }
            else { blackPen.Color = Color.Transparent; }
            for (int i = 0; i < BoxNoX + BoxNoY; i++)
            {
                //lines in one direction:
                Point point1 = new Point(Math.Min(64 * i + 32 - 1, 64 * BoxNoX + 32 - 1), Math.Max(0, 32 * (i - BoxNoX) - 1));
                Point point2 = new Point(Math.Max(0, 64 * (i - BoxNoY) - 1), Math.Min(32 * i + 16 - 1, 32 * BoxNoY + 16 - 1));
                e.Graphics.DrawLine(blackPen, point1, point2);
                //lines in other direction:
                Point point3 = new Point(-Math.Min(0, 64 * (BoxNoY - i) + 32 - 1), Math.Max(0, 32 * (BoxNoY - i) + 16 - 1));
                Point point4 = new Point(Math.Min(64 * i + 32 - 1, 64 * BoxNoX + 32 - 1), Math.Min(32 * (BoxNoY + 1) - 1, 32 * (BoxNoX + BoxNoY + 1 - i) - 1));
                e.Graphics.DrawLine(blackPen, point3, point4);
            }
            blackPen.Dispose();

            //Draw (x,y) locations on grid
            if (DrawXYnumbers)
            {
                System.Drawing.Graphics formGraphics = this.CreateGraphics();
                System.Drawing.Font drawFont = new System.Drawing.Font("Arial", 8);
                System.Drawing.SolidBrush drawBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Yellow);
                System.Drawing.StringFormat drawFormat = new System.Drawing.StringFormat();
                for (int i = 0; i < BoxNoX; i++)
                {
                    for (int j = 0; j < BoxNoY; j++)
                    {
                        int x = i * 64 + 12;
                        int y = j * 32 + 8;
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
            //TO-DO:
            // 1) map should draw only to borders

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
