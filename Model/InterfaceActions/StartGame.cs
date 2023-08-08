using Civ2engine;

namespace Model.InterfaceActions;

public class StartGame : IInterfaceAction
{
    public StartGame(Ruleset ruleSet, Game game)
    {
        RuleSet = ruleSet;
        Game = game;
    }

    public string Name => "StartGame";
    public EventType ActionType => EventType.StartGame;
    public Ruleset RuleSet { get; }
    public Game Game { get; }
}