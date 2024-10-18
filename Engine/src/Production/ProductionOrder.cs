using System.Linq;
using Model;
using Model.Images;
using Model.Interface;

namespace Civ2engine.Production
{
    public abstract class ProductionOrder : IProductionOrder
    {
        private readonly bool[]? _buildList;

        protected ProductionOrder(int cost, ItemType type, int imageIndex, int requiredTech, bool[]? buildList = null,
            int expiresTech = -1)
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
        public abstract string Title { get; }
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

        public abstract IImageSource? GetIcon(IUserInterface activeInterface);

        public abstract bool IsValidBuild(City city);

        public abstract string GetDescription();
        public abstract ListBoxEntry GetBuildListEntry(IUserInterface active);

        public static IProductionOrder[] GetAll(Rules rules)
        {
            return rules.ProductionOrders ??= rules.UnitTypes.Select((u, index) => new UnitProductionOrder(u, index))
                .Cast<IProductionOrder>()
                .Concat(rules.Improvements[1..].Select(((imp, i) => new BuildingProductionOrder(imp, i, rules.FirstWonderIndex)))).ToArray();

        }

    }
}