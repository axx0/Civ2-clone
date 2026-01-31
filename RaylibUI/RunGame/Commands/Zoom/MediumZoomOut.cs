using Civ2engine.Enums;
using Civ2engine.Events;
using Model.Controls;
using JetBrains.Annotations;
using Model.Input;

namespace RaylibUI.RunGame.Commands.Zoom;

[UsedImplicitly]
public class MediumZoomOut(GameScreen gameScreen) :  AlwaysOnCommand(gameScreen,CommandIds.MediumZoomOut, [new Shortcut(Key.X, ctrl: true)])
{
    public override void Action()
    {
        if (GameScreen.Zoom != -3)
            GameScreen.TriggerMapEvent(new MapEventArgs(MapEventType.ZoomChange) { Zoom = -3 });
    }
}