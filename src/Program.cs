using System;
using System.Windows.Forms;

namespace civ2
{    
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //HelpConsole.CreateConsole();
            Settings.LoadConfigSettings();
            Application.Run(new Forms.Main());
        }
    }

}
