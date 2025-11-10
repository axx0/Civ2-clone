using Civ2engine.Enums;
using Civ2engine.Events;
using Model.Menu;
using Raylib_CSharp.Interact;

namespace RaylibUI.RunGame.Commands;

public class ZoomIn(GameScreen gameScreen) : AlwaysOnCommand(gameScreen,CommandIds.ZoomIn, [new Shortcut(KeyboardKey.Z)])
{
    public override void Action()
    {
        if (GameScreen.Zoom < 8)
            GameScreen.TriggerMapEvent(new MapEventArgs(MapEventType.ZoomChange) { Zoom = GameScreen.Zoom + 1 });
    }
}