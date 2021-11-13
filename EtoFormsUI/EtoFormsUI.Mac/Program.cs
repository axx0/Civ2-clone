using Eto.Forms;
using System;

namespace EtoFormsUI.Mac
{
    class MainClass
    {
        [STAThread]
        public static void Main(string[] args)
        {
            var platform = new Eto.Mac.Platform();
            platform.Add<CustomEtoButton.IHandler>(() => new CustomEtoButtonHandler());

            new Application(platform).Run(new Main());
        }
    }
}
