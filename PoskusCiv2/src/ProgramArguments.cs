using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;

namespace PoskusCiv2
{
    public class ProgramArguments
    {
        [Option('n', "read", HelpText = "Name of SAV file to be opened.")]
        public string SAVName { get; set; }

        [Option('p', "read", HelpText = "Path of Civ2 directory.")]
        public string Path { get; set; }

        [Option('v', "verbose", DefaultValue = true, HelpText = "Prints all messages to standard output.")]
        public bool Verbose { get; set; }
    }
}
