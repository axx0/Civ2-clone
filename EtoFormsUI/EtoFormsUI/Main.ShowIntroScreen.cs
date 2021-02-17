using Eto.Forms;
using Eto.Drawing;

namespace EtoFormsUI
{
    public partial class Main : Form
    {
        private PicturePanel sinaiPanel;
        private ChoiceMenuPanel choiceMenu;

        // Load intro screen
        public void ShowIntroScreen()
        {
            // Sinai pic
            sinaiPanel = new PicturePanel(Images.SinaiPic);
            layout.Add(sinaiPanel, new Point((int)(Screen.PrimaryScreen.Bounds.Width * 0.08333), (int)(Screen.PrimaryScreen.Bounds.Height * 0.0933)));

            // Choice menu
            choiceMenu = new ChoiceMenuPanel(this);
            layout.Add(choiceMenu, new Point((int)(Screen.PrimaryScreen.Bounds.Width * 0.745), (int)(Screen.PrimaryScreen.Bounds.Height * 0.570)));

            // Disable main menu items
            foreach (MenuItem item in this.Menu.Items) item.Enabled = false;

            Content = layout;
        }
    }
}
