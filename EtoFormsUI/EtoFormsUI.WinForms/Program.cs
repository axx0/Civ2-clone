using Eto.Forms;
using System;

namespace EtoFormsUI.WinForms
{
    class MainClass
    {
        [STAThread]
        public static void Main(string[] args)
        {
            new Application(Eto.Platforms.WinForms).Run(new Main());
        }
    }
}
