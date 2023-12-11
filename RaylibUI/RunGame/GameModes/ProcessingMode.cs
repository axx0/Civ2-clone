using System.Diagnostics;
using Civ2engine.MapObjects;
using Raylib_cs;
using RaylibUI.RunGame.GameControls.Mapping.Views;

namespace RaylibUI.RunGame.GameModes;

public class ProcessingMode : IGameMode
{
    public IGameView GetDefaultView(GameScreen gameScreen, IGameView? currentView, int viewHeight, int viewWidth,
        bool forceRedraw)
    {
        if (currentView is not StaticView existing)
            return new StaticView(gameScreen, currentView, viewHeight, viewWidth, forceRedraw);
        
        existing.Reset();
        return existing;
    }

    public bool MapClicked(Tile tile, MouseButton mouseButton, bool longClick)
    {
        return false;
    }

    public void HandleKeyPress(KeyboardKey key)
    {
        // Cannot press keys while processing
    }

    public bool Activate()
    {
        Debug.WriteLine("Processing other turns");
        return true;
        
    }
}