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
        public string[] TerrainIrrigate { get; private set; }
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

        public void ReadRULES(string path)
        {
            // Read in SAV directory path. If it doesn't exist there, read from root civ2 directory.
            string rulesPath1 = path + "\\RULES.TXT";
            string rulesPath2 = Settings.Civ2Path + "RULES.TXT";
            string filePath = null;
            if (File.Exists(rulesPath1))
            {
                filePath = rulesPath1;
            }
            else if (File.Exists(rulesPath2))
            {
                filePath = rulesPath2;
            }
            else
            {
                Console.WriteLine("RULES.TXT not found!");
            }

            // Initialize
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
            TechName = new string[100];
            TechAIvalue = new int[100];
            TechModifier = new int[100];
            TechPrereq1 = new string[100];
            TechPrereq2 = new string[100];
            TechEpoch = new int[100];
            TechCategory = new int[100];
            TechShortName = new string[100];
            ImprovementName = new string[67];
            ImprovementCost = new int[67];
            ImprovementUpkeep = new int[67];
            ImprovementPrereq = new string[67];
            ImprovementAdvanceExpiration = new string[67];
            TerrainName = new string[11];
            TerrainMovecost = new int[11];
            TerrainDefense = new int[11];
            TerrainFood = new int[11];
            TerrainShields = new int[11];
            TerrainTrade = new int[11];
            TerrainIrrigate = new string[11];
            TerrainIrrigateBonus = new int[11];
            TerrainIrrigateTurns = new int[11];
            TerrainIrrigateAI = new int[11];
            TerrainMine = new string[11];
            TerrainMineBonus = new int[11];
            TerrainMineTurns = new int[11];
            TerrainMineAI = new int[11];
            TerrainTransform = new string[11];
            TerrainShortName = new string[11];
            TerrainSpecName = new string[22];
            TerrainSpecMovecost = new int[22];
            TerrainSpecDefense = new int[22];
            TerrainSpecFood = new int[22];
            TerrainSpecShields = new int[22];
            TerrainSpecTrade = new int[22];
            GovernmentName = new string[7];
            GovernmentTitleHIS = new string[7];
            GovernmentTitleHER = new string[7];
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
            CaravanCommoditie = new string[16];
            OrderName = new string[11];
            OrderShortcut = new string[11];
            Difficulty = new string[6];
            Attitude = new string[9];

            string line;
            
            // Read the file and display it line by line.  
            StreamReader file = new StreamReader(filePath);
            while ((line = file.ReadLine()) != null)
            {
                // Read COSMIC PRINCIPLES
                if (line == "@COSMIC")
                {
                    List<string> text;                    
                    text = file.ReadLine().Split(';').ToList();
                    RoadMultiplier = Int32.Parse(text[0].Trim());       // Cosmic rule #1
                    text = file.ReadLine().Split(';').ToList();
                    ChanceTriremeLost = Int32.Parse(text[0].Trim());    // Cosmic rule #2
                    text = file.ReadLine().Split(';').ToList();
                    FoodEatenPerTurn = Int32.Parse(text[0].Trim());    // Cosmic rule #3
                    text = file.ReadLine().Split(';').ToList();
                    RowsFoodBox = Int32.Parse(text[0].Trim());    // Cosmic rule #4
                    text = file.ReadLine().Split(';').ToList();
                    RowsShieldBox = Int32.Parse(text[0].Trim());    // Cosmic rule #5
                    text = file.ReadLine().Split(';').ToList();
                    SettlersEatTillMonarchy = Int32.Parse(text[0].Trim());    // Cosmic rule #6
                    text = file.ReadLine().Split(';').ToList();
                    SettlersEatFromCommunism = Int32.Parse(text[0].Trim());    // Cosmic rule #7
                    text = file.ReadLine().Split(';').ToList();
                    CitySizeUnhappyChieftain = Int32.Parse(text[0].Trim());    // Cosmic rule #8
                    text = file.ReadLine().Split(';').ToList();
                    RiotFactor = Int32.Parse(text[0].Trim());    // Cosmic rule #9
                    text = file.ReadLine().Split(';').ToList();
                    ToExceedCitySizeAqueductNeeded = Int32.Parse(text[0].Trim());    // Cosmic rule #10
                    text = file.ReadLine().Split(';').ToList();
                    SewerNeeded = Int32.Parse(text[0].Trim());    // Cosmic rule #11
                    text = file.ReadLine().Split(';').ToList();
                    TechParadigm = Int32.Parse(text[0].Trim());    // Cosmic rule #12
                    text = file.ReadLine().Split(';').ToList();
                    BaseTimeEngineersTransform = Int32.Parse(text[0].Trim());    // Cosmic rule #13
                    text = file.ReadLine().Split(';').ToList();
                    MonarchyPaysSupport = Int32.Parse(text[0].Trim());    // Cosmic rule #14
                    text = file.ReadLine().Split(';').ToList();
                    CommunismPaysSupport = Int32.Parse(text[0].Trim());    // Cosmic rule #15
                    text = file.ReadLine().Split(';').ToList();
                    FundamentalismPaysSupport = Int32.Parse(text[0].Trim());    // Cosmic rule #16
                    text = file.ReadLine().Split(';').ToList();
                    CommunismEquivalentPalaceDistance = Int32.Parse(text[0].Trim());    // Cosmic rule #17
                    text = file.ReadLine().Split(';').ToList();
                    FundamentalismScienceLost = Int32.Parse(text[0].Trim());    // Cosmic rule #18
                    text = file.ReadLine().Split(';').ToList();
                    ShieldPenaltyTypeChange = Int32.Parse(text[0].Trim());    // Cosmic rule #19
                    text = file.ReadLine().Split(';').ToList();
                    MaxParadropRange = Int32.Parse(text[0].Trim());    // Cosmic rule #20
                    text = file.ReadLine().Split(';').ToList();
                    MassThrustParadigm = Int32.Parse(text[0].Trim());    // Cosmic rule #21
                    text = file.ReadLine().Split(';').ToList();
                    MaxEffectiveScienceRate = Int32.Parse(text[0].Trim());    // Cosmic rule #22
                }

                // Read TECH RULES
                if (line == "@CIVILIZE")
                {
                    for (int row = 0; row < 100; row++)
                    {
                        line = file.ReadLine();
                        List<string> text = line.Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                        TechName[row] = text[0];
                        TechAIvalue[row] = Int32.Parse(text[1].Trim());
                        TechModifier[row] = Int32.Parse(text[2].Trim());
                        TechPrereq1[row] = text[3].Trim();
                        TechPrereq2[row] = text[4].Trim();
                        TechEpoch[row] = Int32.Parse(text[5].Trim());
                        TechCategory[row] = Int32.Parse(text[6].Trim());
                        TechShortName[row] = text[7].Trim();
                    }
                }

                // Read IMPROVEMENTS
                if (line == "@IMPROVE")
                {
                    for (int row = 0; row < 67; row++)
                    {
                        line = file.ReadLine();
                        List<string> text = line.Split(',').ToList();
                        ImprovementName[row] = text[0];
                        ImprovementCost[row] = Int32.Parse(text[1].Trim());
                        ImprovementUpkeep[row] = Int32.Parse(text[2].Trim());
                        ImprovementPrereq[row] = text[3].Trim();
                    }
                }

                // Read EXPIRATION OF ADVANCES
                if (line == "@ENDWONDER")
                {
                    // First 39 are city improvements, they have no expiration
                    for (int row = 0; row < 39; row++) 
                        ImprovementAdvanceExpiration[row] = "";

                    // Next 28 are advances
                    for (int row = 0; row < 28; row++)  //for advances
                    {
                        line = file.ReadLine();
                        List<string> text = line.Split(',').ToList();
                        ImprovementAdvanceExpiration[row + 39] = text[0];
                    }
                }

                // Read UNIT RULES
                if (line == "@UNITS")
                {
                    for (int row = 0; row < 62; row++)
                    {
                        line = file.ReadLine();
                        List<string> text = line.Split(',').ToList();
                        UnitName[row] = text[0];
                        UnitUntil[row] = text[1];
                        UnitDomain[row] = Int32.Parse(text[2].Trim());
                        UnitMove[row] = Int32.Parse(text[3].Trim().Replace(".", string.Empty));
                        UnitRange[row] = Int32.Parse(text[4].Trim());
                        UnitAttack[row] = Int32.Parse(text[5].Trim().Replace("a", string.Empty));
                        UnitDefense[row] = Int32.Parse(text[6].Trim().Replace("d", string.Empty));
                        UnitHitp[row] = Int32.Parse(text[7].Trim().Replace("h", string.Empty));
                        UnitFirepwr[row] = Int32.Parse(text[8].Trim().Replace("f", string.Empty));
                        UnitCost[row] = Int32.Parse(text[9].Trim());
                        UnitHold[row] = Int32.Parse(text[10].Trim());
                        UnitAIrole[row] = Int32.Parse(text[11].Trim());
                        UnitPrereq[row] = text[12];
                        UnitFlags[row] = text[13];
                    }
                }

                // Read TERRAIN RULES
                if (line == "@TERRAIN")
                {
                    // First read normal terrain
                    for (int row = 0; row < 11; row++)
                    {
                        line = file.ReadLine();
                        List<string> text = line.Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                        TerrainName[row] = text[0].Trim();
                        TerrainMovecost[row] = Int32.Parse(text[1].Trim());
                        TerrainDefense[row] = Int32.Parse(text[2].Trim());
                        TerrainFood[row] = Int32.Parse(text[3].Trim());
                        TerrainShields[row] = Int32.Parse(text[4].Trim());
                        TerrainTrade[row] = Int32.Parse(text[5].Trim());
                        TerrainIrrigate[row] = text[6].Trim();
                        TerrainIrrigateBonus[row] = Int32.Parse(text[7].Trim());
                        TerrainIrrigateTurns[row] = Int32.Parse(text[8].Trim());
                        TerrainIrrigateAI[row] = Int32.Parse(text[9].Trim());
                        TerrainMine[row] = text[10].Trim();
                        TerrainMineBonus[row] = Int32.Parse(text[11].Trim());
                        TerrainMineTurns[row] = Int32.Parse(text[12].Trim());
                        TerrainMineAI[row] = Int32.Parse(text[13].Trim());
                        TerrainTransform[row] = text[14].Trim();
                        TerrainShortName[row] = text[16].Trim();
                    }

                    // Next read special terrain
                    for (int row = 0; row < 22; row++)
                    {
                        line = file.ReadLine();
                        List<string> text = line.Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                        TerrainSpecName[row] = text[0];
                        TerrainSpecMovecost[row] = Int32.Parse(text[1].Trim());
                        TerrainSpecDefense[row] = Int32.Parse(text[2].Trim());
                        TerrainSpecFood[row] = Int32.Parse(text[3].Trim());
                        TerrainSpecShields[row] = Int32.Parse(text[4].Trim());
                        TerrainSpecTrade[row] = Int32.Parse(text[5].Trim());
                    }
                }

                // Read GOVERNMENTS
                if (line == "@GOVERNMENTS")
                {
                    for (int row = 0; row < 7; row++)
                    {
                        line = file.ReadLine();
                        List<string> text = line.Split(',').ToList();
                        GovernmentName[row] = text[0].Trim();
                        GovernmentTitleHIS[row] = text[1].Trim();
                        GovernmentTitleHER[row] = text[2].Trim();
                    }
                }

                // Read LEADERS
                if (line == "@LEADERS")
                {
                    for (int row = 0; row < 21; row++)
                    {
                        line = file.ReadLine();
                        List<string> text = line.Split(',').ToList();
                        LeaderNameHIS[row] = text[0].Trim();
                        LeaderNameHER[row] = text[1].Trim();
                        LeaderFemale[row] = Int32.Parse(text[2].Trim());
                        LeaderColor[row] = Int32.Parse(text[3].Trim());
                        LeaderCityStyle[row] = Int32.Parse(text[4].Trim());
                        LeaderPlural[row] = text[5].Trim();
                        LeaderAdjective[row] = text[6].Trim();
                        LeaderAttack[row] = Int32.Parse(text[7].Trim());
                        LeaderExpand[row] = Int32.Parse(text[8].Trim());
                        LeaderCivilize[row] = Int32.Parse(text[9].Trim());
                    }
                }

                // Read CARAVAN TRADING COMMODITIES
                if (line == "@CARAVAN")
                {
                    for (int row = 0; row < 16; row++)
                    {
                        line = file.ReadLine();
                        List<string> text = line.Split(',').ToList();
                        CaravanCommoditie[row] = text[0].Trim();
                    }
                }

                // Read ORDERS
                if (line == "@ORDERS")
                {
                    for (int row = 0; row < 11; row++)
                    {
                        line = file.ReadLine();
                        List<string> text = line.Split(',').ToList();
                        OrderName[row] = text[0];
                        OrderShortcut[row] = text[1].Trim();
                    }
                }

                // Read DIFFICULTY
                if (line == "@DIFFICULTY")
                {
                    for (int row = 0; row < 6; row++)
                    {
                        line = file.ReadLine();
                        Difficulty[row] = line;
                    }
                }

                // Read ATTITUDES
                if (line == "@ATTITUDES")
                {
                    for (int row = 0; row < 9; row++)
                    {
                        line = file.ReadLine();
                        Attitude[row] = line;
                    }
                }
            }

            file.Close();
        }
    }
}
