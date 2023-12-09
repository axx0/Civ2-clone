using System.Collections.Generic;

namespace Civ2engine
{
    public class GameData
    {
        public byte GameVersion { get; set; }

        // Options
        public bool[] OptionsArray { get; set; }

        // General game data
        public short TurnNumber { get; set; }
        public short TurnNumberForGameYear { get; set; }
        public short SelectedUnitIndex { get; set; }
        public byte PlayersCivIndex { get; set; }
        public byte WhichCivsMapShown { get; set; }
        public byte PlayersCivilizationNumberUsed { get; set; }
        public bool MapRevealed { get; set; }
        public byte DifficultyLevel { get; set; }
        public byte BarbarianActivity { get; set; }
        public bool[] CivsInPlay { get; set; }
        public bool[] HumanPlayers { get; set; }
        public byte PollutionAmount { get; set; }
        public byte GlobalTempRiseOccured { get; set; }
        public byte NoOfTurnsOfPeace { get; set; }
        public short NumberOfUnits { get; set; }
        public short NumberOfCities { get; set; }

        // Wonders
        public short[] WonderCity { get; set; }
        public bool[] WonderBuilt { get; set; }
        public bool[] WonderDestroyed { get; set; }

        // Civ
        public byte[] CivCityStyle { get; set; }
        public string[] CivLeaderName { get; set; }
        public string[] CivTribeName { get; set; }
        public string[] CivAdjective { get; set; }
        public string[] CivAnarchyTitle { get; set; }
        public string[] CivDespotismTitle { get; set; }
        public string[] CivMonarchyTitle { get; set; }
        public string[] CivCommunismTitle { get; set; }
        public string[] CivFundamentalismTitle { get; set; }
        public string[] CivRepublicTitle { get; set; }
        public string[] CivDemocracyTitle { get; set; }

        // Civ advances & money
        public byte[] RulerGender { get; set; }
        public short[] CivMoney { get; set; }
        public byte[] CivNumber { get; set; }
        public short[] CivResearchProgress { get; set; }
        public byte[] CivResearchingAdvance { get; set; }
        public byte[] CivSciRate { get; set; }
        public byte[] CivTaxRate { get; set; }
        public byte[] CivGovernment { get; set; }
        public byte[] CivReputation { get; set; }
        public bool[][] CivAdvances { get; set; }
        public byte[][] CivActiveUnitsPerUnitType { get; set; }
        public byte[][] CivCasualtiesPerUnitType { get; set; }

        // Transporters
        public short NoTransporters { get; set; }

        // Map data
        public short MapXdim_x2 { get; set; }
        public short MapYdim { get; set; }
        public short MapArea { get; set; }
        public short MapResourceSeed { get; set; }
        public short MapLocatorXdim { get; set; }
        public short MapLocatorYdim { get; set; }
        public short MapNoSecondaryMaps { get; set; }
        public int[][,] MapTerrainType { get; set; }
        public bool[][,] MapRiverPresent { get; set; }
        public bool[][,] MapResourcePresent { get; set; }
        public bool[][,] MapUnitPresent { get; set; }
        public bool[][,] MapCityPresent { get; set; }
        public bool[][,] MapIrrigationPresent { get; set; }
        public bool[][,] MapMiningPresent { get; set; }
        public bool[][,] MapRoadPresent { get; set; }
        public bool[][,] MapRailroadPresent { get; set; }
        public bool[][,] MapFortressPresent { get; set; }
        public bool[][,] MapPollutionPresent { get; set; }
        public bool[][,] MapFarmlandPresent { get; set; }
        public bool[][,] MapAirbasePresent { get; set; }
        public bool[][,] MapTransporterPresent { get; set; }
        public byte[][,] MapIslandNo { get; set; }
        public int[][,] MapSpecialType { get; set; }
        public bool[][,,] MapVisibilityCivs { get; set; }
        public short[] MapSeed { get; set; }

        // Units
        public short[] UnitXloc { get; set; }
        public short[] UnitYloc { get; set; }
        public short[] UnitMap { get; set; }   // TOT only
        public bool[] UnitDead { get; set; }
        public bool[] UnitFirstMove { get; set; }
        public bool[] UnitImmobile { get; set; }
        public bool[] UnitGreyStarShield { get; set; }
        public bool[] UnitVeteran { get; set; }
        public byte[] UnitType { get; set; }
        public byte[] UnitCiv { get; set; }
        public short[] UnitMovePointsLost { get; set; }
        public byte[] UnitHitPointsLost { get; set; }
        public short[] UnitPrevXloc { get; set; }
        public short[] UnitPrevYloc { get; set; }
        public byte[] UnitCaravanCommodity { get; set; }
        public byte[] UnitOrders { get; set; }
        public byte[] UnitHomeCity { get; set; }
        public short[] UnitGotoX { get; set; }
        public short[] UnitGotoY { get; set; }
        public short[] UnitMapNoOfGoto { get; set; }    // TOT only
        public short[] UnitLinkOtherUnitsOnTop { get; set; }
        public short[] UnitLinkOtherUnitsUnder { get; set; }

        // Cities
        public short[] CityXloc { get; set; }
        public short[] CityYloc { get; set; }
        public short[] CityMapNo { get; set; }  // TOT only
        public bool[] CityCivilDisorder { get; set; }
        public bool[] CityWeLoveKingDay { get; set; }
        public bool[] CityImprovementSold { get; set; }
        public bool[] CityStolenAdvance { get; set; }
        public bool[] CityAutobuildMilitaryRule { get; set; }
        public bool[] CityCanBuildCoastal { get; set; }
        public bool[] CityCanBuildHydro { get; set; }
        public bool[] CityCanBuildShips { get; set; }
        public bool[] CityAutobuildMilitaryAdvisor { get; set; }
        public bool[] CityAutobuildDomesticAdvisor { get; set; }
        public bool[] CityObjectivex1 { get; set; }
        public bool[] CityObjectivex3 { get; set; }
        public byte[] CityOwner { get; set; }
        public byte[] CitySize { get; set; }
        public byte[] CityWhoBuiltIt { get; set; }
        public short[] CityFoodInStorage { get; set; }
        public short[] CityShieldsProgress { get; set; }
        public short[] CityNetTrade { get; set; }
        public string[] CityName { get; set; }
        public bool[][] CityDistributionWorkers { get; set; }
        public byte[] CityNoOfSpecialistsx4 { get; set; }
        public bool[][] CityImprovements { get; set; }
        public sbyte[] CityItemInProduction { get; set; }
        public int[] CityActiveTradeRoutes { get; set; }
        public int[][] CityCommoditySupplied { get; set; }
        public int[][] CityCommodityDemanded { get; set; }
        public int[][] CityCommodityInRoute { get; set; }
        public int[][] CityTradeRoutePartnerCity { get; set; }
        public short[] CityScience { get; set; }
        public short[] CityTax { get; set; }
        public short[] CityNoOfTradeIcons { get; set; }
        public byte[] CityTotalFoodProduction { get; set; }
        public byte[] CityTotalShieldProduction { get; set; }
        public byte[] CityHappyCitizens { get; set; }
        public byte[] CityUnhappyCitizens { get; set; }

        // Other data        
        public short[] ActiveCursorXY { get; set; }
        public int[] ClickedXY { get; set; }
        public short Zoom { get; set; }

        // Scenario parameters
        public bool TotalWar { get; set; }
        public bool ObjectiveVictory { get; set; }
        public bool CountWondersAsObjectives { get; set; }
        public bool ForbidGovernmentSwitching { get; set; }
        public bool ForbidTechFromConquests { get; set; }
        public bool ElliminatePollution { get; set; }
        public bool TerrainAnimationLockout { get; set; }   // TOT only
        public bool UnitAnimationLockout { get; set; }   // TOT only
        public bool SPRfileOverride { get; set; }
        public bool SpecialWWIIonlyAI { get; set; }
        public string ScenarioName { get; set; }
        public short TechParadigm { get; set; }
        public short TurnYearIncrement { get; set; }
        public short StartingYear { get; set; }
        public short MaxTurns { get; set; }
        public short ObjectiveProtagonist { get; set; }
        public short NoObjectivesDecisiveVictory { get; set; }
        public short NoObjectivesMarginalVictory { get; set; }
        public short NoObjectivesMarginalDefeat { get; set; }
        public short NoObjectivesDecisiveDefeat { get; set; }
        
        // Events
        public short NumberOfEvents { get; set; }
        public int[] EventTriggerIds { get; set; }
        public int[][] EventActionIds { get; set; }
        public int[][] EventTriggerParam { get; set; }
        public int[][] EventActionParam { get; set; }
        public List<string> EventStrings { get; set; } 
    }
}