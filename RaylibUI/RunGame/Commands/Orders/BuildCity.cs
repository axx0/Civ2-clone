using Civ2engine;
using Civ2engine.Enums;
using Civ2engine.MapObjects;
using Civ2engine.UnitActions;
using Civ2engine.Units;
using Model;
using Model.Core;
using Model.Interface;
using Model.Menu;
using Raylib_cs;

namespace RaylibUI.RunGame.GameModes.Orders;

public class BuildCity : Order
{
    private readonly LocalPlayer _player;
    private const string CityName = "CityName";

    public BuildCity(GameScreen gameScreen) : 
        base(gameScreen, new Shortcut(KeyboardKey.B), CommandIds.BuildCityOrder)
    {
        _player = gameScreen.Player;
    }

    public override bool Update()
    {
        var activeUnit = _player.ActiveUnit;
        
        if (activeUnit == null)
        {
            return SetCommandState(CommandStatus.Invalid);
        }

        var activeTile = _player.ActiveTile;
        if (activeUnit.AIrole != AIroleType.Settle)
        {
            return SetCommandState(CommandStatus.Invalid, errorPopupKeyword: "ONLYSETTLERS");
        }
        if (activeTile.Terrain.Impassable)
        {
            return SetCommandState();
        }
        if (activeTile.Type == TerrainType.Ocean)
        {
            return SetCommandState(errorPopupKeyword: "CITYATSEA");
        }
        if (activeTile.CityHere != null)
        {
            return SetCommandState(activeTile.CityHere.Size < GameScreen.Game.Rules.Cosmic.ToExceedCitySizeAqueductNeeded ? CommandStatus.Normal : CommandStatus.Disabled, Labels.For(LabelIndex.JoinCity) , errorPopupKeyword: "ONLY10");
        }
        if (activeTile.Neighbours().Any(t => t.IsCityPresent))
        {
            return SetCommandState(errorPopupKeyword: "ADJACENTCITY");
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
            CityActions.BuildCity(_player.ActiveTile, _player.ActiveUnit, GameScreen.Game, name);
        }
    }
}