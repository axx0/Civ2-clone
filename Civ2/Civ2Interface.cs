using System.Collections.Specialized;
using System.Numerics;
using Civ2.Dialogs;
using Civ2.Menu;
using Civ2.Rules;
using Civ2engine;
using Civ2engine.Improvements;
using Civ2engine.IO;
using Civ2engine.Units;
using Model;
using Model.Images;
using Model.ImageSets;
using Model.InterfaceActions;
using Model.Menu;
using Raylib_cs;
using RaylibUI;
using RayLibUtils;

using static Model.Menu.CommandIds;

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
    
    public int GetCityIndexForStyle(int cityStyleIndex, City city, int citySize)
    {
        var index = cityStyleIndex switch
        {
            4 => citySize switch
            {
                <= 4 => 0,
                > 4 and <= 7 => 1,
                > 7 and <= 10 => 2,
                _ => 3
            },
            5 => citySize switch
            {
                <= 4 => 0,
                > 4 and <= 10 => 1,
                > 10 and <= 18 => 2,
                _ => 3
            },
            _ => citySize switch
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
    
    public Dictionary<string, PopupBox?> Dialogs { get; set; }
    public abstract void LoadPlayerColours();
    public PlayerColour[] PlayerColours { get; set; }
    public int ExpectedMaps { get; set; } = 1;
    public CommonMapImageSet MapImages { get; } = new();
    public int DefaultDialogWidth => 660; // 660=440*1.5

    public Padding DialogPadding { get; } = new(11);

    
    private CityWindowLayout? _cityWindowLayout;

    protected List<MenuDetails> MenuMap { get; } = new List<MenuDetails>
    {
        new MenuDetails
        {
            Key = "GAME", Defaults = new List<MenuElement>
            {
                new MenuElement("&Game", Shortcut.None, KeyboardKey.KEY_G),
                new MenuElement("Game &Options|Ctrl+O", new Shortcut(KeyboardKey.KEY_O, ctrl: true), KeyboardKey.KEY_O),
                new MenuElement("Graphic O&ptions|Ctrl+P", new Shortcut(KeyboardKey.KEY_P, ctrl: true),
                    KeyboardKey.KEY_P),
                new MenuElement("&City Report Options|Ctrl+E", new Shortcut(KeyboardKey.KEY_E, ctrl: true),
                    KeyboardKey.KEY_C),
                new MenuElement("M&ultiplayer Options|Ctrl+Y", new Shortcut(KeyboardKey.KEY_Y, ctrl: true),
                    KeyboardKey.KEY_U),
                new MenuElement("&Game Profile", Shortcut.None, KeyboardKey.KEY_G),
                new MenuElement("Pick &Music", Shortcut.None, KeyboardKey.KEY_M),
                new MenuElement("&Save Game|Ctrl+S", new Shortcut(KeyboardKey.KEY_S, ctrl: true), KeyboardKey.KEY_S),
                new MenuElement("&Load Game|Ctrl+L", new Shortcut(KeyboardKey.KEY_L, ctrl: true), KeyboardKey.KEY_L),
                new MenuElement("&Join Game|Ctrl+J", new Shortcut(KeyboardKey.KEY_J, ctrl: true), KeyboardKey.KEY_J),
                new MenuElement("Set Pass&word|Ctrl+W", new Shortcut(KeyboardKey.KEY_W, ctrl: true), KeyboardKey.KEY_W),
                new MenuElement("Change &Timer|Ctrl+T", new Shortcut(KeyboardKey.KEY_T, ctrl: true), KeyboardKey.KEY_T),
                new MenuElement("&Retire|Ctrl+R", new Shortcut(KeyboardKey.KEY_R, ctrl: true), KeyboardKey.KEY_R),
                new MenuElement("&Quit|Ctrl+Q", new Shortcut(KeyboardKey.KEY_Q, ctrl: true), KeyboardKey.KEY_Q)
            }
        },
        new MenuDetails
        {
            Key = "KINGDOM", Defaults = new List<MenuElement>
            {
                new MenuElement("&Kingdom", Shortcut.None, KeyboardKey.KEY_K),
                new MenuElement("&Tax Rate|Shift+T", new Shortcut(KeyboardKey.KEY_T, shift: true), KeyboardKey.KEY_T),
                new MenuElement("View T&hrone Room|Shift+H", new Shortcut(KeyboardKey.KEY_H, shift: true),
                    KeyboardKey.KEY_H),
                new MenuElement("Find &City|Shift+C", new Shortcut(KeyboardKey.KEY_C, shift: true), KeyboardKey.KEY_C),
                new MenuElement("&REVOLUTION|Shift+R", new Shortcut(KeyboardKey.KEY_R, shift: true), KeyboardKey.KEY_R)
            }
        },

        new MenuDetails
        {
            Key = "VIEW", Defaults = new List<MenuElement>
            {
                new MenuElement("&View", Shortcut.None, KeyboardKey.KEY_V),
                new MenuElement("&Move Pieces|v", new Shortcut(KeyboardKey.KEY_V), KeyboardKey.KEY_M),
                new MenuElement("&View Pieces|v", new Shortcut(KeyboardKey.KEY_V), KeyboardKey.KEY_V),
                new MenuElement("Zoom &In|z", new Shortcut(KeyboardKey.KEY_Z), KeyboardKey.KEY_I),
                new MenuElement("Zoom &Out|x", new Shortcut(KeyboardKey.KEY_X), KeyboardKey.KEY_O),
                new MenuElement("Max Zoom In|Ctrl+Z", new Shortcut(KeyboardKey.KEY_Z, ctrl: true),
                    KeyboardKey.KEY_NULL),
                new MenuElement("Standard Zoom|Shift+Z", new Shortcut(KeyboardKey.KEY_Z, shift: true),
                    KeyboardKey.KEY_NULL),
                new MenuElement("Medium Zoom Out|Shift+X", new Shortcut(KeyboardKey.KEY_X, shift: true),
                    KeyboardKey.KEY_NULL),
                new MenuElement("Max Zoom Out|Ctrl+X", new Shortcut(KeyboardKey.KEY_X, ctrl: true),
                    KeyboardKey.KEY_NULL),
                new MenuElement("Show Map Grid|Ctrl+G", new Shortcut(KeyboardKey.KEY_G, ctrl: true),
                    KeyboardKey.KEY_NULL),
                new MenuElement("Arrange Windows", Shortcut.None, KeyboardKey.KEY_NULL),
                new MenuElement("Show Hidden Terrain|t", new Shortcut(KeyboardKey.KEY_T), KeyboardKey.KEY_T),
                new MenuElement("&Center View|c", new Shortcut(KeyboardKey.KEY_C), KeyboardKey.KEY_C)
            }
        },

        new MenuDetails
        {
            Key = "@ORDERS", Defaults = new List<MenuElement>
            {
                new MenuElement("&Orders", Shortcut.None, KeyboardKey.KEY_O),
                new MenuElement("&Build New City|b", new Shortcut(KeyboardKey.KEY_B), KeyboardKey.KEY_B, BuildCityOrder, true),
                new MenuElement("Build &Road|r", new Shortcut(KeyboardKey.KEY_R), KeyboardKey.KEY_R, BuildRoadOrder, omitIfNoCommand: true),
                new MenuElement("Build &Irrigation|i", new Shortcut(KeyboardKey.KEY_I), KeyboardKey.KEY_I,BuildIrrigationOrder, omitIfNoCommand:true),
                new MenuElement("Build &Mines|m", new Shortcut(KeyboardKey.KEY_M), KeyboardKey.KEY_M,BuildMineOrder, omitIfNoCommand: true),
                new MenuElement("Build %STRING0", Shortcut.None, KeyboardKey.KEY_NULL, BuildIrrigationOrder, omitIfNoCommand:true),
                new MenuElement("Transform to ...|o", new Shortcut(KeyboardKey.KEY_O), KeyboardKey.KEY_T),
                new MenuElement("Build &Airbase|e", new Shortcut(KeyboardKey.KEY_E), KeyboardKey.KEY_A),
                new MenuElement("Build &Fortress|f", new Shortcut(KeyboardKey.KEY_F), KeyboardKey.KEY_F),
                new MenuElement("Automate Settler|k", new Shortcut(KeyboardKey.KEY_K), KeyboardKey.KEY_NULL),
                new MenuElement("Clean Up &Pollution|p", new Shortcut(KeyboardKey.KEY_P), KeyboardKey.KEY_P),
                new MenuElement("&Pillage|Shift+P", new Shortcut(KeyboardKey.KEY_P, shift: true), KeyboardKey.KEY_P),
                new MenuElement("&Unload|u", new Shortcut(KeyboardKey.KEY_U), KeyboardKey.KEY_U),
                new MenuElement("&Go To|g", new Shortcut(KeyboardKey.KEY_G), KeyboardKey.KEY_G),
                new MenuElement("&Paradrop|p", new Shortcut(KeyboardKey.KEY_P), KeyboardKey.KEY_P),
                new MenuElement("Air&lift|l", new Shortcut(KeyboardKey.KEY_L), KeyboardKey.KEY_L),
                new MenuElement("Set &Home City|h", new Shortcut(KeyboardKey.KEY_H), KeyboardKey.KEY_H),
                new MenuElement("&Fortify|f", new Shortcut(KeyboardKey.KEY_F), KeyboardKey.KEY_F),
                new MenuElement("&Sleep|s", new Shortcut(KeyboardKey.KEY_S), KeyboardKey.KEY_S),
                new MenuElement("&Disband|Shift+D", new Shortcut(KeyboardKey.KEY_D, shift: true), KeyboardKey.KEY_D),
                new MenuElement("&Activate Unit|a", new Shortcut(KeyboardKey.KEY_A), KeyboardKey.KEY_A),
                new MenuElement("&Wait|w", new Shortcut(KeyboardKey.KEY_W), KeyboardKey.KEY_W),
                new MenuElement("S&kip Turn|SPACE", new Shortcut(KeyboardKey.KEY_SPACE), KeyboardKey.KEY_K),
                new MenuElement("End Player Tur&n|Ctrl+N", new Shortcut(KeyboardKey.KEY_T, shift: true),
                    KeyboardKey.KEY_N, EndTurn)
            },
        },

        new MenuDetails
        {
            Key = "ADVISORS", Defaults = new List<MenuElement>
            {
                new MenuElement("&Advisors", Shortcut.None, KeyboardKey.KEY_A),
                new MenuElement("Chat with &Kings|Ctrl+C", new Shortcut(KeyboardKey.KEY_C, ctrl: true),
                    KeyboardKey.KEY_K),
                new MenuElement("Consult &High Council", Shortcut.None, KeyboardKey.KEY_H),
                new MenuElement("&City Status|F1", new Shortcut(KeyboardKey.KEY_F1), KeyboardKey.KEY_C),
                new MenuElement("&Defense Minister|F2", new Shortcut(KeyboardKey.KEY_F2), KeyboardKey.KEY_D),
                new MenuElement("&Foreign Minister|F3", new Shortcut(KeyboardKey.KEY_F3), KeyboardKey.KEY_F),
                new MenuElement("&Attitude Advisor|F4", new Shortcut(KeyboardKey.KEY_F4), KeyboardKey.KEY_A),
                new MenuElement("&Trade Advisor|F5", new Shortcut(KeyboardKey.KEY_F5), KeyboardKey.KEY_T),
                new MenuElement("&Science Advisor|F6", new Shortcut(KeyboardKey.KEY_F6), KeyboardKey.KEY_S),
                new MenuElement("Cas&ualty Timeline|Ctrl-D", new Shortcut(KeyboardKey.KEY_D, ctrl: true),
                    KeyboardKey.KEY_U)
            }
        },

        new MenuDetails
        {
            Key = "WORLD", Defaults = new List<MenuElement>
            {
                new MenuElement("&World", Shortcut.None, KeyboardKey.KEY_W),
                new MenuElement("&Wonders of the World|F7", new Shortcut(KeyboardKey.KEY_F7), KeyboardKey.KEY_W),
                new MenuElement("&Top 5 Cities|F8", new Shortcut(KeyboardKey.KEY_F8), KeyboardKey.KEY_T),
                new MenuElement("&Civilization Score|F9", new Shortcut(KeyboardKey.KEY_F9), KeyboardKey.KEY_C),
                new MenuElement("&Demographics|F11", new Shortcut(KeyboardKey.KEY_F11), KeyboardKey.KEY_D),
                new MenuElement("&Spaceships|F12", new Shortcut(KeyboardKey.KEY_F12), KeyboardKey.KEY_S)
            }
        },

        new MenuDetails
        {
            Key = "CHEAT", Defaults = new List<MenuElement>
            {
                new MenuElement("&Cheat", Shortcut.None, KeyboardKey.KEY_C),
                new MenuElement("Toggle Cheat Mode|Ctrl+K", new Shortcut(KeyboardKey.KEY_K, ctrl: true),
                    KeyboardKey.KEY_NULL),
                new MenuElement("Create &Unit|Shift+F1", new Shortcut(KeyboardKey.KEY_F1, shift: true),
                    KeyboardKey.KEY_U),
                new MenuElement("Reveal &Map|Shift+F2", new Shortcut(KeyboardKey.KEY_F2, shift: true),
                    KeyboardKey.KEY_M),
                new MenuElement("Set &Human Player|Shift+F3", new Shortcut(KeyboardKey.KEY_F3, shift: true),
                    KeyboardKey.KEY_H),
                new MenuElement("Set Game Year|Shift+F4", new Shortcut(KeyboardKey.KEY_F4, shift: true),
                    KeyboardKey.KEY_NULL),
                new MenuElement("&Kill Civilization|Shift+F5", new Shortcut(KeyboardKey.KEY_F5, shift: true),
                    KeyboardKey.KEY_K),
                new MenuElement("Te&chnology Advance|Shift+F6", new Shortcut(KeyboardKey.KEY_F6, shift: true),
                    KeyboardKey.KEY_C),
                new MenuElement("&Edit Technologies|Ctrl+Shift+F6",
                    new Shortcut(KeyboardKey.KEY_F6, ctrl: true, shift: true), KeyboardKey.KEY_E),
                new MenuElement("Force &Government|Shift+F7", new Shortcut(KeyboardKey.KEY_F7, shift: true),
                    KeyboardKey.KEY_G),
                new MenuElement("Change &Terrain At Cursor|Shift+F8", new Shortcut(KeyboardKey.KEY_F8, shift: true),
                    KeyboardKey.KEY_T),
                new MenuElement("Destro&y All Units At Cursor|Ctrl+Shift+D",
                    new Shortcut(KeyboardKey.KEY_D, ctrl: true, shift: true), KeyboardKey.KEY_Y),
                new MenuElement("Change Money|Shift+F9", new Shortcut(KeyboardKey.KEY_F9, shift: true),
                    KeyboardKey.KEY_NULL),
                new MenuElement("Edit Unit|Ctrl+Shift+U", new Shortcut(KeyboardKey.KEY_U, ctrl: true, shift: true),
                    KeyboardKey.KEY_NULL),
                new MenuElement("Edit City|Ctrl+Shift+C", new Shortcut(KeyboardKey.KEY_C, ctrl: true, shift: true),
                    KeyboardKey.KEY_NULL),
                new MenuElement("Edit King|Ctrl+Shift+K", new Shortcut(KeyboardKey.KEY_K, ctrl: true, shift: true),
                    KeyboardKey.KEY_NULL),
                new MenuElement("Scenario Parameters|Ctrl+Shift+P",
                    new Shortcut(KeyboardKey.KEY_P, ctrl: true, shift: true), KeyboardKey.KEY_NULL),
                new MenuElement("Save As Scenario|Ctrl+Shift+S",
                    new Shortcut(KeyboardKey.KEY_S, ctrl: true, shift: true), KeyboardKey.KEY_NULL)
            }
        },

        new MenuDetails
        {
            Key = "EDITOR", Defaults = new List<MenuElement>
            {
                new MenuElement("&Editor", Shortcut.None, KeyboardKey.KEY_E),
                new MenuElement("Toggle &Scenario Flag|Ctrl+F", new Shortcut(KeyboardKey.KEY_F, ctrl: true),
                    KeyboardKey.KEY_S),
                new MenuElement("&Advances Editor|Ctrl+Shift+1",
                    new Shortcut(KeyboardKey.KEY_ONE, ctrl: true, shift: true), KeyboardKey.KEY_A),
                new MenuElement("&Cities Editor|Ctrl+Shift+2",
                    new Shortcut(KeyboardKey.KEY_TWO, ctrl: true, shift: true), KeyboardKey.KEY_C),
                new MenuElement("E&ffects Editor|Ctrl+Shift+3",
                    new Shortcut(KeyboardKey.KEY_THREE, ctrl: true, shift: true), KeyboardKey.KEY_F),
                new MenuElement("&Improvements Editor|Ctrl+Shift+4",
                    new Shortcut(KeyboardKey.KEY_FOUR, ctrl: true, shift: true), KeyboardKey.KEY_I),
                new MenuElement("&Terrain Editor|Ctrl+Shift+5",
                    new Shortcut(KeyboardKey.KEY_FIVE, ctrl: true, shift: true), KeyboardKey.KEY_T),
                new MenuElement("T&ribe Editor|Ctrl+Shift+6",
                    new Shortcut(KeyboardKey.KEY_SIX, ctrl: true, shift: true), KeyboardKey.KEY_R),
                new MenuElement("&Units Editor|Ctrl+Shift+7",
                    new Shortcut(KeyboardKey.KEY_SEVEN, ctrl: true, shift: true), KeyboardKey.KEY_U),
                new MenuElement("&Events Editor|Ctrl+Shift+8",
                    new Shortcut(KeyboardKey.KEY_EIGHT, ctrl: true, shift: true), KeyboardKey.KEY_E)
            }
        },

        new MenuDetails
        {
            Key = "PEDIA", Defaults = new List<MenuElement>
            {
                new MenuElement("&Civilopedia", Shortcut.None, KeyboardKey.KEY_C),
                new MenuElement("Civilization &Advances", Shortcut.None, KeyboardKey.KEY_A),
                new MenuElement("City &Improvements", Shortcut.None, KeyboardKey.KEY_I),
                new MenuElement("&Wonders of the World", Shortcut.None, KeyboardKey.KEY_W),
                new MenuElement("Military &Units", Shortcut.None, KeyboardKey.KEY_U),
                new MenuElement("&Governments", Shortcut.None, KeyboardKey.KEY_G),
                new MenuElement("&Terrain Types", Shortcut.None, KeyboardKey.KEY_T),
                new MenuElement("Game &Concepts", Shortcut.None, KeyboardKey.KEY_C),
                new MenuElement("&About Civilization II", Shortcut.None, KeyboardKey.KEY_A)
            }
        }
    };

    public IList<DropdownMenuContents> ConfigureGameCommands(IList<IGameCommand> commands)
    {
        MenuLoader.LoadMenus(Initialization.ConfigObject.RuleSet);

        var menus = new List<DropdownMenuContents>();
        
        var map = MenuMap;
        foreach (var menu in map)
        {
            var menuContent = new DropdownMenuContents { Commands = new List<MenuCommand>()};
            var loaded = MenuLoader.For(menu.Key);
            if (loaded.Count > 0)
            {
                menuContent.Title = loaded[0].MenuText;
                menuContent.HotKey = loaded[0].Hotkey;
            }
            else
            {
                menuContent.Title = menu.Defaults[0].MenuText;
                menuContent.HotKey = menu.Defaults[0].Hotkey;
            }

            var loadIndex = 0;
            for (int i = 1; i < menu.Defaults.Count; i++)
            {
                var baseCommand = menu.Defaults[i];
                var content = loaded.Count > i ? loaded[i] : baseCommand;

                if (baseCommand.Repeat)
                {
                    var comandsList = commands.Where(c =>
                        c.Id.StartsWith(baseCommand.CommandId) && menu.Defaults.All(d => d.CommandId != c.Id));

                    foreach (var gameCommand in comandsList)
                    {
                        menuContent.Commands.Add(new MenuCommand(
                            baseCommand.MenuText.Replace("%STRING0", gameCommand.Name), KeyboardKey.KEY_NULL,
                            gameCommand.KeyCombo, gameCommand));
                    }
                    continue;
                }
                
                var command = commands.FirstOrDefault(c => c.Id == baseCommand.CommandId);
                if (command == null && baseCommand.OmitIfNoCommand)
                {
                    continue;
                }
                
                var menuCommand = new MenuCommand(content.MenuText, content.Hotkey, content.Shortcut, command);
                menuContent.Commands.Add(menuCommand);
            }
            menus.Add(menuContent);
        }

        return menus;
    }

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

    public PopupBox? GetDialog(string dialogName)
    {
        return Dialogs.GetValueOrDefault(dialogName);
    }

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