using System.Drawing;
using civ2.Enums;

namespace civ2.Terrains
{
    public interface ITerrain
    {
        int X { get; set; }
        int Y { get; set; }
        TerrainType Type { get; set; }
        SpecialType? SpecType { get; set; }

        //From RULES.TXT
        string Name { get; }
        int MoveCost { get; }
        int Defense { get; }
        int Food { get; }
        int Shields { get; }
        int Trade { get; }
        bool CanBeIrrigated { get; }
        TerrainType IrrigationResult { get; }
        int IrrigationBonus { get; }
        int TurnsToIrrigate { get; }
        GovernmentType MinGovrnLevelAItoPerformIrrigation { get; }
        bool CanBeMined { get; }
        TerrainType MiningResult { get; }
        int MiningBonus { get; }
        int TurnsToMine { get; }
        GovernmentType MinGovrnLevelAItoPerformMining { get; }
        bool CanBeTransformed { get; }
        TerrainType TransformResult { get; }

        string SpecName { get; }


        bool Resource { get; set; }
        bool River { get; set; }
        bool UnitPresent { get; }
        bool CityPresent { get; }
        bool Irrigation { get; set; }
        bool Mining { get; set; }
        bool Road { get; set; }
        bool Railroad { get; set; }
        bool Fortress { get; set; }
        bool Pollution { get; set; }
        bool Farmland { get; set; }
        bool Airbase { get; set; }
        int Island { get; set; }
        bool[] Visibility { get; set; }
        string Hexvalue { get; set; }
        Bitmap Graphic { get; set; }
    }
}
