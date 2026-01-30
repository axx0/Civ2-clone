using Civ2engine.Enums;
using Civ2engine.Events;
using Civ2engine.MapObjects;
using Model;
using Model.Controls;
using Raylib_CSharp.Interact;

namespace RaylibUI.RunGame.Commands;

public class MaxZoomIn(GameScreen gameScreen) :  AlwaysOnCommand(gameScreen,CommandIds.MaxZoomIn, [new Shortcut(KeyboardKey.Z, ctrl: true)])
{
    public override void Action()
    {
        if (GameScreen.Zoom < 8)
            GameScreen.TriggerMapEvent(new MapEventArgs(MapEventType.ZoomChange) { Zoom = 8 });
    }
}