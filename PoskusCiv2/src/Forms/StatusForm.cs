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

        Label viewingPiecesLabel, cursorPositionLabel;        
        Label peopleLabel = new Label();
        Label gameYearLabel = new Label();
        Label goldLabel = new Label();
        Label unitNoMovesLabel = new Label();
        Label unitCityLabel = new Label();
        Label unitCivLabel = new Label();
        Label unitTypeLabel = new Label();
        Label unitTerrainLabel = new Label();
        Label roadPresentLabel = new Label();
        Label endOfTurnLabel = new Label();
        PictureBox unitPicture = new PictureBox();
        PictureBox unitShieldPicture = new PictureBox();
        DrawUnits drawUnit = new DrawUnits();

        //timer
        Timer t = new Timer();
        int stej = 0;   //records no of timer ticks


        public StatusForm(MainCiv2Window _mainCiv2Window)
        {
            InitializeComponent();
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
            peopleLabel.AutoSize = true;
            peopleLabel.Location = new Point(13, 45);
            peopleLabel.Font = new Font("Times New Roman", 12);
            peopleLabel.ForeColor = Color.FromArgb(25,25,25);
            peopleLabel.Text = Game.people.ToString("#,##") + " People";
            this.Controls.Add(peopleLabel);
            var pos1 = this.PointToScreen(peopleLabel.Location);            //making the label transparent in panel
            pos1 = panel2.PointToClient(pos1);
            peopleLabel.Parent = panel2;
            peopleLabel.Location = pos1;
            peopleLabel.BackColor = Color.Transparent;

            //Game year label
            gameYearLabel.AutoSize = true;
            gameYearLabel.Location = new Point(13, 63);
            gameYearLabel.Font = new Font("Times New Roman", 12);
            gameYearLabel.ForeColor = Color.FromArgb(25, 25, 25);
            gameYearLabel.BackColor = Color.Transparent;
            gameYearLabel.Text = Math.Abs(Game.gameYear).ToString() + " B.C. (Turn " + Game.gameTurn.ToString() + ")";
            this.Controls.Add(gameYearLabel);
            var pos2 = this.PointToScreen(gameYearLabel.Location);            //making the label transparent in panel
            pos2 = panel2.PointToClient(pos2);
            gameYearLabel.Parent = panel2;
            gameYearLabel.Location = pos2;
            gameYearLabel.BackColor = Color.Transparent;

            //No of gold pieces label
            goldLabel.AutoSize = true;
            goldLabel.Location = new Point(13, 81);
            goldLabel.Font = new Font("Times New Roman", 12);
            goldLabel.ForeColor = Color.FromArgb(25, 25, 25);
            goldLabel.BackColor = Color.Transparent;
            goldLabel.Text = Game.gold.ToString() + " Gold";
            this.Controls.Add(goldLabel);
            var pos3 = this.PointToScreen(goldLabel.Location);            //making the label transparent in panel
            pos3 = panel2.PointToClient(pos3);
            goldLabel.Parent = panel2;
            goldLabel.Location = pos3;
            goldLabel.BackColor = Color.Transparent;

            //Unit no of moves label
            unitNoMovesLabel.AutoSize = true;
            unitNoMovesLabel.Location = new Point(90, 140);
            unitNoMovesLabel.Font = new Font("Times New Roman", 12);
            unitNoMovesLabel.ForeColor = Color.FromArgb(25, 25, 25);
            unitNoMovesLabel.BackColor = Color.Transparent;
            unitNoMovesLabel.Text = "Moves:";
            var pos6 = this.PointToScreen(unitNoMovesLabel.Location);            //making the label transparent in panel
            pos6 = panel1.PointToClient(pos6);
            unitNoMovesLabel.Parent = panel1;
            unitNoMovesLabel.Location = pos6;
            unitNoMovesLabel.BackColor = Color.Transparent;

            //Unit city label
            unitCityLabel.AutoSize = true;
            unitCityLabel.Location = new Point(90, 140+18);
            unitCityLabel.Font = new Font("Times New Roman", 12);
            unitCityLabel.ForeColor = Color.FromArgb(25, 25, 25);
            unitCityLabel.BackColor = Color.Transparent;
            unitCityLabel.Text = "City";
            var pos7 = this.PointToScreen(unitCityLabel.Location);            //making the label transparent in panel
            pos7 = panel1.PointToClient(pos7);
            unitCityLabel.Parent = panel1;
            unitCityLabel.Location = pos7;
            unitCityLabel.BackColor = Color.Transparent;

            //Unit civ label
            unitCivLabel.AutoSize = true;
            unitCivLabel.Location = new Point(90, 140 + 18 + 18);
            unitCivLabel.Font = new Font("Times New Roman", 12);
            unitCivLabel.ForeColor = Color.FromArgb(25, 25, 25);
            unitCivLabel.BackColor = Color.Transparent;
            unitCivLabel.Text = "Civ";
            var pos8 = this.PointToScreen(unitCivLabel.Location);            //making the label transparent in panel
            pos8 = panel1.PointToClient(pos8);
            unitCivLabel.Parent = panel1;
            unitCivLabel.Location = pos8;
            unitCivLabel.BackColor = Color.Transparent;

            //Unit type label
            unitTypeLabel.AutoSize = true;
            unitTypeLabel.Location = new Point(13, 140 + 18 + 18 + 18);
            unitTypeLabel.Font = new Font("Times New Roman", 12);
            unitTypeLabel.ForeColor = Color.FromArgb(25, 25, 25);
            unitTypeLabel.BackColor = Color.Transparent;
            unitTypeLabel.Text = "Type";
            var pos9 = this.PointToScreen(unitTypeLabel.Location);            //making the label transparent in panel
            pos9 = panel1.PointToClient(pos9);
            unitTypeLabel.Parent = panel1;
            unitTypeLabel.Location = pos9;
            unitTypeLabel.BackColor = Color.Transparent;

            //Unit terrain label
            unitTerrainLabel.AutoSize = true;
            unitTerrainLabel.Location = new Point(13, 140 + 18 + 18 + 18 + 18);
            unitTerrainLabel.Font = new Font("Times New Roman", 12);
            unitTerrainLabel.ForeColor = Color.FromArgb(25, 25, 25);
            unitTerrainLabel.BackColor = Color.Transparent;
            unitTerrainLabel.Text = "()";
            var pos10 = this.PointToScreen(unitTerrainLabel.Location);            //making the label transparent in panel
            pos10 = panel1.PointToClient(pos10);
            unitTerrainLabel.Parent = panel1;
            unitTerrainLabel.Location = pos10;
            unitTerrainLabel.BackColor = Color.Transparent;

            //Road present label
            roadPresentLabel.AutoSize = true;
            roadPresentLabel.Location = new Point(13, 140 + 18 + 18 + 18 + 18 + 18);
            roadPresentLabel.Font = new Font("Times New Roman", 12);
            roadPresentLabel.ForeColor = Color.FromArgb(25, 25, 25);
            roadPresentLabel.BackColor = Color.Transparent;
            var pos11 = this.PointToScreen(roadPresentLabel.Location);            //making the label transparent in panel
            pos11 = panel1.PointToClient(pos11);
            roadPresentLabel.Parent = panel1;
            roadPresentLabel.Location = pos11;
            roadPresentLabel.BackColor = Color.Transparent;

            //End-of-turn label
            endOfTurnLabel.AutoSize = true;
            endOfTurnLabel.Location = new Point(20, 510);
            endOfTurnLabel.Font = new Font("Times New Roman", 12);
            endOfTurnLabel.ForeColor = Color.FromArgb(25, 25, 25);
            endOfTurnLabel.BackColor = Color.Transparent;
            endOfTurnLabel.Text = "End of Turn \n (Press ENTER)";
            var pos12 = this.PointToScreen(endOfTurnLabel.Location);            //making the label transparent in panel
            pos12 = panel1.PointToClient(pos12);
            endOfTurnLabel.Parent = panel1;
            endOfTurnLabel.Location = pos12;
            endOfTurnLabel.BackColor = Color.Transparent;
            endOfTurnLabel.Visible = false;

            //Picture of unit in status form
            unitShieldPicture.Location = new Point(5, 30);
            unitShieldPicture.BackColor = Color.Transparent;
            unitPicture.Location = new Point(10, 30);
            unitPicture.BackColor = Color.Transparent;
            unitPicture.Parent = unitShieldPicture;
            //panel1.Controls.Add(unitShieldPicture);
            panel1.Controls.Add(unitPicture);

            UpdateUnitLabels(0);    //show unit labels in the beginning
        }

        //Receive and display X-Y coordinates on right-click on Map
        public void ReceiveMousePositionFromMapForm(int X_coord_mouse, int Y_coord_mouse)
        {
            
            int i = (X_coord_mouse - Y_coord_mouse % 2) / 2;
            int j = Y_coord_mouse;

            cursorPositionLabel.Text = "Loc: (" + X_coord_mouse.ToString() + ", " + Y_coord_mouse.ToString() + ") ";
            
            // + DrawMap.Map.TerrainIsland[Y_coord_mouse * DrawMap.Map.MapXdimension + (int)Math.Floor((double)X_coord_mouse / 2)] + " \n" + "(" + DrawMap.terrainName[DrawMap.Map.Terrain[Y_coord_mouse * DrawMap.Map.MapXdimension + (int)Math.Floor((double)X_coord_mouse / 2)]] + ")";
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
                if (unitInLine != Units_.unitNumber) //cycle through all units
                {
                    viewingPiecesLabel.Text = "Moving Units";
                    cursorPositionLabel.Visible = false;
                    endOfTurnLabel.Visible = false;
                    unitNoMovesLabel.Visible = true;
                    unitCityLabel.Visible = true;
                    unitCivLabel.Visible = true;
                    unitTerrainLabel.Visible = true;
                    unitTypeLabel.Visible = true;
                    unitPicture.Visible = true;
                    int _movesLeft = Game.Instance.ActiveUnit.MovesLeft;
                    if (_movesLeft % 3 == 0)
                    {
                        unitNoMovesLabel.Text = "Moves: " + (_movesLeft / 3).ToString();
                    }
                    else
                    {
                        unitNoMovesLabel.Text = "Moves: " + Convert.ToInt32(Math.Floor((double)_movesLeft / 3)).ToString() + " " + (_movesLeft % 3).ToString() + "/3";
                    }
                    if (Game.Terrain[Game.Instance.ActiveUnit.X, Game.Instance.ActiveUnit.Y].Road && !Game.Terrain[Game.Instance.ActiveUnit.X, Game.Instance.ActiveUnit.Y].CityPresent) { roadPresentLabel.Text = "(Road)"; }
                    else { roadPresentLabel.Text = null; }
                    if (Game.Instance.ActiveUnit.HomeCity == 255) { unitCityLabel.Text = "NONE"; }  //FF in hex
                    else { unitCityLabel.Text = Game.Cities[Game.Instance.ActiveUnit.HomeCity].Name; }
                    unitCivLabel.Text = Game.Civs[Game.Instance.ActiveUnit.Civ].Adjective;
                    if (Game.Instance.ActiveUnit.Veteran) { unitTypeLabel.Text = Game.Instance.ActiveUnit.Name + " (Veteran)"; }
                    else { unitTypeLabel.Text = unitTypeLabel.Text = Game.Instance.ActiveUnit.Name; };
                    unitTerrainLabel.Text = "(" + Game.Terrain[Game.Instance.ActiveUnit.X, Game.Instance.ActiveUnit.Y].Name + ")";
                    unitShieldPicture.Image = Images.UnitShield[(int)Game.Instance.ActiveUnit.Civ];
                    unitPicture.Image = Images.Units[(int)Game.Instance.ActiveUnit.Type];
                }
                else    //end of units, display wait for enter for new turn
                {
                    endOfTurnLabel.Visible = true;
                    cursorPositionLabel.Visible = false;
                    unitNoMovesLabel.Visible = false;
                    unitCityLabel.Visible = false;
                    unitCivLabel.Visible = false;
                    roadPresentLabel.Visible = false;
                    unitTerrainLabel.Visible = false;
                    unitTypeLabel.Visible = false;
                    unitPicture.Visible = false;
                    unitShieldPicture.Visible = false;

                    //timer for animating text
                    t.Interval = 500; // specify interval time as you want (ms)
                    t.Tick += new EventHandler(timer_Tick);
                    t.Start();
                }
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

        void timer_Tick(object sender, EventArgs e)
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
