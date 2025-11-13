using System.Linq;

namespace Civ2engine.Statistics
{
    public static class Power
    {
        /// <summary>
        /// Power rating (also used for power graph)
        /// Calculated as a sum of:
        /// - rating from no of researched techs, including future techs (every 8 techs increase rating by 3)
        /// - rating from population (rating = sum of cities sizes)
        /// - rating from gold (every 256 gold increases rating by 1)
        /// In original limited to max 255. 
        /// Some strange behavior in the original, e.g. score reset to 0 once score from advances reaches 100.
        /// </summary>
        /// <param name="game"></param>
        public static void CalculatePowerRatings(Game game)
        {
            
            foreach (var civilization in game.AllCivilizations)
            {
                // TODO: count in future techs
                civilization.PowerRating.Add(
                    civilization.Cities.Sum(c=>c.Size) + 
                    civilization.Money / 256 + 
                    civilization.Advances.Count(c => c) * 3 / 8);
            }

            foreach (var pair
                in game.AllCivilizations.OrderBy(c => c.PowerRating.Last()).ThenBy(c => c.Id)
                    .Select((civilization, i) => new { civilization, i }))
            {
                pair.civilization.PowerRank = pair.i;
            }
        }
    }
}