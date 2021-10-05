using System;
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
        }
        public void CivilDisorder(City city)
        {
            var dialog = _main.ShowCityDialog( city);
        }

        public void OrderRestored(City city)
        {
            throw new NotImplementedException();
        }

        public void WeLoveTheKingStarted(City city)
        {
            throw new NotImplementedException();
        }

        public void WeLoveTheKingCanceled(City city)
        {
            throw new NotImplementedException();
        }

        public void CantMaintain(City city, Improvement cityImprovement)
        {
            throw new NotImplementedException();
        }

        public void SelectNewAdvance(Game game, Civilization activeCiv)
        {
            throw new NotImplementedException();
        }

        public void CantProduce(City city, ProductionOrder newItem)
        {
            throw new NotImplementedException();
        }

        public void CityProductionComplete(City city)
        {
            throw new NotImplementedException();
        }
    }
}