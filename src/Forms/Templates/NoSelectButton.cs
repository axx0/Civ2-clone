using System.Windows.Forms;

namespace civ2.Forms
{
    class NoSelectButton : Button
    {
        public NoSelectButton()
        {
            SetStyle(ControlStyles.Selectable, false);  //Lose focus from button (cannot be selected by tab)
        }
    }
}
