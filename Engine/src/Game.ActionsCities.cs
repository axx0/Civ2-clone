using System;
using System.Collections.Generic;
using System.Linq;
using Civ2engine.Units;
using Civ2engine.Enums;
using Civ2engine.Events;
using ExtensionMethods;
using System.Diagnostics;

namespace Civ2engine
{
    public partial class Game : BaseInstance
    {
        // Update stats of all cities
        private void CitiesTurn()
        {
            foreach (var city in _activeCiv.Cities)
            {
                // Change food in storage
                city.FoodInStorage += city.SurplusHunger;

                // Change shield production
                city.ShieldsProgress += city.Production;
                
                
                
                // Change city size
                if (city.FoodInStorage > city.MaxFoodInStorage)
                {
                    city.FoodInStorage -= city.MaxFoodInStorage;
                    
                    city.Size += 1;

                    if (city.ImprovementExists(ImprovementType.Granary)) city.FoodInStorage += city.MaxFoodInStorage / 2;

                    AutoAddDistributionWorkers(city);    // Automatically add a workers on a tile
                    city.CalculateOutput(city.Owner.Capital, city.Owner.Government, this);
                }
                else if (city.FoodInStorage < 0)
                {
                    city.FoodInStorage = 0;
                    city.Size -= 1;
                    AutoRemoveWorkersDistribution(city);
                    city.CalculateOutput(city.Owner.Capital, city.Owner.Government, this);
                }
            }
        }

        private void AutoRemoveWorkersDistribution(City city)
        {
            //TODO: remove scuentists & taxmen first
            var tiles = city.WorkedTiles.Where(t => t != city.Location);
            
            var organization = city.OrganizationLevel;
            var hasSupermarket = city.ImprovementExists(ImprovementType.Supermarket);
            var hasSuperhighways = city.ImprovementExists(ImprovementType.Superhighways);

            var unworked = tiles.OrderBy(t =>
                t.GetFood(organization == 0, hasSupermarket) + t.GetShields(organization == 0) +
                t.GetTrade(organization, hasSuperhighways)).First();

            city.WorkedTiles.Remove(unworked);
        }

        public void AutoAddDistributionWorkers(City city)
        {
            // First determine how many workers are to be added
            int workersToBeAdded = city.Size + 1 - city.WorkedTiles.Count;

            var organization = city.OrganizationLevel;
            var hasSupermarket = city.ImprovementExists(ImprovementType.Supermarket);
            var hasSuperhighways = city.ImprovementExists(ImprovementType.Superhighways);

            // Make a list of tiles where you can add workers
            var tilesToAddWorkersTo = Map.CityRadius(city.Location).Where(t =>
                    t.WorkedBy == null && t.Visibility[city.OwnerId] &&
                    !t.UnitsHere.Any(u => u.Owner != city.Owner && u.AttackBase > 0) && t.CityHere == null)
                .OrderByDescending(t =>
                    t.GetFood(organization == 0, hasSupermarket) + t.GetShields(organization == 0) +
                    t.GetTrade(organization, hasSuperhighways)).Take(workersToBeAdded).ToList();

            tilesToAddWorkersTo.ForEach(t => t.WorkedBy = city);
        }
    }
}
