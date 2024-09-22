using System.Collections.Generic;

namespace Civ2engine
{
    public class TerrainImprovement
    {
        /// <summary>
        /// The name of the improvement class
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Improvements have multiple levels
        ///     Irrigation -> Farmland
        ///     Road -> Railroad
        /// </summary>
        public IList<ImprovementLevel> Levels { get; set; }

        /// <summary>
        /// True if all cities are considered to have this improvement (once researched)
        /// </summary>
        public bool AllCitys { get; set; }

        /// <summary>
        /// List of list to accomodate multiple maps first list is map second is terrains for map
        /// </summary>
        public List<List<AllowedTerrain>> AllowedTerrains { get; set; }
        
        /// <summary>
        /// Key to build improvement if captial required format like Shift|D
        /// </summary>
        public string Shortcut { get; set; }

        public int Id { get; set; }
        
        public List<int> AdjacencyRules { get; set; }
        
        /// <summary>
        /// Popup message key to show when max level of improvement has been built
        /// </summary>
        public string MaxLevelReachedMessage { get; set; }

        /// <summary>
        /// Error popup to show when the Adjacency rules are not met
        /// </summary>
        public string NoAdjacencyMessage { get; set; }

        /// <summary>
        /// Multiple improvements in the same non zero exclusive group cannot be built in the same square completing one will destroy all others
        /// </summary>
        public int ExclusiveGroup { get; set; }

        /// <summary>
        /// The Layer to draw this improvement at higher numbers are drawn on top of lower
        /// </summary>
        public int Layer { get; set; }

        /// <summary>
        /// True if this improvement has graphics that change base on it's presence in neighbouring tiles
        /// </summary>
        public bool HasMultiTile { get; set; }

        /// <summary>
        /// True if this improvement is a bad thing that settlers remove rather than add. ie. Pollution
        /// </summary>
        public bool Negative { get; set; }

        /// <summary>
        /// Where this improvement build order should appear in the menu
        /// </summary>
        public int MenuGroup { get; set; }
        
        /// <summary>
        /// True if the improvement should draw over the top of units like a fortress
        /// </summary>
        public bool Foreground { get; set; }

        /// <summary>
        /// If set this improvement wil hide units of the listed Domain
        /// </summary>
        public int HideUnits { get; set; } = -1;
    }
}