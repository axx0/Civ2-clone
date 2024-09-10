using System.Linq;

namespace Civ2engine.Statistics
{
    public static class Power
    {
        /// <summary>
        /// No idea if this is the correct formula and we should be storing the values to make a power graph. I just need something to use as a basis for key civ for now
        /// </summary>
        /// <param name="game"></param>
        public static void CalculatePowerRatings(Game game)
        {
            foreach (var civilization in game.AllCivilizations)
            {
                civilization.PowerRating = civilization.Cities.Count * 2 + civilization.Cities.Sum(c=>c.GetPopulation());
            }

            foreach (var pair
                in game.AllCivilizations.OrderBy(c => c.PowerRating).ThenBy(c => c.Id)
                    .Select((civilization, i) => new { civilization, i }))
            {
                pair.civilization.PowerRank = pair.i;
            }
        }
    }
}