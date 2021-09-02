using Civ2engine.Enums;

namespace Civ2engine.Production
{
    public abstract class ProductionOrder
    {
        protected ProductionOrder(int cost, ItemType type, int imageIndex)
        {
            Cost = cost;
            Type = type;
            ImageIndex = imageIndex;
        }

        public int Cost { get; }
        public ItemType Type { get; }
        public int ImageIndex { get; }
    }
}