using Civ2engine.Enums;
using System.Collections.Generic;

namespace Civ2engine
{
    public class GameData
    {
        public GameVersionType GameVersion { get; set; }

        // Options
        public bool[] Options { get; set; }

        // General game data
        public int TurnNumber { get; set; }
        public int TurnNumberForGameYear { get; set; }
        public int SelectedUnitIndex { get; set; }
        public int PlayersCivIndex { get; set; }
        public int WhichCivsMapShown { get; set; }
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

        // Map data
        public int MapXdim { get; set; }
        public int MapYdim { get; set; }
        public int MapArea { get; set; }
        public int MapResourceSeed { get; set; }
        public int MapLocatorXdim { get; set; }
        public int MapLocatorYdim { get; set; }
        public TerrainType[,] MapTerrainType { get; set; }
        public bool[,] MapRiverPresent { get; set; }
        public bool[,] MapResourcePresent { get; set; }
        public bool[,] MapUnitPresent { get; set; }
        public bool[,] MapCityPresent { get; set; }
        public bool[,] MapIrrigationPresent { get; set; }
        public bool[,] MapMiningPresent { get; set; }
        public bool[,] MapRoadPresent { get; set; }
        public bool[,] MapRailroadPresent { get; set; }
        public bool[,] MapFortressPresent { get; set; }
        public bool[,] MapPollutionPresent { get; set; }
        public bool[,] MapFarmlandPresent { get; set; }
        public bool[,] MapAirbasePresent { get; set; }
        public int[,] MapIslandNo { get; set; }
        public SpecialType[,] MapSpecialType { get; set; }
        public bool[,][] MapVisibilityCivs { get; set; }

        // Units
        public int[] UnitXloc { get; set; }
        public int[] UnitYloc { get; set; }
        public bool[] UnitDead { get; set; }
        public bool[] UnitFirstMove { get; set; }
        public bool[] UnitImmobile { get; set; }
        public bool[] UnitGreyStarShield { get; set; }
        public bool[] UnitVeteran { get; set; }
        public UnitType[] UnitType { get; set; }
        public int[] UnitCiv { get; set; }
        public int[] UnitMovePointsLost { get; set; }
        public int[] UnitHitPointsLost { get; set; }
        public int[] UnitPrevXloc { get; set; }
        public int[] UnitPrevYloc { get; set; }
        public CommodityType[] UnitCaravanCommodity { get; set; }
        public OrderType[] UnitOrders { get; set; }
        public int[] UnitHomeCity { get; set; }
        public int[] UnitGotoX { get; set; }
        public int[] UnitGotoY { get; set; }
        public int[] UnitLinkOtherUnitsOnTop { get; set; }
        public int[] UnitLinkOtherUnitsUnder { get; set; }

        // Cities
        public int[] CityXloc { get; set; }
        public int[] CityYloc { get; set; }
        public bool[] CityCanBuildCoastal { get; set; }
        public bool[] CityAutobuildMilitaryRule { get; set; }
        public bool[] CityStolenTech { get; set; }
        public bool[] CityImprovementSold { get; set; }
        public bool[] CityWeLoveKingDay { get; set; }
        public bool[] CityCivilDisorder { get; set; }
        public bool[] CityCanBuildShips { get; set; }
        public bool[] CityObjectivex3 { get; set; }
        public bool[] CityObjectivex1 { get; set; }
        public int[] CityOwner { get; set; }
        public int[] CitySize { get; set; }
        public int[] CityWhoBuiltIt { get; set; }
        public int[] CityFoodInStorage { get; set; }
        public int[] CityShieldsProgress { get; set; }
        public int[] CityNetTrade { get; set; }
        public string[] CityName { get; set; }
        public bool[][] CityDistributionWorkers { get; set; }
        public int[] CityNoOfSpecialistsx4 { get; set; }
        public bool[][] CityImprovements { get; set; }
        public int[] CityItemInProduction { get; set; }
        public int[] CityActiveTradeRoutes { get; set; }
        public CommodityType[][] CityCommoditySupplied { get; set; }
        public CommodityType[][] CityCommodityDemanded { get; set; }
        public CommodityType[][] CityCommodityInRoute { get; set; }
        public int[][] CityTradeRoutePartnerCity { get; set; }
        public int[] CityScience { get; set; }
        public int[] CityTax { get; set; }
        public int[] CityNoOfTradeIcons { get; set; }
        public int[] CityTotalFoodProduction { get; set; }
        public int[] CityTotalShieldProduction { get; set; }
        public int[] CityHappyCitizens { get; set; }
        public int[] CityUnhappyCitizens { get; set; }

        // Other data        
        public int[] ActiveCursorXY { get; set; }
        public int[] ClickedXY { get; set; }
        public int Zoom { get; set; }

        //public int IndexOfLastActiveUnit { get; set; }

        //public int GameYear { get; set; }
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

    }
}