using System.Collections.Generic;
using Civ2engine.Advances;

namespace Civ2engine
{
    public class AllowedTerrain
    {
        public int TerrainType { get; set; }
        
        public int AiGovernmentBuild { get; set; }
        
        public int BuildTime { get; set; }

        /// <summary>
        /// Tech required for building on specific terrain
        /// </summary>
        public int RequiredTech { get; set; } = AdvancesConstants.Nil;
     
        /// <summary>
        /// Error message shown when civ lacks tech to build on this terrain
        /// </summary>
        public string MissingRequiredTechMessage { get; set; }
        public List<TerrainImprovementAction> Effects { get; set; }
    }
}