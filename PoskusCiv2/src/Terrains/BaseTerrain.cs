using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Terrains
{
    internal class BaseTerrain : ITerrain
    {
        public bool Resource { get; set; }
        public bool River { get; set; }
        public bool UnitPresent { get; set; }
        public bool CityPresent { get; set; }
        public bool Irrigation { get; set; }
        public bool Mining { get; set; }
        public bool Road { get; set; }
        public bool Railroad { get; set; }
        public bool Fortress { get; set; }
        public bool Pollution { get; set; }
        public bool Farmland { get; set; }
        public bool Airbase { get; set; }
        public int Special { get; set; }
        public int Island { get; set; }
        public int Food { get; }
        public int Shields { get; }
        public int Trade { get; }
        public int IrrigEffectsFood { get; }
        public int IrrigEffectsShields { get; }
        public int IrrigEffectsTrade { get; }
        public int MiningEffectsFood { get; }
        public int MiningEffectsShields { get; }
        public int MiningEffectsTrade { get; }
        public int TurnsToIrrigate { get; }
        public int TurnsToMine { get; }
        public string Name { get; set; }
        public string SpecialName { get; set; }
        public TerrainType Type { get; set; }

        public string Hexvalue { get; set; }

        protected BaseTerrain(int food = 1, int shields = 1, int trade = 1, int irrig_effects_food = 1, int irrig_effects_shields = 1, int irrig_effects_trade = 1, int mining_effects_food = 1, int mining_effects_shields = 1, int mining_effects_trade = 1, int turns_to_irrigate = 1, int turns_to_mine = 1)
        {
            Food = food;
            Shields = shields;
            IrrigEffectsFood = irrig_effects_food;
            IrrigEffectsShields = irrig_effects_shields;
            IrrigEffectsTrade = irrig_effects_trade;
            MiningEffectsFood = mining_effects_food;
            MiningEffectsShields = mining_effects_shields;
            MiningEffectsTrade = mining_effects_trade;
            TurnsToMine = turns_to_mine;
        }
    }
}
