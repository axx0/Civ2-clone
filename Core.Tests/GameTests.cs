using System.Runtime.Serialization;
using Civ2engine;
using Civ2engine.Enums;
using Civ2engine.IO;
using Civ2engine.MapObjects;
using Civ2engine.Terrains;
using Model.Core;
using Model.Core.Player;
using Moq;

namespace Core.Tests;

public class GameTests
{
    public GameTests()
    {
        Labels.Items = new string[1000];
        for (int i = 0; i < 1000; i++) Labels.Items[i] = "Label " + i;
    }

    [Fact]
    public void GetRealmName_ReturnsCorrectLabels()
    {
        Labels.Items[(int)LabelIndex.Empire] = "Empire";
        Labels.Items[(int)LabelIndex.Kingdom] = "Kingdom";
        Labels.Items[(int)LabelIndex.PeoplesRepublic] = "People's Republic";
        Labels.Items[(int)LabelIndex.HolyEmpire] = "Holy Empire";
        Labels.Items[(int)LabelIndex.Republic] = "Republic";
        
        var game = (Game)FormatterServices.GetUninitializedObject(typeof(Game));
        
        Assert.Equal("Empire", game.GetRealmName(0));
        Assert.Equal("Empire", game.GetRealmName(1));
        Assert.Equal("Kingdom", game.GetRealmName(2));
        Assert.Equal("People's Republic", game.GetRealmName(3));
        Assert.Equal("Holy Empire", game.GetRealmName(4));
        Assert.Equal("Republic", game.GetRealmName(5));
        Assert.Equal("Republic", game.GetRealmName(6));
    }

    [Fact]
    public void Order2String_ReturnsOrderName_WhenFound()
    {
        var game = (Game)FormatterServices.GetUninitializedObject(typeof(Game));
        var rules = new Rules
        {
            Orders = new Orders[] { new Orders { Type = 1, Name = "Build" } }
        };

        typeof(Game).GetField("_rules", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(game, rules);
            
        Assert.Equal("Build", game.Order2String(1));
    }

    [Fact]
    public void Order2String_ReturnsNoOrdersLabel_WhenNotFound()
    {
        Labels.Items[(int)LabelIndex.NoOrders] = "No Orders";
        var game = (Game)FormatterServices.GetUninitializedObject(typeof(Game));
        var rules = new Rules
        {
            Orders = new Orders[0]
        };

        typeof(Game).GetField("_rules", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(game, rules);
            
        Assert.Equal("No Orders", game.Order2String(99));
    }

    [Fact]
    public void TotalMapArea_CalculatesCorrectly()
    {
        var game = (Game)FormatterServices.GetUninitializedObject(typeof(Game));
        var map1 = new Map(true, 0) { Tile = new Tile[10, 20], XDim = 10, YDim = 20 };
        var map2 = new Map(true, 1) { Tile = new Tile[5, 5], XDim = 5, YDim = 5 };
        
        typeof(Game).GetField("_maps", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(game, new Map[] { map1, map2 });
            
        Assert.Equal(225, game.TotalMapArea);
        Assert.Equal(2, game.NoMaps);
    }

    [Fact]
    public void SetHumanPlayer_SetsCorrectPlayerTypes()
    {
        var game = (Game)FormatterServices.GetUninitializedObject(typeof(Game));
        var civs = new List<Civilization>();
        typeof(Game).GetField("<AllCivilizations>k__BackingField", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(game, civs);
            
        var civ0 = new Civilization { Id = 0, TribeName = "Barbarians" };
        var civ1 = new Civilization { Id = 1, TribeName = "Romans" };
        var civ2 = new Civilization { Id = 2, TribeName = "Greeks" };
        
        civs.Add(civ0);
        civs.Add(civ1);
        civs.Add(civ2);
        
        game.SetHumanPlayer(1);
        
        Assert.Equal(PlayerType.Barbarians, civ0.PlayerType);
        Assert.Equal(PlayerType.Local, civ1.PlayerType);
        Assert.Equal(PlayerType.Ai, civ2.PlayerType);
    }
    [Fact]
    public void TriggerMapEvent_CallsMapChangedOnAllPlayers()
    {
        var game = (Game)FormatterServices.GetUninitializedObject(typeof(Game));
        var player1 = new Mock<IPlayer>();
        var player2 = new Mock<IPlayer>();
        typeof(Game).GetField("<Players>k__BackingField", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(game, new IPlayer[] { player1.Object, player2.Object });
            
        var civ1 = new Civilization { Id = 0 };
        var civ2 = new Civilization { Id = 1 };
        player1.Setup(p => p.Civilization).Returns(civ1);
        player2.Setup(p => p.Civilization).Returns(civ2);
        
        var map = new Map(true, 0);
        var tile = new Tile(0, 0, new Terrain { Specials = [] }, 0, map, 0, new bool[2]);
        var tiles = new List<Tile> { tile };
        
        game.TriggerMapEvent(MapEventType.UpdateMap, tiles);
        
        player1.Verify(p => p.MapChanged(tiles), Times.Once);
        player2.Verify(p => p.MapChanged(tiles), Times.Once);
    }
}
