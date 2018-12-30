using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PoskusCiv2.Imagery;
using PoskusCiv2.Units;
using PoskusCiv2.Improvements;

namespace PoskusCiv2.Forms
{
    public partial class CityForm : Form
    {
        public MainCiv2Window mainCiv2Window;
        Draw Draw = new Draw();
        City ThisCity;
        Bitmap CityDrawing;
        Panel WallpaperPanel, Faces, ResourceMap, CityResources, UnitsFromCity, UnitsInCity, FoodStorage, Production;
        Form CallingForm;
        VScrollBar ImprovementsBar;
        int[,] offsets;

        public CityForm(MainCiv2Window _mainCiv2Window)
        {
            InitializeComponent();
            mainCiv2Window = _mainCiv2Window;
        }

        public CityForm(Form _callingForm, City city)
        {
            InitializeComponent();

            ThisCity = city;

            CallingForm = _callingForm;

            Size = new Size(976, 680);  //normalen zoom = (650,453)
            FormBorderStyle = FormBorderStyle.None;
            BackgroundImage = Images.WallpaperMapForm;
            
            this.Load += new EventHandler(CityForm_Load);
            this.Paint += new PaintEventHandler(CityForm_Paint);
            
            //Sizes & locations of 6 buttons in bottom right corner
            Size buttonSize = new Size(84, 35); //size
            Point buttonXYloc = new Point(695, 580);    //location of 1st button
            int buttonSepX = 5; //separation between buttons in X
            int buttonSepY = 5; //separation between buttons in Y
            Font buttonFont = new Font("Arial", 13);

            //Panel for wallpaper
            WallpaperPanel = new Panel
            {
                Location = new Point(12, 37),    //normal zoom = (8,25)
                Size = new Size(960, 630),      //normal zoom = (640,420)
                BackgroundImage = Images.CityWallpaper,
                BackgroundImageLayout = ImageLayout.Stretch,
                BorderStyle = BorderStyle.Fixed3D
            };
            Controls.Add(WallpaperPanel);
            WallpaperPanel.Paint += new PaintEventHandler(WallpaperPanel_Paint);
            WallpaperPanel.Paint += new PaintEventHandler(ImprovementsList_Paint);

            //Faces panel
            Faces = new Panel
            {
                Location = new Point(10, 10),
                Size = new Size(630, 50),    //stretched by 12.5 %
                BackColor = Color.Transparent
            };
            WallpaperPanel.Controls.Add(Faces);
            Faces.Paint += new PaintEventHandler(Faces_Paint);

            //Resource map panel
            ResourceMap = new Panel
            {
                Location = new Point(7, 125),
                Size = new Size(4 * 72, 4 * 36),    //stretched by 12.5 %
                BackColor = Color.Transparent
            };
            WallpaperPanel.Controls.Add(ResourceMap);
            ResourceMap.Paint += new PaintEventHandler(ResourceMap_Paint);

            //City resources panel
            CityResources = new Panel
            {
                Location = new Point(300, 70),
                Size = new Size(350, 245),    //stretched by 12.5 %
                BackColor = Color.Transparent
            };
            WallpaperPanel.Controls.Add(CityResources);
            CityResources.Paint += new PaintEventHandler(CityResources_Paint);

            //Units from city panel
            UnitsFromCity = new Panel
            {
                Location = new Point(10, 321),
                Size = new Size(270, 104),
                BackColor = Color.Transparent
            };
            WallpaperPanel.Controls.Add(UnitsFromCity);
            UnitsFromCity.Paint += new PaintEventHandler(UnitsFromCity_Paint);

            //Units in city panel
            UnitsInCity = new Panel
            {
                Location = new Point(288, 322),
                Size = new Size(360, 245),
                BackColor = Color.Transparent
            };
            WallpaperPanel.Controls.Add(UnitsInCity);
            UnitsInCity.Paint += new PaintEventHandler(UnitsInCity_Paint);

            //Food storage panel
            FoodStorage = new Panel
            {
                Location = new Point(653, 0),
                Size = new Size(291, 244),
                BackColor = Color.Transparent
            };
            WallpaperPanel.Controls.Add(FoodStorage);
            FoodStorage.Paint += new PaintEventHandler(FoodStorage_Paint);

            //Production panel
            Production = new Panel
            {
                Location = new Point(653, 246),
                Size = new Size(291, 285),
                BackColor = Color.Transparent
            };
            WallpaperPanel.Controls.Add(Production);
            Production.Paint += new PaintEventHandler(Production_Paint);

            //Buy button
            //Button BuyButton = new Button
            Civ2button BuyButton = new Civ2button
            {
                Location = new Point(8, 24),
                Size = new Size(102, 36),
                ForeColor = Color.Black,
                BackColor = Color.FromArgb(192, 192, 192),
                Font = buttonFont,
                FlatStyle = FlatStyle.Flat,
                Text = "Buy"
            };
            BuyButton.FlatAppearance.BorderSize = 0;
            BuyButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(192, 192, 192);
            Production.Controls.Add(BuyButton);
            BuyButton.BringToFront();
            BuyButton.Click += new EventHandler(BuyButton_Click);
            BuyButton.Paint += new PaintEventHandler(Button_Paint);

            //Change button
            Button ChangeButton = new Button
            {
                Location = new Point(180, 24),
                Size = new Size(102, 36),
                ForeColor = Color.Black,
                BackColor = Color.FromArgb(192, 192, 192),
                Font = buttonFont,
                FlatStyle = FlatStyle.Flat,
                Text = "Change"
            };
            ChangeButton.FlatAppearance.BorderSize = 0;
            ChangeButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(192, 192, 192);
            Production.Controls.Add(ChangeButton);
            ChangeButton.BringToFront();
            ChangeButton.Click += new EventHandler(ChangeButton_Click);
            ChangeButton.Paint += new PaintEventHandler(Button_Paint);

            //Info button
            Button InfoButton = new Button
            {
                Location = new Point(buttonXYloc.X, buttonXYloc.Y),
                Size = buttonSize,
                ForeColor = Color.Black,
                BackColor = Color.FromArgb(192, 192, 192),
                Font = buttonFont,
                FlatStyle = FlatStyle.Flat,                
                Text = "Info"
            };
            InfoButton.FlatAppearance.BorderSize = 0;
            InfoButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(192, 192, 192);
            Controls.Add(InfoButton);
            InfoButton.BringToFront();
            InfoButton.Click += new EventHandler(InfoButton_Click);
            InfoButton.Paint += new PaintEventHandler(Button_Paint);
            
            //Map button
            Button MapButton = new Button
            {
                Location = new Point(buttonXYloc.X + buttonSize.Width + buttonSepX, buttonXYloc.Y),
                Size = buttonSize,
                BackColor = Color.FromArgb(192, 192, 192),
                Font = buttonFont,
                FlatStyle = FlatStyle.Flat,
                Text = "Map"
            };
            MapButton.FlatAppearance.BorderSize = 0;
            MapButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(192, 192, 192);
            Controls.Add(MapButton);
            MapButton.BringToFront();
            MapButton.Click += new EventHandler(MapButton_Click);
            MapButton.Paint += new PaintEventHandler(Button_Paint);

            //Rename button
            Button RenameButton = new Button
            {
                Location = new Point(buttonXYloc.X + 2 * buttonSize.Width + 2 * buttonSepX, buttonXYloc.Y),
                Size = buttonSize,
                BackColor = Color.FromArgb(192, 192, 192),
                Font = buttonFont,
                FlatStyle = FlatStyle.Flat,
                Text = "Rename"
            };
            RenameButton.FlatAppearance.BorderSize = 0;
            RenameButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(192, 192, 192);
            Controls.Add(RenameButton);
            RenameButton.BringToFront();
            RenameButton.Click += new EventHandler(RenameButton_Click);
            RenameButton.Paint += new PaintEventHandler(Button_Paint);

            //Happy button
            Button HappyButton = new Button
            {
                Location = new Point(buttonXYloc.X, buttonXYloc.Y + buttonSize.Height + buttonSepY),
                Size = buttonSize,
                BackColor = Color.FromArgb(192, 192, 192),
                Font = buttonFont,
                FlatStyle = FlatStyle.Flat,
                Text = "Happy"
            };
            HappyButton.FlatAppearance.BorderSize = 0;
            HappyButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(192, 192, 192);
            Controls.Add(HappyButton);
            HappyButton.BringToFront();
            HappyButton.Click += new EventHandler(HappyButton_Click);
            HappyButton.Paint += new PaintEventHandler(Button_Paint);

            //View button
            Button ViewButton = new Button
            {
                Location = new Point(buttonXYloc.X + buttonSize.Width + buttonSepX, buttonXYloc.Y + buttonSize.Height + buttonSepY),
                Size = buttonSize,
                BackColor = Color.FromArgb(192, 192, 192),
                Font = buttonFont,
                FlatStyle = FlatStyle.Flat,
                Text = "View"
            };
            ViewButton.FlatAppearance.BorderSize = 0;
            ViewButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(192, 192, 192);
            Controls.Add(ViewButton);
            ViewButton.BringToFront();
            ViewButton.Click += new EventHandler(ViewButton_Click);
            ViewButton.Paint += new PaintEventHandler(Button_Paint);

            //Exit button
            Button ExitButton = new Button
            {
                Location = new Point(buttonXYloc.X + 2 * buttonSize.Width + 2 * buttonSepX, buttonXYloc.Y + buttonSize.Height + buttonSepY),
                Size = buttonSize,
                BackColor = Color.FromArgb(192, 192, 192),
                Font = buttonFont,
                FlatStyle = FlatStyle.Flat,
                Text = "Exit"
            };
            ExitButton.FlatAppearance.BorderSize = 0;
            ExitButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(192, 192, 192);
            Controls.Add(ExitButton);
            ExitButton.BringToFront();
            ExitButton.Click += new EventHandler(ExitButton_Click);
            ExitButton.Paint += new PaintEventHandler(Button_Paint);

            //Improvements vertical bar
            ImprovementsBar = new VScrollBar()
            {
                Location = new Point(270, 433),
                Size = new Size(15, 190),
                Maximum = 66 - 9 + 9    //max improvements=66, 9 can be shown, because of slider size it's 9 elements smaller
            };
            WallpaperPanel.Controls.Add(ImprovementsBar);
            ImprovementsBar.ValueChanged += new EventHandler(ImprovementsBarValueChanged);

            //Initialize city drawing
            CityDrawing = Draw.DrawCityFormMap(ThisCity);

            //Define offset map array
            offsets = new int[20, 2] { { -2, 0 }, { -1, -1 }, { 0, -2 }, { 1, -1 }, { 2, 0 }, { 1, 1 }, { 0, 2 }, { -1, 1 }, { -3, -1 }, { -2, -2 }, { -1, -3 }, { 1, -3 }, { 2, -2 }, { 3, -1 }, { 3, 1 }, { 2, 2 }, { 1, 3 }, { -1, 3 }, { -2, 2 }, { -3, 1 } };
    }

        //Once slider value changes --> redraw improvements list
        private void ImprovementsBarValueChanged(object sender, EventArgs e)
        {
            WallpaperPanel.Invalidate();
        }

        private void CityForm_Load(object sender, EventArgs e)
        {
            Location = new Point(CallingForm.Width / 2 - this.Width / 2, CallingForm.Height / 2 - this.Height / 2 + 60);
        }

        private void CityForm_Paint(object sender, PaintEventArgs e)
        {
            StringFormat sf = new StringFormat();
            sf.LineAlignment = StringAlignment.Center;
            sf.Alignment = StringAlignment.Center;
            string bcad;
            if (Game.Data.GameYear < 0) { bcad = "B.C."; }
            else { bcad = "A.D."; }
            string text = String.Format("City of {0}, {1} {2}, Population {3:n0} (Treasury: {4} Gold)", ThisCity.Name, Math.Abs(Game.Data.GameYear), bcad, ThisCity.Population, Game.Civs[ThisCity.Owner].Money);

            e.Graphics.DrawString(text, new Font("Times New Roman", 18), new SolidBrush(Color.Black), new Point(this.Width / 2 + 1, 20 + 1), sf);
            e.Graphics.DrawString(text, new Font("Times New Roman", 18), new SolidBrush(Color.FromArgb(135, 135, 135)), new Point(this.Width / 2, 20), sf);
            sf.Dispose();
        }

        private void WallpaperPanel_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawString("Resource Map", new Font("Arial", 13), new SolidBrush(Color.FromArgb(243, 183, 7)), new Point(90, 280));
            e.Graphics.DrawString("City Resources", new Font("Arial", 13), new SolidBrush(Color.FromArgb(243, 183, 7)), new Point(400, 70));
            e.Graphics.DrawString("City Improvements", new Font("Arial", 13), new SolidBrush(Color.FromArgb(223, 187, 7)), new Point(56, 433));
        }

        //Draw faces
        private void Faces_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawImage(Draw.DrawFaces(ThisCity, 1.5), 0, 0);
        }

        //Draw city map
        private void ResourceMap_Paint(object sender, PaintEventArgs e)
        {
            //map around city
            e.Graphics.DrawImage(ModifyImage.ResizeImage(CityDrawing, (int)((double)CityDrawing.Width * 1.125), (int)((double)CityDrawing.Height * 1.125)), 0, 0);
            //Food/shield/trade icons around the city (21 of them altogether)
            for (int i = 0; i <= ThisCity.Size; i++)
            {
                e.Graphics.DrawImage(Draw.DrawCityFormMapIcons(ThisCity, ThisCity.PriorityOffsets[i, 0], ThisCity.PriorityOffsets[i, 1]), 36 * (ThisCity.PriorityOffsets[i, 0] + 3) + 13, 18 * (ThisCity.PriorityOffsets[i, 1] + 3) + 11);
            }
        }

        //Draw city resources
        private void CityResources_Paint(object sender, PaintEventArgs e)
        {
            StringFormat sf1 = new StringFormat();
            StringFormat sf2 = new StringFormat();
            sf1.Alignment = StringAlignment.Far;
            sf2.Alignment = StringAlignment.Center;

            //Draw food+surplus/hunger strings
            e.Graphics.DrawString("Food: " + ThisCity.Food.ToString(), new Font("Arial", 14), new SolidBrush(Color.Black), new Point(6, 21));
            e.Graphics.DrawString("Food: " + ThisCity.Food.ToString(), new Font("Arial", 14), new SolidBrush(Color.FromArgb(87, 171, 39)), new Point(5, 20));
            e.Graphics.DrawString("Surplus: " + ThisCity.Surplus.ToString(), new Font("Arial", 14), new SolidBrush(Color.Black), new Point(346, 21), sf1);
            e.Graphics.DrawString("Surplus: " + ThisCity.Surplus.ToString(), new Font("Arial", 14), new SolidBrush(Color.FromArgb(63, 139, 31)), new Point(345, 20), sf1);

            //Draw trade+corruption strings
            e.Graphics.DrawString("Trade: " + ThisCity.Trade.ToString(), new Font("Arial", 14), new SolidBrush(Color.Black), new Point(6, 83));
            e.Graphics.DrawString("Trade: " + ThisCity.Trade.ToString(), new Font("Arial", 14), new SolidBrush(Color.FromArgb(239, 159, 7)), new Point(5, 82));
            e.Graphics.DrawString("Corruption: " + ThisCity.Corruption.ToString(), new Font("Arial", 14), new SolidBrush(Color.Black), new Point(346, 83), sf1);
            e.Graphics.DrawString("Corruption: " + ThisCity.Corruption.ToString(), new Font("Arial", 14), new SolidBrush(Color.FromArgb(227, 83, 15)), new Point(345, 82), sf1);

            //Draw tax/lux/sci strings
            e.Graphics.DrawString("50% Tax: " + ThisCity.Tax.ToString(), new Font("Arial", 14), new SolidBrush(Color.Black), new Point(6, 164));
            e.Graphics.DrawString("50% Tax: " + ThisCity.Tax.ToString(), new Font("Arial", 14), new SolidBrush(Color.FromArgb(239, 159, 7)), new Point(5, 163));
            e.Graphics.DrawString("0% Lux: " + ThisCity.Lux.ToString(), new Font("Arial", 14), new SolidBrush(Color.Black), new Point(180, 164), sf2);
            e.Graphics.DrawString("0% Lux: " + ThisCity.Lux.ToString(), new Font("Arial", 14), new SolidBrush(Color.White), new Point(179, 163), sf2);
            e.Graphics.DrawString("50% Sci: " + ThisCity.Sci.ToString(), new Font("Arial", 14), new SolidBrush(Color.Black), new Point(346, 164), sf1);
            e.Graphics.DrawString("50% Sci: " + ThisCity.Sci.ToString(), new Font("Arial", 14), new SolidBrush(Color.FromArgb(63, 187, 199)), new Point(345, 163), sf1);

            //Support + production icons
            e.Graphics.DrawString("Support: " + ThisCity.Support.ToString(), new Font("Arial", 14), new SolidBrush(Color.Black), new Point(6, 224));
            e.Graphics.DrawString("Support: " + ThisCity.Support.ToString(), new Font("Arial", 14), new SolidBrush(Color.FromArgb(63, 79, 167)), new Point(5, 223));
            e.Graphics.DrawString("Production: " + ThisCity.Production.ToString(), new Font("Arial", 14), new SolidBrush(Color.Black), new Point(346, 224), sf1);
            e.Graphics.DrawString("Production: " + ThisCity.Production.ToString(), new Font("Arial", 14), new SolidBrush(Color.FromArgb(7, 11, 103)), new Point(345, 223), sf1);

            //Draw icons
            e.Graphics.DrawImage(Draw.DrawCityIcons(ThisCity, 5, -2, 5, 3, 7, 2, 6, 5, 3), new Point(7, 42));

            sf1.Dispose();
            sf2.Dispose();
        }

        private void UnitsFromCity_Paint(object sender, PaintEventArgs e)
        {
            int count = 0;
            int row = 0;
            int col = 0;
            double resize_factor = 1;  //orignal images are 0.67 of original, because of 50% scaling it is 0.67*1.5=1
            foreach (IUnit unit in Game.Units.Where(n => n.HomeCity == Game.Cities.FindIndex(x => x == ThisCity)))
            {
                col = count % 5;
                row = count / 5;
                e.Graphics.DrawImage(Draw.DrawUnit(unit, false, resize_factor), new Point((int)(64 * resize_factor * col), (int)(48 * resize_factor * row)));
                count++;

                if (count >= 10) { break; }
            }
        }

        private void UnitsInCity_Paint(object sender, PaintEventArgs e)
        {
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;

            int count = 0;
            int row = 0;
            int col = 0;
            double resize_factor = 1.125;  //orignal images are 25% smaller, because of 50% scaling it is 0.75*1.5=1.125
            foreach (IUnit unit in Game.Units.Where(unit => unit.X == ThisCity.X && unit.Y == ThisCity.Y ))
            {
                col = count % 5;
                row = count / 5;
                e.Graphics.DrawImage(Draw.DrawUnit(unit, false, resize_factor), new Point((int)(64 * resize_factor * col), (int)(48 * resize_factor * row) + 5 * row));
                e.Graphics.DrawString(ThisCity.Name.Substring(0, 3), new Font("Arial", 12), new SolidBrush(Color.Black), new Point((int)(64 * resize_factor * col) + (int)(64 * resize_factor / 2), (int)(48 * resize_factor * row) + 5 * row + (int)(48 * resize_factor)), sf);
                count++;
            }
            sf.Dispose();
        }

        private void ImprovementsList_Paint(object sender, PaintEventArgs e)
        {
            //Draw city improvements
            int x = 12;
            int y = 460;
            int starting = ImprovementsBar.Value;   //starting improvement to draw (changes with slider)
            for (int i = 0; i < 9; i++)
            {
                if ((i + starting) >= (ThisCity.Improvements.Count())) { break; }  //break if no of improvements+wonders to small

                //draw improvements
                e.Graphics.DrawImage(Images.ImprovementsSmall[(int)ThisCity.Improvements[i + starting].Type], new Point(x, y + 15 * i + 2 * i));
                if ((int)ThisCity.Improvements[i + starting].Type < 39) //wonders don't have a sell icon
                {
                    e.Graphics.DrawImage(Images.SellIconLarge, new Point(x + 220, y + 15 * i + 2 * i - 2));
                }
                e.Graphics.DrawString(ThisCity.Improvements[i + starting].Name, new Font("Arial", 13), new SolidBrush(Color.Black), new Point(x + 36, y + 15 * i + 2 * i - 3));
                e.Graphics.DrawString(ThisCity.Improvements[i + starting].Name, new Font("Arial", 13), new SolidBrush(Color.White), new Point(x + 35, y + 15 * i + 2 * i - 3));
            }
        }

        private void FoodStorage_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawImage(Draw.DrawFoodStorage(ThisCity), new Point(0, 0));
        }

        private void Production_Paint(object sender, PaintEventArgs e)
        {

        }

        private void ImprovementsPanel_Paint(object sender, PaintEventArgs e)
        {
            //for (int i = 0; i < 7; i++)
            //{
            //    e.Graphics.DrawString("HELLO", new Font("Arial", 13), new SolidBrush(Color.Black), new Point(0, 20 * i));
            //}            
        }

        private void BuyButton_Click(object sender, EventArgs e)
        {
        }

        private void ChangeButton_Click(object sender, EventArgs e)
        {
            //Use this so the form returns a chosen value (what it has chosen to produce)
            using (var CityChangeForm = new CityChangeForm(ThisCity))
            {
                CityChangeForm.Load += new EventHandler(CityChangeForm_Load);   //so you set the correct size of form
                var result = CityChangeForm.ShowDialog();
                if (result == DialogResult.OK)  //when form is closed
                {
                    int val = CityChangeForm.ChosenValue;   //chosen value from other form
                }
            }
        }

        private void CityChangeForm_Load(object sender, EventArgs e)
        {
            Form frm = sender as Form;
            frm.Width = 686;
            frm.Height = 458;
            frm.Location = new Point(200, 100);
        }

        private void InfoButton_Click(object sender, EventArgs e)
        {
        }

        private void MapButton_Click(object sender, EventArgs e)
        {
        }

        private void RenameButton_Click(object sender, EventArgs e)
        {
            CityRenameForm CityRenameForm = new CityRenameForm(ThisCity);
            CityRenameForm.RefreshCityForm += RefreshThis;
            CityRenameForm.ShowDialog();
        }

        void RefreshThis()
        {
            Refresh();
        }

        private void HappyButton_Click(object sender, EventArgs e)
        {
        }

        private void ViewButton_Click(object sender, EventArgs e)
        {
        }

        private void ExitButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Button_Paint(object sender, PaintEventArgs e)
        {
            ControlPaint.DrawBorder(e.Graphics, this.ClientRectangle,
                SystemColors.ControlLightLight, 1, ButtonBorderStyle.Outset,
                SystemColors.ControlLightLight, 1, ButtonBorderStyle.Outset,
                SystemColors.ControlLightLight, 1, ButtonBorderStyle.Outset,
                SystemColors.ControlLightLight, 1, ButtonBorderStyle.Outset);
        }

    }
}
