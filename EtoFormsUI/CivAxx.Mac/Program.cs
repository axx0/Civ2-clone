using System;
using Eto.Forms;
using EtoFormsUI;

namespace CivAxx.Mac
{
    class Program
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