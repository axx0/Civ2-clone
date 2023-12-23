using System.Numerics;
using Civ2.Dialogs;
using Civ2.Rules;
using Civ2engine;
using Civ2engine.Improvements;
using Civ2engine.IO;
using Civ2engine.Units;
using Model;
using Model.Images;
using Model.ImageSets;
using Model.InterfaceActions;
using Raylib_cs;
using RaylibUI;
using RayLibUtils;

namespace Civ2;

public abstract class Civ2Interface : IUserInterface
{
    public bool CanDisplay(string? title)
    {
        return title == Title;
    }

    public InterfaceStyle Look { get; } = new()
    {
        Outer = new BitmapStorage("ICONS", new Rectangle(199, 322, 64, 32)),
        Inner = new BitmapStorage("ICONS", new Rectangle(298, 190, 32, 32)),
        RadioButtons = new IImageSource[]
            { new BitmapStorage("buttons.png", 0, 0, 32), new BitmapStorage("buttons.png", 32, 0, 32) },
        CheckBoxes = new IImageSource[]
            { new BitmapStorage("buttons.png", 0, 32, 32), new BitmapStorage("buttons.png", 32, 32, 32) },
        DefaultFont = "times-new-roman.ttf",
        BoldFont = "times-new-roman-bold.ttf",
        AlternativeFont = "ARIAL.ttf"
    };


    public abstract string Title { get; }

    public virtual void Initialize()
    {
        Dialogs = PopupBoxReader.LoadPopupBoxes(Settings.Civ2Path);
        foreach (var value in Dialogs.Values)
        {
            value.Width = (int)(value.Width * 1.5m);
        }
        Labels.UpdateLabels(null);

        var handlerInterface = typeof(ICivDialogHandler);
        DialogHandlers = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(t => t != handlerInterface && handlerInterface.IsAssignableFrom(t) && !t.IsAbstract)
            .Select(Activator.CreateInstance)
            .OfType<ICivDialogHandler>()
            .Select(h => h.UpdatePopupData(Dialogs))
            .ToDictionary(k => k.Name);
    }

    protected Dictionary<string, ICivDialogHandler> DialogHandlers { get; private set; }

    public IInterfaceAction ProcessDialog(string dialogName, DialogResult dialogResult)
    {
        if (!DialogHandlers.ContainsKey(dialogName))
        {
            throw new NotImplementedException(dialogName);
        }

        return DialogHandlers[dialogName].HandleDialogResult(dialogResult, DialogHandlers, this);
    }

    public abstract string InitialMenu { get; }

    public IInterfaceAction GetInitialAction()
    {
        return DialogHandlers[InitialMenu].Show(this);
    }

    public virtual IImageSource? BackgroundImage => null;
    
    public int GetCityIndexForStyle(int cityStyleIndex, City city)
    {
        var index = cityStyleIndex switch
        {
            4 => city.Size switch
            {
                <= 4 => 0,
                > 4 and <= 7 => 1,
                > 7 and <= 10 => 2,
                _ => 3
            },
            5 => city.Size switch
            {
                <= 4 => 0,
                > 4 and <= 10 => 1,
                > 10 and <= 18 => 2,
                _ => 3
            },
            _ => city.Size switch
            {
                <= 3 => 0,
                > 3 and <= 5 => 1,
                > 5 and <= 7 => 2,
                _ => 3
            }
        };

        if (index < 3 && city.Improvements.Any(i => i.Effects.ContainsKey(Effects.Capital)))
        {
            index++;
        }

        if (city.Improvements.Any(i => i.Effects.ContainsKey(Effects.Walled)))
        {
            index += 4;
        }

        return index;
    }
    public List<TerrainSet> TileSets { get; } = new();

    public CityImageSet CityImages { get; } = new();

    public UnitSet UnitImages { get; } = new();
    
    

    public Dictionary<string, PopupBox> Dialogs { get; set; }
    public PlayerColour[] PlayerColours { get; set; }
    public int ExpectedMaps { get; set; } = 1;
    public CommonMapImageSet MapImages { get; } = new();
    public int DefaultDialogWidth => 660; // 660=440*1.5

    public Padding DialogPadding { get; } = new(11);

    public IList<string> GetMenuItems()
    {
        MenuLoader.LoadMenus(Initialization.ConfigObject.RuleSet);

        return MenuLoader.Menus;
    }

    private CityWindowLayout? _cityWindowLayout;

    public CityWindowLayout GetCityWindowDefinition()
    {
        if (_cityWindowLayout != null) return _cityWindowLayout;

        float buttonHeight = 24;

        const float buyButtonWidth = 68;
        const int infoButtonWidth = 57;

        _cityWindowLayout = new CityWindowLayout(new BitmapStorage("city"))
        {
            Height = 420, Width = 640,
            InfoPanel = new Rectangle(197, 216, 233, 198),
            TileMap = new Rectangle(7, 65, 188, 137),
            FoodStorage = new Rectangle(452, 0, 165, 162),
            Resources = new ResourceProduction
            {
                TitlePosition = new Vector2(318, 46),
                Resources = new List<ResourceArea>
                {
                    new ConsumableResourceArea(name: "Food",
                        bounds: new Rectangle(199, 75, 238, 16),
                        getDisplayDetails: (val, type) =>
                        {
                            return type switch
                            {
                                OutputType.Consumption => Labels.For(LabelIndex.Food),
                                OutputType.Loss => Labels.For(LabelIndex.Hunger),
                                OutputType.Surplus => Labels.For(LabelIndex.Surplus),
                                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
                            } + ":" + val;
                        }),
                    new ConsumableResourceArea(name: "Trade",
                        bounds: new Rectangle(206, 116, 224, 16),
                        getDisplayDetails: (val, type) =>
                        {
                            return type switch
                            {
                                OutputType.Consumption => Labels.For(LabelIndex.Trade),
                                OutputType.Loss => Labels.For(LabelIndex.Corruption),
                                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
                            } + ":" + val;
                        }, noSurplus: true),
                    new ConsumableResourceArea(name: "Shields",
                        bounds: new Rectangle(199, 181, 238, 16),
                        getDisplayDetails: (val, type) =>
                        {
                            return type switch
                            {
                                OutputType.Consumption => Labels.For(LabelIndex.Support),
                                OutputType.Loss => val > 0
                                    ? Labels.For(LabelIndex.Waste)
                                    : Labels.For(LabelIndex.Shortage),
                                OutputType.Surplus => Labels.For(LabelIndex.Production),
                                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
                            } + ":" + Math.Abs(val);
                        },
                        labelBelow: true
                    ),
                    new SharedResourceArea(new Rectangle(206, 140, 224, 16), true)
                    {
                        Resources = new List<ResourceInfo>
                        {
                            new()
                            {
                                Name = "Tax",
                                GetResourceLabel = (val, city) =>
                                    city.Owner.TaxRate + "% " + Labels.For(LabelIndex.Tax) + ":" + val,
                                Icon = new BitmapStorage("ICONS", _resourceTransparentColor, 16, 320, 14)
                            },
                            new()
                            {
                                Name = "Lux",
                                GetResourceLabel = (val, city) =>
                                    city.Owner.LuxRate + "% " + Labels.For(LabelIndex.Lux) + ":" + val,
                                Icon = new BitmapStorage("ICONS", _resourceTransparentColor, 1, 320, 14)
                            },
                            new()
                            {
                                Name = "Science",
                                GetResourceLabel = (val, city) =>
                                    city.Owner.LuxRate + "% " + Labels.For(LabelIndex.Sci) + ":" + val,
                                Icon = new BitmapStorage("ICONS", _resourceTransparentColor, 31, 320, 14)
                            }
                        }
                    }
                }
            },
            UnitSupport = new UnitSupport
            {
                Position = new Rectangle(8,216,181,69),
                Rows = 2,
                Columns = 4
            }
        };

        _cityWindowLayout.Buttons.Add("Buy", new Rectangle(442, 181, buyButtonWidth, buttonHeight));
        _cityWindowLayout.Buttons.Add("Change", new Rectangle(557, 181, buyButtonWidth, buttonHeight));
        _cityWindowLayout.Buttons.Add("Info", new Rectangle(459, 364, infoButtonWidth, buttonHeight));
        _cityWindowLayout.Buttons.Add("Map", new Rectangle(517, 364, infoButtonWidth, buttonHeight));
        _cityWindowLayout.Buttons.Add("Rename", new Rectangle(575, 364, infoButtonWidth, buttonHeight));
        _cityWindowLayout.Buttons.Add("Happy", new Rectangle(459, 389, infoButtonWidth, buttonHeight));
        _cityWindowLayout.Buttons.Add("View", new Rectangle(517, 389, infoButtonWidth, buttonHeight));
        _cityWindowLayout.Buttons.Add("Exit", new Rectangle(575, 389, infoButtonWidth, buttonHeight));


        return _cityWindowLayout;
    }

    private static IList<Color> _resourceTransparentColor = new[]{ new Color(255, 159, 163, 255)};
    public IList<ResourceImage> ResourceImages { get; } = new List<ResourceImage>
    {
        new(name: "Food", 
            largeImage: new BitmapStorage("ICONS", _resourceTransparentColor, 1, 305, 14),
            smallImage: new BitmapStorage("ICONS", _resourceTransparentColor,49, 334, 10),
            lossImage: new BitmapStorage("ICONS", _resourceTransparentColor,1, 290, 14)),
        new(name: "Shields", 
            largeImage: new BitmapStorage("ICONS", _resourceTransparentColor,16, 305, 14),
            smallImage: new BitmapStorage("ICONS", _resourceTransparentColor,60, 334, 10),
            lossImage: new BitmapStorage("ICONS", _resourceTransparentColor,16, 290, 14)),
        new(name: "Trade", 
            largeImage: new BitmapStorage("ICONS",_resourceTransparentColor, 31, 305, 14),
            smallImage: new BitmapStorage("ICONS",_resourceTransparentColor, 71, 334, 10),
            lossImage: new BitmapStorage("ICONS",_resourceTransparentColor, 31, 290, 14))
    };

    public abstract int UnitsRows { get; }
    public abstract int UnitsPxHeight { get; }
    public abstract Dictionary<string, List<ImageProps>> UnitPICprops { get; set; }
    public abstract Dictionary<string, List<ImageProps>> CitiesPICprops { get; set; }
    public abstract Dictionary<string, List<ImageProps>> TilePICprops { get; set; }
    public abstract Dictionary<string, List<ImageProps>> OverlayPICprops { get; set; }
    public abstract Dictionary<string, List<ImageProps>> IconsPICprops { get; set; }
    public abstract string? GetFallbackPath(string root, int gameType);
    public abstract void GetShieldImages();
    public abstract UnitShield UnitShield(int unitType);
}

