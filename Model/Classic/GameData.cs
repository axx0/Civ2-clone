using System.Collections.Generic;

namespace Civ2engine
{
    public class GameData : IGameData
    {
        public byte GameVersion { get; set; }

        // Options
        public bool[] OptionsArray { get; set; }

        // General game data
        public bool FirstAirUnitBuilt { get; set; }
        public bool FirstNavalUnitBuilt { get; set; }
        public bool FirstCaravanBuilt { get; set; }
        public bool WasRepublicDemocracyAdopted { get; set; }
        public bool FirstSignificantlyDamagedUnit { get; set; }
        public int TurnNumber { get; set; }
        public short TurnNumberForGameYear { get; set; }
        public short SelectedUnitIndex { get; set; }
        public byte PlayersCivIndex { get; set; }
        public byte WhichCivsMapShown { get; set; }
        public byte PlayersCivilizationNumberUsed { get; set; }
        public bool MapRevealed { get; set; }
        public int DifficultyLevel { get; set; }
        public int BarbarianActivity { get; set; }
        public bool[] CivsInPlay { get; set; }
        public bool[] HumanPlayers { get; set; }
        public int NoPollutionSkulls { get; set; }
        public int GlobalTempRiseOccured { get; set; }
        public int NoOfTurnsOfPeace { get; set; }
        public short NumberOfUnits { get; set; }
        public short NumberOfCities { get; set; }

        // Technologies
        public byte[] CivFirstDiscoveredTech { get; set; }
        public bool[,] CivsDiscoveredTechs { get; set; }

        // Wonders
        public short[] WonderCity { get; set; }
        public bool[] WonderBuilt { get; set; }
        public bool[] WonderDestroyed { get; set; }

        // Other
        public Dictionary<string, string> ExtendedMetadata { get; } = new();

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
        public byte[] CivNumberAdvancesResearched { get; set; }
        public byte[] CivNumberFutureTechsResearched { get; set; }
        public byte[] CivSciRate { get; set; }
        public byte[] CivTaxRate { get; set; }
        public byte[] CivGovernment { get; set; }
        public byte[] CivReputation { get; set; }
        public byte[] CivPatience { get; set; }
        public bool[][] CivTreatyContact { get; set; }
        public bool[][] CivTreatyCeaseFire { get; set; }
        public bool[][] CivTreatyPeace { get; set; }
        public bool[][] CivTreatyAlliance { get; set; }
        public bool[][] CivTreatyVendetta { get; set; }
        public bool[][] CivTreatyEmbassy { get; set; }
        public bool[][] CivTreatyWar { get; set; }
        public int[][] CivAttitudes { get; set; }
        public bool[][] CivAdvances { get; set; }
        public short[] CivNumberMilitaryUnits { get; set; }
        public short[] CivNumberCities { get; set; }
        public short[] CivSumCitySizes { get; set; }
        public byte[][] CivActiveUnitsPerType { get; set; }
        public byte[][] CivCasualtiesPerType { get; set; }
        public byte[][] CivUnitsInProductionPerType { get; set; }
        public int[][] CivLastContact { get; set; }
        public bool[] CivHasSpaceship { get; set; }
        public short[] CivSpaceshipEstimatedArrival { get; set; }
        public short[] CivSpaceshipLaunchYear { get; set; }
        public short[] CivSpaceshipStructural { get; set; }
        public short[] CivSpaceshipComponentsPropulsion { get; set; }
        public short[] CivSpaceshipComponentsFuel { get; set; }
        public short[] CivSpaceshipModulesHabitation { get; set; }
        public short[] CivSpaceshipModulesLifeSupport { get; set; }
        public short[] CivSpaceshipModulesSolarPanel { get; set; }

        // @LEADERS2 table from rules.txt (TOT only)
        public byte[][] CivsRelationsToAdvancesGroups { get;set; }

        // Transporters
        public short NoTransporters { get; set; }
        public short[] Transporter1X { get; set; }
        public short[] Transporter1Y { get; set; }
        public byte[] Transporter1MapNo { get; set; }
        public short[] Transporter2X { get; set; }
        public short[] Transporter2Y { get; set; }
        public byte[] Transporter2MapNo { get; set; }
        public byte[] TransporterLook { get; set; }

        // Map data
        public short MapXdimX2 { get; set; }
        public short MapYdim { get; set; }
        public short MapArea { get; set; }
        public short MapResourceSeed { get; set; }
        public short MapLocatorXdim { get; set; }
        public short MapLocatorYdim { get; set; }
        public short MapNoSecondaryMaps { get; set; }
        public bool[][,,] MapUnitVisibility { get; set; }
        public bool[][,,] MapCityVisibility { get; set; }
        public bool[][,,] MapIrrigationVisibility { get; set; }
        public bool[][,,] MapMiningVisibility { get; set; }
        public bool[][,,] MapRoadVisibility { get; set; }
        public bool[][,,] MapRailroadVisibility { get; set; }
        public bool[][,,] MapFortressVisibility { get; set; }
        public bool[][,,] MapPollutionVisibility { get; set; }
        public bool[][,,] MapAirbaseVisibility { get; set; }
        public bool[][,,] MapFarmlandVisibility { get; set; }
        public bool[][,,] MapTransporterVisibility { get; set; }    // TOT only
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
        public byte[][,] MapTileWithinCityRadiusOwner { get; set; }
        public byte[][,] LandSeaIndex { get; set; }
        public int[][,] MapSpecialType { get; set; }
        public bool[][,,] MapTileVisibility { get; set; }
        public int[][,] MapTileFertility { get; set; }
        public int[][,] MapTileOwnership { get; set; }
        public short[] MapSeed { get; set; }

        // Units
        public short[] UnitXloc { get; set; }
        public short[] UnitYloc { get; set; }
        public short[] UnitMap { get; set; }   // TOT only
        public bool[] UnitMadeMoveThisTurn { get; set; }
        public bool[][] UnitVisibility { get; set; }
        public bool[] UnitVeteran { get; set; }
        public bool[] UnitWaitOrder { get; set; }
        public byte[] UnitType { get; set; }
        public byte[] UnitCiv { get; set; }
        public short[] UnitMovePointsLost { get; set; }
        public byte[] UnitHitPointsLost { get; set; }
        public short[] UnitPrevXloc { get; set; }
        public short[] UnitPrevYloc { get; set; }
        public byte[] UnitCounterRoleParameter { get; set; }
        public byte[] UnitOrders { get; set; }
        public byte[] UnitHomeCity { get; set; }
        public short[] UnitGotoX { get; set; }
        public short[] UnitGotoY { get; set; }
        public short[] UnitMapNoOfGoto { get; set; }    // TOT only
        public short[] UnitLinkOtherUnitsOnTop { get; set; }
        public short[] UnitLinkOtherUnitsUnder { get; set; }
        public short[] UnitAnimation { get; set; }  // TOT only
        public short[] UnitOrientation { get; set; }  // TOT only

        // Unit transport settings (TOT only)
        public short[] UnitTransportRelationship { get; set; }    // TOT only
        public short[] UnitTransportBuildTransportSiteMask { get; set; }    // TOT only
        public short[] UnitTransportUseTransportSiteMask { get; set; }    // TOT only
        public short[] UnitTransportNativeTransportAbilityMask { get; set; }    // TOT only

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
        public byte[] CityTurnsExpiredSinceCaptured { get; set; }
        public bool[][] CityWhoKnowsAboutIt { get; set; }
        public int[][] CityLastSizeRevealedToCivs { get; set; }
        public byte[][] CitySpecialists { get; set; }
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
        
        // Tribe cities data
        public byte[] CitiesBuiltSofar { get; set; }

        // Other data        
        public short[] ActiveCursorXy { get; set; }
        public int[] ClickedXy { get; set; }
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
        public bool SpRfileOverride { get; set; }
        public bool SpecialWwiIonlyAi { get; set; }
        public string ScenarioName { get; set; }
        public short TechParadigm { get; set; }
        public int TurnYearIncrement { get; set; }
        public int StartingYear { get; set; }
        public short MaxTurns { get; set; }
        public short ObjectiveProtagonist { get; set; }
        public short NoObjectivesDecisiveVictory { get; set; }
        public short NoObjectivesMarginalVictory { get; set; }
        public short NoObjectivesMarginalDefeat { get; set; }
        public short NoObjectivesDecisiveDefeat { get; set; }

        // Destroyed civs
        public short NoCivsDestroyed { get; set; }
        public short[] TurnCivDestroyed { get; set; }
        public byte[] CivIdThatDestroyedOtherCiv { get; set; }
        public byte[] CivIdThatWasDestroyed { get; set; }
        public string[] NameOfDestroyedCiv { get; set; }
        
        // Events
        public short NumberOfEvents { get; set; }
        public int[] EventTriggerIds { get; set; }
        public int[][] EventActionIds { get; set; }
        public bool[][] EventModifiers { get; set; } // TOT only
        public byte[][] EventTriggerParam { get; set; }
        public byte[][] EventActionParam { get; set; }
        public List<string> EventStrings { get; set; } 
    }
}