using Eto.Forms;
using System;

namespace EtoFormsUI.Gtk
{
    class MainClass
    {
        [STAThread]
        public static void Main(string[] args)
        {
            new Application(Eto.Platforms.Gtk).Run(new Main());
        }
    }
}
