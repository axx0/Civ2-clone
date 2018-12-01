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
    public partial class StatusForm : Form
    {
        public MainCiv2Window mainCiv2Window;

        //Label viewingPiecesLabel, cursorPositionLabel, peopleLabel, gameYearLabel, goldLabel, unitNoMovesLabel, unitCityLabel, unitCivLabel, unitTypeLabel, unitTerrainLabel, roadPresentLabel, endOfTurnLabel;
        //PictureBox unitPicture, unitShieldPicture;
        //DrawUnits drawUnit = new DrawUnits();
        DoubleBufferedPanel UnitPanel, StatsPanel;

        Draw Draw = new Draw();

        //timer
        Timer t = new Timer();
        int stej = 0;   //records no of timer ticks
        
        public StatusForm(MainCiv2Window _mainCiv2Window)
        {
            InitializeComponent();            
            mainCiv2Window = _mainCiv2Window;

            Size = new Size(262, 620);
            StartPosition = FormStartPosition.Manual;
            Location = new Point(1268, 150);
            FormBorderStyle = FormBorderStyle.None;
            BackgroundImage = Images.WallpaperMapForm;
            Paint += new PaintEventHandler(StatusForm_Paint);

            //Stats panel
            StatsPanel = new DoubleBufferedPanel
            {
                Location = new Point(8, 35),
                Size = new Size(248, 62),
                BackgroundImage = Images.WallpaperStatusForm,
                BorderStyle = BorderStyle.Fixed3D
            };
            Controls.Add(StatsPanel);
            StatsPanel.Paint += StatsPanel_Paint;

            //Unit panel
            UnitPanel = new DoubleBufferedPanel
            {
                Location = new Point(8, 102),
                Size = new Size(248, 512),
                BackgroundImage = Images.WallpaperStatusForm,
                BorderStyle = BorderStyle.Fixed3D
            };
            Controls.Add(UnitPanel);
            UnitPanel.Paint += UnitPanel_Paint;

            //Wallpaper
            //tableLayoutPanel1.BackgroundImage = Images.WallpaperMapForm;
            //UnitPanel.BackgroundImage = Images.WallpaperStatusForm;   //Panel background image
            //StatPanel.BackgroundImage = Images.WallpaperStatusForm;
        }

        private void StatusForm_Load(object sender, EventArgs e)
        {            
            ////Viewing pieces label
            //viewingPiecesLabel = new Label
            //{
            //    AutoSize = true,
            //    Location = new Point(70, 115),
            //    Font = new Font("Times New Roman", 12),
            //    ForeColor = Color.White,
            //    Text = "Moving Units",
            //    BackColor = Color.Transparent
            //};
            //Controls.Add(viewingPiecesLabel);
            //var pos4 = this.PointToScreen(viewingPiecesLabel.Location);            //making the label transparent in panel
            //pos4 = UnitPanel.PointToClient(pos4);
            //viewingPiecesLabel.Parent = UnitPanel;
            //viewingPiecesLabel.Location = pos4;
            //viewingPiecesLabel.BackColor = Color.Transparent;

            ////Cursor position label
            //cursorPositionLabel = new Label
            //{
            //    AutoSize = true,
            //    Location = new Point(13, 140),
            //    Font = new Font("Times New Roman", 12),
            //    BackColor = Color.Transparent
            //};
            //Controls.Add(cursorPositionLabel);
            //var pos5 = this.PointToScreen(cursorPositionLabel.Location);            //making the label transparent in panel
            //pos5 = UnitPanel.PointToClient(pos5);
            //cursorPositionLabel.Parent = UnitPanel;
            //cursorPositionLabel.Location = pos5;
            //cursorPositionLabel.BackColor = Color.Transparent;
            //cursorPositionLabel.Visible = false;

            ////No of people label
            //peopleLabel = new Label
            //{
            //    AutoSize = true,
            //    Location = new Point(13, 45),
            //    Font = new Font("Times New Roman", 12),
            //    ForeColor = Color.FromArgb(25, 25, 25),
            //    Text = Game.people.ToString("#,##") + " People"
            //};
            //Controls.Add(peopleLabel);
            //var pos1 = this.PointToScreen(peopleLabel.Location);            //making the label transparent in panel
            //pos1 = StatPanel.PointToClient(pos1);
            //peopleLabel.Parent = StatPanel;
            //peopleLabel.Location = pos1;
            //peopleLabel.BackColor = Color.Transparent;

            ////Game year label
            //gameYearLabel = new Label
            //{
            //    AutoSize = true,
            //    Location = new Point(13, 63),
            //    Font = new Font("Times New Roman", 12),
            //    ForeColor = Color.FromArgb(25, 25, 25),
            //    BackColor = Color.Transparent,
            //    Text = Math.Abs(Game.gameYear).ToString() + " B.C. (Turn " + Game.gameTurn.ToString() + ")"
            //};
            //Controls.Add(gameYearLabel);
            //var pos2 = this.PointToScreen(gameYearLabel.Location);            //making the label transparent in panel
            //pos2 = StatPanel.PointToClient(pos2);
            //gameYearLabel.Parent = StatPanel;
            //gameYearLabel.Location = pos2;
            //gameYearLabel.BackColor = Color.Transparent;

            ////No of gold pieces label
            //Label goldLabel = new Label
            //{
            //    AutoSize = true,
            //    Location = new Point(13, 81),
            //    Font = new Font("Times New Roman", 12),
            //    ForeColor = Color.FromArgb(25, 25, 25),
            //    BackColor = Color.Transparent,
            //    Text = Game.gold.ToString() + " Gold"
            //};
            //Controls.Add(goldLabel);
            //var pos3 = this.PointToScreen(goldLabel.Location);            //making the label transparent in panel
            //pos3 = StatPanel.PointToClient(pos3);
            //goldLabel.Parent = StatPanel;
            //goldLabel.Location = pos3;
            //goldLabel.BackColor = Color.Transparent;

            ////Unit no of moves label
            //Label unitNoMovesLabel = new Label
            //{
            //    AutoSize = true,
            //    Location = new Point(90, 140),
            //    Font = new Font("Times New Roman", 12),
            //    ForeColor = Color.FromArgb(25, 25, 25),
            //    BackColor = Color.Transparent,
            //    Text = "Moves:"
            //};
            //Controls.Add(unitNoMovesLabel);
            //var pos6 = this.PointToScreen(unitNoMovesLabel.Location);            //making the label transparent in panel
            //pos6 = UnitPanel.PointToClient(pos6);
            //unitNoMovesLabel.Parent = UnitPanel;
            //unitNoMovesLabel.Location = pos6;
            //unitNoMovesLabel.BackColor = Color.Transparent;

            ////Unit city label
            //Label unitCityLabel = new Label
            //{
            //    AutoSize = true,
            //    Location = new Point(90, 140 + 18),
            //    Font = new Font("Times New Roman", 12),
            //    ForeColor = Color.FromArgb(25, 25, 25),
            //    BackColor = Color.Transparent,
            //    Text = "City"
            //};
            //Controls.Add(unitCityLabel);
            //var pos7 = this.PointToScreen(unitCityLabel.Location);            //making the label transparent in panel
            //pos7 = UnitPanel.PointToClient(pos7);
            //unitCityLabel.Parent = UnitPanel;
            //unitCityLabel.Location = pos7;
            //unitCityLabel.BackColor = Color.Transparent;

            ////Unit civ label
            //Label unitCivLabel = new Label
            //{
            //    AutoSize = true,
            //    Location = new Point(90, 140 + 18 + 18),
            //    Font = new Font("Times New Roman", 12),
            //    ForeColor = Color.FromArgb(25, 25, 25),
            //    BackColor = Color.Transparent,
            //    Text = "Civ"
            //};
            //Controls.Add(unitCivLabel);
            //var pos8 = this.PointToScreen(unitCivLabel.Location);            //making the label transparent in panel
            //pos8 = UnitPanel.PointToClient(pos8);
            //unitCivLabel.Parent = UnitPanel;
            //unitCivLabel.Location = pos8;
            //unitCivLabel.BackColor = Color.Transparent;

            ////Unit type label
            //Label unitTypeLabel = new Label
            //{
            //    AutoSize = true,
            //    Location = new Point(13, 140 + 18 + 18 + 18),
            //    Font = new Font("Times New Roman", 12),
            //    ForeColor = Color.FromArgb(25, 25, 25),
            //    BackColor = Color.Transparent,
            //    Text = "Type"
            //};
            //Controls.Add(unitTypeLabel);
            //var pos9 = this.PointToScreen(unitTypeLabel.Location);            //making the label transparent in panel
            //pos9 = UnitPanel.PointToClient(pos9);
            //unitTypeLabel.Parent = UnitPanel;
            //unitTypeLabel.Location = pos9;
            //unitTypeLabel.BackColor = Color.Transparent;

            ////Unit terrain label
            //Label unitTerrainLabel = new Label
            //{
            //    AutoSize = true,
            //    Location = new Point(13, 140 + 18 + 18 + 18 + 18),
            //    Font = new Font("Times New Roman", 12),
            //    ForeColor = Color.FromArgb(25, 25, 25),
            //    BackColor = Color.Transparent,
            //    Text = "()"
            //};
            //Controls.Add(unitTerrainLabel);
            //var pos10 = this.PointToScreen(unitTerrainLabel.Location);            //making the label transparent in panel
            //pos10 = UnitPanel.PointToClient(pos10);
            //unitTerrainLabel.Parent = UnitPanel;
            //unitTerrainLabel.Location = pos10;
            //unitTerrainLabel.BackColor = Color.Transparent;

            ////Road present label
            //Label roadPresentLabel = new Label
            //{
            //    AutoSize = true,
            //    Location = new Point(13, 140 + 18 + 18 + 18 + 18 + 18),
            //    Font = new Font("Times New Roman", 12),
            //    ForeColor = Color.FromArgb(25, 25, 25),
            //    BackColor = Color.Transparent
            //};
            //Controls.Add(roadPresentLabel);
            //var pos11 = this.PointToScreen(roadPresentLabel.Location);            //making the label transparent in panel
            //pos11 = UnitPanel.PointToClient(pos11);
            //roadPresentLabel.Parent = UnitPanel;
            //roadPresentLabel.Location = pos11;
            //roadPresentLabel.BackColor = Color.Transparent;

            ////End-of-turn label
            //Label endOfTurnLabel = new Label
            //{
            //    AutoSize = true,
            //    Location = new Point(20, 510),
            //    Font = new Font("Times New Roman", 12),
            //    ForeColor = Color.FromArgb(25, 25, 25),
            //    BackColor = Color.Transparent,
            //    Text = "End of Turn \n (Press ENTER)"
            //};
            //Controls.Add(endOfTurnLabel);
            //var pos12 = this.PointToScreen(endOfTurnLabel.Location);            //making the label transparent in panel
            //pos12 = UnitPanel.PointToClient(pos12);
            //endOfTurnLabel.Parent = UnitPanel;
            //endOfTurnLabel.Location = pos12;
            //endOfTurnLabel.BackColor = Color.Transparent;
            //endOfTurnLabel.Visible = false;

            ////Picture of unit in status form
            //PictureBox unitShieldPicture = new PictureBox
            //{
            //    Location = new Point(5, 30),
            //    BackColor = Color.Transparent
            //};
            ////panel1.Controls.Add(unitShieldPicture);

            //PictureBox unitPicture = new PictureBox
            //{
            //    Location = new Point(10, 30),
            //    BackColor = Color.Transparent,
            //    Parent = unitShieldPicture
            //};
            //UnitPanel.Controls.Add(unitPicture);

            //UpdateUnitLabels(0);    //show unit labels in the beginning
        }

        private void StatusForm_Paint(object sender, PaintEventArgs e)
        {
            StringFormat sf = new StringFormat();
            sf.LineAlignment = StringAlignment.Center;
            sf.Alignment = StringAlignment.Center;
            e.Graphics.DrawString("Status", new Font("Times New Roman", 19), new SolidBrush(Color.Black), new Point(this.Width / 2 + 1, 20), sf);
            e.Graphics.DrawString("Status", new Font("Times New Roman", 19), new SolidBrush(Color.FromArgb(135, 135, 135)), new Point(this.Width / 2, 19), sf);
            sf.Dispose();
        }

        private void StatsPanel_Paint(object sender, PaintEventArgs e)
        {
            string bcad;
            if (Game.Data.GameYear < 0) { bcad = "B.C."; }
            else { bcad = "A.D."; }

            e.Graphics.DrawString("640,000 People", new Font("Times New Roman", 13), new SolidBrush(Color.FromArgb(30, 30, 30)), new Point(5, 2));
            e.Graphics.DrawString(Math.Abs(Game.Data.GameYear).ToString() + " " + bcad + "(Turn " + Game.Data.TurnNumber + ")", new Font("Times New Roman", 13), new SolidBrush(Color.FromArgb(30, 30, 30)), new Point(5, 20));
            e.Graphics.DrawString("250 Gold 5.0.5", new Font("Times New Roman", 13), new SolidBrush(Color.FromArgb(30, 30, 30)), new Point(5, 38));
        }

        private void UnitPanel_Paint(object sender, PaintEventArgs e)
        {
            if (!MapForm.viewingPiecesMode)
            {
                StringFormat sf = new StringFormat();
                sf.LineAlignment = StringAlignment.Center;
                sf.Alignment = StringAlignment.Center;
                e.Graphics.DrawString("Moving Units", new Font("Times New Roman", 12), new SolidBrush(Color.Black), new Point(126, 13), sf);
                e.Graphics.DrawString("Moving Units", new Font("Times New Roman", 12), new SolidBrush(Color.White), new Point(125, 12), sf);

                int _movePointsLeft = 3 * Game.Instance.ActiveUnit.StartingMoves - Game.Instance.ActiveUnit.MovePointsLost;
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

                sf.Dispose();

                e.Graphics.DrawImage(Draw.DrawUnit(Game.Instance.ActiveUnit, false), 10, 30);
            }
            else
            {
                StringFormat sf = new StringFormat();
                sf.LineAlignment = StringAlignment.Center;
                sf.Alignment = StringAlignment.Center;
                e.Graphics.DrawString("Viewing Pieces", new Font("Times New Roman", 12), new SolidBrush(Color.Black), new Point(126, 13), sf);
                e.Graphics.DrawString("Viewing Pieces", new Font("Times New Roman", 12), new SolidBrush(Color.White), new Point(125, 12), sf);
                sf.Dispose();

                int clickedX = (MapForm.ClickedBoxX - MapForm.ClickedBoxY % 2) / 2;    //convert from real to civ-2 style coordinates
                int clickedY = MapForm.ClickedBoxY;
                string sec_line = null;
                if (Game.Terrain[clickedX, clickedY].River) { sec_line = ", River"; }
                e.Graphics.DrawString("Loc: (" + MapForm.ClickedBoxX.ToString() + ", " + MapForm.ClickedBoxY.ToString() + ") " + Game.Terrain[clickedX, clickedY].Island.ToString() + "\n" + "(" + Game.Terrain[clickedX, clickedY].Type + sec_line + ")", new Font("Times New Roman", 12), new SolidBrush(Color.FromArgb(30, 30, 30)), new Point(10, 40));

                //Draw all units on the clicked square
                int count = 0;
                List<IUnit> unitMatches = Game.Units.FindAll(unit => unit.X == clickedX && unit.Y == clickedY);
                foreach (IUnit unit in unitMatches)
                {
                    e.Graphics.DrawImage(Draw.DrawUnit(unit, false), 10, 90 + count * 3 * 18);
                    //Game.Cities[unit.HomeCity].Name
                    //Game.Cities[0].Name
                    e.Graphics.DrawString(Game.Cities[unit.HomeCity].Name, new Font("Times New Roman", 12), new SolidBrush(Color.FromArgb(30, 30, 30)), new Point(80, 90 + count * 3 * 18));
                    e.Graphics.DrawString(unit.Action.ToString(), new Font("Times New Roman", 12), new SolidBrush(Color.FromArgb(30, 30, 30)), new Point(80, 90 + count * 3 * 18 + 18));
                    e.Graphics.DrawString(unit.Name, new Font("Times New Roman", 12), new SolidBrush(Color.FromArgb(30, 30, 30)), new Point(80, 90 + count * 3 * 18 + 2 * 18));
                    count++;
                    if (count > 6)
                    {
                        int c = unitMatches.Count - 7;
                        e.Graphics.DrawString("(" + c.ToString() + " More Units)", new Font("Times New Roman", 12), new SolidBrush(Color.FromArgb(30, 30, 30)), new Point (10, 90 + count * 3 * 18));
                        break;
                    }
                }

                
            }
        }

        //Receive and display X-Y coordinates on right-click on Map
        public void ReceiveMousePositionFromMapForm()
        {
            InvalidatePanel();
        }

        //Update game year label
        public void UpdateGameYearLabel(string myText)
        {
            //this.gameYearLabel.Text = myText;
        }

        //Update unit labels
        //public void UpdateUnitLabels()
        //{
        //    if (MapForm.viewingPiecesMode == false)
        //    {
                //if (unitInLine != Units_.unitNumber) //cycle through all units
                //{
                //    viewingPiecesLabel.Text = "Moving Units";
                //    cursorPositionLabel.Visible = false;
                //    endOfTurnLabel.Visible = false;
                //    unitNoMovesLabel.Visible = true;
                //    unitCityLabel.Visible = true;
                //    unitCivLabel.Visible = true;
                //    unitTerrainLabel.Visible = true;
                //    unitTypeLabel.Visible = true;
                //    unitPicture.Visible = true;
                //    int _movesLeft = Game.Instance.ActiveUnit.MovesLeft;
                //    if (_movesLeft % 3 == 0)
                //    {
                //        unitNoMovesLabel.Text = "Moves: " + (_movesLeft / 3).ToString();
                //    }
                //    else
                //    {
                //        unitNoMovesLabel.Text = "Moves: " + Convert.ToInt32(Math.Floor((double)_movesLeft / 3)).ToString() + " " + (_movesLeft % 3).ToString() + "/3";
                //    }
                //    if (Game.Terrain[Game.Instance.ActiveUnit.X, Game.Instance.ActiveUnit.Y].Road && !Game.Terrain[Game.Instance.ActiveUnit.X, Game.Instance.ActiveUnit.Y].CityPresent) { roadPresentLabel.Text = "(Road)"; }
                //    else { roadPresentLabel.Text = null; }
                //    if (Game.Instance.ActiveUnit.HomeCity == 255) { unitCityLabel.Text = "NONE"; }  //FF in hex
                //    else { unitCityLabel.Text = Game.Cities[Game.Instance.ActiveUnit.HomeCity].Name; }
                //    unitCivLabel.Text = Game.Civs[Game.Instance.ActiveUnit.Civ].Adjective;
                //    if (Game.Instance.ActiveUnit.Veteran) { unitTypeLabel.Text = Game.Instance.ActiveUnit.Name + " (Veteran)"; }
                //    else { unitTypeLabel.Text = unitTypeLabel.Text = Game.Instance.ActiveUnit.Name; };
                //    unitTerrainLabel.Text = "(" + Game.Terrain[Game.Instance.ActiveUnit.X, Game.Instance.ActiveUnit.Y].Name + ")";
                //    unitShieldPicture.Image = Images.UnitShield[(int)Game.Instance.ActiveUnit.Civ];
                //    unitPicture.Image = Images.Units[(int)Game.Instance.ActiveUnit.Type];
                //}
                //else    //end of units, display wait for enter for new turn
                //{
                //    endOfTurnLabel.Visible = true;
                //    cursorPositionLabel.Visible = false;
                //    unitNoMovesLabel.Visible = false;
                //    unitCityLabel.Visible = false;
                //    unitCivLabel.Visible = false;
                //    roadPresentLabel.Visible = false;
                //    unitTerrainLabel.Visible = false;
                //    unitTypeLabel.Visible = false;
                //    unitPicture.Visible = false;
                //    unitShieldPicture.Visible = false;

                //    //timer for animating text
                //    t.Interval = 500; // specify interval time as you want (ms)
                //    t.Tick += new EventHandler(timer_Tick);
                //    t.Start();
                //}
            //}
            //else
            //{
                //viewingPiecesLabel.Text = "Viewing Pieces";
                //cursorPositionLabel.Visible = true;
                //unitNoMovesLabel.Visible = false;
                //unitCityLabel.Visible = false;
                //unitCivLabel.Visible = false;
                //roadPresentLabel.Visible = false;
                //unitTerrainLabel.Visible = false;
                //unitTypeLabel.Visible = false;
                //unitPicture.Visible = false;
                //unitShieldPicture.Visible = false;
        //    }
        //}

        //command to stop animating "press enter for next turn" text
        public void StopTimerInStatusForm()
        {
            t.Stop();
            //endOfTurnLabel.Visible = false;
        }

        void Timer_Tick(object sender, EventArgs e)
        {
            stej += 1;

            //Make pulsating "Press enter for next turn" text
            //if (stej % 2 == 1) { endOfTurnLabel.ForeColor = Color.FromArgb(25, 25, 25); }
            //else { endOfTurnLabel.ForeColor = Color.White; }

        }


        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void flowLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label2_Click_1(object sender, EventArgs e)
        {

        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void StatusForm_MouseClick(object sender, MouseEventArgs e)
        {
            
        }

        private void panel1_MouseClick(object sender, MouseEventArgs e)
        {
            //MapForm.viewingPiecesMode = false;
            //UpdateUnitLabels(Game.unitInLine);
        }

        public void InvalidatePanel()
        {
            UnitPanel.Invalidate();
            StatsPanel.Invalidate();
        }
    }
}
