using System.Collections.Generic;
using System.Linq;
using Civ2engine;
using Civ2engine.Enums;
using Civ2engine.MapObjects;
using Civ2engine.Terrains;
using Civ2engine.Units;
using Eto.Forms;

namespace EtoFormsUI.Players.Orders
{
    public class PillageOrder : Order
    {
        private readonly Game _game;

        public PillageOrder(Main mainForm, string defaultLabel, Game game) : base(mainForm, Keys.Shift | Keys.P, defaultLabel, 1)
        {
            _game = game;
        }

        public override Order Update(Tile activeTile, Unit activeUnit)
        {
            if (activeTile == null || activeTile.IsCityPresent)
            {
                SetCommandState(OrderStatus.Illegal);
                
            }else if (activeUnit == null || activeUnit.AttackBase == 0)
            {
                SetCommandState(OrderStatus.Illegal);
            }
            else
            {
                if (activeTile.Improvements.Count > 0 && activeTile.Improvements.Any(i=> !_game.TerrainImprovements[i.Improvement].Negative))
                {
                    SetCommandState(OrderStatus.Active);
                }
                else
                {
                    SetCommandState(OrderStatus.Illegal);
                }
            }
            return this;
        }

        protected override void Execute(LocalPlayer player)
        {
            var improvements = player.ActiveTile.Improvements.Where(i =>
                _game.TerrainImprovements.ContainsKey(i.Improvement) &&
                !_game.TerrainImprovements[i.Improvement].Negative).ToList();
            ConstructedImprovement improvementToPillage = null;
            if (improvements.Count > 1)
            {
                var popup = _mainForm.popupBoxList["PILLAGEWHAT"];
                var dialog = new Civ2dialog(_mainForm, popup, listbox: new ListboxDefinition
                {
                    LeftText = improvements
                        .Select(i => _game.TerrainImprovements[i.Improvement].Levels[i.Level].Name).ToList()
                });
                dialog.ShowModal();
                if (dialog.SelectedButton == "OK")
                {
                    improvementToPillage = improvements[dialog.SelectedIndex];
                }
            }
            else
            {
                improvementToPillage = improvements.FirstOrDefault();
            }

            if (improvementToPillage != null)
            {
                player.ActiveUnit.MovePointsLost += _game.Rules.Cosmic.MovementMultiplier;
                
                var improvement = _game.TerrainImprovements[improvementToPillage.Improvement];
                player.ActiveTile.RemoveImprovement(improvement,improvementToPillage.Level);
                var tiles = new List<Tile> { player.ActiveTile };
                if (improvement.HasMultiTile)
                {
                    tiles.AddRange(player.ActiveTile.Neighbours());
                }
                _game.TriggerMapEvent(MapEventType.UpdateMap, tiles);
                if (player.ActiveUnit.MovePoints <= 0)
                {
                    _game.ChooseNextUnit();
                }
            }
        }
    }
}