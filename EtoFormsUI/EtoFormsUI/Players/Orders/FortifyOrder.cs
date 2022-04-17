using Civ2engine.Enums;
using Civ2engine.MapObjects;
using Civ2engine.UnitActions;
using Civ2engine.Units;
using Eto.Forms;

namespace EtoFormsUI.Players.Orders
{
    public class FortifyOrder : Order
    {
        public FortifyOrder(Main mainForm, string defaultLabel, int @group) : base(mainForm, Keys.F, defaultLabel, @group)
        {
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
            throw new System.NotImplementedException();
        }
    }
}