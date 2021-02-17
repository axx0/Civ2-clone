using System;
using System.Collections.Generic;

namespace Civ2engine
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

        // Advances
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
        public string[] TerrainSpec1Name { get; private set; }
        public string[] TerrainSpec2Name { get; private set; }
        public int[] TerrainSpec1Movecost { get; private set; }
        public int[] TerrainSpec2Movecost { get; private set; }
        public int[] TerrainSpec1Defense { get; private set; }
        public int[] TerrainSpec2Defense { get; private set; }
        public int[] TerrainSpec1Food { get; private set; }
        public int[] TerrainSpec2Food { get; private set; }
        public int[] TerrainSpec1Shields { get; private set; }
        public int[] TerrainSpec2Shields { get; private set; }
        public int[] TerrainSpec1Trade { get; private set; }
        public int[] TerrainSpec2Trade { get; private set; }

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

            AdvanceName = new string[100];
            AdvanceAIvalue = new int[100];
            AdvanceModifier = new int[100];
            AdvancePrereq1 = new string[100];
            AdvancePrereq2 = new string[100];
            AdvanceEpoch = new int[100];
            AdvanceCategory = new int[100];
            AdvanceShortName = new string[100];
            for (int row = 0; row < 100; row++)
            {
                AdvanceName[row] = rulesList[1][row];
                AdvanceAIvalue[row] = Int32.Parse(rulesList[2][row]);
                AdvanceModifier[row] = Int32.Parse(rulesList[3][row]);
                AdvancePrereq1[row] = rulesList[4][row];
                AdvancePrereq2[row] = rulesList[5][row];
                AdvanceEpoch[row] = Int32.Parse(rulesList[6][row]);
                AdvanceCategory[row] = Int32.Parse(rulesList[7][row]);
                AdvanceShortName[row] = rulesList[8][row];
            }

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
                UnitMove[row] = 3 * Int32.Parse(rulesList[17][row]);
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

            TerrainSpec1Name = new string[11];
            TerrainSpec1Movecost = new int[11];
            TerrainSpec1Defense = new int[11];
            TerrainSpec1Food = new int[11];
            TerrainSpec1Shields = new int[11];
            TerrainSpec1Trade = new int[11];
            TerrainSpec2Name = new string[11];
            TerrainSpec2Movecost = new int[11];
            TerrainSpec2Defense = new int[11];
            TerrainSpec2Food = new int[11];
            TerrainSpec2Shields = new int[11];
            TerrainSpec2Trade = new int[11];
            for (int row = 0; row < 11; row++)
            {
                TerrainSpec1Name[row] = rulesList[44][row];
                TerrainSpec1Movecost[row] = Int32.Parse(rulesList[45][row]);
                TerrainSpec1Defense[row] = Int32.Parse(rulesList[46][row]);
                TerrainSpec1Food[row] = Int32.Parse(rulesList[47][row]);
                TerrainSpec1Shields[row] = Int32.Parse(rulesList[48][row]);
                TerrainSpec1Trade[row] = Int32.Parse(rulesList[49][row]);

                TerrainSpec2Name[row] = rulesList[50][row];
                TerrainSpec2Movecost[row] = Int32.Parse(rulesList[51][row]);
                TerrainSpec2Defense[row] = Int32.Parse(rulesList[52][row]);
                TerrainSpec2Food[row] = Int32.Parse(rulesList[53][row]);
                TerrainSpec2Shields[row] = Int32.Parse(rulesList[54][row]);
                TerrainSpec2Trade[row] = Int32.Parse(rulesList[55][row]);
            }

            GovernmentName = new string[7];
            GovernmentTitleHIS = new string[7];
            GovernmentTitleHER = new string[7];
            for (int row = 0; row < 7; row++)
            {
                GovernmentName[row] = rulesList[56][row];
                GovernmentTitleHIS[row] = rulesList[57][row];
                GovernmentTitleHER[row] = rulesList[58][row];
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
                LeaderNameHIS[row] = rulesList[59][row];
                LeaderNameHER[row] = rulesList[60][row];
                LeaderFemale[row] = Int32.Parse(rulesList[61][row]);
                LeaderColor[row] = Int32.Parse(rulesList[62][row]);
                LeaderCityStyle[row] = Int32.Parse(rulesList[63][row]);
                LeaderPlural[row] = rulesList[64][row];
                LeaderAdjective[row] = rulesList[65][row];
                LeaderAttack[row] = Int32.Parse(rulesList[66][row]);
                LeaderExpand[row] = Int32.Parse(rulesList[67][row]);
                LeaderCivilize[row] = Int32.Parse(rulesList[68][row]);
            }

            CaravanCommoditie = new string[16];
            for (int row = 0; row < 16; row++)
            {
                CaravanCommoditie[row] = rulesList[69][row];
            }

            OrderName = new string[70];
            OrderShortcut = new string[70];
            for (int row = 0; row < 11; row++)
            {
                OrderName[row] = rulesList[70][row];
                OrderShortcut[row] = rulesList[71][row];
            }

            Difficulty = new string[6];
            for (int row = 0; row < 6; row++)
            {
                Difficulty[row] = rulesList[72][row];
            }

            Attitude = new string[9];
            for (int row = 0; row < 9; row++)
            {
                Attitude[row] = rulesList[73][row];
            }
        }
    }
}
