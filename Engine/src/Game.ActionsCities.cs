using System;
using System.Collections.Generic;
using System.Linq;
using Civ2engine.Units;
using Civ2engine.Enums;
using Civ2engine.Events;
using ExtensionMethods;

namespace Civ2engine
{
    public partial class Game : BaseInstance
    {
        // Update stats of all cities
        private void CitiesTurn()
        {
            foreach (City city in _cities.Where(a => a.Owner == _activeCiv))
            {
                // Change food in storage
                city.FoodInStorage += city.SurplusHunger;

                // Change shield production
                city.ShieldsProgress += city.Production;
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
    }
}
