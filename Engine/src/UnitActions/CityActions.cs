using System;
using System.Collections.Generic;
using System.Linq;
using Civ2engine.Advances;
using Civ2engine.Enums;
using Civ2engine.Improvements;
using Civ2engine.MapObjects;
using Civ2engine.Production;
using Civ2engine.Units;

namespace Civ2engine.UnitActions
{
    public static class CityActions 
    {
        public static Action CreateCityBuild(Func<string, BuildCityConfirmResult> confirmCityBuild)
        {
            return () =>
            {
                var game = Game.Instance;
                var unit = game.ActiveUnit;
                if (!unit.TypeDefinition.CanBuildCities) return;

                var tile = unit.CurrentLocation;
                if (!tile.Terrain.CanHaveCity) return;
                
                var confirmed = confirmCityBuild(GetCityName(unit.Owner, game));
                if (confirmed.Build)
                {
                    BuildCity(tile, unit, game, confirmed.Name);
                }
            };
        }

        private static string GetCityName(Civilization civ , Game game)
        {
            var cityCount = game.History.TotalCitiesBuilt(civ.Id);
            var names = game.CityNames;
            var tribe = civ.TribeName.ToUpperInvariant();
            var civCityList = names[names.ContainsKey(tribe) ? tribe : "EXTRA"];
            if (cityCount < civCityList.Count)
            {
                return civCityList[cityCount];
            }
            return "Dummy Name";
        }

        private static void BuildCity(Tile tile, Unit unit, Game game, string name)
        {
            var city = new City
            {
                Location = tile,
                Name = name,
                X = tile.X,
                Y = tile.Y,
                Owner = unit.Owner,
                Size = 1,
                ItemInProduction = game.Rules.ProductionItems.OrderBy(i=>i.Cost).First()
            };
            tile.WorkedBy = city;
            tile.CityHere = city;
            game.AllCities.Add(tile.CityHere);
            unit.Owner.Cities.Add(tile.CityHere);
            if (unit.Owner.Cities.Count == 1)
            {
                var capitalImprovement = ProductionPossibilities.FindByEffect(city.Owner.Id, ImprovementEffect.Capital)
                                         ?? game.Rules.Improvements.Where(i =>
                                             i.Effects.ContainsKey(ImprovementEffect.Capital) &&
                                             city.Owner.AllowedAdvanceGroups[
                                                 game.Rules.Advances[i.Prerequisite].AdvanceGroup] !=
                                             AdvanceGroupAccess.Prohibited).OrderBy(i => i.Cost).FirstOrDefault();
                if (capitalImprovement != null)
                {
                    city.AddImprovement(capitalImprovement);
                }
            }
            game.History.CityBuilt(tile.CityHere);

            game.AutoAddDistributionWorkers(city);
            city.CalculateOutput(city.Owner.Government, game);

            unit.Dead = true;
            unit.MovePointsLost = unit.MovePoints;

            if (tile.Fertility != -2)
            {
                game.CurrentMap.AdjustFertilityForCity(tile);
            }

            game.TriggerMapEvent(MapEventType.UpdateMap, new List<Tile> {tile});

            game.ChooseNextUnit();
        }

        public static void AIBuildCity(Unit unit, Game game)
        {
            BuildCity(unit.CurrentLocation, unit, game, GetCityName(unit.Owner, game));
        }
    }
}