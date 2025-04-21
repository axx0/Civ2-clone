using Civ2engine.Enums;
using Civ2engine.Events;
using Civ2engine.MapObjects;
using Model;
using Model.Dialog;
using Model.Menu;
using Raylib_CSharp.Interact;

namespace RaylibUI.RunGame.Commands;

public class StandardZoom(GameScreen gameScreen) :  AlwaysOnCommand(gameScreen,CommandIds.StandardZoom, [new Shortcut(KeyboardKey.Z, shift: true)])
{
    public override void Action()
    {
        if (GameScreen.Zoom != 0)
            GameScreen.TriggerMapEvent(new MapEventArgs(MapEventType.ZoomChange) { Zoom = 0 });
    }
}