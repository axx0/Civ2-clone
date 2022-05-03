using Civ2engine;
using Civ2engine.Enums;
using Civ2engine.MapObjects;
using Civ2engine.Units;
using Eto.Forms;

namespace EtoFormsUI.Players.Orders
{
    public class SleepOrder : Order
    {
        private readonly Game _game;

        public SleepOrder(Main mainForm, string defaultLabel, Game game) : base(mainForm, Keys.Space, defaultLabel, 5)
        {
            _game = game;
        }

        public override Order Update(Tile activeTile, Unit activeUnit)
        {
            SetCommandState(activeUnit != null ? OrderStatus.Active : OrderStatus.Illegal);
            return this;
        }

        protected override void Execute(LocalPlayer player)
        {
            player.ActiveUnit.Order = OrderType.Sleep;
            _game.ChooseNextUnit();
        }
    }
}