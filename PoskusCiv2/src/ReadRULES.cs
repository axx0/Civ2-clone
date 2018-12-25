using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace PoskusCiv2
{
    class ReadFiles
    {
        //Cosmic rules
        public static int[] CosmicRules = new int[22];
        //Units
        public static string[] UnitName = new string[62];
        public static string[] UnitUntil = new string[62];
        public static int[] UnitDomain = new int[62];
        public static int[] UnitMove = new int[62];
        public static int[] UnitRange = new int[62];
        public static int[] UnitAttack = new int[62];
        public static int[] UnitDefense = new int[62];
        public static int[] UnitHitp = new int[62];
        public static int[] UnitFirepwr = new int[62];
        public static int[] UnitCost = new int[62];
        public static int[] UnitHold = new int[62];
        public static int[] UnitAIrole = new int[62];
        public static string[] UnitPrereq = new string[62];
        public static string[] UnitFlags = new string[62];
        //Tech/advances
        public static string[] TechName = new string[100];
        public static int[] TechAIvalue = new int[100];
        public static int[] TechModifier = new int[100];
        public static string[] TechPrereq1 = new string[100];
        public static string[] TechPrereq2 = new string[100];
        public static int[] TechEpoch = new int[100];
        public static int[] TechCategory = new int[100];
        public static string[] TechShortName = new string[100];
        //City improvements
        public static string[] ImprovementName = new string[67];
        public static int[] ImprovementCost = new int[67];
        public static int[] ImprovementUpkeep = new int[67];
        public static string[] ImprovementPrereq = new string[67];
        //Expiration of advances
        public static string[] ImprovementAdvanceExpiration = new string[67];
        //Terrain
        public static string[] TerrainName = new string[11];
        public static int[] TerrainMovecost = new int[11];
        public static int[] TerrainDefense = new int[11];
        public static int[] TerrainFood = new int[11];
        public static int[] TerrainShields = new int[11];
        public static int[] TerrainTrade = new int[11];
        public static string[] TerrainIrrigate = new string[11];
        public static int[] TerrainIrrigateBonus = new int[11];
        public static int[] TerrainIrrigateTurns = new int[11];
        public static int[] TerrainIrrigateAI = new int[11];
        public static string[] TerrainMine = new string[11];
        public static int[] TerrainMineBonus = new int[11];
        public static int[] TerrainMineTurns = new int[11];
        public static int[] TerrainMineAI = new int[11];
        public static string[] TerrainTransform = new string[11];
        public static string[] TerrainShortName = new string[11];
        //Special terrain
        public static string[] TerrainSpecName = new string[22];
        public static int[] TerrainSpecMovecost = new int[22];
        public static int[] TerrainSpecDefense = new int[22];
        public static int[] TerrainSpecFood = new int[22];
        public static int[] TerrainSpecShields = new int[22];
        public static int[] TerrainSpecTrade = new int[22];
        //Governments
        public static string[] GovernmentName = new string[7];
        public static string[] GovernmentTitleHIS = new string[7];
        public static string[] GovernmentTitleHER = new string[7];
        //Leaders
        public static string[] LeaderNameHIS = new string[21];
        public static string[] LeaderNameHER = new string[21];
        public static int[] LeaderFemale = new int[21];
        public static int[] LeaderColor = new int[21];
        public static int[] LeaderCityStyle = new int[21];
        public static string[] LeaderPlural = new string[21];
        public static string[] LeaderAdjective = new string[21];
        public static int[] LeaderAttack = new int[21];
        public static int[] LeaderExpand = new int[21];
        public static int[] LeaderCivilize = new int[21];
        //Trading commodities
        public static string[] CaravanCommoditie = new string[16];
        //Orders
        public static string[] OrderName = new string[11];
        public static string[] OrderShortcut = new string[11];
        //Difficulty
        public static string[] Difficulty = new string[6];
        //Attitudes
        public static string[] Attitude = new string[9];

        public static void ReadRULES(string filePath)
        {
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
                        CosmicRules[row] = Int32.Parse(text[0].Replace(" ", string.Empty));
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
                        TechAIvalue[row] = Int32.Parse(text[1].Replace(" ", string.Empty));
                        TechModifier[row] = Int32.Parse(text[2].Replace(" ", string.Empty));
                        TechPrereq1[row] = text[3].Replace(" ", string.Empty);
                        TechPrereq2[row] = text[4].Replace(" ", string.Empty);
                        TechEpoch[row] = Int32.Parse(text[5].Replace(" ", string.Empty));
                        TechCategory[row] = Int32.Parse(text[6].Replace(" ", string.Empty));
                        TechShortName[row] = text[7].Replace(" ", string.Empty);
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
                        ImprovementCost[row] = Int32.Parse(text[1].Replace(" ", string.Empty));
                        ImprovementUpkeep[row] = Int32.Parse(text[2].Replace(" ", string.Empty));
                        ImprovementPrereq[row] = text[3].Replace(" ", string.Empty);
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
                        UnitDomain[row] = Int32.Parse(text[2].Replace(" ", string.Empty));
                        UnitMove[row] = Int32.Parse((text[3].Replace(" ", string.Empty)).Replace(".", string.Empty));
                        UnitRange[row] = Int32.Parse(text[4].Replace(" ", string.Empty));
                        UnitAttack[row] = Int32.Parse((text[5].Replace(" ", string.Empty)).Replace("a", string.Empty));
                        UnitDefense[row] = Int32.Parse((text[6].Replace(" ", string.Empty)).Replace("d", string.Empty));
                        UnitHitp[row] = Int32.Parse((text[7].Replace(" ", string.Empty)).Replace("h", string.Empty));
                        UnitFirepwr[row] = Int32.Parse((text[8].Replace(" ", string.Empty)).Replace("f", string.Empty));
                        UnitCost[row] = Int32.Parse(text[9].Replace(" ", string.Empty));
                        UnitHold[row] = Int32.Parse(text[10].Replace(" ", string.Empty));
                        UnitAIrole[row] = Int32.Parse(text[11].Replace(" ", string.Empty));
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
                        TerrainName[row] = text[0];
                        TerrainMovecost[row] = Int32.Parse(text[1].Replace(" ", string.Empty));
                        TerrainDefense[row] = Int32.Parse(text[2].Replace(" ", string.Empty));
                        TerrainFood[row] = Int32.Parse(text[3].Replace(" ", string.Empty));
                        TerrainShields[row] = Int32.Parse(text[4].Replace(" ", string.Empty));
                        TerrainTrade[row] = Int32.Parse(text[5].Replace(" ", string.Empty));
                        TerrainIrrigate[row] = text[6].Replace(" ", string.Empty);
                        TerrainIrrigateBonus[row] = Int32.Parse(text[7].Replace(" ", string.Empty));
                        TerrainIrrigateTurns[row] = Int32.Parse(text[8].Replace(" ", string.Empty));
                        TerrainIrrigateAI[row] = Int32.Parse(text[9].Replace(" ", string.Empty));
                        TerrainMine[row] = text[10].Replace(" ", string.Empty);
                        TerrainMineBonus[row] = Int32.Parse(text[11].Replace(" ", string.Empty));
                        TerrainMineTurns[row] = Int32.Parse(text[12].Replace(" ", string.Empty));
                        TerrainMineAI[row] = Int32.Parse(text[13].Replace(" ", string.Empty));
                        TerrainTransform[row] = text[14].Replace(" ", string.Empty);
                        TerrainShortName[row] = text[16].Replace(" ", string.Empty);
                    }

                    //Next read special terrain
                    for (int row = 0; row < 22; row++)
                    {
                        line = file.ReadLine();
                        List<string> text = line.Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                        TerrainSpecName[row] = text[0];
                        TerrainSpecMovecost[row] = Int32.Parse(text[1].Replace(" ", string.Empty));
                        TerrainSpecDefense[row] = Int32.Parse(text[2].Replace(" ", string.Empty));
                        TerrainSpecFood[row] = Int32.Parse(text[3].Replace(" ", string.Empty));
                        TerrainSpecShields[row] = Int32.Parse(text[4].Replace(" ", string.Empty));
                        TerrainSpecTrade[row] = Int32.Parse(text[5].Replace(" ", string.Empty));
                    }
                }

                //Read GOVERNMENTS
                if (line == "@GOVERNMENTS")
                {
                    for (int row = 0; row < 7; row++)
                    {
                        line = file.ReadLine();
                        List<string> text = line.Split(',').ToList();
                        GovernmentName[row] = text[0].Replace(" ", string.Empty);
                        GovernmentTitleHIS[row] = text[1].Replace(" ", string.Empty);
                        GovernmentTitleHER[row] = text[2].Replace(" ", string.Empty);
                    }
                }

                //Read LEADERS
                if (line == "@LEADERS")
                {
                    for (int row = 0; row < 21; row++)
                    {
                        line = file.ReadLine();
                        List<string> text = line.Split(',').ToList();
                        LeaderNameHIS[row] = text[0];
                        LeaderNameHER[row] = text[1].Replace(" ", string.Empty);
                        LeaderFemale[row] = Int32.Parse(text[2].Replace(" ", string.Empty));
                        LeaderColor[row] = Int32.Parse(text[3].Replace(" ", string.Empty));
                        LeaderCityStyle[row] = Int32.Parse(text[4].Replace(" ", string.Empty));
                        LeaderPlural[row] = text[5].Replace(" ", string.Empty);
                        LeaderAdjective[row] = text[6].Replace(" ", string.Empty);
                        LeaderAttack[row] = Int32.Parse(text[7].Replace(" ", string.Empty));
                        LeaderExpand[row] = Int32.Parse(text[8].Replace(" ", string.Empty));
                        LeaderCivilize[row] = Int32.Parse(text[9].Replace(" ", string.Empty));
                    }
                }

                //Read CARAVAN TRADING COMMODITIES
                if (line == "@CARAVAN")
                {
                    for (int row = 0; row < 16; row++)
                    {
                        line = file.ReadLine();
                        List<string> text = line.Split(',').ToList();
                        CaravanCommoditie[row] = text[0].Replace(" ", string.Empty);
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
                        OrderShortcut[row] = text[1].Replace(" ", string.Empty);
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
