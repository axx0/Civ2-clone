using System.Collections.Generic;
using System.Linq;
using Civ2engine.Units;
using Civ2engine.Enums;
using Civ2engine.Advances;
using Civ2engine.MapObjects;
using Civ2engine.Production;
using Model.Core;

namespace Civ2engine
{
    public partial class Game : IGame
    {
        // Update stats of all cities
        private void CitiesTurn(IPlayer player)
        {
            var currentScienceCost = AdvanceFunctions.CalculateScienceCost(this, _activeCiv);

            var foodRows = Rules.Cosmic.RowsFoodBox;
            var shieldRows = Rules.Cosmic.RowsShieldBox;
            
            foreach (var city in _activeCiv.Cities)
            {
                city.ImprovementSold = false;
                
                // Change food in storage
                city.FoodInStorage += city.SurplusHunger;

                var shields = city.Production;

                //TODO: Combine these calls
                var tax = city.GetTax();
                var science = city.GetScience();
                
                // Change city size
                if (city.FoodInStorage < 0)
                {
                    city.FoodInStorage = 0;
                    city.ShrinkCity(this);
                }
                else
                {
                    var maxFood = (city.Size + 1) * foodRows;
                    if (city.FoodInStorage > maxFood)
                    {
                        city.GrowCity(this);
                        city.ResetFoodStorage(foodRows);
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

                    if (city.CivilDisorder)
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

                    if (newItem != null)
                    {
                        city.ItemInProduction = newItem;
                    }
                }

                city.ShieldsProgress += shields;
                
                
                if (city.ShieldsProgress >= city.ItemInProduction.Cost * shieldRows)
                {
                    if (city.ItemInProduction.CompleteProduction(city, Rules))
                    {
                        city.ShieldsProgress = 0;
                        player.CityProductionComplete(city);
                    }
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
                    if (_activeCiv.ReseachingAdvance < 0)
                    {
                        var researchPossibilities = AdvanceFunctions.CalculateAvailableResearch(this, _activeCiv);
                        player.SelectNewAdvance(this, researchPossibilities);
                        currentScienceCost = AdvanceFunctions.CalculateScienceCost(this, _activeCiv);
                    }else if (currentScienceCost <= _activeCiv.Science)
                    {
                        this.GiveAdvance(_activeCiv.ReseachingAdvance, _activeCiv);
                        _activeCiv.Science -= currentScienceCost;
                    }
                }
            }
        }
    }
}
