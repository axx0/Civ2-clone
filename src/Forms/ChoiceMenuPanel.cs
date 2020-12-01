using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using civ2.Bitmaps;
using civ2.Enums;

namespace civ2.Forms
{
    public partial class ChoiceMenuPanel : Civ2panel
    {
        Panel MainPanel;
        RadioButton[] RadioBtn = new RadioButton[8];
        MainWindow mainForm;

        public ChoiceMenuPanel(MainWindow _MainWindow) : base((int)(_MainWindow.ClientSize.Width * 0.174), (int)(_MainWindow.ClientSize.Height * 0.34))
        {
            InitializeComponent();
            mainForm = _MainWindow;
            BackgroundImage = Images.PanelOuterWallpaper;
            //Size = new Size((int)(_MainWindow.ClientSize.Width * 0.174), (int)(_MainWindow.ClientSize.Height * 0.34));
            Paint += new PaintEventHandler(ChoiceMenu_Paint);

            // Stats panel
            MainPanel = new Panel
            {
                Location = new Point((int)(this.Width * 0.02694), (int)(this.Height * 0.10465)),
                Size = new Size(this.Width - 19, (int)(this.Height * 0.7674)),
                BackgroundImage = Images.WallpaperStatusForm
            };
            Controls.Add(MainPanel);
            MainPanel.Paint += MainPanel_Paint;

            // Radio buttons
            string[] txt = { "Start a New Game", "Start on Premade World", "Customize World", "Begin Scenario", "Load a Game", "Multiplayer Game", "View Hall of Fame", "View Credits" };
            for (int i = 0; i < 7; i++)
            {
                RadioBtn[i] = new RadioButton
                {
                    Text = txt[i],
                    Location = new Point(10, (int)(MainPanel.Height / 7) * i),
                    BackColor = Color.Transparent,
                    Font = new Font("Times New Roman", 18),
                    ForeColor = Color.FromArgb(51, 51, 51),
                    AutoSize = true
                };
                MainPanel.Controls.Add(RadioBtn[i]);
            }
            RadioBtn[0].Checked = true;

            // OK button
            Civ2button OKButton = new Civ2button
            {
                Location = new Point((int)(this.Width * 0.02694), (int)(this.Height * 0.8779)),
                Size = new Size((int)(this.Width * 0.4671), (int)(this.Width * 0.10465)),
                Font = new Font("Times New Roman", 11),
                Text = "OK"
            };
            Controls.Add(OKButton);
            OKButton.Click += new EventHandler(OKButton_Click);

            // Cancel button
            Civ2button CancelButton = new Civ2button
            {
                Location = new Point((int)(this.Width * 0.50299), (int)(this.Height * 0.8779)),
                Size = new Size((int)(this.Width * 0.4671), (int)(this.Width * 0.10465)),
                Font = new Font("Times New Roman", 11),
                Text = "Cancel"
            };
            Controls.Add(CancelButton);
            CancelButton.Click += new EventHandler(CancelButton_Click);
        }

        //public ChoiceMenuPanel(IContainer container)
        //{
        //    container.Add(this);

        //    InitializeComponent();
        //}

        private void OKButton_Click(object sender, EventArgs e) 
        {
            ChoseResult();
        }

        public void ChoseResult()
        {
            // Load game
            if (RadioBtn[4].Checked)
            {
                OpenFileDialog ofd = new OpenFileDialog
                {
                    InitialDirectory = Program.Path,
                    Title = "Select Game To Load",
                    Filter = "Save Files (*.sav)|*.SAV"
                };

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    // Get SAV name & directory name from result
                    string directoryPath = Path.GetDirectoryName(ofd.FileName);
                    string SAVname = Path.GetFileName(ofd.FileName);
                    mainForm.ChoiceMenuResult(IntroScreenChoiceType.LoadGame, directoryPath, SAVname);
                }
            }
        }

        private void CancelButton_Click(object sender, EventArgs e) 
        { 
            Application.Exit(); 
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Enter:
                    //ChoseResult();
                    break;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void MainPanel_Paint(object sender, PaintEventArgs e)
        {
            // Draw line borders
            e.Graphics.DrawLine(new Pen(Color.FromArgb(67, 67, 67)), 0, 0, MainPanel.Width - 2, 0);   //1st layer of border
            e.Graphics.DrawLine(new Pen(Color.FromArgb(67, 67, 67)), 0, 0, 0, MainPanel.Height - 2);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(223, 223, 223)), MainPanel.Width - 1, 0, MainPanel.Width - 1, MainPanel.Height - 1);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(223, 223, 223)), 0, MainPanel.Height - 1, MainPanel.Width - 1, MainPanel.Height - 1);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(67, 67, 67)), 1, 1, MainPanel.Width - 3, 1);   //2nd layer of border
            e.Graphics.DrawLine(new Pen(Color.FromArgb(67, 67, 67)), 1, 1, 1, MainPanel.Height - 3);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(223, 223, 223)), MainPanel.Width - 2, 1, MainPanel.Width - 2, MainPanel.Height - 2);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(223, 223, 223)), 1, MainPanel.Height - 2, MainPanel.Width - 2, MainPanel.Height - 2);
            e.Dispose();
        }

        private void ChoiceMenu_Paint(object sender, PaintEventArgs e)
        {
            //// Draw border around panel
            //e.Graphics.DrawLine(new Pen(Color.FromArgb(227, 227, 227)), 0, 0, this.Width - 2, 0);   //1st layer of border
            //e.Graphics.DrawLine(new Pen(Color.FromArgb(227, 227, 227)), 0, 0, 0, this.Height - 2);
            //e.Graphics.DrawLine(new Pen(Color.FromArgb(105, 105, 105)), this.Width - 1, 0, this.Width - 1, this.Height - 1);
            //e.Graphics.DrawLine(new Pen(Color.FromArgb(105, 105, 105)), 0, this.Height - 1, this.Width - 1, this.Height - 1);
            //e.Graphics.DrawLine(new Pen(Color.FromArgb(255, 255, 255)), 1, 1, this.Width - 3, 1);   //2nd layer of border
            //e.Graphics.DrawLine(new Pen(Color.FromArgb(255, 255, 255)), 1, 1, 1, this.Height - 3);
            //e.Graphics.DrawLine(new Pen(Color.FromArgb(160, 160, 160)), this.Width - 2, 1, this.Width - 2, this.Height - 2);
            //e.Graphics.DrawLine(new Pen(Color.FromArgb(160, 160, 160)), 1, this.Height - 2, this.Width - 2, this.Height - 2);
            //e.Graphics.DrawLine(new Pen(Color.FromArgb(240, 240, 240)), 2, 2, this.Width - 4, 2);   //3rd layer of border
            //e.Graphics.DrawLine(new Pen(Color.FromArgb(240, 240, 240)), 2, 2, 2, this.Height - 4);
            //e.Graphics.DrawLine(new Pen(Color.FromArgb(240, 240, 240)), this.Width - 3, 2, this.Width - 3, this.Height - 3);
            //e.Graphics.DrawLine(new Pen(Color.FromArgb(240, 240, 240)), 2, this.Height - 3, this.Width - 3, this.Height - 3);
            //e.Graphics.DrawLine(new Pen(Color.FromArgb(223, 223, 223)), 3, 3, this.Width - 5, 3);   //4th layer of border
            //e.Graphics.DrawLine(new Pen(Color.FromArgb(223, 223, 223)), 3, 3, 3, this.Height - 5);
            //e.Graphics.DrawLine(new Pen(Color.FromArgb(67, 67, 67)), this.Width - 4, 3, this.Width - 4, this.Height - 4);
            //e.Graphics.DrawLine(new Pen(Color.FromArgb(67, 67, 67)), 3, this.Height - 4, this.Width - 4, this.Height - 4);
            //e.Graphics.DrawLine(new Pen(Color.FromArgb(223, 223, 223)), 4, 4, this.Width - 6, 4);   //5th layer of border
            //e.Graphics.DrawLine(new Pen(Color.FromArgb(223, 223, 223)), 4, 4, 4, this.Height - 6);
            //e.Graphics.DrawLine(new Pen(Color.FromArgb(67, 67, 67)), this.Width - 5, 4, this.Width - 5, this.Height - 5);
            //e.Graphics.DrawLine(new Pen(Color.FromArgb(67, 67, 67)), 4, this.Height - 5, this.Width - 5, this.Height - 5);

            // Title
            StringFormat sf = new StringFormat
            {
                LineAlignment = StringAlignment.Center,
                Alignment = StringAlignment.Center
            };
            string text = "Civilization II Multiplayer Gold";
            e.Graphics.DrawString(text, new Font("Times New Roman", 17), new SolidBrush(Color.Black), new Point(this.Width / 2 + 1, 20 + 1), sf);
            e.Graphics.DrawString(text, new Font("Times New Roman", 17), new SolidBrush(Color.FromArgb(135, 135, 135)), new Point(this.Width / 2, 20), sf);
            sf.Dispose();
            e.Dispose();
        }
    }
}
