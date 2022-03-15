using Civ2engine.Enums;

namespace Civ2engine
{
    public class Order
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Key { get; set; }
        public OrderType Type { get; set; }
    }
}