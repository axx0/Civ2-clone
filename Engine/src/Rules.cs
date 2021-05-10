using System;
using System.Collections.Generic;
using Civ2engine.Advances;
using Civ2engine.Improvements;
using Civ2engine.Terrains;
using Civ2engine.Units;

namespace Civ2engine
{
    public class Rules
    {
        // Game rules from RULES.txt

        // Cosmic rules
        public CosmicRules Cosmic { get; } = new();

        // Units
        public string[] UnitName { get; set; }
        public string[] UnitUntil { get; private set; }
        public int[] UnitDomain { get; private set; }
        public int[] UnitMove { get; private set; }
        public int[] UnitRange { get; private set; }
        public int[] UnitAttack { get; private set; }
        public int[] UnitDefense { get; private set; }
        public int[] UnitHitp { get; private set; }
        public int[] UnitFirepwr { get; private set; }
        public int[] UnitCost { get; private set; }
        public int[] UnitHold { get; private set; }
        public int[] UnitAIrole { get; private set; }
        public string[] UnitPrereq { get; private set; }
        public string[] UnitFlags { get; private set; }

        // Advances
        public Advance[] Advances { get; set; }

        public Dictionary<string, int> AdvanceMappings = new()
        {
            {"nil", -1},
            {"no", -2}
        };
        public string[] AdvanceName { get; private set; }
        public int[] AdvanceAIvalue { get; private set; }
        public int[] AdvanceModifier { get; private set; }
        public string[] AdvancePrereq1 { get; private set; }
        public string[] AdvancePrereq2 { get; private set; }
        public int[] AdvanceEpoch { get; private set; }
        public int[] AdvanceCategory { get; private set; }
        public string[] AdvanceShortName { get; private set; }

        // City improvements
        public string[] ImprovementName { get; private set; }
        public int[] ImprovementCost { get; private set; }
        public int[] ImprovementUpkeep { get; private set; }
        public string[] ImprovementPrereq { get; private set; }

        // Expiration of advances
        public string[] ImprovementAdvanceExpiration { get; private set; }

        // Terrain
        public string[] TerrainName { get; private set; }
        public int[] TerrainMovecost { get; private set; }
        public int[] TerrainDefense { get; private set; }
        public int[] TerrainFood { get; private set; }
        public int[] TerrainShields { get; private set; }
        public int[] TerrainTrade { get; private set; }
        public string[] TerrainCanIrrigate { get; private set; }
        public int[] TerrainIrrigateBonus { get; private set; }
        public int[] TerrainIrrigateTurns { get; private set; }
        public int[] TerrainIrrigateAI { get; private set; }
        public string[] TerrainCanMine { get; private set; }
        public int[] TerrainMineBonus { get; private set; }
        public int[] TerrainMineTurns { get; private set; }
        public int[] TerrainMineAI { get; private set; }
        public string[] TerrainTransform { get; private set; }
        public string[] TerrainShortName { get; private set; }

        // Special terrain
        public string[] TerrainSpecName { get; private set; }
        public int[] TerrainSpecMovecost { get; private set; }
        public int[] TerrainSpecDefense { get; private set; }
        public int[] TerrainSpecFood { get; private set; }
        public int[] TerrainSpecShields { get; private set; }
        public int[] TerrainSpecTrade { get; private set; }

        // Governments
        public string[] GovernmentName { get; private set; }
        public string[] GovernmentTitleHIS { get; private set; }
        public string[] GovernmentTitleHER { get; private set; }

        // Leaders
        public string[] LeaderNameHIS { get; private set; }
        public string[] LeaderNameHER { get; private set; }
        public int[] LeaderFemale { get; private set; }
        public int[] LeaderColor { get; private set; }
        public int[] LeaderCityStyle { get; private set; }
        public string[] LeaderPlural { get; private set; }
        public string[] LeaderAdjective { get; private set; }
        public int[] LeaderAttack { get; private set; }
        public int[] LeaderExpand { get; private set; }
        public int[] LeaderCivilize { get; private set; }

        // Trading commodities
        public string[] CaravanCommoditie { get; private set; }

        // Orders
        public string[] OrderName { get; private set; }
        public string[] OrderShortcut { get; private set; }

        // Difficulty
        public string[] Difficulty { get; private set; }

        // Attitudes
        public string[] Attitude { get; private set; }
        public Improvement[] Improvements { get; set; }
        public UnitDefinition[] UnitTypes { get; set; }
        public Terrain[] Terrains { get; set; }

        public void Set(List<string[]> rulesList)
        {
            ImprovementName = new string[67];
            ImprovementCost = new int[67];
            ImprovementUpkeep = new int[67];
            ImprovementPrereq = new string[67];
            ImprovementAdvanceExpiration = new string[67];
            for (int row = 0; row < 67; row++)
            {
                ImprovementName[row] = rulesList[9][row];
                ImprovementCost[row] = Int32.Parse(rulesList[10][row]);
                ImprovementUpkeep[row] = Int32.Parse(rulesList[11][row]);
                ImprovementPrereq[row] = rulesList[12][row];
                ImprovementAdvanceExpiration[row] = rulesList[13][row];
            }

            UnitName = new string[62];
            UnitUntil = new string[62];
            UnitDomain = new int[62];
            UnitMove = new int[62];
            UnitRange = new int[62];
            UnitAttack = new int[62];
            UnitDefense = new int[62];
            UnitHitp = new int[62];
            UnitFirepwr = new int[62];
            UnitCost = new int[62];
            UnitHold = new int[62];
            UnitAIrole = new int[62];
            UnitPrereq = new string[62];
            UnitFlags = new string[62];
            for (int row = 0; row < 62; row++)
            {
                UnitName[row] = rulesList[14][row];
                UnitUntil[row] = rulesList[15][row];
                UnitDomain[row] = Int32.Parse(rulesList[16][row]);
                UnitMove[row] = Cosmic.RoadMultiplier * Int32.Parse(rulesList[17][row]);
                UnitRange[row] = Int32.Parse(rulesList[18][row]);
                UnitAttack[row] = Int32.Parse(rulesList[19][row]);
                UnitDefense[row] = Int32.Parse(rulesList[20][row]);
                UnitHitp[row] = 10 * Int32.Parse(rulesList[21][row]);
                UnitFirepwr[row] = Int32.Parse(rulesList[22][row]);
                UnitCost[row] = Int32.Parse(rulesList[23][row]);
                UnitHold[row] = Int32.Parse(rulesList[24][row]);
                UnitAIrole[row] = Int32.Parse(rulesList[25][row]);
                UnitPrereq[row] = rulesList[26][row];
                UnitFlags[row] = rulesList[27][row];
            }

            TerrainName = new string[11];
            TerrainMovecost = new int[11];
            TerrainDefense = new int[11];
            TerrainFood = new int[11];
            TerrainShields = new int[11];
            TerrainTrade = new int[11];
            TerrainCanIrrigate = new string[11];
            TerrainIrrigateBonus = new int[11];
            TerrainIrrigateTurns = new int[11];
            TerrainIrrigateAI = new int[11];
            TerrainCanMine = new string[11];
            TerrainMineBonus = new int[11];
            TerrainMineTurns = new int[11];
            TerrainMineAI = new int[11];
            TerrainTransform = new string[11];
            TerrainShortName = new string[11];
            for (int row = 0; row < 11; row++)
            {
                TerrainName[row] = rulesList[28][row];
                TerrainMovecost[row] = Int32.Parse(rulesList[29][row]);
                TerrainDefense[row] = Int32.Parse(rulesList[30][row]);
                TerrainFood[row] = Int32.Parse(rulesList[31][row]);
                TerrainShields[row] = Int32.Parse(rulesList[32][row]);
                TerrainTrade[row] = Int32.Parse(rulesList[33][row]);
                TerrainCanIrrigate[row] = rulesList[34][row];
                TerrainIrrigateBonus[row] = Int32.Parse(rulesList[35][row]);
                TerrainIrrigateTurns[row] = Int32.Parse(rulesList[36][row]);
                TerrainIrrigateAI[row] = Int32.Parse(rulesList[37][row]);
                TerrainCanMine[row] = rulesList[38][row];
                TerrainMineBonus[row] = Int32.Parse(rulesList[39][row]);
                TerrainMineTurns[row] = Int32.Parse(rulesList[40][row]);
                TerrainMineAI[row] = Int32.Parse(rulesList[41][row]);
                TerrainTransform[row] = rulesList[42][row];
                TerrainShortName[row] = rulesList[43][row];
            }

            TerrainSpecName = new string[22];
            TerrainSpecMovecost = new int[22];
            TerrainSpecDefense = new int[22];
            TerrainSpecFood = new int[22];
            TerrainSpecShields = new int[22];
            TerrainSpecTrade = new int[22];
            for (int row = 0; row < 22; row++)
            {
                TerrainSpecName[row] = rulesList[44][row];
                TerrainSpecMovecost[row] = Int32.Parse(rulesList[45][row]);
                TerrainSpecDefense[row] = Int32.Parse(rulesList[46][row]);
                TerrainSpecFood[row] = Int32.Parse(rulesList[47][row]);
                TerrainSpecShields[row] = Int32.Parse(rulesList[48][row]);
                TerrainSpecTrade[row] = Int32.Parse(rulesList[49][row]);
            }

            GovernmentName = new string[7];
            GovernmentTitleHIS = new string[7];
            GovernmentTitleHER = new string[7];
            for (int row = 0; row < 7; row++)
            {
                GovernmentName[row] = rulesList[50][row];
                GovernmentTitleHIS[row] = rulesList[51][row];
                GovernmentTitleHER[row] = rulesList[52][row];
            }

            LeaderNameHIS = new string[21];
            LeaderNameHER = new string[21];
            LeaderFemale = new int[21];
            LeaderColor = new int[21];
            LeaderCityStyle = new int[21];
            LeaderPlural = new string[21];
            LeaderAdjective = new string[21];
            LeaderAttack = new int[21];
            LeaderExpand = new int[21];
            LeaderCivilize = new int[21];
            for (int row = 0; row < 21; row++)
            {
                LeaderNameHIS[row] = rulesList[53][row];
                LeaderNameHER[row] = rulesList[54][row];
                LeaderFemale[row] = Int32.Parse(rulesList[55][row]);
                LeaderColor[row] = Int32.Parse(rulesList[56][row]);
                LeaderCityStyle[row] = Int32.Parse(rulesList[57][row]);
                LeaderPlural[row] = rulesList[58][row];
                LeaderAdjective[row] = rulesList[59][row];
                LeaderAttack[row] = Int32.Parse(rulesList[60][row]);
                LeaderExpand[row] = Int32.Parse(rulesList[61][row]);
                LeaderCivilize[row] = Int32.Parse(rulesList[62][row]);
            }

            CaravanCommoditie = new string[16];
            for (int row = 0; row < 16; row++)
            {
                CaravanCommoditie[row] = rulesList[63][row];
            }

            OrderName = new string[70];
            OrderShortcut = new string[70];
            for (int row = 0; row < 11; row++)
            {
                OrderName[row] = rulesList[64][row];
                OrderShortcut[row] = rulesList[65][row];
            }

            Difficulty = new string[6];
            for (int row = 0; row < 6; row++)
            {
                Difficulty[row] = rulesList[66][row];
            }

            Attitude = new string[9];
            for (int row = 0; row < 9; row++)
            {
                Attitude[row] = rulesList[67][row];
            }
        }
    }
}
