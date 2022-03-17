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

        public abstract bool CompleteProduction(City city, Rules rules);

        public bool CanBuild(Civilization targetCiv)
        {
            if (_buildList == null)
            {
                return true;
            }

            if (_buildList.Length > targetCiv.NormalColour)
            {
                return _buildList[targetCiv.NormalColour];
            }

            return _buildList.Length <= targetCiv.Id || _buildList[targetCiv.Id];
        }

        public abstract bool IsValidBuild(City city);

        public abstract string GetDescription();
    }
}