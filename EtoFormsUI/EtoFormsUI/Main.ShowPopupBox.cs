using System;
using System.IO;
using Eto.Forms;
using Eto.Drawing;
using Civ2engine.Events;
using Civ2engine;
using System.Collections.Generic;
using System.Linq;
using EtoFormsUI.Initialization;

namespace EtoFormsUI
{
    public partial class Main : Form
    {
        private string savDirectory, savName;

        private void LocateStartingFiles(string title, FileFilter filter, Func<Ruleset, string, bool> initializer)
        {
            using var ofd = new OpenFileDialog
            {
                Directory = new Uri(Settings.Civ2Path),
                Title = title,
                Filters = {filter}
            };

            var result = ofd.ShowDialog(this.ParentWindow);
            //sinaiPanel.Dispose();
            sinaiPanel = null;
            if (result == DialogResult.Ok)
            {
                // Get SAV name & directory name from result
                savDirectory = Path.GetDirectoryName(ofd.FileName);
                var ruleSet = new Ruleset
                {
                    FolderPath = savDirectory,
                    Root = Settings.SearchPaths.FirstOrDefault(p => savDirectory.StartsWith(p)) ?? Settings.SearchPaths[0]
                };
                savName = Path.GetFileName(ofd.FileName);
                if (initializer(ruleSet, savName))
                {
                    Sounds.LoadSounds(ruleSet.Paths);
                    Playgame();
                    return;
                }
            }

            MainMenu();
        }

        public void Playgame()
        {
            Sounds.Stop();
            Sounds.PlaySound(GameSounds.MenuOk);

            var playerCiv = Game.GetPlayerCiv;

            var dialog = new Civ2dialog(this, popupBoxList["LOADOK"], new List<string>
                    {
                        playerCiv.LeaderTitle, playerCiv.LeaderName,
                        playerCiv.TribeName, Game.GetGameYearString,
                        Game.DifficultyLevel.ToString()
                    });
            dialog.ShowModal(this);
            StartGame();
            Sounds.PlaySound(GameSounds.MenuOk);
        }

        public void ShowCityDialog(string dialog, IList<string> replaceStrings)
        {
            var popupbox = new Civ2dialog(this, popupBoxList[dialog], replaceStrings);
            popupbox.ShowModal(Parent);
        }
    }
}
