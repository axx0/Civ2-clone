using Civ2engine;
using Civ2engine.Enums;
using Civ2engine.IO;
using Civ2engine.MapObjects;
using Civ2engine.UnitActions;
using Civ2engine.Units;
using Eto.Forms;

namespace EtoFormsUI.Players.Orders
{
    public class FortifyOrder : Order
    {
        private readonly Game _game;

        public FortifyOrder(Main mainForm, string defaultLabel, Game game) : base(mainForm, Keys.F, defaultLabel, 2)
        {
            _game = game;
        }

        public override Order Update(Tile activeTile, Unit activeUnit)
        {
            if (activeUnit == null)
            {
                SetCommandState(OrderStatus.Illegal);
            }
            else if (activeUnit.AIrole == AIroleType.Settle)
            {
                SetCommandState(OrderStatus.Illegal);
            }
            else
            {
                var canFortifyHere = UnitFunctions.CanFortifyHere(activeUnit, activeTile);
                SetCommandState(canFortifyHere.Enabled ? OrderStatus.Active : OrderStatus.Disabled);
            }

            return this;
        }

        protected override void Execute(LocalPlayer player)
        {
            player.ActiveUnit.Order = OrderType.Fortify;
            player.ActiveUnit.MovePointsLost = player.ActiveUnit.MaxMovePoints;
            _game.ChooseNextUnit();
        }
    }
}