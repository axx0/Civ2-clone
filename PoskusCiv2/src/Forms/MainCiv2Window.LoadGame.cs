using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoskusCiv2.Forms
{
    public partial class MainCiv2Window
    {
		public void LoadGame(string fileName)
        {
            mapForm = new MapForm(this);
            mapForm.MdiParent = this;
            mapForm.Location = new Point(0, 0);
            mapForm.Show();

            MainIcon.SendToBack();
            SinaiIcon.Hide();
        }
    }
}
