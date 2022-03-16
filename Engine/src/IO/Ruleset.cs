using System.Runtime.CompilerServices;

namespace Civ2engine
{
    public class Ruleset
    {
        public string Name { get; init; }
        
        public string FolderPath { get; init; }
        
        public string Root { get; init; }

        private string[] _paths = null;
        
        public string[] Paths
        {
            get { return _paths ??= (Root == FolderPath ? new[] { FolderPath } : new[] { FolderPath, Root }); }
        }

        public bool QuickStart { get; set; }
    }
}