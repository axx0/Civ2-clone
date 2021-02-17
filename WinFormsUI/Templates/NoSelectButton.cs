using System.Windows.Forms;

namespace WinFormsUI
{
    public class NoSelectButton : Button
    {
        public NoSelectButton()
        {
            SetStyle(ControlStyles.Selectable, false);  // Lose focus from button (cannot be selected by tab)
        }
    }
}
