using Civ2;
using Civ2.Dialogs;
using Civ2engine.IO;
using JetBrains.Annotations;
using Model;
using Model.Controls;
using Model.Images;
using Model.ImageSets;
using Model.Input;
using Model.Interface;
using Model.Utils;
using Raylib_CSharp.Colors;
using Raylib_CSharp.Images;
using Raylib_CSharp.Interact;
using Raylib_CSharp.Rendering;
using Raylib_CSharp.Textures;
using Raylib_CSharp.Transformations;
using RaylibUtils;
using static Model.Controls.CommandIds;

namespace TOT;

/// <summary>
/// This class is dynamically instantiated when ToT files are detected
/// </summary>
[UsedImplicitly]
public class TestOfTimeInterface(IMain main) : Civ2Interface(main)
{
    public override string Title => "Test of Time";

    public override string InitialMenu => "STARTMENU";

    public override InterfaceStyle Look { get; } = new()
    {
        OuterTitleTop = Enumerable.Range(0, 6).Select(col => new BitmapStorage("dialog", 1 + 59 * col, 94, 58, 28)).ToArray<IImageSource>(),
        OuterThinTop = Enumerable.Range(0, 6).Select(col => new BitmapStorage("dialog", 1 + 59 * col, 123, 58, 12)).ToArray<IImageSource>(),
        OuterBottom = Enumerable.Range(0, 6).Select(col => new BitmapStorage("dialog", 1 + 59 * col, 136, 58, 11)).ToArray<IImageSource>(),
        OuterMiddle = Enumerable.Range(0, 6).Select(col => new BitmapStorage("dialog", 1 + 59 * col, 148, 58, 13)).ToArray<IImageSource>(),
        OuterLeft = Enumerable.Range(0, 6).Select(col => new BitmapStorage("dialog", 112 + 12 * col, 168, 11, 29)).ToArray<IImageSource>(),
        OuterRight = Enumerable.Range(0, 6).Select(col => new BitmapStorage("dialog", 112 + 13 * col, 198, 12, 29)).ToArray<IImageSource>(),
        OuterTitleTopLeft = new BitmapStorage("dialog", 1, 168, 14, 30),
        OuterTitleTopRight = new BitmapStorage("dialog", 16, 168, 14, 30),
        OuterThinTopLeft = new BitmapStorage("dialog", 1, 199, 14, 14),
        OuterThinTopRight = new BitmapStorage("dialog", 16, 199, 14, 14),
        OuterMiddleLeft = new BitmapStorage("dialog", 1, 214, 14, 18),
        OuterMiddleRight = new BitmapStorage("dialog", 16, 214, 14, 18),
        OuterBottomLeft = new BitmapStorage("dialog", 1, 233, 14, 14),
        OuterBottomRight = new BitmapStorage("dialog", 16, 233, 14, 14),

        Inner = Enumerable.Range(0, 6).Select(col => new BitmapStorage("dialog", new Rectangle(1 + 93 * col, 1, 92, 92))).ToArray<IImageSource>(),
        InnerAlt = new BitmapStorage("ICONS", new Rectangle(298, 190, 32, 32)),

        Button = Enumerable.Range(0, 14).Select(col => new BitmapStorage("dialog", new Rectangle(449 + 17 * col, 94, 16, 30))).ToArray<IImageSource>(),
        ButtonClicked = Enumerable.Range(0, 14).Select(col => new BitmapStorage("dialog", new Rectangle(449 + 17 * col, 125, 16, 30))).ToArray<IImageSource>(),

        RadioButtons =
        [
            new BitmapStorage("dialog", 903, 94, 33, 33), 
          new BitmapStorage("dialog", 869, 94, 33, 33)
        ],
        CheckBoxes =
        [
            new BitmapStorage("dialog", 805, 94, 29, 29),
          new BitmapStorage("dialog", 775, 94, 29, 29)
        ],

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
        LabelFontSize = 25,
        CityWindowFont = Fonts.Arial,
        CityWindowFontSize = 16,
        MenuFont = Fonts.Arial,
        MenuFontSize = 14,
        StatusPanelLabelFont = Fonts.TnRbold,
        StatusPanelLabelColor = new Color(189, 189, 189, 255),
        StatusPanelLabelColorShadow = Color.Black,
        MovingUnitsViewingPiecesLabelColor = new Color(189, 189, 189, 255),
        MovingUnitsViewingPiecesLabelColorShadow = Color.Black,
    };

    public override bool IsButtonInOuterPanel => false;

    protected override IEnumerable<Ruleset> GenerateRulesets(string path, string title)
    {

        var original = "Original";

        // TODO: This method looks really scrambled... why do we no longer look in the root or at these extra folders??? something is wrong here!!
        // var other_default_game_modes = new[] { "SciFi", "Fantasy" };

        var originalPath = Path.Combine(path, original);

        var originalERxists = Directory.Exists(originalPath);
        var rules = FileUtilities.GetFile(path, "rules.txt");
        if (rules != null)
        {
            yield return new Ruleset(title, new Dictionary<string, string>
            {
                { "Test-Of-Time", "Original" }
            }, originalPath);

            foreach (var subdirectory in Directory.EnumerateDirectories(path))
            {
                var scnRules = FileUtilities.GetFile(subdirectory, "rules.txt");
                if (scnRules != null)
                {
                    var name = "";
                    
                    var game = FileUtilities.GetFile( subdirectory,"game.txt");
                    if (game != null)
                    {
                        foreach (var line in File.ReadLines(game))
                        {
                            if (!line.StartsWith("@title")) continue;
                            name = line[7..] + " - " + Path.GetFileName(subdirectory);
                            break;
                        }
                    }

                    if (string.IsNullOrWhiteSpace(name))
                    {
                        name = Path.GetFileName(subdirectory);
                    }

                    if (originalERxists && subdirectory.Equals(originalPath))
                    {
                        //yield return new Ruleset (name, new Dictionary<string, string>
                        //{

                        //    //{ "TOT-Scenario", subdirectory }
                        //    { "TOT-Scenario", Path.GetFileName(subdirectory) }
                        //}, subdirectory, path);
                    }
                    else
                    {
                        yield return new Ruleset(name, new Dictionary<string, string>
                        {
                            //{ "TOT-Scenario", subdirectory }
                            { "TOT-Scenario", Path.GetFileName(subdirectory) }
                        }, subdirectory, originalPath);
                    }
                }
            }
        }
    }
    

    public override Padding GetPadding(float headerLabelHeight, bool footer)
    {
        var paddingTop = headerLabelHeight != 0 ? 28 : 12;

        return new Padding(paddingTop, bottom: 11, left: 11, right: 12);
    }
    public override Padding DialogPadding => new(12, bottom: 11, left: 11, right: 12);


    public override void Initialize()
    {
        base.Initialize();

        if (Dialogs.TryGetValue(MainMenu.Title + "2", out var menu2))
        {
            //var existingDialog = DialogHandlers[MainMenu.Title].Dialog;
            //existingDialog.Options.Texts = menu2.Options.Concat(existingDialog.Options.Texts.Skip(5)).ToList();
            var existingDialog = DialogHandlers[MainMenu.Title].Dialog;
            if (existingDialog?.Options != null && menu2?.Options != null)
                existingDialog.Options.Texts = menu2.Options.Concat(existingDialog.Options.Texts.Skip(5)).ToList();
        }


        PicSources.Add("unit",
            Enumerable.Range(0, 9 * UnitsRows).Select(i => new BitmapStorage("UNITS",
                    new Rectangle(1 + 65 * (i % 9), 1 + (UnitsPxHeight + 1) * (i / 9), 64, UnitsPxHeight), true, true))
                .ToArray<IImageSource>());
        PicSources.Add("HPshield", [new BitmapStorage("UNITS", new Rectangle(586, 1, 32, 10), true)]);
        PicSources.Add("textColours", Enumerable.Range(0, 9).Select(col =>
            new BitmapStorage("CITIES", new Rectangle(1 + 15 * col, 421, 14, 3), true)).ToArray<IImageSource>());
        PicSources.Add("flags", Enumerable.Range(0, 2 * 9).Select(i =>
                new BitmapStorage("CITIES", new Rectangle(1 + 15 * (i % 9), 425 + 23 * (i / 9), 14, 22), true))
            .ToArray<IImageSource>());
        PicSources.Add("fortify", [new BitmapStorage("CITIES", new Rectangle(143, 423, 64, 48), true)]);
        PicSources.Add("fortress", [new BitmapStorage("CITIES", new Rectangle(208, 423, 64, 48), true)]);
        PicSources.Add("airbase,empty", [new BitmapStorage("CITIES", new Rectangle(273, 423, 64, 48), true)]);
        PicSources.Add("airbase,full", [new BitmapStorage("CITIES", new Rectangle(338, 423, 64, 48), true)]);
        PicSources.Add("base1", Enumerable.Range(0, 11).Select(row =>
            new BitmapStorage("TERRAIN1", new Rectangle(1, 1 + 33 * row, 64, 32), true)).ToArray<IImageSource>());
        PicSources.Add("base2", Enumerable.Range(0, 11).Select(row =>
            new BitmapStorage("TERRAIN1", new Rectangle(66, 1 + 33 * row, 64, 32), true)).ToArray<IImageSource>());
        PicSources.Add("special1", Enumerable.Range(0, 11).Select(row =>
            new BitmapStorage("TERRAIN1", new Rectangle(131, 1 + 33 * row, 64, 32), true)).ToArray<IImageSource>());
        PicSources.Add("special2", Enumerable.Range(0, 11).Select(row =>
            new BitmapStorage("TERRAIN1", new Rectangle(196, 1 + 33 * row, 64, 32), true)).ToArray<IImageSource>());
        PicSources.Add("road", Enumerable.Range(0, 9).Select(col =>
            new BitmapStorage("TERRAIN1", new Rectangle(1 + 65 * col, 364, 64, 32), true)).ToArray<IImageSource>());
        PicSources.Add("railroad", Enumerable.Range(0, 9).Select(col =>
            new BitmapStorage("TERRAIN1", new Rectangle(1 + 65 * col, 397, 64, 32), true)).ToArray<IImageSource>());
        PicSources.Add("irrigation", [new BitmapStorage("TERRAIN1", new Rectangle(456, 100, 64, 32), true)]);
        PicSources.Add("farmland", [new BitmapStorage("TERRAIN1", new Rectangle(456, 133, 64, 32), true)]);
        PicSources.Add("mine", [new BitmapStorage("TERRAIN1", new Rectangle(456, 166, 64, 32), true)]);
        PicSources.Add("pollution", [new BitmapStorage("TERRAIN1", new Rectangle(456, 199, 64, 32), true)]);
        PicSources.Add("shield", [new BitmapStorage("TERRAIN1", new Rectangle(456, 232, 64, 32), true)]);
        PicSources.Add("hut", [new BitmapStorage("TERRAIN1", new Rectangle(456, 265, 64, 32), true)]);
        PicSources.Add("dither", [new BitmapStorage("TERRAIN1", new Rectangle(1, 447, 64, 32), true)]);
        PicSources.Add("blank", [new BitmapStorage("TERRAIN1", new Rectangle(131, 447, 64, 32), true)]);
        PicSources.Add("connection", Enumerable.Range(0, 2 * 8).Select(i =>
                new BitmapStorage("TERRAIN2", new Rectangle(1 + 65 * (i % 8), 1 + 33 * (i / 8), 64, 32), true))
            .ToArray<IImageSource>());
        PicSources.Add("river", Enumerable.Range(0, 2 * 8).Select(i =>
                new BitmapStorage("TERRAIN2", new Rectangle(1 + 65 * (i % 8), 67 + 33 * (i / 8), 64, 32), true))
            .ToArray<IImageSource>());
        PicSources.Add("forest", Enumerable.Range(0, 2 * 8).Select(i =>
                new BitmapStorage("TERRAIN2", new Rectangle(1 + 65 * (i % 8), 133 + 33 * (i / 8), 64, 32), true))
            .ToArray<IImageSource>());
        PicSources.Add("mountain", Enumerable.Range(0, 2 * 8).Select(i =>
                new BitmapStorage("TERRAIN2", new Rectangle(1 + 65 * (i % 8), 199 + 33 * (i / 8), 64, 32), true))
            .ToArray<IImageSource>());
        PicSources.Add("hill", Enumerable.Range(0, 2 * 8).Select(i =>
                new BitmapStorage("TERRAIN2", new Rectangle(1 + 65 * (i % 8), 265 + 33 * (i / 8), 64, 32), true))
            .ToArray<IImageSource>());
        PicSources.Add("riverMouth", Enumerable.Range(0, 4).Select(col =>
            new BitmapStorage("TERRAIN2", new Rectangle(1 + 65 * col, 331, 64, 32), true)).ToArray<IImageSource>());
        PicSources.Add("viewPiece", [new BitmapStorage("ICONS", new Rectangle(199, 256, 64, 32), true)]);
        PicSources.Add("gridlines", [new BitmapStorage("ICONS", new Rectangle(183, 430, 64, 32), true)]);
        PicSources.Add("gridlines,visible", [new BitmapStorage("ICONS", new Rectangle(248, 430, 64, 32), true)]);
        PicSources.Add("battleAnim", Enumerable.Range(0, 8).Select(col =>
            new BitmapStorage("ICONS", new Rectangle(1 + 33 * col, 356, 32, 32), true)).ToArray<IImageSource>());
        PicSources.Add("researchProgress", Enumerable.Range(0, 4).Select(col =>
            new BitmapStorage("ICONS", new Rectangle(49 + 15 * col, 290, 14, 14), true)).ToArray<IImageSource>());
        PicSources.Add("globalWarming", Enumerable.Range(0, 4).Select(col =>
            new BitmapStorage("ICONS", new Rectangle(49 + 15 * col, 305, 14, 14), true)).ToArray<IImageSource>());
        PicSources.Add("close", [new BitmapStorage("ICONS", new Rectangle(1, 389, 16, 16))]);
        PicSources.Add("zoomIn", [new BitmapStorage("ICONS", new Rectangle(18, 389, 16, 16))]);
        PicSources.Add("zoomOut", [new BitmapStorage("ICONS", new Rectangle(35, 389, 16, 16))]);
        PicSources.Add("backgroundImage", [new BinaryStorage("Tiles.dll", 0x100740, 0x12702)]);
        PicSources.Add("backgroundImageSmall1", [
            new BinaryStorage("Tiles.dll", 0xF5C44, 0xAAFC, new Rectangle(332, 134, 64, 64))
        ]);
        PicSources.Add("backgroundImageSmall2", [
            new BinaryStorage("Tiles.dll", 0xF5C44, 0xAAFC, new Rectangle(398, 134, 64, 64))
        ]);
        PicSources.Add("cityBuiltAncient", [new BinaryStorage("Tiles.dll", 0xE3D1C, 0x5A34)]);
        PicSources.Add("cityBuiltModern", [new BinaryStorage("Tiles.dll", 0xE9750, 0x5C2D)]);
        PicSources.Add("observatoryPic", [new BinaryStorage("Intro.dll", 0x1E630, 0xACDC)]);
        PicSources.Add("horzionPic", [new BinaryStorage("Intro.dll", 0x1E630, 0xACDC)]);
        PicSources.Add("creaturePic", [new BinaryStorage("Intro.dll", 0x2EBB4, 0x14600)]);
        PicSources.Add("duelPic", [new BinaryStorage("Intro.dll", 0x431B4, 0x177AF)]);
        PicSources.Add("knightsPic", [new BinaryStorage("Intro.dll", 0x5A964, 0xEB67)]);
        PicSources.Add("labPic", [new BinaryStorage("Intro.dll", 0x694CC, 0xFC4B)]);
        PicSources.Add("templePic", [new BinaryStorage("Intro.dll", 0x79118, 0x10EBB)]);
        PicSources.Add("manorPic", [new BinaryStorage("Intro.dll", 0x89FD4, 0x13AA8)]);
        PicSources.Add("bookPic", [new BinaryStorage("Intro.dll", 0x9DA7C, 0x162AE)]);
        PicSources.Add("alienplanetPic", [new BinaryStorage("Intro.dll", 0xB3D2C, 0x988D)]);
        PicSources.Add("rocksPic", [new BinaryStorage("Intro.dll", 0xBD5BC, 0xBF78)]);
        PicSources.Add("shipPic", [new BinaryStorage("Intro.dll", 0xC9534, 0x5A03)]);
        PicSources.Add("hotplanetPic", [new BinaryStorage("Intro.dll", 0xCEF38, 0x9130)]);


        var src = new IImageSource[6 * 8];
        for (var row = 0; row < 6; row++)
        {
            for (var col = 0; col < 4; col++)
            {
                src[8 * row + col] = new BitmapStorage("CITIES", new Rectangle(1 + 65 * col, 39 + 49 * row, 64, 48),
                    true, true); // Open cities
                src[8 * row + 4 + col] = new BitmapStorage("CITIES",
                    new Rectangle(334 + 65 * col, 39 + 49 * row, 64, 48), true, true); // Walled cities
            }
        }

        PicSources.Add("city", src);

        src = new IImageSource[4 * 8];
        for (var i = 0; i < 8; i++)
        {
            src[4 * i + 0] = new BitmapStorage("TERRAIN2", new Rectangle(1 + 66 * i, 429, 32, 16), true);
            src[4 * i + 1] = new BitmapStorage("TERRAIN2", new Rectangle(1 + 66 * i, 446, 32, 16), true);
            src[4 * i + 2] = new BitmapStorage("TERRAIN2", new Rectangle(1 + 66 * i, 463, 32, 16), true);
            src[4 * i + 3] = new BitmapStorage("TERRAIN2", new Rectangle(34 + 66 * i, 463, 32, 16), true);
        }

        PicSources.Add("coastline", src);


        DialogHandlers["STARTMENU"].Dialog.Decorations
            .Add(new Decoration(PicSources["observatoryPic"][0], new Point(0.08, 0.09)));
        DialogHandlers["MAINMENU"].Dialog.Decorations
            .Add(new Decoration(PicSources["observatoryPic"][0], new Point(0.08, 0.09)));
        DialogHandlers["SIZEOFMAP"].Dialog.Decorations
            .Add(new Decoration(PicSources["horzionPic"][0], new Point(0.08, 0.09)));
        DialogHandlers["DIFFICULTY"].Dialog.Decorations
            .Add(new Decoration(PicSources["creaturePic"][0], new Point(0.08, 0.09)));
        DialogHandlers["ENEMIES"].Dialog.Decorations
            .Add(new Decoration(PicSources["duelPic"][0], new Point(0.08, 0.09)));
        DialogHandlers["BARBARITY"].Dialog.Decorations
            .Add(new Decoration(PicSources["knightsPic"][0], new Point(0.08, 0.09)));
        DialogHandlers["RULES"].Dialog.Decorations.Add(new Decoration(PicSources["bookPic"][0], new Point(0.08, 0.09)));
        DialogHandlers["ADVANCED"].Dialog.Decorations
            .Add(new Decoration(PicSources["bookPic"][0], new Point(0.08, 0.09)));
        //DialogHandlers["OPPONENT"].Dialog.Decorations.Add(new Decoration(PicSources["bookPic"][0], new Point(0.08, 0.09)));
        DialogHandlers["ACCELERATED"].Dialog.Decorations
            .Add(new Decoration(PicSources["bookPic"][0], new Point(0.08, 0.09)));
        DialogHandlers["GENDER"].Dialog.Decorations.Add(new Decoration(PicSources["labPic"][0], new Point(0.08, 0.09)));
        DialogHandlers["TRIBE"].Dialog.Decorations
            .Add(new Decoration(PicSources["templePic"][0], new Point(0.08, 0.09)));
        DialogHandlers["CUSTOMTRIBE"].Dialog.Decorations
            .Add(new Decoration(PicSources["templePic"][0], new Point(0.08, 0.09)));
        DialogHandlers["CUSTOMTRIBE2"].Dialog.Decorations
            .Add(new Decoration(PicSources["templePic"][0], new Point(0.08, 0.09)));
        DialogHandlers["CUSTOMCITY"].Dialog.Decorations
            .Add(new Decoration(PicSources["manorPic"][0], new Point(0.08, 0.09)));
        DialogHandlers["CUSTOMFORM"].Dialog.Decorations
            .Add(new Decoration(PicSources["shipPic"][0], new Point(0.08, 0.09)));
        DialogHandlers["CUSTOMCLIMATE"].Dialog.Decorations
            .Add(new Decoration(PicSources["rocksPic"][0], new Point(0.08, 0.09)));
        DialogHandlers["CUSTOMTEMP"].Dialog.Decorations
            .Add(new Decoration(PicSources["hotplanetPic"][0], new Point(0.08, 0.09)));
        DialogHandlers["CUSTOMAGE"].Dialog.Decorations
            .Add(new Decoration(PicSources["alienplanetPic"][0], new Point(0.08, 0.09)));
    }

    protected override List<MenuDetails> MenuMap { get; } =
    [
        new()
        {
            Key = "GAME", Defaults = new List<MenuElement>
            {
                new("&Game", Shortcut.None, Key.G),
                new("Game &Options|Ctrl+O", new Shortcut(Key.O, ctrl: true), Key.O,
                    commandId: GameOptions),
                new("Graphic O&ptions|Ctrl+P", new Shortcut(Key.P, ctrl: true),
                    Key.P, commandId: GraphicOptions),
                new("&City Report Options|Ctrl+E", new Shortcut(Key.E, ctrl: true),
                    Key.C, commandId: CityReportOptions),
                new("M&ultiplayer Options|Ctrl+Y", new Shortcut(Key.Y, ctrl: true),
                    Key.U),
                new("-", Shortcut.None, Key.None),
                new("&Game Profile", Shortcut.None, Key.G),
                new("-", Shortcut.None, Key.None),
                new("Pick &Music", Shortcut.None, Key.M),
                new("&Save Game|Ctrl+S", new Shortcut(Key.S, ctrl: true), Key.S),
                new("&Load Game|Ctrl+L", new Shortcut(Key.L, ctrl: true), Key.L),
                new("&Join Game|Ctrl+J", new Shortcut(Key.J, ctrl: true), Key.J),
                new("-", Shortcut.None, Key.None),
                new("Set Pass&word|Ctrl+W", new Shortcut(Key.W, ctrl: true), Key.W),
                new("Change &Timer|Ctrl+T", new Shortcut(Key.T, ctrl: true), Key.T),
                new("-", Shortcut.None, Key.None),
                new("&Retire|Ctrl+R", new Shortcut(Key.R, ctrl: true), Key.R),
                new("&Quit|Ctrl+Q", new Shortcut(Key.Q, ctrl: true), Key.Q,
                    commandId: QuitGame)
            }
        },

        new()
        {
            Key = "KINGDOM", Defaults = new List<MenuElement>
            {
                new("&Kingdom", Shortcut.None, Key.K),
                new("&Tax Rate|Shift+T", new Shortcut(Key.T, shift: true), Key.T),
                new("-", Shortcut.None, Key.None),
                new("Find &City|Shift+C", new Shortcut(Key.C, shift: true), Key.C),
                new("-", Shortcut.None, Key.None),
                new("&REVOLUTION|Shift+R", new Shortcut(Key.R, shift: true), Key.R)
            }
        },


        new()
        {
            Key = "VIEW", Defaults = new List<MenuElement>
            {
                new("&View", Shortcut.None, Key.V),
                new("&Move Pieces|v", new Shortcut(Key.V), Key.M),
                new("&View Pieces|v", new Shortcut(Key.V), Key.V),
                new("-", Shortcut.None, Key.None),
                new("Zoom &In|z", new Shortcut(Key.Z), Key.I, commandId: ZoomIn),
                new("Zoom &Out|X", new Shortcut(Key.X), Key.O, commandId: ZoomOut),
                new("-", Shortcut.None, Key.None),
                new("Max Zoom In|Ctrl+Z", new Shortcut(Key.Z, ctrl: true),
                    Key.None, commandId: MaxZoomIn),
                new("Standard Zoom|Shift+Z", new Shortcut(Key.Z, shift: true),
                    Key.None, commandId: StandardZoom),
                new("Medium Zoom Out|Shift+X", new Shortcut(Key.X, shift: true),
                    Key.None, commandId: MediumZoomOut),
                new("Max Zoom Out|Ctrl+X", new Shortcut(Key.X, ctrl: true),
                    Key.None, commandId: MaxZoomOut),
                new("-", Shortcut.None, Key.None),
                new("Show Map Grid|Ctrl+G", new Shortcut(Key.G, ctrl: true),
                    Key.None, commandId: ShowMapGrid),
                new("Arrange Windows", Shortcut.None, Key.None),
                new("Show Hidden Terrain|t", new Shortcut(Key.T), Key.T),
                new("&Center View|c", new Shortcut(Key.C), Key.C),
                new("Map Layout", Shortcut.None, Key.None, commandId: MapLayoutToggle),
                new("City Layout", Shortcut.None, Key.None),
            },
        },


        new()
        {
            Key = "@ORDERS", Defaults = new List<MenuElement>
            {
                new("&Orders", Shortcut.None, Key.O),
                new("&Build New City|b", new Shortcut(Key.B), Key.B, BuildCityOrder, true),
                new("Build &Road|r", new Shortcut(Key.R), Key.R, BuildRoadOrder,
                    omitIfNoCommand: true),
                new("Build &Irrigation|i", new Shortcut(Key.I), Key.I, BuildIrrigationOrder,
                    omitIfNoCommand: true),
                new("Build &Mines|m", new Shortcut(Key.M), Key.M, BuildMineOrder,
                    omitIfNoCommand: true),
                new("Build %STRING0", Shortcut.None, Key.None, BuildIrrigationOrder,
                    omitIfNoCommand: true),
                new("Transform to ...|o", new Shortcut(Key.O), Key.None),
                new("Build &Airbase|e", new Shortcut(Key.E), Key.A),
                new("Build &Teleporter|j", new Shortcut(Key.J), Key.T),
                new("Build &Fortress|f", new Shortcut(Key.F), Key.F),
                new("Automate Settler|k", new Shortcut(Key.K), Key.None),
                new("Clean Up &Pollution|p", new Shortcut(Key.P), Key.P),
                new("&Pillage|Shift+P", new Shortcut(Key.P, shift: true), Key.P),
                new("&Unload|u", new Shortcut(Key.U), Key.U),
                new("&Go To|g", new Shortcut(Key.G), Key.G),
                new("&Paradrop|p", new Shortcut(Key.P), Key.P),
                new("Air&lift|l", new Shortcut(Key.L), Key.L),
                new("Teleport|n", new Shortcut(Key.N), Key.None),
                new("Set &Home City|h", new Shortcut(Key.H), Key.H),
                new("&Fortify|f", new Shortcut(Key.F), Key.F),
                new("&Sleep|s", new Shortcut(Key.S), Key.S),
                new("&Disband|Shift+D", new Shortcut(Key.D, shift: true), Key.D),
                new("&Activate Unit|a", new Shortcut(Key.A), Key.A),
                new("&Wait|w", new Shortcut(Key.W), Key.W),
                new("S&kip Turn|SPACE", new Shortcut(Key.Space), Key.K),
                new("End Player Tur&n|Ctrl+N", new Shortcut(Key.T, shift: true),
                    Key.N, EndTurn)
            },
        },


        new()
        {
            Key = "ADVISORS", Defaults = new List<MenuElement>
            {
                new("&Advisors", Shortcut.None, Key.A),
                new("Chat with &Kings|Ctrl+C", new Shortcut(Key.C, ctrl: true),
                    Key.K),
                new("-", Shortcut.None, Key.None),
                new("&City Status|F1", new Shortcut(Key.F1), Key.C),
                new("&Defense Minister|F2", new Shortcut(Key.F2), Key.D),
                new("&Foreign Minister|F3", new Shortcut(Key.F3), Key.F),
                new("-", Shortcut.None, Key.None),
                new("&Attitude Advisor|F4", new Shortcut(Key.F4), Key.A),
                new("&Trade Advisor|F5", new Shortcut(Key.F5), Key.T),
                new("&Science Advisor|F6", new Shortcut(Key.F6), Key.S),
                new("-", Shortcut.None, Key.None),
                new("Cas&ualty Timeline|Ctrl-D", new Shortcut(Key.D, ctrl: true),
                    Key.U)
            },
        },


        new()
        {
            Key = "WORLD", Defaults = new List<MenuElement>
            {
                new("&World", Shortcut.None, Key.W),
                new("&Wonders of the World|F7", new Shortcut(Key.F7), Key.W),
                new("&Top 5 Cities|F8", new Shortcut(Key.F8), Key.T),
                new("&Civilization Score|F9", new Shortcut(Key.F9), Key.C),
                new("-", Shortcut.None, Key.None),
                new("&Demographics|F11", new Shortcut(Key.F11), Key.D),
                new("&Spaceships|F12", new Shortcut(Key.F12), Key.S)
            },
        },


        new()
        {
            Key = "CHEAT", Defaults = new List<MenuElement>
            {
                new("&Cheat", Shortcut.None, Key.C),
                new("Toggle Cheat Mode|Ctrl+K", new Shortcut(Key.K, ctrl: true),
                    Key.None),
                new("-", Shortcut.None, Key.None),
                new("Create &Unit|Shift+F1", new Shortcut(Key.F1, shift: true),
                    Key.U),
                new("Reveal &Map|Shift+F2", new Shortcut(Key.F2, shift: true),
                    Key.M, CheatRevealMapCommand),
                new("Set &Human Player|Shift+F3", new Shortcut(Key.F3, shift: true),
                    Key.H),
                new("-", Shortcut.None, Key.None),
                new("Set Game Year|Shift+F4", new Shortcut(Key.F4, shift: true),
                    Key.None),
                new("&Kill Civilization|Shift+F5", new Shortcut(Key.F5, shift: true),
                    Key.K),
                new("-", Shortcut.None, Key.None),
                new("Te&chnology Advance|Shift+F6", new Shortcut(Key.F6, shift: true),
                    Key.C),
                new("&Edit Technologies|Ctrl+Shift+F6",
                    new Shortcut(Key.F6, ctrl: true, shift: true), Key.E),
                new("Force &Government|Shift+F7", new Shortcut(Key.F7, shift: true),
                    Key.G),
                new("Change &Terrain At Cursor|Shift+F8", new Shortcut(Key.F8, shift: true),
                    Key.T),
                new("Destro&Y All Units At Cursor|Ctrl+Shift+D",
                    new Shortcut(Key.D, ctrl: true, shift: true), Key.Y),
                new("Change Money|Shift+F9", new Shortcut(Key.F9, shift: true),
                    Key.None, CheatChangeMoneyCommand),
                new("-", Shortcut.None, Key.None),
                new("Edit Unit|Ctrl+Shift+U", new Shortcut(Key.U, ctrl: true, shift: true),
                    Key.None),
                new("Edit City|Ctrl+Shift+C", new Shortcut(Key.C, ctrl: true, shift: true),
                    Key.None),
                new("Edit King|Ctrl+Shift+K", new Shortcut(Key.K, ctrl: true, shift: true),
                    Key.None),
                new("-", Shortcut.None, Key.None),
                new("Scenario Parameters|Ctrl+Shift+P",
                    new Shortcut(Key.P, ctrl: true, shift: true), Key.None),
                new("Save As Scenario|Ctrl+Shift+S",
                    new Shortcut(Key.S, ctrl: true, shift: true), Key.None),
                new("-", Shortcut.None, Key.None),
                new("Lua Console|Ctrl+Shift+9", new Shortcut(Key.D9, true, true), Key.L,
                    omitIfNoCommand: true, commandId: OpenLuaConsole)
            },
        },


        new()
        {
            Key = "MAP", Defaults = new List<MenuElement>
            {
                new("&Map", Shortcut.None, Key.M),
                new("Toggle Cheat Mode|Ctrl+K", new Shortcut(Key.K, ctrl: true),
                    Key.None),
                new("-", Shortcut.None, Key.None),
                new("&Select Map|Ctrl+Shift+1",
                    new Shortcut(Key.D1, ctrl: true, shift: true), Key.S),
                new("&Import Map|Ctrl+Shift+2",
                    new Shortcut(Key.D2, ctrl: true, shift: true), Key.I),
                new("&Export Map|Ctrl+Shift+3",
                    new Shortcut(Key.D3, ctrl: true, shift: true), Key.E),
            }
        },


        new()
        {
            Key = "PEDIA", Defaults = new List<MenuElement>
            {
                new("&Civilopedia", Shortcut.None, Key.C),
                new("Civilization &Advances", Shortcut.None, Key.A),
                new("City &Improvements", Shortcut.None, Key.I),
                new("&Wonders of the World", Shortcut.None, Key.W),
                new("Military &Units", Shortcut.None, Key.U),
                new("-", Shortcut.None, Key.None),
                new("&Governments", Shortcut.None, Key.G),
                new("&Terrain Types", Shortcut.None, Key.T),
                new("-", Shortcut.None, Key.None),
                new("Game &Concepts", Shortcut.None, Key.C),
                new("-", Shortcut.None, Key.None),
                new("&About Test of Time", Shortcut.None, Key.A)
            },
        }
    ];

    public override int UnitsRows => 9;
    public override int UnitsPxHeight => 64;

    public override Dictionary<string, IImageSource[]> PicSources { get; } = new();

    public override ListboxLooks GetListboxLooks(ListboxType? type)
    {
        return type switch
        {
            ListboxType.Default => new ListboxLooks
            {
                Font = Fonts.Tnr,
                FontSize = 12,
                TextColorFront = Color.Black,
                BoxBackgroundColor = new Color(67, 67, 67, 255)
            },
            _ => new ListboxLooks(),
        };
    }

    public override void LoadPlayerColours()
    {
        var playerColours = new PlayerColour[9];
        for (var col = 0; col < 9; col++)
        {
            var imageColours = Images.ExtractBitmap(PicSources["textColours"][col], this).LoadColors();
            var textColour = imageColours[2 * Images.ExtractBitmap(PicSources["textColours"][col], this).Width + 0];
            var shieldColour = imageColours[2 * Images.ExtractBitmap(PicSources["textColours"][col], this).Width + 0];
            textColour.A = 255; // to avoid any upper-left-pixel transparency issues
            shieldColour.A = 255;
            Image.UnloadColors(imageColours);

            // This is not exact, but a good approximation of what TOT does with shield colours
            var lightColour = new Color((byte)(shieldColour.R / 2), (byte)(shieldColour.G / 2), (byte)(shieldColour.B / 2), 255);
            var darkColour = new Color((byte)(shieldColour.R / 4), (byte)(shieldColour.G / 4), (byte)(shieldColour.B / 4), 255);

            playerColours[col] = new PlayerColour
            {
                Image = PicSources["flags"][col],
                TextColour = textColour,
                LightColour = lightColour,
                DarkColour = darkColour
            };
            Images.ExtractBitmap(PicSources["flags"][col], this);
        }
        PlayerColours = playerColours;
    }

    public override void GetShieldImages()
    {
        Color replacementColour = new(255, 0, 255, 0);

        var shield = Images.ExtractBitmap(PicSources["HPshield"][0], this);
        var shieldFront = shield.Copy();

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
        HPbarColours = [new Color(247, 0, 0, 255), new Color(255, 222, 74, 255), new Color(82, 173, 33, 255)],
        HPbarSizeForColours = [5, 13],
        OrderOffset = new(9 / 2f, 1),
        OrderTextHeight = Images.ExtractBitmap(PicSources["HPshield"][0], this).Height - 1
    };

    public override void DrawBorderWallpaper(Wallpaper wp, ref Image destination, int height, int width, Padding padding, bool statusPanel)
    {
        var topLeft = padding.Top == 12 ? wp.OuterThinTopLeft : wp.OuterTitleTopLeft;
        var top = padding.Top == 12 ? wp.OuterThinTop : wp.OuterTitleTop;
        var topRight = padding.Top == 12 ? wp.OuterThinTopRight : wp.OuterTitleTopRight;

        var rnd = new Random();
        var len = top.Length;

        // Top border
        destination.Draw(topLeft, new Rectangle(0, 0, topLeft.Width, topLeft.Height), new Rectangle(0, 0, topLeft.Width, topLeft.Height), Color.White);
        var topCols = (width - topLeft.Width - topRight.Width) / top[0].Width + 1;
        for (var col = 0; col < topCols; col++)
        {
            destination.Draw(top[rnd.Next(len)], new Rectangle(0, 0, top[0].Width, top[0].Height), new Rectangle(topLeft.Width + top[0].Width * col, 0, top[0].Width, top[0].Height), Color.White);
        }
        destination.Draw(topRight, new Rectangle(0, 0, topRight.Width, topRight.Height), new Rectangle(width - topRight.Width, 0, topRight.Width, topRight.Height), Color.White);

        // Left-right border
        var sideRows = (height - topLeft.Height - wp.OuterBottomLeft.Height) / wp.OuterLeft[0].Height + 1;
        for (var row = 0; row < sideRows; row++)
        {
            destination.Draw(wp.OuterLeft[rnd.Next(len)], new Rectangle(0, 0, wp.OuterLeft[0].Width, wp.OuterLeft[0].Height), new Rectangle(0, topLeft.Height + wp.OuterLeft[0].Height * row, wp.OuterLeft[0].Width, wp.OuterLeft[0].Height), Color.White);
            destination.Draw(wp.OuterRight[rnd.Next(len)], new Rectangle(0, 0, wp.OuterRight[0].Width, wp.OuterRight[0].Height), new Rectangle(width - wp.OuterRight[0].Width, topRight.Height + wp.OuterRight[0].Height * row, wp.OuterRight[0].Width, wp.OuterRight[0].Height), Color.White);
        }

        // Bottom border
        destination.Draw(wp.OuterBottomLeft, new Rectangle(0, 0, wp.OuterBottomLeft.Width, wp.OuterBottomLeft.Height), new Rectangle(0, height - wp.OuterBottomLeft.Height, wp.OuterBottomLeft.Width, wp.OuterBottomLeft.Height), Color.White);
        var btmCols = (width - wp.OuterBottomLeft.Width - wp.OuterBottomRight.Width) / wp.OuterBottom[0].Width + 1;
        for (var col = 0; col < btmCols; col++)
        {
            destination.Draw(wp.OuterBottom[rnd.Next(len)], new Rectangle(0, 0, wp.OuterBottom[0].Width, wp.OuterBottom[0].Height), new Rectangle(wp.OuterBottomLeft.Width + wp.OuterBottom[0].Width * col, height - wp.OuterBottom[0].Height, wp.OuterBottom[0].Width, wp.OuterBottom[0].Height), Color.White);
        }
        destination.Draw(wp.OuterBottomRight, new Rectangle(0, 0, wp.OuterBottomRight.Width, wp.OuterBottomRight.Height), new Rectangle(width - wp.OuterBottomRight.Width, height - wp.OuterBottomRight.Height, wp.OuterBottomRight.Width, wp.OuterBottomRight.Height), Color.White);

        if (statusPanel)
        {
            destination.Draw(wp.OuterMiddleLeft, new Rectangle(0, 0, wp.OuterMiddleLeft.Width, wp.OuterMiddleLeft.Height), new Rectangle(0, 85, wp.OuterMiddleLeft.Width, wp.OuterMiddleLeft.Height), Color.White);
            var mdlCols = (width - wp.OuterMiddleLeft.Width - wp.OuterMiddleRight.Width) / wp.OuterMiddle[0].Width;
            for (var col = 0; col < mdlCols; col++)
            {
                destination.Draw(wp.OuterMiddle[rnd.Next(len)], new Rectangle(0, 0, wp.OuterMiddle[0].Width, wp.OuterMiddle[0].Height), new Rectangle(wp.OuterMiddleLeft.Width + wp.OuterMiddle[0].Width * col, 88, wp.OuterMiddle[0].Width, wp.OuterMiddle[0].Height), Color.White);
            }
            var leftToDraw = width - mdlCols * wp.OuterMiddle[0].Width - wp.OuterMiddleLeft.Width - wp.OuterMiddleRight.Width;
            destination.Draw(wp.OuterMiddle[rnd.Next(len)], new Rectangle(0, 0, leftToDraw, wp.OuterMiddle[0].Height), new Rectangle(wp.OuterMiddleLeft.Width + mdlCols * wp.OuterMiddle[0].Width, 88, leftToDraw, wp.OuterMiddle[0].Height), Color.White);
            destination.Draw(wp.OuterMiddleRight, new Rectangle(0, 0, wp.OuterMiddleRight.Width, wp.OuterMiddleRight.Height), new Rectangle(width - wp.OuterMiddleRight.Width, 85, wp.OuterMiddleRight.Width, wp.OuterMiddleRight.Height), Color.White);
        }
    }

    public override void DrawBorderLines(ref Image destination, int height, int width, Padding padding, bool statusPanel) { }

    public override void DrawButton(Texture2D texture, Rectangle bounds)
    {
        Graphics.DrawTexture(texture, (int)bounds.X, (int)bounds.Y, Color.White);
    }
}