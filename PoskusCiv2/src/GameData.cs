using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace PoskusCiv2
{
    class GameData
    {
        string path = @"C:\CIV 2\Civ2\RULES.txt";
        string line;
        public static Unit unit = new Unit();

        public void ReadDataFiles()
        {           

            using (StreamReader reader = new StreamReader(path))
            {
                while (!reader.EndOfStream)
                {
                    line = reader.ReadLine();
                    if (line == "@UNITS")
                    {
                        for (int i = 0; i < 62; i++)
                        {
                            line = reader.ReadLine();
                            string[] ar = line.Split(',');
                            unit.type = ar[0];
                            unit.until = ar[1];
                            unit.domain = Convert.ToInt32(ar[2]);
                        }

                    }

                }
            }

        }
    }
}
