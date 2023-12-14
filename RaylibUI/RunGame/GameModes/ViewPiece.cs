using System.Numerics;
using Civ2engine;
using Civ2engine.MapObjects;
using Raylib_cs;
using RaylibUI.BasicTypes.Controls;
using RaylibUI.RunGame.GameControls.Mapping.Views;

namespace RaylibUI.RunGame.GameModes;

public class ViewPiece : IGameMode
{
    private readonly GameScreen _gameScreen;
    private readonly LabelControl _title;

    public ViewPiece(GameScreen gameScreen)
    {
        _title = new LabelControl(gameScreen, Labels.For(LabelIndex.ViewingPieces), eventTransparent: true,
            alignment: TextAlignment.Center);
        
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

    public IGameView GetDefaultView(GameScreen gameScreen, IGameView? currentView, int viewHeight, int viewWidth,
        bool forceRedraw)
    {
        if (!forceRedraw && currentView is WaitingView animation)
        {
            if (animation.ViewWidth == viewWidth && animation.ViewHeight == viewHeight &&
                animation.Location == gameScreen.Game.ActiveTile)

            {
                animation.Reset();
                return animation;
            }
        }
        _gameScreen.StatusPanel.Update();
        return new WaitingView(gameScreen, currentView, viewHeight, viewWidth, forceRedraw);
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

    public void PanelClick()
    {
        if (_gameScreen.Player.ActiveUnit is {Dead: false})
        {
            _gameScreen.ActiveMode = _gameScreen.Moving;
        }
        else
        {
            _gameScreen.Game.ChooseNextUnit();
        }
    }

    public IList<IControl> GetSidePanelContents(Rectangle bounds)
    {
        var res = new List<IControl> { _title };
        var labelHeight = _title.GetPreferredHeight();
        _title.Bounds = bounds with { height = labelHeight };

        var currentY = bounds.y + 20;

        // Draw location & tile type on active square
        var activeTile = _gameScreen.Player.ActiveTile;
        res.Add(new LabelControl(_gameScreen, $"Loc: ({activeTile.X}, {activeTile.Y}) {activeTile.Island}", true)
        {
            Bounds = bounds with { height = labelHeight, y = currentY }
        });
        currentY += 20;

        res.Add(new LabelControl(_gameScreen, $"({activeTile.Type})", true)
        {
            Bounds = bounds with { height = labelHeight, y = currentY }
        });

        currentY += 20;

        

        //int count;
        //for (count = 0; count < Math.Min(_unitsOnThisTile.Count, maxUnitsToDraw); count++)
        //{
        //    //e.Graphics.DrawImage(ModifyImage.Resize(Draw.Unit(UnitsOnThisTile[count], false, 0), (int)Math.Round(64 * 1.15), (int)Math.Round(48 * 1.15)), 6, 70 + count * 56);
        //    //e.Graphics.DrawImage(ModifyImage.Resize(Draw.Unit(UnitsOnThisTile[count], false, 0), 0), 6, 70 + count * 56);  // TODO: do this again!!!
        //    Draw.Text(e.Graphics, _unitsOnThisTile[count].HomeCity.Name, _font, StringAlignment.Near, StringAlignment.Near, _frontColor, new Point(79, 70 + count * 56), _backColor, 1, 1);
        //    Draw.Text(e.Graphics, _unitsOnThisTile[count].Order.ToString(), _font, StringAlignment.Near, StringAlignment.Near, _frontColor, new Point(79, 88 + count * 56), _backColor, 1, 1); // TODO: give proper conversion of orders to string
        //    Draw.Text(e.Graphics, _unitsOnThisTile[count].Name, _font, StringAlignment.Near, StringAlignment.Near, _frontColor, new Point(79, 106 + count * 56), _backColor, 1, 1);
        //}
        //if (count < _unitsOnThisTile.Count)
        //{
        //    string _moreUnits = (_unitsOnThisTile.Count - count == 1) ? "More Unit" : "More Units";
        //    Draw.Text(e.Graphics, $"({_unitsOnThisTile.Count() - count} {_moreUnits})", _font, StringAlignment.Near, StringAlignment.Near, _frontColor, new Point(5, UnitPanel.Height - 27), _backColor, 1, 1);
        //}
        return res;
    }
}
