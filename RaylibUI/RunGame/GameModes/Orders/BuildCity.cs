using Civ2engine;
using Civ2engine.Enums;
using Civ2engine.MapObjects;
using Civ2engine.UnitActions;
using Civ2engine.Units;
using Model.Interface;
using Raylib_cs;

namespace RaylibUI.RunGame.GameModes.Orders;

public class BuildCity : Order
{
    private readonly Game _game;

    public BuildCity(GameScreen gameScreen, string defaultLabel, Game game) : 
        base(gameScreen, KeyboardKey.KEY_B, defaultLabel, 0)
    {
        _game = game;
    }

    public override Order Update(Tile activeTile, Unit activeUnit)
    {
        if (activeTile == null || activeUnit == null)
        {
            SetCommandState(OrderStatus.Illegal);
        }
        else
        {
            if (activeUnit.AIrole != AIroleType.Settle)
            {
                SetCommandState(OrderStatus.Illegal, errorPopupKeyword: "ONLYSETTLERS");
            }else if (activeTile.Terrain.Impassable)
            {
                SetCommandState();
            }else if (activeTile.Type == TerrainType.Ocean)
            {
                SetCommandState(OrderStatus.Disabled, errorPopupKeyword: "CITYATSEA");
            }else if (activeTile.IsCityPresent)
            {
                SetCommandState(activeTile.CityHere.Size < _game.Rules.Cosmic.ToExceedCitySizeAqueductNeeded ? OrderStatus.Active : OrderStatus.Disabled, Labels.For(LabelIndex.JoinCity) , errorPopupKeyword: "ONLY10");
            }else if (activeTile.Neighbours().Any(t => t.IsCityPresent))
            {
                SetCommandState(OrderStatus.Disabled, errorPopupKeyword: "ADJACENTCITY");
            }
            else
            {
                SetCommandState(OrderStatus.Active);
            }
        }
        return this;
    }

    protected override void Execute(LocalPlayer player)
    {
        if (player.ActiveUnit.CurrentLocation.IsCityPresent)
        {
            player.ActiveTile.CityHere.GrowCity(_game);
            var unit = player.ActiveUnit;
            unit.Dead = true;
            unit.MovePointsLost = unit.MovePoints;
            _game.ChooseNextUnit();
        }
        else
        {
            //var box = GameScreen.popupBoxList["NAMECITY"];
            //if (box.Options is not null)
            //{
            //    box.Text = box.Options;
            //    box.Options = null; 
            //}

            //var name = CityActions.GetCityName(player.Civ, _game);
            //var cityNameDialog = new Civ2dialog(_mainForm, box,
            //    textBoxes: new List<TextBoxDefinition>
            //    {
            //        new()
            //        {
            //            index = 0,
            //            InitialValue = name,
            //            Name = "CityName",
            //            Width = 225
            //        }
            //    });
            //cityNameDialog.ShowModal(_mainForm);
            //if (cityNameDialog.SelectedIndex != int.MinValue)
            //{
            //    CityActions.BuildCity(player.ActiveTile, player.ActiveUnit, _game, name);
            //}
        }
    }
}