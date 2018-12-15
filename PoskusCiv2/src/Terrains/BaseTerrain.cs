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
        public string Name { get; set; }
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

        public int Movecost { get; }
        public int Defense { get; }
        public int Food { get; }
        public int Shields { get; }
        public int Trade { get; }
        public bool CanIrrigate { get; }
        public TerrainType? IrrigationResult { get; }
        public int IrrigationBonus { get; }
        public int TurnsToIrrigate { get; }
        public int AIirrigation { get; }
        public bool CanMine { get; }
        public TerrainType? MiningResult { get; }
        public int MiningBonus { get; }
        public int TurnsToMine { get; }
        public int AImining { get; }
        public TerrainType? TransformResult { get; }

        public int Special { get; set; }    //0...no, 1...special1, 2...special2
        public int Island { get; set; }
        public int SpecialFood1 { get; }
        public int SpecialFood2 { get; }
        public int SpecialShields1 { get; }
        public int SpecialShields2 { get; }
        public int SpecialTrade1 { get; }
        public int SpecialTrade2 { get; }


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

        protected BaseTerrain(int movecost, int defense, int food, int shields, int trade, bool can_irrigate, int irrigate_bonus, int irrigate_settler_turns, int ai_irrigate, bool can_mine, int mine_bonus, int mine_settler_turns, int ai_mine, TerrainType transform)
        {
            Movecost = movecost;
            Defense = defense;
            Food = food;
            Shields = shields;
            Trade = trade;
            CanIrrigate = can_irrigate;
            IrrigationResult = null;
            IrrigationBonus = irrigate_bonus;
            TurnsToIrrigate = irrigate_settler_turns;
            AIirrigation = ai_irrigate;
            CanMine = can_mine;
            MiningResult = null;
            MiningBonus = mine_bonus;
            TurnsToMine = mine_settler_turns;
            AImining = ai_mine;
            if (transform == TerrainType.Ocean) { TransformResult = null; }
            else { TransformResult = transform; }

            //SpecialFood1 = spec_food1;
            //SpecialFood2 = spec_food2;
            //SpecialShields1 = spec_shields1;
            //SpecialShields2 = spec_shields2;
            //SpecialTrade1 = spec_trade1;
            //SpecialTrade2 = spec_trade2;
        }

        protected BaseTerrain(int movecost, int defense, int food, int shields, int trade, TerrainType irrigate_change, int irrigate_bonus, int irrigate_settler_turns, int ai_irrigate, bool can_mine, int mine_bonus, int mine_settler_turns, int ai_mine, TerrainType transform)
        {
            Movecost = movecost;
            Defense = defense;
            Food = food;
            Shields = shields;
            Trade = trade;
            CanIrrigate = true;
            IrrigationResult = irrigate_change;
            IrrigationBonus = irrigate_bonus;
            TurnsToIrrigate = irrigate_settler_turns;
            AIirrigation = ai_irrigate;
            CanMine = can_mine;
            MiningResult = null;
            MiningBonus = mine_bonus;
            TurnsToMine = mine_settler_turns;
            AImining = ai_mine;
            if (transform == TerrainType.Ocean) { TransformResult = null; }
            else { TransformResult = transform; }
        }

        protected BaseTerrain(int movecost, int defense, int food, int shields, int trade, bool can_irrigate, int irrigate_bonus, int irrigate_settler_turns, int ai_irrigate, TerrainType mine_change, int mine_bonus, int mine_settler_turns, int ai_mine, TerrainType transform)
        {
            Movecost = movecost;
            Defense = defense;
            Food = food;
            Shields = shields;
            Trade = trade;
            CanIrrigate = can_irrigate;
            IrrigationResult = null;
            IrrigationBonus = irrigate_bonus;
            TurnsToIrrigate = irrigate_settler_turns;
            AIirrigation = ai_irrigate;
            CanMine = true;
            MiningResult = mine_change;
            MiningBonus = mine_bonus;
            TurnsToMine = mine_settler_turns;
            AImining = ai_mine;
            if (transform == TerrainType.Ocean) { TransformResult = null; }
            else { TransformResult = transform; }
        }

        protected BaseTerrain(int movecost, int defense, int food, int shields, int trade, TerrainType irrigate_change, int irrigate_bonus, int irrigate_settler_turns, int ai_irrigate, TerrainType mine_change, int mine_bonus, int mine_settler_turns, int ai_mine, TerrainType transform)
        {
            Movecost = movecost;
            Defense = defense;
            Food = food;
            Shields = shields;
            Trade = trade;
            CanIrrigate = true;
            IrrigationResult = irrigate_change;
            IrrigationBonus = irrigate_bonus;
            TurnsToIrrigate = irrigate_settler_turns;
            AIirrigation = ai_irrigate;
            CanMine = true;
            MiningResult = mine_change;
            MiningBonus = mine_bonus;
            TurnsToMine = mine_settler_turns;
            AImining = ai_mine;
            if (transform == TerrainType.Ocean) { TransformResult = null; }
            else { TransformResult = transform; }
        }
    }
}
