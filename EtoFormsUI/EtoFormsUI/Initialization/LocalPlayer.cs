using System;
using System.Collections.Generic;
using Civ2engine;
using Civ2engine.Improvements;
using Civ2engine.Production;

namespace EtoFormsUI
{
    public class LocalPlayer : IPlayer
    {
        private readonly Main _main;

        public LocalPlayer(Main main)
        {
            _main = main;
            UI = new UserInterfaceCommands(main);
        }
        public void CivilDisorder(City city)
        {
            _main.ShowCityDialog("DISORDER", new [] { city.Name });
        }

        public void OrderRestored(City city)
        {
            _main.ShowCityDialog("RESTORED", new [] { city.Name });
        }

        public void WeLoveTheKingStarted(City city)
        {
            _main.ShowCityDialog("WELOVEKING", new [] { city.Name, city.Owner.LeaderTitle });
        }

        public void WeLoveTheKingCanceled(City city)
        {
            _main.ShowCityDialog("WEDONTLOVEKING", new [] { city.Name, city.Owner.LeaderTitle });
        }

        public void CantMaintain(City city, Improvement cityImprovement)
        {
            throw new NotImplementedException();
        }

        public void SelectNewAdvance(Game game, Civilization activeCiv, IList<int> researchPossibilities)
        {
            throw new NotImplementedException();
        }

        public void CantProduce(City city, ProductionOrder newItem)
        {
            throw new NotImplementedException();
        }

        public void CityProductionComplete(City city)
        {
            
        }

        public IInterfaceCommands UI { get; } 
    }
}