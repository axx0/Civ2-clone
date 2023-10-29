using Civ2engine;
using Civ2engine.Enums;
using Civ2engine.MapObjects;
using Raylib_cs;
using RaylibUI.RunGame.GameControls.GameModes;
using RaylibUI.RunGame.GameControls.Mapping.Views;

namespace RaylibUI.RunGame;

public class ViewPiece : IGameMode
{
    private readonly GameScreen _gameScreen;

    public ViewPiece(GameScreen gameScreen)
    {
        _gameScreen = gameScreen;
       Actions = new Dictionary<KeyboardKey, Action>
            {
                {
                    KeyboardKey.KEY_ENTER, () =>
                    {
                        if (_gameScreen.Game.ActiveTile.CityHere != null)
                        {
                            _gameScreen.ShowCityWindow(_gameScreen.Game.ActiveTile.CityHere);
                        }
                        else if (_gameScreen.Game.ActiveTile.UnitsHere.Any(u => u.MovePoints > 0))
                        {
                            _gameScreen.ActivateUnits(_gameScreen.Game.ActiveTile);
                        }
                        /*else if (_gameScreen.StatusPanel.WaitingAtEndOfTurn)
                        {
                            main.StatusPanel.End_WaitAtEndOfTurn();
                        }*/
                    }
                },

                { KeyboardKey.KEY_KP_7, () => SetActive(-1, -1) }, { KeyboardKey.KEY_KP_8, () => SetActive(0, -2) },
                { KeyboardKey.KEY_KP_9, () => SetActive(1, -1) },
                { KeyboardKey.KEY_KP_1, () => SetActive(1, 1) }, { KeyboardKey.KEY_KP_2, () => SetActive(0, 2) },
                { KeyboardKey.KEY_KP_3, () => SetActive(-1, 1) },
                { KeyboardKey.KEY_KP_4, () => SetActive(-2, 0) }, { KeyboardKey.KEY_KP_6, () => SetActive(2, 0) },

                { KeyboardKey.KEY_UP, () => SetActive(0, -2) }, { KeyboardKey.KEY_DOWN, () => SetActive(0, 2) },
                { KeyboardKey.KEY_LEFT, () => SetActive(-2, 0) }, { KeyboardKey.KEY_RIGHT, () => SetActive(2, 0) },
            };
        }

    public Dictionary<KeyboardKey,Action> Actions { get; set; }

    private void SetActive(int deltaX, int deltaY)
    {
        var activeTile = _gameScreen.Game.ActiveTile;
        var newX = activeTile.X + deltaX;
        var newY = activeTile.Y + deltaY;
        if (activeTile.Map.IsValidTileC2(newX, newY))
        {
            _gameScreen.Game.ActiveTile = activeTile.Map.TileC2(newX, newY);
        }
        else if (!activeTile.Map.Flat && newY >= -1 && newY < activeTile.Map.YDim)
        {
            if (newX < 0)
            {
                newX += activeTile.Map.XDimMax;
            }
            else
            {
                newX -= activeTile.Map.XDimMax;
            }

            if (activeTile.Map.IsValidTileC2(newX, newY))
            {
                _gameScreen.Game.ActiveTile = activeTile.Map.TileC2(newX, newY);
            }
        }
    }

    public IGameView GetDefaultView(GameScreen gameScreen, IGameView? currentView, int viewHeight, int viewWidth)
    {
        if (currentView is WaitingView animation)
        {
            if (animation.ViewWidth == viewWidth && animation.ViewHeight == viewHeight &&
                animation.Location == gameScreen.Game.ActiveTile)

            {
                animation.Reset();
                return animation;
            }
        }

        return new WaitingView(gameScreen, currentView, viewHeight, viewWidth);
    }

    public bool MapClicked(Tile tile, MouseButton mouseButton, bool longClick)
    {
        if (mouseButton == MouseButton.MOUSE_BUTTON_LEFT)
        {
            if (tile.CityHere != null)
            {
                _gameScreen.ShowCityWindow(tile.CityHere);
            }
            else
            {
                return _gameScreen.ActivateUnits(tile);
            }
        }

        _gameScreen.Game.ActiveTile = tile;
        return true;
    }

    public void HandleKeyPress(KeyboardKey key)
    {
        if (Actions.ContainsKey(key))
        {
            Actions[key]();
        }
    }

    public bool Activate()
    {
        return true;
    }
}
