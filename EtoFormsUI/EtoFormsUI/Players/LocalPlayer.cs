using System;
using System.Collections.Generic;
using System.Linq;
using Civ2engine;
using Civ2engine.Advances;
using Civ2engine.Enums;
using Civ2engine.Improvements;
using Civ2engine.MapObjects;
using Civ2engine.Production;
using Civ2engine.Units;

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

        public Tile ActiveTile
        {
            get => _activeTile;
            set
            {
                if (_activeTile != value)
                {
                    _activeTile = value;
                    _main.Orders.ForEach(o=> o.Update(_activeTile, _activeUnit));
                }
            }
        }

        private Unit _activeUnit;
        public Civilization Civ { get; private set; }
        private Tile _activeTile;

        public Unit ActiveUnit
        {
            get
            {
                return _activeUnit;
            }
            set
            {
                if (value == null)
                {
                    _activeUnit = null;
                }else if (!value.TurnEnded && !value.Dead)
                {
                    _activeTile = value.CurrentLocation;
                    _activeUnit = value;
                    _main.Orders.ForEach(o=> o.Update(_activeTile, _activeUnit));
                }
                else
                {
#if DEBUG
                    throw new NotSupportedException("Tried to set ended unit to active");
#endif
                }
                
            }
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

        public void SelectNewAdvance(Game game, List<Advance> researchPossibilities)
        {
            var popup = _main.popupBoxList["RESEARCH"];
            var dialog = new Civ2dialog(_main, popup, new List<string> { "wise men" },
                listbox: new ListboxDefinition { LeftText = researchPossibilities.Select(a => a.Name).ToList() });
            dialog.ShowModal();
            Civ.ReseachingAdvance = researchPossibilities[dialog.SelectedIndex].Index;
        }

        public void CantProduce(City city, ProductionOrder newItem)
        {
            ShowCityDialog(city, "BADBUILD");
        }

        public void CityProductionComplete(City city)
        {
            ShowCityDialog(city, "BUILT");
        }

        private void ShowCityDialog(City city, string dialogName)
        {
            var popup = _main.popupBoxList[dialogName];
            popup.Options ??= new List<string> { Labels.For(LabelIndex.ZoomToCity), Labels.For(LabelIndex.Continue) };
            var dialog = new Civ2dialog(_main, popup,
                new List<string>
                    { city.Name, city.ItemInProduction.GetDescription(), city.Owner.Adjective, Labels.For(LabelIndex.builds) });
            dialog.ShowModal();
            if (dialog.SelectedIndex == 0)
            {
                _main.mapPanel.ShowCityWindow(city);
            }
        }

        public IInterfaceCommands UI { get; }
        public List<Unit> WaitingList { get; } = new ();

        public IPlayer SetCiv(Civilization civilization)
        {
            Civ = civilization;
            ActiveTile = civilization.Units.FirstOrDefault(u => u.Order == OrderType.NoOrders)?.CurrentLocation ??
                         civilization.Cities.FirstOrDefault()?.Location;
            return this;
        }

        public void NotifyImprovementEnabled(TerrainImprovement improvement, int level)
        {
            var dialogKey = improvement.Levels[level].EnabledMessage;
            if (!string.IsNullOrWhiteSpace(dialogKey))
            {
                UI.ShowDialog(dialogKey);
            }
        }
    }
}