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
        readonly RadioButton[] RadioBtn = new RadioButton[8];
        readonly Main mainForm;

        public ChoiceMenuPanel(Main _MainWindow) : 
            base((int)(_MainWindow.ClientSize.Width * 0.174), (int)(_MainWindow.ClientSize.Height * 0.34), "Civilization II Multiplayer Gold", true)
        {
            InitializeComponent();
            mainForm = _MainWindow;

            // Radio buttons
            string[] txt = { "Start a New Game", "Start on Premade World", "Customize World", "Begin Scenario", "Load a Game", "Multiplayer Game", "View Hall of Fame", "View Credits" };
            for (int i = 0; i < 7; i++)
            {
                RadioBtn[i] = new RadioButton
                {
                    Text = txt[i],
                    Location = new Point(10, (int)(DrawPanel.Height / 7) * i),
                    BackColor = Color.Transparent,
                    Font = new Font("Times New Roman", 18),
                    ForeColor = Color.FromArgb(51, 51, 51),
                    AutoSize = true
                };
                DrawPanel.Controls.Add(RadioBtn[i]);
            }
            RadioBtn[0].Checked = true;

            // OK button
            Civ2button OKButton = new Civ2button
            {
                Location = new Point(9, Height - 42),
                Size = new Size(156, 36),
                Font = new Font("Times New Roman", 11),
                Text = "OK"
            };
            Controls.Add(OKButton);
            OKButton.Click += new EventHandler(OKButton_Click);

            // Cancel button
            Civ2button CancelButton = new Civ2button
            {
                Location = new Point(168, Height - 42),
                Size = new Size(157, 36),
                Font = new Font("Times New Roman", 11),
                Text = "Cancel"
            };
            Controls.Add(CancelButton);
            CancelButton.Click += new EventHandler(CancelButton_Click);
        }

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

    }
}
