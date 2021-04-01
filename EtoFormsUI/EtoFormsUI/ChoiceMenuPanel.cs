using System;
using Eto.Forms;
using Civ2engine;
using System.IO;

namespace EtoFormsUI
{
    public partial class ChoiceMenuPanel : RadiobuttonPanel
    {
        private Main main;

        public ChoiceMenuPanel(Main parent) :
            base(parent, 333, 344, "Civilization II Multiplayer Gold", new string[] { "Start a New Game", "Start on Premade World", "Customize World", "Begin Scenario", "Load a Game", "Multiplayer Game", "View Hall of Fame", "View Credits" }, new string[] { "OK", "Cancel" })
        {
            main = parent;

            // Define abort button (= Cancel) so that is also called with Esc
            AbortButton = Button[1];
            AbortButton.Click += (sender, e) =>
            {
                foreach (MenuItem item in parent.Menu.Items) item.Enabled = true;
                Application.Instance.Quit();
            };

            // Define default button (= OK) so that it is also called with return key
            DefaultButton = Button[0];
            DefaultButton.Click += (sender, e) =>
            {
                // Load game
                if (RadioBtnList.SelectedIndex == 4)
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
                        this.Close();
                    }
                }
            };
        }
    }
}
