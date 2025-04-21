using Civ2engine.Enums;
using Civ2engine.Events;
using Model.Menu;
using Raylib_CSharp.Interact;

namespace RaylibUI.RunGame.Commands;

public class ZoomOut(GameScreen gameScreen)
    : AlwaysOnCommand(gameScreen, CommandIds.ZoomOut, [new Shortcut(KeyboardKey.X)])
{
    public override void Action()
    {
        if (GameScreen.Zoom > -7)
            GameScreen.TriggerMapEvent(new MapEventArgs(MapEventType.ZoomChange) { Zoom = GameScreen.Zoom - 1 });
    }
}