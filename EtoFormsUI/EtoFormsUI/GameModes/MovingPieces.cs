using System;
using System.Collections.Generic;
using System.Linq;
using Civ2engine;
using Civ2engine.Terrains;
using Civ2engine.UnitActions;
using Civ2engine.UnitActions.Move;
using Eto.Drawing;
using Eto.Forms;
using EtoFormsUI.Animations;

namespace EtoFormsUI.GameModes
{
    public class MovingPieces : IGameMode
    {
        private Game _game; 
        public bool Init(IGameMode previous, Game game)
        {
            _game = game;
            if (game.ActiveUnit is not {MovePoints: > 0})
            {
                game.ChooseNextUnit();
            }
            return true;
        }

        public IDictionary<Keys, Action> Actions { get; set; }
        public bool MapClicked(Tile clickedXy, MapPanel mapPanel, Main main, MouseButtons eButtons)
        {
            if (eButtons == MouseButtons.Primary)
            {
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
                main.ViewPiece.ActiveTile = clickedXy;
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

        public Tile ActiveTile => _game.ActiveUnit.CurrentLocation;

        public void DrawStatusPanel(Graphics eGraphics, PanelStyle style, int unitPanelHeight)
        {
            var activeTile = ActiveTile;

            Draw.Text(eGraphics, "Moving Units", style.Font, Colors.White, new Point(119, 10), true, true, Colors.Black,
                1, 0);

            // Show active unit info
            Draw.Unit(eGraphics, _game.ActiveUnit, false, 1, new Point(7, 27));

            // Show move points correctly
            var commonMultiplier = _game.Rules.Cosmic.MovementMultiplier;
            var remainingFullPoints = _game.ActiveUnit.MovePoints / commonMultiplier;
            var fractionalMove = _game.ActiveUnit.MovePoints % commonMultiplier;

            string moveText;
            if (fractionalMove > 0)
            {
                var gcf = Utils.GreatestCommonFactor(fractionalMove, commonMultiplier);
                moveText =
                    $"Moves: {(remainingFullPoints > 0 ? remainingFullPoints : "")} {fractionalMove / gcf}/{commonMultiplier / gcf}";
            }
            else
            {
                moveText = $"Moves: {remainingFullPoints}";
            }

            Draw.Text(eGraphics, moveText, style.Font, style.FrontColor, new Point(79, 25), false, false,
                style.BackColor, 1, 1);

            // Show other unit info
            var _cityName = (_game.ActiveUnit.HomeCity == null) ? "NONE" : _game.ActiveUnit.HomeCity.Name;
            Draw.Text(eGraphics, _cityName, style.Font, style.FrontColor, new Point(79, 43), false, false,
                style.BackColor, 1, 1);
            Draw.Text(eGraphics, _game.GetActiveCiv.Adjective, style.Font, style.FrontColor, new Point(79, 61), false,
                false, style.BackColor, 1, 1);
            var _column = 83;
            Draw.Text(eGraphics, _game.ActiveUnit.Name, style.Font, style.FrontColor, new Point(5, _column), false,
                false, style.BackColor, 1, 1);
            _column += 18;

            if (activeTile != null)
            {
                Draw.Text(eGraphics, $"({activeTile.Type})", style.Font, style.FrontColor, new Point(5, _column),
                    false,
                    false, style.BackColor, 1, 1);

                // If road/railroad/irrigation/farmland/mine present
                string improvementText = null;
                if (activeTile.Railroad) improvementText = "Railroad";
                else if (activeTile.Road) improvementText = "Road";

                if (activeTile.Mining)
                    improvementText = improvementText != null ? $"{improvementText}, Mining" : "Mining";
                else if (activeTile.Farmland)
                    improvementText = improvementText != null ? $"{improvementText}, Farmland" : "Farmland";
                else if (activeTile.Irrigation)
                    improvementText = improvementText != null ? $"{improvementText}, Irrigation" : "Irrigation";

                if (!string.IsNullOrEmpty(improvementText))
                {
                    _column += 18;
                    Draw.Text(eGraphics, $"({improvementText})", style.Font, style.FrontColor,
                        new Point(5, _column), false,
                        false, style.BackColor, 1, 1);
                }

                // If airbase/fortress present
                if (activeTile.Airbase || activeTile.Fortress)
                {
                    _column += 18;
                    string airbaseText = null;
                    if (activeTile.Fortress) airbaseText = "Fortress";
                    if (activeTile.Airbase) airbaseText = "Airbase";
                    Draw.Text(eGraphics, $"({airbaseText})", style.Font, style.FrontColor, new Point(5, _column),
                        false,
                        false, style.BackColor, 1, 1);
                }

                // If pollution present
                if (activeTile.Pollution)
                {
                    _column += 18;
                    Draw.Text(eGraphics, "(Pollution)", style.Font, style.FrontColor, new Point(5, _column), false,
                        false,
                        style.BackColor, 1, 1);
                }

                _column += 5;

                // Show info for other units on the tile
                int drawCount = 0;
                foreach (var unit in activeTile.UnitsHere.Where(u => u != _game.ActiveUnit))
                {
                    // First check if there is vertical space still left for drawing in panel
                    if (_column + 69 > unitPanelHeight) break;

                    // Draw unit
                    Draw.Unit(eGraphics, unit, false, 1, new Point(7, _column + 27));
                    // Show other unit info
                    _column += 20;
                    _cityName = (unit.HomeCity == null) ? "NONE" : unit.HomeCity.Name;
                    Draw.Text(eGraphics, _cityName, style.Font, style.FrontColor, new Point(80, _column), false,
                        false,
                        style.BackColor, 1, 1);
                    _column += 18;
                    Draw.Text(eGraphics,  _game.Order2string(unit.Order), style.Font, style.FrontColor,
                        new Point(80, _column),
                        false, false, style.BackColor, 1, 1);
                    _column += 18;
                    Draw.Text(eGraphics, unit.Name, style.Font, style.FrontColor, new Point(80, _column), false,
                        false,
                        style.BackColor, 1, 1);

                    //System.Diagnostics.Debug.WriteLine($"{unit.Name} drawn");

                    drawCount++;
                }

                // If not all units were drawn print a message
                if (activeTile.UnitsHere.Count - 1 != drawCount) // -1 because you must not count in active unit
                {
                    _column += 22;
                    moveText = activeTile.UnitsHere.Count - 1 - drawCount == 1 ? "Unit" : "Units";
                    Draw.Text(eGraphics, $"({activeTile.UnitsHere.Count - 1 - drawCount} More {moveText})",
                        style.Font,
                        style.FrontColor, new Point(9, _column), false, false, style.BackColor, 1, 1);
                }
            }
        }

        public MovingPieces(Main main)
        {
            Actions = new Dictionary<Keys, Action>
            {
                {
                    Keys.B, CityActions.CreateCityBuild((name) =>
                    {
                        var box = main.popupBoxList["NAMECITY"];
                        if (box.Options is not null)
                        {
                            box.Text = box.Options;
                            box.Options = null; 
                        }
                        var cityNameDialog = new Civ2dialog(main, main.popupBoxList["NAMECITY"],
                            textBoxes: new List<TextBoxDefinition>
                            {
                                new()
                                {
                                    index = 0,
                                    InitialValue = name,
                                    Name = "CityName",
                                    Width = 225
                                }
                            });
                        cityNameDialog.ShowModal(main);
                        return new BuildCityConfirmResult
                        {
                            Build = cityNameDialog.SelectedIndex != int.MinValue,
                            Name = cityNameDialog.TextValues["CityName"]
                        };
                    })
                },

                {Keys.Keypad7, MovementFunctions.TryMoveNorthWest}, {Keys.Keypad8, MovementFunctions.TryMoveNorth},
                {Keys.Keypad9, MovementFunctions.TryMoveNorthEast},
                {Keys.Keypad1, MovementFunctions.TryMoveSouthWest}, {Keys.Keypad2, MovementFunctions.TryMoveSouth},
                {Keys.Keypad3, MovementFunctions.TryMoveSouthEast},
                {Keys.Keypad4, MovementFunctions.TryMoveWest}, {Keys.Keypad6, MovementFunctions.TryMoveEast},

                {Keys.Up, MovementFunctions.TryMoveNorth}, {Keys.Down, MovementFunctions.TryMoveSouth},
                {Keys.Left, MovementFunctions.TryMoveWest}, {Keys.Right, MovementFunctions.TryMoveEast},

                {Keys.Space, () => _game.ActiveUnit.SkipTurn()},
                {Keys.S, () => _game.ActiveUnit.Sleep()}

            };
        }
    }
}