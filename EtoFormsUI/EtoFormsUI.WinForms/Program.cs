using Eto.Forms;
using System;

namespace EtoFormsUI.WinForms
{
    class MainClass
    {
        [STAThread]
        public static void Main(string[] args)
        {
            var platform = new Eto.WinForms.Platform();
            platform.Add<CustomEtoButton.IHandler>(() => new CustomEtoButtonHandler());
            platform.Add<VScrollBar.IHandler>(() => new VScrollBarHandler());
            platform.Add<HScrollBar.IHandler>(() => new HScrollBarHandler());

            new Application(platform).Run(new Main());
        }
    }
}
