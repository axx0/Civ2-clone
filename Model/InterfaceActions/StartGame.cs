using Civ2engine;
using Model.Core;

namespace Model.InterfaceActions;

public class StartGame(IGame game, IDictionary<string, string?>? viewData) : IInterfaceAction
{
    public string Name => "StartGame";
    public EventType ActionType => EventType.StartGame;
    public IGame Game { get; } = game;
    public IDictionary<string,string?>? ViewData { get; } = viewData;
}