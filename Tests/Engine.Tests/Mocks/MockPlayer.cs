using Civ2engine;
using Civ2engine.Enums;
using Civ2engine.Events;
using Civ2engine.MapObjects;
using Model.Core;
using Model.Core.Advances;
using Model.Core.Units;

namespace Engine.Tests;

public class MockPlayer : IPlayer
{
    public MockPlayer(Civilization civ)
    {
        Civilization = civ;
    }

    public Civilization Civilization { get; }
    public Tile ActiveTile { get; set; }
    public Unit? ActiveUnit { get; }
    public List<Unit> WaitingList { get; }
    public void CivilDisorder(City city)
    {
        throw new NotImplementedException();
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

    public void SelectNewAdvance(IGame game, List<Advance> researchPossibilities)
    {
        throw new NotImplementedException();
    }

    public void CantProduce(City city, IProductionOrder? newItem)
    {
        throw new NotImplementedException();
    }

    public void CityProductionComplete(City city)
    {
        throw new NotImplementedException();
    }

    public IInterfaceCommands Ui { get; }
    public void NotifyImprovementEnabled(TerrainImprovement improvement, int level)
    {
        throw new NotImplementedException();
    }

    public void MapChanged(List<Tile> tiles)
    {
        throw new NotImplementedException();
    }

    public void WaitingAtEndOfTurn(IGame game)
    {
        throw new NotImplementedException();
    }

    public void NotifyAdvanceResearched(int advance)
    {
        throw new NotImplementedException();
    }

    public void FoodShortage(City city)
    {
        throw new NotImplementedException();
    }

    public void CityDecrease(City city)
    {
        throw new NotImplementedException();
    }

    public void TurnStart(int turnNumber)
    {
        throw new NotImplementedException();
    }

    public void SetUnitActive(Unit? unit, bool move)
    {
        throw new NotImplementedException();
    }

    public void UnitLost(Unit unit, Unit? killedBy)
    {
    }

    public void UnitsLost(List<Unit> deadUnits, Unit? killedBy)
    {
    }

    public void UnitMoved(Unit unit, Tile tileTo, Tile tileFrom)
    {
    }

    public void CombatHappened(CombatEventArgs combatEventArgs)
    {
        throw new NotImplementedException();
    }

    public void MoveBlocked(Unit unit, BlockedReason blockedReason)
    {
        throw new NotImplementedException();
    }
}