using System.Numerics;
using System.Runtime.Intrinsics.X86;
using Civ2;
using Civ2.Dialogs;
using Civ2engine;
using Civ2engine.Enums;
using Civ2engine.IO;
using JetBrains.Annotations;
using Model;
using Model.Images;
using Model.ImageSets;
using Model.Interface;
using Model.Menu;
using Raylib_CSharp.Transformations;
using Raylib_CSharp.Images;
using Raylib_CSharp.Colors;
using Raylib_CSharp.Interact;
using Raylib_CSharp.Textures;
using RaylibUtils;
using static Model.Menu.CommandIds;
using Raylib_CSharp.Rendering;

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
        OuterTitleTop = Enumerable.Range(0, 6).Select(col => new BitmapStorage("dialog", 1 + 59 * col, 94, 58, 28)).ToArray(),
        OuterThinTop = Enumerable.Range(0, 6).Select(col => new BitmapStorage("dialog", 1 + 59 * col, 123, 58, 12)).ToArray(),
        OuterBottom = Enumerable.Range(0, 6).Select(col => new BitmapStorage("dialog", 1 + 59 * col, 136, 58, 11)).ToArray(),
        OuterMiddle = Enumerable.Range(0, 6).Select(col => new BitmapStorage("dialog", 1 + 59 * col, 148, 58, 13)).ToArray(),
        OuterLeft = Enumerable.Range(0, 6).Select(col => new BitmapStorage("dialog", 112 + 12 * col, 168, 11, 29)).ToArray(),
        OuterRight = Enumerable.Range(0, 6).Select(col => new BitmapStorage("dialog", 112 + 13 * col, 198, 12, 29)).ToArray(),
        OuterTitleTopLeft = new BitmapStorage("dialog", 1, 168, 14, 30),
        OuterTitleTopRight = new BitmapStorage("dialog", 16, 168, 14, 30),
        OuterThinTopLeft = new BitmapStorage("dialog", 1, 199, 14, 14),
        OuterThinTopRight = new BitmapStorage("dialog", 16, 199, 14, 14),
        OuterMiddleLeft = new BitmapStorage("dialog", 1, 214, 14, 18),
        OuterMiddleRight = new BitmapStorage("dialog", 16, 214, 14, 18),
        OuterBottomLeft = new BitmapStorage("dialog", 1, 233, 14, 14),
        OuterBottomRight = new BitmapStorage("dialog", 16, 233, 14, 14),

        Inner = Enumerable.Range(0, 6).Select(col => new BitmapStorage("dialog", new Rectangle(1 + 93 * col, 1, 92, 92))).ToArray(),
        InnerAlt = new BitmapStorage("ICONS", new Rectangle(298, 190, 32, 32)),

        Button = Enumerable.Range(0, 14).Select(col => new BitmapStorage("dialog", new Rectangle(449 + 17 * col, 94, 16, 30))).ToArray(),
        ButtonClicked = Enumerable.Range(0, 14).Select(col => new BitmapStorage("dialog", new Rectangle(449 + 17 * col, 125, 16, 30))).ToArray(),

        RadioButtons = new IImageSource[]
        { new BitmapStorage("dialog", 903, 94, 33, 33), 
          new BitmapStorage("dialog", 869, 94, 33, 33) },
        CheckBoxes = new IImageSource[]
        { new BitmapStorage("dialog", 805, 94, 29, 29),
          new BitmapStorage("dialog", 775, 94, 29, 29) },

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
        var other_default_game_modes = new[] { "SciFi", "Fantasy" };

        var originalPath = Path.Combine(path, original);

        var originalERxists = Directory.Exists(originalPath);
        var rules = originalPath + Path.DirectorySeparatorChar + "rules.txt";
        if (File.Exists(rules))
        {
            yield return new Ruleset(title, new Dictionary<string, string>
            {
                { "Test-Of-Time", "Original" }
            }, originalPath);

            foreach (var subdirectory in Directory.EnumerateDirectories(path))
            {
                var scnRules = Path.Combine(subdirectory, "rules.txt");
                if (File.Exists(scnRules))
                {
                    var game = subdirectory + Path.DirectorySeparatorChar + "game.txt";
                    var name = "";
                    if (File.Exists(game))
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
                        //yield return new Ruleset(name, new Dictionary<string, string>
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
        int paddingTop = headerLabelHeight != 0 ? 28 : 12;

        return new Padding(paddingTop, bottom: 11, left: 11, right: 12);
    }
    public override Padding DialogPadding => new(12, bottom: 11, left: 11, right: 12);


    public override void Initialize()
    {
        base.Initialize();

        if (Dialogs.TryGetValue(MainMenu.Title + "2", out var menu2))
        {
            var existingDialog = DialogHandlers[MainMenu.Title].Dialog.Dialog;
            existingDialog.Options = menu2.Options.Concat(existingDialog.Options.Skip(5)).ToList();
        }

        PicSources = new()
        {
            { "unit", Enumerable.Range(0, 9 * UnitsRows).Select(i => new BitmapStorage("UNITS", new Rectangle(1 + 65 * (i % 9), 1 + (UnitsPxHeight + 1) * (i / 9), 64, UnitsPxHeight), true, true)).ToArray() },
            { "HPshield", new[] { new BitmapStorage("UNITS", new Rectangle(586, 1, 32, 10), true) } },
            { "textColours", Enumerable.Range(0, 9).Select(col =>
                    new BitmapStorage("CITIES", new Rectangle(1 + 15 * col, 421, 14, 3), true)).ToArray() },
            { "flags", Enumerable.Range(0, 2 * 9).Select(i =>
                    new BitmapStorage("CITIES", new Rectangle(1 + 15 * (i % 9), 425 + 23 * (i / 9), 14, 22), true)).ToArray() },
            { "fortify", new[] { new BitmapStorage("CITIES", new Rectangle(143, 423, 64, 48), true) } },
            { "fortress", new[] { new BitmapStorage("CITIES", new Rectangle(208, 423, 64, 48), true) } },
            { "airbase,empty", new[] { new BitmapStorage("CITIES", new Rectangle(273, 423, 64, 48), true) } },
            { "airbase,full", new[] { new BitmapStorage("CITIES", new Rectangle(338, 423, 64, 48), true) } },
            { "base1", Enumerable.Range(0, 11).Select(row =>
                        new BitmapStorage("TERRAIN1", new Rectangle(1, 1 + 33 * row, 64, 32), true)).ToArray() },
            { "base2", Enumerable.Range(0, 11).Select(row =>
                        new BitmapStorage("TERRAIN1", new Rectangle(66, 1 + 33 * row, 64, 32), true)).ToArray() },
            { "special1", Enumerable.Range(0, 11).Select(row =>
                        new BitmapStorage("TERRAIN1", new Rectangle(131, 1 + 33 * row, 64, 32), true)).ToArray() },
            { "special2", Enumerable.Range(0, 11).Select(row =>
                        new BitmapStorage("TERRAIN1", new Rectangle(196, 1 + 33 * row, 64, 32), true)).ToArray() },
            { "road", Enumerable.Range(0, 9).Select(col =>
                        new BitmapStorage("TERRAIN1", new Rectangle(1 + 65 * col, 364, 64, 32), true)).ToArray() },
            { "railroad", Enumerable.Range(0, 9).Select(col =>
                        new BitmapStorage("TERRAIN1", new Rectangle(1 + 65 * col, 397, 64, 32), true)).ToArray() },
            { "irrigation", new[] { new BitmapStorage("TERRAIN1", new Rectangle(456, 100, 64, 32), true) } },
            { "farmland", new[] { new BitmapStorage("TERRAIN1", new Rectangle(456, 133, 64, 32), true) } },
            { "mine", new[] { new BitmapStorage("TERRAIN1", new Rectangle(456, 166, 64, 32), true) } },
            { "pollution", new[] { new BitmapStorage("TERRAIN1", new Rectangle(456, 199, 64, 32), true) } },
            { "shield", new[] { new BitmapStorage("TERRAIN1", new Rectangle(456, 232, 64, 32), true) } },
            { "hut", new[] { new BitmapStorage("TERRAIN1", new Rectangle(456, 265, 64, 32), true) } },
            { "dither", new[] { new BitmapStorage("TERRAIN1", new Rectangle(1, 447, 64, 32), true) } },
            { "blank", new[] { new BitmapStorage("TERRAIN1", new Rectangle(131, 447, 64, 32), true) } },
            { "connection", Enumerable.Range(0, 2 * 8).Select(i =>
                    new BitmapStorage("TERRAIN2", new Rectangle(1 + 65 * (i % 8), 1 + 33 * (i / 8), 64, 32), true)).ToArray() },
            { "river", Enumerable.Range(0, 2 * 8).Select(i =>
                    new BitmapStorage("TERRAIN2", new Rectangle(1 + 65 * (i % 8), 67 + 33 * (i / 8), 64, 32), true)).ToArray() },
            { "forest", Enumerable.Range(0, 2 * 8).Select(i =>
                    new BitmapStorage("TERRAIN2", new Rectangle(1 + 65 * (i % 8), 133 + 33 * (i / 8), 64, 32), true)).ToArray() },
            { "mountain", Enumerable.Range(0, 2 * 8).Select(i =>
                    new BitmapStorage("TERRAIN2", new Rectangle(1 + 65 * (i % 8), 199 + 33 * (i / 8), 64, 32), true)).ToArray() },
            { "hill", Enumerable.Range(0, 2 * 8).Select(i =>
                    new BitmapStorage("TERRAIN2", new Rectangle(1 + 65 * (i % 8), 265 + 33 * (i / 8), 64, 32), true)).ToArray() },
            { "riverMouth", Enumerable.Range(0, 4).Select(col =>
                    new BitmapStorage("TERRAIN2", new Rectangle(1 + 65 * col, 331, 64, 32), true)).ToArray() },
            { "viewPiece", new[] { new BitmapStorage("ICONS", new Rectangle(199, 256, 64, 32), true) } },
            { "gridlines", new[] { new BitmapStorage("ICONS", new Rectangle(183, 430, 64, 32), true) } },
            { "gridlines,visible", new[] { new BitmapStorage("ICONS", new Rectangle(248, 430, 64, 32), true) } },
            { "battleAnim", Enumerable.Range(0, 8).Select(col =>
                    new BitmapStorage("ICONS", new Rectangle(1 + 33 * col, 356, 32, 32), true)).ToArray() },
            { "researchProgress", Enumerable.Range(0, 4).Select(col =>
                    new BitmapStorage("ICONS", new Rectangle(49 + 15 * col, 290, 14, 14), true)).ToArray() },
            { "globalWarming", Enumerable.Range(0, 4).Select(col =>
                    new BitmapStorage("ICONS", new Rectangle(49 + 15 * col, 305, 14, 14), true)).ToArray() },
            { "zoomIn", new[] { new BitmapStorage("ICONS", new Rectangle(18, 389, 16, 16)) } },
            { "zoomOut", new[] { new BitmapStorage("ICONS", new Rectangle(35, 389, 16, 16)) } },
            { "backgroundImage", new[]{ new BinaryStorage("Tiles.dll", 0x100740, 0x12702) } },
            { "backgroundImageSmall1", new[]{ new BinaryStorage("Tiles.dll", 0xF5C44, 0xAAFC, new Rectangle(332, 134, 64, 64)) } },
            { "backgroundImageSmall2", new[]{ new BinaryStorage("Tiles.dll", 0xF5C44, 0xAAFC, new Rectangle(398, 134, 64, 64)) } },
            { "cityBuiltAncient", new[]{ new BinaryStorage("Tiles.dll", 0xE3D1C, 0x5A34) } },
            { "cityBuiltModern", new[]{ new BinaryStorage("Tiles.dll", 0xE9750, 0x5C2D) } },
            { "observatoryPic", new[]{ new BinaryStorage("Intro.dll", 0x1E630, 0xACDC) } },
            { "horzionPic", new[]{ new BinaryStorage("Intro.dll", 0x1E630, 0xACDC) } },
            { "creaturePic", new[]{ new BinaryStorage("Intro.dll", 0x2EBB4, 0x14600) } },
            { "duelPic", new[]{ new BinaryStorage("Intro.dll", 0x431B4, 0x177AF) } },
            { "knightsPic", new[]{ new BinaryStorage("Intro.dll", 0x5A964, 0xEB67) } },
            { "labPic", new[]{ new BinaryStorage("Intro.dll", 0x694CC, 0xFC4B) } },
            { "templePic", new[]{ new BinaryStorage("Intro.dll", 0x79118, 0x10EBB) } },
            { "manorPic", new[]{ new BinaryStorage("Intro.dll", 0x89FD4, 0x13AA8) } },
            { "bookPic", new[]{ new BinaryStorage("Intro.dll", 0x9DA7C, 0x162AE) } },
            { "alienplanetPic", new[]{ new BinaryStorage("Intro.dll", 0xB3D2C, 0x988D) } },
            { "rocksPic", new[]{ new BinaryStorage("Intro.dll", 0xBD5BC, 0xBF78) } },
            { "shipPic", new[]{ new BinaryStorage("Intro.dll", 0xC9534, 0x5A03) } },
            { "hotplanetPic", new[]{ new BinaryStorage("Intro.dll", 0xCEF38, 0x9130) } },
        };

        var src = new BitmapStorage[6 * 8];
        for (int row = 0; row < 6; row++)
        {
            for (int col = 0; col < 4; col++)
            {
                src[8 * row + col] = new BitmapStorage("CITIES", new Rectangle(1 + 65 * col, 39 + 49 * row, 64, 48), true, true);    // Open cities
                src[8 * row + 4 + col] = new BitmapStorage("CITIES", new Rectangle(334 + 65 * col, 39 + 49 * row, 64, 48), true, true);    // Walled cities
            }
        }
        PicSources.Add("city", src);

        src = new BitmapStorage[4 * 8];
        for (int i = 0; i < 8; i++)
        {
            src[4 * i + 0] = new BitmapStorage("TERRAIN2", new Rectangle(1 + 66 * i, 429, 32, 16), true);
            src[4 * i + 1] = new BitmapStorage("TERRAIN2", new Rectangle(1 + 66 * i, 446, 32, 16), true);
            src[4 * i + 2] = new BitmapStorage("TERRAIN2", new Rectangle(1 + 66 * i, 463, 32, 16), true);
            src[4 * i + 3] = new BitmapStorage("TERRAIN2", new Rectangle(34 + 66 * i, 463, 32, 16), true);
        }
        PicSources.Add("coastline", src);


        DialogHandlers["STARTMENU"].Dialog.Decorations.Add(new Decoration(PicSources["observatoryPic"][0], new Point(0.08, 0.09)));
        DialogHandlers["MAINMENU"].Dialog.Decorations.Add(new Decoration(PicSources["observatoryPic"][0], new Point(0.08, 0.09)));
        DialogHandlers["SIZEOFMAP"].Dialog.Decorations.Add(new Decoration(PicSources["horzionPic"][0], new Point(0.08, 0.09)));
        DialogHandlers["DIFFICULTY"].Dialog.Decorations.Add(new Decoration(PicSources["creaturePic"][0], new Point(0.08, 0.09)));
        DialogHandlers["ENEMIES"].Dialog.Decorations.Add(new Decoration(PicSources["duelPic"][0], new Point(0.08, 0.09)));
        DialogHandlers["BARBARITY"].Dialog.Decorations.Add(new Decoration(PicSources["knightsPic"][0], new Point(0.08, 0.09)));
        DialogHandlers["RULES"].Dialog.Decorations.Add(new Decoration(PicSources["bookPic"][0], new Point(0.08, 0.09)));
        DialogHandlers["ADVANCED"].Dialog.Decorations.Add(new Decoration(PicSources["bookPic"][0], new Point(0.08, 0.09)));
        //DialogHandlers["OPPONENT"].Dialog.Decorations.Add(new Decoration(PicSources["bookPic"][0], new Point(0.08, 0.09)));
        DialogHandlers["ACCELERATED"].Dialog.Decorations.Add(new Decoration(PicSources["bookPic"][0], new Point(0.08, 0.09)));
        DialogHandlers["GENDER"].Dialog.Decorations.Add(new Decoration(PicSources["labPic"][0], new Point(0.08, 0.09)));
        DialogHandlers["TRIBE"].Dialog.Decorations.Add(new Decoration(PicSources["templePic"][0], new Point(0.08, 0.09)));
        DialogHandlers["CUSTOMTRIBE"].Dialog.Decorations.Add(new Decoration(PicSources["templePic"][0], new Point(0.08, 0.09)));
        DialogHandlers["CUSTOMTRIBE2"].Dialog.Decorations.Add(new Decoration(PicSources["templePic"][0], new Point(0.08, 0.09)));
        DialogHandlers["CUSTOMCITY"].Dialog.Decorations.Add(new Decoration(PicSources["manorPic"][0], new Point(0.08, 0.09)));
        DialogHandlers["CUSTOMFORM"].Dialog.Decorations.Add(new Decoration(PicSources["shipPic"][0], new Point(0.08, 0.09)));
        DialogHandlers["CUSTOMCLIMATE"].Dialog.Decorations.Add(new Decoration(PicSources["rocksPic"][0], new Point(0.08, 0.09)));
        DialogHandlers["CUSTOMTEMP"].Dialog.Decorations.Add(new Decoration(PicSources["hotplanetPic"][0], new Point(0.08, 0.09)));
        DialogHandlers["CUSTOMAGE"].Dialog.Decorations.Add(new Decoration(PicSources["alienplanetPic"][0], new Point(0.08, 0.09)));
    }

    protected override List<MenuDetails> MenuMap { get; } = new List<MenuDetails>
    {
        new MenuDetails
        {
            Key = "GAME", Defaults = new List<MenuElement>
            {
                new MenuElement("&Game", Shortcut.None, KeyboardKey.G),
                new MenuElement("Game &Options|Ctrl+O", new Shortcut(KeyboardKey.O, ctrl: true), KeyboardKey.O, commandId: GameOptions),
                new MenuElement("Graphic O&ptions|Ctrl+P", new Shortcut(KeyboardKey.P, ctrl: true),
                    KeyboardKey.P, commandId: GraphicOptions),
                new MenuElement("&City Report Options|Ctrl+E", new Shortcut(KeyboardKey.E, ctrl: true),
                    KeyboardKey.C, commandId: CityReportOptions),
                new MenuElement("M&ultiplayer Options|Ctrl+Y", new Shortcut(KeyboardKey.Y, ctrl: true),
                    KeyboardKey.U),
                new MenuElement("-", Shortcut.None, KeyboardKey.Null),
                new MenuElement("&Game Profile", Shortcut.None, KeyboardKey.G),
                new MenuElement("-", Shortcut.None, KeyboardKey.Null),
                new MenuElement("Pick &Music", Shortcut.None, KeyboardKey.M),
                new MenuElement("&Save Game|Ctrl+S", new Shortcut(KeyboardKey.S, ctrl: true), KeyboardKey.S),
                new MenuElement("&Load Game|Ctrl+L", new Shortcut(KeyboardKey.L, ctrl: true), KeyboardKey.L),
                new MenuElement("&Join Game|Ctrl+J", new Shortcut(KeyboardKey.J, ctrl: true), KeyboardKey.J),
                new MenuElement("-", Shortcut.None, KeyboardKey.Null),
                new MenuElement("Set Pass&word|Ctrl+W", new Shortcut(KeyboardKey.W, ctrl: true), KeyboardKey.W),
                new MenuElement("Change &Timer|Ctrl+T", new Shortcut(KeyboardKey.T, ctrl: true), KeyboardKey.T),
                new MenuElement("-", Shortcut.None, KeyboardKey.Null),
                new MenuElement("&Retire|Ctrl+R", new Shortcut(KeyboardKey.R, ctrl: true), KeyboardKey.R),
                new MenuElement("&Quit|Ctrl+Q", new Shortcut(KeyboardKey.Q, ctrl: true), KeyboardKey.Q, commandId: QuitGame)
            }
        },
        new MenuDetails
        {
            Key = "KINGDOM", Defaults = new List<MenuElement>
            {
                new MenuElement("&Kingdom", Shortcut.None, KeyboardKey.K),
                new MenuElement("&Tax Rate|Shift+T", new Shortcut(KeyboardKey.T, shift: true), KeyboardKey.T),
                new MenuElement("-", Shortcut.None, KeyboardKey.Null),
                new MenuElement("Find &City|Shift+C", new Shortcut(KeyboardKey.C, shift: true), KeyboardKey.C),
                new MenuElement("-", Shortcut.None, KeyboardKey.Null),
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
                new MenuElement("-", Shortcut.None, KeyboardKey.Null),
                new MenuElement("Zoom &In|z", new Shortcut(KeyboardKey.Z), KeyboardKey.I, commandId: ZoomIn),
                new MenuElement("Zoom &Out|X", new Shortcut(KeyboardKey.X), KeyboardKey.O, commandId: ZoomOut),
                new MenuElement("-", Shortcut.None, KeyboardKey.Null),
                new MenuElement("Max Zoom In|Ctrl+Z", new Shortcut(KeyboardKey.Z, ctrl: true),
                    KeyboardKey.Null, commandId: MaxZoomIn),
                new MenuElement("Standard Zoom|Shift+Z", new Shortcut(KeyboardKey.Z, shift: true),
                    KeyboardKey.Null, commandId: StandardZoom),
                new MenuElement("Medium Zoom Out|Shift+X", new Shortcut(KeyboardKey.X, shift: true),
                    KeyboardKey.Null, commandId: MediumZoomOut),
                new MenuElement("Max Zoom Out|Ctrl+X", new Shortcut(KeyboardKey.X, ctrl: true),
                    KeyboardKey.Null, commandId: MaxZoomOut),
                new MenuElement("-", Shortcut.None, KeyboardKey.Null),
                new MenuElement("Show Map Grid|Ctrl+G", new Shortcut(KeyboardKey.G, ctrl: true),
                    KeyboardKey.Null, commandId: ShowMapGrid),
                new MenuElement("Arrange Windows", Shortcut.None, KeyboardKey.Null),
                new MenuElement("Show Hidden Terrain|t", new Shortcut(KeyboardKey.T), KeyboardKey.T),
                new MenuElement("&Center View|c", new Shortcut(KeyboardKey.C), KeyboardKey.C),
                new MenuElement("Map Layout", Shortcut.None, KeyboardKey.Null, commandId: MapLayoutToggle),
                new MenuElement("City Layout", Shortcut.None, KeyboardKey.Null),
            },
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
                new MenuElement("-", Shortcut.None, KeyboardKey.Null),
                new MenuElement("&City Status|F1", new Shortcut(KeyboardKey.F1), KeyboardKey.C),
                new MenuElement("&Defense Minister|F2", new Shortcut(KeyboardKey.F2), KeyboardKey.D),
                new MenuElement("&Foreign Minister|F3", new Shortcut(KeyboardKey.F3), KeyboardKey.F),
                new MenuElement("-", Shortcut.None, KeyboardKey.Null),
                new MenuElement("&Attitude Advisor|F4", new Shortcut(KeyboardKey.F4), KeyboardKey.A),
                new MenuElement("&Trade Advisor|F5", new Shortcut(KeyboardKey.F5), KeyboardKey.T),
                new MenuElement("&Science Advisor|F6", new Shortcut(KeyboardKey.F6), KeyboardKey.S),
                new MenuElement("-", Shortcut.None, KeyboardKey.Null),
                new MenuElement("Cas&ualty Timeline|Ctrl-D", new Shortcut(KeyboardKey.D, ctrl: true),
                    KeyboardKey.U)
            },
        },

        new MenuDetails
        {
            Key = "WORLD", Defaults = new List<MenuElement>
            {
                new MenuElement("&World", Shortcut.None, KeyboardKey.W),
                new MenuElement("&Wonders of the World|F7", new Shortcut(KeyboardKey.F7), KeyboardKey.W),
                new MenuElement("&Top 5 Cities|F8", new Shortcut(KeyboardKey.F8), KeyboardKey.T),
                new MenuElement("&Civilization Score|F9", new Shortcut(KeyboardKey.F9), KeyboardKey.C),
                new MenuElement("-", Shortcut.None, KeyboardKey.Null),
                new MenuElement("&Demographics|F11", new Shortcut(KeyboardKey.F11), KeyboardKey.D),
                new MenuElement("&Spaceships|F12", new Shortcut(KeyboardKey.F12), KeyboardKey.S)
            },
        },

        new MenuDetails
        {
            Key = "CHEAT", Defaults = new List<MenuElement>
            {
                new MenuElement("&Cheat", Shortcut.None, KeyboardKey.C),
                new MenuElement("Toggle Cheat Mode|Ctrl+K", new Shortcut(KeyboardKey.K, ctrl: true),
                    KeyboardKey.Null),
                new MenuElement("-", Shortcut.None, KeyboardKey.Null),
                new MenuElement("Create &Unit|Shift+F1", new Shortcut(KeyboardKey.F1, shift: true),
                    KeyboardKey.U),
                new MenuElement("Reveal &Map|Shift+F2", new Shortcut(KeyboardKey.F2, shift: true),
                    KeyboardKey.M, CheatRevealMapCommand),
                new MenuElement("Set &Human Player|Shift+F3", new Shortcut(KeyboardKey.F3, shift: true),
                    KeyboardKey.H),
                new MenuElement("-", Shortcut.None, KeyboardKey.Null),
                new MenuElement("Set Game Year|Shift+F4", new Shortcut(KeyboardKey.F4, shift: true),
                    KeyboardKey.Null),
                new MenuElement("&Kill Civilization|Shift+F5", new Shortcut(KeyboardKey.F5, shift: true),
                    KeyboardKey.K),
                new MenuElement("-", Shortcut.None, KeyboardKey.Null),
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
                new MenuElement("-", Shortcut.None, KeyboardKey.Null),
                new MenuElement("Edit Unit|Ctrl+Shift+U", new Shortcut(KeyboardKey.U, ctrl: true, shift: true),
                    KeyboardKey.Null),
                new MenuElement("Edit City|Ctrl+Shift+C", new Shortcut(KeyboardKey.C, ctrl: true, shift: true),
                    KeyboardKey.Null),
                new MenuElement("Edit King|Ctrl+Shift+K", new Shortcut(KeyboardKey.K, ctrl: true, shift: true),
                    KeyboardKey.Null),
                new MenuElement("-", Shortcut.None, KeyboardKey.Null),
                new MenuElement("Scenario Parameters|Ctrl+Shift+P",
                    new Shortcut(KeyboardKey.P, ctrl: true, shift: true), KeyboardKey.Null),
                new MenuElement("Save As Scenario|Ctrl+Shift+S",
                    new Shortcut(KeyboardKey.S, ctrl: true, shift: true), KeyboardKey.Null),
                new MenuElement("-", Shortcut.None, KeyboardKey.Null),
                new MenuElement("Lua Console|Ctrl+Shift+9", new Shortcut(KeyboardKey.Nine,true,true), KeyboardKey.L, omitIfNoCommand: true, commandId: OpenLuaConsole)
            },
        },

        new MenuDetails
        {
            Key = "MAP", Defaults = new List<MenuElement>
            {
                new MenuElement("&Map", Shortcut.None, KeyboardKey.M),
                new MenuElement("Toggle Cheat Mode|Ctrl+K", new Shortcut(KeyboardKey.K, ctrl: true),
                    KeyboardKey.Null),
                new MenuElement("-", Shortcut.None, KeyboardKey.Null),
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
                new MenuElement("-", Shortcut.None, KeyboardKey.Null),
                new MenuElement("&Governments", Shortcut.None, KeyboardKey.G),
                new MenuElement("&Terrain Types", Shortcut.None, KeyboardKey.T),
                new MenuElement("-", Shortcut.None, KeyboardKey.Null),
                new MenuElement("Game &Concepts", Shortcut.None, KeyboardKey.C),
                new MenuElement("-", Shortcut.None, KeyboardKey.Null),
                new MenuElement("&About Test of Time", Shortcut.None, KeyboardKey.A)
            },
        }
    };

    public override int UnitsRows => 9;
    public override int UnitsPxHeight => 64;

    public override Dictionary<string, IImageSource[]> PicSources { get; set; }

    public override void LoadPlayerColours()
    {
        var playerColours = new PlayerColour[9];
        for (int col = 0; col < 9; col++)
        {
            unsafe
            {
                var imageColours = Images.ExtractBitmap(PicSources["textColours"][col], this).LoadColors();
                var textColour = imageColours[2 * Images.ExtractBitmap(PicSources["textColours"][col], this).Width + 0];
                var shieldColour = imageColours[2 * Images.ExtractBitmap(PicSources["textColours"][col], this).Width + 0];
                textColour.A = 255; // to avoid any upper-left-pixel transparency issues
                shieldColour.A = 255;
                Image.UnloadColors(imageColours);

                // This is not exact, but a good aproximation what TOT does with shield colours
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
        HPbarColours = new[] { new Color(247, 0, 0, 255), new Color(255, 222, 74, 255), new Color(82, 173, 33, 255) },
        HPbarSizeForColours = new[] { 5, 13 },
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
        for (int col = 0; col < topCols; col++)
        {
            destination.Draw(top[rnd.Next(len)], new Rectangle(0, 0, top[0].Width, top[0].Height), new Rectangle(topLeft.Width + top[0].Width * col, 0, top[0].Width, top[0].Height), Color.White);
        }
        destination.Draw(topRight, new Rectangle(0, 0, topRight.Width, topRight.Height), new Rectangle(width - topRight.Width, 0, topRight.Width, topRight.Height), Color.White);

        // Left-right border
        var sideRows = (height - topLeft.Height - wp.OuterBottomLeft.Height) / wp.OuterLeft[0].Height + 1;
        for (int row = 0; row < sideRows; row++)
        {
            destination.Draw(wp.OuterLeft[rnd.Next(len)], new Rectangle(0, 0, wp.OuterLeft[0].Width, wp.OuterLeft[0].Height), new Rectangle(0, topLeft.Height + wp.OuterLeft[0].Height * row, wp.OuterLeft[0].Width, wp.OuterLeft[0].Height), Color.White);
            destination.Draw(wp.OuterRight[rnd.Next(len)], new Rectangle(0, 0, wp.OuterRight[0].Width, wp.OuterRight[0].Height), new Rectangle(width - wp.OuterRight[0].Width, topRight.Height + wp.OuterRight[0].Height * row, wp.OuterRight[0].Width, wp.OuterRight[0].Height), Color.White);
        }

        // Bottom border
        destination.Draw(wp.OuterBottomLeft, new Rectangle(0, 0, wp.OuterBottomLeft.Width, wp.OuterBottomLeft.Height), new Rectangle(0, height - wp.OuterBottomLeft.Height, wp.OuterBottomLeft.Width, wp.OuterBottomLeft.Height), Color.White);
        var btmCols = (width - wp.OuterBottomLeft.Width - wp.OuterBottomRight.Width) / wp.OuterBottom[0].Width + 1;
        for (int col = 0; col < btmCols; col++)
        {
            destination.Draw(wp.OuterBottom[rnd.Next(len)], new Rectangle(0, 0, wp.OuterBottom[0].Width, wp.OuterBottom[0].Height), new Rectangle(wp.OuterBottomLeft.Width + wp.OuterBottom[0].Width * col, height - wp.OuterBottom[0].Height, wp.OuterBottom[0].Width, wp.OuterBottom[0].Height), Color.White);
        }
        destination.Draw(wp.OuterBottomRight, new Rectangle(0, 0, wp.OuterBottomRight.Width, wp.OuterBottomRight.Height), new Rectangle(width - wp.OuterBottomRight.Width, height - wp.OuterBottomRight.Height, wp.OuterBottomRight.Width, wp.OuterBottomRight.Height), Color.White);

        if (statusPanel)
        {
            destination.Draw(wp.OuterMiddleLeft, new Rectangle(0, 0, wp.OuterMiddleLeft.Width, wp.OuterMiddleLeft.Height), new Rectangle(0, 85, wp.OuterMiddleLeft.Width, wp.OuterMiddleLeft.Height), Color.White);
            var mdlCols = (width - wp.OuterMiddleLeft.Width - wp.OuterMiddleRight.Width) / wp.OuterMiddle[0].Width;
            for (int col = 0; col < mdlCols; col++)
            {
                destination.Draw(wp.OuterMiddle[rnd.Next(len)], new Rectangle(0, 0, wp.OuterMiddle[0].Width, wp.OuterMiddle[0].Height), new Rectangle(wp.OuterMiddleLeft.Width + wp.OuterMiddle[0].Width * col, 88, wp.OuterMiddle[0].Width, wp.OuterMiddle[0].Height), Color.White);
            }
            var leftToDraw = width - mdlCols * wp.OuterMiddle[0].Width - wp.OuterMiddleLeft.Width - wp.OuterMiddleRight.Width;
            destination.Draw(wp.OuterMiddle[rnd.Next(len)], new Rectangle(0, 0, leftToDraw, wp.OuterMiddle[0].Height), new Rectangle(wp.OuterMiddleLeft.Width + mdlCols * wp.OuterMiddle[0].Width, 88, leftToDraw, wp.OuterMiddle[0].Height), Color.White);
            destination.Draw(wp.OuterMiddleRight, new Rectangle(0, 0, wp.OuterMiddleRight.Width, wp.OuterMiddleRight.Height), new Rectangle(width - wp.OuterMiddleRight.Width, 85, wp.OuterMiddleRight.Width, wp.OuterMiddleRight.Height), Color.White);
        }
    }

    public override void DrawBorderLines(ref Image destination, int height, int width, Padding padding, bool statusPanel) { }

    public override void DrawButton(Texture2D texture, int x, int y, int w, int h)
    {
        Graphics.DrawTexture(texture, x, y, Color.White);
    }

    public TestOfTimeInterface(IMain main) : base(main)
    {
    }
}