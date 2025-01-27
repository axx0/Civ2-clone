using System.Numerics;
using System.Xml;
using System.Xml.Linq;
using Civ2engine;
using Civ2engine.Enums;
using Civ2engine.Events;
using Civ2engine.MapObjects;
using Civ2engine.Terrains;
using Model;
using Model.Menu;
using Raylib_CSharp.Interact;
using Raylib_CSharp.Transformations;
using RaylibUI.BasicTypes.Controls;
using RaylibUI.Forms;
using RaylibUI.RunGame.GameControls;
using RaylibUI.RunGame.GameControls.Mapping.Views;

namespace RaylibUI.RunGame.GameModes;

public class ViewPiece : IGameMode
{
    private readonly GameScreen _gameScreen;
    private readonly LabelControl _title;
    private readonly InterfaceStyle _look;

    public ViewPiece(GameScreen gameScreen)
    {
        _gameScreen = gameScreen;
        _look = gameScreen.MainWindow.ActiveInterface.Look;

        _title = new LabelControl(gameScreen, Labels.For(LabelIndex.ViewingPieces), true, alignment: TextAlignment.Center, font: _look.StatusPanelLabelFont, fontSize: 18, spacing: 0, colorFront: _look.MovingUnitsViewingPiecesLabelColor, colorShadow: _look.MovingUnitsViewingPiecesLabelColorShadow, shadowOffset: new Vector2(1, 0));

        Actions = new Dictionary<KeyboardKey, Func<bool>>
            {
                {
                    KeyboardKey.Enter, () =>
                    {
                        var playerActiveTile = _gameScreen.Game.ActivePlayer.ActiveTile;
                        if (playerActiveTile.CityHere != null)
                        {
                            _gameScreen.ShowCityWindow(playerActiveTile.CityHere);
                            return true;
                        }
                        if (playerActiveTile.UnitsHere.Any(u => u.MovePoints > 0))
                        {
                            _gameScreen.ActivateUnits(playerActiveTile);
                            return true;
                        }
                        /*else if (_gameScreen.StatusPanel.WaitingAtEndOfTurn)
                        {
                            main.StatusPanel.End_WaitAtEndOfTurn();
                        }*/
                        return false;
                    }
                },

                { KeyboardKey.Kp7, () => SetActive(-1, -1) }, { KeyboardKey.Kp8, () => SetActive(0, -2) },
                { KeyboardKey.Kp9, () => SetActive(1, -1) },
                { KeyboardKey.Kp1, () => SetActive(1, 1) }, { KeyboardKey.Kp2, () => SetActive(0, 2) },
                { KeyboardKey.Kp3, () => SetActive(-1, 1) },
                { KeyboardKey.Kp4, () => SetActive(-2, 0) }, { KeyboardKey.Kp6, () => SetActive(2, 0) },

                { KeyboardKey.Up, () => SetActive(0, -2) }, { KeyboardKey.Down, () => SetActive(0, 2) },
                { KeyboardKey.Left, () => SetActive(-2, 0) }, { KeyboardKey.Right, () => SetActive(2, 0) },
            };
        }

    public Dictionary<KeyboardKey,Func<bool>> Actions { get; set; }

    private bool SetActive(int deltaX, int deltaY)
    {
        var activeTile = _gameScreen.Game.ActivePlayer.ActiveTile;
        var newX = activeTile.X + deltaX;
        var newY = activeTile.Y + deltaY;
        if (activeTile.Map.IsValidTileC2(newX, newY))
        {
            _gameScreen.Game.ActivePlayer.ActiveTile = activeTile.Map.TileC2(newX, newY);
            return true;
        }
        if (!activeTile.Map.Flat && newY >= -1 && newY < activeTile.Map.YDim)
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
                _gameScreen.Game.ActivePlayer.ActiveTile = activeTile.Map.TileC2(newX, newY);
                return true;
            }
        }

        return false;
    }

    public IGameView GetDefaultView(GameScreen gameScreen, IGameView? currentView, int viewHeight, int viewWidth,
        bool forceRedraw)
    {
        if (!forceRedraw && currentView is WaitingView animation)
        {
            if (animation.ViewWidth == viewWidth && animation.ViewHeight == viewHeight &&
                animation.Location == gameScreen.Game.ActivePlayer.ActiveTile)

            {
                animation.Reset();
                return animation;
            }
        }
        _gameScreen.StatusPanel.Update();
        return new WaitingView(gameScreen, currentView, viewHeight, viewWidth, forceRedraw);
    }

    public bool MapClicked(Tile tile, MouseButton mouseButton)
    {
        if (mouseButton == MouseButton.Left)
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

        _gameScreen.Game.ActivePlayer.ActiveTile = tile;
        return true;
    }

    public bool HandleKeyPress(Shortcut key)
    {
        if (Actions.ContainsKey(key.Key))
        {
            return Actions[key.Key]();
        }
        return false;
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
        var controls = new List<IControl> { _title };

        var fontSize = _gameScreen.ToTPanelLayout ? 14 : 18;
        _title.FontSize = fontSize;
        var unitZoom = _gameScreen.ToTPanelLayout ? -1 : 1;
        var labelHeight = _title.TextSize.Y;

        _title.Bounds = bounds with { Height = labelHeight };

        var currentX = bounds.X;
        var currentY = bounds.Y + _title.Bounds.Height;

        // Draw location & tile type on active square
        var activeTile = _gameScreen.Player.ActiveTile;
        var label1 = new StatusLabel(_gameScreen, $"{Labels.For(LabelIndex.Loc)}: ({activeTile.X}, {activeTile.Y}) {activeTile.Island}", fontSize: fontSize);
        var label1W = label1.TextSize.X;
        label1.Bounds = new Rectangle(currentX, currentY, label1W, labelHeight);
        controls.Add(label1);
        currentY += labelHeight;

        // Tile name
        var terrainName = activeTile.Name;
        if (activeTile.River)
        {
            terrainName = string.Join(", ", terrainName, Labels.For(LabelIndex.River));
        }
        var tileLabel = new StatusLabel(_gameScreen, $"({terrainName})", fontSize: fontSize);
        var tileLabelWidth = tileLabel.TextSize.X;
        tileLabel.Bounds = new Rectangle(currentX, currentY, tileLabelWidth, labelHeight);
        controls.Add(tileLabel);
        currentY += labelHeight;

        // Specials on tile?
        float specialsLabelWidth = 0;
        if (_gameScreen.Player.ActiveTile.SpecialsName is not null)
        {
            var specialsLabel = new StatusLabel(_gameScreen, $"({_gameScreen.Player.ActiveTile.SpecialsName})", fontSize: fontSize);
            specialsLabelWidth = specialsLabel.TextSize.X;
            specialsLabel.Bounds = new Rectangle(currentX, currentY, specialsLabelWidth, labelHeight);
            controls.Add(specialsLabel);
            currentY += labelHeight;
        }

        // If road/railroad/irrigation/farmland/mine present
        var improvements = activeTile.Improvements.Select(c => new
        { Imp = _gameScreen.Game.TerrainImprovements[c.Improvement], Const = c }).ToList();
        var improvementText = string.Join(", ",
            improvements.Where(i => i.Imp.ExclusiveGroup != ImprovementTypes.DefenceGroup && !i.Imp.Negative)
                .Select(i => i.Imp.Levels[i.Const.Level].Name));
        float improvLabelWidth = 0;
        if (!string.IsNullOrWhiteSpace(improvementText) && activeTile.CityHere == null)
        {
            var improvLabel = new StatusLabel(_gameScreen, $"({improvementText})", fontSize: fontSize);
            improvLabelWidth = improvLabel.TextSize.X;
            improvLabel.Bounds = new Rectangle(currentX, currentY, improvLabelWidth, labelHeight);
            controls.Add(improvLabel);
            currentY += labelHeight;
        }

        // If airbase/fortress present
        float airbaseLabelWidth = 0;
        if (improvements.Any(i => i.Imp.ExclusiveGroup == ImprovementTypes.DefenceGroup))
        {
            var airbaseText = string.Join(", ",
                improvements.Where(i => i.Imp.ExclusiveGroup == ImprovementTypes.DefenceGroup)
                    .Select(i => i.Imp.Levels[i.Const.Level].Name));
            var airbaseLabel = new StatusLabel(_gameScreen, $"({airbaseText})", fontSize: fontSize);
            airbaseLabelWidth = airbaseLabel.TextSize.X;
            airbaseLabel.Bounds = new Rectangle(currentX, currentY, airbaseLabelWidth, labelHeight);
            controls.Add(airbaseLabel);
            currentY += labelHeight;
        }

        // If pollution present
        var pollutionText = string.Join(", ",
            improvements.Where(i => i.Imp.Negative)
                .Select(i => i.Imp.Levels[i.Const.Level].Name));
        float pollutionLabelWidth = 0;
        if (!string.IsNullOrWhiteSpace(pollutionText))
        {
            var pollutionLabel = new StatusLabel(_gameScreen, $"({pollutionText})", fontSize: fontSize);
            pollutionLabelWidth = pollutionLabel.TextSize.X;
            pollutionLabel.Bounds = new Rectangle(currentX, currentY, pollutionLabelWidth, labelHeight);
            controls.Add(pollutionLabel);
            currentY += labelHeight;
            currentY += labelHeight;
        }

        if (_gameScreen.ToTPanelLayout)
        {
            var maximum = new[] { label1W, tileLabelWidth, specialsLabelWidth,
                improvLabelWidth, airbaseLabelWidth, pollutionLabelWidth }.Max();
            currentX += maximum;
            currentY = bounds.Y + labelHeight;
        }
        else
        {
            currentX = bounds.X;
        }

        // Units on tile
        var unitsOnTile = activeTile.UnitsHere.ToList();
        for (int i = 0; i < unitsOnTile.Count; i++)
        {
            var unit = unitsOnTile[i];

            var cityNameLabel = new StatusLabel(_gameScreen, (unit.HomeCity == null) ? Labels.For(LabelIndex.NONE) :
                unit.HomeCity.Name, fontSize: fontSize);
            var cityNameLabelWidth = cityNameLabel.TextSize.X;

            var orderLabel = new StatusLabel(_gameScreen, _gameScreen.Game.Order2String(unit.Order), fontSize: fontSize);
            var orderLabelWidth = orderLabel.TextSize.X;

            var nameLabel = new StatusLabel(_gameScreen, unit.Veteran ? $"{unit.Name} ({Labels.For(LabelIndex.Veteran)})" :
                unit.Name, fontSize: fontSize);
            var nameLabelWidth = nameLabel.TextSize.X;

            var unitDisplay = new UnitDisplay(_gameScreen, unit, _gameScreen.Game, new Vector2(currentX, currentY),
                _gameScreen.Main.ActiveInterface, ImageUtils.ZoomScale(unitZoom));

            var moveText = unitsOnTile.Count - i == 1 ? Labels.For(LabelIndex.Unit) : Labels.For(LabelIndex.Units);
            var unitsLeftLabel = new StatusLabel(_gameScreen, $"({unitsOnTile.Count - i} {Labels.For(LabelIndex.More)} {moveText})", fontSize: fontSize);
            var unitsLeftLabelWidth = unitsLeftLabel.TextSize.X;

            if (_gameScreen.ToTPanelLayout)
            {
                var maximum = new[] { currentX + unitDisplay.Width + cityNameLabelWidth,
                               currentX + unitDisplay.Width + orderLabelWidth,
                               currentX + unitDisplay.Width + nameLabelWidth }.Max();

                // max for next unit
                float maximum_nu = 0;
                if (i < unitsOnTile.Count - 1)
                {
                    var next_unit = unitsOnTile[i + 1];
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
                if (i == unitsOnTile.Count - 1 &&
                    maximum < bounds.X + bounds.Width ||
                    i < unitsOnTile.Count - 1 &&
                    (maximum_nu < bounds.X + bounds.Width ||
                    maximum_nu >= bounds.X + bounds.Width &&
                    maximum + unitsLeftLabelWidth < bounds.X + bounds.Width))
                {
                    unitDisplay.Location = new Vector2(unitDisplay.Location.X, unitDisplay.Location.Y);
                    controls.Add(unitDisplay);
                    cityNameLabel.Bounds = new Rectangle(currentX + unitDisplay.Width, currentY, cityNameLabelWidth, labelHeight);
                    orderLabel.Bounds = new Rectangle(currentX + unitDisplay.Width, currentY + labelHeight, orderLabelWidth, labelHeight);
                    nameLabel.Bounds = new Rectangle(currentX + unitDisplay.Width, currentY + 2 * labelHeight, nameLabelWidth, labelHeight);
                    controls.Add(cityNameLabel);
                    controls.Add(orderLabel);
                    controls.Add(nameLabel);

                    currentX = maximum;
                }
                // Units left text
                else if (i == unitsOnTile.Count - 1 &&
                    currentX + unitsLeftLabelWidth < bounds.X + bounds.Width ||
                    i < unitsOnTile.Count - 1 &&
                    maximum_nu >= bounds.X + bounds.Width &&
                    currentX + unitsLeftLabelWidth < bounds.X + bounds.Width)
                {
                    unitsLeftLabel.Bounds = new Rectangle(currentX, currentY, unitsLeftLabelWidth, labelHeight);
                    controls.Add(unitsLeftLabel);
                    break;
                }
            }
            else
            {
                // Draw unit
                if (i == unitsOnTile.Count - 1 &&
                    currentY + 3 * labelHeight < bounds.Y + bounds.Height ||
                    i < unitsOnTile.Count - 1 &&
                    (currentY + 6 * labelHeight < bounds.Y + bounds.Height ||
                    currentY + 6 * labelHeight >= bounds.Y + bounds.Height &&
                    currentY + 4 * labelHeight < bounds.Y + bounds.Height))
                {
                    unitDisplay.Location = new Vector2(unitDisplay.Location.X, unitDisplay.Location.Y + (3 * labelHeight - unitDisplay.Height) / 2);
                    controls.Add(unitDisplay);
                    cityNameLabel.Bounds = new Rectangle(currentX + unitDisplay.Width, currentY, cityNameLabelWidth, labelHeight);
                    orderLabel.Bounds = new Rectangle(currentX + unitDisplay.Width, currentY + labelHeight, orderLabelWidth, labelHeight);
                    nameLabel.Bounds = new Rectangle(currentX + unitDisplay.Width, currentY + 2 * labelHeight, nameLabelWidth, labelHeight);
                    controls.Add(cityNameLabel);
                    controls.Add(orderLabel);
                    controls.Add(nameLabel);

                    currentY += 3 * labelHeight;
                }
                // Units left text
                else if (i == unitsOnTile.Count - 1 &&
                    currentY + labelHeight < bounds.Y + bounds.Height ||
                    i < unitsOnTile.Count - 1 &&
                    currentY + 6 * labelHeight >= bounds.Y + bounds.Height &&
                    currentY + labelHeight < bounds.Y + bounds.Height)
                {
                    unitsLeftLabel.Bounds = new Rectangle(currentX, currentY, unitsLeftLabelWidth, labelHeight);
                    controls.Add(unitsLeftLabel);
                    break;
                }
            }
        }

        controls.ForEach(c => c.OnResize());
        return controls;
    }

    public void MouseDown(Tile tile)
    {
        // NO op
    }

    public void MouseClear()
    {
        
    }
}
