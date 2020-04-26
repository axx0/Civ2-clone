using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ExtensionMethods;
using RTciv2.Imagery;
using RTciv2.Units;
using RTciv2.Enums;
using RTciv2.Improvements;

namespace RTciv2.Forms
{
    //public partial class CityForm : Form
    public partial class CityForm : Civ2form
    {
        public MainCiv2Window mainCiv2Window;
        City ThisCity;
        DoubleBufferedPanel CallingForm, WallpaperPanel, Faces, ResourceMap, CityResources, UnitsFromCity, UnitsInCity, FoodStorage, ProductionPanel;
        VScrollBar ImprovementsBar;
        int[,] offsets;
        int ProductionItem;

        public CityForm(MainCiv2Window _mainCiv2Window)
        {
            InitializeComponent();
            mainCiv2Window = _mainCiv2Window;
        }

        public CityForm(DoubleBufferedPanel _callingForm, City city)
        {
            InitializeComponent();

            ThisCity = city;

            CallingForm = _callingForm;

            Size = new Size(976, 681);  //normalen zoom = (657,459)
            FormBorderStyle = FormBorderStyle.None;
            BackgroundImage = Images.WallpaperMapForm;
            
            this.Load += CityForm_Load;
            this.Paint += CityForm_Paint;

            #region Panels
            //Panel for wallpaper
            WallpaperPanel = new DoubleBufferedPanel
            {
                Location = new Point(11, 39),    //normal zoom = (8,25)
                Size = new Size(954, 631),      //normal zoom = (640,420)
                BackgroundImage = ModifyImage.ResizeImage(Images.CityWallpaper, 954, 631)
            };
            Controls.Add(WallpaperPanel);
            WallpaperPanel.Paint += WallpaperPanel_Paint;
            WallpaperPanel.Paint += ImprovementsList_Paint;
            WallpaperPanel.MouseClick += WallpaperPanel_Clicked;
            #endregion

            #region Buttons
            //Buy button
            Civ2button BuyButton = new Civ2button
            {
                Location = new Point(651 + 8, 248 + 24),
                Size = new Size(102, 36),
                Font = new Font("Arial", 10.5F),
                Text = "Buy"
            };
            WallpaperPanel.Controls.Add(BuyButton);
            BuyButton.Click += new EventHandler(BuyButton_Click);

            //Change button
            Civ2button ChangeButton = new Civ2button
            {
                Location = new Point(651 + 180, 248 + 24),
                Size = new Size(102, 36),
                Font = new Font("Arial", 10.5F),
                Text = "Change"
            };
            WallpaperPanel.Controls.Add(ChangeButton);
            ChangeButton.Click += new EventHandler(ChangeButton_Click);

            //Info button
            Civ2button InfoButton = new Civ2button
            {
                Location = new Point(684, 546), //original (461, 366)
                Size = new Size(85, 36),  //original (57, 24)
                Font = new Font("Arial", 10.5F),
                Text = "Info"
            };
            WallpaperPanel.Controls.Add(InfoButton);
            InfoButton.Click += new EventHandler(InfoButton_Click);

            //Map button
            Civ2button MapButton = new Civ2button
            {
                Location = new Point(771, 546), //original (519, 366)
                Size = new Size(85, 36),  //original (57, 24)
                Font = new Font("Arial", 10.5F),
                Text = "Map"
            };
            WallpaperPanel.Controls.Add(MapButton);
            MapButton.Click += new EventHandler(MapButton_Click);

            //Rename button
            Civ2button RenameButton = new Civ2button
            {
                Location = new Point(858, 546), //original (577, 366)
                Size = new Size(85, 36),  //original (57, 24)
                Font = new Font("Arial", 10.5F),
                Text = "Rename"
            };
            WallpaperPanel.Controls.Add(RenameButton);
            RenameButton.Click += new EventHandler(RenameButton_Click);

            //Happy button
            Civ2button HappyButton = new Civ2button
            {
                Location = new Point(684, 583), //original (461, 391)
                Size = new Size(85, 36),  //original (57, 24)
                Font = new Font("Arial", 10.5F),
                Text = "Happy"
            };
            WallpaperPanel.Controls.Add(HappyButton);
            HappyButton.Click += new EventHandler(HappyButton_Click);

            //View button
            Civ2button ViewButton = new Civ2button
            {
                Location = new Point(771, 583), //original (519, 391)
                Size = new Size(85, 36),  //original (57, 24)
                Font = new Font("Arial", 10.5F),
                Text = "View"
            };
            WallpaperPanel.Controls.Add(ViewButton);
            ViewButton.Click += new EventHandler(ViewButton_Click);

            //Exit button
            Civ2button ExitButton = new Civ2button
            {
                Location = new Point(858, 583), //original (577, 391)
                Size = new Size(85, 36),  //original (57, 24)
                Font = new Font("Arial", 10.5F),
                Text = "Exit"
            };
            WallpaperPanel.Controls.Add(ExitButton);
            ExitButton.Click += new EventHandler(ExitButton_Click);

            //Next city (UP) button
            NoSelectButton NextCityButton = new NoSelectButton
            {
                Location = new Point(652, 548), //original (440, 367)
                Size = new Size(32, 36),  //original (21, 24)
                BackColor = Color.FromArgb(107, 107, 107)
            };
            NextCityButton.FlatStyle = FlatStyle.Flat;
            WallpaperPanel.Controls.Add(NextCityButton);
            NextCityButton.Click += new EventHandler(NextCityButton_Click);
            NextCityButton.Paint += new PaintEventHandler(NextCityButton_Paint);

            //Previous city (DOWN) button
            NoSelectButton PrevCityButton = new NoSelectButton
            {
                Location = new Point(652, 585), //original (440, 392)
                Size = new Size(32, 36),  //original (21, 24)
                BackColor = Color.FromArgb(107, 107, 107)
            };
            PrevCityButton.FlatStyle = FlatStyle.Flat;
            WallpaperPanel.Controls.Add(PrevCityButton);
            PrevCityButton.Click += new EventHandler(PrevCityButton_Click);
            PrevCityButton.Paint += new PaintEventHandler(PrevCityButton_Paint);
            #endregion

            //Improvements vertical bar
            ImprovementsBar = new VScrollBar()
            {
                Location = new Point(270, 433),
                Size = new Size(15, 190),
                Maximum = 66 - 9 + 9    //max improvements=66, 9 can be shown, because of slider size it's 9 elements smaller
            };
            WallpaperPanel.Controls.Add(ImprovementsBar);
            ImprovementsBar.ValueChanged += new EventHandler(ImprovementsBarValueChanged);

            //Define offset map array
            offsets = new int[20, 2] { { -2, 0 }, { -1, -1 }, { 0, -2 }, { 1, -1 }, { 2, 0 }, { 1, 1 }, { 0, 2 }, { -1, 1 }, { -3, -1 }, { -2, -2 }, { -1, -3 }, { 1, -3 }, { 2, -2 }, { 3, -1 }, { 3, 1 }, { 2, 2 }, { 1, 3 }, { -1, 3 }, { -2, 2 }, { -3, 1 } };

            ProductionItem = 0; //item appearing in production menu on loadgame
        }

        //Once slider value changes --> redraw improvements list
        private void ImprovementsBarValueChanged(object sender, EventArgs e)
        {
            WallpaperPanel.Invalidate();
            Update();
        }

        private void CityForm_Load(object sender, EventArgs e)
        {
            Location = new Point(CallingForm.Width / 2 - this.Width / 2, CallingForm.Height / 2 - this.Height / 2 + 60);
        }

        private void CityForm_Paint(object sender, PaintEventArgs e)
        {
            //Main text
            StringFormat sf = new StringFormat();
            sf.LineAlignment = StringAlignment.Center;
            sf.Alignment = StringAlignment.Center;
            string bcad = (Data.GameYear < 0) ? "B.C.": "A.D.";
            string text = 
                String.Format("City of " + ThisCity.Name + ", " + Math.Abs(Data.GameYear).ToString() + " " + bcad.ToString() +
                ", Population " + ThisCity.Population.ToString("###,###", new NumberFormatInfo() { NumberDecimalSeparator = "," }) +
                " (Treasury: " + Game.Civs[ThisCity.Owner].Money.ToString() + " Gold)");
            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;
            e.Graphics.DrawString(text, new Font("Times New Roman", 15), new SolidBrush(Color.Black), new Point(this.Width / 2 + 1, 20 + 1), sf);
            e.Graphics.DrawString(text, new Font("Times New Roman", 15), new SolidBrush(Color.FromArgb(135, 135, 135)), new Point(this.Width / 2, 20), sf);
            //Border of wallpaper
            e.Graphics.DrawLine(new Pen(Color.FromArgb(67, 67, 67)), 9, 37, this.Width - 11, 37);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(67, 67, 67)), 10, 38, this.Width - 12, 38);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(67, 67, 67)), 9, 37, 9, this.Height - 11);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(67, 67, 67)), 10, 38, 10, this.Height - 12);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(223, 223, 223)), this.Width - 11, 38, this.Width - 11, this.Height - 11);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(223, 223, 223)), this.Width - 10, 37, this.Width - 10, this.Height - 11);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(223, 223, 223)), 9, this.Height - 10, this.Width - 10, this.Height - 10);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(223, 223, 223)), 10, this.Height - 11, this.Width - 11, this.Height - 11);
            sf.Dispose();
            e.Dispose();
        }

        private void WallpaperPanel_Paint(object sender, PaintEventArgs e)
        {
            StringFormat sf1 = new StringFormat();
            StringFormat sf2 = new StringFormat();
            sf1.Alignment = StringAlignment.Far;
            sf2.Alignment = StringAlignment.Center;
            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;

            #region Panel names
            e.Graphics.DrawString("Citizens", new Font("Arial", 11), new SolidBrush(Color.FromArgb(67, 67, 67)), new Point(117, 71));
            e.Graphics.DrawString("Citizens", new Font("Arial", 11), new SolidBrush(Color.FromArgb(223, 187, 63)), new Point(116, 70));
            e.Graphics.DrawString("Resource Map", new Font("Arial", 11), new SolidBrush(Color.FromArgb(0, 51, 0)), new Point(91, 284));
            e.Graphics.DrawString("Resource Map", new Font("Arial", 11), new SolidBrush(Color.FromArgb(223, 187, 63)), new Point(90, 283));
            e.Graphics.DrawString("City Resources", new Font("Arial", 11), new SolidBrush(Color.FromArgb(67, 67, 67)), new Point(412, 71));
            e.Graphics.DrawString("City Resources", new Font("Arial", 11), new SolidBrush(Color.FromArgb(223, 187, 63)), new Point(411, 70));
            e.Graphics.DrawString("City Improvements", new Font("Arial", 11), new SolidBrush(Color.FromArgb(67, 67, 67)), new Point(68, 436));
            e.Graphics.DrawString("City Improvements", new Font("Arial", 11), new SolidBrush(Color.FromArgb(223, 187, 63)), new Point(67, 435));            
            e.Graphics.DrawString("Food Storage", new Font("Arial", 11), new SolidBrush(Color.FromArgb(0, 0, 0)), new Point(743, 1));
            e.Graphics.DrawString("Food Storage", new Font("Arial", 11), new SolidBrush(Color.FromArgb(75, 155, 35)), new Point(742, 0));
            #endregion

            #region Citizens
            e.Graphics.DrawImage(Images.DrawCitizens(ThisCity, 1.5), 7, 13);
            #endregion

            #region Food storage
            e.Graphics.DrawImage(Images.DrawFoodStorage(ThisCity), new Point(651, 0));
            #endregion

            #region Production panel
            //Show item currently in production (ProductionItem=0...61 are units, 62...127 are improvements)
            //Units are scaled by 1.15 compared to original, improvements are size 54x30
            if (ThisCity.ItemInProduction < 62)    //units
            {
                e.Graphics.DrawImage(ModifyImage.ResizeImage(Images.Units[ThisCity.ItemInProduction], 74, 55), new Point(651 + 106, 248 + 7));
            }
            else    //improvements
            {
                e.Graphics.DrawString(ReadFiles.ImprovementName[ThisCity.ItemInProduction - 62 + 1], new Font("Arial", 14), new SolidBrush(Color.Black), 651 + 146 + 1, 248 + 3 + 1, sf2);
                e.Graphics.DrawString(ReadFiles.ImprovementName[ThisCity.ItemInProduction - 62 + 1], new Font("Arial", 14), new SolidBrush(Color.FromArgb(63, 79, 167)), 651 + 146, 248 + 3, sf2);
                e.Graphics.DrawImage(Images.ImprovementsLarge[ThisCity.ItemInProduction - 62 + 1], new Point(651 + 119, 248 + 28));
            }
            e.Graphics.DrawImage(Images.DrawCityProduction(ThisCity), new Point(651, 248));  //draw production shields and sqare around them
            #endregion

            #region Resource map
            Bitmap ResourceMap = Images.DrawCityResourcesMap(ThisCity);
            e.Graphics.DrawImage(ModifyImage.ResizeImage(ResourceMap, (int)(ResourceMap.Width * 1.125), (int)(ResourceMap.Height * 1.125)), 7, 125);
            #endregion

            #region City resources
            //1) FOOD+SURPLUS/HUNGER
            //Text
            int food = ThisCity.Food;
            int surpHung = ThisCity.SurplusHunger;
            e.Graphics.DrawString($"Food: {food}", new Font("Arial", 11), new SolidBrush(Color.Black), new Point(304, 94));
            e.Graphics.DrawString($"Food: {food}", new Font("Arial", 11), new SolidBrush(Color.FromArgb(87, 171, 39)), new Point(303, 93));
            string surpHungSt = (surpHung >= 0) ? "Surplus" : "Hunger";
            e.Graphics.DrawString($"{surpHungSt}: {Math.Abs(surpHung)}", new Font("Arial", 11), new SolidBrush(Color.Black), new Point(644, 94), sf1);
            e.Graphics.DrawString($"{surpHungSt}: {Math.Abs(surpHung)}", new Font("Arial", 11), new SolidBrush(Color.FromArgb(63, 139, 31)), new Point(643, 93), sf1);
            //Spacing between icons
            int spacing;
            switch (food + Math.Abs(surpHung))
            {
                case int n when (n >= 1 && n <= 15): spacing = 23; break;    //50 % larger (orignal = 15, 1 pixel gap)
                case int n when (n == 16 || n == 17): spacing = 20; break;   //50 % larger (orignal = 13, 1 pixel overlap)
                case int n when (n == 18 || n == 19): spacing = 17; break;   //50 % larger (orignal = 11, 3 pixel overlap)
                case int n when (n == 20 || n == 21): spacing = 15; break;   //50 % larger (orignal = 10, 4 pixel overlap)
                case int n when (n == 22 || n == 23): spacing = 14; break;   //50 % larger (orignal = 9, 5 pixel overlap)
                case int n when (n == 24 || n == 25): spacing = 12; break;   //50 % larger (orignal = 8, 6 pixel overlap)
                case int n when (n >= 26 && n <= 29): spacing = 11; break;   //50 % larger (orignal = 7, 7 pixel overlap)
                case int n when (n >= 30 && n <= 33): spacing = 9; break;    //50 % larger (orignal = 6, 8 pixel overlap)
                case int n when (n >= 34 && n <= 37): spacing = 8; break;    //50 % larger (orignal = 5, 9 pixel overlap)
                case int n when (n >= 38 && n <= 49): spacing = 6; break;    //50 % larger (orignal = 4, 10 pixel overlap)
                case int n when (n >= 50 && n <= 65): spacing = 5; break;    //50 % larger (orignal = 3, 11 pixel overlap)
                case int n when (n >= 66): spacing = 3; break;               //50 % larger (orignal = 2, 12 pixel overlap)
                default: spacing = 2; break;
            }
            //First draw background rectangle
            //e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(71, 147, 31)), 0, 0, spacing * foodIcons + 21 - spacing + 6, 23); //background square for food
            //e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(55, 123, 23)), x_size - (spacing * Math.Abs(surplusIcons) + 21 - spacing + 3), 0, spacing * Math.Abs(surplusIcons) + 21 - spacing + 6, 23); //background square for surplus/hunger
            //Draw food & surplus icons
            for (int i = 0; i < food; i++) e.Graphics.DrawImage(Images.CitymapFoodLargeBigger, 310 + i * spacing, 116);
            for (int i = 0; i < Math.Abs(surpHung); i++)
                if (surpHung < 0) e.Graphics.DrawImage(Images.CitymapHungerLargeBigger, 646 - (spacing * Math.Abs(surpHung) + 21 - spacing) + i * spacing, 116); //hunger
                else e.Graphics.DrawImage(Images.CitymapFoodLargeBigger, 646 - (spacing * Math.Abs(surpHung) + 21 - spacing) + i * spacing, 116); //surplus

            //2) TRADE+CORRUPTION
            //Text
            int trade = ThisCity.Trade;
            int corrupt = ThisCity.Corruption;
            e.Graphics.DrawString($"Trade: {trade}", new Font("Arial", 11), new SolidBrush(Color.Black), new Point(304, 155));
            e.Graphics.DrawString($"Trade: {trade}", new Font("Arial", 11), new SolidBrush(Color.FromArgb(239, 159, 7)), new Point(303, 154));
            e.Graphics.DrawString($"Corruption: {corrupt}", new Font("Arial", 11), new SolidBrush(Color.Black), new Point(644, 155), sf1);
            e.Graphics.DrawString($"Corruption: {corrupt}", new Font("Arial", 11), new SolidBrush(Color.FromArgb(227, 83, 15)), new Point(643, 154), sf1);
            //Spacing between icons
            switch (trade + Math.Abs(corrupt))
            {
                case int n when (n >= 1 && n <= 15): spacing = 23; break;    //50 % larger (orignal = 15, 1 pixel gap)
                case int n when (n == 16 || n == 17): spacing = 20; break;   //50 % larger (orignal = 13, 1 pixel overlap)
                case int n when (n == 18 || n == 19): spacing = 17; break;   //50 % larger (orignal = 11, 3 pixel overlap)
                case int n when (n == 20 || n == 21): spacing = 15; break;   //50 % larger (orignal = 10, 4 pixel overlap)
                case int n when (n == 22 || n == 23): spacing = 14; break;   //50 % larger (orignal = 9, 5 pixel overlap)
                case int n when (n == 24 || n == 25): spacing = 12; break;   //50 % larger (orignal = 8, 6 pixel overlap)
                case int n when (n >= 26 && n <= 29): spacing = 11; break;   //50 % larger (orignal = 7, 7 pixel overlap)
                case int n when (n >= 30 && n <= 33): spacing = 9; break;    //50 % larger (orignal = 6, 8 pixel overlap)
                case int n when (n >= 34 && n <= 37): spacing = 8; break;    //50 % larger (orignal = 5, 9 pixel overlap)
                case int n when (n >= 38 && n <= 49): spacing = 6; break;    //50 % larger (orignal = 4, 10 pixel overlap)
                case int n when (n >= 50 && n <= 65): spacing = 5; break;    //50 % larger (orignal = 3, 11 pixel overlap)
                case int n when (n >= 66): spacing = 3; break;               //50 % larger (orignal = 2, 12 pixel overlap)
                default: spacing = 2; break;
            }
            //First draw background rectangle
            //graphics.FillRectangle(new SolidBrush(Color.FromArgb(71, 147, 31)), 0, 0, spacing * foodIcons + 21 - spacing + 6, 23); //background square for food
            //graphics.FillRectangle(new SolidBrush(Color.FromArgb(55, 123, 23)), x_size - (spacing * Math.Abs(surplusIcons) + 21 - spacing + 3), 0, spacing * Math.Abs(surplusIcons) + 21 - spacing + 6, 23); //background square for surplus/hunger
            //Draw trade & corruption icons
            for (int i = 0; i < trade; i++) e.Graphics.DrawImage(Images.CitymapTradeLargeBigger, 310 + i * spacing, 175);
            for (int i = 0; i < Math.Abs(corrupt); i++) e.Graphics.DrawImage(Images.CitymapCorruptionLargeBigger, 640 - (spacing * Math.Abs(corrupt) + 21 - spacing) + i * spacing, 175);

            //3) TAX/SCI/LUX
            //Text
            int tax = ThisCity.Tax;
            int sci = ThisCity.Science;
            int lux = ThisCity.Lux;
            e.Graphics.DrawString($"{Game.Civs[ThisCity.Owner].TaxRate}% Tax: {tax}", new Font("Arial", 11), new SolidBrush(Color.Black), new Point(304, 236));
            e.Graphics.DrawString($"{Game.Civs[ThisCity.Owner].TaxRate}% Tax: {tax}", new Font("Arial", 11), new SolidBrush(Color.FromArgb(239, 159, 7)), new Point(303, 235));
            e.Graphics.DrawString($"{Game.Civs[ThisCity.Owner].LuxRate}% Lux: {lux}", new Font("Arial", 11), new SolidBrush(Color.Black), new Point(474, 236), sf2);
            e.Graphics.DrawString($"{Game.Civs[ThisCity.Owner].LuxRate}% Lux: {lux}", new Font("Arial", 11), new SolidBrush(Color.White), new Point(473, 235), sf2);
            e.Graphics.DrawString($"{Game.Civs[ThisCity.Owner].ScienceRate}% Sci: {sci}", new Font("Arial", 11), new SolidBrush(Color.Black), new Point(644, 236), sf1);
            e.Graphics.DrawString($"{Game.Civs[ThisCity.Owner].ScienceRate}% Sci: {sci}", new Font("Arial", 11), new SolidBrush(Color.FromArgb(63, 187, 199)), new Point(643, 235), sf1);
            //Spacing between icons
            switch (tax + sci + lux)
            {
                case int n when (n >= 1 && n <= 15): spacing = 23; break;    //50 % larger (orignal = 15, 1 pixel gap)
                case int n when (n == 16 || n == 17): spacing = 20; break;   //50 % larger (orignal = 13, 1 pixel overlap)
                case int n when (n == 18 || n == 19): spacing = 17; break;   //50 % larger (orignal = 11, 3 pixel overlap)
                case int n when (n == 20 || n == 21): spacing = 15; break;   //50 % larger (orignal = 10, 4 pixel overlap)
                case int n when (n == 22 || n == 23): spacing = 14; break;   //50 % larger (orignal = 9, 5 pixel overlap)
                case int n when (n == 24 || n == 25): spacing = 12; break;   //50 % larger (orignal = 8, 6 pixel overlap)
                case int n when (n >= 26 && n <= 29): spacing = 11; break;   //50 % larger (orignal = 7, 7 pixel overlap)
                case int n when (n >= 30 && n <= 33): spacing = 9; break;    //50 % larger (orignal = 6, 8 pixel overlap)
                case int n when (n >= 34 && n <= 37): spacing = 8; break;    //50 % larger (orignal = 5, 9 pixel overlap)
                case int n when (n >= 38 && n <= 49): spacing = 6; break;    //50 % larger (orignal = 4, 10 pixel overlap)
                case int n when (n >= 50 && n <= 65): spacing = 5; break;    //50 % larger (orignal = 3, 11 pixel overlap)
                case int n when (n >= 66): spacing = 3; break;               //50 % larger (orignal = 2, 12 pixel overlap)
                default: spacing = 2; break;
            }
            //Draw tax+sci+lux icons
            for (int i = 0; i < tax; i++) e.Graphics.DrawImage(Images.CitymapTaxLargeBigger, 310 + i * spacing, 211);  //tax
            for (int i = 0; i < lux; i++) e.Graphics.DrawImage(Images.CitymapLuxLargeBigger, 310 + 169 - (21 + spacing * (lux - 1)) / 2 + i * spacing, 211);  //lux
            for (int i = 0; i < sci; i++) e.Graphics.DrawImage(Images.CitymapSciLargeBigger, 640 - (21 + spacing * (sci - 1)) + i * spacing, 211);  //science
            //TODO: change location of tax/sci/lux icons & text if one of these is set to 0%

            //4) SUPPORT+WASTE+PRODUCTION/SHORTAGE
            //Text
            int support = ThisCity.Support;
            int waste = ThisCity.Waste;
            int prod = ThisCity.Production;
            e.Graphics.DrawString($"Support: {support}", new Font("Arial", 11), new SolidBrush(Color.Black), new Point(304, 297));
            e.Graphics.DrawString($"Support: {support}", new Font("Arial", 11), new SolidBrush(Color.FromArgb(63, 79, 167)), new Point(303, 296));
            e.Graphics.DrawString($"Waste: {waste}", new Font("Arial", 11), new SolidBrush(Color.FromArgb(159, 159, 159)), new Point(474, 297), sf2);
            e.Graphics.DrawString($"Waste: {waste}", new Font("Arial", 11), new SolidBrush(Color.FromArgb(11, 11, 11)), new Point(473, 296), sf2);
            e.Graphics.DrawString($"Production: {prod}", new Font("Arial", 11), new SolidBrush(Color.Black), new Point(644, 297), sf1);
            e.Graphics.DrawString($"Production: {prod}", new Font("Arial", 11), new SolidBrush(Color.FromArgb(7, 11, 103)), new Point(643, 296), sf1);
            //Spacing between icons
            switch (support + waste + prod)
            {
                case int n when (n >= 1 && n <= 15): spacing = 23; break;    //50 % larger (orignal = 15, 1 pixel gap)
                case int n when (n == 16 || n == 17): spacing = 20; break;   //50 % larger (orignal = 13, 1 pixel overlap)
                case int n when (n == 18 || n == 19): spacing = 17; break;   //50 % larger (orignal = 11, 3 pixel overlap)
                case int n when (n == 20 || n == 21): spacing = 15; break;   //50 % larger (orignal = 10, 4 pixel overlap)
                case int n when (n == 22 || n == 23): spacing = 14; break;   //50 % larger (orignal = 9, 5 pixel overlap)
                case int n when (n == 24 || n == 25): spacing = 12; break;   //50 % larger (orignal = 8, 6 pixel overlap)
                case int n when (n >= 26 && n <= 29): spacing = 11; break;   //50 % larger (orignal = 7, 7 pixel overlap)
                case int n when (n >= 30 && n <= 33): spacing = 9; break;    //50 % larger (orignal = 6, 8 pixel overlap)
                case int n when (n >= 34 && n <= 37): spacing = 8; break;    //50 % larger (orignal = 5, 9 pixel overlap)
                case int n when (n >= 38 && n <= 49): spacing = 6; break;    //50 % larger (orignal = 4, 10 pixel overlap)
                case int n when (n >= 50 && n <= 65): spacing = 5; break;    //50 % larger (orignal = 3, 11 pixel overlap)
                case int n when (n >= 66): spacing = 3; break;               //50 % larger (orignal = 2, 12 pixel overlap)
                default: spacing = 2; break;
            }
            //Draw support+waste+prod icons
            for (int i = 0; i < support; i++) e.Graphics.DrawImage(Images.CitymapSupportLargeBigger, 310 + i * spacing, 272);  //support
            //for (int i = 0; i < waste; i++) e.Graphics.DrawImage(Images.CitymapLuxLargeBigger, 310 + 169 - (21 + spacing * (lux - 1)) / 2 + i * spacing, 211);  //waste
            for (int i = 0; i < prod; i++) e.Graphics.DrawImage(Images.CitymapSupportLargeBigger, 640 - (21 + spacing * (prod - 1)) + i * spacing, 272);  //production
            #endregion

            #region Units present
            //Draw units present in the city
            List<IUnit> unitsInThisCity = ThisCity.UnitsInCity;
            int[] offset = new int[2] { 0, 0 };
            for (int i = 0; i < Math.Min(unitsInThisCity.Count(), 18); i++) //more than 18 units are not shown
            {
                //First determine offsets of units drawn
                if (unitsInThisCity.Count() <= 5) offset = new int[2] { 1 + 72 * i, 32 };
                else
                {
                    if (i < 5) offset = new int[2] { 1 + 72 * i, 3 };
                    else if (i < 10) offset = new int[2] { 1 + 72 * (i - 5), 61 };
                    else if (i < 14) offset = new int[2] { 37 + 72 * (i - 10), 32 };
                    else offset = new int[2] { 37 + 72 * (i - 14), 90 };
                }
                //Then draw units
                e.Graphics.DrawImage(Images.CreateUnitBitmap(unitsInThisCity[i], false, 9), new Point(291 + offset[0], 320 + offset[1]));
                //Then draw the names of cities (names only for first 10 units)
                if (i < 10)
                {
                    string name = (ThisCity.Name.Length < 4) ? ThisCity.Name : ThisCity.Name.Substring(0, 3);   //get first 3 characters of city name
                    e.Graphics.DrawString(name, new Font("Arial", 12, FontStyle.Bold), new SolidBrush(Color.FromArgb(135, 135, 135)), 291 + offset[0] + 37, 320 + offset[1] + 56, sf2);
                    e.Graphics.DrawString(name, new Font("Arial", 12, FontStyle.Bold), new SolidBrush(Color.Black), 291 + offset[0] + 36, 320 + offset[1] + 55, sf2);
                }
            }
            //Name of panel (shown only if no of units in city < 6)
            if (unitsInThisCity.Count() <= 5)
            {
                e.Graphics.DrawString("Units Present", new Font("Arial", 11), new SolidBrush(Color.FromArgb(67, 67, 67)), new Point(414, 327));
                e.Graphics.DrawString("Units Present", new Font("Arial", 11), new SolidBrush(Color.FromArgb(223, 187, 63)), new Point(413, 326));
            }
            #endregion

            #region Units supported
            //Draw units supported by the city
            List<IUnit> unitsSupportedByThisCity = ThisCity.SupportedUnits;   //all units supported by this city
            bool[] unitsHaveCosts = ThisCity.SupportedUnitsWhichCostShields();  //which of supported units require upkeep
            offset = new int[2] { 0, 0 };
            for (int i = 0; i < Math.Min(unitsSupportedByThisCity.Count(), 8); i++) //more than 8 units are not shown
            {
                //First determine offsets of units drawn
                if (unitsSupportedByThisCity.Count() <= 4) offset = new int[2] { 4 + (64 + 5) * i, 34 };
                else
                {
                    if (i < 4) offset = new int[2] { 4 + (64 + 5) * i, 8 };
                    else offset = new int[2] { 4 + (64 + 5) * (i - 4), 60 };
                }
                //Then draw units
                e.Graphics.DrawImage(Images.CreateUnitBitmap(unitsSupportedByThisCity[i], false, 8), new Point(6 + offset[0], 320 + offset[1]));
                //If unit is settler/engineer => draw food icon
                if (unitsSupportedByThisCity[i].Type == UnitType.Settlers || unitsSupportedByThisCity[i].Type == UnitType.Engineers)
                    e.Graphics.DrawImage(Images.CitymapFoodSmallBigger, new Point(7 + offset[0], 351 + offset[1]));
                //If unit requires upkeep => draw shield
                if (unitsHaveCosts[i])
                {
                    if (unitsSupportedByThisCity[i].Type == UnitType.Settlers || unitsSupportedByThisCity[i].Type == UnitType.Engineers)
                        e.Graphics.DrawImage(Images.CitymapShieldSmallBigger, new Point(25 + offset[0], 351 + offset[1]));
                    else
                        e.Graphics.DrawImage(Images.CitymapShieldSmallBigger, new Point(7 + offset[0], 351 + offset[1]));
                }
            }
            //Name of panel (shown only if no of units in city < 5)
            if (unitsSupportedByThisCity.Count() <= 4)
            {
                e.Graphics.DrawString("Units Supported", new Font("Arial", 11), new SolidBrush(Color.FromArgb(67, 67, 67)), new Point(80, 327));
                e.Graphics.DrawString("Units Supported", new Font("Arial", 11), new SolidBrush(Color.FromArgb(223, 187, 63)), new Point(79, 326));
            }
            #endregion

            #region Commodities
            e.Graphics.DrawString($"Supplies: {ThisCity.CommoditySupplied[0]}, {ThisCity.CommoditySupplied[1]}, {ThisCity.CommoditySupplied[2]}", new Font("Arial", 13, FontStyle.Bold), new SolidBrush(Color.FromArgb(67, 67, 67)), new Point(295, 523));
            e.Graphics.DrawString($"Supplies: {ThisCity.CommoditySupplied[0]}, {ThisCity.CommoditySupplied[1]}, {ThisCity.CommoditySupplied[2]}", new Font("Arial", 13, FontStyle.Bold), new SolidBrush(Color.FromArgb(227, 83, 15)), new Point(294, 522));
            e.Graphics.DrawString($"Demands: {ThisCity.CommodityDemanded[0]}, {ThisCity.CommodityDemanded[1]}, {ThisCity.CommodityDemanded[2]}", new Font("Arial", 13, FontStyle.Bold), new SolidBrush(Color.FromArgb(67, 67, 67)), new Point(295, 543));
            e.Graphics.DrawString($"Demands: {ThisCity.CommodityDemanded[0]}, {ThisCity.CommodityDemanded[1]}, {ThisCity.CommodityDemanded[2]}", new Font("Arial", 13, FontStyle.Bold), new SolidBrush(Color.FromArgb(227, 83, 15)), new Point(294, 542));
            #endregion
            sf1.Dispose(); 
            sf2.Dispose();
            e.Dispose();
        }

        private void WallpaperPanel_Clicked(object sender, MouseEventArgs e)
        {
            int[,] offsets = new int[21, 2] { { 0, 0 }, { -1, -3 }, { -3, -1 }, { -3, 1 }, { -1, 3 }, { 1, 3 }, { 3, 1 }, { 3, -1 }, { 1, -3 }, { -2, -2 }, { -2, 2 }, { 2, 2 },
                                              { 2, -2 }, { 0, -2 }, { -1, -1 }, { -2, 0 }, { -1, 1 }, { 0, 2 }, { 1, 1 }, { 2, 0 }, { 1, -1 } };    //offset of squares from city square (0,0)
            int locX = e.Location.X - 7;    //reduce by panel offset
            int locY = e.Location.Y - 125;
            if ((locX >= 0) && (locX <= (int)(ResourceMap.Width * 1.125)) && (locY >= 0) && (locY <= (int)(ResourceMap.Height * 1.125)))    //clicked within resource map area
            {
                int[] coords = Ext.PxToCoords(locX, locY, (int)(8 * 1.125));
                coords[0] -= 3; //center naj je (0,0)
                coords[1] -= 3;
                for (int i = 0; i < 21; i++)
                    if (coords[0] == offsets[i, 0] && coords[1] == offsets[i, 1])
                    {
                        if (ThisCity.DistributionWorkers[i] == 1 && !(coords[0] == 0 & coords[1] == 0))   //you clicked on a tile with a worker, but not the city
                        {
                            ThisCity.DistributionWorkers[i] = 0;
                        }
                        else if (ThisCity.DistributionWorkers[i] == 0 && ThisCity.DistributionWorkers.Sum() < (ThisCity.Size + 1))    //there are available workers to be placed elsewhere
                        {
                            ThisCity.DistributionWorkers[i] = 1;
                        }
                        else
                        {
                            //TODO: optimize worker positions in city view if clicked on empty squares or city
                        }
                        Refresh();
                    }
            }
        }

        private void ImprovementsList_Paint(object sender, PaintEventArgs e)
        {
            //Draw city improvements
            int x = 12;
            int y = 460;
            int starting = ImprovementsBar.Value;   //starting improvement to draw (changes with slider)
            for (int i = 0; i < 9; i++)
            {
                if ((i + starting) >= (ThisCity.Improvements.Count()))
                    break;  //break if no of improvements+wonders to small

                //draw improvements
                e.Graphics.DrawImage(Images.ImprovementsSmall[(int)ThisCity.Improvements[i + starting].Type], new Point(x, y + 15 * i + 2 * i));
                if ((int)ThisCity.Improvements[i + starting].Type < 39) //wonders don't have a sell icon
                {
                    e.Graphics.DrawImage(Images.SellIconLarge, new Point(x + 220, y + 15 * i + 2 * i - 2));
                }
                e.Graphics.DrawString(ThisCity.Improvements[i + starting].Name, new Font("Arial", 13), new SolidBrush(Color.Black), new Point(x + 36, y + 15 * i + 2 * i - 3));
                e.Graphics.DrawString(ThisCity.Improvements[i + starting].Name, new Font("Arial", 13), new SolidBrush(Color.White), new Point(x + 35, y + 15 * i + 2 * i - 3));
            }
            e.Dispose();
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
                    if (ThisCity.ItemInProduction < 62) 
                        cost = ReadFiles.UnitCost[ThisCity.ItemInProduction];
                    else 
                        cost = ReadFiles.ImprovementCost[ThisCity.ItemInProduction - 62 + 1];
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
            e.Dispose();
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
            e.Dispose();
        }

        private void ExitButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
