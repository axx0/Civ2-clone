using System;
using System.Collections.Generic;
using Civ2engine.Advances;
using Civ2engine.Improvements;
using Civ2engine.MapObjects;
using Civ2engine.Production;
using Civ2engine.Units;

namespace Civ2engine
{
    public interface IPlayer
    {
        Civilization Civilization { get; }
        Tile ActiveTile { get; set; }
        
        Unit ActiveUnit { get; set; }
        
        List<Unit> WaitingList { get; }

        void CivilDisorder(City city);
        void OrderRestored(City city);
        void WeLoveTheKingStarted(City city);
        void WeLoveTheKingCanceled(City city);
        void CantMaintain(City city, Improvement cityImprovement);
        void SelectNewAdvance(Game game, List<Advance> researchPossibilities);
        
        void CantProduce(City city, ProductionOrder newItem);
        
        void CityProductionComplete(City city);
        IInterfaceCommands UI { get; }
        void NotifyImprovementEnabled(TerrainImprovement improvement, int level);
    }   

    public interface IInterfaceCommands
    {
        void ShowDialog(string dialogKey);
        Tuple<string, int, List<bool>> ShowDialog(PopupBox popupBox, List<bool> checkBoxOptionStates = null);
        void SavePopup(string key, PopupBox popup);
    }
}