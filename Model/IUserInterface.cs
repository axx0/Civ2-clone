using Civ2engine;
using Model.Images;
using Model.ImageSets;
using Model.InterfaceActions;
using Raylib_cs;
using RaylibUI;

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
    int GetCityIndexForStyle(int cityStyleIndex, City city);
    
    List<TerrainSet> TileSets { get; }
    
    CityImageSet CityImages { get; }
    
    UnitSet UnitImages { get; }

    PlayerColour[] PlayerColours { get; }

    CommonMapImageSet MapImages { get; }
    int DefaultDialogWidth { get; }
    IList<string> GetMenuItems();
    CityWindowLayout GetCityWindowDefinition();
}

public class CityWindowLayout
{
    public Image Image { get; init; }
    public int Height { get; init; }
    public int Width { get; init; }
}