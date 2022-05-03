using System.Collections.Generic;

namespace Civ2engine
{
    public class ImprovementLevel
    {
        /// <summary>
        /// The name for this level Eg Road/Railroad
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Tech required to build this level
        /// </summary>
        public int RequiredTech { get; set; }

        /// <summary>
        /// Error popup to show when build is attempted without required tech
        /// </summary>
        public string MissingRequiredTechMessage { get; set; }
        
        /// <summary>
        /// The label for building this
        /// </summary>
        public string BuildLabel { get; set; }
        
        /// <summary>
        /// Extra multiplier if this level is harder or easier to build
        /// </summary>
        public int BuildCostMultiplier { get; set; }
        
        public List<TerrainImprovementAction> Effects { get; set; }
        
    }
}