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
            // Sinai pic
            sinaiPanel = new PicturePanel(Images.SinaiPic);
            layout.Add(sinaiPanel, new Point((int)(Screen.PrimaryScreen.Bounds.Width * 0.08333), (int)(Screen.PrimaryScreen.Bounds.Height * 0.0933)));

            Content = layout;
        }

        public void MainMenu()
        {
            OnPopupboxEvent?.Invoke(null, new PopupboxEventArgs("MAINMENU"));
        }
    }
}
