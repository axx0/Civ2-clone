using Civ2engine.Enums;
using Civ2engine.Events;
using Civ2engine.MapObjects;
using Model;
using Model.Dialog;
using Model.Menu;
using Raylib_CSharp.Interact;

namespace RaylibUI.RunGame.Commands;

public class ZoomOut : IGameCommand
{
    private readonly GameScreen _gameScreen;
    private Map _map;

    public ZoomOut(GameScreen gameScreen)
    {
        _gameScreen = gameScreen;
        _map = gameScreen.Game.CurrentMap;
    }

    public string Id => CommandIds.ZoomOut;
    public Shortcut[] ActivationKeys { get; set; } = { new (KeyboardKey.X) };
    public CommandStatus Status => CommandStatus.Normal;

    public bool Update()
    {
        return true;
    }

    public void Action()
    {
        if (_map.Zoom > -7)
            _gameScreen.TriggerMapEvent(new MapEventArgs(MapEventType.ZoomChange) { Zoom = _map.Zoom - 1 });
    }

    public MenuCommand? Command { get; set; }
    public string ErrorDialog { get; } = string.Empty;
    public DialogImageElements? ErrorImage { get; } = null;
    public string? Name { get; }
}