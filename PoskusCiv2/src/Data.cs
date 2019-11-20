using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTciv2
{
    public class Data
    {
        //Cosmic rules from RULES.TXT
        public int RoadMultiplier = ReadFiles.CosmicRules[0];
        public int ChanceTriremeLost = ReadFiles.CosmicRules[1];
        public int FoodEatenPerTurn = ReadFiles.CosmicRules[2];
        public int RowsFoodBox = ReadFiles.CosmicRules[3];
        public int RowsShieldBox = ReadFiles.CosmicRules[4];
        public int SettlersEatTillMonarchy = ReadFiles.CosmicRules[5];
        public int SettlersEatFromCommunism = ReadFiles.CosmicRules[6];
        public int CitySizeUnhappyChieftain = ReadFiles.CosmicRules[7];
        public int RiotFactor = ReadFiles.CosmicRules[8];
        public int AqueductNeeded = ReadFiles.CosmicRules[9];
        public int SewerNeeded = ReadFiles.CosmicRules[10];
        public int TechParadigm = ReadFiles.CosmicRules[11];
        public int BaseTimeEngineersTransform = ReadFiles.CosmicRules[12];
        public int MonarchyPaysSupport = ReadFiles.CosmicRules[13];
        public int CommunismPaysSupport = ReadFiles.CosmicRules[14];
        public int FundamentalismPaysSupport = ReadFiles.CosmicRules[15];
        public int CommunismEquivalentPalaceDistance = ReadFiles.CosmicRules[16];
        public int FundamentalismScienceLost = ReadFiles.CosmicRules[17];
        public int ShieldPenaltyTypeChange = ReadFiles.CosmicRules[18];
        public int MaxParadropRange = ReadFiles.CosmicRules[19];
        public int MassThrustParadigm = ReadFiles.CosmicRules[20];
        public int MaxEffectiveScienceRate = ReadFiles.CosmicRules[21];
        
        public int TurnNumber { get; set; }
        public int TurnNumberForGameYear { get; set; }
        public int UnitSelectedIndex { get; set; }
        public int HumanPlayerUsed { get; set; }
        public int[] CivsInPlay { get; set; }
        public int PlayersMapUsed { get; set; }
        public int PlayersCivilizationNumberUsed { get; set; }
        public bool MapRevealed { get; set; }
        public int DifficultyLevel { get; set; }
        public int BarbarianActivity { get; set; }
        public int PollutionAmount { get; set; }
        public int GlobalTempRiseOccured { get; set; }
        public int NoOfTurnsOfPeace { get; set; }
        public int NumberOfUnits { get; set; }
        public int NumberOfCities { get; set; }
        public int MapXdim { get; set; }
        public int MapYdim { get; set; }
        public int MapArea { get; set; }
        public int MapSeed { get; set; }
        public int MapLocatorXdim { get; set; }
        public int MapLocatorYdim { get; set; }

        public int IndexOfLastActiveUnit { get; set; }

        private int _gameYear;
        public int GameYear
        {
            get
            {
                if (TurnNumber < 250) _gameYear = -4000 + TurnNumber * 20;
                else if (TurnNumber >= 250 && TurnNumber < 300) _gameYear = 1000 + (TurnNumber - 250) * 10;
                else if (TurnNumber >= 300 && TurnNumber < 350) _gameYear = 1500 + (TurnNumber - 300) * 5;
                else if (TurnNumber >= 350 && TurnNumber < 400) _gameYear = 1750 + (TurnNumber - 350) * 2;
                else _gameYear = 1850 + (TurnNumber - 400);
                return _gameYear;
            }
        }
    }
}
