using Civ2engine;

namespace Model.InterfaceActions;

public class StartGame : IInterfaceAction
{
    public StartGame(Game game)
    {
        Game = game;
    }

    public string Name => "StartGame";
    public EventType ActionType => EventType.StartGame;
    public Game Game { get; }
}