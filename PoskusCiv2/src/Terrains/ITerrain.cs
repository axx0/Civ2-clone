using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Terrains
{
    public interface ITerrain
    {
        TerrainType Type { get; set; }
        SpecialType SpecialType { get; set; }
        string Name { get; set; }
        string SpecialName { get; }
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
        
        int Movecost { get; }
        int Defense { get; }
        int Food { get; }
        int Shields { get; }
        int Trade { get; }
        bool CanIrrigate { get; }
        TerrainType? IrrigationResult { get; }
        int IrrigationBonus { get; }
        int TurnsToIrrigate { get; }
        int AIirrigation { get; }
        bool CanMine { get; }
        TerrainType? MiningResult { get; }
        int MiningBonus { get; }
        int TurnsToMine { get; }
        int AImining { get; }
        TerrainType? TransformResult { get; }

        int Special { get; set; }
        int Island { get; set; }
        int SpecialFood1 { get; }
        int SpecialFood2 { get; }
        int SpecialShields1 { get; }
        int SpecialShields2 { get; }
        int SpecialTrade1 { get; }
        int SpecialTrade2 { get; }

        string Hexvalue { get; set; }
    }
}
