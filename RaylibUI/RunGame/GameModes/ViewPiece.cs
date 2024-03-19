using System.Numerics;
using System.Xml.Linq;
using Civ2engine;
using Civ2engine.Enums;
using Civ2engine.Events;
using Civ2engine.MapObjects;
using Civ2engine.Terrains;
using Model;
using Model.Menu;
using Raylib_cs;
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
    private bool _endOfTurn;

    public ViewPiece(GameScreen gameScreen)
    {
        _look = gameScreen.MainWindow.ActiveInterface.Look;

        _title = new LabelControl(gameScreen, Labels.For(LabelIndex.ViewingPieces), true, alignment: TextAlignment.Center, font: _look.StatusPanelLabelFont, fontSize: _look.StatusPanelLabelFontSize, colorFront: _look.MovingUnitsViewingPiecesLabelColor, colorShadow: _look.MovingUnitsViewingPiecesLabelColorShadow, shadowOffset: new Vector2(1, 0), spacing: 0);

        _gameScreen = gameScreen;
        _gameScreen.Game.OnPlayerEvent += PlayerEventTriggered;

        Actions = new Dictionary<KeyboardKey, Func<bool>>
            {
                {
                    KeyboardKey.Enter, () =>
                    {
                        if (_gameScreen.Game.ActiveTile.CityHere != null)
                        {
                            _gameScreen.ShowCityWindow(_gameScreen.Game.ActiveTile.CityHere);
                            return true;
                        }
                        if (_gameScreen.Game.ActiveTile.UnitsHere.Any(u => u.MovePoints > 0))
                        {
                            _gameScreen.ActivateUnits(_gameScreen.Game.ActiveTile);
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
        var activeTile = _gameScreen.Game.ActiveTile;
        var newX = activeTile.X + deltaX;
        var newY = activeTile.Y + deltaY;
        if (activeTile.Map.IsValidTileC2(newX, newY))
        {
            _gameScreen.Game.ActiveTile = activeTile.Map.TileC2(newX, newY);
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
                _gameScreen.Game.ActiveTile = activeTile.Map.TileC2(newX, newY);
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

        _gameScreen.Game.ActiveTile = tile;
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
        var startY = bounds.Y + 61;
        _title.Bounds = bounds with { Y = startY, Height = _title.GetPreferredHeight() };

        var labelHeight = 18;
        var currentY = startY + 34;

        // Draw location & tile type on active square
        var activeTile = _gameScreen.Player.ActiveTile;

        controls.Add(new StatusLabel(_gameScreen, $"{Labels.For(LabelIndex.Loc)}: ({activeTile.X}, {activeTile.Y}) {activeTile.Island}")
        {
            Bounds = bounds with { Height = labelHeight, Y = currentY }
        });
        currentY += labelHeight;

        var terrainName = activeTile.Name;
        if (activeTile.River)
        {
            terrainName = string.Join(", ", terrainName, Labels.For(LabelIndex.River));
        }
        controls.Add(new StatusLabel(_gameScreen, $"({terrainName})")
        {
            Bounds = bounds with { Height = labelHeight, Y = currentY }
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

        // Units on tile
        var offsetX = 7;
        currentY += 7;
        var deltaEndTurn = _endOfTurn ? 46 : 0;
        var unitsCanBeDisplayed = Math.Floor((bounds.Y + bounds.Height - currentY - deltaEndTurn) / 56);
        if (activeTile.UnitsHere.Count > unitsCanBeDisplayed)
        {
            var delta = 26 + deltaEndTurn;
            unitsCanBeDisplayed = Math.Floor((bounds.Y + bounds.Height - currentY - delta) / 56); // Take bottom texts into account
        }
        else
        {
            unitsCanBeDisplayed = activeTile.UnitsHere.Count;
        }
        for (int i = 0; i < unitsCanBeDisplayed; i++)
        {
            var unit = activeTile.UnitsHere[i];
            var unitImage = new UnitDisplay(_gameScreen, unit,
                new Vector2(bounds.X + offsetX, currentY), _gameScreen.Main.ActiveInterface, (float)9 / 8);
            controls.Add(unitImage);
            currentY += unitImage.Height + 8;
            var cityName = (unit.HomeCity == null) ? Labels.For(LabelIndex.NONE) : unit.HomeCity.Name;
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
        if (activeTile.UnitsHere.Count > unitsCanBeDisplayed)
        {
            var moveText = activeTile.UnitsHere.Count - unitsCanBeDisplayed == 1 ? Labels.For(LabelIndex.Unit) : Labels.For(LabelIndex.Units);
            controls.Add(new StatusLabel(_gameScreen, $"({activeTile.UnitsHere.Count - unitsCanBeDisplayed} {Labels.For(LabelIndex.More)} {moveText})")
            {
                Bounds = bounds with { Y = currentY, Height = labelHeight }
            });
        }

        // End of turn message
        var btmOffset = 31;
        if (_endOfTurn)
        {
            controls.Add(new StatusLabel(_gameScreen, Labels.For(LabelIndex.EndOfTurn), switchColors: _look.EndOfTurnColors, switchTime: 500)
            {
                Bounds = bounds with { Y = bounds.Y + bounds.Height - btmOffset - 17, Height = labelHeight }
            });
            controls.Add(new StatusLabel(_gameScreen, $"({Labels.For(LabelIndex.PressEnter)})", switchColors: _look.EndOfTurnColors, switchTime: 500)
            {
                Bounds = bounds with { Y = bounds.Y + bounds.Height - btmOffset, Height = labelHeight }
            });
        }

        controls.ForEach(c => c.OnResize());
        return controls;
    }

    private void PlayerEventTriggered(object sender, PlayerEventArgs e)
    {
        switch (e.EventType)
        {
            case PlayerEventType.NewTurn:
                {
                    _endOfTurn = false;
                    break;
                }
            case PlayerEventType.WaitingAtEndOfTurn:
                {
                    _endOfTurn = true;
                    break;
                }
            default: break;
        }
    }
}
