using Civ2engine;
using Civ2engine.IO;
using Model.Images;
using Model.ImageSets;
using Model.InterfaceActions;
using RaylibUI;
using Raylib_cs;
using System.Numerics;

namespace Model;

public interface IUserInterface
{
    bool CanDisplay(string? title);
    InterfaceStyle Look { get; }
    string Title { get; }
    void Initialize();
    IInterfaceAction ProcessDialog(string dialogName, DialogResult dialogResult);
    IInterfaceAction GetInitialAction();
    
    IImageSource? BackgroundImage { get; }
    int GetCityIndexForStyle(int cityStyleIndex, City city, int citySize);
    void LoadPlayerColours();
    
    List<TerrainSet> TileSets { get; }
    
    CityImageSet CityImages { get; }
    
    UnitSet UnitImages { get; }

    PlayerColour[] PlayerColours { get; }

    CommonMapImageSet MapImages { get; }
    int DefaultDialogWidth { get; }
    Padding DialogPadding { get; }
    IList<DropdownMenuContents> ConfigureGameCommands(IList<IGameCommand> commands);
    
    CityWindowLayout GetCityWindowDefinition();

    IList<ResourceImage> ResourceImages { get; }
    PopupBox? GetDialog(string dialogName);

    UnitShield UnitShield(int unitType);
}