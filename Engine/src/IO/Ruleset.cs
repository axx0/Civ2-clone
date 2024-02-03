using System.Collections.Generic;

namespace Civ2engine
{
    public class Ruleset
    {
        public string Name { get; init; }
        
        public string FolderPath { get; init; }

        /// <summary>
        /// Base folder for TOT game types
        /// </summary>
        public List<string> FallbackPaths { get; init; }
        
        public string Root { get; init; }

        private string[] _paths = null;
        
        public string[] Paths
        {
            get 
            {
                if (_paths == null)
                {
                    List<string> _pathsList = new();
                    _pathsList.Add(FolderPath);
                    _pathsList.AddRange(FallbackPaths);
                    if (!_pathsList.Contains(Root))
                    {
                        _pathsList.Add(Root);
                    }
                    _paths = _pathsList.ToArray();
                }
                return _paths;
            }
        }

        public bool QuickStart { get; set; }
    }
}