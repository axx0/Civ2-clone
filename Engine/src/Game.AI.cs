using Civ2engine.Enums;

namespace Civ2engine
{
    public partial class Game : BaseInstance
    {
        public OrderType UnitOrder()
        {
            OrderType order = OrderType.MoveN;

            return order;
        }
    }
}
