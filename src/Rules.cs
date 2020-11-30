using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace civ2
{
    public class Rules
    {
        // Game rules from RULES.txt

        // Cosmic rules
        public int RoadMultiplier { get; private set; }
        public int ChanceTriremeLost { get; private set; }
        public int FoodEatenPerTurn { get; private set; }
        public int RowsFoodBox { get; private set; }
        public int RowsShieldBox { get; private set; }
        public int SettlersEatTillMonarchy { get; private set; }
        public int SettlersEatFromCommunism { get; private set; }
        public int CitySizeUnhappyChieftain { get; private set; }
        public int RiotFactor { get; private set; }
        public int ToExceedCitySizeAqueductNeeded { get; private set; }
        public int SewerNeeded { get; private set; }
        public int TechParadigm { get; private set; }
        public int BaseTimeEngineersTransform { get; private set; }
        public int MonarchyPaysSupport { get; private set; }
        public int CommunismPaysSupport { get; private set; }
        public int FundamentalismPaysSupport { get; private set; }
        public int CommunismEquivalentPalaceDistance { get; private set; }
        public int FundamentalismScienceLost { get; private set; }
        public int ShieldPenaltyTypeChange { get; private set; }
        public int MaxParadropRange { get; private set; }
        public int MassThrustParadigm { get; private set; }
        public int MaxEffectiveScienceRate { get; private set; }

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

        // Tech/advances
        public string[] TechName { get; private set; }
        public int[] TechAIvalue { get; private set; }
        public int[] TechModifier { get; private set; }
        public string[] TechPrereq1 { get; private set; }
        public string[] TechPrereq2 { get; private set; }
        public int[] TechEpoch { get; private set; }
        public int[] TechCategory { get; private set; }
        public string[] TechShortName { get; private set; }

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
        public bool[] TerrainCanIrrigate { get; private set; }
        public int[] TerrainIrrigateBonus { get; private set; }
        public int[] TerrainIrrigateTurns { get; private set; }
        public int[] TerrainIrrigateAI { get; private set; }
        public string[] TerrainMine { get; private set; }
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

        public void Set(List<string[]> rulesList)
        {
            // Set cosmic principles
            RoadMultiplier = Int32.Parse(rulesList[0][0]);
            ChanceTriremeLost = Int32.Parse(rulesList[0][1]);
            FoodEatenPerTurn = Int32.Parse(rulesList[0][2]);
            RowsFoodBox = Int32.Parse(rulesList[0][3]);
            RowsShieldBox = Int32.Parse(rulesList[0][4]);
            SettlersEatTillMonarchy = Int32.Parse(rulesList[0][5]);
            SettlersEatFromCommunism = Int32.Parse(rulesList[0][6]);
            CitySizeUnhappyChieftain = Int32.Parse(rulesList[0][7]);
            RiotFactor = Int32.Parse(rulesList[0][8]);
            ToExceedCitySizeAqueductNeeded = Int32.Parse(rulesList[0][9]);
            SewerNeeded = Int32.Parse(rulesList[0][10]);
            TechParadigm = Int32.Parse(rulesList[0][11]);
            BaseTimeEngineersTransform = Int32.Parse(rulesList[0][12]);
            MonarchyPaysSupport = Int32.Parse(rulesList[0][13]);
            CommunismPaysSupport = Int32.Parse(rulesList[0][14]);
            FundamentalismPaysSupport = Int32.Parse(rulesList[0][15]);
            CommunismEquivalentPalaceDistance = Int32.Parse(rulesList[0][16]);
            FundamentalismScienceLost = Int32.Parse(rulesList[0][17]);
            ShieldPenaltyTypeChange = Int32.Parse(rulesList[0][18]);
            MaxParadropRange = Int32.Parse(rulesList[0][19]);
            MassThrustParadigm = Int32.Parse(rulesList[0][20]);
            MaxEffectiveScienceRate = Int32.Parse(rulesList[0][21]);

            for (int row = 0; row < 100; row++)
            {
                TechName[row] = rulesList[1][row];
                TechAIvalue[row] = Int32.Parse(rulesList[2][row]);
                TechModifier[row] = Int32.Parse(rulesList[3][row]);
                TechPrereq1[row] = rulesList[4][row];
                TechPrereq2[row] = rulesList[5][row];
                TechEpoch[row] = Int32.Parse(rulesList[6][row]);
                TechCategory[row] = Int32.Parse(rulesList[7][row]);
                TechShortName[row] = rulesList[8][row];
            }

            for (int row = 0; row < 67; row++)
            {
                ImprovementName[row] = rulesList[9][row];
                ImprovementCost[row] = Int32.Parse(rulesList[10][row]);
                ImprovementUpkeep[row] = Int32.Parse(rulesList[11][row]);
                ImprovementPrereq[row] = rulesList[12][row];
                ImprovementAdvanceExpiration[row] = rulesList[13][row];
            }

            for (int row = 0; row < 62; row++)
            {
                UnitName[row] = rulesList[14][row];
                UnitUntil[row] = rulesList[15][row];
                UnitDomain[row] = Int32.Parse(rulesList[16][row]);
                UnitMove[row] = Int32.Parse(rulesList[17][row]);
                UnitRange[row] = Int32.Parse(rulesList[18][row]);
                UnitAttack[row] = Int32.Parse(rulesList[19][row]);
                UnitDefense[row] = Int32.Parse(rulesList[20][row]);
                UnitHitp[row] = Int32.Parse(rulesList[21][row]);
                UnitFirepwr[row] = Int32.Parse(rulesList[22][row]);
                UnitCost[row] = Int32.Parse(rulesList[23][row]);
                UnitHold[row] = Int32.Parse(rulesList[24][row]);
                UnitAIrole[row] = Int32.Parse(rulesList[25][row]);
                UnitPrereq[row] = rulesList[26][row];
                UnitFlags[row] = rulesList[27][row];
            }

            for (int row = 0; row < 11; row++)
            {
                TerrainName[row] = rulesList[28][row];
                TerrainMovecost[row] = Int32.Parse(rulesList[29][row]);
                TerrainDefense[row] = Int32.Parse(rulesList[30][row]);
                TerrainFood[row] = Int32.Parse(rulesList[31][row]);
                TerrainShields[row] = Int32.Parse(rulesList[32][row]);
                TerrainTrade[row] = Int32.Parse(rulesList[33][row]);
                TerrainCanIrrigate[row] = rulesList[34][row] == "yes";
                TerrainIrrigateBonus[row] = Int32.Parse(rulesList[35][row]);
                TerrainIrrigateTurns[row] = Int32.Parse(rulesList[36][row]);
                TerrainIrrigateAI[row] = Int32.Parse(rulesList[37][row]);
                TerrainMine[row] = rulesList[38][row];
                TerrainMineBonus[row] = Int32.Parse(rulesList[39][row]);
                TerrainMineTurns[row] = Int32.Parse(rulesList[40][row]);
                TerrainMineAI[row] = Int32.Parse(rulesList[41][row]);
                TerrainTransform[row] = rulesList[42][row];
                TerrainShortName[row] = rulesList[43][row];
            }

            for (int row = 0; row < 22; row++)
            {
                TerrainSpecName[row] = rulesList[44][row];
                TerrainSpecMovecost[row] = Int32.Parse(rulesList[45][row]);
                TerrainSpecDefense[row] = Int32.Parse(rulesList[46][row]);
                TerrainSpecFood[row] = Int32.Parse(rulesList[47][row]);
                TerrainSpecShields[row] = Int32.Parse(rulesList[48][row]);
                TerrainSpecTrade[row] = Int32.Parse(rulesList[49][row]);
            }

            for (int row = 0; row < 7; row++)
            {
                GovernmentName[row] = rulesList[50][row];
                GovernmentTitleHIS[row] = rulesList[51][row];
                GovernmentTitleHER[row] = rulesList[52][row];
            }

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

            for (int row = 0; row < 16; row++)
            {
                CaravanCommoditie[row] = rulesList[63][row];
            }

            for (int row = 0; row < 11; row++)
            {
                OrderName[row] = rulesList[64][row];
                OrderShortcut[row] = rulesList[65][row];
            }

            for (int row = 0; row < 6; row++)
            {
                Difficulty[row] = rulesList[66][row];
            }

            for (int row = 0; row < 9; row++)
            {
                Attitude[row] = rulesList[67][row];
            }
        }

    }
}
