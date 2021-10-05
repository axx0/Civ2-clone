using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Civ2engine.Units;
using Civ2engine.Enums;
using Civ2engine.Events;
using ExtensionMethods;
using System.Diagnostics;
using Civ2engine.Advances;
using Civ2engine.Production;

namespace Civ2engine
{
    public partial class Game : BaseInstance
    {
        // Update stats of all cities
        private void CitiesTurn(IPlayer player)
        {
            var currentScienceCost = AdvanceFunctions.CalculateScienceCost(_activeCiv);

            var foodRows = Rules.Cosmic.RowsFoodBox;
            var shieldRows = Rules.Cosmic.RowsShieldBox;
            
            foreach (var city in _activeCiv.Cities)
            {
                city.ImprovementSold = false;
                
                // Change food in storage
                city.FoodInStorage += city.SurplusHunger;

                var shields = city.Production;

                var tax = city.Tax;

                var science = city.Science;
                
                // Change city size
                if (city.FoodInStorage < 0)
                {
                    city.FoodInStorage = 0;
                    city.Size -= 1;
                    AutoRemoveWorkersDistribution(city);
                    city.CalculateOutput(city.Owner.Capital, city.Owner.Government, this);
                }
                else
                {
                    var maxFood = (city.Size + 1) * foodRows;
                    if (city.FoodInStorage > maxFood)
                    {
                        city.FoodInStorage = 0;

                        city.Size += 1;

                        if (city.ImprovementExists(ImprovementType.Granary))
                            city.FoodInStorage += maxFood / 2;

                        AutoAddDistributionWorkers(city); // Automatically add a workers on a tile
                        city.CalculateOutput(city.Owner.Capital, city.Owner.Government, this);
                    }
                }

                if (city.UnhappyCitizens > 0)
                {
                    if (city.WeLoveKingDay)
                    {
                        player.WeLoveTheKingCanceled(city);
                        city.WeLoveKingDay = false;
                    }

                    if (city.UnhappyCitizens > city.HappyCitizens)
                    {
                        player.CivilDisorder(city);
                        city.CivilDisorder = true;
                        continue;
                    }
                    else if (city.CivilDisorder)
                    {
                        city.CivilDisorder = false;
                        player.OrderRestored(city);
                    }

                }
                else
                {
                    if (city.CivilDisorder)
                    {
                        city.CivilDisorder = false;
                        player.OrderRestored(city);
                    }

                    if (city.HappyCitizens >= city.Size - city.UnhappyCitizens - city.HappyCitizens)
                    {
                        if (!city.WeLoveKingDay)
                        {
                            player.WeLoveTheKingStarted(city);
                        }

                        city.WeLoveKingDay = true;
                    }
                }

                if (!ProductionPossibilities.ProductionValid(city))
                {
                    var newItem = ProductionPossibilities.AutoNext(city);

                    player.CantProduce(city, newItem);
                    
                    city.ItemInProduction = newItem;
                }
                
                
                if (city.ShieldsProgress >= city.ItemInProduction.Cost * shieldRows)
                {
                    city.ItemInProduction.CompleteProduction(city, Rules);
                    city.ShieldsProgress = 0;

                    player.CityProductionComplete(city);
                }

                _activeCiv.Money += tax;

                foreach (var cityImprovement in city.Improvements)
                {
                    if (cityImprovement.Upkeep > 0)
                    {
                        if (_activeCiv.Money >= cityImprovement.Upkeep)
                        {
                            _activeCiv.Money -= cityImprovement.Upkeep;
                        }
                        else
                        {
                            //Sell it !!
                            city.SellImprovement(cityImprovement);
                            player.CantMaintain(city, cityImprovement);
                        }
                    }
                }

                if (science > 0)
                {
                    _activeCiv.Science += science;
                    if (currentScienceCost <= _activeCiv.Science)
                    {
                        this.GiveAdvance(_activeCiv.ReseachingAdvance, _activeCiv.Id);
                        _activeCiv.Science -= currentScienceCost;
                        player.SelectNewAdvance(this, _activeCiv);
                        currentScienceCost = AdvanceFunctions.CalculateScienceCost(_activeCiv);
                    }
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
