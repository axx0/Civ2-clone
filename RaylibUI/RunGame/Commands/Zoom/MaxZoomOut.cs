using Civ2engine.Enums;
using Civ2engine.Events;
using Civ2engine.MapObjects;
using Model;
using Model.Dialog;
using Model.Menu;
using Raylib_CSharp.Interact;

namespace RaylibUI.RunGame.Commands;

public class MaxZoomOut(GameScreen gameScreen) :  AlwaysOnCommand(gameScreen,CommandIds.MaxZoomOut, [new Shortcut(KeyboardKey.G, ctrl: true)])
{
    public override void Action()
    {
        if (GameScreen.Zoom > -7)
            GameScreen.TriggerMapEvent(new MapEventArgs(MapEventType.ZoomChange) { Zoom = -7 });
    }
}