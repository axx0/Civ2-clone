using System.Collections.Generic;
using System.Linq;

namespace Civ2engine.IO
{
    public class Ruleset
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">A name for this ruleset, use to display it if user is selecting rules</param>
        /// <param name="metadata">Extra data to identify this ruleset</param>
        /// <param name="paths">The ruleset search paths</param>
        public Ruleset(string name, Dictionary<string,string> metadata, params string[] paths)
        {
            Name = name;
            Metadata = metadata;
            FolderPath = paths.First();
            Root = paths.Last();

            _paths = paths;
        }

        public string Name { get; }
        
        public string FolderPath { get; }
        
        public string Root { get; }

        private readonly string[] _paths;

        public Ruleset(Ruleset parent, string extraPath) : this(parent.Name + "_Extended",
            new Dictionary<string, string>(),
            Enumerable.Repeat(extraPath, 1).Concat(parent.Paths).ToArray())
        {
            InterfaceIndex = parent.InterfaceIndex;
        }

        public string[] Paths => _paths;
        public int InterfaceIndex { get; set; }
        public Dictionary<string, string> Metadata { get; } 
    }
}