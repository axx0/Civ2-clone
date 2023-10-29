using Civ2engine.MapObjects;
using Civ2engine.UnitActions.Move;
using Raylib_cs;
using RaylibUI.RunGame.GameControls.GameModes;
using RaylibUI.RunGame.GameControls.Mapping.Views;

namespace RaylibUI.RunGame;

public class MovingPieces : IGameMode
{
    private readonly GameScreen _gameScreen;

    public MovingPieces(GameScreen gameScreen)
    {
        _gameScreen = gameScreen;
        Actions = new Dictionary<KeyboardKey, Action>
        {
            /*{
                Keys.Enter, () =>
                {
                    if (main.StatusPanel.WaitingAtEndOfTurn) main.StatusPanel.End_WaitAtEndOfTurn();
                }
            },*/

            {KeyboardKey.KEY_KP_7, MovementFunctions.TryMoveNorthWest}, {KeyboardKey.KEY_KP_8, MovementFunctions.TryMoveNorth},
            {KeyboardKey.KEY_KP_9, MovementFunctions.TryMoveNorthEast},
            {KeyboardKey.KEY_KP_1, MovementFunctions.TryMoveSouthWest}, {KeyboardKey.KEY_KP_2, MovementFunctions.TryMoveSouth},
            {KeyboardKey.KEY_KP_3, MovementFunctions.TryMoveSouthEast},
            {KeyboardKey.KEY_KP_4, MovementFunctions.TryMoveWest}, {KeyboardKey.KEY_KP_6, MovementFunctions.TryMoveEast},

            {KeyboardKey.KEY_UP, MovementFunctions.TryMoveNorth}, {KeyboardKey.KEY_DOWN, MovementFunctions.TryMoveSouth},
            {KeyboardKey.KEY_LEFT, MovementFunctions.TryMoveWest}, {KeyboardKey.KEY_RIGHT, MovementFunctions.TryMoveEast},

            {KeyboardKey.KEY_SPACE, () =>
                {
                    _gameScreen.Game.ActiveUnit?.SkipTurn();
                    _gameScreen.Game.ChooseNextUnit();
                }
            },
            {KeyboardKey.KEY_S, () =>
                {
                    _gameScreen.Game.ActiveUnit?.Sleep();
                    _gameScreen.Game.ChooseNextUnit();
                }
            },
        };
    }

    public Dictionary<KeyboardKey,Action> Actions { get; set; }

    public IGameView GetDefaultView(GameScreen gameScreen, IGameView? currentView, int viewHeight, int viewWidth)
    {
        if (currentView is UnitReadyView animation)
        {
            if (animation.ViewWidth == viewWidth && animation.ViewHeight == viewHeight && animation.Unit == gameScreen.Player.ActiveUnit)

            {
                animation.Reset();
                return animation;
            }
        }

        return new UnitReadyView(gameScreen, currentView, viewHeight, viewWidth, gameScreen.Player.ActiveUnit);
    }

    public bool MapClicked(Tile tile, MouseButton mouseButton, bool longClick)
    {
        if (mouseButton == MouseButton.MOUSE_BUTTON_LEFT)
        {
            //TODO: port GOTO support
            // if (longClick && (e.Modifiers & Keys.Control) != Keys.Control)
            // {
            //     var unit = _player.ActiveUnit;
            //     var path = Path.CalculatePathBetween(_game, _player.ActiveTile, clickedXy, unit.Domain, unit.MaxMovePoints,
            //         unit.Owner, unit.Alpine, unit.IgnoreZonesOfControl);
            //     if (path != null)
            //     {
            //         unit.GoToX = clickedXy.X;
            //         unit.GoToY = clickedXy.Y;
            //         unit.Order = OrderType.GoTo;
            //         path.Follow(_game, unit);
            //         if (!unit.AwaitingOrders)
            //         {
            //             _game.ChooseNextUnit();
            //         }
            //         return false;
            //     }
            // }
            var city = tile.CityHere;
            if (city == null)
            {
                return _gameScreen.ActivateUnits(tile);
            }

            _gameScreen.ShowCityWindow(city);
        }
        else
        {
            _gameScreen.Game.ActiveTile = tile;
            _gameScreen.ActiveMode = _gameScreen.ViewPiece;
        }

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
        if (_gameScreen.Player != _gameScreen.Game.ActivePlayer)
        {
            return false;
        }
            
        if (_gameScreen.Player.ActiveUnit is not {MovePoints: > 0})
        {
            _gameScreen.Game.ChooseNextUnit();
        }
        return _gameScreen.Player.ActiveUnit != null;
    }
}