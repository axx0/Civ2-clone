using System.Collections.Generic;
using System.Linq;
using Civ2engine.Enums;
using Civ2engine.Statistics;
using Civ2engine.Units;
using Model;
using Model.Constants;
using Model.Core.Units;
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

        public override ListboxGroup GetBuildListEntry(IUserInterface active, City city)
        {
            return new ListboxGroup
            {
                Elements = [ new() { Icon = GetIcon(active), Width = 2 * 36 + 2, ScaleIcon = 0.75f },
                             new() { Text = _unitDefinition.Name, Width = 200 },
                             new() { Text = $"({(10 * _unitDefinition.Cost - city.ShieldsProgress) / city.Production} Turns, ADM: " +
                             $"{_unitDefinition.Attack}/{_unitDefinition.Defense}/{_unitDefinition.Move / 3} " +
                             $"HP: {_unitDefinition.Hitp / 10}/{_unitDefinition.Firepwr})", RightAligned = true } ],
                Height = 24,
            };
        }
    }
}