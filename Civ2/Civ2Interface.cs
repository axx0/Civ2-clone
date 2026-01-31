using System.Text.RegularExpressions;
using Civ2.Dialogs;
using Civ2.Dialogs.NewGame;
using Civ2.Menu;
using Civ2.Rules;
using Civ2engine;
using Civ2engine.IO;
using Model;
using Model.Images;
using Model.ImageSets;
using Model.InterfaceActions;
using Model.Controls;
using Raylib_CSharp.Transformations;
using Raylib_CSharp.Images;
using Raylib_CSharp.Textures;
using Raylib_CSharp.Colors;
using Model.Constants;
using Model.Core;
using Model.Core.Advances;
using Model.Input;
using System.Numerics;

namespace Civ2;

public abstract class Civ2Interface(IMain main) : IUserInterface
{
    public bool CanDisplay(string? title)
    {
        return title != null && title.Contains(Title);
    }

    public abstract InterfaceStyle Look { get; }

    public abstract string Title { get; }

    public virtual void Initialize()
    {
        Dialogs = PopupBoxReader.LoadPopupBoxes(MainApp.ActiveRuleSet.Paths, "game.txt");
        // Add popups not in game.txt
        var extraPopups = new List<string> { "SCENCHOSECIV", "SCENINTRO", "SCENCUSTOMINTRO" };
        foreach (var popup in extraPopups)
        {
            Dialogs.Add(popup, new PopupBox());
        }
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

    public IImageSource? ScenTitleImage { get; set; } = null;
    
    public int GetCityIndexForStyle(int cityStyleIndex, City city, int citySize)
    {
        var index = cityStyleIndex switch
        {
            4 => citySize switch
            {
                <= 4 => 0,
                <= 7 => 1,
                <= 10 => 2,
                _ => 3
            },
            5 => citySize switch
            {
                <= 4 => 0,
                <= 10 => 1,
                <= 18 => 2,
                _ => 3
            },
            _ => citySize switch
            {
                <= 3 => 0,
                <= 5 => 1,
                <= 7 => 2,
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
    public List<TerrainSet> TileSets { get; } = [];

    public CityImageSet CityImages { get; } = new();

    public UnitSet UnitImages { get; } = new();

    public Dictionary<string, PopupBox> Dialogs { get; set; }
    public abstract void LoadPlayerColours();
    public PlayerColour[] PlayerColours { get; set; }
    public int ExpectedMaps { get; set; } = 1;
    public CommonMapImageSet MapImages { get; } = new();
    public int DefaultDialogWidth => 660; // 660=440*1.5

    public abstract Padding DialogPadding { get; }
    
    private CityWindowLayout? _cityWindowLayout;

    protected abstract List<MenuDetails> MenuMap { get; }

    public IList<DropdownMenuContents> ConfigureGameCommands(IList<IGameCommand> commands)
    {
        MenuLoader.LoadMenus(MainApp.ActiveRuleSet);

        var menus = new List<DropdownMenuContents>();
        
        var map = MenuMap;
        foreach (var menu in map)
        {
            // Find rows with separator and remove them
            List<int> separatorRows = [];
            for (var i = 0; i < menu.Defaults.Count; i++)
            {
                if (menu.Defaults[i].MenuText == "-")
                {
                    separatorRows.Add(i);
                }
            }
            for (var i = separatorRows.Count; i-- > 0;)
            {
                menu.Defaults.RemoveAt(separatorRows[i]);
                separatorRows[i] -= i + 2;
            }

            //separatorRows = new List<int> { 0, 1, 2, 3, 4, 5, 6 };
            var menuContent = new DropdownMenuContents { Commands = new List<MenuCommand>(), SeparatorRows = separatorRows.ToArray() };
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
            for (var i = 1; i < menu.Defaults.Count; i++)
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
                            baseCommand.MenuText.Replace(
                                "%STRING0", gameCommand.Name), Key.None,
                            gameCommand.ActivationKeys[0], gameCommand));
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

    public abstract ListboxLooks GetListboxLooks(ListboxType? type);

    public CityWindowLayout GetCityWindowDefinition()
    {
        if (_cityWindowLayout != null) return _cityWindowLayout;

        float buttonHeight = 24;

        const float buyButtonWidth = 68;
        const int infoButtonWidth = 57;

        _cityWindowLayout = new CityWindowLayout(new BitmapStorage("city", new Rectangle(0, 0, 640, 421)))
        {
            Height = 421, Width = 636,
            InfoPanel = new InfoPanel
            {
                Box = new Rectangle(193, 215, 242, 198),
                UnitsPresent = new UnitBox
                {
                    Box = new Rectangle(0, 0, 232, 84),
                    Rows = 2,
                    Columns = 5
                }
            },
            TileMap = new Rectangle(7, 65, 188, 137),
            FoodStorage = new Rectangle(437, 0, 195, 163),
            CitizensBox = new Rectangle(3, 2, 433, 44),
            Production = new()
            {
                Type = "Box",
                Box = new Rectangle(437, 165, 195, 191),
                IconLocation = new(97.5f, 18)
            },
            Improvements = new ImprovementsBox()
            {
                Box = new Rectangle(5, 306, 170, 108),
                Rows = 9,
                LabelColor = Color.White,
                LabelColorShadow = Color.Black,
                ShadowOffset = new Vector2(1, 0)
            },
            Resources = new ResourceProduction
            {
                TitlePosition = new Rectangle(199, 46, 238, 15),
                Resources = new List<ResourceArea>
                {
                    new ConsumableResourceArea(name: "Food",
                        bounds: new Rectangle(203, 75, 230, 13),
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
                        Resources =
                        [
                            new()
                            {
                                Name = "Tax",
                                GetResourceLabel = (val, city) =>
                                    city.Owner.TaxRate + "% " + Labels.For(LabelIndex.Tax) + ":" + val,
                                Icon = new BitmapStorage("ICONS", 16, 320, 14)
                            },

                            new()
                            {
                                Name = "Lux",
                                GetResourceLabel = (val, city) =>
                                    city.Owner.LuxRate + "% " + Labels.For(LabelIndex.Lux) + ":" + val,
                                Icon = new BitmapStorage("ICONS", 1, 320, 14)
                            },

                            new()
                            {
                                Name = "Science",
                                GetResourceLabel = (val, city) =>
                                    city.Owner.LuxRate + "% " + Labels.For(LabelIndex.Sci) + ":" + val,
                                Icon = new BitmapStorage("ICONS", 31, 320, 14)
                            }
                        ]
                    }
                }
            },
            UnitSupport = new UnitBox
            {
                Box = new Rectangle(7, 215, 184, 69),
                Rows = 2,
                Columns = 4
            },
        };

        _cityWindowLayout.Buttons.Add("Buy", new(Labels.For(LabelIndex.Buy), new(5, 16, buyButtonWidth, buttonHeight)));
        _cityWindowLayout.Buttons.Add("Change", new(Labels.For(LabelIndex.Change), new(120, 16, buyButtonWidth, buttonHeight)));
        _cityWindowLayout.Buttons.Add("Info", new(Labels.For(LabelIndex.Info), new(459, 364, infoButtonWidth, buttonHeight)));
        _cityWindowLayout.Buttons.Add("Map", new(Labels.For(LabelIndex.Map), new(517, 364, infoButtonWidth, buttonHeight)));
        _cityWindowLayout.Buttons.Add("Rename", new(Labels.For(LabelIndex.Rename), new(575, 364, infoButtonWidth, buttonHeight)));
        _cityWindowLayout.Buttons.Add("Happy", new(Labels.For(LabelIndex.Happy), new(459, 389, infoButtonWidth, buttonHeight)));
        _cityWindowLayout.Buttons.Add("View", new(Labels.For(LabelIndex.View), new(517, 389, infoButtonWidth, buttonHeight)));
        _cityWindowLayout.Buttons.Add("Exit", new("Exit", new(575, 389, infoButtonWidth, buttonHeight)));

        _cityWindowLayout.Labels.Add("FoodStorage", new(Labels.For(LabelIndex.FoodStorage), new(437, 0, 195, 12), new Color(75, 155, 35, 255), Color.Black));
        _cityWindowLayout.Labels.Add("CityImprovements", new(Labels.For(LabelIndex.CityImprovements), new(3, 291, 189, 12), new Color(223, 187, 63, 255), new Color(67, 67, 67, 255)));
        _cityWindowLayout.Labels.Add("UnitsPresent", new(Labels.For(LabelIndex.UnitsPresent), new(0, 0, 232, 12), new Color(223, 187, 63, 255), new Color(67, 67, 67, 255)));
        _cityWindowLayout.Labels.Add("UnitsSupported", new(Labels.For(LabelIndex.UnitsSupported), new(3, 215, 189, 12), new Color(223, 187, 63, 255), new Color(67, 67, 67, 255)));
        _cityWindowLayout.Labels.Add("ItemInProduction", new("", new(0, 4, 195, 12), new Color(63, 79, 167, 255), Color.Black));
        _cityWindowLayout.Labels.Add("Supplies", new(Labels.For(LabelIndex.Supplies), new(0, 130, 232, 12), new Color(227, 83, 15, 255), new Color(67, 67, 67, 255)));
        _cityWindowLayout.Labels.Add("Demands", new(Labels.For(LabelIndex.Demands), new(0, 143, 232, 12), new Color(227, 83, 15, 255), new Color(67, 67, 67, 255)));
        _cityWindowLayout.Labels.Add("ResourceMap", new(Labels.For(LabelIndex.ResourceMap), new(0, 125, 189, 12), new Color(223, 187, 63, 255), new Color(0, 51, 0, 255)));
        _cityWindowLayout.Labels.Add("Citizens", new(Labels.For(LabelIndex.Citizens), new(0, 46, 189, 12), new Color(223, 187, 63, 255), new Color(67, 67, 67, 255)));


        return _cityWindowLayout;
    }

    public IList<ResourceImage> ResourceImages { get; } = new List<ResourceImage>
    {
        new(name: "Food", 
            largeImage: new BitmapStorage("ICONS", 1, 305, 14),
            smallImage: new BitmapStorage("ICONS",49, 334, 10),
            lossImage: new BitmapStorage("ICONS",1, 290, 14)),
        new(name: "Shields", 
            largeImage: new BitmapStorage("ICONS", 16, 305, 14),
            smallImage: new BitmapStorage("ICONS", 60, 334, 10),
            lossImage: new BitmapStorage("ICONS", 16, 290, 14)),
        new(name: "Trade", 
            largeImage: new BitmapStorage("ICONS", 31, 305, 14),
            smallImage: new BitmapStorage("ICONS", 71, 334, 10),
            lossImage: new BitmapStorage("ICONS", 31, 290, 14))
    };

    public PopupBox? GetDialog(string dialogName)
    {
        return Dialogs.GetValueOrDefault(dialogName);
    }

    public abstract int UnitsRows { get; }
    public abstract int UnitsPxHeight { get; }
    public abstract Dictionary<string, IImageSource[]> PicSources { get; }
    public abstract void GetShieldImages();
    public abstract UnitShield UnitShield(int unitType);
    public abstract void DrawBorderWallpaper(Wallpaper wallpaper, ref Image destination, int height, int width, Padding padding, bool statusPanel);
    public abstract void DrawBorderLines(ref Image destination, int height, int width, Padding padding, bool statusPanel);
    public abstract void DrawButton(Texture2D texture, Rectangle bounds);
    public IList<Ruleset> FindRuleSets(string[] searchPaths)
    {
        var sets = new List<Ruleset>();
        foreach (var path in searchPaths)
        {
            var gameTxt = Path.Combine(path, "Game.txt");
            if (!File.Exists(gameTxt)) continue;
            
            var title = File.ReadLines(gameTxt)
                .Where(l => l.StartsWith("@title"))
                .Select(l => l.Split("=", 2)[1])
                .FirstOrDefault();
            if (title != null && CanDisplay(title))
            {
                sets.AddRange(GenerateRulesets(path, title));
            }   
        }

        return sets;
    }

    public IMain MainApp { get; } = main;

    protected abstract IEnumerable<Ruleset> GenerateRulesets(string path, string title);
    
    public abstract Padding GetPadding(float headerLabelHeight, bool footer);
    public abstract bool IsButtonInOuterPanel { get; }
    
    public int InterfaceIndex { get; set; }

    public IInterfaceAction HandleLoadScenario(IGame game, string scnName, string scnDirectory)
    {
        ExpectedMaps = game.NoMaps;
        Initialization.LoadGraphicsAssets(this);

        var config = Initialization.ConfigObject;
        config.TechParadigm = game.ScenarioData.TechParadigm;
        config.ScenarioName = game.ScenarioData.Name;
        config.CivNames = game.AllCivilizations.Select(c => c.TribeName).ToArray();
        config.CivGenders = game.AllCivilizations.Select(c => c.LeaderGender).ToArray();
        config.LeaderNames = game.AllCivilizations.Select(c => c.LeaderName).ToArray();
        config.StartingYear = game.ScenarioData.StartingYear;
        config.TurnYearIncrement = game.ScenarioData.TurnYearIncrement;
        config.DifficultyLevel = game.DifficultyLevel;
        config.MaxTurns = game.ScenarioData.MaxTurns;
        config.CivsInPlay = game.AllCivilizations.Select(c => c.Alive).ToArray(); ;

        Initialization.Start(game);

        var titleImage = "Title.gif";
        var foundTitleImage = Directory.EnumerateFiles(scnDirectory, titleImage, new EnumerationOptions { MatchCasing = MatchCasing.CaseInsensitive }).FirstOrDefault();
        if (foundTitleImage != null)
        {
            ScenTitleImage = new BitmapStorage(foundTitleImage);
        }

        // Load custom intro if it exists in txt file
        var introFile = Regex.Replace(scnName, ".scn", ".txt", RegexOptions.IgnoreCase);
        var foundIntroFile = Directory.EnumerateFiles(scnDirectory, introFile, new EnumerationOptions { MatchCasing = MatchCasing.CaseInsensitive }).FirstOrDefault();
        if (foundIntroFile != null)
        {
            var boxes = new Dictionary<string, PopupBox>();
            TextFileParser.ParseFile(Path.Combine(scnDirectory, foundIntroFile), new PopupBoxReader (boxes), true);
            if (boxes.TryGetValue("SCENARIO", out var dialogInfo))
            {
                DialogHandlers[ScenCustomIntro.Title].UpdatePopupData(new()
                {
                    { ScenCustomIntro.Title, dialogInfo }
                });

                return DialogHandlers[ScenCustomIntro.Title].Show(this);
            }
        }

        // Load default intro
        return DialogHandlers[ScenarioLoadedDialog.Title].Show(this);
    }

    public IInterfaceAction HandleLoadGame(IGame game, Civ2engine.Rules rules, Ruleset ruleset,
        Dictionary<string, string?> viewData)
    {
        ExpectedMaps = game.NoMaps;
        Initialization.LoadGraphicsAssets(this);
        Initialization.ViewData = viewData;
        Initialization.Start(game);
        return DialogHandlers[LoadOk.Title].Show(this);
    }

    public IInterfaceAction InitNewGame(bool quickStart)
    {
        Initialization.LoadGraphicsAssets(this);
        
        if (quickStart)
        {
            Initialization.ConfigObject.QuickStart = true;
            Initialization.ConfigObject.WorldSize = [50, 80];
            Initialization.ConfigObject.NumberOfCivs = this.PlayerColours.Length - 1;
            Initialization.ConfigObject.BarbarianActivity = Initialization.ConfigObject.Random.Next(5);
            
            Initialization.ConfigObject.MapTask = MapGenerator.GenerateMap(Initialization.ConfigObject);
            return DialogHandlers[DifficultyHandler.Title].Show(this);
        }

        return DialogHandlers[WorldSizeHandler.Title].Show(this);
    }

    public IImageSource? GetImprovementImage(Improvement improvement, int firstWonderIndex)
    {
        var y = 1;
        var x = 343;
        var index = improvement.Type;
        var columns = 8;
        if (improvement.IsWonder)
        {
            y += 105;
            index -= firstWonderIndex;
            columns = 7;
        }
        else
        {
            index -= 1; //Remove nothing as it has no image
        }

        var (addRows, addColumns) = Math.DivRem(index, columns);

        y += addRows * 21;
        x += addColumns * 37;
        
        return new BitmapStorage("icons", x, y, 36, 20);
    }

    public IImageSource? GetAdvanceImage(Advance advance)
    {
        var x = 343 + (advance.KnowledgeCategory) * 37;
        var y = 211 + (advance.Epoch) * 21;
        
        return new BitmapStorage("icons", x, y, 36, 20);
    }

    public string GetScientistName(int epoch)
    {
        return Labels.For(epoch < 3 ? LabelIndex.wisemen : LabelIndex.scientists);
    }
}