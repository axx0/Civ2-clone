using System;
using System.Numerics;
using Civ2;
using Civ2engine;
using JetBrains.Annotations;
using Model;
using Model.Images;
using Model.ImageSets;
using Model.Interface;
using Model.Menu;
using Raylib_cs;
using RayLibUtils;
using static Model.Menu.CommandIds;

namespace Civ2Gold;

[UsedImplicitly]
public class Civ2GoldInterface : Civ2Interface
{
    public override string Title => "Civilization II Multiplayer Gold";

    public override string InitialMenu => "MAINMENU";

    public override InterfaceStyle Look { get; } = new()
    {
        Outer = new BitmapStorage("ICONS", new Rectangle(199, 322, 64, 32)),
        Inner = new BitmapStorage("ICONS", new Rectangle(298, 190, 32, 32)),

        RadioButtons = new IImageSource[]
        { new BitmapStorage("buttons.png", 0, 0, 32), new BitmapStorage("buttons.png", 32, 0, 32) },
        CheckBoxes = new IImageSource[]
        { new BitmapStorage("buttons.png", 0, 32, 32), new BitmapStorage("buttons.png", 32, 32, 32) },
        
        DefaultFont = Fonts.TNR,
        ButtonFont = Fonts.TNR,
        ButtonFontSize = 20,
        ButtonColour = Color.BLACK,
        HeaderLabelFont = Fonts.TNRbold,
        HeaderLabelFontSizeNormal = 28,
        HeaderLabelFontSizeLarge = 34,
        CityHeaderLabelFontSizeNormal = 18,
        CityHeaderLabelFontSizeLarge = 28,
        CityHeaderLabelFontSizeSmall = 16,
        HeaderLabelShadow = true,
        HeaderLabelColour = new Color(135, 135, 135, 255),
        LabelFont = Fonts.TNR,
        LabelColour = Color.BLACK,
        CityWindowFont = Fonts.Arial,
        CityWindowFontSize = 16,
        MenuFont = Fonts.Arial,
        MenuFontSize = 14
    };

    public override bool IsButtonInOuterPanel => true;

    public override Padding GetPadding(float headerLabelHeight, bool footer)
    {
        int paddingTop = headerLabelHeight != 0 ? 7 + Math.Max((int)(-3 + 2 / 9 * headerLabelHeight + headerLabelHeight), (int)headerLabelHeight) : 11;
        int paddtingBtm = footer ? 46 : 10;

        return new Padding(paddingTop, bottom:paddtingBtm, left:11, right:11);
    }

    public override Padding DialogPadding => new(11);

    public override void Initialize()
    {
        base.Initialize();

        DialogHandlers["MAINMENU"].Dialog.Decorations.Add(new Decoration(SinaiPic, new Point(0.08, 0.09)));
        DialogHandlers["SIZEOFMAP"].Dialog.Decorations.Add(new Decoration(StPeterburgPic, new Point(0, 0.09)));
        DialogHandlers["CUSTOMSIZE"].Dialog.Decorations.Add(new Decoration(StPeterburgPic, new Point(0, 0.09)));
        DialogHandlers["CUSTOMLAND"].Dialog.Decorations.Add(new Decoration(StPeterburgPic, new Point(0, 0.09)));
        DialogHandlers["CUSTOMFORM"].Dialog.Decorations.Add(new Decoration(IslandPic, new Point(0, 0.09)));
        DialogHandlers["CUSTOMCLIMATE"].Dialog.Decorations.Add(new Decoration(DesertPic, new Point(0, 0.09)));
        DialogHandlers["CUSTOMTEMP"].Dialog.Decorations.Add(new Decoration(SnowPic, new Point(0, 0.09)));
        DialogHandlers["CUSTOMAGE"].Dialog.Decorations.Add(new Decoration(CanyonPic, new Point(0, 0.09)));
        DialogHandlers["DIFFICULTY"].Dialog.Decorations.Add(new Decoration(MingGeneralPic, new Point(-0.08, 0.09)));
        DialogHandlers["ENEMIES"].Dialog.Decorations.Add(new Decoration(AncientPersonsPic, new Point(0.08, 0.09)));
        DialogHandlers["BARBARITY"].Dialog.Decorations.Add(new Decoration(BarbariansPic, new Point(-0.08, 0.09)));
        DialogHandlers["RULES"].Dialog.Decorations.Add(new Decoration(GalleyPic, new Point(0.08, 0.09)));
        DialogHandlers["ADVANCED"].Dialog.Decorations.Add(new Decoration(GalleyPic, new Point(-0.08, 0.09)));
        DialogHandlers["ACCELERATED"].Dialog.Decorations.Add(new Decoration(GalleyPic, new Point(0.08, 0.09)));
        DialogHandlers["GENDER"].Dialog.Decorations.Add(new Decoration(PeoplePic1, new Point(0.0, 0.09)));
        DialogHandlers["TRIBE"].Dialog.Decorations.Add(new Decoration(PeoplePic2, new Point(0.0, 0.09)));
        DialogHandlers["CUSTOMTRIBE"].Dialog.Decorations.Add(new Decoration(PeoplePic2, new Point(0.0, 0.09)));
        DialogHandlers["CUSTOMTRIBE2"].Dialog.Decorations.Add(new Decoration(PeoplePic2, new Point(0.0, 0.09)));
        DialogHandlers["NAME"].Dialog.Decorations.Add(new Decoration(PeoplePic2, new Point(0.0, 0.09)));
        DialogHandlers["CUSTOMCITY"].Dialog.Decorations.Add(new Decoration(TemplePic, new Point(0.08, 0.09)));


        // Initialize properties of Units from image
        UnitPICprops = new Dictionary<string, List<ImageProps>>
        {
            { "unit", Enumerable.Range(0, 9 * UnitsRows).Select(i => new ImageProps
                        { Rect = new Rectangle(1 + 65 * (i % 9), 1 + (UnitsPxHeight + 1) * (i / 9), 64, UnitsPxHeight) }).ToList() },
            { "HPshield", new List<ImageProps> { new ImageProps() { Rect = new Rectangle(597, 30, 12, 20) } } },
            { "backShield1", new List<ImageProps> { new ImageProps() { Rect = new Rectangle(586, 1, 12, 20) } } },
            { "backShield2", new List<ImageProps> { new ImageProps() { Rect = new Rectangle(599, 1, 12, 20) } } }
        };

        // Initialize properties of Cities from image
        CitiesPICprops = new Dictionary<string, List<ImageProps>>
        {
            { "textColors", Enumerable.Range(0, 9).Select(col => 
                    new ImageProps { Rect = new Rectangle(1 + 15 * col, 423, 14, 1) }).ToList() },
            { "flags", Enumerable.Range(0, 2 * 9).Select(i =>
                    new ImageProps { Rect = new Rectangle(1 + 15 * (i % 9), 425 + 23 * (i / 9), 14, 22) }).ToList() },
            { "fortify", new List<ImageProps> { new ImageProps() { Rect = new Rectangle(143, 423, 64, 48) } } },
            { "fortress", new List<ImageProps> { new ImageProps() { Rect = new Rectangle(208, 423, 64, 48) } } },
            { "airbase,empty", new List<ImageProps> { new ImageProps() { Rect = new Rectangle(273, 423, 64, 48) } } },
            { "airbase,full", new List<ImageProps> { new ImageProps() { Rect = new Rectangle(338, 423, 64, 48) } } },
        };
        var props = new List<ImageProps>();
        for (int row = 0; row < 6; row++)
        {
            props.AddRange(Enumerable.Range(0, 4).Select(col =>
                    new ImageProps { Rect = new Rectangle(1 + 65 * col, 39 + 49 * row, 64, 48) }).ToList());    // Open cities
            props.AddRange(Enumerable.Range(0, 4).Select(col =>
                    new ImageProps { Rect = new Rectangle(334 + 65 * col, 39 + 49 * row, 64, 48) }).ToList());  // Walled cities
        }
        props.AddRange(Enumerable.Range(0, 4).Select(col =>
                    new ImageProps { Rect = new Rectangle(1 + 65 * col, 347, 64, 48) }).ToList());    // Modern alt.
        props.AddRange(Enumerable.Range(0, 4).Select(col =>
                    new ImageProps { Rect = new Rectangle(333 + 65 * col, 347, 64, 48) }).ToList());
        CitiesPICprops.Add("city", props);

        // Initialize properties of Tiles from image
        TilePICprops = new Dictionary<string, List<ImageProps>>
        {
            { "base1", Enumerable.Range(0, 11).Select(row => 
                        new ImageProps { Rect = new Rectangle(1, 1 + 33 * row, 64, 32) }).ToList() },
            { "base2", Enumerable.Range(0, 11).Select(row =>
                        new ImageProps { Rect = new Rectangle(66, 1 + 33 * row, 64, 32) }).ToList() },
            { "special1", Enumerable.Range(0, 11).Select(row =>
                        new ImageProps { Rect = new Rectangle(131, 1 + 33 * row, 64, 32) }).ToList() },
            { "special2", Enumerable.Range(0, 11).Select(row =>
                        new ImageProps { Rect = new Rectangle(196, 1 + 33 * row, 64, 32) }).ToList() },
            { "road", Enumerable.Range(0, 9).Select(col =>
                        new ImageProps { Rect = new Rectangle(1 + 65 * col, 363, 64, 32) }).ToList() },
            { "railroad", Enumerable.Range(0, 9).Select(col =>
                        new ImageProps { Rect = new Rectangle(1 + 65 * col, 397, 64, 32) }).ToList() },
            { "irrigation", new List<ImageProps> { new ImageProps() { Rect = new Rectangle(456, 100, 64, 32) } } },
            { "farmland", new List<ImageProps> { new ImageProps() { Rect = new Rectangle(456, 133, 64, 32) } } },
            { "mine", new List<ImageProps> { new ImageProps() { Rect = new Rectangle(456, 166, 64, 32) } } },
            { "pollution", new List<ImageProps> { new ImageProps() { Rect = new Rectangle(456, 199, 64, 32) } } },
            { "shield", new List<ImageProps> { new ImageProps() { Rect = new Rectangle(456, 232, 64, 32) } } },
            { "hut", new List<ImageProps> { new ImageProps() { Rect = new Rectangle(456, 265, 64, 32) } } },
            { "dither", new List<ImageProps> { new ImageProps() { Rect = new Rectangle(1, 447, 64, 32) } } },
            { "blank", new List<ImageProps> { new ImageProps() { Rect = new Rectangle(131, 447, 64, 32) } } }
        };

        // Initialize properties of Overlay tiles from image
        OverlayPICprops = new Dictionary<string, List<ImageProps>>
        {
            { "connection", Enumerable.Range(0, 2 * 8).Select(i =>
                    new ImageProps { Rect = new Rectangle(1 + 65 * (i % 8), 1 + 33 * (i / 8), 64, 32) }).ToList() },
            { "river", Enumerable.Range(0, 2 * 8).Select(i =>
                    new ImageProps { Rect = new Rectangle(1 + 65 * (i % 8), 67 + 33 * (i / 8), 64, 32) }).ToList() },
            { "forest", Enumerable.Range(0, 2 * 8).Select(i =>
                    new ImageProps { Rect = new Rectangle(1 + 65 * (i % 8), 133 + 33 * (i / 8), 64, 32) }).ToList() },
            { "mountain", Enumerable.Range(0, 2 * 8).Select(i =>
                    new ImageProps { Rect = new Rectangle(1 + 65 * (i % 8), 199 + 33 * (i / 8), 64, 32) }).ToList() },
            { "hill", Enumerable.Range(0, 2 * 8).Select(i =>
                    new ImageProps { Rect = new Rectangle(1 + 65 * (i % 8), 265 + 33 * (i / 8), 64, 32) }).ToList() },
            { "riverMouth", Enumerable.Range(0, 4).Select(col =>
                    new ImageProps { Rect = new Rectangle(1 + 65 * col, 331, 64, 32) }).ToList() }
        };
        props = new List<ImageProps>();
        for (int i = 0; i < 8; i++)
        {
            props.Add(new ImageProps { Rect = new Rectangle(1 + 66 * i, 429, 32, 16) });
            props.Add(new ImageProps { Rect = new Rectangle(1 + 66 * i, 446, 32, 16) });
            props.Add(new ImageProps { Rect = new Rectangle(1 + 66 * i, 463, 32, 16) });
            props.Add(new ImageProps { Rect = new Rectangle(34 + 66 * i, 463, 32, 16) });
        }
        OverlayPICprops.Add("coastline", props);

        // Initialize properties of Icons tiles from image
        IconsPICprops = new Dictionary<string, List<ImageProps>>
        {
            { "viewPiece", new List<ImageProps> { new ImageProps() { Rect = new Rectangle(199, 256, 64, 32) } } },
            { "gridlines", new List<ImageProps> { new ImageProps() { Rect = new Rectangle(183, 430, 64, 32) } } },
            { "gridlines,visible", new List<ImageProps> { new ImageProps() { Rect = new Rectangle(248, 430, 64, 32) } } },
            { "battleAnim", Enumerable.Range(0, 8).Select(col =>
                    new ImageProps { Rect = new Rectangle(1 + 33 * col, 356, 32, 32) }).ToList() }
        };
    }

    protected override List<MenuDetails> MenuMap { get; } = new List<MenuDetails>
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
                new MenuElement("Zoom &Out|X", new Shortcut(KeyboardKey.KEY_X), KeyboardKey.KEY_O),
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
                new MenuElement("Destro&Y All Units At Cursor|Ctrl+Shift+D",
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

    private static readonly IImageSource SinaiPic = new BinaryStorage
        { FileName = "Intro.dll", DataStart = 0x1E630, Length = 0x9F78 };

    private static readonly IImageSource StPeterburgPic = new BinaryStorage
        { FileName = "Intro.dll", DataStart = 0x285A8,  Length = 0x15D04 };

    private static readonly IImageSource IslandPic = new BinaryStorage
    { FileName = "Intro.dll", DataStart = 0xDA49C, Length = 0x8980 };

    private static readonly IImageSource DesertPic = new BinaryStorage
    { FileName = "Intro.dll", DataStart = 0xD0140, Length = 0xA35A };

    private static readonly IImageSource SnowPic = new BinaryStorage
    { FileName = "Intro.dll", DataStart = 0xE2E1C, Length = 0xA925 };

    private static readonly IImageSource CanyonPic = new BinaryStorage
    { FileName = "Intro.dll", DataStart = 0xC51B8, Length = 0xAF88 };

    private static readonly IImageSource MingGeneralPic = new BinaryStorage
    { FileName = "Intro.dll", DataStart = 0x3E2AC, Length = 0x1D183 };

    private static readonly IImageSource AncientPersonsPic = new BinaryStorage
    { FileName = "Intro.dll", DataStart = 0x5B430, Length = 0x15D04 };

    private static readonly IImageSource BarbariansPic = new BinaryStorage
    { FileName = "Intro.dll", DataStart = 0x71134, Length = 0x13D5B };

    private static readonly IImageSource GalleyPic = new BinaryStorage
    { FileName = "Intro.dll", DataStart = 0xB6A3C, Length = 0xE77A };

    private static readonly IImageSource PeoplePic1 = new BinaryStorage
    { FileName = "Intro.dll", DataStart = 0x84E90, Length = 0x129CE };

    private static readonly IImageSource PeoplePic2 = new BinaryStorage
    { FileName = "Intro.dll", DataStart = 0x97860, Length = 0x139A0 };

    private static readonly IImageSource TemplePic = new BinaryStorage
    { FileName = "Intro.dll", DataStart = 0xAB200, Length = 0xB839 };

    

    public override IImageSource BackgroundImage => new BinaryStorage
        { FileName = "Tiles.dll", DataStart = 0xF7454, Length = 0x1389D };

    public override int UnitsRows => 7;
    public override int UnitsPxHeight => 48;
    public override Dictionary<string, List<ImageProps>> UnitPICprops { get; set; }
    public override Dictionary<string, List<ImageProps>> CitiesPICprops { get; set; }
    public override Dictionary<string, List<ImageProps>> TilePICprops { get; set; }
    public override Dictionary<string, List<ImageProps>> OverlayPICprops { get; set; }
    public override Dictionary<string, List<ImageProps>> IconsPICprops { get; set; }

    public override string? GetFallbackPath(string root, int gameType) => null;

    public override void GetShieldImages()
    {
        Color ShadowColour = new(51, 51, 51, 255);
        Color ReplacementColour = new(255, 0, 0, 255);

        var shield = UnitPICprops["backShield1"][0].Image;
        var shieldFront = Raylib.ImageCopy(shield);
        Raylib.ImageDrawRectangle(ref shieldFront, 0, 0, shieldFront.Width, 7, Color.BLACK);

        var shadow = UnitPICprops["backShield2"][0].Image;
        Raylib.ImageColorReplace(ref shadow, ReplacementColour, ShadowColour);

        UnitImages.Shields = new MemoryStorage(shieldFront, "Unit-Shield", ReplacementColour);
        UnitImages.ShieldBack = new MemoryStorage(shield, "Unit-Shield-Back", ReplacementColour, true);
        UnitImages.ShieldShadow = new MemoryStorage(shadow, "Unit-Shield-Shadow");
    }

    public override void LoadPlayerColours()
    {
        var playerColours = new PlayerColour[9];
        for (int col = 0; col < 9; col++)
        {
            unsafe
            {
                var imageColours = Raylib.LoadImageColors(CitiesPICprops["textColors"][col].Image);
                var textColour = imageColours[0];

                imageColours = Raylib.LoadImageColors(CitiesPICprops["flags"][col].Image);
                var lightColour = imageColours[3 * CitiesPICprops["flags"][col].Image.Width + 8];

                imageColours = Raylib.LoadImageColors(CitiesPICprops["flags"][9 + col].Image);
                var darkColour = imageColours[3 * CitiesPICprops["flags"][9 + col].Image.Width + 5];
                Raylib.UnloadImageColors(imageColours);

                playerColours[col] = new PlayerColour
                {
                    Normal = CitiesPICprops["flags"][col].Image,
                    FlagTexture = Raylib.LoadTextureFromImage(CitiesPICprops["flags"][col].Image),
                    TextColour = textColour,
                    LightColour = lightColour,
                    DarkColour = darkColour
                };
            }
        }
        PlayerColours = playerColours;
    }

    public override UnitShield UnitShield(int unitType) => new()
    {
        ShieldInFrontOfUnit = false,
        Offset = UnitImages.Units[unitType].FlagLoc,
        StackingOffset = new(UnitImages.Units[unitType].FlagLoc.X < UnitImages.UnitRectangle.Width / 2 ? -4 : 4, 0),
        ShadowOffset = new(UnitImages.Units[unitType].FlagLoc.X < UnitImages.UnitRectangle.Width / 2 ? -1 : 1, 1),
        DrawShadow = true,
        HPbarOffset = new(0, 2),
        HPbarSize = new(12, 3),
        HPbarColours = new[] { new Color(243, 0, 0, 255), new Color(255, 223, 79, 255), new Color(87, 171, 39, 255) },
        HPbarSizeForColours = new[] { 3, 8 },
        OrderOffset = new(UnitPICprops["backShield1"][0].Image.Width / 2f, 7),
        OrderTextHeight = UnitPICprops["backShield1"][0].Image.Height - 7,
    };

    /// <summary>
    /// Draw outer border wallpaper around panel
    /// </summary>
    /// <param name="wallpaper">Wallpaper image to tile onto the border</param>
    /// <param name="destination">the Image we're rendering to</param>
    /// <param name="Height">final image Height</param>
    /// <param name="Width">final image Width</param>
    /// <param name="topWidth">Width of top border</param>
    /// <param name="footerWidth">Width of the footer</param>
    public override void DrawBorderWallpaper(Wallpaper wallpaper, ref Image destination, int Height, int Width, Padding padding)
    {
        int rows = Height / wallpaper.Outer.Height + 1;
        var columns = Width / wallpaper.Outer.Width + 1;
        var headerSourceRec = new Rectangle { Height = padding.Top, Width = wallpaper.Outer.Width };
        for (int col = 0; col < columns; col++)
        {
            Raylib.ImageDraw(ref destination, wallpaper.Outer, headerSourceRec,
                new Rectangle(col * wallpaper.Outer.Width, 0, wallpaper.Outer.Width, padding.Top),
                Color.WHITE);
        }
        var leftSide = new Rectangle { Height = wallpaper.Outer.Height, Width = DialogPadding.Left };

        var rightEdge = Width - DialogPadding.Right;
        var rightOffset = Width % wallpaper.Outer.Width;
        var rightSide = new Rectangle { X = rightOffset, Height = wallpaper.Outer.Height, Width = DialogPadding.Right };

        for (int row = 0; row < rows; row++)
        {
            Raylib.ImageDraw(ref destination, wallpaper.Outer, leftSide,
                new Rectangle(0, row * wallpaper.Outer.Height, DialogPadding.Left, wallpaper.Outer.Height),
                Color.WHITE);
            Raylib.ImageDraw(ref destination, wallpaper.Outer, rightSide,
                new Rectangle(rightEdge, row * wallpaper.Outer.Height, DialogPadding.Right, wallpaper.Outer.Height),
                Color.WHITE);
        }

        var bottomEdge = Height - padding.Bottom;
        var bottomOffset = Height % wallpaper.Outer.Height;
        var bottomSource = new Rectangle { Y = bottomOffset, Height = padding.Bottom, Width = wallpaper.Outer.Width };
        for (int col = 0; col < columns; col++)
        {
            Raylib.ImageDraw(ref destination, wallpaper.Outer, bottomSource,
                new Rectangle(col * wallpaper.Outer.Width, bottomEdge, wallpaper.Outer.Width, padding.Bottom),
                Color.WHITE);
        }
    }

    public override void DrawBorderLines(ref Image destination, int Height, int Width, Padding padding)
    {
        // Outer border
        var pen1 = new Color(227, 227, 227, 255);
        var pen2 = new Color(105, 105, 105, 255);
        var pen3 = new Color(255, 255, 255, 255);
        var pen4 = new Color(160, 160, 160, 255);
        var pen5 = new Color(240, 240, 240, 255);
        var pen6 = new Color(223, 223, 223, 255);
        var pen7 = new Color(67, 67, 67, 255);
        Raylib.ImageDrawLine(ref destination, 0, 0, Width - 2, 0, pen1); // 1st layer of border
        Raylib.ImageDrawLine(ref destination, 0, 0, Width - 2, 0, pen1);
        Raylib.ImageDrawLine(ref destination, 0, 0, 0, Height - 2, pen1);
        Raylib.ImageDrawLine(ref destination, Width - 1, 0, Width - 1, Height - 1, pen2);
        Raylib.ImageDrawLine(ref destination, 0, Height - 1, Width - 1, Height - 1, pen2);
        Raylib.ImageDrawLine(ref destination, 1, 1, Width - 3, 1, pen3); // 2nd layer of border
        Raylib.ImageDrawLine(ref destination, 1, 1, 1, Height - 3, pen3);
        Raylib.ImageDrawLine(ref destination, Width - 2, 1, Width - 2, Height - 2, pen4);
        Raylib.ImageDrawLine(ref destination, 1, Height - 2, Width - 2, Height - 2, pen4);
        Raylib.ImageDrawLine(ref destination, 2, 2, Width - 4, 2, pen5); // 3rd layer of border
        Raylib.ImageDrawLine(ref destination, 2, 2, 2, Height - 4, pen5);
        Raylib.ImageDrawLine(ref destination, Width - 3, 2, Width - 3, Height - 3, pen5);
        Raylib.ImageDrawLine(ref destination, 2, Height - 3, Width - 3, Height - 3, pen5);
        Raylib.ImageDrawLine(ref destination, 3, 3, Width - 5, 3, pen6); // 4th layer of border
        Raylib.ImageDrawLine(ref destination, 3, 3, 3, Height - 5, pen6);
        Raylib.ImageDrawLine(ref destination, Width - 4, 3, Width - 4, Height - 4, pen7);
        Raylib.ImageDrawLine(ref destination, 3, Height - 4, Width - 4, Height - 4, pen7);
        Raylib.ImageDrawLine(ref destination, 4, 4, Width - 6, 4, pen6); // 5th layer of border
        Raylib.ImageDrawLine(ref destination, 4, 4, 4, Height - 6, pen6);
        Raylib.ImageDrawLine(ref destination, Width - 5, 4, Width - 5, Height - 5, pen7);
        Raylib.ImageDrawLine(ref destination, 4, Height - 5, Width - 5, Height - 5, pen7);

        // Inner panel
        Raylib.ImageDrawLine(ref destination, 9, padding.Top - 1, 9 + (Width - 18 - 1), padding.Top - 1, pen7); // 1st layer of border
        Raylib.ImageDrawLine(ref destination, 10, padding.Top - 1, 10, Height - padding.Bottom - 1, pen7);
        Raylib.ImageDrawLine(ref destination, Width - 11, padding.Top - 1, Width - 11, Height - padding.Bottom - 1, pen6);
        Raylib.ImageDrawLine(ref destination, 9, Height - padding.Bottom, Width - 9 - 1, Height - padding.Bottom, pen6);
        Raylib.ImageDrawLine(ref destination, 10, padding.Top - 2, 9 + (Width - 18 - 2), padding.Top - 2, pen7); // 2nd layer of border
        Raylib.ImageDrawLine(ref destination, 9, padding.Top - 2, 9, Height - padding.Bottom, pen7);
        Raylib.ImageDrawLine(ref destination, Width - 10, padding.Top - 2, Width - 10, Height - padding.Bottom, pen6);
        Raylib.ImageDrawLine(ref destination, 9, Height - padding.Bottom + 1, Width - 9 - 1, Height - padding.Bottom + 1, pen6);
    }
}