using civ2.Enums;

namespace civ2
{
    public class GameData
    {
        // Options
        public bool[] Options { get; set; }

        // General game data
        public int TurnNumber { get; set; }
        public int TurnNumberForGameYear { get; set; }
        public int SelectedUnitIndex { get; set; }
        public int HumanPlayer { get; set; }
        public int PlayersMapUsed { get; set; }
        public int PlayersCivilizationNumberUsed { get; set; }
        public bool MapRevealed { get; set; }
        public DifficultyType DifficultyLevel { get; set; }
        public BarbarianActivityType BarbarianActivity { get; set; }
        public bool[] CivsInPlay { get; set; }
        public int PollutionAmount { get; set; }
        public int GlobalTempRiseOccured { get; set; }
        public int NoOfTurnsOfPeace { get; set; }
        public int NumberOfUnits { get; set; }
        public int NumberOfCities { get; set; }

        // Wonders
        public int[] WonderCity { get; set; }
        public bool[] WonderBuilt { get; set; }
        public bool[] WonderDestroyed { get; set; }

        // Civ
        public int[] CivCityStyle { get; set; }
        public string[] CivLeaderName { get; set; }
        public string[] CivTribeName { get; set; }
        public string[] CivAdjective { get; set; }

        // Civ tech & money
        public int[] RulerGender { get; set; }
        public int[] CivMoney { get; set; }
        public int[] CivNumber { get; set; }
        public int[] CivResearchProgress { get; set; }
        public int[] CivResearchingTech { get; set; }
        public int[] CivSciRate { get; set; }
        public int[] CivTaxRate { get; set; }
        public int[] CivGovernment { get; set; }
        public int[] CivReputation { get; set; }
        public bool[] CivTechs { get; set; }


        public int MapXdim { get; set; }
        public int MapYdim { get; set; }
        public int MapArea { get; set; }
        public int MapSeed { get; set; }
        public int MapLocatorXdim { get; set; }
        public int MapLocatorYdim { get; set; }
        public int IndexOfLastActiveUnit { get; set; }

        public int GameYear { get; set; }
        //{
        //    get
        //    {
        //        if (TurnNumber < 250) _gameYear = -4000 + (TurnNumber - 1) * 20;
        //        else if (TurnNumber >= 250 && TurnNumber < 300) _gameYear = 1000 + (TurnNumber - 1 - 250) * 10;
        //        else if (TurnNumber >= 300 && TurnNumber < 350) _gameYear = 1500 + (TurnNumber - 1 - 300) * 5;
        //        else if (TurnNumber >= 350 && TurnNumber < 400) _gameYear = 1750 + (TurnNumber - 1 - 350) * 2;
        //        else _gameYear = 1850 + (TurnNumber - 1 - 400);
        //        return _gameYear;
        //    }
        //}

        public GameVersionType GameVersion { get; set; }

    }
}