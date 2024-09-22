using Civ2engine;
using Model.Core;

namespace Model.InterfaceActions;

public class StartGame : IInterfaceAction
{
    public StartGame(IGame game)
    {
        Game = game;
    }

    public string Name => "StartGame";
    public EventType ActionType => EventType.StartGame;
    public IGame Game { get; }
}