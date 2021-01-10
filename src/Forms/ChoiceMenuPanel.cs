using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using civ2.Enums;
using civ2.Bitmaps;

namespace civ2.Forms
{
    public partial class ChoiceMenuPanel : Civ2panel
    {
        private readonly Civ2radioBtn[] _radioBtn;
        private readonly Main _mainForm;

        public ChoiceMenuPanel(Main mainForm) :
            base((int)(mainForm.ClientSize.Width * 0.174), (int)(mainForm.ClientSize.Height * 0.34), "Civilization II Multiplayer Gold", 38, 46)
        {
            _mainForm = mainForm;

            // Radio buttons
            _radioBtn = new Civ2radioBtn[8];
            string[] txt = { "Start a New Game", "Start on Premade World", "Customize World", "Begin Scenario", "Load a Game", "Multiplayer Game", "View Hall of Fame", "View Credits" };
            for (int i = 0; i < 7; i++)
            {
                _radioBtn[i] = new Civ2radioBtn
                {
                    Text = txt[i],
                    Location = new Point(10, (int)(DrawPanel.Height / 7) * i),
                };
                DrawPanel.Controls.Add(_radioBtn[i]);
            }
            _radioBtn[0].Checked = true;

            // OK button
            var OKButton = new Civ2button
            {
                Location = new Point(9, Height - 42),
                Size = new Size(156, 36),
                Text = "OK"
            };
            Controls.Add(OKButton);
            OKButton.Click += OKButton_Click;

            // Cancel button
            var CancelButton = new Civ2button
            {
                Location = new Point(168, Height - 42),
                Size = new Size(157, 36),
                Text = "Cancel"
            };
            Controls.Add(CancelButton);
            CancelButton.Click += CancelButton_Click;
        }

        private void OKButton_Click(object sender, EventArgs e)
        {
            ChoseResult();
        }

        public void ChoseResult()
        {
            // Load game
            if (_radioBtn[4].Checked)
            {
                var ofd = new OpenFileDialog
                {
                    InitialDirectory = Settings.Civ2Path,
                    Title = "Select Game To Load",
                    Filter = "Save Files (*.sav)|*.SAV"
                };

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    // Get SAV name & directory name from result
                    string directoryPath = Path.GetDirectoryName(ofd.FileName);
                    string SAVname = Path.GetFileName(ofd.FileName);
                    _mainForm.ChoiceMenuResult(IntroScreenChoiceType.LoadGame, directoryPath, SAVname);
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
