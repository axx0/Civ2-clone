using System.Data.Common;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using System.Xml.Linq;
using Civ2engine;
using Civ2engine.Enums;
using Civ2engine.MapObjects;
using Civ2engine.Terrains;
using Civ2engine.UnitActions;
using Civ2engine.Units;
using Model;
using Model.Core;
using Model.Controls;
using Raylib_CSharp.Interact;
using Raylib_CSharp.Transformations;
using RaylibUI.BasicTypes.Controls;
using RaylibUI.RunGame.GameControls;
using RaylibUI.RunGame.GameControls.Mapping.Views;
using Path = Civ2engine.Units.Path;

namespace RaylibUI.RunGame.GameModes;

public class MovingPieces : IGameMode
{
    private readonly GameScreen _gameScreen;
    private readonly LabelControl _title;
    private readonly InterfaceStyle _look;
    private DateTime? _downTime;
    private readonly TimeSpan _holdTime = TimeSpan.FromMilliseconds(15);

    public MovingPieces(GameScreen gameScreen)
    {
        _gameScreen = gameScreen;
        _look = gameScreen.MainWindow.ActiveInterface.Look;

        _title = new LabelControl(gameScreen, Labels.For(LabelIndex.MovingUnits), true, 
            horizontalAlignment: HorizontalAlignment.Center, font: _look.StatusPanelLabelFont, fontSize: 18, spacing: 0, 
            colorFront: _look.MovingUnitsViewingPiecesLabelColor, colorShadow: _look.MovingUnitsViewingPiecesLabelColorShadow, 
            shadowOffset: new Vector2(1, 0));

        Actions = new Dictionary<Shortcut, Action<IGame>>
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

    public Dictionary<Shortcut, Action<IGame>> Actions { get; set; }

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

    public bool MapClicked(Tile tile, MouseButton mouseButton)
    {
        if (mouseButton == MouseButton.Left)
        {
            // GOTO support
            if (_downTime.HasValue && DateTime.Now - _downTime.Value > _holdTime && !(Input.IsKeyDown(KeyboardKey.LeftControl) || Input.IsKeyDown(KeyboardKey.RightControl)))
            {
                var unit = _gameScreen.Player.ActiveUnit;
                var path = Path.CalculatePathBetween(_gameScreen.Game, _gameScreen.Player.ActiveTile, tile, unit.Domain, unit.MaxMovePoints,
                    unit.Owner, unit.Alpine, unit.IgnoreZonesOfControl);
                if (path != null)
                {
                    unit.GoToX = tile.X;
                    unit.GoToY = tile.Y;
                    unit.Order = (int)OrderType.GoTo;
                    path.Follow(_gameScreen.Game, unit);
                    if (!unit.AwaitingOrders)
                    {
                        _gameScreen.Game.ChooseNextUnit();
                    }
                    return false;
                }
            }
            var city = tile.CityHere;
            if (city == null)
            {
                return _gameScreen.ActivateUnits(tile);
            }

            _gameScreen.ShowCityWindow(city);
        }
        else
        {
            _gameScreen.Game.ActivePlayer.ActiveTile = tile;
            _gameScreen.ActiveMode = _gameScreen.ViewPiece;
        }

        return true;
    }

    public bool HandleKeyPress(Shortcut command)
    {
        if (Actions.ContainsKey(command))
        {
            Actions[command](_gameScreen.Game);
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

        var fontSize = _gameScreen.ToTPanelLayout ? 14 : 18;
        _title.FontSize = fontSize;
        var unitZoom = _gameScreen.ToTPanelLayout ? -1 : 1;
        var labelHeight = _title.TextSize.Y;

        _title.Location = new(bounds.X, bounds.Y);
        _title.Width = (int)bounds.Width;
        _title.Height = (int)labelHeight;

        var currentX = bounds.X;
        var currentY = bounds.Y + _title.Height;

        // Active unit
        var activeUnit = _gameScreen.Player.ActiveUnit;
        var unitDisplay = new UnitDisplay(_gameScreen, activeUnit, _gameScreen.Game,
            new Vector2(currentX, currentY), _gameScreen.Main.ActiveInterface, ImageUtils.ZoomScale(unitZoom));
        controls.Add(unitDisplay);

        // Show move points correctly
        var commonMultiplier = _gameScreen.Game.Rules.Cosmic.MovementMultiplier;
        var remainingFullPoints = activeUnit.MovePoints / commonMultiplier;
        var fractionalMove = activeUnit.MovePoints % commonMultiplier;
        string movesText;
        if (fractionalMove > 0)
        {
            var gcf = Utils.GreatestCommonFactor(fractionalMove, commonMultiplier);
            movesText =
                $"{Labels.For(LabelIndex.Moves)}: {(remainingFullPoints > 0 ? remainingFullPoints : "")} {fractionalMove / gcf}/{commonMultiplier / gcf}";
        }
        else
        {
            movesText = $"{Labels.For(LabelIndex.Moves)}: {remainingFullPoints}";
        }
        var movesLabel = new StatusLabel(_gameScreen, movesText, fontSize: fontSize);
        movesLabel.Location = new(unitDisplay.Location.X + unitDisplay.Width, currentY);
        movesLabel.Width = (int)movesLabel.TextSize.X;
        movesLabel.Height = (int)labelHeight;
        controls.Add(movesLabel);
        currentY += labelHeight;

        // Show other unit info
        var cityName = (activeUnit.HomeCity == null) ? Labels.For(LabelIndex.NONE) : activeUnit.HomeCity.Name;
        var cityNameLabel = new StatusLabel(_gameScreen, cityName, fontSize: fontSize);
        cityNameLabel.Location = new(unitDisplay.Location.X + unitDisplay.Width, currentY);
        cityNameLabel.Width = (int)cityNameLabel.TextSize.X;
        cityNameLabel.Height = (int)labelHeight;
        controls.Add(cityNameLabel);
        currentY += labelHeight;
        var ownerLabel = new StatusLabel(_gameScreen, _gameScreen.Player.Civilization.Adjective, fontSize: fontSize);
        ownerLabel.Location = new(unitDisplay.Location.X + unitDisplay.Width, currentY);
        ownerLabel.Width = (int)ownerLabel.TextSize.X;
        ownerLabel.Height = (int)labelHeight;
        controls.Add(ownerLabel);
        currentY += labelHeight;
        var nameLabel = new StatusLabel(_gameScreen, activeUnit.Veteran ? $"{activeUnit.Name} ({Labels.For(LabelIndex.Veteran)})" : 
            activeUnit.Name, fontSize: fontSize);
        nameLabel.Location = new(currentX, unitDisplay.Location.Y + unitDisplay.Height);
        nameLabel.Width = (int)nameLabel.TextSize.X;
        nameLabel.Height = (int)labelHeight;
        controls.Add(nameLabel);
        currentY = unitDisplay.Location.Y + unitDisplay.Height + labelHeight;

        // Tile name
        var activeTile = _gameScreen.Player.ActiveTile;
        var terrainName = activeTile.Name;
        if (activeTile.River)
        {
            terrainName = string.Join(", ", terrainName, Labels.For(LabelIndex.River));
        }
        var tileLabel = new StatusLabel(_gameScreen, $"({terrainName})", fontSize: fontSize);
        var tileLabelWidth = tileLabel.TextSize.X;
        tileLabel.Location = new(currentX, currentY);
        tileLabel.Width = (int)tileLabel.TextSize.X;
        tileLabel.Height = (int)labelHeight;
        controls.Add(tileLabel);
        currentY += labelHeight;

        // Specials on tile?
        if (_gameScreen.Player.ActiveTile.SpecialsName is not null)
        {
            var specialsLabel = new StatusLabel(_gameScreen, $"({_gameScreen.Player.ActiveTile.SpecialsName})", fontSize: fontSize);
            specialsLabel.Location = new(currentX, currentY);
            specialsLabel.Width = (int)specialsLabel.TextSize.X;
            specialsLabel.Height = (int)labelHeight;
            controls.Add(specialsLabel);
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
            var improvLabel = new StatusLabel(_gameScreen, $"({improvementText})", fontSize: fontSize);
            improvLabel.Location = new(currentX, currentY);
            improvLabel.Width = (int)improvLabel.TextSize.X;
            improvLabel.Height = (int)labelHeight;
            controls.Add(improvLabel);
            currentY += labelHeight;
        }

        // If airbase/fortress present
        if (improvements.Any(i => i.Imp.ExclusiveGroup == ImprovementTypes.DefenceGroup))
        {
            var airbaseText = string.Join(", ",
                improvements.Where(i => i.Imp.ExclusiveGroup == ImprovementTypes.DefenceGroup)
                    .Select(i => i.Imp.Levels[i.Const.Level].Name));
            var airbaseLabel = new StatusLabel(_gameScreen, $"({airbaseText})", fontSize: fontSize);
            airbaseLabel.Location = new(currentX, currentY);
            airbaseLabel.Width = (int)airbaseLabel.TextSize.X;
            airbaseLabel.Height = (int)labelHeight;
            controls.Add(airbaseLabel);
            currentY += labelHeight;
        }

        // If pollution present
        var pollutionText = string.Join(", ",
            improvements.Where(i => i.Imp.Negative)
                .Select(i => i.Imp.Levels[i.Const.Level].Name));
        if (!string.IsNullOrWhiteSpace(pollutionText))
        {
            var pollutionLabel = new StatusLabel(_gameScreen, $"({pollutionText})", fontSize: fontSize);
            pollutionLabel.Location = new(currentX, currentY);
            pollutionLabel.Width = (int)pollutionLabel.TextSize.X;
            pollutionLabel.Height = (int)labelHeight;
            controls.Add(pollutionLabel);
            currentY += labelHeight;
            currentY += labelHeight;
        }

        if (_gameScreen.ToTPanelLayout)
        {
            currentX = new[] { unitDisplay.Bounds.X + unitDisplay.Width + movesLabel.Width,
                               unitDisplay.Bounds.X + unitDisplay.Width + cityNameLabel.Width,
                               unitDisplay.Bounds.X + unitDisplay.Width + ownerLabel.Width }.Max();
            currentY = bounds.Y + labelHeight;
        }
        else
        {
            currentX = bounds.X;
        }

        // The rest of units on tile
        var unitsLeftOnTile = activeTile.UnitsHere.Where(u => u != activeUnit).ToList();
        for (int i = 0; i < unitsLeftOnTile.Count; i++)
        {
            var unit = unitsLeftOnTile[i];

            cityNameLabel = new StatusLabel(_gameScreen, (unit.HomeCity == null) ? Labels.For(LabelIndex.NONE) : 
                unit.HomeCity.Name, fontSize: fontSize);
            cityNameLabel.Width = (int)cityNameLabel.TextSize.X;

            var orderLabel = new StatusLabel(_gameScreen, _gameScreen.Game.Order2String(unit.Order), fontSize: fontSize);
            orderLabel.Width = (int)orderLabel.TextSize.X;

            nameLabel = new StatusLabel(_gameScreen, unit.Veteran ? $"{unit.Name} ({Labels.For(LabelIndex.Veteran)})" :
                unit.Name, fontSize: fontSize);
            nameLabel.Width = (int)nameLabel.TextSize.X;

            unitDisplay = new UnitDisplay(_gameScreen, unit, _gameScreen.Game, new Vector2(currentX, currentY), 
                _gameScreen.Main.ActiveInterface, ImageUtils.ZoomScale(unitZoom));

            var moveText = unitsLeftOnTile.Count - i == 1 ? Labels.For(LabelIndex.Unit) : Labels.For(LabelIndex.Units);
            var unitsLeftLabel = new StatusLabel(_gameScreen, $"({unitsLeftOnTile.Count - i} {Labels.For(LabelIndex.More)} {moveText})", fontSize: fontSize);
            var unitsLeftLabelWidth = unitsLeftLabel.TextSize.X;

            if (_gameScreen.ToTPanelLayout)
            {
                var maximum = new[] { currentX + unitDisplay.Width + cityNameLabel.Width,
                               currentX + unitDisplay.Width + orderLabel.Width,
                               currentX + unitDisplay.Width + nameLabel.Width }.Max();

                // max for next unit
                float maximum_nu = 0;
                if (i < unitsLeftOnTile.Count - 1)
                {
                    var next_unit = unitsLeftOnTile[i + 1];
                    var cityNameLabel_nu = new StatusLabel(_gameScreen, (next_unit.HomeCity == null) ? Labels.For(LabelIndex.NONE) :
                                    next_unit.HomeCity.Name, fontSize: fontSize);
                    var cityNameLabelWidth_nu = cityNameLabel_nu.TextSize.X;
                    var orderLabel_nu = new StatusLabel(_gameScreen, _gameScreen.Game.Order2String(next_unit.Order), fontSize: fontSize);
                    var orderLabelWidth_nu = orderLabel_nu.TextSize.X;
                    var nameLabel_nu = new StatusLabel(_gameScreen, next_unit.Veteran ? $"{next_unit.Name} ({Labels.For(LabelIndex.Veteran)})" :
                            next_unit.Name, fontSize: fontSize);
                    var nameLabelWidth_nu = nameLabel_nu.TextSize.X;

                    maximum_nu = new[] { maximum + unitDisplay.Width + cityNameLabelWidth_nu,
                               maximum + unitDisplay.Width + orderLabelWidth_nu,
                               maximum + unitDisplay.Width + nameLabelWidth_nu }.Max();
                }

                // Draw unit
                if (i == unitsLeftOnTile.Count - 1 &&
                    maximum < bounds.X + bounds.Width ||
                    i < unitsLeftOnTile.Count - 1 &&
                    (maximum_nu < bounds.X + bounds.Width ||
                    maximum_nu >= bounds.X + bounds.Width &&
                    maximum + unitsLeftLabelWidth < bounds.X + bounds.Width))
                {
                    unitDisplay.Location = new Vector2(unitDisplay.Location.X, unitDisplay.Location.Y);
                    controls.Add(unitDisplay);
                    cityNameLabel.Location = new(currentX + unitDisplay.Width, currentY);
                    cityNameLabel.Height = (int)labelHeight;
                    orderLabel.Location = new(currentX + unitDisplay.Width, currentY + labelHeight);
                    orderLabel.Height = (int)labelHeight;
                    nameLabel.Location = new(currentX + unitDisplay.Width, currentY + 2 * labelHeight);
                    nameLabel.Height = (int)labelHeight;
                    controls.Add(cityNameLabel);
                    controls.Add(orderLabel);
                    controls.Add(nameLabel);

                    currentX = maximum;
                }
                // Units left text
                else if (i == unitsLeftOnTile.Count - 1 &&
                    currentX + unitsLeftLabelWidth < bounds.X + bounds.Width ||
                    i < unitsLeftOnTile.Count - 1 &&
                    maximum_nu >= bounds.X + bounds.Width &&
                    currentX + unitsLeftLabelWidth < bounds.X + bounds.Width)
                {
                    unitsLeftLabel.Location = new(currentX, currentY);
                    unitsLeftLabel.Height = (int)labelHeight;
                    controls.Add(unitsLeftLabel);
                    break;
                }
            }
            else
            {
                // Draw unit
                if (i == unitsLeftOnTile.Count - 1 &&
                    currentY + 3 * labelHeight < bounds.Y + bounds.Height ||
                    i < unitsLeftOnTile.Count - 1 &&
                    (currentY + 6 * labelHeight < bounds.Y + bounds.Height ||
                    currentY + 6 * labelHeight >= bounds.Y + bounds.Height &&
                    currentY + 4 * labelHeight < bounds.Y + bounds.Height))
                {
                    unitDisplay.Location = new Vector2(unitDisplay.Location.X, unitDisplay.Location.Y + (3 * labelHeight - unitDisplay.Height) / 2);
                    controls.Add(unitDisplay);
                    cityNameLabel.Location = new(currentX + unitDisplay.Width, currentY);
                    cityNameLabel.Height = (int)labelHeight;
                    orderLabel.Location = new(currentX + unitDisplay.Width, currentY + labelHeight);
                    orderLabel.Height = (int)labelHeight;
                    nameLabel.Location = new(currentX + unitDisplay.Width, currentY + 2 * labelHeight);
                    nameLabel.Height = (int)labelHeight;
                    controls.Add(cityNameLabel);
                    controls.Add(orderLabel);
                    controls.Add(nameLabel);

                    currentY += 3 * labelHeight;
                }
                // Units left text
                else if (i == unitsLeftOnTile.Count - 1 &&
                    currentY + labelHeight < bounds.Y + bounds.Height ||
                    i < unitsLeftOnTile.Count - 1 &&
                    currentY + 6 * labelHeight >= bounds.Y + bounds.Height &&
                    currentY + labelHeight < bounds.Y + bounds.Height)
                {
                    unitsLeftLabel.Location = new(currentX, currentY);
                    unitsLeftLabel.Height = (int)labelHeight;
                    controls.Add(unitsLeftLabel);
                    break;
                }
            }
        }

        controls.ForEach(c => c.OnResize());
        return controls;
    }

    
    private const string GotoCursor = "GOTO_TO";
    public void MouseDown(Tile tile)
    {
        _downTime = DateTime.Now;
        _gameScreen.Main.Schedule(GotoCursor, _holdTime, () =>
        {
            Input.SetMouseCursor(MouseCursor.Crosshair);
        });
    }

    public void MouseClear()
    {
        _downTime = null;
        _gameScreen.Main.ClearSchedule(GotoCursor);
        Input.SetMouseCursor(MouseCursor.Arrow);
    }
}