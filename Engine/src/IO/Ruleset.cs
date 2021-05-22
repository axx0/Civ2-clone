using System.Runtime.CompilerServices;

namespace Civ2engine
{
    public class Ruleset
    {
        public string Name { get; set; }
        
        public string FolderPath { get; set; }
        
        public string Root { get; set; }

        public string[] Paths
        {
            get
            {
                return Root == FolderPath ? new[] { FolderPath } : new[] {FolderPath, Root};
            }
        }
    }
}