namespace Civ2engine
{
    public class Ruleset
    {
        public string Name { get; init; }
        
        public string FolderPath { get; init; }

        /// <summary>
        /// Base folder for TOT game types
        /// </summary>
        public string? FallbackPath { get; init; }
        
        public string Root { get; init; }

        private string[] _paths = null;
        
        public string[] Paths
        {
            get 
            {
                if (_paths == null)
                {
                    if (Root == FolderPath)
                    {
                        if (FallbackPath == null || FallbackPath == FolderPath)
                        {
                            _paths = new[] { FolderPath };
                        }
                        else
                        {
                            _paths = new[] { FolderPath, FallbackPath };
                        }
                    }
                    else
                    {
                        if (FallbackPath == null || FallbackPath == FolderPath || FallbackPath == Root)
                        {
                            _paths = new[] { FolderPath, Root };
                        }
                        else
                        {
                            _paths = new[] { FolderPath, FallbackPath, Root };
                        }
                    }
                }
                return _paths;
            }
        }

        public bool QuickStart { get; set; }
    }
}