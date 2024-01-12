using Civ2engine;
using Civ2engine.Enums;
using Civ2engine.MapObjects;
using Civ2engine.UnitActions;
using Civ2engine.Units;
using Model;
using Model.Interface;
using Model.Menu;
using Raylib_cs;

namespace RaylibUI.RunGame.GameModes.Orders;

public class BuildCity : Order
{
    private readonly LocalPlayer _player;
    private const string CityName = "CityName";

    public BuildCity(GameScreen gameScreen) : 
        base(gameScreen, new Shortcut(KeyboardKey.KEY_B), CommandIds.BuildCityOrder)
    {
        _player = gameScreen.Player;
    }

    public override void Update()
    {
        var activeUnit = _player.ActiveUnit;
        
        if (activeUnit == null)
        {
            SetCommandState(CommandStatus.Invalid);
        }
        else
        {
            var activeTile = _player.ActiveTile;
            if (activeUnit.AIrole != AIroleType.Settle)
            {
                SetCommandState(CommandStatus.Invalid, errorPopupKeyword: "ONLYSETTLERS");
            }else if (activeTile.Terrain.Impassable)
            {
                SetCommandState();
            }else if (activeTile.Type == TerrainType.Ocean)
            {
                SetCommandState(errorPopupKeyword: "CITYATSEA");
            }else if (activeTile.CityHere != null)
            {
                SetCommandState(activeTile.CityHere.Size < _gameScreen.Game.Rules.Cosmic.ToExceedCitySizeAqueductNeeded ? CommandStatus.Normal : CommandStatus.Disabled, Labels.For(LabelIndex.JoinCity) , errorPopupKeyword: "ONLY10");
            }else if (activeTile.Neighbours().Any(t => t.IsCityPresent))
            {
                SetCommandState(errorPopupKeyword: "ADJACENTCITY");
            }
            else
            {
                SetCommandState(CommandStatus.Normal);
            }
        }
    }

    public override void Action()
    {
        if (_player.ActiveUnit.CurrentLocation.IsCityPresent)
        {
            _player.ActiveTile.CityHere.GrowCity(_gameScreen.Game);
            var unit = _player.ActiveUnit;
            unit.Dead = true;
            unit.MovePointsLost = unit.MovePoints;
            _gameScreen.Game.ChooseNextUnit();
        }
        else
        {
            
            var name = CityActions.GetCityName(_player.Civilization, _gameScreen.Game);
            _gameScreen.ShowPopup("NAMECITY", handleButtonClick: Build,
                
                textBoxes: new List<TextBoxDefinition>
            {
                new()
                {
                    index = 0,
                    InitialValue = name,
                    Name = CityName,
                    Width = 225
                }
            });
        }
    }

    private void Build(string button, int selectedIndex, IList<bool>? check, IDictionary<string, string>? textBoxes)
    {
        if (textBoxes != null && selectedIndex != int.MinValue && textBoxes.TryGetValue(CityName, out var name))
        {
            CityActions.BuildCity(_player.ActiveTile, _player.ActiveUnit, _gameScreen.Game, name);
        }
    }
}