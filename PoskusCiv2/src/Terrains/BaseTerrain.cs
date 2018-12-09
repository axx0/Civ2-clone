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
        public int Special { get; set; }    //0...no, 1...special1, 2...special2
        public int Island { get; set; }
        public int Food { get; }
        public int SpecialFood1 { get; }
        public int SpecialFood2 { get; }
        public int Shields { get; }
        public int SpecialShields1 { get; }
        public int SpecialShields2 { get; }
        public int Trade { get; }
        public int SpecialTrade1 { get; }
        public int SpecialTrade2 { get; }
        public int IrrigEffectsFood { get; }
        public int IrrigEffectsShields { get; }
        public int IrrigEffectsTrade { get; }
        public int MiningEffectsFood { get; }
        public int MiningEffectsShields { get; }
        public int MiningEffectsTrade { get; }
        public int TurnsToIrrigate { get; }
        public int TurnsToMine { get; }
        public string Name { get; set; }

        public string SpecialName
        {
            get
            {
                if (SpecialType == SpecialType.Oasis) { return "Oasis"; }
                else if (SpecialType == SpecialType.DesertOil) { return "Desert Oil"; }
                else if (SpecialType == SpecialType.Pheasant) { return "Pheasant"; }
                else if (SpecialType == SpecialType.Silk) { return "Silk"; }
                else if (SpecialType == SpecialType.Ivory) { return "Ivory"; }
                else if (SpecialType == SpecialType.GlacierOil) { return "Glacier Oil"; }
                else if (SpecialType == SpecialType.GrasslandShield) { return "Shield"; }
                else if (SpecialType == SpecialType.Coal) { return "Coal"; }
                else if (SpecialType == SpecialType.Wine) { return "Wine"; }
                else if (SpecialType == SpecialType.Gems) { return "Gems"; }
                else if (SpecialType == SpecialType.Fruit) { return "Fruit"; }
                else if (SpecialType == SpecialType.Gold) { return "Gold"; }
                else if (SpecialType == SpecialType.Iron) { return "Iron"; }
                else if (SpecialType == SpecialType.Fish) { return "Fish"; }
                else if (SpecialType == SpecialType.Whales) { return "Whales"; }
                else if (SpecialType == SpecialType.Buffalo) { return "Buffalo"; }
                else if (SpecialType == SpecialType.Wheat) { return "Wheat"; }
                else if (SpecialType == SpecialType.Peat) { return "Peat"; }
                else if (SpecialType == SpecialType.Spice) { return "Spice"; }
                else if (SpecialType == SpecialType.Game) { return "Game"; }
                else if (SpecialType == SpecialType.Furs) { return "Furs"; }
                else { return ""; }
            }
        }

        public TerrainType Type { get; set; }
        public SpecialType SpecialType { get; set; }

        public string Hexvalue { get; set; }

        protected BaseTerrain(int food = 1, int shields = 1, int trade = 1, int spec_food1 = 1, int spec_shields1 = 1, int spec_trade1 = 1, int spec_food2 = 1, int spec_shields2 = 1, int spec_trade2 = 1, int irrig_effects_food = 1, int irrig_effects_shields = 1, int irrig_effects_trade = 1, int mining_effects_food = 1, int mining_effects_shields = 1, int mining_effects_trade = 1, int turns_to_irrigate = 1, int turns_to_mine = 1)
        {
            Food = food;
            SpecialFood1 = spec_food1;
            SpecialFood2 = spec_food2;
            Shields = shields;
            SpecialShields1 = spec_shields1;
            SpecialShields2 = spec_shields2;
            Trade = trade;
            SpecialTrade1 = spec_trade1;
            SpecialTrade2 = spec_trade2;
            IrrigEffectsFood = irrig_effects_food;
            IrrigEffectsShields = irrig_effects_shields;
            IrrigEffectsTrade = irrig_effects_trade;
            MiningEffectsFood = mining_effects_food;
            MiningEffectsShields = mining_effects_shields;
            MiningEffectsTrade = mining_effects_trade;
            TurnsToIrrigate = turns_to_irrigate;
            TurnsToMine = turns_to_mine;
        }
    }
}
