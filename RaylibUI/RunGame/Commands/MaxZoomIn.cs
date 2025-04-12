using Civ2engine.Enums;
using Civ2engine.Events;
using Civ2engine.MapObjects;
using Model;
using Model.Dialog;
using Model.Menu;
using Raylib_CSharp.Interact;

namespace RaylibUI.RunGame.Commands;

public class MaxZoomIn : IGameCommand
{
    private readonly GameScreen _gameScreen;

    public MaxZoomIn(GameScreen gameScreen)
    {
        _gameScreen = gameScreen;
    }

    public string Id => CommandIds.MaxZoomIn;
    public Shortcut[] ActivationKeys { get; set; } = { new (KeyboardKey.Z, ctrl: true) };
    public CommandStatus Status => CommandStatus.Normal;

    public bool Update()
    {
        return true;
    }

    public void Action()
    {
        if (_gameScreen.Zoom < 8)
            _gameScreen.TriggerMapEvent(new MapEventArgs(MapEventType.ZoomChange) { Zoom = 8 });
    }

    public bool Checked => false;
    public MenuCommand? Command { get; set; }
    public string ErrorDialog { get; } = string.Empty;
    public DialogImageElements? ErrorImage { get; } = null;
    public string? Name { get; }
}