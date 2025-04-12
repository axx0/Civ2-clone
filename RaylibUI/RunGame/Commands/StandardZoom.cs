using Civ2engine.Enums;
using Civ2engine.Events;
using Civ2engine.MapObjects;
using Model;
using Model.Dialog;
using Model.Menu;
using Raylib_CSharp.Interact;

namespace RaylibUI.RunGame.Commands;

public class StandardZoom : IGameCommand
{
    private readonly GameScreen _gameScreen;

    public StandardZoom(GameScreen gameScreen)
    {
        _gameScreen = gameScreen;
    }

    public string Id => CommandIds.StandardZoom;
    public Shortcut[] ActivationKeys { get; set; } = { new (KeyboardKey.Z, shift: true) };
    public CommandStatus Status => CommandStatus.Normal;

    public bool Update()
    {
        return true;
    }

    public void Action()
    {
        if (_gameScreen.Zoom != 0)
            _gameScreen.TriggerMapEvent(new MapEventArgs(MapEventType.ZoomChange) { Zoom = 0 });
    }

    public bool Checked => false;
    public MenuCommand? Command { get; set; }
    public string ErrorDialog { get; } = string.Empty;
    public DialogImageElements? ErrorImage { get; } = null;
    public string? Name { get; }
}