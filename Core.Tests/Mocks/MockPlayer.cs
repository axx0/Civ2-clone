using Civ2engine;
using Civ2engine.Enums;
using Civ2engine.Events;
using Civ2engine.MapObjects;
using Model.Core.Advances;
using Model.Core.GoodyHuts.Outcomes;
using Model.Core.Player;
using Model.Core.Units;

namespace Core.Tests.Mocks;

public class MockPlayer : IPlayer
{
    public MockPlayer(Civilization civ)
    {
        Civilization = civ;
    }

    public Civilization Civilization { get; }
    public Tile ActiveTile { get; set; }
    public Unit? ActiveUnit { get; set; }
    public List<Unit> WaitingList { get; } = new();

    public bool MoveBlockedCalled { get; private set; }
    public BlockedReason LastBlockedReason { get; private set; }

    public void CivilDisorder(City city)
    {
    }

    public void OrderRestored(City city)
    {
    }

    public void WeLoveTheKingStarted(City city)
    {
    }

    public void WeLoveTheKingCanceled(City city)
    {
    }

    public void CantMaintain(City city, Improvement cityImprovement)
    {
    }

    public void SelectNewAdvance(List<Advance> researchPossibilities)
    {
    }

    public void CantProduce(City city, IProductionOrder? newItem)
    {
    }

    public void CityProductionComplete(City city)
    {
    }

    public IInterfaceCommands Ui { get; set; }

    public void NotifyImprovementEnabled(TerrainImprovement improvement, int level)
    {
    }

    public void MapChanged(List<Tile> tiles)
    {
    }

    public void WaitingAtEndOfTurn()
    {
    }

    public void NotifyAdvanceResearched(int advance)
    {
    }

    public void FoodShortage(City city)
    {
    }

    public void CityDecrease(City city)
    {
    }

    public void TurnStart(int turnNumber)
    {
    }

    public void SetUnitActive(Unit? unit, bool move)
    {
        ActiveUnit = unit;
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
    }

    public void MoveBlocked(Unit unit, BlockedReason blockedReason)
    {
        MoveBlockedCalled = true;
        LastBlockedReason = blockedReason;
    }

    public void GoodyHutTriggered(Unit unit, GoodyHutOutcomeResult outcome)
    {
    }
}
