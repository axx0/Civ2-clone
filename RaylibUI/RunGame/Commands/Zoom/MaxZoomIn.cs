using Civ2engine.Enums;
using Civ2engine.Events;
using JetBrains.Annotations;
using Model.Input;
using Model.Menu;

namespace RaylibUI.RunGame.Commands.Zoom;

[UsedImplicitly]
public class MaxZoomIn(GameScreen gameScreen) :  AlwaysOnCommand(gameScreen,CommandIds.MaxZoomIn, [new Shortcut(Key.Z, ctrl: true)])
{
    public override void Action()
    {
        if (GameScreen.Zoom < 8)
            GameScreen.TriggerMapEvent(new MapEventArgs(MapEventType.ZoomChange) { Zoom = 8 });
    }
}