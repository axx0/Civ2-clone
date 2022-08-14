using System;
using Eto.Forms;

namespace CivAxx.Wpf
{
    // ReSharper disable once ClassNeverInstantiated.Global
    class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            new Application(Eto.Platforms.Wpf).Run(new MainForm());
        }
    }
}