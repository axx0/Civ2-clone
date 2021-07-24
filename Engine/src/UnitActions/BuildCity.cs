using System;
using System.Collections.Generic;
using Civ2engine.Enums;
using Civ2engine.Terrains;

namespace Civ2engine.UnitActions
{
    public static class BuildCity 
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
                
                var confirmed = confirmCityBuild("Dummy Name");
                if (confirmed.Build)
                {
                    tile.CityHere = new City
                    {
                        Location = tile,
                        Name = confirmed.Name,
                        X = tile.X,
                        Y = tile.Y,
                        Owner = unit.Owner,
                        Size = 1,
                    };
                    game.GetCities.Add(tile.CityHere);
                    unit.Owner.Cities.Add(tile.CityHere);
                    
                    game.AutoAddDistributionWorkers(tile.CityHere);

                    unit.Dead = true;
                    unit.MovePointsLost = unit.MovePoints;
                    
                    game.TriggerMapEvent(MapEventType.UpdateMap, new List<Tile>{ tile });
                    
                    game.ChooseNextUnit();
                }
            };
        }
    }
}