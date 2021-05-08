using System;
using System.IO;
using Eto.Forms;
using Eto.Drawing;
using Civ2engine.Events;
using Civ2engine;
using System.Collections.Generic;

namespace EtoFormsUI
{
    public partial class Main : Form
    {
        private string savDirectory, savName;

        private void PopupboxEvent(object sender, PopupboxEventArgs e)
        {
            switch (e.BoxName)
            {
                case "MAINMENU":
                    {
                        var mainMenu = new Civ2dialog_v2(this, popupBoxList.Find(p => p.Name == "MAINMENU"));
                        mainMenu.Location = new Point((int)(Screen.PrimaryScreen.Bounds.Width * 0.745), (int)(Screen.PrimaryScreen.Bounds.Height * 0.570));
                        mainMenu.ShowModal(Parent);
                        // Load game
                        if (mainMenu.SelectedIndex == 4)
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
                                savDirectory = ofd.Directory.LocalPath;
                                savName = Path.GetFileName(ofd.FileName);
                                LoadGameInitialization(savDirectory, savName);
                                Sounds.Stop();

                                OnPopupboxEvent?.Invoke(null, new PopupboxEventArgs("LOADOK", new List<string> { Game.GetActiveCiv.LeaderTitle, Game.GetActiveCiv.LeaderName, Game.GetActiveCiv.TribeName, Game.GetGameYearString, Game.DifficultyLevel.ToString() }));
                            }
                        }
                        break;
                    }
                case "LOADOK":
                    {
                        var menu = new Civ2dialog_v2(this, popupBoxList.Find(p => p.Name == "LOADOK"), e.ReplaceStrings);
                        menu.ShowModal(Parent);
                        StartGame();
                        break;
                    }
                default: break;
            }
        }
    }
}
