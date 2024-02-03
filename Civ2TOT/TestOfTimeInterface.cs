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
        ButtonColour = Color.Black,
        HeaderLabelFont = Fonts.Arial,
        HeaderLabelFontSizeNormal = 20,
        HeaderLabelShadow = false,
        HeaderLabelColour = Color.Black,
        LabelFont = Fonts.Arial,
        LabelColour = Color.LightGray,
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
        UnitPicProps = new Dictionary<string, List<ImageProps>>
        {
            { "unit", Enumerable.Range(0, 9 * UnitsRows).Select(i => new ImageProps
                        { Rect = new Rectangle(1 + 65 * (i % 9), 1 + (UnitsPxHeight + 1) * (i / 9), 64, UnitsPxHeight) }).ToList() },
            { "HPshield", new List<ImageProps> { new ImageProps() { Rect = new Rectangle(586, 1, 32, 10) } } },
        };

        // Initialize properties of Cities from image
        CitiesPicProps = new Dictionary<string, List<ImageProps>>
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
        CitiesPicProps.Add("city", props);

        // Initialize properties of Tiles from image
        TilePicProps = new Dictionary<string, List<ImageProps>>
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
        OverlayPicProps = new Dictionary<string, List<ImageProps>>
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
        OverlayPicProps.Add("coastline", props);

        // Initialize properties of Icons tiles from image
        IconsPicProps = new Dictionary<string, List<ImageProps>>
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
                new MenuElement("&Game", Shortcut.None, KeyboardKey.G),
                new MenuElement("Game &Options|Ctrl+O", new Shortcut(KeyboardKey.O, ctrl: true), KeyboardKey.O),
                new MenuElement("Graphic O&ptions|Ctrl+P", new Shortcut(KeyboardKey.P, ctrl: true),
                    KeyboardKey.P),
                new MenuElement("&City Report Options|Ctrl+E", new Shortcut(KeyboardKey.E, ctrl: true),
                    KeyboardKey.C),
                new MenuElement("M&ultiplayer Options|Ctrl+Y", new Shortcut(KeyboardKey.Y, ctrl: true),
                    KeyboardKey.U),
                new MenuElement("&Game Profile", Shortcut.None, KeyboardKey.G),
                new MenuElement("Pick &Music", Shortcut.None, KeyboardKey.M),
                new MenuElement("&Save Game|Ctrl+S", new Shortcut(KeyboardKey.S, ctrl: true), KeyboardKey.S),
                new MenuElement("&Load Game|Ctrl+L", new Shortcut(KeyboardKey.L, ctrl: true), KeyboardKey.L),
                new MenuElement("&Join Game|Ctrl+J", new Shortcut(KeyboardKey.J, ctrl: true), KeyboardKey.J),
                new MenuElement("Set Pass&word|Ctrl+W", new Shortcut(KeyboardKey.W, ctrl: true), KeyboardKey.W),
                new MenuElement("Change &Timer|Ctrl+T", new Shortcut(KeyboardKey.T, ctrl: true), KeyboardKey.T),
                new MenuElement("&Retire|Ctrl+R", new Shortcut(KeyboardKey.R, ctrl: true), KeyboardKey.R),
                new MenuElement("&Quit|Ctrl+Q", new Shortcut(KeyboardKey.Q, ctrl: true), KeyboardKey.Q)
            }
        },
        new MenuDetails
        {
            Key = "KINGDOM", Defaults = new List<MenuElement>
            {
                new MenuElement("&Kingdom", Shortcut.None, KeyboardKey.K),
                new MenuElement("&Tax Rate|Shift+T", new Shortcut(KeyboardKey.T, shift: true), KeyboardKey.T),
                new MenuElement("Find &City|Shift+C", new Shortcut(KeyboardKey.C, shift: true), KeyboardKey.C),
                new MenuElement("&REVOLUTION|Shift+R", new Shortcut(KeyboardKey.R, shift: true), KeyboardKey.R)
            }
        },

        new MenuDetails
        {
            Key = "VIEW", Defaults = new List<MenuElement>
            {
                new MenuElement("&View", Shortcut.None, KeyboardKey.V),
                new MenuElement("&Move Pieces|v", new Shortcut(KeyboardKey.V), KeyboardKey.M),
                new MenuElement("&View Pieces|v", new Shortcut(KeyboardKey.V), KeyboardKey.V),
                new MenuElement("Zoom &In|z", new Shortcut(KeyboardKey.Z), KeyboardKey.I),
                new MenuElement("Zoom &Out|X", new Shortcut(KeyboardKey.X), KeyboardKey.O),
                new MenuElement("Max Zoom In|Ctrl+Z", new Shortcut(KeyboardKey.Z, ctrl: true),
                    KeyboardKey.Null),
                new MenuElement("Standard Zoom|Shift+Z", new Shortcut(KeyboardKey.Z, shift: true),
                    KeyboardKey.Null),
                new MenuElement("Medium Zoom Out|Shift+X", new Shortcut(KeyboardKey.X, shift: true),
                    KeyboardKey.Null),
                new MenuElement("Max Zoom Out|Ctrl+X", new Shortcut(KeyboardKey.X, ctrl: true),
                    KeyboardKey.Null),
                new MenuElement("Show Map Grid|Ctrl+G", new Shortcut(KeyboardKey.G, ctrl: true),
                    KeyboardKey.Null),
                new MenuElement("Arrange Windows", Shortcut.None, KeyboardKey.Null),
                new MenuElement("Show Hidden Terrain|t", new Shortcut(KeyboardKey.T), KeyboardKey.T),
                new MenuElement("&Center View|c", new Shortcut(KeyboardKey.C), KeyboardKey.C),
                new MenuElement("Map Layout", Shortcut.None, KeyboardKey.Null),
                new MenuElement("City Layout", Shortcut.None, KeyboardKey.Null),
            }
        },

        new MenuDetails
        {
            Key = "@ORDERS", Defaults = new List<MenuElement>
            {
                new MenuElement("&Orders", Shortcut.None, KeyboardKey.O),
                new MenuElement("&Build New City|b", new Shortcut(KeyboardKey.B), KeyboardKey.B, BuildCityOrder, true),
                new MenuElement("Build &Road|r", new Shortcut(KeyboardKey.R), KeyboardKey.R, BuildRoadOrder, omitIfNoCommand: true),
                new MenuElement("Build &Irrigation|i", new Shortcut(KeyboardKey.I), KeyboardKey.I,BuildIrrigationOrder, omitIfNoCommand:true),
                new MenuElement("Build &Mines|m", new Shortcut(KeyboardKey.M), KeyboardKey.M,BuildMineOrder, omitIfNoCommand: true),
                new MenuElement("Build %STRING0", Shortcut.None, KeyboardKey.Null, BuildIrrigationOrder, omitIfNoCommand:true),
                new MenuElement("Transform to ...|o", new Shortcut(KeyboardKey.O), KeyboardKey.Null),
                new MenuElement("Build &Airbase|e", new Shortcut(KeyboardKey.E), KeyboardKey.A),
                new MenuElement("Build &Teleporter|j", new Shortcut(KeyboardKey.J), KeyboardKey.T),
                new MenuElement("Build &Fortress|f", new Shortcut(KeyboardKey.F), KeyboardKey.F),
                new MenuElement("Automate Settler|k", new Shortcut(KeyboardKey.K), KeyboardKey.Null),
                new MenuElement("Clean Up &Pollution|p", new Shortcut(KeyboardKey.P), KeyboardKey.P),
                new MenuElement("&Pillage|Shift+P", new Shortcut(KeyboardKey.P, shift: true), KeyboardKey.P),
                new MenuElement("&Unload|u", new Shortcut(KeyboardKey.U), KeyboardKey.U),
                new MenuElement("&Go To|g", new Shortcut(KeyboardKey.G), KeyboardKey.G),
                new MenuElement("&Paradrop|p", new Shortcut(KeyboardKey.P), KeyboardKey.P),
                new MenuElement("Air&lift|l", new Shortcut(KeyboardKey.L), KeyboardKey.L),
                new MenuElement("Teleport|n", new Shortcut(KeyboardKey.N), KeyboardKey.Null),
                new MenuElement("Set &Home City|h", new Shortcut(KeyboardKey.H), KeyboardKey.H),
                new MenuElement("&Fortify|f", new Shortcut(KeyboardKey.F), KeyboardKey.F),
                new MenuElement("&Sleep|s", new Shortcut(KeyboardKey.S), KeyboardKey.S),
                new MenuElement("&Disband|Shift+D", new Shortcut(KeyboardKey.D, shift: true), KeyboardKey.D),
                new MenuElement("&Activate Unit|a", new Shortcut(KeyboardKey.A), KeyboardKey.A),
                new MenuElement("&Wait|w", new Shortcut(KeyboardKey.W), KeyboardKey.W),
                new MenuElement("S&kip Turn|SPACE", new Shortcut(KeyboardKey.Space), KeyboardKey.K),
                new MenuElement("End Player Tur&n|Ctrl+N", new Shortcut(KeyboardKey.T, shift: true),
                    KeyboardKey.N, EndTurn)
            },
        },

        new MenuDetails
        {
            Key = "ADVISORS", Defaults = new List<MenuElement>
            {
                new MenuElement("&Advisors", Shortcut.None, KeyboardKey.A),
                new MenuElement("Chat with &Kings|Ctrl+C", new Shortcut(KeyboardKey.C, ctrl: true),
                    KeyboardKey.K),
                new MenuElement("&City Status|F1", new Shortcut(KeyboardKey.F1), KeyboardKey.C),
                new MenuElement("&Defense Minister|F2", new Shortcut(KeyboardKey.F2), KeyboardKey.D),
                new MenuElement("&Foreign Minister|F3", new Shortcut(KeyboardKey.F3), KeyboardKey.F),
                new MenuElement("&Attitude Advisor|F4", new Shortcut(KeyboardKey.F4), KeyboardKey.A),
                new MenuElement("&Trade Advisor|F5", new Shortcut(KeyboardKey.F5), KeyboardKey.T),
                new MenuElement("&Science Advisor|F6", new Shortcut(KeyboardKey.F6), KeyboardKey.S),
                new MenuElement("Cas&ualty Timeline|Ctrl-D", new Shortcut(KeyboardKey.D, ctrl: true),
                    KeyboardKey.U)
            }
        },

        new MenuDetails
        {
            Key = "WORLD", Defaults = new List<MenuElement>
            {
                new MenuElement("&World", Shortcut.None, KeyboardKey.W),
                new MenuElement("&Wonders of the World|F7", new Shortcut(KeyboardKey.F7), KeyboardKey.W),
                new MenuElement("&Top 5 Cities|F8", new Shortcut(KeyboardKey.F8), KeyboardKey.T),
                new MenuElement("&Civilization Score|F9", new Shortcut(KeyboardKey.F9), KeyboardKey.C),
                new MenuElement("&Demographics|F11", new Shortcut(KeyboardKey.F11), KeyboardKey.D),
                new MenuElement("&Spaceships|F12", new Shortcut(KeyboardKey.F12), KeyboardKey.S)
            }
        },

        new MenuDetails
        {
            Key = "CHEAT", Defaults = new List<MenuElement>
            {
                new MenuElement("&Cheat", Shortcut.None, KeyboardKey.C),
                new MenuElement("Toggle Cheat Mode|Ctrl+K", new Shortcut(KeyboardKey.K, ctrl: true),
                    KeyboardKey.Null),
                new MenuElement("Create &Unit|Shift+F1", new Shortcut(KeyboardKey.F1, shift: true),
                    KeyboardKey.U),
                new MenuElement("Reveal &Map|Shift+F2", new Shortcut(KeyboardKey.F2, shift: true),
                    KeyboardKey.M),
                new MenuElement("Set &Human Player|Shift+F3", new Shortcut(KeyboardKey.F3, shift: true),
                    KeyboardKey.H),
                new MenuElement("Set Game Year|Shift+F4", new Shortcut(KeyboardKey.F4, shift: true),
                    KeyboardKey.Null),
                new MenuElement("&Kill Civilization|Shift+F5", new Shortcut(KeyboardKey.F5, shift: true),
                    KeyboardKey.K),
                new MenuElement("Te&chnology Advance|Shift+F6", new Shortcut(KeyboardKey.F6, shift: true),
                    KeyboardKey.C),
                new MenuElement("&Edit Technologies|Ctrl+Shift+F6",
                    new Shortcut(KeyboardKey.F6, ctrl: true, shift: true), KeyboardKey.E),
                new MenuElement("Force &Government|Shift+F7", new Shortcut(KeyboardKey.F7, shift: true),
                    KeyboardKey.G),
                new MenuElement("Change &Terrain At Cursor|Shift+F8", new Shortcut(KeyboardKey.F8, shift: true),
                    KeyboardKey.T),
                new MenuElement("Destro&Y All Units At Cursor|Ctrl+Shift+D",
                    new Shortcut(KeyboardKey.D, ctrl: true, shift: true), KeyboardKey.Y),
                new MenuElement("Change Money|Shift+F9", new Shortcut(KeyboardKey.F9, shift: true),
                    KeyboardKey.Null),
                new MenuElement("Edit Unit|Ctrl+Shift+U", new Shortcut(KeyboardKey.U, ctrl: true, shift: true),
                    KeyboardKey.Null),
                new MenuElement("Edit City|Ctrl+Shift+C", new Shortcut(KeyboardKey.C, ctrl: true, shift: true),
                    KeyboardKey.Null),
                new MenuElement("Edit King|Ctrl+Shift+K", new Shortcut(KeyboardKey.K, ctrl: true, shift: true),
                    KeyboardKey.Null),
                new MenuElement("Scenario Parameters|Ctrl+Shift+P",
                    new Shortcut(KeyboardKey.P, ctrl: true, shift: true), KeyboardKey.Null),
                new MenuElement("Save As Scenario|Ctrl+Shift+S",
                    new Shortcut(KeyboardKey.S, ctrl: true, shift: true), KeyboardKey.Null)
            }
        },

        new MenuDetails
        {
            Key = "MAP", Defaults = new List<MenuElement>
            {
                new MenuElement("&Map", Shortcut.None, KeyboardKey.M),
                new MenuElement("Toggle Cheat Mode|Ctrl+K", new Shortcut(KeyboardKey.K, ctrl: true),
                    KeyboardKey.Null),
                new MenuElement("&Select Map|Ctrl+Shift+1",
                    new Shortcut(KeyboardKey.One, ctrl: true, shift: true), KeyboardKey.S),
                new MenuElement("&Import Map|Ctrl+Shift+2",
                    new Shortcut(KeyboardKey.Two, ctrl: true, shift: true), KeyboardKey.I),
                new MenuElement("&Export Map|Ctrl+Shift+3",
                    new Shortcut(KeyboardKey.Three, ctrl: true, shift: true), KeyboardKey.E),
            }
        },

        new MenuDetails
        {
            Key = "PEDIA", Defaults = new List<MenuElement>
            {
                new MenuElement("&Civilopedia", Shortcut.None, KeyboardKey.C),
                new MenuElement("Civilization &Advances", Shortcut.None, KeyboardKey.A),
                new MenuElement("City &Improvements", Shortcut.None, KeyboardKey.I),
                new MenuElement("&Wonders of the World", Shortcut.None, KeyboardKey.W),
                new MenuElement("Military &Units", Shortcut.None, KeyboardKey.U),
                new MenuElement("&Governments", Shortcut.None, KeyboardKey.G),
                new MenuElement("&Terrain Types", Shortcut.None, KeyboardKey.T),
                new MenuElement("Game &Concepts", Shortcut.None, KeyboardKey.C),
                new MenuElement("&About Test of Time", Shortcut.None, KeyboardKey.A)
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
    
    public override Dictionary<string, List<ImageProps>> UnitPicProps { get; set; }
    public override Dictionary<string, List<ImageProps>> CitiesPicProps { get; set; }
    public override Dictionary<string, List<ImageProps>> TilePicProps { get; set; }
    public override Dictionary<string, List<ImageProps>> OverlayPicProps { get; set; }
    public override Dictionary<string, List<ImageProps>> IconsPicProps { get; set; }

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
                var imageColours = Raylib.LoadImageColors(CitiesPicProps["textColors"][col].Image);
                var textColour = imageColours[2 * CitiesPicProps["textColors"][col].Image.Width + 0];
                var shieldColour = imageColours[2 * CitiesPicProps["textColors"][col].Image.Width + 0];
                textColour.A = 255; // to avoid any upper-left-pixel transparency issues
                shieldColour.A = 255;
                Raylib.UnloadImageColors(imageColours);

                // This is not exact, but a good aproximation what TOT does with shield colours
                var lightColour = new Color(shieldColour.R / 2, shieldColour.G / 2, shieldColour.B / 2, 255);
                var darkColour = new Color(shieldColour.R / 4, shieldColour.G / 4, shieldColour.B / 4, 255);

                playerColours[col] = new PlayerColour
                {
                    Normal = CitiesPicProps["flags"][col].Image,
                    FlagTexture = Raylib.LoadTextureFromImage(CitiesPicProps["flags"][col].Image),
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
        Color replacementColour = new(255, 0, 255, 0);

        var shield = UnitPicProps["HPshield"][0].Image;
        var shieldFront = Raylib.ImageCopy(shield);

        UnitImages.Shields = new MemoryStorage(shieldFront, "Unit-Shield", replacementColour);
        UnitImages.ShieldBack = new MemoryStorage(shield, "Unit-Shield-Back", replacementColour, true);
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
        OrderTextHeight = UnitPicProps["HPshield"][0].Image.Height - 1
    };

    public override void DrawBorderWallpaper(Wallpaper wp, ref Image destination, int height, int width, Padding padding)
    {
        var topLeft = padding.Top == 12 ? wp.OuterThinTopLeft : wp.OuterTitleTopLeft;
        var top = padding.Top == 12 ? wp.OuterThinTop : wp.OuterTitleTop;
        var topRight = padding.Top == 12 ? wp.OuterThinTopRight : wp.OuterTitleTopRight;

        // Top border
        Raylib.ImageDraw(ref destination, topLeft, new Rectangle(0, 0, topLeft.Width, topLeft.Height), new Rectangle(0, 0, topLeft.Width, topLeft.Height), Color.White);
        var topCols = (width - topLeft.Width - topRight.Width) / top.Width + 1;
        for (int col = 0; col < topCols; col++)
        {
            Raylib.ImageDraw(ref destination, top, new Rectangle(0, 0, top.Width, top.Height), new Rectangle(topLeft.Width + top.Width * col, 0, top.Width, top.Height), Color.White);
        }
        Raylib.ImageDraw(ref destination, topRight, new Rectangle(0, 0, topRight.Width, topRight.Height), new Rectangle(width - topRight.Width, 0, topRight.Width, topRight.Height), Color.White);

        // Left-right border
        var sideRows = (height - topLeft.Height - wp.OuterBottomLeft.Height) / wp.OuterLeft.Height + 1;
        for (int row = 0; row < sideRows; row++)
        {
            Raylib.ImageDraw(ref destination, wp.OuterLeft, new Rectangle(0, 0, wp.OuterLeft.Width, wp.OuterLeft.Height), new Rectangle(0, topLeft.Height + wp.OuterLeft.Height * row, wp.OuterLeft.Width, wp.OuterLeft.Height), Color.White);
            Raylib.ImageDraw(ref destination, wp.OuterRight, new Rectangle(0, 0, wp.OuterRight.Width, wp.OuterRight.Height), new Rectangle(width - wp.OuterRight.Width, topRight.Height + wp.OuterRight.Height * row, wp.OuterRight.Width, wp.OuterRight.Height), Color.White);
        }

        // Bottom border
        Raylib.ImageDraw(ref destination, wp.OuterBottomLeft, new Rectangle(0, 0, wp.OuterBottomLeft.Width, wp.OuterBottomLeft.Height), new Rectangle(0, height - wp.OuterBottomLeft.Height, wp.OuterBottomLeft.Width, wp.OuterBottomLeft.Height), Color.White);
        var btmCols = (width - wp.OuterBottomLeft.Width - wp.OuterBottomRight.Width) / wp.OuterBottom.Width + 1;
        for (int col = 0; col < btmCols; col++)
        {
            Raylib.ImageDraw(ref destination, wp.OuterBottom, new Rectangle(0, 0, wp.OuterBottom.Width, wp.OuterBottom.Height), new Rectangle(wp.OuterBottomLeft.Width + wp.OuterBottom.Width * col, height - wp.OuterBottom.Height, wp.OuterBottom.Width, wp.OuterBottom.Height), Color.White);
        }
        Raylib.ImageDraw(ref destination, wp.OuterBottomRight, new Rectangle(0, 0, wp.OuterBottomRight.Width, wp.OuterBottomRight.Height), new Rectangle(width - wp.OuterBottomRight.Width, height - wp.OuterBottomRight.Height, wp.OuterBottomRight.Width, wp.OuterBottomRight.Height), Color.White);
    }

    public override void DrawBorderLines(ref Image destination, int height, int width, Padding padding) { }
}