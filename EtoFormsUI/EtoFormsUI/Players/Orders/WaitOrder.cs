using Civ2engine;
using Civ2engine.IO;
using Civ2engine.MapObjects;
using Civ2engine.Units;
using Eto.Forms;

namespace EtoFormsUI.Players.Orders
{
    public class WaitOrder : Order
    {
        private readonly Game _game;

        public WaitOrder(Main mainForm, string defaultLabel, Game instance): base(mainForm, Keys.W, defaultLabel, 5)
        {
            _game = instance;
        }

        public override Order Update(Tile activeTile, Unit activeUnit)
        {
            SetCommandState(activeUnit != null ? OrderStatus.Active : OrderStatus.Illegal);
            return this;
        }

        protected override void Execute(LocalPlayer player)
        {
            player.WaitingList.Add(player.ActiveUnit);
            _game.ChooseNextUnit();
        }
    }
}