using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RTciv2.Forms
{
    class NoSelectButton : Button
    {
        public NoSelectButton()
        {
            SetStyle(ControlStyles.Selectable, false);  //Lose focus from button (cannot be selected by tab)
        }
    }
}
