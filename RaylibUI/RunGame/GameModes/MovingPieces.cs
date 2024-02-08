using System.Data.Common;
using System.Numerics;
using Civ2engine;
using Civ2engine.MapObjects;
using Civ2engine.Terrains;
using Civ2engine.UnitActions.Move;
using Civ2engine.Units;
using Model;
using Model.Menu;
using Raylib_cs;
using RaylibUI.BasicTypes.Controls;
using RaylibUI.RunGame.GameControls;
using RaylibUI.RunGame.GameControls.Mapping;
using RaylibUI.RunGame.GameControls.Mapping.Views;

namespace RaylibUI.RunGame.GameModes;

public class MovingPieces : IGameMode
{
    private readonly GameScreen _gameScreen;
    private readonly LabelControl _title;
    private readonly InterfaceStyle _look;

    public MovingPieces(GameScreen gameScreen)
    {
        _gameScreen = gameScreen;
        _look = gameScreen.MainWindow.ActiveInterface.Look;

        _title = new LabelControl(gameScreen, Labels.For(LabelIndex.MovingUnits), true, alignment: TextAlignment.Center, font: _look.StatusPanelLabelFont, fontSize: _look.StatusPanelLabelFontSize, colorFront: _look.MovingUnitsViewingPiecesLabelColor, colorShadow: _look.MovingUnitsViewingPiecesLabelColorShadow, shadowOffset: new Vector2(1, 0), spacing: 0);

        Actions = new Dictionary<Shortcut, Action>
        {
            /*{
                Keys.Enter, () =>
                {
                    if (main.StatusPanel.WaitingAtEndOfTurn) main.StatusPanel.End_WaitAtEndOfTurn();
                }
            },*/

            {new Shortcut(KeyboardKey.Kp7), MovementFunctions.TryMoveNorthWest}, {new Shortcut(KeyboardKey.Kp8), MovementFunctions.TryMoveNorth},
            {new Shortcut(KeyboardKey.Kp9), MovementFunctions.TryMoveNorthEast},
            {new Shortcut(KeyboardKey.Kp1), MovementFunctions.TryMoveSouthWest}, {new Shortcut(KeyboardKey.Kp2), MovementFunctions.TryMoveSouth},
            {new Shortcut(KeyboardKey.Kp3), MovementFunctions.TryMoveSouthEast},
            {new Shortcut(KeyboardKey.Kp4), MovementFunctions.TryMoveWest}, {new Shortcut(KeyboardKey.Kp6), MovementFunctions.TryMoveEast},

            {new Shortcut(KeyboardKey.Up), MovementFunctions.TryMoveNorth}, {new Shortcut(KeyboardKey.Down), MovementFunctions.TryMoveSouth},
            {new Shortcut(KeyboardKey.Left), MovementFunctions.TryMoveWest}, {new Shortcut(KeyboardKey.Right), MovementFunctions.TryMoveEast},
        };
    }

    public Dictionary<Shortcut, Action> Actions { get; set; }

    public IGameView GetDefaultView(GameScreen gameScreen, IGameView? currentView, int viewHeight, int viewWidth,
        bool forceRedraw)
    {
        if (!forceRedraw && currentView is UnitReadyView animation)
        {
            if (animation.ViewWidth == viewWidth && animation.ViewHeight == viewHeight && animation.Unit == gameScreen.Player.ActiveUnit)

            {
                animation.Reset();
                return animation;
            }
        }

        _gameScreen.StatusPanel.Update();
        return new UnitReadyView(gameScreen, currentView, viewHeight, viewWidth, gameScreen.Player.ActiveUnit, forceRedraw);
    }

    public bool MapClicked(Tile tile, MouseButton mouseButton, bool longClick)
    {
        if (mouseButton == MouseButton.Left)
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

    public bool HandleKeyPress(Shortcut command)
    {
        if (Actions.ContainsKey(command))
        {
            Actions[command]();
            return true;
        }

        return false;
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

    public void PanelClick()
    {
        _gameScreen.ActiveMode = _gameScreen.ViewPiece;
    }

    public IList<IControl> GetSidePanelContents(Rectangle bounds)
    {
        var controls = new List<IControl> { _title };
        var startY = bounds.Y + 61;
        _title.Bounds = bounds with { Y = startY, Height = _title.GetPreferredHeight() };

        var labelHeight = 18;
        var currentY = startY + labelHeight + 16;

        // Active unit
        var offsetX = 7;
        var activeUnit = _gameScreen.Player.ActiveUnit;
        var unitDisplay = new UnitDisplay(_gameScreen, activeUnit,
            new Vector2(bounds.X + offsetX, currentY), _gameScreen.Main.ActiveInterface, (float)9/8);
        controls.Add(unitDisplay);

        // Show move points correctly
        var commonMultiplier = _gameScreen.Game.Rules.Cosmic.MovementMultiplier;
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

        currentY -= 3;
        controls.Add(new StatusLabel(_gameScreen, moveText)
        {
            Bounds = bounds with { X = unitDisplay.Bounds.X + unitDisplay.Width + 2, Y = currentY, Height = labelHeight }
        });
        currentY += labelHeight;

        // Show other unit info
        var cityName = (activeUnit.HomeCity == null) ? Labels.For(LabelIndex.NONE) : activeUnit.HomeCity.Name;
        controls.Add(new StatusLabel(_gameScreen, cityName)
        {
            Bounds = bounds with { X = unitDisplay.Bounds.X + unitDisplay.Width + 2, Y = currentY, Height = labelHeight }
        });
        currentY += labelHeight;
        controls.Add(new StatusLabel(_gameScreen, _gameScreen.Player.Civilization.Adjective)
        {
            Bounds = bounds with { X = unitDisplay.Bounds.X + unitDisplay.Width + 2, Y = currentY, Height = labelHeight }
        });
        currentY += labelHeight + 4;

        controls.Add(new StatusLabel(_gameScreen,
            activeUnit.Veteran ? $"{activeUnit.Name} ({Labels.For(LabelIndex.Veteran)})" : activeUnit.Name)
        {
            Bounds = bounds with { Y = currentY, Height = labelHeight }
        });
        currentY += labelHeight;

        // Tile name
        var activeTile = _gameScreen.Player.ActiveTile;
        var terrainName = activeTile.Name;
        if (activeTile.River)
        {
            terrainName = string.Join(", ", terrainName, Labels.For(LabelIndex.River));
        }
        controls.Add(new StatusLabel(_gameScreen, $"({terrainName})")
        {
            Bounds = bounds with { Y = currentY, Height = labelHeight }
        });
        currentY += labelHeight;

        // Specials on tile?
        if (_gameScreen.Player.ActiveTile.SpecialsName is not null)
        {
            var specials = _gameScreen.Player.ActiveTile.SpecialsName;
            controls.Add(new StatusLabel(_gameScreen, $"({specials})")
            {
                Bounds = bounds with { Y = currentY, Height = labelHeight }
            });
            currentY += labelHeight;
        }

        // If road/railroad/irrigation/farmland/mine present
        var improvements = activeTile.Improvements.Select(c => new
            { Imp = _gameScreen.Game.TerrainImprovements[c.Improvement], Const = c }).ToList();

        var improvementText = string.Join(", ",
            improvements.Where(i => i.Imp.ExclusiveGroup != ImprovementTypes.DefenceGroup && !i.Imp.Negative)
                .Select(i => i.Imp.Levels[i.Const.Level].Name));

        if (!string.IsNullOrWhiteSpace(improvementText) && activeTile.CityHere == null)
        {
            controls.Add(new StatusLabel(_gameScreen, $"({improvementText})")
            {
                Bounds = bounds with { Y = currentY, Height = labelHeight }
            });
            currentY += labelHeight;
        }

        // If airbase/fortress present
        if (improvements.Any(i => i.Imp.ExclusiveGroup == ImprovementTypes.DefenceGroup))
        {
            var airbaseText = string.Join(", ",
                improvements.Where(i => i.Imp.ExclusiveGroup == ImprovementTypes.DefenceGroup)
                    .Select(i => i.Imp.Levels[i.Const.Level].Name));
            controls.Add(new StatusLabel(_gameScreen, $"({airbaseText})")
            {
                Bounds = bounds with { Y = currentY, Height = labelHeight }
            });
            currentY += labelHeight;
        }

        // If pollution present
        var pollutionText = string.Join(", ",
            improvements.Where(i => i.Imp.Negative)
                .Select(i => i.Imp.Levels[i.Const.Level].Name));
        if (!string.IsNullOrWhiteSpace(pollutionText))
        {
            controls.Add(new StatusLabel(_gameScreen, $"({pollutionText})")
            {
                Bounds = bounds with { Y = currentY, Height = labelHeight }
            });
            currentY += labelHeight;
        }

        // The rest of units on tile
        currentY += 7;
        var unitsLeftOnTile = activeTile.UnitsHere.Where(u => u != activeUnit).ToList();
        var unitsCanBeDisplayed = Math.Floor((bounds.Y + bounds.Height - currentY) / (unitDisplay.Height + 8));
        if (unitsLeftOnTile.Count > unitsCanBeDisplayed)
        {
            unitsCanBeDisplayed = Math.Floor((bounds.Y + bounds.Height - currentY - 26) / (unitDisplay.Height + 8)); // Take bottom text into account
        }
        else
        {
            unitsCanBeDisplayed = unitsLeftOnTile.Count;
        }
        for (int i = 0; i < unitsCanBeDisplayed; i++)
        {
            var unit = unitsLeftOnTile[i];
            var unitImage = new UnitDisplay(_gameScreen, unit,
                new Vector2(bounds.X + offsetX, currentY), _gameScreen.Main.ActiveInterface, (float)9 / 8);
            controls.Add(unitImage);
            currentY += unitImage.Height + 8;
            cityName = (unit.HomeCity == null) ? Labels.For(LabelIndex.NONE) : unit.HomeCity.Name;
            controls.Add(new StatusLabel(_gameScreen, cityName)
            {
                Bounds = new Rectangle(unitImage.Bounds.X + unitImage.Width + 2, unitImage.Location.Y, bounds.Width - unitImage.Width, labelHeight)
            });
            controls.Add(new StatusLabel(_gameScreen, _gameScreen.Game.Order2String(unit.Order))
            {
                Bounds = new Rectangle(unitImage.Bounds.X + unitImage.Width + 2, unitImage.Location.Y + labelHeight, bounds.Width - unitImage.Width, labelHeight)
            });
            controls.Add(new StatusLabel(_gameScreen, unit.Veteran ? $"{unit.Name} ({Labels.For(LabelIndex.Veteran)})" : unit.Name)
            {
                Bounds = new Rectangle(unitImage.Bounds.X + unitImage.Width + 2, unitImage.Location.Y + 2 * labelHeight, bounds.Width - unitImage.Width, labelHeight)
            });
        }

        // If not all units were drawn print a message
        if (unitsLeftOnTile.Count > unitsCanBeDisplayed)
        {
            moveText = unitsLeftOnTile.Count - unitsCanBeDisplayed == 1 ? Labels.For(LabelIndex.Unit) : Labels.For(LabelIndex.Units);
            controls.Add(new StatusLabel(_gameScreen, $"({unitsLeftOnTile.Count - unitsCanBeDisplayed} {Labels.For(LabelIndex.More)} {moveText})")
            {
                Bounds = bounds with { Y = bounds.Y + bounds.Height - 28, Height = labelHeight }
            });
        }

        controls.ForEach(c => c.OnResize());
        return controls;
    }
}