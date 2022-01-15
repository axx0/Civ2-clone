using Eto.Forms;
using Eto.Drawing;
using EtoFormsUI.Initialization;

namespace EtoFormsUI
{
    public partial class Main : Form
    {
        private PicturePanel sinaiPanel;

        public void MainMenu()
        {
            // Sinai pic
            sinaiPanel = new PicturePanel(Images.SinaiPic);
            layout.Add(sinaiPanel, new Point((int)(Screen.PrimaryScreen.Bounds.Width * 0.08333), (int)(Screen.PrimaryScreen.Bounds.Height * 0.0933)));

            var popupBox = new Civ2dialogV2(this, popupBoxList["MAINMENU"])
            {
                Location = new Point((int)(Screen.PrimaryScreen.Bounds.Width * 0.745),
                    (int)(Screen.PrimaryScreen.Bounds.Height * 0.570))
            };
            popupBox.ShowModal(this);

            sinaiPanel.Dispose();
            switch (popupBox.SelectedIndex)
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
