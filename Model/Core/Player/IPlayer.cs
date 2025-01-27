using System;
using System.Collections.Generic;
using Civ2engine.Advances;
using Civ2engine.MapObjects;
using Civ2engine.Production;
using Civ2engine.Units;
using Model.Core;
using Model.Core.Advances;

namespace Civ2engine
{
    public interface IPlayer
    {
        Civilization Civilization { get; }
        Tile ActiveTile { get; set; }

        Unit? ActiveUnit { get; }
        
        List<Unit> WaitingList { get; }

        void CivilDisorder(City city);
        void OrderRestored(City city);
        void WeLoveTheKingStarted(City city);
        void WeLoveTheKingCanceled(City city);
        void CantMaintain(City city, Improvement cityImprovement);
        void SelectNewAdvance(IGame game, List<Advance> researchPossibilities);
        
        void CantProduce(City city, IProductionOrder? newItem);
        
        void CityProductionComplete(City city);
        IInterfaceCommands Ui { get; }
        
        void NotifyImprovementEnabled(TerrainImprovement improvement, int level);
        void MapChanged(List<Tile> tiles);
        void WaitingAtEndOfTurn(IGame game);
        void NotifyAdvanceResearched(int advance);
        void FoodShortage(City city);
        void CityDecrease(City city);
        void TurnStart(int turnNumber);
        void SetUnitActive(Unit? unit, bool move);
    }   

    public interface IInterfaceCommands
    {
        void ShowDialog(string dialogKey);
        Tuple<string, int, List<bool>> ShowDialog(PopupBox popupBox, List<bool> checkBoxOptionStates = null);
        void SavePopup(string key, PopupBox popup);
    }
}