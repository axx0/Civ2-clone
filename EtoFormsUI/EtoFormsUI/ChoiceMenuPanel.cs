using System;
using Eto.Forms;
using Eto.Drawing;
using Civ2engine;
using System.IO;

namespace EtoFormsUI
{
    public partial class ChoiceMenuPanel : RadioButtonPanel
    {
        private Main main;

        public ChoiceMenuPanel(Main mainForm) :
            base((int)(mainForm.Screen.WorkingArea.Width * 0.174), (int)(mainForm.Screen.WorkingArea.Height * 0.34), "Civilization II Multiplayer Gold", new string[] { "Start a New Game", "Start on Premade World", "Customize World", "Begin Scenario", "Load a Game", "Multiplayer Game", "View Hall of Fame", "View Credits" }, new string[] { "OK", "Cancel" })
        {
            main = mainForm;

            Button[0].Click += OK_Clicked;
            Button[1].Click += Cancel_Clicked;
        }

        private void OK_Clicked(object sender, EventArgs e)
        {
            // Load game
            if (RadioBtn[4].Checked)
            {
                using var ofd = new OpenFileDialog
                {
                    Directory = new Uri(Settings.Civ2Path),
                    Title = "Select Game To Load",
                    Filters = { new FileFilter("Save Files (*.sav)", ".SAV") }
                };

                if (ofd.ShowDialog(this.ParentWindow) == DialogResult.Ok)
                {
                    // Get SAV name & directory name from result
                    string directoryPath = ofd.Directory.LocalPath;
                    string SAVname = Path.GetFileName(ofd.FileName);
                    main.LoadGameInitialization(directoryPath, SAVname);
                    main.Sounds.Stop();
                }
            }
        }

        private void Cancel_Clicked(object sender, EventArgs e)
        {
            Application.Instance.Quit();
        }

    }
}
