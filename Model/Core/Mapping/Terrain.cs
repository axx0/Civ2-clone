using Civ2engine.Enums;
using Model.Core;

namespace Civ2engine.Terrains
{
    public class Terrain : ITerrain
    {
        public TerrainType Type { get; set; }

        // From RULES.TXT
        public string Name { get; set; }
        
        public int MoveCost { get; set; }
        public int Defense { get; set; }
        public int Food { get; set; }
        public int Shields { get; set; }
        public int Trade  { get; set; }
        public int IrrigationBonus { get; set; }
        public int TurnsToIrrigate { get; set; }
        public int MinGovrnLevelAItoPerformIrrigation { get; set; }   // Be careful, 0=never!
        public bool CanBeMined => CanMine != -2 ;  // yes meaning the result can be mining or transform. of terrain

        public int MiningBonus { get; set; }
        public int TurnsToMine { get; set; }
        
        public int RoadBonus { get; set; }
        public int MinGovrnLevelAItoPerformMining { get; set; }     // Be careful, 0=never!
        public bool CanBeTransformed => Transform != -2 ; // usually only ocean can't be transformed

        // TODO: put special resources logic into here
        public int CanIrrigate { get; set; }
        public int CanMine { get; set; }
        public int Transform { get; set; }
        public Special[] Specials { get; set; }
        public bool Impassable { get; set; }
        public bool CanHaveCity => !Impassable && TerrainType.Ocean != Type;
    }
}
