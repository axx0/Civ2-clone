using Civ2.Dialogs;
using Civ2.Rules;
using Civ2engine;
using Civ2engine.Improvements;
using Civ2engine.IO;
using Model;
using Model.Images;
using Model.ImageSets;
using Model.InterfaceActions;
using Raylib_cs;
using RaylibUI;

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

    public IInterfaceAction GetInitialAction()
    {
        return DialogHandlers["MAINMENU"].Show(this);
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
    public int ExpectedMaps { get; set; } = 1; //TODO: extract to specific locations because TOT has four 
    public CommonMapImageSet MapImages { get; } = new();
    public int DefaultDialogWidth => 660; // 660=440*1.5
    public IList<string> GetMenuItems()
    {
        MenuLoader.LoadMenus(Initialization.ConfigObject.RuleSet);

        return MenuLoader.Menus;
    }
}

