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
        string Name { get; set; }
        string SpecialName { get; }
        int Special { get; set; }
        int Island { get; set; }
        int Food { get; }
        int SpecialFood1 { get; }
        int SpecialFood2 { get; }
        int Shields { get; }
        int SpecialShields1 { get; }
        int SpecialShields2 { get; }
        int Trade { get; }
        int SpecialTrade1 { get; }
        int SpecialTrade2 { get; }
        int IrrigEffectsFood { get; }
        int IrrigEffectsShields { get; }
        int IrrigEffectsTrade { get; }
        int MiningEffectsFood { get; }
        int MiningEffectsShields { get; }
        int MiningEffectsTrade { get; }
        int TurnsToIrrigate { get; }
        int TurnsToMine { get; }

        string Hexvalue { get; set; }
    }
}
