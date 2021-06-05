using Civ2engine;
using Eto.Forms;

namespace EtoFormsUI
{
    /// <summary>
    /// "Reveal Map" panel in cheat menu.
    /// </summary>
    public class RevealMapPanel : RadiobuttonPanel
    {
        private Map Map => Game.Instance.CurrentMap;

        public RevealMapPanel(Main mainForm, string[] activeCivs) :
            base(mainForm, 686, 4 + activeCivs.Length * 32 + 38 + 46, "Select Map View", activeCivs, new string[] { "OK", "Cancel" })
        {
            RadioBtnList.SelectedIndex = activeCivs.Length - 2;    // TODO: this default selection doesn't work for some reason
            innerPanel.Invalidate();

            DefaultButton = Button[0];
            DefaultButton.Click += (sender, e) => 
            {
                if (RadioBtnList.SelectedIndex > activeCivs.Length - 3)
                {
                    Map.MapRevealed = RadioBtnList.SelectedIndex == activeCivs.Length - 2;
                }
                else
                {
                    Map.WhichCivsMapShown = RadioBtnList.SelectedIndex;
                }

                foreach (MenuItem item in mainForm.Menu.Items) item.Enabled = true;
                Close();
            };

            AbortButton = Button[1];
            AbortButton.Click += (sender, e) =>
            {
                foreach (MenuItem item in mainForm.Menu.Items) item.Enabled = true;
                Close();
            };
        }
    }
}