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

        Label viewingPiecesLabel, cursorPositionLabel, peopleLabel, gameYearLabel, goldLabel, unitNoMovesLabel, unitCityLabel, unitCivLabel, unitTypeLabel, unitTerrainLabel, roadPresentLabel, endOfTurnLabel;
        PictureBox unitPicture, unitShieldPicture;
        DrawUnits drawUnit = new DrawUnits();

        //timer
        Timer t = new Timer();
        int stej = 0;   //records no of timer ticks


        public StatusForm(MainCiv2Window _mainCiv2Window)
        {
            InitializeComponent();
            this.Paint += StatusForm_Paint;
            mainCiv2Window = _mainCiv2Window;
        }

        private void StatusForm_Load(object sender, EventArgs e)
        {
            //Wallpaper
            tableLayoutPanel1.BackgroundImage = Images.WallpaperMapForm;            
            panel1.BackgroundImage = Images.WallpaperStatusForm;   //Panel background image
            panel2.BackgroundImage = Images.WallpaperStatusForm;
            
            //Viewing pieces label
            viewingPiecesLabel = new Label
            {
                AutoSize = true,
                Location = new Point(70, 115),
                Font = new Font("Times New Roman", 12),
                ForeColor = Color.White,
                Text = "Moving Units",
                BackColor = Color.Transparent
            };
            Controls.Add(viewingPiecesLabel);
            var pos4 = this.PointToScreen(viewingPiecesLabel.Location);            //making the label transparent in panel
            pos4 = panel1.PointToClient(pos4);
            viewingPiecesLabel.Parent = panel1;
            viewingPiecesLabel.Location = pos4;
            viewingPiecesLabel.BackColor = Color.Transparent;

            //Cursor position label
            cursorPositionLabel = new Label
            {
                AutoSize = true,
                Location = new Point(13, 140),
                Font = new Font("Times New Roman", 12),
                BackColor = Color.Transparent
            };
            Controls.Add(cursorPositionLabel);
            var pos5 = this.PointToScreen(cursorPositionLabel.Location);            //making the label transparent in panel
            pos5 = panel1.PointToClient(pos5);
            cursorPositionLabel.Parent = panel1;
            cursorPositionLabel.Location = pos5;
            cursorPositionLabel.BackColor = Color.Transparent;
            cursorPositionLabel.Visible = false;

            //No of people label
            peopleLabel = new Label
            {
                AutoSize = true,
                Location = new Point(13, 45),
                Font = new Font("Times New Roman", 12),
                ForeColor = Color.FromArgb(25, 25, 25),
                Text = Game.people.ToString("#,##") + " People"
            };
            Controls.Add(peopleLabel);
            var pos1 = this.PointToScreen(peopleLabel.Location);            //making the label transparent in panel
            pos1 = panel2.PointToClient(pos1);
            peopleLabel.Parent = panel2;
            peopleLabel.Location = pos1;
            peopleLabel.BackColor = Color.Transparent;

            //Game year label
            gameYearLabel = new Label
            {
                AutoSize = true,
                Location = new Point(13, 63),
                Font = new Font("Times New Roman", 12),
                ForeColor = Color.FromArgb(25, 25, 25),
                BackColor = Color.Transparent,
                Text = Math.Abs(Game.gameYear).ToString() + " B.C. (Turn " + Game.gameTurn.ToString() + ")"
            };
            Controls.Add(gameYearLabel);
            var pos2 = this.PointToScreen(gameYearLabel.Location);            //making the label transparent in panel
            pos2 = panel2.PointToClient(pos2);
            gameYearLabel.Parent = panel2;
            gameYearLabel.Location = pos2;
            gameYearLabel.BackColor = Color.Transparent;

            //No of gold pieces label
            Label goldLabel = new Label
            {
                AutoSize = true,
                Location = new Point(13, 81),
                Font = new Font("Times New Roman", 12),
                ForeColor = Color.FromArgb(25, 25, 25),
                BackColor = Color.Transparent,
                Text = Game.gold.ToString() + " Gold"
            };
            Controls.Add(goldLabel);
            var pos3 = this.PointToScreen(goldLabel.Location);            //making the label transparent in panel
            pos3 = panel2.PointToClient(pos3);
            goldLabel.Parent = panel2;
            goldLabel.Location = pos3;
            goldLabel.BackColor = Color.Transparent;

            //Unit no of moves label
            Label unitNoMovesLabel = new Label
            {
                AutoSize = true,
                Location = new Point(90, 140),
                Font = new Font("Times New Roman", 12),
                ForeColor = Color.FromArgb(25, 25, 25),
                BackColor = Color.Transparent,
                Text = "Moves:"
            };
            Controls.Add(unitNoMovesLabel);
            var pos6 = this.PointToScreen(unitNoMovesLabel.Location);            //making the label transparent in panel
            pos6 = panel1.PointToClient(pos6);
            unitNoMovesLabel.Parent = panel1;
            unitNoMovesLabel.Location = pos6;
            unitNoMovesLabel.BackColor = Color.Transparent;

            //Unit city label
            Label unitCityLabel = new Label
            {
                AutoSize = true,
                Location = new Point(90, 140 + 18),
                Font = new Font("Times New Roman", 12),
                ForeColor = Color.FromArgb(25, 25, 25),
                BackColor = Color.Transparent,
                Text = "City"
            };
            Controls.Add(unitCityLabel);
            var pos7 = this.PointToScreen(unitCityLabel.Location);            //making the label transparent in panel
            pos7 = panel1.PointToClient(pos7);
            unitCityLabel.Parent = panel1;
            unitCityLabel.Location = pos7;
            unitCityLabel.BackColor = Color.Transparent;

            //Unit civ label
            Label unitCivLabel = new Label
            {
                AutoSize = true,
                Location = new Point(90, 140 + 18 + 18),
                Font = new Font("Times New Roman", 12),
                ForeColor = Color.FromArgb(25, 25, 25),
                BackColor = Color.Transparent,
                Text = "Civ"
            };
            Controls.Add(unitCivLabel);
            var pos8 = this.PointToScreen(unitCivLabel.Location);            //making the label transparent in panel
            pos8 = panel1.PointToClient(pos8);
            unitCivLabel.Parent = panel1;
            unitCivLabel.Location = pos8;
            unitCivLabel.BackColor = Color.Transparent;

            //Unit type label
            Label unitTypeLabel = new Label
            {
                AutoSize = true,
                Location = new Point(13, 140 + 18 + 18 + 18),
                Font = new Font("Times New Roman", 12),
                ForeColor = Color.FromArgb(25, 25, 25),
                BackColor = Color.Transparent,
                Text = "Type"
            };
            Controls.Add(unitTypeLabel);
            var pos9 = this.PointToScreen(unitTypeLabel.Location);            //making the label transparent in panel
            pos9 = panel1.PointToClient(pos9);
            unitTypeLabel.Parent = panel1;
            unitTypeLabel.Location = pos9;
            unitTypeLabel.BackColor = Color.Transparent;

            //Unit terrain label
            Label unitTerrainLabel = new Label
            {
                AutoSize = true,
                Location = new Point(13, 140 + 18 + 18 + 18 + 18),
                Font = new Font("Times New Roman", 12),
                ForeColor = Color.FromArgb(25, 25, 25),
                BackColor = Color.Transparent,
                Text = "()"
            };
            Controls.Add(unitTerrainLabel);
            var pos10 = this.PointToScreen(unitTerrainLabel.Location);            //making the label transparent in panel
            pos10 = panel1.PointToClient(pos10);
            unitTerrainLabel.Parent = panel1;
            unitTerrainLabel.Location = pos10;
            unitTerrainLabel.BackColor = Color.Transparent;

            //Road present label
            Label roadPresentLabel = new Label
            {
                AutoSize = true,
                Location = new Point(13, 140 + 18 + 18 + 18 + 18 + 18),
                Font = new Font("Times New Roman", 12),
                ForeColor = Color.FromArgb(25, 25, 25),
                BackColor = Color.Transparent
            };
            Controls.Add(roadPresentLabel);
            var pos11 = this.PointToScreen(roadPresentLabel.Location);            //making the label transparent in panel
            pos11 = panel1.PointToClient(pos11);
            roadPresentLabel.Parent = panel1;
            roadPresentLabel.Location = pos11;
            roadPresentLabel.BackColor = Color.Transparent;

            //End-of-turn label
            Label endOfTurnLabel = new Label
            {
                AutoSize = true,
                Location = new Point(20, 510),
                Font = new Font("Times New Roman", 12),
                ForeColor = Color.FromArgb(25, 25, 25),
                BackColor = Color.Transparent,
                Text = "End of Turn \n (Press ENTER)"
            };
            Controls.Add(endOfTurnLabel);
            var pos12 = this.PointToScreen(endOfTurnLabel.Location);            //making the label transparent in panel
            pos12 = panel1.PointToClient(pos12);
            endOfTurnLabel.Parent = panel1;
            endOfTurnLabel.Location = pos12;
            endOfTurnLabel.BackColor = Color.Transparent;
            endOfTurnLabel.Visible = false;

            //Picture of unit in status form
            PictureBox unitShieldPicture = new PictureBox
            {
                Location = new Point(5, 30),
                BackColor = Color.Transparent
            };
            //panel1.Controls.Add(unitShieldPicture);

            PictureBox unitPicture = new PictureBox
            {
                Location = new Point(10, 30),
                BackColor = Color.Transparent,
                Parent = unitShieldPicture
            };
            panel1.Controls.Add(unitPicture);

            UpdateUnitLabels(0);    //show unit labels in the beginning
        }

        private void StatusForm_Paint(object sender, PaintEventArgs e)
        {
            //NOT WORKING -- BEHIND PANEL!
            StringFormat sf = new StringFormat();
            sf.LineAlignment = StringAlignment.Center;
            sf.Alignment = StringAlignment.Center;
            e.Graphics.DrawString("Status", new Font("Times New Roman", 18), new SolidBrush(Color.Black), new Point(this.Width / 2 + 1, 11), sf);
            e.Graphics.DrawString("Status", new Font("Times New Roman", 18), new SolidBrush(Color.FromArgb(135, 135, 135)), new Point(this.Width / 2, 10), sf);
            sf.Dispose();
        }

        //Receive and display X-Y coordinates on right-click on Map
        public void ReceiveMousePositionFromMapForm(int X_coord_mouse, int Y_coord_mouse)
        {            
            int x = 2 * X_coord_mouse + Y_coord_mouse % 2;    //convert from real to civ-2 style coordinates
            int y = Y_coord_mouse;
            string sec_line = null;
            if (Game.Terrain[X_coord_mouse, Y_coord_mouse].River) { sec_line = ", River"; }
            cursorPositionLabel.Text = "Loc: (" + x.ToString() + ", " + y.ToString() + ") " + Game.Terrain[X_coord_mouse, Y_coord_mouse].Island.ToString() + " \n" + "(" + Game.Terrain[X_coord_mouse, Y_coord_mouse].Type + sec_line + ")";
        }

        //Update game year label
        public void UpdateGameYearLabel(string myText)
        {
            this.gameYearLabel.Text = myText;
        }

        //Update unit labels
        public void UpdateUnitLabels(int unitInLine)
        {
            if (MapForm.viewingPiecesMode == false)
            {
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
            }
            else
            {
                viewingPiecesLabel.Text = "Viewing Pieces";
                cursorPositionLabel.Visible = true;
                unitNoMovesLabel.Visible = false;
                unitCityLabel.Visible = false;
                unitCivLabel.Visible = false;
                roadPresentLabel.Visible = false;
                unitTerrainLabel.Visible = false;
                unitTypeLabel.Visible = false;
                unitPicture.Visible = false;
                unitShieldPicture.Visible = false;
            }
        }

        //command to stop animating "press enter for next turn" text
        public void StopTimerInStatusForm()
        {
            t.Stop();
            endOfTurnLabel.Visible = false;
        }

        void Timer_Tick(object sender, EventArgs e)
        {
            stej += 1;

            //Make pulsating "Press enter for next turn" text
            if (stej % 2 == 1) { endOfTurnLabel.ForeColor = Color.FromArgb(25, 25, 25); }
            else { endOfTurnLabel.ForeColor = Color.White; }

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
            MapForm.viewingPiecesMode = false;
            UpdateUnitLabels(Game.unitInLine);
        }
    }
}
