using System;
using System.Collections.Generic;
using System.Linq;
using Civ2engine;
using Civ2engine.Advances;
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

        public void SelectNewAdvance(Game game, Civilization activeCiv, List<Advance> researchPossibilities)
        {
            var popup = _main.popupBoxList["RESEARCH"];
            var dialog = new Civ2dialog(_main, popup, new List<string> { "wise men" },
                listbox: new ListboxDefinition { LeftText = researchPossibilities.Select(a => a.Name).ToList() });
            dialog.ShowModal();
            activeCiv.ReseachingAdvance = researchPossibilities[0].Index;
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