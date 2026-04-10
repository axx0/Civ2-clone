using Civ2engine;
using Civ2engine.Scripting.ScriptObjects;
using Model.Core;
using Model.Core.Advances;
using Model.Core.Player;
using Moq;
using Neo.IronLua;

namespace Core.Tests.Ai;

public class AiPlayerTests
{
    private (Game game, Mock<AiInterface> mockAi, Civilization civ, Rules rules, List<Advance> advances) CreateMocks()
    {
        var mockAiInterface = new Mock<AiInterface>(null, null, 0, null);
        var civilization = new Civilization { Id = 1, TribeName = "Romans" };
        var game = (Game)System.Runtime.Serialization.FormatterServices.GetUninitializedObject(typeof(Game));
        
        var random = new FastRandom(42);
        typeof(Game).GetProperty("Random")?.SetValue(game, random);
        
        var rules = (Rules)System.Runtime.Serialization.FormatterServices.GetUninitializedObject(typeof(Rules));
        typeof(Game).GetField("_rules", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(game, rules);
        
        var advances = new List<Advance>();
        rules.Advances = advances.ToArray();

        return (game, mockAiInterface, civilization, rules, advances);
    }

    [Fact]
    public void SelectNewAdvance_SetsReseachingAdvance_FromTech()
    {
        // Arrange
        var (game, mockAi, civilization, rules, advances) = CreateMocks();
        var aiPlayer = new AiPlayer(0, civilization, null, game, mockAi.Object);

        var advance = new Advance { Index = 0, Name = "Writing" };
        advances.Add(advance);
        rules.Advances = advances.ToArray();
        var possibilities = new List<Advance> { advance };
        
        var tech = new Tech(rules.Advances, 0);
        var luaResult = new LuaResult(tech);

        mockAi.Setup(ai => ai.Call(AiEvent.ResearchComplete, It.IsAny<LuaTable>()))
              .Returns(luaResult);

        // Act
        aiPlayer.SelectNewAdvance(possibilities);

        // Assert
        Assert.Equal(0, civilization.ReseachingAdvance);
    }

    [Fact]
    public void SelectNewAdvance_SetsReseachingAdvance_FromAdvance()
    {
        // Arrange
        var (game, mockAi, civilization, rules, advances) = CreateMocks();
        var aiPlayer = new AiPlayer(0, civilization, null, game, mockAi.Object);

        var advance = new Advance { Index = 0, Name = "Writing" };
        advances.Add(advance);
        rules.Advances = advances.ToArray();
        var possibilities = new List<Advance> { advance };
        
        var luaResult = new LuaResult(advance);

        mockAi.Setup(ai => ai.Call(AiEvent.ResearchComplete, It.IsAny<LuaTable>()))
              .Returns(luaResult);

        // Act
        aiPlayer.SelectNewAdvance(possibilities);

        // Assert
        Assert.Equal(0, civilization.ReseachingAdvance);
    }

    [Fact]
    public void SelectNewAdvance_SetsReseachingAdvance_FromIndex()
    {
        // Arrange
        var (game, mockAi, civilization, rules, advances) = CreateMocks();
        var aiPlayer = new AiPlayer(0, civilization, null, game, mockAi.Object);

        var advance = new Advance { Index = 0, Name = "Writing" };
        advances.Add(advance);
        rules.Advances = advances.ToArray();
        var possibilities = new List<Advance> { advance };
        
        var luaResult = new LuaResult(0); // Index in researchPossibilities

        mockAi.Setup(ai => ai.Call(AiEvent.ResearchComplete, It.IsAny<LuaTable>()))
              .Returns(luaResult);

        // Act
        aiPlayer.SelectNewAdvance(possibilities);

        // Assert
        Assert.Equal(0, civilization.ReseachingAdvance);
    }
}
