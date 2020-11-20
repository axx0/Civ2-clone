using System;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using civ2.Units;
using civ2.Enums;

namespace civ2
{
    public class HelpConsole
    {
        [DllImport("kernel32.dll",
        EntryPoint = "AllocConsole",
        SetLastError = true,
        CharSet = CharSet.Auto,
        CallingConvention = CallingConvention.StdCall)]
        private static extern int AllocConsole();

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr CreateFile(
            string lpFileName,
            uint dwDesiredAccess,
            uint dwShareMode,
            uint lpSecurityAttributes,
            uint dwCreationDisposition,
            uint dwFlagsAndAttributes,
            uint hTemplateFile);

        [DllImport("kernel32.dll")]
        static extern bool FreeConsole();

        private const int MY_CODE_PAGE = 437;
        private const uint GENERIC_WRITE = 0x40000000;
        private const uint FILE_SHARE_WRITE = 0x2;
        private const uint OPEN_EXISTING = 0x3;

        public static void CreateConsole()
        {
            AllocConsole();

            IntPtr stdHandle = CreateFile(
                "CONOUT$",
                GENERIC_WRITE,
                FILE_SHARE_WRITE,
                0, OPEN_EXISTING, 0, 0
            );

            SafeFileHandle safeFileHandle = new SafeFileHandle(stdHandle, true);
            FileStream fileStream = new FileStream(safeFileHandle, FileAccess.Write);
            Encoding encoding = Encoding.GetEncoding(MY_CODE_PAGE);
            StreamWriter standardOutput = new StreamWriter(fileStream, encoding);
            standardOutput.AutoFlush = true;
            Console.SetOut(standardOutput);

            var t = new System.Threading.Thread(() =>
            {
                AllocConsole();
                Console.Title = "OpenCiv2 console";
                for (; ; )
                {
                    Console.Write(">>");
                    var cmd = Console.ReadLine();
                    if (cmd.ToLower() == "quit") break;
                    if (cmd.ToLower() == "list units")
                    {
                        //List for all civs
                        int count = 0;
                        foreach(IUnit unit in Game.Units)
                            Console.WriteLine($"#{count++} {unit.Type}, XY=({unit.X},{unit.Y}), {Game.Civs[unit.CivId].TribeName}");
                        Console.WriteLine();
                        //List for specific civs
                        foreach(Civilization civ in Game.Civs)
                        {
                            Console.WriteLine($"{civ.TribeName}");
                            foreach (IUnit unit in Game.Units.Where(n => Game.Civs[n.CivId].TribeName == civ.TribeName))
                            {
                                int id = Game.Units.FindIndex(n => n == unit);
                                string active;
                                if (unit.TurnEnded) active = "(turn ended)";
                                else active = "(active)";
                                string order;
                                if (unit.Order == OrderType.Fortify) order = "fortifying";
                                else if (unit.Order == OrderType.Fortified) order = "fortified";
                                else if (unit.Order == OrderType.Sleep) order = "sleep";
                                else if (unit.Order == OrderType.BuildFortress) order = "building fortress";
                                else if (unit.Order == OrderType.BuildRoad) order = "building road";
                                else if (unit.Order == OrderType.BuildIrrigation) order = "building irrigation";
                                else if (unit.Order == OrderType.BuildMine) order = "building mine";
                                else if (unit.Order == OrderType.Transform) order = "transforming";
                                else if (unit.Order == OrderType.CleanPollution) order = "cleaning pollution";
                                else if (unit.Order == OrderType.BuildAirbase) order = "building airbase";
                                else if (unit.Order == OrderType.GoTo) order = "Go-To";
                                else order = "no orders";
                                Console.WriteLine($"#{id} {unit.Type}, XY=({unit.X},{unit.Y}), {active}, {order}");
                            }
                            Console.WriteLine();
                        }
                    }
                    if (cmd.ToLower() == "help")
                    {
                        Console.WriteLine("'list units' ... list units of all civs");
                        Console.WriteLine();
                    }
                }
                FreeConsole();
            });
            t.IsBackground = true;
            t.Start();
        }
    }    
}
