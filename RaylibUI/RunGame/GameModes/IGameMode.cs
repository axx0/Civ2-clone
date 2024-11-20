using Civ2engine.MapObjects;
using Model.Menu;
using Raylib_CSharp.Interact;
using Raylib_CSharp.Transformations;
using RaylibUI.RunGame.GameControls.Mapping.Views;

namespace RaylibUI.RunGame.GameModes;

public interface IGameMode
{
    IGameView GetDefaultView(GameScreen gameScreen, IGameView? currentView, int viewHeight, int viewWidth,
        bool forceRedraw);
    bool MapClicked(Tile tile, MouseButton mouseButton);
    bool HandleKeyPress(Shortcut key);
    bool Activate();
    void PanelClick();
    IList<IControl> GetSidePanelContents(Rectangle bounds);
    void MouseDown(Tile tile);
    void MouseClear();
}       