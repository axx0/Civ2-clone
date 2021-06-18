using System.Drawing;
using Civ2engine.Enums;

namespace Civ2engine.Terrains
{
    public interface ITerrain
    {
        int X { get; }
        int Y { get; }
        TerrainType Type { get; }
        int special { get; }
        bool HasShield { get; }

        //From RULES.TXT
        string Name { get; }
        int MoveCost { get; }
        int Defense { get; }
        int Food { get; }
        int Shields { get; }
        int Trade { get; }
        bool CanBeIrrigated { get; }
        TerrainType IrrigationResult { get; }

        GovernmentType MinGovrnLevelAItoPerformIrrigation { get; }
        bool CanBeMined { get; }
        TerrainType MiningResult { get; }
        int MiningBonus { get; }
        int TurnsToMine { get; }
        GovernmentType MinGovrnLevelAItoPerformMining { get; }
        bool CanBeTransformed { get; }
        TerrainType TransformResult { get; }
        bool Resource { get; set; }
        bool River { get; set; }
        bool IsUnitPresent { get; }
        bool IsCityPresent { get; }
        bool Irrigation { get; set; }
        bool Mining { get; set; }
        bool Road { get; set; }
        bool Railroad { get; set; }
        bool Fortress { get; set; }
        bool Pollution { get; set; }
        bool Farmland { get; set; }
        bool Airbase { get; set; }
        int Island { get; set; }
        string Hexvalue { get; set; }
        Bitmap Graphic { get; set; }
    }
}
