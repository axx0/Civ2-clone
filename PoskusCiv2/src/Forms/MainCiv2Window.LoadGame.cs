using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTciv2.Forms
{
    public partial class MainCiv2Window
    {
		public void LoadGame(string fileName)
        {
            this.Enabled = true;    //enable the menu after intro screen

            Game.LoadGame(fileName);

            //mapForm = new MapForm(this);
            //mapForm.MdiParent = this;
            //mapForm.Show();
            //mapForm.Location = new Point(0, 0);

            //worldMapForm = new WorldMapForm(this);
            //worldMapForm.MdiParent = this;
            //worldMapForm.Show();
            //worldMapForm.Location = new Point((int)(ClientSize.Width * 0.8625) - 6 + 1, 0);

            //statusForm = new StatusForm(this);
            //statusForm.MdiParent = this;
            //statusForm.Show();
            //statusForm.Location = new Point((int)(ClientSize.Width * 0.8625) - 6 + 1, worldMapForm.Height + 1);

            MainIcon.SendToBack();
            SinaiIcon.Hide();
            AreWeInIntroScreen = false;
        }
    }
}
