using Civ2engine.Production;
using Model;
using Model.Interface;

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
    
    ListBoxEntry GetBuildListEntry(IUserInterface active, int firstWonderIndex);
}