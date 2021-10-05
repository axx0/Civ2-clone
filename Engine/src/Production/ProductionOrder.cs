using Civ2engine.Enums;

namespace Civ2engine.Production
{
    public abstract class ProductionOrder
    {
        private readonly bool[] _buildList;

        protected ProductionOrder(int cost, ItemType type, int imageIndex, int requiredTech, bool[] buildList = null, int expiresTech = -1)
        {
            _buildList = buildList;
            Cost = cost;
            Type = type;
            ImageIndex = imageIndex;
            RequiredTech = requiredTech;
            ExpiresTech = expiresTech;
        }

        public int ExpiresTech { get; }
        public int Cost { get; }
        public ItemType Type { get; }
        public int ImageIndex { get; }
        public int RequiredTech { get; }

        public abstract void CompleteProduction(City city, Rules rules);

        public bool CanBuild(int targetCiv)
        {
            return _buildList == null || _buildList.Length < targetCiv || _buildList[targetCiv];
        }

        public abstract bool IsValidBuild(City city);
    }
}