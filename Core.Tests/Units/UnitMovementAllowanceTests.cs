using Civ2engine.Enums;
using Model.Core.Units;

namespace Core.Tests.Units;

public class UnitMovementAllowanceTests
{
    // Horsemen-style example from the CivFanatics thread: M = 6, H = 10, road multiplier r = 3

    [Fact]
    public void FullHitpoints_KeepsFullMove()
    {
        Assert.Equal(6, Unit.MovementAllowance(6, 10, 10, 3, UnitGas.Ground));
    }

    [Theory]
    [InlineData(5, 3)]   // floor(5 * 6 / 10) = 3, already a multiple of 3
    [InlineData(4, 3)]   // floor(4 * 6 / 10) = 2, rounded up to 3
    [InlineData(1, 3)]   // floor(0.6) = 0, minimum of one road move
    public void LandUnit_DamagedMovesLess(int hitpoints, int expected)
    {
        Assert.Equal(expected, Unit.MovementAllowance(6, 10, hitpoints, 3, UnitGas.Ground));
    }

    [Fact]
    public void AirUnit_KeepsFullMoveWhenDamaged()
    {
        Assert.Equal(6, Unit.MovementAllowance(6, 10, 1, 3, UnitGas.Air));
    }

    [Fact]
    public void SeaUnit_MinimumIsTwoRoadMoves()
    {
        Assert.Equal(6, Unit.MovementAllowance(9, 10, 1, 3, UnitGas.Sea));
    }

    [Fact]
    public void ZeroMove_StaysZero()
    {
        Assert.Equal(0, Unit.MovementAllowance(0, 10, 5, 3, UnitGas.Ground));
    }

    [Fact]
    public void DamagedUnit_MovePointsUseReducedAllowance()
    {
        var unit = new Unit
        {
            TypeDefinition = new UnitDefinition
            {
                Move = 6, Hitp = 10, Domain = UnitGas.Ground, MovementMultiplier = 3
            },
            HitPointsLost = 5
        };

        Assert.Equal(3, unit.MovePoints);
    }
}
