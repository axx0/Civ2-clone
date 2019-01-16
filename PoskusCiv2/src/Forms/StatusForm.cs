using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ExtensionMethods;
using PoskusCiv2.Imagery;
using PoskusCiv2.Enums;
using PoskusCiv2.Units;

namespace PoskusCiv2.Forms
{
    public partial class StatusForm : Civ2form
    {
        public MainCiv2Window mainCiv2Window;

        DoubleBufferedPanel UnitPanel, StatsPanel;
        
         Draw Draw = new Draw();

        //timer
        Timer timer = new Timer();
        int TimerCounter { get; set; }   //records no of timer ticks
        bool EndOfTurnMessage { get; set; }   //records no of timer ticks

        public StatusForm(MainCiv2Window _mainCiv2Window)
        {
            InitializeComponent();
            mainCiv2Window = _mainCiv2Window;

            Size = new Size((int)(_mainCiv2Window.ClientSize.Width * 0.1375), (int)((_mainCiv2Window.ClientSize.Height - 30) * 0.85));
            Paint += new PaintEventHandler(StatusForm_Paint);

            //Stats panel
            StatsPanel = new DoubleBufferedPanel
            {
                Location = new Point(9, 38),
                Size = new Size(this.ClientSize.Width - 19, 64),
                BackgroundImage = Images.WallpaperStatusForm
            };
            Controls.Add(StatsPanel);
            StatsPanel.Paint += StatsPanel_Paint;

            //Unit panel
            UnitPanel = new DoubleBufferedPanel
            {
                Location = new Point(9, 106),
                Size = new Size(this.ClientSize.Width - 19, this.ClientSize.Height - 114),
                BackgroundImage = Images.WallpaperStatusForm
            };
            Controls.Add(UnitPanel);
            UnitPanel.Paint += UnitPanel_Paint;

            //Timer properties
            timer.Interval = 500;   //500 ms
            timer.Tick += new EventHandler(TimerTick);

            EndOfTurnMessage = false;
        }

        private void StatusForm_Load(object sender, EventArgs e) { }

        private void StatusForm_Paint(object sender, PaintEventArgs e)
        {
            StringFormat sf = new StringFormat();
            sf.LineAlignment = StringAlignment.Center;
            sf.Alignment = StringAlignment.Center;
            e.Graphics.DrawString("Status", new Font("Times New Roman", 19), new SolidBrush(Color.Black), new Point(this.Width / 2 + 1, 20 + 1), sf);
            e.Graphics.DrawString("Status", new Font("Times New Roman", 19), new SolidBrush(Color.FromArgb(135, 135, 135)), new Point(this.Width / 2, 20), sf);
            sf.Dispose();
        }

        private void StatsPanel_Paint(object sender, PaintEventArgs e)
        {
            string bcad;
            if (Game.Data.GameYear < 0) { bcad = "B.C."; }
            else { bcad = "A.D."; }
            e.Graphics.DrawString(Game.Civs[1].Population + " People", new Font("Times New Roman", 12), new SolidBrush(Color.FromArgb(191, 191, 191)), new Point(5 + 1, 2 + 1));
            e.Graphics.DrawString(Game.Civs[1].Population + " People", new Font("Times New Roman", 12), new SolidBrush(Color.FromArgb(30, 30, 30)), new Point(5, 2));
            e.Graphics.DrawString(Math.Abs(Game.Data.GameYear).ToString() + " " + bcad + "(Turn " + Game.Data.TurnNumber + ")", new Font("Times New Roman", 12), new SolidBrush(Color.FromArgb(191, 191, 191)), new Point(5 + 1, 20 + 1));
            e.Graphics.DrawString(Math.Abs(Game.Data.GameYear).ToString() + " " + bcad + "(Turn " + Game.Data.TurnNumber + ")", new Font("Times New Roman", 12), new SolidBrush(Color.FromArgb(30, 30, 30)), new Point(5, 20));
            e.Graphics.DrawString(Game.Civs[Game.Data.HumanPlayerUsed].Money.ToString() + " Gold 5.0.5", new Font("Times New Roman", 12), new SolidBrush(Color.FromArgb(191, 191, 191)), new Point(5 + 1, 38 + 1));
            e.Graphics.DrawString(Game.Civs[Game.Data.HumanPlayerUsed].Money.ToString() + " Gold 5.0.5", new Font("Times New Roman", 12), new SolidBrush(Color.FromArgb(30, 30, 30)), new Point(5, 38));
            //Draw line borders
            e.Graphics.DrawLine(new Pen(Color.FromArgb(67, 67, 67)), 0, 0, StatsPanel.Width - 2, 0);   //1st layer of border
            e.Graphics.DrawLine(new Pen(Color.FromArgb(67, 67, 67)), 0, 0, 0, StatsPanel.Height - 2);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(223, 223, 223)), StatsPanel.Width - 1, 0, StatsPanel.Width - 1, StatsPanel.Height - 1);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(223, 223, 223)), 0, StatsPanel.Height - 1, StatsPanel.Width - 1, StatsPanel.Height - 1);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(67, 67, 67)), 1, 1, StatsPanel.Width - 3, 1);   //2nd layer of border
            e.Graphics.DrawLine(new Pen(Color.FromArgb(67, 67, 67)), 1, 1, 1, StatsPanel.Height - 3);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(223, 223, 223)), StatsPanel.Width - 2, 1, StatsPanel.Width - 2, StatsPanel.Height - 2);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(223, 223, 223)), 1, StatsPanel.Height - 2, StatsPanel.Width - 2, StatsPanel.Height - 2);
        }

        private void UnitPanel_Paint(object sender, PaintEventArgs e)
        {
            //Draw line borders
            e.Graphics.DrawLine(new Pen(Color.FromArgb(67, 67, 67)), 0, 0, UnitPanel.Width - 2, 0);   //1st layer of border
            e.Graphics.DrawLine(new Pen(Color.FromArgb(67, 67, 67)), 0, 0, 0, UnitPanel.Height - 2);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(223, 223, 223)), UnitPanel.Width - 1, 0, UnitPanel.Width - 1, UnitPanel.Height - 1);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(223, 223, 223)), 0, UnitPanel.Height - 1, UnitPanel.Width - 1, UnitPanel.Height - 1);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(67, 67, 67)), 1, 1, UnitPanel.Width - 3, 1);   //2nd layer of border
            e.Graphics.DrawLine(new Pen(Color.FromArgb(67, 67, 67)), 1, 1, 1, UnitPanel.Height - 3);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(223, 223, 223)), UnitPanel.Width - 2, 1, UnitPanel.Width - 2, UnitPanel.Height - 2);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(223, 223, 223)), 1, UnitPanel.Height - 2, UnitPanel.Width - 2, UnitPanel.Height - 2);

            //End turn message
            if (EndOfTurnMessage)
            {
                Color brushColor;
                if (TimerCounter % 2 == 1) brushColor = Color.FromArgb(135, 135, 135);
                else brushColor = Color.White;
                e.Graphics.DrawString("End of Turn\n(Press ENTER)", new Font("Times New Roman", 12), new SolidBrush(Color.Black), new Point(10 + 1, UnitPanel.Height - 50));
                e.Graphics.DrawString("End of Turn\n(Press ENTER)", new Font("Times New Roman", 12), new SolidBrush(brushColor), new Point(10, UnitPanel.Height - 50));
            }

            StringFormat sf = new StringFormat();
            sf.LineAlignment = StringAlignment.Center;
            sf.Alignment = StringAlignment.Center;

            if (!MapForm.viewingPiecesMode)
            {
                e.Graphics.DrawString("Moving Units", new Font("Times New Roman", 12), new SolidBrush(Color.Black), new Point(UnitPanel.Width / 2, 13), sf);
                e.Graphics.DrawString("Moving Units", new Font("Times New Roman", 12), new SolidBrush(Color.White), new Point(UnitPanel.Width / 2, 12), sf);

                int _movePointsLeft = 3 * Game.Instance.ActiveUnit.MoveRate - Game.Instance.ActiveUnit.MovePointsLost;
                string movesLeft;
                if (_movePointsLeft % 3 == 0)
                {
                    movesLeft = "Moves: " + (_movePointsLeft / 3).ToString();
                }
                else
                {
                    movesLeft = "Moves: " + Convert.ToInt32(Math.Floor((double)_movePointsLeft / 3)).ToString() + " " + (_movePointsLeft % 3).ToString() + "/3";
                }
                e.Graphics.DrawString(movesLeft, new Font("Times New Roman", 12), new SolidBrush(Color.FromArgb(30, 30, 30)), new Point(70, 30));

                string homeCity;
                if (Game.Instance.ActiveUnit.HomeCity == 255) { homeCity = "NONE"; }  //FF in hex
                else { homeCity = Game.Cities[Game.Instance.ActiveUnit.HomeCity].Name; }
                e.Graphics.DrawString(homeCity, new Font("Times New Roman", 12), new SolidBrush(Color.FromArgb(30, 30, 30)), new Point(70, 48));

                string adjective = Game.Civs[Game.Instance.ActiveUnit.Civ].Adjective;
                e.Graphics.DrawString(adjective, new Font("Times New Roman", 12), new SolidBrush(Color.FromArgb(30, 30, 30)), new Point(70, 66));

                string unitName;
                if (Game.Instance.ActiveUnit.Veteran) { unitName = Game.Instance.ActiveUnit.Name + " (Veteran)"; }
                else { unitName = Game.Instance.ActiveUnit.Name; };
                e.Graphics.DrawString(unitName, new Font("Times New Roman", 12), new SolidBrush(Color.FromArgb(30, 30, 30)), new Point(10, 85));

                string unitTerrain = "(" + Game.Terrain[Game.Instance.ActiveUnit.X, Game.Instance.ActiveUnit.Y].Name + ")";
                e.Graphics.DrawString(unitTerrain, new Font("Times New Roman", 12), new SolidBrush(Color.FromArgb(30, 30, 30)), new Point(10, 103));

                e.Graphics.DrawImage(Draw.DrawUnit(Game.Instance.ActiveUnit, false, 1), 10, 30);
            }
            else
            {
                e.Graphics.DrawString("Viewing Pieces", new Font("Times New Roman", 12), new SolidBrush(Color.Black), new Point(UnitPanel.Width / 2, 13), sf);
                e.Graphics.DrawString("Viewing Pieces", new Font("Times New Roman", 12), new SolidBrush(Color.White), new Point(UnitPanel.Width / 2, 12), sf);

                int clickedX = (MapForm.ClickedBoxX - MapForm.ClickedBoxY % 2) / 2;    //convert from real to civ-2 style coordinates
                int clickedY = MapForm.ClickedBoxY;
                string sec_line = null;
                if (Game.Terrain[clickedX, clickedY].River) { sec_line = ", River"; }
                string third_line = null;
                if (Game.Terrain[clickedX, clickedY].SpecType != null && Game.Terrain[clickedX, clickedY].SpecType != SpecialType.GrasslandShield) { third_line = "(" + Game.Terrain[clickedX, clickedY].SpecName + ")"; }
                e.Graphics.DrawString("Loc: (" + MapForm.ClickedBoxX.ToString() + ", " + MapForm.ClickedBoxY.ToString() + ") " + Game.Terrain[clickedX, clickedY].Island.ToString() + "\n" + "(" + Game.Terrain[clickedX, clickedY].Type + sec_line + ")" + "\n" + third_line, new Font("Times New Roman", 12), new SolidBrush(Color.FromArgb(30, 30, 30)), new Point(10, 40));

                //Draw all units on the clicked square
                int count = 0;
                List<IUnit> unitMatches = Game.Units.FindAll(unit => unit.X == clickedX && unit.Y == clickedY);
                foreach (IUnit unit in unitMatches)
                {
                    e.Graphics.DrawImage(Draw.DrawUnit(unit, false, 1), 10, 90 + count * 3 * 18);
                    //Game.Cities[unit.HomeCity].Name
                    //Game.Cities[0].Name
                    e.Graphics.DrawString(Game.Cities[unit.HomeCity].Name, new Font("Times New Roman", 12), new SolidBrush(Color.FromArgb(30, 30, 30)), new Point(80, 90 + count * 3 * 18));
                    e.Graphics.DrawString(unit.Action.ToString(), new Font("Times New Roman", 12), new SolidBrush(Color.FromArgb(30, 30, 30)), new Point(80, 90 + count * 3 * 18 + 18));
                    e.Graphics.DrawString(unit.Name, new Font("Times New Roman", 12), new SolidBrush(Color.FromArgb(30, 30, 30)), new Point(80, 90 + count * 3 * 18 + 2 * 18));
                    count++;
                    if (count > 6)
                    {
                        int c = unitMatches.Count - 7;
                        e.Graphics.DrawString("(" + c.ToString() + " More Units)", new Font("Times New Roman", 12), new SolidBrush(Color.FromArgb(30, 30, 30)), new Point(10, 90 + count * 3 * 18));
                        break;
                    }
                }
            }
            sf.Dispose();
        }

        //Receive and display X-Y coordinates on right-click on Map
        public void ReceiveMousePositionFromMapForm()
        {
            RefreshStatusForm();
        }

        public void ShowEndOfTurnMessage()
        {
            EndOfTurnMessage = true;
            timer.Start();
            TimerCounter = 0;
        }

        public void HideEndOfTurnMessage()
        {
            EndOfTurnMessage = false;
            timer.Stop();
        }

        void TimerTick(object sender, EventArgs e)
        {
            //Make pulsating "Press enter for next turn" text
            TimerCounter++;
            UnitPanel.Refresh();
        }

        public void RefreshStatusForm()
        {
            UnitPanel.Refresh();
            StatsPanel.Refresh();
        }
    }
}
