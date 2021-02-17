using System;
using System.Windows.Forms;

namespace WinFormsUI
{
    static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Civ2engine.Settings.LoadConfigSettings();
            Application.Run(new Main());
        }
    }
}
