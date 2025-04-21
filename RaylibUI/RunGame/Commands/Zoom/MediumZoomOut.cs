using Civ2engine.Enums;
using Civ2engine.Events;
using Civ2engine.MapObjects;
using Model;
using Model.Dialog;
using Model.Menu;
using Raylib_CSharp.Interact;

namespace RaylibUI.RunGame.Commands;

public class MediumZoomOut(GameScreen gameScreen) :  AlwaysOnCommand(gameScreen,CommandIds.MediumZoomOut, [new Shortcut(KeyboardKey.X, ctrl: true)])
{
    public override void Action()
    {
        if (GameScreen.Zoom != -3)
            GameScreen.TriggerMapEvent(new MapEventArgs(MapEventType.ZoomChange) { Zoom = -3 });
    }
}