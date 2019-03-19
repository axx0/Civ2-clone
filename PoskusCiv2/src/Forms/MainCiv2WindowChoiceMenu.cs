using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using PoskusCiv2.Imagery;

namespace PoskusCiv2.Forms
{
    public partial class MainCiv2WindowChoiceMenu : Civ2form
    {
        Panel MainPanel;
        RadioButton NewGameButton, PremadeWorldButton, CustomizeWorldButton, BeginScenButton, LoadGameButton, MultiplayerButton, HOFButton, CreditsButton;
        MainCiv2Window mainForm;

        public MainCiv2WindowChoiceMenu(MainCiv2Window _mainCiv2Window)
        {
            InitializeComponent();
            mainForm = _mainCiv2Window;
            Size = new Size((int)(_mainCiv2Window.ClientSize.Width * 0.174), (int)(_mainCiv2Window.ClientSize.Height * 0.34));
            Paint += new PaintEventHandler(MainCiv2WindowChoiceMenu_Paint);

            //Stats panel
            MainPanel = new Panel
            {
                Location = new Point((int)(this.Width * 0.02694), (int)(this.Height * 0.10465)),
                Size = new Size(this.Width - 19, (int)(this.Height * 0.7674)),
                BackgroundImage = Images.WallpaperStatusForm
            };
            Controls.Add(MainPanel);
            MainPanel.Paint += MainPanel_Paint;

            //Radio button 1
            NewGameButton = new RadioButton
            {
                Text = "Start a New Game",
                Location = new Point(10, (int)(MainPanel.Height / 7) * 0),
                BackColor = Color.Transparent,
                Font = new Font("Times New Roman", 15.0f),
                ForeColor = Color.FromArgb(51, 51, 51),
                AutoSize = true
            };
            MainPanel.Controls.Add(NewGameButton);

            //Radio button 2
            PremadeWorldButton = new RadioButton
            {
                Text = "Start on Premade World",
                Location = new Point(10, (int)(MainPanel.Height / 7) * 1),
                BackColor = Color.Transparent,
                Font = new Font("Times New Roman", 15.0f),
                ForeColor = Color.FromArgb(51, 51, 51),
                AutoSize = true
            };
            MainPanel.Controls.Add(PremadeWorldButton);

            //Radio button 3
            CustomizeWorldButton = new RadioButton
            {
                Text = "Customize World",
                Location = new Point(10, (int)(MainPanel.Height / 7) * 2),
                BackColor = Color.Transparent,
                Font = new Font("Times New Roman", 15.0f),
                ForeColor = Color.FromArgb(51, 51, 51),
                AutoSize = true
            };
            MainPanel.Controls.Add(CustomizeWorldButton);

            //Radio button 4
            BeginScenButton = new RadioButton
            {
                Text = "Begin Scenario",
                Location = new Point(10, (int)(MainPanel.Height / 7) * 3),
                BackColor = Color.Transparent,
                Font = new Font("Times New Roman", 15.0f),
                ForeColor = Color.FromArgb(51, 51, 51),
                AutoSize = true
            };
            MainPanel.Controls.Add(BeginScenButton);

            //Radio button 5
            LoadGameButton = new RadioButton
            {
                Text = "Load a Game",
                Location = new Point(10, (int)(MainPanel.Height / 7) * 4),
                BackColor = Color.Transparent,
                Font = new Font("Times New Roman", 15.0f),
                ForeColor = Color.FromArgb(51, 51, 51),
                AutoSize = true
            };
            MainPanel.Controls.Add(LoadGameButton);

            //Radio button 6
            MultiplayerButton = new RadioButton
            {
                Text = "Multiplayer Game",
                Location = new Point(10, (int)(MainPanel.Height / 7) * 5),
                BackColor = Color.Transparent,
                Font = new Font("Times New Roman", 15.0f),
                ForeColor = Color.FromArgb(51, 51, 51),
                AutoSize = true
            };
            MainPanel.Controls.Add(MultiplayerButton);

            //Radio button 7
            HOFButton = new RadioButton
            {
                Text = "View Hall of Fame",
                Location = new Point(10, (int)(MainPanel.Height / 7) * 6),
                BackColor = Color.Transparent,
                Font = new Font("Times New Roman", 15.0f),
                ForeColor = Color.FromArgb(51, 51, 51),
                AutoSize = true
            };
            MainPanel.Controls.Add(HOFButton);

            //Radio button 8
            CreditsButton = new RadioButton
            {
                Text = "View Credits",
                Location = new Point(10, (int)(MainPanel.Height / 7) * 7),
                BackColor = Color.Transparent,
                Font = new Font("Times New Roman", 15.0f),
                ForeColor = Color.FromArgb(51, 51, 51),
                AutoSize = true
            };
            MainPanel.Controls.Add(CreditsButton);

            //OK button
            Civ2button OKButton = new Civ2button
            {
                Location = new Point((int)(this.Width * 0.02694), (int)(this.Height * 0.8779)),
                Size = new Size((int)(this.Width * 0.4671), (int)(this.Width * 0.10465)),
                Font = new Font("Times New Roman", 11),
                Text = "OK"
            };
            Controls.Add(OKButton);
            OKButton.Click += new EventHandler(OKButton_Click);

            //Cancel button
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

        private void MainCiv2WindowChoiceMenu_Load(object sender, EventArgs e) { }

        private void OKButton_Click(object sender, EventArgs e)
        {
            //Load game
            if (LoadGameButton.Checked)
            {
                OpenFileDialog ofd = new OpenFileDialog
                {
                    InitialDirectory = "C:\\DOS\\CIV 2\\Civ2\\",
                    Title = "Select Game To Load",
                    Filter = "Save Files (*.sav)|*.SAV"
                };

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    mainForm.ChoiceMenuResult(2, ofd.FileName);
                    //DialogResult = DialogResult.OK;
                    Close();
                }
            }
        }

        private void CancelButton_Click(object sender, EventArgs e) { Application.Exit(); }

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

        private void MainCiv2WindowChoiceMenu_Paint(object sender, PaintEventArgs e)
        {
            StringFormat sf = new StringFormat();
            sf.LineAlignment = StringAlignment.Center;
            sf.Alignment = StringAlignment.Center;
            string text = "Civilization II Multiplayer Gold";
            e.Graphics.DrawString(text, new Font("Times New Roman", 14), new SolidBrush(Color.Black), new Point(this.Width / 2 + 1, 16 + 1), sf);
            e.Graphics.DrawString(text, new Font("Times New Roman", 14), new SolidBrush(Color.FromArgb(135, 135, 135)), new Point(this.Width / 2, 16), sf);
            sf.Dispose();
        }
    }
}
