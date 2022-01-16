using System.IO;
using Eto.Forms;
using Eto.Drawing;
using EtoFormsUI.Initialization;
using Civ2engine;

namespace EtoFormsUI
{
    public partial class Main : Form
    {
        private PicturePanel sinaiPanel;

        public void MainMenu()
        {
            byte[] bytes = File.ReadAllBytes(Settings.Civ2Path + "Intro.dll");  // For loading images from intro.dll

            sinaiPanel = new PicturePanel(Images.ExtractBitmap(bytes, "sinaiPic"));
            layout.Add(sinaiPanel, new Point(160, 76));

            var mainMenuDialog = new Civ2dialog(this, popupBoxList["MAINMENU"]);
            mainMenuDialog.Location = new Point((int)(Screen.PrimaryScreen.Bounds.Width - mainMenuDialog.Width - 156),
                                                (int)(Screen.PrimaryScreen.Bounds.Height - mainMenuDialog.Height - 72));
            mainMenuDialog.ShowModal(this);

            sinaiPanel.Dispose();
            switch (mainMenuDialog.SelectedIndex)
            {
                //New Game
                case 0:
                    {
                        NewGame.Start(this, false);
                        break;
                    }

                // Start premade
                case 1:
                    {
                        LocateStartingFiles("Select Map To Load",
                            new FileFilter("Save Files (*.mp)", ".mp"), StartPremadeInit);
                        break;
                    }

                //Customise World
                case 2:
                    {
                        NewGame.Start(this, true);
                        break;
                    }

                // Load scenario
                case 3:
                    {
                        LocateStartingFiles("Select Scenario To Load",
                            new FileFilter("Save Files (*.scn)", ".scn"), LoadScenarioInit);
                        break;
                    }

                // Load game
                case 4:
                    {
                        LocateStartingFiles("Select Game To Load", new FileFilter("Save Files (*.sav)", ".SAV"),
                            LoadGameInitialization
                        );
                        break;
                    }
            }
        }
    }
}
