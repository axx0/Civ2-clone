using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using civ2.Bitmaps;

namespace civ2.Forms
{
    public partial class NameCityPanel : DoubleBufferedPanel
    {
        MapPanel mapForm;
        Panel MainPanel;

        //public NameCityPanel(MapForm _mapForm)
        public NameCityPanel(MapPanel _mapForm)
        {
            InitializeComponent();
            mapForm = _mapForm;
            Size = new Size(686, 126);
            Location = new Point(mapForm.Width / 2 - this.Width / 2, mapForm.Height / 2 - this.Height / 2);
            BackgroundImage = Images.WallpaperMapForm;
            Paint += new PaintEventHandler(NameCityPanel_Paint);

            MainPanel = new Panel
            {
                Location = new Point(9, 36),
                Size = new Size(668, 46),
                BackgroundImage = Images.WallpaperStatusForm
            };
            Controls.Add(MainPanel);
            MainPanel.Paint += MainPanel_Paint;

            //OK button
            Civ2button OKButton = new Civ2button
            {
                Location = new Point(9, 84),
                Size = new Size(333, 36),
                Font = new Font("Times New Roman", 11),
                Text = "OK"
            };
            Controls.Add(OKButton);
            OKButton.Click += new EventHandler(OKButton_Click);

            //Cancel button
            Civ2button CancelButton = new Civ2button
            {
                Location = new Point(344, 84),
                Size = new Size(333, 36),
                Font = new Font("Times New Roman", 11),
                Text = "Cancel"
            };
            Controls.Add(CancelButton);
            CancelButton.Click += new EventHandler(CancelButton_Click);
        }

        public NameCityPanel(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }

        private void OKButton_Click(object sender, EventArgs e)
        {
            //mapForm.CityNamePanelResult(true, "Hello");
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            //mapForm.CityNamePanelResult(false, "");
        }

        private void MainPanel_Paint(object sender, PaintEventArgs e)
        {
            //Draw line borders
            e.Graphics.DrawLine(new Pen(Color.FromArgb(67, 67, 67)), 0, 0, MainPanel.Width - 2, 0);   //1st layer of border
            e.Graphics.DrawLine(new Pen(Color.FromArgb(67, 67, 67)), 0, 0, 0, MainPanel.Height - 2);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(223, 223, 223)), MainPanel.Width - 1, 0, MainPanel.Width - 1, MainPanel.Height - 1);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(223, 223, 223)), 0, MainPanel.Height - 1, MainPanel.Width - 1, MainPanel.Height - 1);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(67, 67, 67)), 1, 1, MainPanel.Width - 3, 1);   //2nd layer of border
            e.Graphics.DrawLine(new Pen(Color.FromArgb(67, 67, 67)), 1, 1, 1, MainPanel.Height - 3);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(223, 223, 223)), MainPanel.Width - 2, 1, MainPanel.Width - 2, MainPanel.Height - 2);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(223, 223, 223)), 1, MainPanel.Height - 2, MainPanel.Width - 2, MainPanel.Height - 2);
        }

        private void NameCityPanel_Paint(object sender, PaintEventArgs e)
        {
            //Draw border around panel
            e.Graphics.DrawLine(new Pen(Color.FromArgb(227, 227, 227)), 0, 0, this.Width - 2, 0);   //1st layer of border
            e.Graphics.DrawLine(new Pen(Color.FromArgb(227, 227, 227)), 0, 0, 0, this.Height - 2);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(105, 105, 105)), this.Width - 1, 0, this.Width - 1, this.Height - 1);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(105, 105, 105)), 0, this.Height - 1, this.Width - 1, this.Height - 1);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(255, 255, 255)), 1, 1, this.Width - 3, 1);   //2nd layer of border
            e.Graphics.DrawLine(new Pen(Color.FromArgb(255, 255, 255)), 1, 1, 1, this.Height - 3);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(160, 160, 160)), this.Width - 2, 1, this.Width - 2, this.Height - 2);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(160, 160, 160)), 1, this.Height - 2, this.Width - 2, this.Height - 2);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(240, 240, 240)), 2, 2, this.Width - 4, 2);   //3rd layer of border
            e.Graphics.DrawLine(new Pen(Color.FromArgb(240, 240, 240)), 2, 2, 2, this.Height - 4);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(240, 240, 240)), this.Width - 3, 2, this.Width - 3, this.Height - 3);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(240, 240, 240)), 2, this.Height - 3, this.Width - 3, this.Height - 3);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(223, 223, 223)), 3, 3, this.Width - 5, 3);   //4th layer of border
            e.Graphics.DrawLine(new Pen(Color.FromArgb(223, 223, 223)), 3, 3, 3, this.Height - 5);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(67, 67, 67)), this.Width - 4, 3, this.Width - 4, this.Height - 4);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(67, 67, 67)), 3, this.Height - 4, this.Width - 4, this.Height - 4);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(223, 223, 223)), 4, 4, this.Width - 6, 4);   //5th layer of border
            e.Graphics.DrawLine(new Pen(Color.FromArgb(223, 223, 223)), 4, 4, 4, this.Height - 6);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(67, 67, 67)), this.Width - 5, 4, this.Width - 5, this.Height - 5);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(67, 67, 67)), 4, this.Height - 5, this.Width - 5, this.Height - 5);

            //Title
            StringFormat sf = new StringFormat
            {
                LineAlignment = StringAlignment.Center,
                Alignment = StringAlignment.Center
            };
            string text = "What Shall We Name This City?";
            e.Graphics.DrawString(text, new Font("Times New Roman", 16), new SolidBrush(Color.Black), new Point(this.Width / 2 + 1, 20 + 1), sf);
            e.Graphics.DrawString(text, new Font("Times New Roman", 16), new SolidBrush(Color.FromArgb(135, 135, 135)), new Point(this.Width / 2, 20), sf);
            sf.Dispose();
        }
    }
}
