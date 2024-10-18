using Civ2engine.Advances;
using Civ2engine.Production;

namespace Civ2engine
{
    public static class GameTurn
    {
        /// <summary>
        /// Updates the stats of all cities for the active player's turn.
        /// </summary>
        /// <param name="game">The game instance.</param>
        /// <param name="player">The active player.</param>
        /// <remarks>
        /// This method performs the following actions for each city:
        /// - Updates food storage and city size based on surplus/deficit
        /// - Handles civil disorder and "We Love the King Day" events
        /// - Manages production and item completion
        /// - Collects taxes and pays for city improvements
        /// - Contributes to research progress
        /// </remarks>
        public static void CitiesTurn(this Game game, IPlayer player)
        {
            var activeCiv = game.GetActiveCiv;
            var currentScienceCost = AdvanceFunctions.CalculateScienceCost(game, activeCiv);

            var rules = game.Rules;
            
            var foodRows = rules.Cosmic.RowsFoodBox;
            var shieldRows = rules.Cosmic.RowsShieldBox;

            foreach (var city in activeCiv.Cities)
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
                    city.ShrinkCity(game);
                    player.CityDecrease(city);
                }
                else if (city.SurplusHunger < 0 && city.FoodInStorage + city.SurplusHunger < 0)
                {
                    player.FoodShortage(city);
                }
                else
                {
                    var maxFood = (city.Size + 1) * foodRows;
                    if (city.FoodInStorage > maxFood)
                    {
                        city.GrowCity(game);
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
                    if (city.ItemInProduction.CompleteProduction(city, rules))
                    {
                        city.ShieldsProgress = 0;
                        player.CityProductionComplete(city);
                    }
                }

                activeCiv.Money += tax;

                foreach (var cityImprovement in city.Improvements)
                {
                    if (cityImprovement.Upkeep > 0)
                    {
                        if (activeCiv.Money >= cityImprovement.Upkeep)
                        {
                            activeCiv.Money -= cityImprovement.Upkeep;
                        }
                        else
                        {
                            //Sell it !!
                            city.SellImprovement(cityImprovement);
                            activeCiv.Money += cityImprovement.Cost;
                            player.CantMaintain(city, cityImprovement);
                        }
                    }
                }

                if (science > 0)
                {
                    activeCiv.Science += science;
                    if (activeCiv.ReseachingAdvance < 0)
                    {
                        var researchPossibilities = AdvanceFunctions.CalculateAvailableResearch(game, activeCiv);
                        player.SelectNewAdvance(game, researchPossibilities);
                        currentScienceCost = AdvanceFunctions.CalculateScienceCost(game, activeCiv);
                    }
                    else if (currentScienceCost <= activeCiv.Science)
                    {
                        player.NotifyAdvanceResearched(activeCiv.ReseachingAdvance);
                        game.GiveAdvance(activeCiv.ReseachingAdvance, activeCiv);
                        activeCiv.Science -= currentScienceCost;
                    }
                }
            }
        }
    }
}
