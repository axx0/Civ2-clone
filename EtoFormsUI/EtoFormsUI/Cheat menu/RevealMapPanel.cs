using System;
using Civ2engine;
using Civ2engine.Events;

namespace EtoFormsUI
{
    /// <summary>
    /// "Reveal Map" panel in cheat menu.
    /// </summary>
    public class RevealMapPanel : RadiobuttonPanel
    {
        private Map Map => Map.Instance;
        private Main main;
        public static event EventHandler<MapEventArgs> OnMapEvent;

        public RevealMapPanel(Main mainForm, string[] activeCivs) :
            base(mainForm, 686, 4 + activeCivs.Length * 32 + 38 + 46, "Select Map View", activeCivs, new string[] { "OK", "Cancel" })
        {
            main = mainForm;

            DefaultButton = Button[0];
            DefaultButton.Click += (sender, e) => 
            {
                Map.WhichCivsMapShown = RadioBtnList.SelectedIndex;
                Close();
            };

            AbortButton = Button[1];
            AbortButton.Click += (sender, e) => Close();
        }
    }
}