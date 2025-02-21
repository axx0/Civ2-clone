using Civ2engine;
using Civ2engine.Enums;
using Civ2engine.MapObjects;
using Civ2engine.UnitActions;
using Civ2engine.Units;
using Model;
using Model.Constants;
using Model.Core;
using Model.Interface;
using Model.Menu;
using Raylib_CSharp.Interact;
using RaylibUI.Controls;
using RaylibUtils;

namespace RaylibUI.RunGame.GameModes.Orders;

public class BuildCity : Order
{
    private readonly LocalPlayer _player;
    private const string CityName = "CityName";
    private IUserInterface _active;
    private GameScreen _screen;
    private City _city;

    public BuildCity(GameScreen gameScreen) : 
        base(gameScreen, new Shortcut(KeyboardKey.B), CommandIds.BuildCityOrder)
    {
        _screen = gameScreen;
        _player = gameScreen.Player;
        _active = gameScreen.Main.ActiveInterface;
    }

    public override bool Update()
    {
        var activeUnit = _player.ActiveUnit;
        
        if (activeUnit == null)
        {
            return SetCommandState(CommandStatus.Invalid);
        }

        var activeTile = activeUnit.CurrentLocation;
        if (activeUnit.AiRole != AiRoleType.Settle)
        {
            return SetCommandState(errorPopupKeyword: "ONLYSETTLERS", errorPopupImage: new(_active.PicSources["unit"][0], 2));
        }
        if (activeTile.Terrain.Impassable)
        {
            return SetCommandState();
        }
        if (activeTile.Type == TerrainType.Ocean)
        {
            return SetCommandState(errorPopupKeyword: "CITYATSEA");
        }
        if (activeTile.CityHere != null || activeTile.Neighbours().Any(t => t.IsCityPresent))
        {
            var city = activeTile.CityHere ?? activeTile.Neighbours().First(t => t.IsCityPresent).CityHere;

            var cityStyleIndex = _screen.Game.Players[city.OwnerId].Civilization.CityStyle;
            if (city.Owner.Epoch == (int)EpochType.Industrial)
            {
                cityStyleIndex = 4;
            }
            else if (city.Owner.Epoch == (int)EpochType.Modern)
            {
                cityStyleIndex = 5;
            }

            var sizeIncrement =
                _screen.Main.ActiveInterface.GetCityIndexForStyle(cityStyleIndex, city, city.Size);
            var cityImage = _active.CityImages.Sets[cityStyleIndex][sizeIncrement];
            var flagImage = _screen.Main.ActiveInterface.PlayerColours[city.OwnerId];

            if (activeTile.CityHere != null)
            {
                return SetCommandState(activeTile.CityHere.Size < GameScreen.Game.Rules.Cosmic.ToExceedCitySizeAqueductNeeded ? CommandStatus.Normal : CommandStatus.Disabled, Labels.For(LabelIndex.JoinCity), errorPopupKeyword: "ONLY10", errorPopupImage: new(new[] { cityImage.Image, flagImage.Image }, 2, coords: new int[,] { { 0, 0 }, { (int)cityImage.FlagLoc.X, (int)cityImage.FlagLoc.Y - Images.GetImageHeight(flagImage.Image, _active) - 5 } }));
            }
            else
            {
                return SetCommandState(errorPopupKeyword: "ADJACENTCITY", errorPopupImage: new(new[] { cityImage.Image, flagImage.Image }, 2, coords: new int[,] { { 0, 0 }, { (int)cityImage.FlagLoc.X, (int)cityImage.FlagLoc.Y - Images.GetImageHeight(flagImage.Image, _active) - 5 } }));
            }
        }

        return SetCommandState(CommandStatus.Normal);
    }

    public override void Action()
    {
        if (_player.ActiveUnit.CurrentLocation.IsCityPresent)
        {
            _player.ActiveTile.CityHere.GrowCity(GameScreen.Game);
            var unit = _player.ActiveUnit;
            unit.Dead = true;
            unit.MovePointsLost = unit.MovePoints;
            GameScreen.Game.ChooseNextUnit();
        }
        else
        {
            var name = CityActions.GetCityName(_player.Civilization, GameScreen.Game);
            GameScreen.ShowPopup("NAMECITY", handleButtonClick: Build,
            textBoxes: new List<TextBoxDefinition>
            {
                new()
                {
                    Index = 0,
                    InitialValue = name,
                    Name = CityName,
                    Width = 225
                }
            });
        }
    }

    private void Build(string button, int selectedIndex, IList<bool>? check, IDictionary<string, string>? textBoxes)
    {
        if (textBoxes != null && button == Labels.Ok && textBoxes.TryGetValue(CityName, out var name))
        {
            _city = CityActions.BuildCity(_player.ActiveTile, _player.ActiveUnit, GameScreen.Game, name);

            GameScreen.ShowPopup("FOUNDED", handleButtonClick: OpenCityWindow, 
                dialogImage: new(new[] { _player.Civilization.Epoch < 2 ? _active.PicSources["cityBuiltAncient"][0] : _active.PicSources["cityBuiltModern"][0] }),
                replaceStrings: new List<string> { name, GameScreen.Game.Date.GameYearString(GameScreen.Game.TurnNumber) });
        }
    }

    private void OpenCityWindow(string button, int selectedIndex, IList<bool>? check, IDictionary<string, string>? textBoxes)
    {
        if (button == Labels.Ok)
        {
            GameScreen.ShowCityWindow(_city);   // TODO: chose next unit after handling button click
            GameScreen.Game.ChooseNextUnit();
        }
    }
}