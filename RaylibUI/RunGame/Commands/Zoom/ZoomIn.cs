using Civ2engine.Enums;
using Civ2engine.Events;
using Model.Controls;
using JetBrains.Annotations;
using Model.Input;

namespace RaylibUI.RunGame.Commands.Zoom;

[UsedImplicitly]
public class ZoomIn(GameScreen gameScreen) : AlwaysOnCommand(gameScreen,CommandIds.ZoomIn, [new Shortcut(Key.Z)])
{
    public override void Action()
    {
        if (GameScreen.Zoom < 8)
            GameScreen.TriggerMapEvent(new MapEventArgs(MapEventType.ZoomChange) { Zoom = GameScreen.Zoom + 1 });
    }
}