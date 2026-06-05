using Civ2engine;
using Model.Controls;
using Model.Core.Cities;
using Model.Core.GameRules;
using Model.Images;

namespace Model.Core.Production;

public interface IProductionOrder
{
    bool IsValidBuild(City city);
    int RequiredTech { get; }
    int ExpiresTech { get; }
    ItemType Type { get; }
    int Cost { get; }
    string Title { get; }
    bool CompleteProduction(City city, Rules rules);
    bool CanBuild(Civilization civilization);

    IImageSource? GetIcon(IUserInterface activeInterface);

    string GetDescription();
    ListboxGroup GetBuildListEntry(IUserInterface active, City city, int index);
}