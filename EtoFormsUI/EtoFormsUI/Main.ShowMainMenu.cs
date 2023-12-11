using System;
using System.Collections.Generic;
using System.Linq;
using Eto.Forms;
using Eto.Drawing;
using Civ2engine;
using Model;
using DialogResult = Model.DialogResult;

namespace EtoFormsUI
{
    public partial class Main 
    {
        public void MainMenu()
        {
            var action = ActiveInterface.GetInitialAction();
            var picturePanels = new List<PicturePanel>();

            try
            {
                do
                {
                    if (action.FileInfo != null)
                    {
                        var file = action.FileInfo;
                        var ofd = new OpenFileDialog()
                        {
                            Directory = new Uri(Settings.Civ2Path),
                            Title = file.Title
                        };

                        var fialDialogResult = ofd.ShowDialog(this);

                        action = this.ActiveInterface.ProcessFile(ofd.Filenames,
                            fialDialogResult == Eto.Forms.DialogResult.Ok);
                    }
                    else if (action.MenuElement != null)
                    {
                        var menu = action.MenuElement;

                        picturePanels = UpdatePicturePanels(picturePanels, menu);


                        var dialog = new Civ2dialog(this, menu.Dialog);
                        dialog.Location = new Point(
                            (int)(Screen.PrimaryScreen.Bounds.Width) - dialog.Width - menu.DialogPos.X,
                            (int)(Screen.PrimaryScreen.Bounds.Height) - dialog.Height - menu.DialogPos.Y);

                        dialog.ShowModal(this);

                        var dialogResult =
                            new DialogResult(dialog.SelectedButton, dialog.SelectedIndex, dialog.CheckboxReturnStates);

                        action = this.ActiveInterface.ProcessDialog(menu.Dialog.Name, dialogResult);
                    }
                    else
                    {
                        action = null;
                    }
                } while (action != null);
            }
            finally
            {
                foreach (var panel in picturePanels)
                {
                    if (panel != null)
                    {
                        layout.Remove(panel);
                        panel.Dispose();
                    }
                }
            }
        }

        private List<PicturePanel> UpdatePicturePanels(List<PicturePanel> picturePanels, DialogElements dialog)
        {
            var existingPanels = picturePanels.ToList();
            var newPanels = new List<PicturePanel>();
            foreach (var d in dialog.Decorations)
            {
                var key = d.Image.Key;
                var existing = existingPanels.FirstOrDefault(p => p.Key == key);
                if (existing != null)
                {
                    existingPanels.Remove(existing);
                    newPanels.Add(existing);
                    if (existing.Location.X != d.Location.X || existing.Location.Y != d.Location.Y)
                    {
                        layout.Remove(existing);
                        layout.Add(existing, d.Location.X, d.Location.Y);
                    }
                }
            }

            foreach (var existingPanel in existingPanels)
            {
                layout.Remove(existingPanel);
                existingPanel.Dispose();
            }

            picturePanels = newPanels;
            return picturePanels;
        }
    }
}
