using System.Numerics;
using System.Runtime.Intrinsics.X86;
using Civ2;
using Civ2.Dialogs;
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

namespace TOT;

/// <summary>
/// This class is dynamically instantiated when ToT files are detected
/// </summary>
[UsedImplicitly]
public class TestOfTimeInterface : Civ2Interface
{
    public override string Title => "Test of Time";

    public override string InitialMenu => "STARTMENU";

    public override InterfaceStyle Look { get; } = new()
    {
        OuterTitleTop = Enumerable.Range(0, 6).Select(col => new BitmapStorage("dialog.png", 1 + 59 * col, 94, 58, 28)).ToArray(),
        OuterThinTop = Enumerable.Range(0, 6).Select(col => new BitmapStorage("dialog.png", 1 + 59 * col, 123, 58, 12)).ToArray(),
        OuterBottom = Enumerable.Range(0, 6).Select(col => new BitmapStorage("dialog.png", 1 + 59 * col, 136, 58, 11)).ToArray(),
        OuterMiddle = Enumerable.Range(0, 6).Select(col => new BitmapStorage("dialog.png", 1 + 59 * col, 148, 58, 13)).ToArray(),
        OuterLeft = Enumerable.Range(0, 6).Select(col => new BitmapStorage("dialog.png", 112 + 12 * col, 168, 11, 29)).ToArray(),
        OuterRight = Enumerable.Range(0, 6).Select(col => new BitmapStorage("dialog.png", 112 + 13 * col, 198, 12, 29)).ToArray(),
        OuterTitleTopLeft = new BitmapStorage("dialog.png", new[] { new Color(255, 0, 255, 255) }, 1, 168, 14, 30),
        OuterTitleTopRight = new BitmapStorage("dialog.png", new[] { new Color(255, 0, 255, 255) }, 16, 168, 14, 30),
        OuterThinTopLeft = new BitmapStorage("dialog.png", new[] { new Color(255, 0, 255, 255) }, 1, 199, 14, 14),
        OuterThinTopRight = new BitmapStorage("dialog.png", new[] { new Color(255, 0, 255, 255) }, 16, 199, 14, 14),
        OuterMiddleLeft = new BitmapStorage("dialog.png", new[] { new Color(255, 0, 255, 255) }, 1, 214, 14, 18),
        OuterMiddleRight = new BitmapStorage("dialog.png", new[] { new Color(255, 0, 255, 255) }, 16, 214, 14, 18),
        OuterBottomLeft = new BitmapStorage("dialog.png", new[] { new Color(255, 0, 255, 255) }, 1, 233, 14, 14),
        OuterBottomRight = new BitmapStorage("dialog.png", new[] { new Color(255, 0, 255, 255) }, 16, 233, 14, 14),

        Inner = new BitmapStorage("dialog.png", new Rectangle(1, 1, 92, 92)),

        RadioButtons = new IImageSource[]
        { new BitmapStorage("dialog.png", new[]{ new Color(255, 0, 255, 255) }, 903, 94, 33, 33), 
          new BitmapStorage("dialog.png", new[]{ new Color(255, 0, 255, 255) }, 869, 94, 33, 33) },
        CheckBoxes = new IImageSource[]
        { new BitmapStorage("dialog.png", new[]{ new Color(255, 0, 255, 255) }, 805, 94, 29, 29),
          new BitmapStorage("dialog.png", new[]{ new Color(255, 0, 255, 255) }, 775, 94, 29, 29) },

        DefaultFont = Fonts.Arial,
        ButtonFont = Fonts.Arial,
        ButtonFontSize = 20,
        ButtonColour = Color.BLACK,
        HeaderLabelFont = Fonts.Arial,
        HeaderLabelFontSizeNormal = 20,
        HeaderLabelShadow = false,
        HeaderLabelColour = Color.BLACK,
        LabelFont = Fonts.Arial,
        LabelColour = Color.LIGHTGRAY,
        CityWindowFont = Fonts.Arial,
        CityWindowFontSize = 16,
        MenuFont = Fonts.Arial,
        MenuFontSize = 14
    };

    public override bool IsButtonInOuterPanel => false;

    public override Padding GetPadding(float headerLabelHeight, bool footer)
    {
        int paddingTop = headerLabelHeight != 0 ? 28 : 12;

        return new Padding(paddingTop, bottom: 11, left: 11, right: 12);
    }
    public override Padding DialogPadding => new(12, bottom: 11, left: 11, right: 12);


    public override void Initialize()
    {
        base.Initialize();

        DialogHandlers["STARTMENU"].Dialog.Decorations.Add(new Decoration(ObservatoryPic, new Point(0.08, 0.09)));
        DialogHandlers["MAINMENU"].Dialog.Decorations.Add(new Decoration(ObservatoryPic, new Point(0.08, 0.09)));
        DialogHandlers["SIZEOFMAP"].Dialog.Decorations.Add(new Decoration(HorzionPic, new Point(0.08, 0.09)));
        DialogHandlers["DIFFICULTY"].Dialog.Decorations.Add(new Decoration(CreaturePic, new Point(0.08, 0.09)));
        DialogHandlers["ENEMIES"].Dialog.Decorations.Add(new Decoration(DuelPic, new Point(0.08, 0.09)));
        DialogHandlers["BARBARITY"].Dialog.Decorations.Add(new Decoration(KnightsPic, new Point(0.08, 0.09)));
        DialogHandlers["RULES"].Dialog.Decorations.Add(new Decoration(BookPic, new Point(0.08, 0.09)));
        DialogHandlers["ADVANCED"].Dialog.Decorations.Add(new Decoration(BookPic, new Point(0.08, 0.09)));
        //DialogHandlers["OPPONENT"].Dialog.Decorations.Add(new Decoration(BookPic, new Point(0.08, 0.09)));
        DialogHandlers["ACCELERATED"].Dialog.Decorations.Add(new Decoration(BookPic, new Point(0.08, 0.09)));
        DialogHandlers["GENDER"].Dialog.Decorations.Add(new Decoration(LabPic, new Point(0.08, 0.09)));
        DialogHandlers["TRIBE"].Dialog.Decorations.Add(new Decoration(TemplePic, new Point(0.08, 0.09)));
        DialogHandlers["CUSTOMTRIBE"].Dialog.Decorations.Add(new Decoration(TemplePic, new Point(0.08, 0.09)));
        DialogHandlers["CUSTOMTRIBE2"].Dialog.Decorations.Add(new Decoration(TemplePic, new Point(0.08, 0.09)));
        DialogHandlers["CUSTOMCITY"].Dialog.Decorations.Add(new Decoration(ManorPic, new Point(0.08, 0.09)));
        DialogHandlers["CUSTOMFORM"].Dialog.Decorations.Add(new Decoration(ShipPic, new Point(0.08, 0.09)));
        DialogHandlers["CUSTOMCLIMATE"].Dialog.Decorations.Add(new Decoration(RocksPic, new Point(0.08, 0.09)));
        DialogHandlers["CUSTOMTEMP"].Dialog.Decorations.Add(new Decoration(HotplanetPic, new Point(0.08, 0.09)));
        DialogHandlers["CUSTOMAGE"].Dialog.Decorations.Add(new Decoration(AlienplanetPic, new Point(0.08, 0.09)));

        if (Dialogs.TryGetValue(MainMenu.Title + "2", out var menu2))
        {
            var existingDialog = DialogHandlers[MainMenu.Title].Dialog.Dialog;
            existingDialog.Options = menu2.Options.Concat(existingDialog.Options.Skip(5)).ToList();
        }


        // Initialize properties of Units from image
        UnitPICprops = new Dictionary<string, List<ImageProps>>
        {
            { "unit", Enumerable.Range(0, 9 * UnitsRows).Select(i => new ImageProps
                        { Rect = new Rectangle(1 + 65 * (i % 9), 1 + (UnitsPxHeight + 1) * (i / 9), 64, UnitsPxHeight) }).ToList() },
            { "HPshield", new List<ImageProps> { new ImageProps() { Rect = new Rectangle(586, 1, 32, 10) } } },
        };

        // Initialize properties of Cities from image
        CitiesPICprops = new Dictionary<string, List<ImageProps>>
        {
            { "textColors", Enumerable.Range(0, 9).Select(col =>
                    new ImageProps { Rect = new Rectangle(1 + 15 * col, 421, 14, 3) }).ToList() },
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
                        new ImageProps { Rect = new Rectangle(1 + 65 * col, 364, 64, 32) }).ToList() },
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
                new MenuElement("&Center View|c", new Shortcut(KeyboardKey.KEY_C), KeyboardKey.KEY_C),
                new MenuElement("Map Layout", Shortcut.None, KeyboardKey.KEY_NULL),
                new MenuElement("City Layout", Shortcut.None, KeyboardKey.KEY_NULL),
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
                new MenuElement("Transform to ...|o", new Shortcut(KeyboardKey.KEY_O), KeyboardKey.KEY_NULL),
                new MenuElement("Build &Airbase|e", new Shortcut(KeyboardKey.KEY_E), KeyboardKey.KEY_A),
                new MenuElement("Build &Teleporter|j", new Shortcut(KeyboardKey.KEY_J), KeyboardKey.KEY_T),
                new MenuElement("Build &Fortress|f", new Shortcut(KeyboardKey.KEY_F), KeyboardKey.KEY_F),
                new MenuElement("Automate Settler|k", new Shortcut(KeyboardKey.KEY_K), KeyboardKey.KEY_NULL),
                new MenuElement("Clean Up &Pollution|p", new Shortcut(KeyboardKey.KEY_P), KeyboardKey.KEY_P),
                new MenuElement("&Pillage|Shift+P", new Shortcut(KeyboardKey.KEY_P, shift: true), KeyboardKey.KEY_P),
                new MenuElement("&Unload|u", new Shortcut(KeyboardKey.KEY_U), KeyboardKey.KEY_U),
                new MenuElement("&Go To|g", new Shortcut(KeyboardKey.KEY_G), KeyboardKey.KEY_G),
                new MenuElement("&Paradrop|p", new Shortcut(KeyboardKey.KEY_P), KeyboardKey.KEY_P),
                new MenuElement("Air&lift|l", new Shortcut(KeyboardKey.KEY_L), KeyboardKey.KEY_L),
                new MenuElement("Teleport|n", new Shortcut(KeyboardKey.KEY_N), KeyboardKey.KEY_NULL),
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
            Key = "MAP", Defaults = new List<MenuElement>
            {
                new MenuElement("&Map", Shortcut.None, KeyboardKey.KEY_M),
                new MenuElement("Toggle Cheat Mode|Ctrl+K", new Shortcut(KeyboardKey.KEY_K, ctrl: true),
                    KeyboardKey.KEY_NULL),
                new MenuElement("&Select Map|Ctrl+Shift+1",
                    new Shortcut(KeyboardKey.KEY_ONE, ctrl: true, shift: true), KeyboardKey.KEY_S),
                new MenuElement("&Import Map|Ctrl+Shift+2",
                    new Shortcut(KeyboardKey.KEY_TWO, ctrl: true, shift: true), KeyboardKey.KEY_I),
                new MenuElement("&Export Map|Ctrl+Shift+3",
                    new Shortcut(KeyboardKey.KEY_THREE, ctrl: true, shift: true), KeyboardKey.KEY_E),
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
                new MenuElement("&About Test of Time", Shortcut.None, KeyboardKey.KEY_A)
            }
        }
    };

    private static readonly IImageSource ObservatoryPic = new BinaryStorage
    { FileName = "Intro.dll", DataStart = 0x1E630, Length = 0xACDC };

    private static readonly IImageSource HorzionPic = new BinaryStorage
    { FileName = "Intro.dll", DataStart = 0x2390C, Length = 0x58A5 };

    private static readonly IImageSource CreaturePic = new BinaryStorage
    { FileName = "Intro.dll", DataStart = 0x2EBB4, Length = 0x14600 };

    private static readonly IImageSource DuelPic = new BinaryStorage
    { FileName = "Intro.dll", DataStart = 0x431B4, Length = 0x177AF };

    private static readonly IImageSource KnightsPic = new BinaryStorage
    { FileName = "Intro.dll", DataStart = 0x5A964, Length = 0xEB67 };

    private static readonly IImageSource LabPic = new BinaryStorage
    { FileName = "Intro.dll", DataStart = 0x694CC, Length = 0xFC4B };

    private static readonly IImageSource TemplePic = new BinaryStorage
    { FileName = "Intro.dll", DataStart = 0x79118, Length = 0x10EBB };

    private static readonly IImageSource ManorPic = new BinaryStorage
    { FileName = "Intro.dll", DataStart = 0x89FD4, Length = 0x13AA8 };

    private static readonly IImageSource BookPic = new BinaryStorage
    { FileName = "Intro.dll", DataStart = 0x9DA7C, Length = 0x162AE };

    private static readonly IImageSource AlienplanetPic = new BinaryStorage
    { FileName = "Intro.dll", DataStart = 0xB3D2C, Length = 0x988D };

    private static readonly IImageSource RocksPic = new BinaryStorage
    { FileName = "Intro.dll", DataStart = 0xBD5BC, Length = 0xBF78 };

    private static readonly IImageSource ShipPic = new BinaryStorage
    { FileName = "Intro.dll", DataStart = 0xC9534, Length = 0x5A03 };

    private static readonly IImageSource HotplanetPic = new BinaryStorage
    { FileName = "Intro.dll", DataStart = 0xCEF38, Length = 0x9130 };

    public override int UnitsRows => 9;
    public override int UnitsPxHeight => 64;
    
    public override Dictionary<string, List<ImageProps>> UnitPICprops { get; set; }
    public override Dictionary<string, List<ImageProps>> CitiesPICprops { get; set; }
    public override Dictionary<string, List<ImageProps>> TilePICprops { get; set; }
    public override Dictionary<string, List<ImageProps>> OverlayPICprops { get; set; }
    public override Dictionary<string, List<ImageProps>> IconsPICprops { get; set; }

    public override string? GetFallbackPath(string root, int gameType)
    {
        string? path = null;
        switch (gameType)
        {
            case 0:
                path = root + Path.DirectorySeparatorChar + "Original";
                break;
            case 1:
                path = root + Path.DirectorySeparatorChar + "Scifi";
                break;    
            case 2:
                path = root + Path.DirectorySeparatorChar + "Fantasy";
                break;
            default:
                break;
        }
        return Directory.Exists(path) ? path : null;
    }

    public override void LoadPlayerColours()
    {
        var playerColours = new PlayerColour[9];
        for (int col = 0; col < 9; col++)
        {
            unsafe
            {
                var imageColours = Raylib.LoadImageColors(CitiesPICprops["textColors"][col].Image);
                var textColour = imageColours[2 * CitiesPICprops["textColors"][col].Image.width + 0];
                var shieldColour = imageColours[2 * CitiesPICprops["textColors"][col].Image.width + 0];
                textColour.a = 255; // to avoid any upper-left-pixel transparency issues
                shieldColour.a = 255;
                Raylib.UnloadImageColors(imageColours);

                // This is not exact, but a good aproximation what TOT does with shield colours
                var lightColour = new Color(shieldColour.r / 2, shieldColour.g / 2, shieldColour.b / 2, 255);
                var darkColour = new Color(shieldColour.r / 4, shieldColour.g / 4, shieldColour.b / 4, 255);

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

    public override void GetShieldImages()
    {
        Color ReplacementColour = new(255, 0, 255, 0);

        var shield = UnitPICprops["HPshield"][0].Image;
        var shieldFront = Raylib.ImageCopy(shield);

        UnitImages.Shields = new MemoryStorage(shieldFront, "Unit-Shield", ReplacementColour);
        UnitImages.ShieldBack = new MemoryStorage(shield, "Unit-Shield-Back", ReplacementColour, true);
    }

    public override UnitShield UnitShield(int unitType) => new()
    {
        ShieldInFrontOfUnit = true,
        Offset = new(16, 0),
        StackingOffset = new(-4, 0),
        DrawShadow = false,
        HPbarOffset = new(10, 4),
        HPbarSize = new(20, 2),
        HPbarColours = new[] { new Color(247, 0, 0, 255), new Color(255, 222, 74, 255), new Color(82, 173, 33, 255) },
        HPbarSizeForColours = new[] { 5, 13 },
        OrderOffset = new(9 / 2f, 1),
        OrderTextHeight = UnitPICprops["HPshield"][0].Image.height - 1
    };

    public override void DrawBorderWallpaper(Wallpaper wp, ref Image destination, int height, int width, Padding padding)
    {
        var top_left = padding.Top == 12 ? wp.OuterThinTopLeft : wp.OuterTitleTopLeft;
        var top = padding.Top == 12 ? wp.OuterThinTop : wp.OuterTitleTop;
        var top_right = padding.Top == 12 ? wp.OuterThinTopRight : wp.OuterTitleTopRight;

        // Top border
        Raylib.ImageDraw(ref destination, top_left, new Rectangle(0, 0, top_left.width, top_left.height), new Rectangle(0, 0, top_left.width, top_left.height), Color.WHITE);
        var topCols = (width - top_left.width - top_right.width) / top.width + 1;
        for (int col = 0; col < topCols; col++)
        {
            Raylib.ImageDraw(ref destination, top, new Rectangle(0, 0, top.width, top.height), new Rectangle(top_left.width + top.width * col, 0, top.width, top.height), Color.WHITE);
        }
        Raylib.ImageDraw(ref destination, top_right, new Rectangle(0, 0, top_right.width, top_right.height), new Rectangle(width - top_right.width, 0, top_right.width, top_right.height), Color.WHITE);

        // Left-right border
        var sideRows = (height - top_left.height - wp.OuterBottomLeft.height) / wp.OuterLeft.height + 1;
        for (int row = 0; row < sideRows; row++)
        {
            Raylib.ImageDraw(ref destination, wp.OuterLeft, new Rectangle(0, 0, wp.OuterLeft.width, wp.OuterLeft.height), new Rectangle(0, top_left.height + wp.OuterLeft.height * row, wp.OuterLeft.width, wp.OuterLeft.height), Color.WHITE);
            Raylib.ImageDraw(ref destination, wp.OuterRight, new Rectangle(0, 0, wp.OuterRight.width, wp.OuterRight.height), new Rectangle(width - wp.OuterRight.width, top_right.height + wp.OuterRight.height * row, wp.OuterRight.width, wp.OuterRight.height), Color.WHITE);
        }

        // Bottom border
        Raylib.ImageDraw(ref destination, wp.OuterBottomLeft, new Rectangle(0, 0, wp.OuterBottomLeft.width, wp.OuterBottomLeft.height), new Rectangle(0, height - wp.OuterBottomLeft.height, wp.OuterBottomLeft.width, wp.OuterBottomLeft.height), Color.WHITE);
        var btmCols = (width - wp.OuterBottomLeft.width - wp.OuterBottomRight.width) / wp.OuterBottom.width + 1;
        for (int col = 0; col < btmCols; col++)
        {
            Raylib.ImageDraw(ref destination, wp.OuterBottom, new Rectangle(0, 0, wp.OuterBottom.width, wp.OuterBottom.height), new Rectangle(wp.OuterBottomLeft.width + wp.OuterBottom.width * col, height - wp.OuterBottom.height, wp.OuterBottom.width, wp.OuterBottom.height), Color.WHITE);
        }
        Raylib.ImageDraw(ref destination, wp.OuterBottomRight, new Rectangle(0, 0, wp.OuterBottomRight.width, wp.OuterBottomRight.height), new Rectangle(width - wp.OuterBottomRight.width, height - wp.OuterBottomRight.height, wp.OuterBottomRight.width, wp.OuterBottomRight.height), Color.WHITE);
    }

    public override void DrawBorderLines(ref Image destination, int height, int width, Padding padding) { }
}