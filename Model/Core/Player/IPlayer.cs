using System;
using System.Collections.Generic;
using Civ2engine.Advances;
using Civ2engine.Enums;
using Civ2engine.Events;
using Civ2engine.MapObjects;
using Civ2engine.Production;
using Civ2engine.Units;
using Model.Core;
using Model.Core.Advances;
using Model.Core.Units;

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

        /// <summary>
        /// Set current unit as active unit, and move it if the move parameter is true.
        ///  If the unit parameter is null, set ActiveUnit to null.
        /// </summary>
        /// <param name="unit"></param>
        /// <param name="move"></param>
        /// <returns></returns>
        void SetUnitActive(Unit? unit, bool move);
        void UnitLost(Unit unit, Unit? killedBy);
        
        void UnitsLost(List<Unit> deadUnits, Unit? killedBy);
        
        /// <summary>
        /// Called to notify the player that a unit has moved.
        /// </summary>
        /// <param name="unit"></param>
        /// <param name="tileTo"></param>
        /// <param name="tileFrom"></param>
        void UnitMoved(Unit unit, Tile tileTo, Tile tileFrom);

        /// <summary>
        /// Called when a combat happens between two units that we can see, not necessarily our units
        /// </summary>
        /// <param name="combatEventArgs"></param>
        void CombatHappened(CombatEventArgs combatEventArgs);
        
        /// <summary>
        /// Called when a move order can't be followed and has been canceled. 
        /// </summary>
        /// <param name="unit"></param>
        /// <param name="blockedReason"></param>
        void MoveBlocked(Unit unit, BlockedReason blockedReason);
    }   

    public interface IInterfaceCommands
    {
        void ShowDialog(string dialogKey);
        Tuple<string, int, List<bool>> ShowDialog(PopupBox popupBox, List<bool> checkBoxOptionStates = null);
        void SavePopup(string key, PopupBox popup);
    }
}