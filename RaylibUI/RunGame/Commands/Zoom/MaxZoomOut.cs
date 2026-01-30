using Civ2engine.Enums;
using Civ2engine.Events;
using Model.Controls;
using JetBrains.Annotations;
using Model.Input;

namespace RaylibUI.RunGame.Commands.Zoom;

[UsedImplicitly]
public class MaxZoomOut(GameScreen gameScreen) :  AlwaysOnCommand(gameScreen,CommandIds.MaxZoomOut, [new Shortcut(Key.G, ctrl: true)])
{
    public override void Action()
    {
        if (GameScreen.Zoom > -7)
            GameScreen.TriggerMapEvent(new MapEventArgs(MapEventType.ZoomChange) { Zoom = -7 });
    }
}