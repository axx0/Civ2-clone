using System.Collections.Generic;
using Civ2engine.IO;
using Civ2engine.Units;

namespace Civ2engine
{
    public partial class Game : BaseInstance
    {
        public static void Create(Rules rules, GameData gameData)
        {
            _instance = new Game(rules, gameData);
        }
        public Game(Rules rules, GameData gameData)
        {
            _rules = rules;

            //_civsInPlay = SAVgameData.CivsInPlay;
            _gameVersion = gameData.GameVersion;

            if (gameData.Options != null)
            {
                _options = gameData.Options;
            }
            else
            {
                _options = new Options();
                _options.Set(gameData.OptionsArray);
            }

            _turnNumber = gameData.TurnNumber;
            TurnNumberForGameYear = gameData.TurnNumberForGameYear;
            _difficultyLevel = gameData.DifficultyLevel;
            _barbarianActivity = gameData.BarbarianActivity;
            PollutionAmount = gameData.PollutionAmount;
            GlobalTempRiseOccured = gameData.GlobalTempRiseOccured;
            NoOfTurnsOfPeace = gameData.NoOfTurnsOfPeace;
            NumberOfUnits = gameData.NumberOfUnits;
            NumberOfCities = gameData.NumberOfCities;

            if (gameData.Civilizations != null)
            {
                GetCivs.AddRange(gameData.Civilizations);
            }
            else
            {
                // Create all 8 civs (tribes)
                for (var i = 0; i < 8; i++)
                {
                    CreateCiv(i, gameData.PlayersCivIndex, gameData.CivsInPlay[i], gameData.CivCityStyle[i],
                        gameData.CivLeaderName[i], gameData.CivTribeName[i], gameData.CivAdjective[i],
                        gameData.RulerGender[i], gameData.CivMoney[i], gameData.CivNumber[i],
                        gameData.CivResearchProgress[i], gameData.CivResearchingTech[i], gameData.CivSciRate[i],
                        gameData.CivTaxRate[i], gameData.CivGovernment[i], gameData.CivReputation[i],
                        gameData.CivTechs);
                }
            }

            // Create cities
            for (var i = 0; i < gameData.NumberOfCities; i++)
            {
                CreateCity(gameData.CityXloc[i], gameData.CityYloc[i], gameData.CityCanBuildCoastal[i],
                    gameData.CityAutobuildMilitaryRule[i], gameData.CityStolenTech[i],
                    gameData.CityImprovementSold[i], gameData.CityWeLoveKingDay[i],
                    gameData.CityCivilDisorder[i], gameData.CityCanBuildShips[i], gameData.CityObjectivex3[i],
                    gameData.CityObjectivex1[i], gameData.CityOwner[i], gameData.CitySize[i],
                    gameData.CityWhoBuiltIt[i], gameData.CityFoodInStorage[i], gameData.CityShieldsProgress[i],
                    gameData.CityNetTrade[i], gameData.CityName[i], gameData.CityDistributionWorkers[i],
                    gameData.CityNoOfSpecialistsx4[i], gameData.CityImprovements[i],
                    gameData.CityItemInProduction[i], gameData.CityActiveTradeRoutes[i],
                    gameData.CityCommoditySupplied[i], gameData.CityCommodityDemanded[i],
                    gameData.CityCommodityInRoute[i], gameData.CityTradeRoutePartnerCity[i],
                    gameData.CityScience[i], gameData.CityTax[i], gameData.CityNoOfTradeIcons[i],
                    gameData.CityTotalFoodProduction[i], gameData.CityTotalShieldProduction[i],
                    gameData.CityHappyCitizens[i], gameData.CityUnhappyCitizens[i]);
            }

            // Create units
            for (int i = 0; i < gameData.NumberOfUnits; i++)
            {
                CreateUnit(gameData.UnitType[i], gameData.UnitXloc[i], gameData.UnitYloc[i],
                    gameData.UnitDead[i], gameData.UnitFirstMove[i], gameData.UnitGreyStarShield[i],
                    gameData.UnitVeteran[i], gameData.UnitCiv[i], gameData.UnitMovePointsLost[i],
                    gameData.UnitHitPointsLost[i], gameData.UnitPrevXloc[i], gameData.UnitPrevYloc[i],
                    gameData.UnitCaravanCommodity[i], gameData.UnitOrders[i], gameData.UnitHomeCity[i],
                    gameData.UnitGotoX[i], gameData.UnitGotoY[i], gameData.UnitLinkOtherUnitsOnTop[i],
                    gameData.UnitLinkOtherUnitsUnder[i]);
            }

            //_activeXY = SAVgameData.ActiveCursorXY; // Active unit or view piece coords (if it's active unit, you really don't need this)

            _activeUnit = gameData.SelectedUnitIndex == -1 ? null : AllUnits.Find(unit => unit.Id == gameData.SelectedUnitIndex);    // null means all units have ended turn
            _playerCiv = GetCivs[gameData.PlayersCivIndex];
            _activeCiv = _playerCiv;
        }
    }
}
