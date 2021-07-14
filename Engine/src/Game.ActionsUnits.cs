using System;
using Civ2engine.Units;
using Civ2engine.Enums;
using Civ2engine.Events;

namespace Civ2engine
{
    public partial class Game : BaseInstance
    {
        public static event EventHandler<MapEventArgs> OnMapEvent;
        public static event EventHandler<UnitEventArgs> OnUnitEvent;

        // Choose next unit for orders. If all units ended turn, update cities.
        public void ChooseNextUnit()
        {
            Unit nextUnit = null;
            var units = _activeCiv.Units;
            int unitIndex;
            for (unitIndex = _activeUnit.Id + 1; unitIndex < units.Count && nextUnit == null; unitIndex++)
            {
                if (!units[unitIndex].Dead && units[unitIndex].Owner == _activeCiv && units[unitIndex].AwaitingOrders)
                {
                    nextUnit = units[unitIndex];
                }
            }

            for (unitIndex = 0; nextUnit == null && unitIndex < _activeUnit.Id; unitIndex++)
            {
                if (!units[unitIndex].Dead && units[unitIndex].Owner == _activeCiv && units[unitIndex].AwaitingOrders)
                {
                    nextUnit = units[unitIndex];
                }
            }
            // End turn if no units awaiting orders
            if (nextUnit == null)
            {
                if (Options.AlwaysWaitAtEndOfTurn && _activeCiv == _playerCiv)
                {
                    OnMapEvent?.Invoke(null, new MapEventArgs(MapEventType.WaitAtEndOfTurn));
                }
                else
                {
                    ChoseNextCiv();
                }
            }
            // Choose next unit
            else
            {
                _activeUnit = nextUnit;
                
                OnUnitEvent?.Invoke(null, new UnitEventArgs(UnitEventType.StatusUpdate));
            }
        }
    }
}
