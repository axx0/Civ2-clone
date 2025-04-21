using System.Numerics;
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
using Model.Menu;
using Raylib_CSharp.Interact;
using Raylib_CSharp.Transformations;
using Raylib_CSharp.Images;
using Raylib_CSharp.Textures;
using RaylibUtils;
using static Model.Menu.CommandIds;
using Civ2.Dialogs.Scenario;
using Civ2engine.OriginalSaves;
using Model.Constants;
using Model.Core;
using Model.Core.Advances;
using Model.Dialog;

namespace Civ2;

public abstract class Civ2Interface : IUserInterface
{
    protected Civ2Interface(IMain main)
    {
        MainApp = main;
    }
    
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
            Dictionary<string, PopupBox?> popup2 = new() { { popup, new PopupBox() } };
            Dialogs.Add(popup2.Keys.First(), popup2.Values.First());
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
            List<int> separatorRows = new();
            for (int i = 0; i < menu.Defaults.Count; i++)
            {
                if (menu.Defaults[i].MenuText == "-")
                {
                    separatorRows.Add(i);
                }
            }
            for (int i = separatorRows.Count; i-- > 0;)
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
                            baseCommand.MenuText.Replace(
                                "%STRING0", gameCommand.Name), KeyboardKey.Null,
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
    public abstract Dictionary<string, IImageSource[]> PicSources { get; set; }
    public abstract void GetShieldImages();
    public abstract UnitShield UnitShield(int unitType);
    public abstract void DrawBorderWallpaper(Wallpaper wallpaper, ref Image destination, int height, int width, Padding padding, bool statusPanel);
    public abstract void DrawBorderLines(ref Image destination, int height, int width, Padding padding, bool statusPanel);
    public abstract void DrawButton(Texture2D texture, int x, int y, int w, int h);
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

    public IMain MainApp { get; }

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
            var boxes = new Dictionary<string, PopupBox?>();
            TextFileParser.ParseFile(Path.Combine(scnDirectory, foundIntroFile), new PopupBoxReader { Boxes = boxes }, true);
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
            Initialization.ConfigObject.WorldSize = new[] { 50, 80 };
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