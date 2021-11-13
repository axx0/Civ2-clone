using Eto.Forms;
using System;

namespace EtoFormsUI.Gtk
{
    class MainClass
    {
        [STAThread]
        public static void Main(string[] args)
        {
            var platform = new Eto.GtkSharp.Platform();
            platform.Add<CustomEtoButton.IHandler>(() => new CustomEtoButtonHandler());
            
            new Application(platform).Run(new Main());
        }
    }
}
