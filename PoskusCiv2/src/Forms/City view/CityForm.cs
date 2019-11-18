using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RTciv2.Imagery;
using RTciv2.Units;
using RTciv2.Improvements;

namespace RTciv2.Forms
{
    //public partial class CityForm : Form
    public partial class CityForm : Civ2form
    {
        public MainCiv2Window mainCiv2Window;
        //Draw Draw = new Draw();
        City ThisCity;
        Bitmap CityDrawing;
        DoubleBufferedPanel WallpaperPanel, Faces, ResourceMap, CityResources, UnitsFromCity, UnitsInCity, FoodStorage, ProductionPanel;
        Form CallingForm;
        VScrollBar ImprovementsBar;
        int[,] offsets;
        int ProductionItem;

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

            Size = new Size(986, 689);  //normalen zoom = (657,459)
            FormBorderStyle = FormBorderStyle.None;
            BackgroundImage = Images.WallpaperMapForm;
            
            this.Load += new EventHandler(CityForm_Load);
            this.Paint += new PaintEventHandler(CityForm_Paint);

            //Panel for wallpaper
            WallpaperPanel = new DoubleBufferedPanel
            {
                Location = new Point(12, 37),    //normal zoom = (8,25)
                Size = new Size(960, 630),      //normal zoom = (640,420)
                BackgroundImage = ModifyImage.ResizeImage(Images.CityWallpaper, 960, 630),
            };
            Controls.Add(WallpaperPanel);
            WallpaperPanel.Paint += new PaintEventHandler(WallpaperPanel_Paint);
            WallpaperPanel.Paint += new PaintEventHandler(ImprovementsList_Paint);

            //Faces panel
            Faces = new DoubleBufferedPanel
            {
                Location = new Point(10, 10),
                Size = new Size(630, 50),    //stretched by 12.5 %
                BackColor = Color.Transparent
            };
            WallpaperPanel.Controls.Add(Faces);
            Faces.Paint += new PaintEventHandler(Faces_Paint);

            //Resource map panel
            ResourceMap = new DoubleBufferedPanel
            {
                Location = new Point(7, 125),
                Size = new Size(4 * 72, 4 * 36),    //stretched by 12.5 %
                BackColor = Color.Transparent
            };
            WallpaperPanel.Controls.Add(ResourceMap);
            ResourceMap.Paint += new PaintEventHandler(ResourceMap_Paint);

            //City resources panel
            CityResources = new DoubleBufferedPanel
            {
                Location = new Point(300, 70),
                Size = new Size(350, 245),    //stretched by 12.5 %
                BackColor = Color.Transparent
            };
            WallpaperPanel.Controls.Add(CityResources);
            CityResources.Paint += new PaintEventHandler(CityResources_Paint);

            //Units from city panel
            UnitsFromCity = new DoubleBufferedPanel
            {
                Location = new Point(10, 321),
                Size = new Size(270, 104),
                BackColor = Color.Transparent
            };
            WallpaperPanel.Controls.Add(UnitsFromCity);
            UnitsFromCity.Paint += new PaintEventHandler(UnitsFromCity_Paint);

            //Units in city panel
            UnitsInCity = new DoubleBufferedPanel
            {
                Location = new Point(288, 322),
                Size = new Size(360, 245),
                BackColor = Color.Transparent
            };
            WallpaperPanel.Controls.Add(UnitsInCity);
            UnitsInCity.Paint += new PaintEventHandler(UnitsInCity_Paint);

            //Food storage panel
            FoodStorage = new DoubleBufferedPanel
            {
                Location = new Point(653, 0),
                Size = new Size(291, 244),
                BackColor = Color.Transparent
            };
            WallpaperPanel.Controls.Add(FoodStorage);
            FoodStorage.Paint += new PaintEventHandler(FoodStorage_Paint);

            //Production panel
            ProductionPanel = new DoubleBufferedPanel
            {
                Location = new Point(657, 249),
                Size = new Size(293, 287),
                BackColor = Color.Transparent
            };
            WallpaperPanel.Controls.Add(ProductionPanel);
            ProductionPanel.Paint += new PaintEventHandler(ProductionPanel_Paint);

            //Buy button
            Civ2button BuyButton = new Civ2button
            {
                Location = new Point(8, 24),
                Size = new Size(102, 36),
                Font = new Font("Arial", 13),
                Text = "Buy"
            };
            ProductionPanel.Controls.Add(BuyButton);
            BuyButton.Click += new EventHandler(BuyButton_Click);

            //Change button
            Civ2button ChangeButton = new Civ2button
            {
                Location = new Point(180, 24),
                Size = new Size(102, 36),
                Font = new Font("Arial", 13),
                Text = "Change"
            };
            ProductionPanel.Controls.Add(ChangeButton);
            ChangeButton.Click += new EventHandler(ChangeButton_Click);

            //Info button
            Civ2button InfoButton = new Civ2button
            {
                Location = new Point(692, 549), //original (461, 366)
                Size = new Size(86, 36),  //original (57, 24)
                Font = new Font("Arial", 13),
                Text = "Info"
            };
            WallpaperPanel.Controls.Add(InfoButton);
            InfoButton.Click += new EventHandler(InfoButton_Click);

            //Map button
            Civ2button MapButton = new Civ2button
            {
                Location = new Point(779, 549), //original (519, 366)
                Size = new Size(86, 36),  //original (57, 24)
                Font = new Font("Arial", 13),
                Text = "Map"
            };
            WallpaperPanel.Controls.Add(MapButton);
            MapButton.Click += new EventHandler(MapButton_Click);

            //Rename button
            Civ2button RenameButton = new Civ2button
            {
                Location = new Point(866, 549), //original (577, 366)
                Size = new Size(86, 36),  //original (57, 24)
                Font = new Font("Arial", 13),
                Text = "Rename"
            };
            WallpaperPanel.Controls.Add(RenameButton);
            RenameButton.Click += new EventHandler(RenameButton_Click);

            //Happy button
            Civ2button HappyButton = new Civ2button
            {
                Location = new Point(692, 587), //original (461, 391)
                Size = new Size(86, 36),  //original (57, 24)
                Font = new Font("Arial", 13),
                Text = "Happy"
            };
            WallpaperPanel.Controls.Add(HappyButton);
            HappyButton.Click += new EventHandler(HappyButton_Click);

            //View button
            Civ2button ViewButton = new Civ2button
            {
                Location = new Point(779, 587), //original (519, 391)
                Size = new Size(86, 36),  //original (57, 24)
                Font = new Font("Arial", 13),
                Text = "View"
            };
            WallpaperPanel.Controls.Add(ViewButton);
            ViewButton.Click += new EventHandler(ViewButton_Click);

            //Exit button
            Civ2button ExitButton = new Civ2button
            {
                Location = new Point(866, 587), //original (577, 391)
                Size = new Size(86, 36),  //original (57, 24)
                Font = new Font("Arial", 13),
                Text = "Exit"
            };
            WallpaperPanel.Controls.Add(ExitButton);
            ExitButton.Click += new EventHandler(ExitButton_Click);

            //Next city (UP) button
            Button NextCityButton = new Button
            {
                Location = new Point(660, 550), //original (440, 367)
                Size = new Size(32, 36),  //original (21, 24)
                BackColor = Color.FromArgb(107, 107, 107)
            };
            NextCityButton.FlatStyle = FlatStyle.Flat;
            WallpaperPanel.Controls.Add(NextCityButton);
            NextCityButton.Click += new EventHandler(NextCityButton_Click);
            NextCityButton.Paint += new PaintEventHandler(NextCityButton_Paint);

            //Previous city (DOWN) button
            Button PrevCityButton = new Button
            {
                Location = new Point(660, 588), //original (440, 392)
                Size = new Size(32, 36),  //original (21, 24)
                BackColor = Color.FromArgb(107, 107, 107)
            };
            PrevCityButton.FlatStyle = FlatStyle.Flat;
            WallpaperPanel.Controls.Add(PrevCityButton);
            PrevCityButton.Click += new EventHandler(PrevCityButton_Click);
            PrevCityButton.Paint += new PaintEventHandler(PrevCityButton_Paint);

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
            //CityDrawing = Draw.DrawCityFormMap(ThisCity);

            //Define offset map array
            offsets = new int[20, 2] { { -2, 0 }, { -1, -1 }, { 0, -2 }, { 1, -1 }, { 2, 0 }, { 1, 1 }, { 0, 2 }, { -1, 1 }, { -3, -1 }, { -2, -2 }, { -1, -3 }, { 1, -3 }, { 2, -2 }, { 3, -1 }, { 3, 1 }, { 2, 2 }, { 1, 3 }, { -1, 3 }, { -2, 2 }, { -3, 1 } };

            ProductionItem = 0; //item appearing in production menu on loadgame
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
            //Borders of panel
            e.Graphics.DrawLine(new Pen(Color.FromArgb(223, 223, 223)), 0, WallpaperPanel.Height - 2, WallpaperPanel.Width, WallpaperPanel.Height - 2);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(223, 223, 223)), 0, WallpaperPanel.Height - 1, WallpaperPanel.Width, WallpaperPanel.Height - 1);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(223, 223, 223)), WallpaperPanel.Width - 2, 0, WallpaperPanel.Width - 2, WallpaperPanel.Height);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(223, 223, 223)), WallpaperPanel.Width - 1, 0, WallpaperPanel.Width - 1, WallpaperPanel.Height);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(67, 67, 67)), 0, 0, WallpaperPanel.Width - 2, 0);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(67, 67, 67)), 0, 1, WallpaperPanel.Width - 3, 1);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(67, 67, 67)), 0, 0, 0, WallpaperPanel.Height - 2);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(67, 67, 67)), 1, 0, 1, WallpaperPanel.Height - 3);

            //Texts
            e.Graphics.DrawString("Resource Map", new Font("Arial", 13), new SolidBrush(Color.FromArgb(243, 183, 7)), new Point(90, 280));
            e.Graphics.DrawString("City Resources", new Font("Arial", 13), new SolidBrush(Color.FromArgb(243, 183, 7)), new Point(400, 70));
            e.Graphics.DrawString("City Improvements", new Font("Arial", 13), new SolidBrush(Color.FromArgb(223, 187, 7)), new Point(56, 433));
        }

        //Draw faces
        private void Faces_Paint(object sender, PaintEventArgs e)
        {
           //e.Graphics.DrawImage(Draw.DrawFaces(ThisCity, 1.5), 0, 0);
        }

        //Draw city map
        private void ResourceMap_Paint(object sender, PaintEventArgs e)
        {
            //map around city
            e.Graphics.DrawImage(ModifyImage.ResizeImage(CityDrawing, (int)((double)CityDrawing.Width * 1.125), (int)((double)CityDrawing.Height * 1.125)), 0, 0);
            //Food/shield/trade icons around the city (21 of them altogether)
            for (int i = 0; i <= ThisCity.Size; i++)
            {
                //e.Graphics.DrawImage(Draw.DrawCityFormMapIcons(ThisCity, ThisCity.PriorityOffsets[i, 0], ThisCity.PriorityOffsets[i, 1]), 36 * (ThisCity.PriorityOffsets[i, 0] + 3) + 13, 18 * (ThisCity.PriorityOffsets[i, 1] + 3) + 11);
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
            e.Graphics.DrawString("50% Sci: " + ThisCity.Science.ToString(), new Font("Arial", 14), new SolidBrush(Color.Black), new Point(346, 164), sf1);
            e.Graphics.DrawString("50% Sci: " + ThisCity.Science.ToString(), new Font("Arial", 14), new SolidBrush(Color.FromArgb(63, 187, 199)), new Point(345, 163), sf1);

            //Support + production icons
            e.Graphics.DrawString("Support: " + ThisCity.Support.ToString(), new Font("Arial", 14), new SolidBrush(Color.Black), new Point(6, 224));
            e.Graphics.DrawString("Support: " + ThisCity.Support.ToString(), new Font("Arial", 14), new SolidBrush(Color.FromArgb(63, 79, 167)), new Point(5, 223));
            e.Graphics.DrawString("Production: " + ThisCity.Production.ToString(), new Font("Arial", 14), new SolidBrush(Color.Black), new Point(346, 224), sf1);
            e.Graphics.DrawString("Production: " + ThisCity.Production.ToString(), new Font("Arial", 14), new SolidBrush(Color.FromArgb(7, 11, 103)), new Point(345, 223), sf1);

            //Draw icons
            //e.Graphics.DrawImage(Draw.DrawCityIcons(ThisCity, 5, -2, 5, 3, 7, 2, 6, 5, 3), new Point(7, 42));

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
                //e.Graphics.DrawImage(Draw.DrawUnit(unit, false, resize_factor), new Point((int)(64 * resize_factor * col), (int)(48 * resize_factor * row)));
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
                //e.Graphics.DrawImage(Draw.DrawUnit(unit, false, resize_factor), new Point((int)(64 * resize_factor * col), (int)(48 * resize_factor * row) + 5 * row));
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
            //e.Graphics.DrawImage(Draw.DrawFoodStorage(ThisCity), new Point(0, 0));
        }

        private void ProductionPanel_Paint(object sender, PaintEventArgs e)
        {
            //Show item currently in production (ProductionItem=0...61 are units, 62...127 are improvements)
            //Units are scaled by 1.15 compared to original, improvements are size 54x30
            if (ThisCity.ItemInProduction < 62)    //units
            {
                e.Graphics.DrawImage(ModifyImage.ResizeImage(Images.Units[ThisCity.ItemInProduction], 74, 55), new Point(106, 7));
            }
            else    //improvements
            {
                StringFormat sf = new StringFormat();
                sf.Alignment = StringAlignment.Center;
                e.Graphics.DrawString(ReadFiles.ImprovementName[ThisCity.ItemInProduction - 62 + 1], new Font("Arial", 14), new SolidBrush(Color.Black), 146 + 1, 3 + 1, sf);
                e.Graphics.DrawString(ReadFiles.ImprovementName[ThisCity.ItemInProduction - 62 + 1], new Font("Arial", 14), new SolidBrush(Color.FromArgb(63, 79, 167)), 146, 3, sf);
                e.Graphics.DrawImage(Images.ImprovementsLarge[ThisCity.ItemInProduction - 62 + 1], new Point(119, 28));
                sf.Dispose();
            }

            //e.Graphics.DrawImage(Draw.DrawCityProduction(ThisCity), new Point(0, 0));  //draw production shields and sqare around them
        }

        private void ImprovementsPanel_Paint(object sender, PaintEventArgs e)
        {          
        }

        private void BuyButton_Click(object sender, EventArgs e)
        {
            //Use this so the form returns a chosen value (what it has chosen to produce)
            using (var CityBuyForm = new CityBuyForm(ThisCity))
            {
                CityBuyForm.Load += new EventHandler(CityBuyForm_Load);   //so you set the correct size of form
                var result = CityBuyForm.ShowDialog();
                if (result == DialogResult.OK)  //buying item activated
                {
                    int cost = 0;
                    if (ThisCity.ItemInProduction < 62) cost = ReadFiles.UnitCost[ThisCity.ItemInProduction];
                    else cost = ReadFiles.ImprovementCost[ThisCity.ItemInProduction - 62 + 1];
                    Game.Civs[1].Money -= 10 * cost - ThisCity.ShieldsProgress;
                    ThisCity.ShieldsProgress = 10 * cost;
                    ProductionPanel.Refresh();
                }
            }
        }

        private void CityBuyForm_Load(object sender, EventArgs e)
        {
            Form frm = sender as Form;
            frm.Location = new Point(250, 300);
            frm.Width = 758;
            frm.Height = 212;
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
                    ProductionPanel.Refresh();
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

        private void HappyButton_Click(object sender, EventArgs e) { }

        private void ViewButton_Click(object sender, EventArgs e) { }

        private void NextCityButton_Click(object sender, EventArgs e) { }

        private void PrevCityButton_Click(object sender, EventArgs e) { }

        private void NextCityButton_Paint(object sender, PaintEventArgs e)
        {
            //Draw lines in button
            e.Graphics.DrawLine(new Pen(Color.FromArgb(175, 175, 175)), 1, 1, 30, 1);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(175, 175, 175)), 1, 2, 29, 2);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(175, 175, 175)), 1, 1, 1, 33);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(175, 175, 175)), 2, 1, 2, 32);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(43, 43, 43)), 1, 34, 30, 34);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(43, 43, 43)), 2, 33, 30, 33);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(43, 43, 43)), 29, 2, 29, 33);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(43, 43, 43)), 30, 1, 30, 33);
            //Draw the arrow icon
            e.Graphics.DrawImage(Images.NextCityLarge, 2, 1);
        }
                
        private void PrevCityButton_Paint(object sender, PaintEventArgs e)
        {
            //Draw lines in button
            e.Graphics.DrawLine(new Pen(Color.FromArgb(175, 175, 175)), 1, 1, 30, 1);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(175, 175, 175)), 1, 2, 29, 2);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(175, 175, 175)), 1, 1, 1, 33);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(175, 175, 175)), 2, 1, 2, 32);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(43, 43, 43)), 1, 34, 30, 34);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(43, 43, 43)), 2, 33, 30, 33);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(43, 43, 43)), 29, 2, 29, 33);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(43, 43, 43)), 30, 1, 30, 33);
            //Draw the arrow icon
            e.Graphics.DrawImage(Images.PrevCityLarge, 2, 1);
        }

        private void ExitButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
