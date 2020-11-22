namespace civ2
{
    public static class Data
    {
        //Cosmic rules from RULES.TXT
        public static int RoadMultiplier { get { return Rules.CosmicRules[0]; } }
        public static int ChanceTriremeLost { get { return Rules.CosmicRules[1]; } }
        public static int FoodEatenPerTurn { get { return Rules.CosmicRules[2]; } }
        public static int RowsFoodBox { get { return Rules.CosmicRules[3]; } }
        public static int RowsShieldBox { get { return Rules.CosmicRules[4]; } }
        public static int SettlersEatTillMonarchy { get { return Rules.CosmicRules[5]; } }
        public static int SettlersEatFromCommunism { get { return Rules.CosmicRules[6]; } }
        public static int CitySizeUnhappyChieftain { get { return Rules.CosmicRules[7]; } }
        public static int RiotFactor { get { return Rules.CosmicRules[8]; } }
        public static int AqueductNeeded { get { return Rules.CosmicRules[9]; } }
        public static int SewerNeeded { get { return Rules.CosmicRules[10]; } }
        public static int TechParadigm { get { return Rules.CosmicRules[11]; } }
        public static int BaseTimeEngineersTransform { get { return Rules.CosmicRules[12]; } }
        public static int MonarchyPaysSupport { get { return Rules.CosmicRules[13]; } }
        public static int CommunismPaysSupport { get { return Rules.CosmicRules[14]; } }
        public static int FundamentalismPaysSupport { get { return Rules.CosmicRules[15]; } }
        public static int CommunismEquivalentPalaceDistance { get { return Rules.CosmicRules[16]; } }
        public static int FundamentalismScienceLost { get { return Rules.CosmicRules[17]; } }
        public static int ShieldPenaltyTypeChange { get { return Rules.CosmicRules[18]; } }
        public static int MaxParadropRange { get { return Rules.CosmicRules[19]; } }
        public static int MassThrustParadigm { get { return Rules.CosmicRules[20]; } }
        public static int MaxEffectiveScienceRate { get { return Rules.CosmicRules[21]; } }

        public static int TurnNumber { get; set; }
        public static int TurnNumberForGameYear { get; set; }
        public static int SelectedUnitIndex { get; set; }
        public static int HumanPlayer { get; set; }
        public static int[] CivsInPlay { get; set; }
        public static int PlayersMapUsed { get; set; }
        public static int PlayersCivilizationNumberUsed { get; set; }
        public static bool MapRevealed { get; set; }
        public static int DifficultyLevel { get; set; }
        public static int BarbarianActivity { get; set; }
        public static int PollutionAmount { get; set; }
        public static int GlobalTempRiseOccured { get; set; }
        public static int NoOfTurnsOfPeace { get; set; }
        public static int NumberOfUnits { get; set; }
        public static int NumberOfCities { get; set; }
        public static int MapXdim { get; set; }
        public static int MapYdim { get; set; }
        public static int MapArea { get; set; }
        public static int MapSeed { get; set; }
        public static int MapLocatorXdim { get; set; }
        public static int MapLocatorYdim { get; set; }
        public static int IndexOfLastActiveUnit { get; set; }

        private static int _gameYear;
        public static int GameYear
        {
            get
            {
                if (TurnNumber < 250) _gameYear = -4000 + (TurnNumber - 1) * 20;
                else if (TurnNumber >= 250 && TurnNumber < 300) _gameYear = 1000 + (TurnNumber - 1 - 250) * 10;
                else if (TurnNumber >= 300 && TurnNumber < 350) _gameYear = 1500 + (TurnNumber - 1 - 300) * 5;
                else if (TurnNumber >= 350 && TurnNumber < 400) _gameYear = 1750 + (TurnNumber - 1 - 350) * 2;
                else _gameYear = 1850 + (TurnNumber - 1 - 400);
                return _gameYear;
            }
        }

        public static int[] ActiveXY { get; set; }
        public static int[] ClickedXY { get; set; }
    }
}