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
using PoskusCiv2.Improvements;

namespace PoskusCiv2.Forms
{
    public partial class CityForm : Form
    {
        public MainCiv2Window mainCiv2Window;
        Draw Draw = new Draw();
        Bitmap CityDrawing;
        Panel CityPanel;
        Form CallingForm;
        Label ok;
        VScrollBar ImprovementsBar;

        public CityForm(MainCiv2Window _mainCiv2Window)
        {
            InitializeComponent();
            mainCiv2Window = _mainCiv2Window;
        }

        public CityForm(Form _callingForm, City ThisCity)
        {
            InitializeComponent();

            CallingForm = _callingForm;

            Size = new Size(976, 680);  //normalen zoom = (650,453)
            FormBorderStyle = FormBorderStyle.None;
            BackgroundImage = Images.WallpaperMapForm;
            
            //CenterToParent();  //the parent form is not MapForm, so this is not really centered
            this.Load += new EventHandler(CityForm_Load);
            this.Paint += new PaintEventHandler((sender, e) => CityForm_Paint(this, e, ThisCity));
            
            //Sizes & locations of 6 buttons in bottom right corner
            Size buttonSize = new Size(84, 35); //size
            Point buttonXYloc = new Point(695, 580);    //location of 1st button
            int buttonSepX = 5; //separation between buttons in X
            int buttonSepY = 5; //separation between buttons in Y
            Font buttonFont = new Font("Arial", 13);

            //Panel for wallpaper
            CityPanel = new Panel
            {
                Location = new Point(12, 37),    //normal zoom = (8,25)
                Size = new Size(960, 630),      //normal zoom = (640,420)
                BackgroundImage = Images.CityWallpaper,
                BackgroundImageLayout = ImageLayout.Stretch,
                BorderStyle = BorderStyle.Fixed3D
            };
            Controls.Add(CityPanel);
            CityPanel.Paint += new PaintEventHandler((sender, e) => CityPanel_Paint(this, e));
            CityPanel.Paint += new PaintEventHandler((sender, e) => PaintImprovementsList(this, e, ThisCity));

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
            RenameButton.Click += new EventHandler((sender, e) => RenameButton_Click(this, e, ThisCity));
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

            //TESTING ...
            ImprovementsBar = new VScrollBar()
            {
                Location = new Point(270, 433),
                Size = new Size(15, 190),
                Maximum = 66 - 9 + 9    //max improvements=66, 9 can be shown, because of slider size it's 9 elements smaller
            };
            CityPanel.Controls.Add(ImprovementsBar);
            ImprovementsBar.ValueChanged += new EventHandler(ImprovementsBarValueChanged);

            CityDrawing = Draw.DrawCityFormMap(ThisCity);
        }

        //Once slider value changes --> redraw improvements list
        private void ImprovementsBarValueChanged(object sender, EventArgs e)
        {
            CityPanel.Invalidate();
        }

        private void CityForm_Load(object sender, EventArgs e)
        {
            Location = new Point(CallingForm.Width / 2 - this.Width / 2, CallingForm.Height / 2 - this.Height / 2 + 60);
        }

        private void CityForm_Paint(object sender, PaintEventArgs e, City ThisCity)
        {
            StringFormat sf = new StringFormat();
            sf.LineAlignment = StringAlignment.Center;
            sf.Alignment = StringAlignment.Center;
            string bcad;
            if (Game.Data.GameYear < 0) { bcad = "B.C."; }
            else { bcad = "A.D."; }
            string text = String.Format("City of {0}, {1} {2}, Population {3:n0} (Treasury: 250 Gold)", ThisCity.Name, Math.Abs(Game.Data.GameYear), bcad, ThisCity.Population);

            e.Graphics.DrawString(text, new Font("Times New Roman", 18), new SolidBrush(Color.Black), new Point(this.Width / 2 + 1, 20 + 1), sf);
            e.Graphics.DrawString(text, new Font("Times New Roman", 18), new SolidBrush(Color.FromArgb(135, 135, 135)), new Point(this.Width / 2, 20), sf);
            sf.Dispose();
        }

        private void CityPanel_Paint(object sender, PaintEventArgs e)
        {
            //The city image is stretched by 12,5%
            int x = 8, y = 125;
            e.Graphics.DrawImage(ModifyImage.ResizeImage(CityDrawing, (int)((double)CityDrawing.Width * 1.125), (int)((double)CityDrawing.Height * 1.125)), new Point(x, y));
            e.Graphics.DrawString("Resource Map", new Font("Arial", 13), new SolidBrush(Color.FromArgb(243, 183, 7)), new Point(100, 280));

            e.Graphics.DrawString("City Improvements", new Font("Arial", 13), new SolidBrush(Color.FromArgb(223, 187, 7)), new Point(56, 433));
        }

        private void PaintImprovementsList(object sender, PaintEventArgs e, City ThisCity)
        {
            Console.WriteLine("Count improements={0}, count wonders={1}", ThisCity.Improvements.Count(), ThisCity.Wonders.Count());

            //Draw city improvements
            int x = 12;
            int y = 460;
            int starting = ImprovementsBar.Value;   //starting improvement to draw (changes with slider)
            for (int i = 0; i < 9; i++)
            {
                if ((i + starting) >= (ThisCity.Improvements.Count() + ThisCity.Wonders.Count())) { break; }  //break if no of improvements+wonders to small

                //first draw improvements
                if (i + starting < ThisCity.Improvements.Count())
                {
                    e.Graphics.DrawImage(Images.ImprovementsSmall[(int)ThisCity.Improvements[i + starting].Type], new Point(x, y + 15 * i + 2 * i));
                    e.Graphics.DrawImage(Images.SellIconLarge, new Point(x + 220, y + 15 * i + 2 * i - 2));
                    e.Graphics.DrawString(ThisCity.Improvements[i + starting].Name, new Font("Arial", 13), new SolidBrush(Color.Black), new Point(x + 36, y + 15 * i + 2 * i - 3));
                    e.Graphics.DrawString(ThisCity.Improvements[i + starting].Name, new Font("Arial", 13), new SolidBrush(Color.White), new Point(x + 35, y + 15 * i + 2 * i - 3));
                }
                //if all improvements are drawn, start drawing wonders (w/o sell icon)
                else
                {
                    int j = i - ThisCity.Improvements.Count();
                    e.Graphics.DrawImage(Images.WondersSmall[(int)ThisCity.Wonders[j + starting].WType], new Point(x, y + 15 * i + 2 * i));
                    e.Graphics.DrawString(ThisCity.Wonders[j + starting].Name, new Font("Arial", 13), new SolidBrush(Color.Black), new Point(x + 36, y + 15 * i + 2 * i - 3));
                    e.Graphics.DrawString(ThisCity.Wonders[j + starting].Name, new Font("Arial", 13), new SolidBrush(Color.White), new Point(x + 35, y + 15 * i + 2 * i - 3));
                }
            }
        }

        private void ImprovementsPanel_Paint(object sender, PaintEventArgs e)
        {
            //for (int i = 0; i < 7; i++)
            //{
            //    e.Graphics.DrawString("HELLO", new Font("Arial", 13), new SolidBrush(Color.Black), new Point(0, 20 * i));
            //}            
        }

        private void InfoButton_Click(object sender, EventArgs e)
        {
        }

        private void MapButton_Click(object sender, EventArgs e)
        {
        }

        private void RenameButton_Click(object sender, EventArgs e, City ThisCity)
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
