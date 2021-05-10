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
        public UnitDefinition[] UnitTypes { get; set; }

        // Advances
        public Advance[] Advances { get; set; }

        public readonly Dictionary<string, int> AdvanceMappings = new()
        {
            {"nil", -1},
            {"no", -2}
        };
        
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
        
        
        public Terrain[] Terrains { get; set; }

        public void Set(List<string[]> rulesList)
        {
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
