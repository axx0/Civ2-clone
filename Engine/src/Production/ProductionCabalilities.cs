using System.Collections.Generic;
using System.Linq;
using Civ2engine.Advances;
using Model.Constants;

namespace Civ2engine.Production
{
    public static class ProductionPossibilities
    {
        private static List<IProductionOrder>[] _availableProducts;

        public static void InitializeProductionLists(IEnumerable<Civilization> civs, IProductionOrder[] possibleOrders)
        {
            var orders = possibleOrders
                .Where(o => o.RequiredTech != AdvancesConstants.No && o.ExpiresTech != AdvancesConstants.No).ToList();

            _availableProducts = civs.Select(c =>
                    orders.Where(o =>
                        (o.ExpiresTech == AdvancesConstants.Nil || (o.ExpiresTech < c.Advances.Length && !c.Advances[o.ExpiresTech])) &&
                        (o.RequiredTech == AdvancesConstants.Nil || (o.RequiredTech < c.Advances.Length && c.Advances[o.RequiredTech]))).ToList())
                .ToArray();
        }

        public static void AddItems(int targetCiv, IEnumerable<IProductionOrder> items)
        {
            _availableProducts[targetCiv].AddRange(items);
        }
        
        public static void RemoveItems(int targetCiv, IEnumerable<IProductionOrder> items)
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

        public static IProductionOrder? AutoNext(City city)
        {
            return _availableProducts[city.OwnerId]
                .Where(p => p.RequiredTech == city.ItemInProduction.ExpiresTech && p.Type == city.ItemInProduction.Type)
                .MinBy(p => p.Cost);
        }

        public static Improvement? FindByEffect(int targetCiv, Effects effect)
        {
            return _availableProducts[targetCiv].OfType<BuildingProductionOrder>()
                .Where(p => p.Improvement.Effects.ContainsKey(effect)).Select(o => o.Improvement).FirstOrDefault();
        }

        public static IList<IProductionOrder> GetAllowedProductionOrders(City thisCity)
        {
            return _availableProducts[thisCity.OwnerId].Where(i => i.IsValidBuild(thisCity)).ToList();
        }
    }
}