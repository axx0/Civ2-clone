using Civ2engine.Enums;
using Civ2engine.MapObjects;
using Civ2engine.UnitActions;
using Model.Core;
using Model.Core.GameRules;
using Model.Core.Mapping;
using Model.Core.Player;
using Model.Core.Units;
using Moq;

namespace Core.Tests.Units;

public class MovementVisibilityTests
{
    [Theory]
    [InlineData(false, false)]
    [InlineData(true, false)]
    [InlineData(true, true)]
    public void MovingRevealsDestinationBeforeMapAndAnimationNotifications(bool goTo, bool observerKnowsDestination)
    {
        var map = new Map(true, 5) { XDim = 8, YDim = 6, Tile = new Tile[8, 6] };
        var terrain = new Terrain { Type = TerrainType.Plains, Specials = [], MoveCost = 1 };
        for (var x = 0; x < map.XDim; x++)
        for (var y = 0; y < map.YDim; y++)
            map.Tile[x, y] = new Tile(x * 2 + y % 2, y, terrain, 5, map, x, new bool[2]);
        var from = map.Tile[2, 2];
        var to = map.Tile[3, 2];
        var civ = new Civilization { Id = 0 };
        var unit = new Unit
        {
            Owner = civ, CurrentLocation = from, X = from.X, Y = from.Y,
            TypeDefinition = new UnitDefinition { Domain = UnitGas.Ground, Move = 6, Flags = new bool[15] },
            Order = (int)(goTo ? OrderType.GoTo : OrderType.NoOrders)
        };
        civ.Units.Add(unit);
        from.SetVisible(0);
        to.SetVisible(1, observerKnowsDestination);
        var player = new Mock<IPlayer>();
        var observer = new Mock<IPlayer>();
        var game = new Mock<IGame>();
        game.SetupGet(g => g.Rules).Returns(new Rules());
        game.SetupGet(g => g.Players).Returns([player.Object, observer.Object]);
        game.SetupGet(g => g.ActivePlayer).Returns(player.Object);
        var mapRefreshed = false;
        game.Setup(g => g.UpdateTiles(It.IsAny<IList<Tile>>())).Callback<IList<Tile>>(tiles =>
        {
            Assert.Contains(from, tiles);
            Assert.Contains(to, tiles);
            Assert.True(to.IsVisible(0));
            Assert.All(to.Neighbours(), tile => Assert.True(tile.IsVisible(0)));
            Assert.Equal(tiles.Count, tiles.Distinct().Count());
            mapRefreshed = true;
        });
        player.Setup(p => p.UnitMoved(unit, to, from)).Callback(() =>
        {
            Assert.True(mapRefreshed);
            Assert.True(to.IsVisible(0));
        });

        MovementFunctions.ExecuteUnitMove(game.Object, unit, to, from);

        Assert.Same(to, unit.CurrentLocation);
        player.Verify(p => p.UnitMoved(unit, to, from), Times.Once);
        observer.Verify(p => p.UnitMoved(unit, to, from), observerKnowsDestination ? Times.Once() : Times.Never());
        Assert.False(map.Tile[7, 5].IsVisible(0));
        Assert.False(map.Tile[2, 3].IsVisible(1));
        Assert.Equal((int)(goTo ? OrderType.GoTo : OrderType.NoOrders), unit.Order);
    }
}
