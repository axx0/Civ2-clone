using Civ2engine.Enums;
using Civ2engine.Events;
using JetBrains.Annotations;
using Model.Input;
using Model.Menu;

namespace RaylibUI.RunGame.Commands.Zoom;

[UsedImplicitly]
public class StandardZoom(GameScreen gameScreen) :  AlwaysOnCommand(gameScreen,CommandIds.StandardZoom, [new Shortcut(Key.Z, shift: true)])
{
    public override void Action()
    {
        if (GameScreen.Zoom != 0)
            GameScreen.TriggerMapEvent(new MapEventArgs(MapEventType.ZoomChange) { Zoom = 0 });
    }
}