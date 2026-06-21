using Model.Core;
using Model.Core.Production;
using Model.Core.Units;
using Model.Core.Cities;
using Model.Core.Rules;
using Model.Interfaces;

namespace Civ2engine.Production
{
    public class UnitProductionOrder(UnitDefinition unitDefinition, int index) : ProductionOrder(unitDefinition.Cost,
        ItemType.Unit,
        index, unitDefinition.Prereq, unitDefinition.CivCanBuild, unitDefinition.Until)
    {
        public override string Title => unitDefinition.Name;

        public override IImageSource? GetIcon(IUserInterface activeInterface)
        {
            if (activeInterface.UnitImages.Units is { } unitImages &&
                unitDefinition.Type >= 0 && unitDefinition.Type < unitImages.Length)
            {
                return unitImages[unitDefinition.Type].MapImage ?? unitImages[unitDefinition.Type].UiImage;
            }

            return activeInterface.PicSources.TryGetValue("unit", out var unitIcons) &&
                   unitDefinition.Type >= 0 && unitDefinition.Type < unitIcons.Length
                ? unitIcons[unitDefinition.Type]
                : null;
        }

        private bool HasFossArtIcon(IUserInterface activeInterface)
        {
            return activeInterface.UnitImages.Units is { } unitImages &&
                   unitDefinition.Type >= 0 && unitDefinition.Type < unitImages.Length &&
                   unitImages[unitDefinition.Type].MapImage != null;
        }

        public override bool IsValidBuild(City city)
        {
            return unitDefinition.Domain != UnitGas.Sea || city.IsNextToOcean();
        }

        public override string GetDescription()
        {
            return unitDefinition.Name;
        }

        public override void CompleteProduction(City city, Rules rules)
        {
            // Basic implementation - extend as needed
            Console.WriteLine($"Completing production of {unitDefinition.Name} in city {city.Name}");
        }

        public override string GetBuildListEntry(IUserInterface activeInterface, City city)
        {
            return $"{unitDefinition.Name} ({unitDefinition.Type})";
        }
    }
}