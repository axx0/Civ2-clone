using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RTciv2.Enums;

namespace RTciv2.Terrains
{
    public interface ITerrain
    {
        TerrainType Type { get; set; }
        SpecialType? SpecType { get; set; }

        //From RULES.TXT
        string Name { get; set; }
        string SpecName { get; set; }
        int MoveCost { get; set; }
        int Defense { get; set; }
        int Food { get; set; }
        int Shields { get; set; }
        int Trade { get; set; }
        bool CanIrrigate { get; set; }
        TerrainType? IrrigationResult { get; set; }
        int IrrigationBonus { get; set; }
        int TurnsToIrrigate { get; set; }
        int AIirrigation { get; set; }
        bool CanMine { get; set; }
        TerrainType? MiningResult { get; set; }
        int MiningBonus { get; set; }
        int TurnsToMine { get; set; }
        int AImining { get; set; }
        TerrainType? TransformResult { get; set; }

        bool Resource { get; set; }
        bool River { get; set; }
        bool UnitPresent { get; set; }
        bool CityPresent { get; set; }
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
    }
}
