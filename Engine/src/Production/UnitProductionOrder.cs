using System;
using System.Linq;
using Civ2engine.Enums;
using Civ2engine.Improvements;
using Civ2engine.Units;

namespace Civ2engine.Production
{
    public class UnitProductionOrder : ProductionOrder
    {
        private readonly UnitDefinition _unitDefinition;

        public UnitProductionOrder(UnitDefinition unitDefinition, int index) : base(unitDefinition.Cost, ItemType.Unit,
            index, unitDefinition.Prereq, unitDefinition.CivCanBuild,  unitDefinition.Until)
        {
            _unitDefinition = unitDefinition;
        }

        public override void CompleteProduction(City city, Rules rules)
        {
            var veteran = city.Improvements.Any(i =>
                i.Effects.ContainsKey(ImprovementEffect.Veteran) &&
                i.Effects[ImprovementEffect.Veteran] == (int)_unitDefinition.Domain);
            
            var unit = new Unit
            {
                Id = city.Owner.Units.Max(u => u.Id) + 1,
                X = city.X,
                Y = city.Y,
                HomeCity = city,
                CurrentLocation = city.Location,
                Owner = city.Owner,
                TypeDefinition = _unitDefinition,
                Veteran = veteran,
                Order = OrderType.NoOrders
            };
            unit.Owner.Units.Add(unit);

            if (!unit.FreeSupport(unit.Owner.Government == GovernmentType.Fundamentalism))
            {
                city.SetUnitSupport(rules.Cosmic);
            }
        }

        public override bool IsValidBuild(City city)
        {
            return _unitDefinition.Domain != UnitGAS.Sea || city.IsNextToOcean;
        }
    }
}