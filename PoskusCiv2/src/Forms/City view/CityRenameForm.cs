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

namespace RTciv2.Forms
{
    public partial class CityRenameForm : Form
    {
        TextBox RenameTextBox;
        public event Action RefreshCityForm;

        public CityRenameForm(City ThisCity)
        {
            InitializeComponent();

            //Size = new Size(480, 120);
            FormBorderStyle = FormBorderStyle.None;
            BackgroundImage = Images.WallpaperMapForm;
            CenterToParent();
            Paint += CityRenameForm_Paint;

            //Panel in the middle
            Panel CityPanel = new Panel
            {
                Location = new Point(5, 23),
                Size = new Size(455, 30),
                BackgroundImage = Images.WallpaperStatusForm,
                BorderStyle = BorderStyle.Fixed3D
            };
            Controls.Add(CityPanel);

            Label NewCityLabel = new Label
            {
                Location = new Point(5, 30),
                Text = "New City Name:",
                Font = new Font("Arial", 7.5f),
                ForeColor = Color.Black
            };
            Controls.Add(NewCityLabel);
            var pos1 = this.PointToScreen(NewCityLabel.Location);            //making the label transparent in panel
            pos1 = CityPanel.PointToClient(pos1);
            NewCityLabel.Parent = CityPanel;
            NewCityLabel.Location = pos1;
            NewCityLabel.BackColor = Color.Transparent;

            //Textbox for renaming city
            RenameTextBox = new TextBox
            {
                Location = new Point(130, 25),
                Size = new Size(200, 25),
                Text = ThisCity.Name,
                Font = new Font("Times New Roman", 11)
            };
            Controls.Add(RenameTextBox);
            var pos2 = this.PointToScreen(RenameTextBox.Location);            //making the label transparent in panel
            pos2 = CityPanel.PointToClient(pos2);
            RenameTextBox.Parent = CityPanel;
            RenameTextBox.Location = pos2;
            RenameTextBox.BackColor = Color.White;
            RenameTextBox.KeyPress += new KeyPressEventHandler((sender, e) => Keypressed(this, e, ThisCity));

            //Sizes & locations of 2 buttons in bottom
            Size buttonSize = new Size(225, 23); //size
            Point buttonXYloc = new Point(5, 55);    //location of 1st button
            Font buttonFont = new Font("Times New Roman", 9.0f);

            //OK button
            Button OKButton = new Button
            {
                Location = new Point(buttonXYloc.X, buttonXYloc.Y),
                Size = buttonSize,
                BackColor = Color.FromArgb(192, 192, 192),
                Font = buttonFont,
                FlatStyle = FlatStyle.Flat,
                Text = "OK"
            };
            OKButton.FlatAppearance.BorderSize = 0;
            OKButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(192, 192, 192);
            OKButton.BringToFront();
            Controls.Add(OKButton);
            OKButton.Click += new EventHandler((sender, e) => OKButton_Click(this, e, ThisCity));
            OKButton.Paint += new PaintEventHandler(Button_Paint);

            //Cancel button
            Button CancelButton = new Button
            {
                Location = new Point(buttonXYloc.X + buttonSize.Width + 5, buttonXYloc.Y),
                Size = buttonSize,
                BackColor = Color.FromArgb(192, 192, 192),
                Font = buttonFont,
                FlatStyle = FlatStyle.Flat,
                Text = "Cancel"
            };
            CancelButton.FlatAppearance.BorderSize = 0;
            CancelButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(192, 192, 192);
            CancelButton.BringToFront();
            Controls.Add(CancelButton);
            CancelButton.Click += new EventHandler(CancelButton_Click);
            CancelButton.Paint += new PaintEventHandler(Button_Paint);
        }

        private void CityRenameForm_Load(object sender, EventArgs e)
        {
        }

        private void CityRenameForm_Paint(object sender, PaintEventArgs e)
        {
            StringFormat sf = new StringFormat();
            sf.LineAlignment = StringAlignment.Center;
            sf.Alignment = StringAlignment.Center;
            e.Graphics.DrawString("What Shall We Rename This City?", new Font("Times New Roman", 12), new SolidBrush(Color.Black), new Point(this.Width / 2 + 1, 13 + 1), sf);
            e.Graphics.DrawString("What Shall We Rename This City?", new Font("Times New Roman", 12), new SolidBrush(Color.FromArgb(135, 135, 135)), new Point(this.Width / 2, 13), sf);
            sf.Dispose();
        }

        private void OKButton_Click(object sender, EventArgs e, City ThisCity)
        {
            RenameCity(ThisCity);
        }
        
        private void Keypressed(object sender, KeyPressEventArgs e, City ThisCity)
        {
            if (e.KeyChar == (char)Keys.Enter) { RenameCity(ThisCity); };   //if enter is pressed
        }

        private void RenameCity(City ThisCity)
        {
            string NewCityName = RenameTextBox.Text;
            Game.Cities.Find(city => city.X == ThisCity.X && city.Y == ThisCity.Y).Name = NewCityName;
            RefreshCityForm();
            this.Close();
        }

        private void CancelButton_Click(object sender, EventArgs e)
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
