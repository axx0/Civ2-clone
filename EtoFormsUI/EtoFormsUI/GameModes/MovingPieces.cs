using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Civ2engine;
using Civ2engine.Enums;
using Civ2engine.Improvements;
using Civ2engine.MapObjects;
using Civ2engine.Terrains;
using Civ2engine.UnitActions;
using Civ2engine.UnitActions.Move;
using Civ2engine.Units;
using Eto.Drawing;
using Eto.Forms;
using EtoFormsUI.Animations;
using Graphics = Eto.Drawing.Graphics;
using Order = EtoFormsUI.Players.Orders.Order;
using Point = Eto.Drawing.Point;

namespace EtoFormsUI.GameModes
{
    public class MovingPieces : IGameMode
    {
        private readonly Game _game;
        private LocalPlayer _player;

        public bool Activate(IGameMode previous, IPlayer currentPlayer)
        {
            if (currentPlayer is not LocalPlayer player)
            {
                return false;
            }

            _player = player;
            
            if (player.ActiveUnit is not {MovePoints: > 0})
            {
                _game.ChooseNextUnit();
            }
            return player.ActiveUnit != null;
        }

        public IDictionary<Keys, Action> Actions { get; set; }

        private TimeSpan _gotoThreshold = new TimeSpan(1000);

        public bool MapClicked(Tile clickedXy, MapPanel mapPanel, Main main, bool longClick, MouseEventArgs e,
            int[] mouseDownTile)
        {
            if (e.Buttons == MouseButtons.Primary)
            {
                if (longClick && (e.Modifiers & Keys.Control) != Keys.Control)
                {
                    var unit = _player.ActiveUnit;
                    var path = Path.CalculatePathBetween(_game, _player.ActiveTile, clickedXy, unit.Domain, unit.MaxMovePoints,
                        unit.Owner, unit.Alpine, unit.IgnoreZonesOfControl);
                    if (path != null)
                    {
                        unit.GoToX = clickedXy.X;
                        unit.GoToY = clickedXy.Y;
                        unit.Order = OrderType.GoTo;
                        path.Follow(_game, unit);
                        if (!unit.AwaitingOrders)
                        {
                            _game.ChooseNextUnit();
                        }
                        return false;
                    }
                }
                var city = clickedXy.CityHere;
                if (city == null)
                {
                    return mapPanel.ActivateUnits(clickedXy);
                }

                mapPanel.ShowCityWindow(city);
            }
            else
            {
                main.CurrentGameMode = main.ViewPiece;
                Game.Instance.ActiveTile = clickedXy;
            }

            return true;
        }

        public bool PanelClick(Game game, Main main)
        {
            main.CurrentGameMode = main.ViewPiece;
            return true;
        }

        public IAnimation GetDefaultAnimation(Game game, IAnimation currentAnimation)
        {
            if (currentAnimation is not WaitingAnimation animation) return new WaitingAnimation(game, game.ActiveUnit, game.ActiveUnit.CurrentLocation);
            if (animation.Unit != game.ActiveUnit) return new WaitingAnimation(game, game.ActiveUnit, game.ActiveUnit.CurrentLocation);
            animation.Reset();
            
            return animation;
        }

        public void DrawStatusPanel(Graphics eGraphics, PanelStyle style, int unitPanelHeight)
        {
            var activeTile = _player.ActiveTile;

            Draw.Text(eGraphics, Labels.For(LabelIndex.MovingUnits), style.Font, Colors.White, new Point(119, 10), true, true, Colors.Black,
                1, 0);

            // Show active unit info
            var activeUnit = _game.ActiveUnit;
            Draw.Unit(eGraphics, activeUnit, false, 1, new Point(7, 27));

            // Show move points correctly
            var commonMultiplier = _game.Rules.Cosmic.MovementMultiplier;
            var remainingFullPoints = activeUnit.MovePoints / commonMultiplier;
            var fractionalMove = activeUnit.MovePoints % commonMultiplier;

            string moveText;
            if (fractionalMove > 0)
            {
                var gcf = Utils.GreatestCommonFactor(fractionalMove, commonMultiplier);
                moveText =
                    $"{Labels.For(LabelIndex.Moves)}: {(remainingFullPoints > 0 ? remainingFullPoints : "")} {fractionalMove / gcf}/{commonMultiplier / gcf}";
            }
            else
            {
                moveText = $"{Labels.For(LabelIndex.Moves)}: {remainingFullPoints}";
            }

            Draw.Text(eGraphics, moveText, style.Font, style.FrontColor, new Point(79, 25), false, false,
                style.BackColor, 1, 1);

            // Show other unit info
            var cityName = (activeUnit.HomeCity == null) ? Labels.For(LabelIndex.NONE) : activeUnit.HomeCity.Name;
            Draw.Text(eGraphics, cityName, style.Font, style.FrontColor, new Point(79, 43), false, false,
                style.BackColor, 1, 1);
            Draw.Text(eGraphics, _game.GetActiveCiv.Adjective, style.Font, style.FrontColor, new Point(79, 61), false,
                false, style.BackColor, 1, 1);
            var column = 83;
            Draw.Text(eGraphics, activeUnit.Veteran ? $"{activeUnit.Name} ({Labels.For(LabelIndex.Veteran)})" :activeUnit.Name, style.Font, style.FrontColor, new Point(5, column), false,
                false, style.BackColor, 1, 1);
            column += 18;

            if (activeTile != null)
            {
                Draw.Text(eGraphics, $"({activeTile.Name})", style.Font, style.FrontColor, new Point(5, column),
                    false,
                    false, style.BackColor, 1, 1);

                // If road/railroad/irrigation/farmland/mine present
                var improvements = activeTile.Improvements.Select(c => new
                    { Imp = _game.TerrainImprovements[c.Improvement], Const = c }).ToList();

                var improvementText = string.Join(", ",
                    improvements.Where(i => i.Imp.ExclusiveGroup != ImprovementTypes.DefenceGroup && !i.Imp.Negative)
                        .Select(i => i.Imp.Levels[i.Const.Level].Name));
                
                if (!string.IsNullOrWhiteSpace(improvementText))
                {
                    column += 18;
                    Draw.Text(eGraphics, $"({improvementText})", style.Font, style.FrontColor,
                        new Point(5, column), false,
                        false, style.BackColor, 1, 1);
                }

                // If airbase/fortress present
                if (improvements.Any(i=>i.Imp.ExclusiveGroup == ImprovementTypes.DefenceGroup))
                {
                    column += 18;
                    var airbaseText = string.Join(", ",
                        improvements.Where(i => i.Imp.ExclusiveGroup == ImprovementTypes.DefenceGroup)
                            .Select(i => i.Imp.Levels[i.Const.Level].Name));
                    Draw.Text(eGraphics, $"({airbaseText})", style.Font, style.FrontColor, new Point(5, column),
                        false,
                        false, style.BackColor, 1, 1);
                }

                // If pollution present
                var pollutionText = string.Join(", ",
                    improvements.Where(i => i.Imp.Negative)
                        .Select(i => i.Imp.Levels[i.Const.Level].Name));
                if (!string.IsNullOrWhiteSpace(pollutionText))
                {
                    column += 18;
                    Draw.Text(eGraphics, $"({pollutionText})", style.Font, style.FrontColor, new Point(5, column), false,
                        false,
                        style.BackColor, 1, 1);
                }

                column += 5;

                // Show info for other units on the tile
                int drawCount = 0;
                foreach (var unit in activeTile.UnitsHere.Where(u => u != activeUnit))
                {
                    // First check if there is vertical space still left for drawing in panel
                    if (column + 69 > unitPanelHeight) break;

                    // Draw unit
                    Draw.Unit(eGraphics, unit, false, 1, new Point(7, column + 27));
                    // Show other unit info
                    column += 20;
                    cityName = (unit.HomeCity == null) ? Labels.For(LabelIndex.NONE) : unit.HomeCity.Name;
                    Draw.Text(eGraphics, cityName, style.Font, style.FrontColor, new Point(80, column), false,
                        false,
                        style.BackColor, 1, 1);
                    column += 18;
                    Draw.Text(eGraphics,  _game.Order2string(unit.Order), style.Font, style.FrontColor,
                        new Point(80, column),
                        false, false, style.BackColor, 1, 1);
                    column += 18;
                    Draw.Text(eGraphics, unit.Veteran ? $"{unit.Name} ({Labels.For(LabelIndex.Veteran)})" : unit.Name, style.Font, style.FrontColor, new Point(80, column), false,
                        false,
                        style.BackColor, 1, 1);

                    //System.Diagnostics.Debug.WriteLine($"{unit.Name} drawn");

                    drawCount++;
                }

                // If not all units were drawn print a message
                if (activeTile.UnitsHere.Count - 1 != drawCount) // -1 because you must not count in active unit
                {
                    column += 22;
                    moveText = activeTile.UnitsHere.Count - 1 - drawCount == 1 ? "Unit" : "Units";
                    Draw.Text(eGraphics, $"({activeTile.UnitsHere.Count - 1 - drawCount} More {moveText})",
                        style.Font,
                        style.FrontColor, new Point(9, column), false, false, style.BackColor, 1, 1);
                }
            }
        }

        public void HandleKeyPress(Main main, KeyEventArgs e)
        {
            var key = e.Key | e.Modifiers;
            var order = main.Orders.OrderByDescending(o=>o.Status).FirstOrDefault(o=> o.ActivationCommand == key);
            if (order != null)
            {
                order.ExecuteCommand();
            }
            else
            {
                if (!Actions.ContainsKey(e.Key)) return;
                Actions[e.Key]();
            }
        }



        public MovingPieces(Main main, Game game)
        {
            _game = game;
            Actions = new Dictionary<Keys, Action>
            {
                {
                    Keys.Enter, () =>
                    {
                        if (main.StatusPanel.WaitingAtEndOfTurn) main.StatusPanel.End_WaitAtEndOfTurn();
                    }
                },

                {Keys.Keypad7, MovementFunctions.TryMoveNorthWest}, {Keys.Keypad8, MovementFunctions.TryMoveNorth},
                {Keys.Keypad9, MovementFunctions.TryMoveNorthEast},
                {Keys.Keypad1, MovementFunctions.TryMoveSouthWest}, {Keys.Keypad2, MovementFunctions.TryMoveSouth},
                {Keys.Keypad3, MovementFunctions.TryMoveSouthEast},
                {Keys.Keypad4, MovementFunctions.TryMoveWest}, {Keys.Keypad6, MovementFunctions.TryMoveEast},

                {Keys.Up, MovementFunctions.TryMoveNorth}, {Keys.Down, MovementFunctions.TryMoveSouth},
                {Keys.Left, MovementFunctions.TryMoveWest}, {Keys.Right, MovementFunctions.TryMoveEast},

                {Keys.Space, () =>
                    {
                        _game.ActiveUnit.SkipTurn();
                        _game.ChooseNextUnit();
                    }
                },
                {Keys.S, () =>
                    {
                        _game.ActiveUnit.Sleep();
                        _game.ChooseNextUnit();
                    }
                },
            };
        }
    }
}