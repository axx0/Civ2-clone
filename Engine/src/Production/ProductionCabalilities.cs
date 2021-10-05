using System.Collections.Generic;
using System.Linq;

namespace Civ2engine.Production
{
    public static class ProductionPossibilities
    {
        private static List<ProductionOrder>[] _availableProducts;

        public static void InitializeProductionLists(IEnumerable<Civilization> civs, ProductionOrder[] possibleOrders)
        {
            _availableProducts = civs.Select(c => possibleOrders.Where(o => o.RequiredTech == -1).ToList()).ToArray();
        }

        public static void AddItems(int targetCiv, IEnumerable<ProductionOrder> items)
        {
            _availableProducts[targetCiv].AddRange(items);
        }
        
        public static void RemoveItems(int targetCiv, IEnumerable<ProductionOrder> items)
        {
            var itemList = items.ToList();
            if (itemList.Count > 0)
            {
                _availableProducts[targetCiv].RemoveAll(i => itemList.Contains(i));
            }
        }

        public static bool ProductionValid(City city)
        {
            return _availableProducts[city.OwnerId].Contains(city.ItemInProduction) && city.ItemInProduction.IsValidBuild(city);
        }

        public static ProductionOrder AutoNext(City city)
        {
            return _availableProducts[city.OwnerId]
                .Where(p => p.RequiredTech == city.ItemInProduction.ExpiresTech && p.Type == city.ItemInProduction.Type)
                .OrderBy(p => p.Cost).FirstOrDefault();
        }
    }
}