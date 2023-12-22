using Civ2engine;
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
    int GetCityIndexForStyle(int cityStyleIndex, City city);
    
    List<TerrainSet> TileSets { get; }
    
    CityImageSet CityImages { get; }
    
    UnitSet UnitImages { get; }

    PlayerColour[] PlayerColours { get; }

    CommonMapImageSet MapImages { get; }
    int DefaultDialogWidth { get; }
    Padding DialogPadding { get; }
    IList<string> GetMenuItems();
    CityWindowLayout GetCityWindowDefinition();

    IList<ResourceImage> ResourceImages { get; }
    Vector2 GetShieldStackingOffset(int stackingDir);
    Vector2 GetHealthbarOffset();
    Vector2 GetHPbarSize();
    Color GetHPbarColour(int hpBarX);
    Vector2 GetShieldOrderTextOffset(Texture2D shieldTexture);
    int GetShieldOrderTextHeight(Texture2D shieldTexture);
}