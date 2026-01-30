using Civ2engine.Production;
using Model;
using Model.Controls;
using Model.Images;

namespace Civ2engine;

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
    ListboxGroup GetBuildListEntry(IUserInterface active, City city);
}