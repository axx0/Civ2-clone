using Civ2engine.MapObjects;
using Raylib_cs;
using RaylibUI.RunGame.GameControls.Mapping.Views;

namespace RaylibUI.RunGame.GameControls.GameModes;

public interface IGameMode
{
    IGameView GetDefaultView(GameScreen gameScreen, IGameView? currentView, int viewHeight, int viewWidth);
    bool MapClicked(Tile tile, MouseButton mouseButton, bool longClick);
    void HandleKeyPress(KeyboardKey key);
    bool Activate();
}       