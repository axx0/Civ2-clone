using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace civ2
{
    public class Rules
    {
        // Game rules from RULES.txt

        //Cosmic rules
        public static int[] CosmicRules { get; private set; }
        //Units
        public static string[] UnitName { get; private set; }
        public static string[] UnitUntil { get; private set; }
        public static int[] UnitDomain { get; private set; }
        public static int[] UnitMove { get; private set; }
        public static int[] UnitRange { get; private set; }
        public static int[] UnitAttack { get; private set; }
        public static int[] UnitDefense { get; private set; }
        public static int[] UnitHitp { get; private set; }
        public static int[] UnitFirepwr { get; private set; }
        public static int[] UnitCost { get; private set; }
        public static int[] UnitHold { get; private set; }
        public static int[] UnitAIrole { get; private set; }
        public static string[] UnitPrereq { get; private set; }
        public static string[] UnitFlags { get; private set; }
        //Tech/advances
        public static string[] TechName { get; private set; }
        public static int[] TechAIvalue { get; private set; }
        public static int[] TechModifier { get; private set; }
        public static string[] TechPrereq1 { get; private set; }
        public static string[] TechPrereq2 { get; private set; }
        public static int[] TechEpoch { get; private set; }
        public static int[] TechCategory { get; private set; }
        public static string[] TechShortName { get; private set; }
        //City improvements
        public static string[] ImprovementName { get; private set; }
        public static int[] ImprovementCost { get; private set; }
        public static int[] ImprovementUpkeep { get; private set; }
        public static string[] ImprovementPrereq { get; private set; }
        //Expiration of advances
        public static string[] ImprovementAdvanceExpiration { get; private set; }
        //Terrain
        public static string[] TerrainName { get; private set; }
        public static int[] TerrainMovecost { get; private set; }
        public static int[] TerrainDefense { get; private set; }
        public static int[] TerrainFood { get; private set; }
        public static int[] TerrainShields { get; private set; }
        public static int[] TerrainTrade { get; private set; }
        public static string[] TerrainIrrigate { get; private set; }
        public static int[] TerrainIrrigateBonus { get; private set; }
        public static int[] TerrainIrrigateTurns { get; private set; }
        public static int[] TerrainIrrigateAI { get; private set; }
        public static string[] TerrainMine { get; private set; }
        public static int[] TerrainMineBonus { get; private set; }
        public static int[] TerrainMineTurns { get; private set; }
        public static int[] TerrainMineAI { get; private set; }
        public static string[] TerrainTransform { get; private set; }
        public static string[] TerrainShortName { get; private set; }
        //Special terrain
        public static string[] TerrainSpecName { get; private set; }
        public static int[] TerrainSpecMovecost { get; private set; }
        public static int[] TerrainSpecDefense { get; private set; }
        public static int[] TerrainSpecFood { get; private set; }
        public static int[] TerrainSpecShields { get; private set; }
        public static int[] TerrainSpecTrade { get; private set; }
        //Governments
        public static string[] GovernmentName { get; private set; }
        public static string[] GovernmentTitleHIS { get; private set; }
        public static string[] GovernmentTitleHER { get; private set; }
        //Leaders
        public static string[] LeaderNameHIS { get; private set; }
        public static string[] LeaderNameHER { get; private set; }
        public static int[] LeaderFemale { get; private set; }
        public static int[] LeaderColor { get; private set; }
        public static int[] LeaderCityStyle { get; private set; }
        public static string[] LeaderPlural { get; private set; }
        public static string[] LeaderAdjective { get; private set; }
        public static int[] LeaderAttack { get; private set; }
        public static int[] LeaderExpand { get; private set; }
        public static int[] LeaderCivilize { get; private set; }
        //Trading commodities
        public static string[] CaravanCommoditie { get; private set; }
        //Orders
        public static string[] OrderName { get; private set; }
        public static string[] OrderShortcut { get; private set; }
        //Difficulty
        public static string[] Difficulty { get; private set; }
        //Attitudes
        public static string[] Attitude { get; private set; }

        public static void ReadRULES(string filePath)
        {
            // Initialize
            CosmicRules = new int[22];
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
                //Read COSMIC PRINCIPLES
                if (line == "@COSMIC")
                {
                    for (int row = 0; row < 22; row++)
                    {
                        line = file.ReadLine();
                        List<string> text = line.Split(';').ToList();
                        CosmicRules[row] = Int32.Parse(text[0].Trim());
                    }
                }

                //Read TECH RULES
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

                //Read IMPROVEMENTS
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

                //Read EXPIRATION OF ADVANCES
                if (line == "@ENDWONDER")
                {
                    //First 39 are city improvements, they have no expiration
                    for (int row = 0; row < 39; row++) { ImprovementAdvanceExpiration[row] = ""; }

                    //Next 28 are advances
                    for (int row = 0; row < 28; row++)  //for advances
                    {
                        line = file.ReadLine();
                        List<string> text = line.Split(',').ToList();
                        ImprovementAdvanceExpiration[row + 39] = text[0];
                    }
                }

                //Read UNIT RULES
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

                //Read TERRAIN RULES
                if (line == "@TERRAIN")
                {
                    //First read normal terrain
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

                    //Next read special terrain
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

                //Read GOVERNMENTS
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

                //Read LEADERS
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

                //Read CARAVAN TRADING COMMODITIES
                if (line == "@CARAVAN")
                {
                    for (int row = 0; row < 16; row++)
                    {
                        line = file.ReadLine();
                        List<string> text = line.Split(',').ToList();
                        CaravanCommoditie[row] = text[0].Trim();
                    }
                }

                //Read ORDERS
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

                //Read DIFFICULTY
                if (line == "@DIFFICULTY")
                {
                    for (int row = 0; row < 6; row++)
                    {
                        line = file.ReadLine();
                        Difficulty[row] = line;
                    }
                }

                //Read ATTITUDES
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
