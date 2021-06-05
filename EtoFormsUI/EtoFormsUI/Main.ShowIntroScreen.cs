using Civ2engine.Events;
using Eto.Forms;
using Eto.Drawing;

namespace EtoFormsUI
{
    public partial class Main : Form
    {
        private PicturePanel sinaiPanel;

        // Load intro screen
        public void ShowIntroScreen()
        {
            Content = layout;
        }

        public void MainMenu()
        {
            OnPopupboxEvent?.Invoke(null, new PopupboxEventArgs("MAINMENU"));
        }
    }
}
