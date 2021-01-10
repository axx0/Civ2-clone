using System.Windows.Forms;
using System.Drawing;
using civ2.Bitmaps;

namespace civ2.Forms
{
    public class Civ2radioBtn : RadioButton
    {
        public Civ2radioBtn()
        {
            BackColor = Color.Transparent;
            Font = new Font("Times New Roman", 18);
            ForeColor = Color.FromArgb(51, 51, 51);
            AutoSize = true;
        }
    }
}
