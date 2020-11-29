using System.Collections.Generic;
using System.IO;
using civ2.Bitmaps;
using civ2.Units;
using civ2.Enums;

namespace civ2
{
    public partial class Game : BaseInstance
    {
        public static Game LoadGame(string savDirectoryPath, string SAVname)
        {
            // Import graphical assets from SAV directory. If they don't exist import from root civ2 directory.
            Images.LoadGraphicsAssetsFromFiles(savDirectoryPath);

            // Read SAV file & RULES.txt
            ReadGameData rd = new ReadGameData();
            GameData gameData = rd.Read_SAV_and_RULES(savDirectoryPath, SAVname);

            // Make an instance of a new game & map
            _instance = new Game(gameData);
            Map.Instance.GenerateMap(gameData);


            //_instance.ActiveUnit = Data.SelectedUnitIndex == -1 ? null : Units.Find(unit => unit.Id == Data.SelectedUnitIndex);    //null means all units have ended turn
            //_instance._activeCiv = _instance._civs[Data.HumanPlayer];
            //_instance.ActiveUnit = Game.Units[0];   //temp!!!
            //_instance.ActiveCiv = Game.Civs[0];   //temp!!!
            
            return _instance;   // Return instance so it can be read by forms
        }

        private Game(GameData SAVgameData)
        {
            _units = new List<IUnit>();
            _casualties = new List<IUnit>();
            _cities = new List<City>();
            _civs = new List<Civilization>();

            _gameVersion = SAVgameData.GameVersion;

            _options.Set(SAVgameData.Options);
            _rules.Set(SAVgameData.Rules);

            TurnNumber = SAVgameData.TurnNumber;
            TurnNumberForGameYear = SAVgameData.TurnNumberForGameYear;
            SelectedUnitIndex = SAVgameData.SelectedUnitIndex;
            HumanPlayer = SAVgameData.HumanPlayer;
            PlayersMapUsed = SAVgameData.PlayersMapUsed;
            PlayersCivilizationNumberUsed = SAVgameData.PlayersCivilizationNumberUsed;
            _difficultyLevel = SAVgameData.DifficultyLevel;
            _barbarianActivity = SAVgameData.BarbarianActivity;
            CivsInPlay = SAVgameData.CivsInPlay;
            PollutionAmount = SAVgameData.PollutionAmount;
            GlobalTempRiseOccured = SAVgameData.GlobalTempRiseOccured;
            NoOfTurnsOfPeace = SAVgameData.NoOfTurnsOfPeace;
            NumberOfUnits = SAVgameData.NumberOfUnits;
            NumberOfCities = SAVgameData.NumberOfCities;

            // Create all 8 civs (tribes)
            for (int i = 0; i < 8; i++)
            {
                CreateCiv(i, SAVgameData.HumanPlayer, SAVgameData.CivCityStyle[i], SAVgameData.CivLeaderName[i], SAVgameData.CivTribeName[i],
                    SAVgameData.CivAdjective[i], SAVgameData.RulerGender[i], SAVgameData.CivMoney[i], SAVgameData.CivNumber[i],
                    SAVgameData.CivResearchProgress[i], SAVgameData.CivResearchingTech[i], SAVgameData.CivSciRate[i], SAVgameData.CivTaxRate[i],
                    SAVgameData.CivGovernment[i], SAVgameData.CivReputation[i], SAVgameData.CivTechs);
            }

            // Create units
            for (int i = 0; i < SAVgameData.NumberOfUnits; i++)
            {
                CreateUnit(SAVgameData.UnitType[i], SAVgameData.UnitXloc[i], SAVgameData.UnitYloc[i], SAVgameData.UnitDead[i],
                    SAVgameData.UnitFirstMove[i], SAVgameData.UnitGreyStarShield[i], SAVgameData.UnitVeteran[i], SAVgameData.UnitCiv[i],
                    SAVgameData.UnitMovePointsLost[i], SAVgameData.UnitHitPointsLost[i], SAVgameData.UnitLastMove[i], SAVgameData.UnitCaravanCommodity[i],
                    SAVgameData.UnitOrders[i], SAVgameData.UnitHomeCity[i], SAVgameData.UnitGotoX[i], SAVgameData.UnitGotoY[i],
                    SAVgameData.UnitLinkOtherUnitsOnTop[i], SAVgameData.UnitLinkOtherUnitsUnder[i]);
            }

            // Create cities
            for (int i = 0; i < SAVgameData.NumberOfCities; i++)
            {
                CreateCity(SAVgameData.CityXloc[i], SAVgameData.CityYloc[i], SAVgameData.CityCanBuildCoastal[i], SAVgameData.CityAutobuildMilitaryRule[i],
                    SAVgameData.CityStolenTech[i], SAVgameData.CityImprovementSold[i], SAVgameData.CityWeLoveKingDay[i], SAVgameData.CityCivilDisorder[i],
                    SAVgameData.CityCanBuildShips[i], SAVgameData.CityObjectivex3[i], SAVgameData.CityObjectivex1[i], SAVgameData.CityOwner[i],
                    SAVgameData.CitySize[i], SAVgameData.CityWhoBuiltIt[i], SAVgameData.CityFoodInStorage[i], SAVgameData.CityShieldsProgress[i],
                    SAVgameData.CityNetTrade[i], SAVgameData.CityName[i], SAVgameData.CityDistributionWorkers[i], SAVgameData.CityNoOfSpecialistsx4[i],
                    SAVgameData.CityImprovements[i], SAVgameData.CityItemInProduction[i], SAVgameData.CityActiveTradeRoutes[i], SAVgameData.CityCommoditySupplied[i],
                    SAVgameData.CityCommodityDemanded[i], SAVgameData.CityCommodityInRoute[i], SAVgameData.CityTradeRoutePartnerCity[i], SAVgameData.CityScience[i],
                    SAVgameData.CityTax[i], SAVgameData.CityNoOfTradeIcons[i], SAVgameData.CityFoodProduction[i], SAVgameData.CityShieldProduction[i],
                    SAVgameData.CityHappyCitizens[i], SAVgameData.CityUnhappyCitizens[i]);
            }

            ActiveCursorXY = SAVgameData.ActiveCursorXY;
            ClickedXY = SAVgameData.ClickedXY;
        }



    }
}
