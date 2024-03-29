using System;
using System.Linq;
using Civ2engine;
using Civ2engine.MapObjects;
using Civ2engine.Terrains;
using Civ2engine.Units;
using Eto.Drawing;
using Eto.Forms;
using EtoFormsUI.Animations;

namespace EtoFormsUI.GameModes;

/// <summary>
/// This class allows a visyalization of the goto pathing, which would be an interesting feature and helps with debugging
/// </summary>
public class Goto : IGameMode
{
    private readonly Game _game;
    private Unit _unit;

    public Goto(Game game)
    {
        _game = game;
    }
    public bool Activate(IGameMode previous, IPlayer currentPlayer)
    {
        if (currentPlayer.ActiveUnit != null)
        {
            _unit = currentPlayer.ActiveUnit;
        }

        return _unit != null;
    }

    public bool MapClicked(Tile clickedXy, MapPanel mapPanel, Main main, bool interval, MouseEventArgs mouseEventArgs,
        int[] mouseDownTile)
    {
        var path = Path.CalculatePathBetween(_game, _unit.CurrentLocation, clickedXy, _unit.Domain, _unit.MaxMovePoints,
            _unit.Owner, _unit.Alpine, _unit.IgnoreZonesOfControl);
        mapPanel.PathTiles = path?.Tiles;
        mapPanel.PathDebug = path?.RouteDebugData;
        return true;
    }

    public bool PanelClick(Game game, Main main)
    {
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
            var activeTile = _unit.CurrentLocation;

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

    public void HandleKeyPress(Main main, KeyEventArgs keyEventArgs)
    {
        if (keyEventArgs.Key == Keys.Escape)
        {
            main.CurrentGameMode = main.Moving;
        }
    }
}