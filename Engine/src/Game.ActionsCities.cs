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
            foreach (City city in GetCities.Where(a => a.Owner == _activeCiv))
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
                }
                else if (city.FoodInStorage < 0)
                {
                    city.FoodInStorage = 0;
                    city.Size -= 1;
                }
            }
        }

        public void BuildCity(string cityName)
        {
            int x = _activeUnit.X;
            int y = _activeUnit.Y;
            bool[] improvements = new bool[34];
            bool[] wonders = new bool[28];
            for (int i = 0; i < 34; i++) improvements[i] = false;
            for (int i = 0; i < 28; i++) wonders[i] = false;
            //Game.CreateCity(x, y, false, false, false, false, false, false, false, false, false, Game.Instance.ActiveUnit.Civ, 1, Game.Instance.ActiveUnit.Civ, 0, 0, 0, cityName, 0, 0, 0, 0, improvements, 0, 0, 0, 0, 0, 0, 0, 0, 0, wonders);

            DeleteUnit(_activeUnit);
        }

        private void AutoAddDistributionWorkers(City city)
        {
            int[,] offsets = new int[21, 2] { { 0, 0 }, { -1, -3 }, { -3, -1 }, { -3, 1 }, { -1, 3 }, { 1, 3 }, { 3, 1 }, { 3, -1 }, { 1, -3 }, { -2, -2 }, { -2, 2 }, { 2, 2 }, { 2, -2 }, { 0, -2 }, { -1, -1 }, { -2, 0 }, { -1, 1 }, { 0, 2 }, { 1, 1 }, { 2, 0 }, { 1, -1 } };    // Offset of squares from city square (0,0)

            // First determine how many workers are to be added
            int workersToBeAdded = city.Size + 1 - city.DistributionWorkers.Count(c => c);

            // Make a list of tiles where you can add workers
            Dictionary<int[], int> tileOffsetWhereWorkersCanBeAddedDict = new Dictionary<int[], int>();
            int x, y;
            for (int i = 0; i < 21; i++)
            {
                x = city.X + offsets[i, 0];
                y = city.Y + offsets[i, 1];

                // TODO: block tiles already occupied by workers from other cities
                if (i != 0 && // Home city is here
                    Map.IsTileVisibleC2(x, y, city.Owner.Id) &&   // Tile must be visible
                    !Game.UnitsHere(x, y).Any(u => u.Owner != city.Owner && u.Type != UnitType.Settlers && u.Type != UnitType.Engineers && u.Type != UnitType.Spy && u.Type != UnitType.Diplomat && u.Type != UnitType.Explorer) &&    // Tile mustn't have enemy unit (certain units allowed)
                    Game.CityHere(x, y) == null &&  // Tile mustn't be occupied by other city
                    !city.DistributionWorkers[i])   // Tile mustn't already have workers
                {
                    tileOffsetWhereWorkersCanBeAddedDict.Add(new int[] { offsets[i, 0], offsets[i, 1] }, Map.TileC2(x, y).Food + Map.TileC2(x, y).Shields + Map.TileC2(x, y).Trade);
                }
            }

            // Sort tiles where workers will be added according to (food+shields+trade) in descending order
            List<KeyValuePair<int[], int>> tileOffsetWhereWorkersCanBeAdded = tileOffsetWhereWorkersCanBeAddedDict.ToList();
            tileOffsetWhereWorkersCanBeAdded.Sort((pair1, pair2) => pair2.Value.CompareTo(pair1.Value));
            foreach (var offset in tileOffsetWhereWorkersCanBeAdded)
            {
                if (workersToBeAdded > 0)
                {
                    for (int i = 0; i < 21; i++)
                    {
                        if (offsets[i, 0] == offset.Key[0] && offsets[i, 1] == offset.Key[1]) city.DistributionWorkers[i] = true;
                    }
                    workersToBeAdded -= 1;
                }
                else break;
            }
        }
    }
}
