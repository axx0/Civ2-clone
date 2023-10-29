using System;
using System.Collections.Generic;
using System.Linq;
using Civ2engine;
using Civ2engine.Enums;
using Civ2engine.MapObjects;
using Civ2engine.Terrains;
using Eto.Drawing;
using Eto.Forms;
using EtoFormsUI.Animations;

namespace EtoFormsUI.GameModes
{
    public class ViewPiece : IGameMode
    {
        private readonly Game _game;
        private readonly Main _main;

        public ViewPiece(Game game, Main main)
        {
            _game = game;
            _main = main;
            Actions = new Dictionary<Keys, Action>
            {
                {
                    Keys.Enter, () =>
                    {
                        if (_game.ActiveTile.CityHere != null)
                        {
                            main.mapPanel.ShowCityWindow(_game.ActiveTile.CityHere);
                        }
                        else if (_game.ActiveTile.UnitsHere.Any(U => U.MovePoints > 0))
                        {
                            main.mapPanel.ActivateUnits(_game.ActiveTile);
                        }
                        else if (main.StatusPanel.WaitingAtEndOfTurn)
                        {
                            main.StatusPanel.End_WaitAtEndOfTurn();
                        }
                    }
                },

                { Keys.Keypad7, () => SetActive(-1, -1) }, { Keys.Keypad8, () => SetActive(0, -2) },
                { Keys.Keypad9, () => SetActive(1, -1) },
                { Keys.Keypad3, () => SetActive(1, 1) }, { Keys.Keypad2, () => SetActive(0, 2) },
                { Keys.Keypad1, () => SetActive(-1, 1) },
                { Keys.Keypad4, () => SetActive(-2, 0) }, { Keys.Keypad6, () => SetActive(2, 0) },

                { Keys.Up, () => SetActive(0, -2) }, { Keys.Down, () => SetActive(0, 2) },
                { Keys.Left, () => SetActive(-2, 0) }, { Keys.Right, () => SetActive(2, 0) },
            };
        }

        private void SetActive(int deltaX, int deltaY)
        {
            var activeTile = _game.ActiveTile;
            var newX = activeTile.X + deltaX;
            var newY = activeTile.Y + deltaY;
            if (_game.CurrentMap.IsValidTileC2(newX, newY))
            {
                _game.ActiveTile = _game.CurrentMap.TileC2(newX, newY);
            }
            else if (!_game.Options.FlatEarth && newY >= -1 && newY < _game.CurrentMap.YDim)
            {
                if (newX < 0)
                {
                    newX += _game.CurrentMap.XDimMax;
                }
                else
                {
                    newX -= _game.CurrentMap.XDimMax;
                }

                if (_game.CurrentMap.IsValidTileC2(newX, newY))
                {
                    _game.ActiveTile = _game.CurrentMap.TileC2(newX, newY);
                }
            }
        }

        public bool Activate(IGameMode previous, IPlayer currentPlayer)
        {
            return true;
        }

        public Dictionary<Keys, Action> Actions { get; } 
        public bool MapClicked(Tile clickedXy, MapPanel mapPanel, Main main, bool interval, MouseEventArgs e,
            int[] mouseDownTile)
        {
            if (e.Buttons == MouseButtons.Primary)
            {
                if (clickedXy.CityHere != null)
                {
                    mapPanel.ShowCityWindow(clickedXy.CityHere);
                }
                else
                {
                    return mapPanel.ActivateUnits(clickedXy);
                }

            }
            _game.ActiveTile = clickedXy;
            return true;
        }

        public bool PanelClick(Game game, Main main)
        {
            if (game.ActiveUnit is {Dead: false})
            {
                main.CurrentGameMode = main.Moving;
            }
            else
            {
                game.ChooseNextUnit();
            }
            return true;
        }

        public IAnimation GetDefaultAnimation(Game game, IAnimation currentAnimation)
        {
            if (currentAnimation is not WaitingAnimation animation) return new WaitingAnimation(game, null, _game.ActiveTile);
            if (animation.Unit != null || animation.Location != _game.ActiveTile) return new WaitingAnimation(game, null, _game.ActiveTile);
            animation.Reset();
            return animation;
        }

        public void DrawStatusPanel(Graphics eGraphics, PanelStyle style, int unitPanelHeight)
        {
            Draw.Text(eGraphics, "Viewing Pieces", style.Font, Colors.White, new Point(119, 10), true, true,
                Colors.Black, 1, 0);

            // Draw location & tile type on active square
            if (_game.ActiveTile != null)
            {
                Draw.Text(eGraphics,
                    $"Loc: ({_game.ActiveTile.X}, {_game.ActiveTile.Y}) {_game.ActiveTile.Island}",
                    style.Font, style.FrontColor, new Point(5, 27), false, false, style.BackColor, 1, 1);
                Draw.Text(eGraphics,
                    $"({_game.ActiveTile.Type})", style.Font,
                    style.FrontColor, new Point(5, 45), false, false, style.BackColor, 1, 1);
            }
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
        }

        public void HandleKeyPress(Main eventArgs, KeyEventArgs e)
        {
            if (Actions.ContainsKey(e.Key))
            {
                Actions[e.Key]();
            }
        }

        public void HandleCommand(Command command)
        {
            
        }
    }
}