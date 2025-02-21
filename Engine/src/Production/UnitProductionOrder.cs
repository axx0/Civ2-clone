using System.Linq;
using Civ2engine.Enums;
using Civ2engine.Statistics;
using Civ2engine.Units;
using Model;
using Model.Constants;
using Model.Images;
using Model.Interface;

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

        public override string Title => _unitDefinition.Name;

        public override bool CompleteProduction(City city, Rules rules)
        {
            if (_unitDefinition.AIrole == AiRoleType.Settle && city.Size == 1)
            {
                return false;
            }

            var veteran = city.Improvements.Any(i =>
                i.Effects.ContainsKey(Effects.Veteran) &&
                i.Effects[Effects.Veteran] == (int)_unitDefinition.Domain);

            var unit = new Unit
            {
                Id = city.Owner.Units.Any() ? city.Owner.Units.Max(u => u.Id) + 1 : 0,
                X = city.X,
                Y = city.Y,
                HomeCity = city,
                CurrentLocation = city.Location,
                Owner = city.Owner,
                TypeDefinition = _unitDefinition,
                Veteran = veteran,
                Order = (int)OrderType.NoOrders
            };
            unit.Owner.Units.Add(unit);

            if (_unitDefinition.AIrole == AiRoleType.Settle)
            {
                city.Size -= 1;
            }

            var government = rules.Governments[city.Owner.Government];
            if (!unit.FreeSupport(government.UnitTypesAlwaysFree))
            {
                city.SetUnitSupport(government);
            }

            return true;
        }

        public override IImageSource? GetIcon(IUserInterface activeInterface)
        {
            return activeInterface.UnitImages.Units[_unitDefinition.Type].Image;
        }

        public override bool IsValidBuild(City city)
        {
            return _unitDefinition.Domain != UnitGas.Sea || city.IsNextToOcean();
        }

        public override string GetDescription()
        {
            return _unitDefinition.Name;
        }

        public override ListBoxEntry GetBuildListEntry(IUserInterface active)
        {
            return new ListBoxEntry { Icon = GetIcon(active), LeftText = _unitDefinition.Name };
        }
    }
}