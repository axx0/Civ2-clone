using Civ2;
using Civ2engine;
using Civ2engine.IO;
using JetBrains.Annotations;
using Model;
using Model.Images;
using Model.ImageSets;
using Model.Interface;
using Model.Menu;
using Model.Utils;
using Raylib_CSharp.Colors;
using Raylib_CSharp.Images;
using Raylib_CSharp.Interact;
using Raylib_CSharp.Rendering;
using Raylib_CSharp.Textures;
using Raylib_CSharp.Transformations;
using RaylibUtils;
using static Model.Menu.CommandIds;

namespace Civ2Gold;

[UsedImplicitly]
public class Civ2GoldInterface(IMain main) : Civ2Interface(main)
{
    public override string Title => "Civilization II Multiplayer Gold";

    public override string InitialMenu => "MAINMENU";

    public override InterfaceStyle Look { get; } = new()
    {
        Outer = new BitmapStorage("ICONS", new Rectangle(199, 322, 64, 32)),
        Inner = [new BitmapStorage("ICONS", new Rectangle(298, 190, 32, 32))],

        RadioButtons = [new BitmapStorage("buttons.png", 0, 0, 32), new BitmapStorage("buttons.png", 32, 0, 32)],
        CheckBoxes = [new BitmapStorage("buttons.png", 0, 32, 32), new BitmapStorage("buttons.png", 32, 32, 32)],
        
        DefaultFont = Fonts.Tnr,
        ButtonFont = Fonts.Tnr,
        ButtonFontSize = 20,
        ButtonColour = Color.Black,
        HeaderLabelFont = Fonts.TnRbold,
        HeaderLabelFontSizeNormal = 28,
        HeaderLabelFontSizeLarge = 34,
        CityHeaderLabelFontSizeNormal = 18,
        CityHeaderLabelFontSizeLarge = 28,
        CityHeaderLabelFontSizeSmall = 16,
        HeaderLabelShadow = true,
        HeaderLabelColour = new Color(135, 135, 135, 255),
        LabelFont = Fonts.Tnr,
        LabelFontSize = 25,
        LabelColour = new Color(31, 31, 31, 255),
        LabelShadowColour = new Color(191, 191, 191, 255),
        CityWindowFont = Fonts.Arial,
        CityWindowFontSize = 15,
        MenuFont = Fonts.Arial,
        MenuFontSize = 14,
        StatusPanelLabelFont = Fonts.TnRbold,
        StatusPanelLabelColor = new Color(51, 51, 51, 255),
        StatusPanelLabelColorShadow = new Color(191, 191, 191, 255),
        MovingUnitsViewingPiecesLabelColor = Color.White,
        MovingUnitsViewingPiecesLabelColorShadow = Color.Black,
        EndOfTurnColors = [new Color(135, 135, 135, 255), Color.White],
    };

    public override bool IsButtonInOuterPanel => true;
    
    public override Padding GetPadding(float headerLabelHeight, bool footer)
    {
        var paddingTop = headerLabelHeight != 0 ? 7 + Math.Max((int)(-3 + 2 / 9f * headerLabelHeight + headerLabelHeight), (int)headerLabelHeight) : 11;
        var paddingBtm = footer ? 46 : 10;

        return new Padding(paddingTop, bottom:paddingBtm, left:11, right:11);
    }

    public override Padding DialogPadding => new(11);

    public override void Initialize()
    {
        base.Initialize();

        PicSources.Add("unit",
            Enumerable.Range(0, 9 * UnitsRows).Select(i => new BitmapStorage("UNITS",
                new Rectangle(1 + 65 * (i % 9), 1 + (UnitsPxHeight + 1) * (i / 9f), 64, UnitsPxHeight),
                searchFlagLoc: true)).ToArray<IImageSource>());
        PicSources.Add("HPshield", [new BitmapStorage("UNITS", new Rectangle(597, 30, 12, 20))]);
        PicSources.Add("backShield1", [new BitmapStorage("UNITS", new Rectangle(586, 1, 12, 20))]);
        PicSources.Add("backShield2", [new BitmapStorage("UNITS", new Rectangle(599, 1, 12, 20))]);
        PicSources.Add("textColours", Enumerable.Range(0, 9).Select(col =>
            new BitmapStorage("CITIES", new Rectangle(1 + 15 * col, 423, 14, 1))).ToArray<IImageSource>());
        PicSources.Add("flags", Enumerable.Range(0, 2 * 9).Select(i =>
                new BitmapStorage("CITIES", new Rectangle(1 + 15 * (i % 9f), 425 + 23 * (i / 9f), 14, 22)))
            .ToArray<IImageSource>());
        PicSources.Add("fortify", [new BitmapStorage("CITIES", new Rectangle(143, 423, 64, 48))]);
        PicSources.Add("fortress", [new BitmapStorage("CITIES", new Rectangle(208, 423, 64, 48))]);
        PicSources.Add("airbase,empty", [new BitmapStorage("CITIES", new Rectangle(273, 423, 64, 48))]);
        PicSources.Add("airbase,full", [new BitmapStorage("CITIES", new Rectangle(338, 423, 64, 48))]);
        PicSources.Add("base1", Enumerable.Range(0, 11).Select(row =>
            new BitmapStorage("TERRAIN1", new Rectangle(1, 1 + 33 * row, 64, 32))).ToArray<IImageSource>());
        PicSources.Add("base2", Enumerable.Range(0, 11).Select(row =>
            new BitmapStorage("TERRAIN1", new Rectangle(66, 1 + 33 * row, 64, 32))).ToArray<IImageSource>());
        PicSources.Add("special1", Enumerable.Range(0, 11).Select(row =>
            new BitmapStorage("TERRAIN1", new Rectangle(131, 1 + 33 * row, 64, 32))).ToArray<IImageSource>());
        PicSources.Add("special2", Enumerable.Range(0, 11).Select(row =>
            new BitmapStorage("TERRAIN1", new Rectangle(196, 1 + 33 * row, 64, 32))).ToArray<IImageSource>());
        PicSources.Add("road", Enumerable.Range(0, 9).Select(col =>
            new BitmapStorage("TERRAIN1", new Rectangle(1 + 65 * col, 363, 64, 32))).ToArray<IImageSource>());
        PicSources.Add("railroad", Enumerable.Range(0, 9).Select(col =>
            new BitmapStorage("TERRAIN1", new Rectangle(1 + 65 * col, 397, 64, 32))).ToArray<IImageSource>());
        PicSources.Add("irrigation", [new BitmapStorage("TERRAIN1", new Rectangle(456, 100, 64, 32))]);
        PicSources.Add("farmland", [new BitmapStorage("TERRAIN1", new Rectangle(456, 133, 64, 32))]);
        PicSources.Add("mine", [new BitmapStorage("TERRAIN1", new Rectangle(456, 166, 64, 32))]);
        PicSources.Add("pollution", [new BitmapStorage("TERRAIN1", new Rectangle(456, 199, 64, 32))]);
        PicSources.Add("shield", [new BitmapStorage("TERRAIN1", new Rectangle(456, 232, 64, 32))]);
        PicSources.Add("hut", [new BitmapStorage("TERRAIN1", new Rectangle(456, 265, 64, 32))]);
        PicSources.Add("dither", [new BitmapStorage("TERRAIN1", new Rectangle(1, 447, 64, 32))]);
        PicSources.Add("blank", [new BitmapStorage("TERRAIN1", new Rectangle(131, 447, 64, 32))]);
        PicSources.Add("connection", Enumerable.Range(0, 2 * 8).Select(i =>
                new BitmapStorage("TERRAIN2", new Rectangle(1 + 65 * (i % 8), 1 + 33 * (i / 8f), 64, 32)))
            .ToArray<IImageSource>());
        PicSources.Add("river", Enumerable.Range(0, 2 * 8).Select(i =>
                new BitmapStorage("TERRAIN2", new Rectangle(1 + 65 * (i % 8), 67 + 33 * (i / 8f), 64, 32)))
            .ToArray<IImageSource>());
        PicSources.Add("forest", Enumerable.Range(0, 2 * 8).Select(i =>
                new BitmapStorage("TERRAIN2", new Rectangle(1 + 65 * (i % 8), 133 + 33 * (i / 8f), 64, 32)))
            .ToArray<IImageSource>());
        PicSources.Add("mountain", Enumerable.Range(0, 2 * 8).Select(i =>
                new BitmapStorage("TERRAIN2", new Rectangle(1 + 65 * (i % 8), 199 + 33 * (i / 8f), 64, 32)))
            .ToArray<IImageSource>());
        PicSources.Add("hill", Enumerable.Range(0, 2 * 8).Select(i =>
                new BitmapStorage("TERRAIN2", new Rectangle(1 + 65 * (i % 8), 265 + 33 * (i / 8f), 64, 32)))
            .ToArray<IImageSource>());
        PicSources.Add("riverMouth", Enumerable.Range(0, 4).Select(col =>
            new BitmapStorage("TERRAIN2", new Rectangle(1 + 65 * col, 331, 64, 32))).ToArray<IImageSource>());
        PicSources.Add("viewPiece", [new BitmapStorage("ICONS", new Rectangle(199, 256, 64, 32))]);
        PicSources.Add("gridlines", [new BitmapStorage("ICONS", new Rectangle(183, 430, 64, 32))]);
        PicSources.Add("gridlines,visible", [new BitmapStorage("ICONS", new Rectangle(248, 430, 64, 32))]);
        PicSources.Add("battleAnim", Enumerable.Range(0, 8).Select(col =>
            new BitmapStorage("ICONS", new Rectangle(1 + 33 * col, 356, 32, 32))).ToArray<IImageSource>());
        PicSources.Add("researchProgress", Enumerable.Range(0, 4).Select(col =>
            new BitmapStorage("ICONS", new Rectangle(49 + 15 * col, 290, 14, 14))).ToArray<IImageSource>());
        PicSources.Add("globalWarming", Enumerable.Range(0, 4).Select(col =>
            new BitmapStorage("ICONS", new Rectangle(49 + 15 * col, 305, 14, 14))).ToArray<IImageSource>());
        PicSources.Add("close", [new BitmapStorage("ICONS", new Rectangle(1, 389, 16, 16))]);
        PicSources.Add("zoomIn", [new BitmapStorage("ICONS", new Rectangle(18, 389, 16, 16))]);
        PicSources.Add("zoomOut", [new BitmapStorage("ICONS", new Rectangle(35, 389, 16, 16))]);
        PicSources.Add("backgroundImage", [new BinaryStorage("Tiles.dll", 0xF7454, 0x1389D)]);
        PicSources.Add("backgroundImageSmall1", [
                new BinaryStorage("Tiles.dll", 0xED354, 0xA0FD, new Rectangle(332, 134, 64, 64))
            ]
        );
        PicSources.Add("backgroundImageSmall2", [
                new BinaryStorage("Tiles.dll", 0xED354, 0xA0FD, new Rectangle(398, 134, 64, 64))
            ]
        );
        PicSources.Add("cityBuiltAncient", [new BinaryStorage("Tiles.dll", 0xDEDA4, 0x46FF)]);
        PicSources.Add("cityBuiltModern", [new BinaryStorage("Tiles.dll", 0xE34A4, 0x4A42)]);
        PicSources.Add("sinaiPic", [new BinaryStorage("Intro.dll", 0x1E630, 0x9F78)]);
        PicSources.Add("stPeterburgPic", [new BinaryStorage("Intro.dll", 0x285A8, 0x15D04)]);
        PicSources.Add("desertPic", [new BinaryStorage("Intro.dll", 0xD0140, 0xA35A)]);
        PicSources.Add("snowPic", [new BinaryStorage("Intro.dll", 0xE2E1C, 0xA925)]);
        PicSources.Add("canyonPic", [new BinaryStorage("Intro.dll", 0xC51B8, 0xAF88)]);
        PicSources.Add("mingGeneralPic", [new BinaryStorage("Intro.dll", 0x3E2AC, 0x1D183)]);
        PicSources.Add("islandPic", [new BinaryStorage("Intro.dll", 0xDA49C, 0x8980)]);
        PicSources.Add("ancientPersonsPic", [new BinaryStorage("Intro.dll", 0x5B430, 0x15D04)]);
        PicSources.Add("barbariansPic", [new BinaryStorage("Intro.dll", 0x71134, 0x13D5B)]);
        PicSources.Add("galleyPic", [new BinaryStorage("Intro.dll", 0xB6A3C, 0xE77A)]);
        PicSources.Add("peoplePic1", [new BinaryStorage("Intro.dll", 0x84E90, 0x129CE)]);
        PicSources.Add("peoplePic2", [new BinaryStorage("Intro.dll", 0x97860, 0x139A0)]);
        PicSources.Add("templePic", [new BinaryStorage("Intro.dll", 0xAB200, 0xB839)]);


        var src = new IImageSource[6 * 8];
        for (var row = 0; row < 6; row++)
        {
            for (var col = 0; col < 4; col++)
            {
                src[8 * row + col] = new BitmapStorage("CITIES", new Rectangle(1 + 65 * col, 39 + 49 * row, 64, 48),
                    searchFlagLoc: true); // Open cities
                src[8 * row + 4 + col] = new BitmapStorage("CITIES",
                    new Rectangle(334 + 65 * col, 39 + 49 * row, 64, 48), searchFlagLoc: true); // Walled cities
            }
        }

        PicSources.Add("city", src);

        src = new IImageSource[4 * 8];
        for (var i = 0; i < 8; i++)
        {
            src[4 * i + 0] = new BitmapStorage("TERRAIN2", new Rectangle(1 + 66 * i, 429, 32, 16));
            src[4 * i + 1] = new BitmapStorage("TERRAIN2", new Rectangle(1 + 66 * i, 446, 32, 16));
            src[4 * i + 2] = new BitmapStorage("TERRAIN2", new Rectangle(1 + 66 * i, 463, 32, 16));
            src[4 * i + 3] = new BitmapStorage("TERRAIN2", new Rectangle(34 + 66 * i, 463, 32, 16));
        }

        PicSources.Add("coastline", src);


        DialogHandlers["MAINMENU"].Dialog.Decorations
            .Add(new Decoration(PicSources["sinaiPic"][0], new Point(0.08, 0.09)));
        DialogHandlers["SIZEOFMAP"].Dialog.Decorations
            .Add(new Decoration(PicSources["stPeterburgPic"][0], new Point(0, 0.09)));
        DialogHandlers["CUSTOMSIZE"].Dialog.Decorations
            .Add(new Decoration(PicSources["stPeterburgPic"][0], new Point(0, 0.09)));
        DialogHandlers["CUSTOMLAND"].Dialog.Decorations
            .Add(new Decoration(PicSources["stPeterburgPic"][0], new Point(0, 0.09)));
        DialogHandlers["CUSTOMFORM"].Dialog.Decorations
            .Add(new Decoration(PicSources["islandPic"][0], new Point(0, 0.09)));
        DialogHandlers["CUSTOMCLIMATE"].Dialog.Decorations
            .Add(new Decoration(PicSources["desertPic"][0], new Point(0, 0.09)));
        DialogHandlers["CUSTOMTEMP"].Dialog.Decorations
            .Add(new Decoration(PicSources["snowPic"][0], new Point(0, 0.09)));
        DialogHandlers["CUSTOMAGE"].Dialog.Decorations
            .Add(new Decoration(PicSources["canyonPic"][0], new Point(0, 0.09)));
        DialogHandlers["DIFFICULTY"].Dialog.Decorations
            .Add(new Decoration(PicSources["mingGeneralPic"][0], new Point(-0.08, 0.09)));
        DialogHandlers["ENEMIES"].Dialog.Decorations
            .Add(new Decoration(PicSources["ancientPersonsPic"][0], new Point(0.08, 0.09)));
        DialogHandlers["BARBARITY"].Dialog.Decorations
            .Add(new Decoration(PicSources["barbariansPic"][0], new Point(-0.08, 0.09)));
        DialogHandlers["RULES"].Dialog.Decorations
            .Add(new Decoration(PicSources["galleyPic"][0], new Point(0.08, 0.09)));
        DialogHandlers["ADVANCED"].Dialog.Decorations
            .Add(new Decoration(PicSources["galleyPic"][0], new Point(-0.08, 0.09)));
        DialogHandlers["ACCELERATED"].Dialog.Decorations
            .Add(new Decoration(PicSources["galleyPic"][0], new Point(0.08, 0.09)));
        DialogHandlers["GENDER"].Dialog.Decorations
            .Add(new Decoration(PicSources["peoplePic1"][0], new Point(0.0, 0.09)));
        DialogHandlers["TRIBE"].Dialog.Decorations
            .Add(new Decoration(PicSources["peoplePic2"][0], new Point(0.0, 0.09)));
        DialogHandlers["CUSTOMTRIBE"].Dialog.Decorations
            .Add(new Decoration(PicSources["peoplePic2"][0], new Point(0.0, 0.09)));
        DialogHandlers["CUSTOMTRIBE2"].Dialog.Decorations
            .Add(new Decoration(PicSources["peoplePic2"][0], new Point(0.0, 0.09)));
        DialogHandlers["NAME"].Dialog.Decorations
            .Add(new Decoration(PicSources["peoplePic2"][0], new Point(0.0, 0.09)));
        DialogHandlers["CUSTOMCITY"].Dialog.Decorations
            .Add(new Decoration(PicSources["templePic"][0], new Point(0.08, 0.09)));
    }

    protected override List<MenuDetails> MenuMap { get; } =
    [
        new()
        {
            Key = "GAME", Defaults = new List<MenuElement>
            {
                new("&Game", Shortcut.None, KeyboardKey.G),
                new("Game &Options|Ctrl+O", new Shortcut(KeyboardKey.O, ctrl: true), KeyboardKey.O,
                    commandId: GameOptions),
                new("Graphic O&ptions|Ctrl+P", new Shortcut(KeyboardKey.P, ctrl: true),
                    KeyboardKey.P, commandId: GraphicOptions),
                new("&City Report Options|Ctrl+E", new Shortcut(KeyboardKey.E, ctrl: true),
                    KeyboardKey.C, commandId: CityReportOptions),
                new("M&ultiplayer Options|Ctrl+Y", new Shortcut(KeyboardKey.Y, ctrl: true),
                    KeyboardKey.U),
                new("&Game Profile", Shortcut.None, KeyboardKey.G),
                new("-", Shortcut.None, KeyboardKey.Null),
                new("Pick &Music", Shortcut.None, KeyboardKey.M),
                new("-", Shortcut.None, KeyboardKey.Null),
                new("&Save Game|Ctrl+S", new Shortcut(KeyboardKey.S, ctrl: true), KeyboardKey.S,
                    commandId: SaveGame),
                new("&Load Game|Ctrl+L", new Shortcut(KeyboardKey.L, ctrl: true), KeyboardKey.L,
                    commandId: LoadGame),
                new("&Join Game|Ctrl+J", new Shortcut(KeyboardKey.J, ctrl: true), KeyboardKey.J),
                new("-", Shortcut.None, KeyboardKey.Null),
                new("Set Pass&word|Ctrl+W", new Shortcut(KeyboardKey.W, ctrl: true), KeyboardKey.W),
                new("Change &Timer|Ctrl+T", new Shortcut(KeyboardKey.T, ctrl: true), KeyboardKey.T),
                new("-", Shortcut.None, KeyboardKey.Null),
                new("&Retire|Ctrl+R", new Shortcut(KeyboardKey.R, ctrl: true), KeyboardKey.R),
                new("&Quit|Ctrl+Q", new Shortcut(KeyboardKey.Q, ctrl: true), KeyboardKey.Q,
                    commandId: QuitGame)
            },
        },

        new()
        {
            Key = "KINGDOM", Defaults = new List<MenuElement>
            {
                new("&Kingdom", Shortcut.None, KeyboardKey.K),
                new("&Tax Rate|Shift+T", new Shortcut(KeyboardKey.T, shift: true), KeyboardKey.T),
                new("-", Shortcut.None, KeyboardKey.Null),
                new("View T&hrone Room|Shift+H", new Shortcut(KeyboardKey.H, shift: true),
                    KeyboardKey.H),
                new("Find &City|Shift+C", new Shortcut(KeyboardKey.C, shift: true), KeyboardKey.C),
                new("-", Shortcut.None, KeyboardKey.Null),
                new("&REVOLUTION|Shift+R", new Shortcut(KeyboardKey.R, shift: true), KeyboardKey.R)
            },
        },


        new()
        {
            Key = "VIEW", Defaults = new List<MenuElement>
            {
                new("&View", Shortcut.None, KeyboardKey.V),
                new("&Move Pieces|v", new Shortcut(KeyboardKey.V), KeyboardKey.M),
                new("&View Pieces|v", new Shortcut(KeyboardKey.V), KeyboardKey.V),
                new("-", Shortcut.None, KeyboardKey.Null),
                new("Zoom &In|z", new Shortcut(KeyboardKey.Z), KeyboardKey.I, commandId: ZoomIn),
                new("Zoom &Out|X", new Shortcut(KeyboardKey.X), KeyboardKey.O, commandId: ZoomOut),
                new("-", Shortcut.None, KeyboardKey.Null),
                new("Max Zoom In|Ctrl+Z", new Shortcut(KeyboardKey.Z, ctrl: true),
                    KeyboardKey.Null, commandId: MaxZoomIn),
                new("Standard Zoom|Shift+Z", new Shortcut(KeyboardKey.Z, shift: true),
                    KeyboardKey.Null, commandId: StandardZoom),
                new("Medium Zoom Out|Shift+X", new Shortcut(KeyboardKey.X, shift: true),
                    KeyboardKey.Null, commandId: MediumZoomOut),
                new("Max Zoom Out|Ctrl+X", new Shortcut(KeyboardKey.X, ctrl: true),
                    KeyboardKey.Null, commandId: MaxZoomOut),
                new("-", Shortcut.None, KeyboardKey.Null),
                new("Show Map Grid|Ctrl+G", new Shortcut(KeyboardKey.G, ctrl: true),
                    KeyboardKey.Null, commandId: ShowMapGrid),
                new("Arrange Windows", Shortcut.None, KeyboardKey.Null),
                new("Show Hidden Terrain|t", new Shortcut(KeyboardKey.T), KeyboardKey.T),
                new("&Center View|c", new Shortcut(KeyboardKey.C), KeyboardKey.C)
            },
        },


        new()
        {
            Key = "@ORDERS", Defaults = new List<MenuElement>
            {
                new("&Orders", Shortcut.None, KeyboardKey.O),
                new("&Build New City|b", new Shortcut(KeyboardKey.B), KeyboardKey.B, BuildCityOrder, true),
                new("Build &Road|r", new Shortcut(KeyboardKey.R), KeyboardKey.R, BuildRoadOrder,
                    omitIfNoCommand: true),
                new("Build &Irrigation|i", new Shortcut(KeyboardKey.I), KeyboardKey.I, BuildIrrigationOrder,
                    omitIfNoCommand: true),
                new("Build &Mines|m", new Shortcut(KeyboardKey.M), KeyboardKey.M, BuildMineOrder,
                    omitIfNoCommand: true),
                new("Build %STRING0", Shortcut.None, KeyboardKey.Null, BuildIrrigationOrder,
                    omitIfNoCommand: true),
                new("Transform to ...|o", new Shortcut(KeyboardKey.O), KeyboardKey.T),
                new("Build &Airbase|e", new Shortcut(KeyboardKey.E), KeyboardKey.A),
                new("Build &Fortress|f", new Shortcut(KeyboardKey.F), KeyboardKey.F),
                new("Automate Settler|k", new Shortcut(KeyboardKey.K), KeyboardKey.Null),
                new("Clean Up &Pollution|p", new Shortcut(KeyboardKey.P), KeyboardKey.P),
                new("&Pillage|Shift+P", new Shortcut(KeyboardKey.P, shift: true), KeyboardKey.P),
                new("&Unload|u", new Shortcut(KeyboardKey.U), KeyboardKey.U),
                new("&Go To|g", new Shortcut(KeyboardKey.G), KeyboardKey.G),
                new("&Paradrop|p", new Shortcut(KeyboardKey.P), KeyboardKey.P),
                new("Air&lift|l", new Shortcut(KeyboardKey.L), KeyboardKey.L),
                new("Set &Home City|h", new Shortcut(KeyboardKey.H), KeyboardKey.H),
                new("&Fortify|f", new Shortcut(KeyboardKey.F), KeyboardKey.F),
                new("&Sleep|s", new Shortcut(KeyboardKey.S), KeyboardKey.S),
                new("&Disband|Shift+D", new Shortcut(KeyboardKey.D, shift: true), KeyboardKey.D),
                new("&Activate Unit|a", new Shortcut(KeyboardKey.A), KeyboardKey.A),
                new("&Wait|w", new Shortcut(KeyboardKey.W), KeyboardKey.W),
                new("S&kip Turn|SPACE", new Shortcut(KeyboardKey.Space), KeyboardKey.K),
                new("End Player Tur&n|Ctrl+N", new Shortcut(KeyboardKey.T, shift: true),
                    KeyboardKey.N, EndTurn)
            },
        },


        new()
        {
            Key = "ADVISORS", Defaults = new List<MenuElement>
            {
                new("&Advisors", Shortcut.None, KeyboardKey.A),
                new("Chat with &Kings|Ctrl+C", new Shortcut(KeyboardKey.C, ctrl: true),
                    KeyboardKey.K),
                new("Consult &High Council", Shortcut.None, KeyboardKey.H),
                new("-", Shortcut.None, KeyboardKey.Null),
                new("&City Status|F1", new Shortcut(KeyboardKey.F1), KeyboardKey.C),
                new("&Defense Minister|F2", new Shortcut(KeyboardKey.F2), KeyboardKey.D),
                new("&Foreign Minister|F3", new Shortcut(KeyboardKey.F3), KeyboardKey.F),
                new("-", Shortcut.None, KeyboardKey.Null),
                new("&Attitude Advisor|F4", new Shortcut(KeyboardKey.F4), KeyboardKey.A),
                new("&Trade Advisor|F5", new Shortcut(KeyboardKey.F5), KeyboardKey.T),
                new("&Science Advisor|F6", new Shortcut(KeyboardKey.F6), KeyboardKey.S),
                new("-", Shortcut.None, KeyboardKey.Null),
                new("Cas&ualty Timeline|Ctrl-D", new Shortcut(KeyboardKey.D, ctrl: true),
                    KeyboardKey.U)
            },
        },


        new()
        {
            Key = "WORLD", Defaults = new List<MenuElement>
            {
                new("&World", Shortcut.None, KeyboardKey.W),
                new("&Wonders of the World|F7", new Shortcut(KeyboardKey.F7), KeyboardKey.W),
                new("&Top 5 Cities|F8", new Shortcut(KeyboardKey.F8), KeyboardKey.T),
                new("&Civilization Score|F9", new Shortcut(KeyboardKey.F9), KeyboardKey.C),
                new("-", Shortcut.None, KeyboardKey.Null),
                new("&Demographics|F11", new Shortcut(KeyboardKey.F11), KeyboardKey.D),
                new("&Spaceships|F12", new Shortcut(KeyboardKey.F12), KeyboardKey.S)
            },
        },


        new()
        {
            Key = "CHEAT", Defaults = new List<MenuElement>
            {
                new("&Cheat", Shortcut.None, KeyboardKey.C),
                new("Toggle Cheat Mode|Ctrl+K", new Shortcut(KeyboardKey.K, ctrl: true),
                    KeyboardKey.Null),
                new("-", Shortcut.None, KeyboardKey.Null),
                new("Create &Unit|Shift+F1", new Shortcut(KeyboardKey.F1, shift: true),
                    KeyboardKey.U),
                new("Reveal &Map|Shift+F2", new Shortcut(KeyboardKey.F2, shift: true),
                    KeyboardKey.M, CheatRevealMapCommand),
                new("Set &Human Player|Shift+F3", new Shortcut(KeyboardKey.F3, shift: true),
                    KeyboardKey.H),
                new("-", Shortcut.None, KeyboardKey.Null),
                new("Set Game Year|Shift+F4", new Shortcut(KeyboardKey.F4, shift: true),
                    KeyboardKey.Null),
                new("&Kill Civilization|Shift+F5", new Shortcut(KeyboardKey.F5, shift: true),
                    KeyboardKey.K),
                new("-", Shortcut.None, KeyboardKey.Null),
                new("Te&chnology Advance|Shift+F6", new Shortcut(KeyboardKey.F6, shift: true),
                    KeyboardKey.C),
                new("&Edit Technologies|Ctrl+Shift+F6",
                    new Shortcut(KeyboardKey.F6, ctrl: true, shift: true), KeyboardKey.E),
                new("Force &Government|Shift+F7", new Shortcut(KeyboardKey.F7, shift: true),
                    KeyboardKey.G),
                new("Change &Terrain At Cursor|Shift+F8", new Shortcut(KeyboardKey.F8, shift: true),
                    KeyboardKey.T),
                new("Destro&Y All Units At Cursor|Ctrl+Shift+D",
                    new Shortcut(KeyboardKey.D, ctrl: true, shift: true), KeyboardKey.Y),
                new("Change Money|Shift+F9", new Shortcut(KeyboardKey.F9, shift: true),
                    KeyboardKey.Null, CheatChangeMoneyCommand),
                new("-", Shortcut.None, KeyboardKey.Null),
                new("Edit Unit|Ctrl+Shift+U", new Shortcut(KeyboardKey.U, ctrl: true, shift: true),
                    KeyboardKey.Null),
                new("Edit City|Ctrl+Shift+C", new Shortcut(KeyboardKey.C, ctrl: true, shift: true),
                    KeyboardKey.Null),
                new("Edit King|Ctrl+Shift+K", new Shortcut(KeyboardKey.K, ctrl: true, shift: true),
                    KeyboardKey.Null),
                new("-", Shortcut.None, KeyboardKey.Null),
                new("Scenario Parameters|Ctrl+Shift+P",
                    new Shortcut(KeyboardKey.P, ctrl: true, shift: true), KeyboardKey.Null),
                new("Save As Scenario|Ctrl+Shift+S",
                    new Shortcut(KeyboardKey.S, ctrl: true, shift: true), KeyboardKey.Null)
            },
        },


        new()
        {
            Key = "EDITOR", Defaults = new List<MenuElement>
            {
                new("&Editor", Shortcut.None, KeyboardKey.E),
                new("Toggle &Scenario Flag|Ctrl+F", new Shortcut(KeyboardKey.F, ctrl: true),
                    KeyboardKey.S),
                new("-", Shortcut.None, KeyboardKey.Null),
                new("&Advances Editor|Ctrl+Shift+1",
                    new Shortcut(KeyboardKey.One, ctrl: true, shift: true), KeyboardKey.A),
                new("&Cities Editor|Ctrl+Shift+2",
                    new Shortcut(KeyboardKey.Two, ctrl: true, shift: true), KeyboardKey.C),
                new("E&ffects Editor|Ctrl+Shift+3",
                    new Shortcut(KeyboardKey.Three, ctrl: true, shift: true), KeyboardKey.F),
                new("&Improvements Editor|Ctrl+Shift+4",
                    new Shortcut(KeyboardKey.Four, ctrl: true, shift: true), KeyboardKey.I),
                new("&Terrain Editor|Ctrl+Shift+5",
                    new Shortcut(KeyboardKey.Five, ctrl: true, shift: true), KeyboardKey.T),
                new("T&ribe Editor|Ctrl+Shift+6",
                    new Shortcut(KeyboardKey.Six, ctrl: true, shift: true), KeyboardKey.R),
                new("&Units Editor|Ctrl+Shift+7",
                    new Shortcut(KeyboardKey.Seven, ctrl: true, shift: true), KeyboardKey.U),
                new("&Events Editor|Ctrl+Shift+8",
                    new Shortcut(KeyboardKey.Eight, ctrl: true, shift: true), KeyboardKey.E),
                new("-", Shortcut.None, KeyboardKey.Null),
                new("Lua Console|Ctrl+Shift+9", new Shortcut(KeyboardKey.Nine, true, true), KeyboardKey.L,
                    omitIfNoCommand: true, commandId: OpenLuaConsole)
            },
        },


        new()
        {
            Key = "PEDIA", Defaults = new List<MenuElement>
            {
                new("&Civilopedia", Shortcut.None, KeyboardKey.C),
                new("Civilization &Advances", Shortcut.None, KeyboardKey.A),
                new("City &Improvements", Shortcut.None, KeyboardKey.I),
                new("&Wonders of the World", Shortcut.None, KeyboardKey.W),
                new("Military &Units", Shortcut.None, KeyboardKey.U),
                new("-", Shortcut.None, KeyboardKey.Null),
                new("&Governments", Shortcut.None, KeyboardKey.G),
                new("&Terrain Types", Shortcut.None, KeyboardKey.T),
                new("-", Shortcut.None, KeyboardKey.Null),
                new("Game &Concepts", Shortcut.None, KeyboardKey.C),
                new("-", Shortcut.None, KeyboardKey.Null),
                new("&About Civilization II", Shortcut.None, KeyboardKey.A)
            },
        }
    ];

    public override int UnitsRows => 7;
    public override int UnitsPxHeight => 48;
    public override Dictionary<string, IImageSource[]> PicSources { get; } = new();

    public override void GetShieldImages()
    {
        Color shadowColour = new(51, 51, 51, 255);
        Color replacementColour = new(255, 0, 0, 255);

        var shield = Images.ExtractBitmap(PicSources["backShield1"][0], this);
        var shieldFront = shield.Copy();
        var shieldBack = shield.Copy();
        shieldFront.DrawRectangle(0, 0, shieldFront.Width, 7, Color.Black);
        shield.ReplaceColor(replacementColour, shadowColour);

        UnitImages.Shields = new MemoryStorage(shieldFront, "Unit-Shield", replacementColour);
        UnitImages.ShieldBack = new MemoryStorage(shieldBack, "Unit-Shield-Back", replacementColour, true);
        UnitImages.ShieldShadow = new MemoryStorage(shield, "Unit-Shield-Shadow");
    }

    public override void LoadPlayerColours()
    {
        var playerColours = new PlayerColour[9];
        for (var col = 0; col < 9; col++)
        {
            var imageColours = Images.ExtractBitmap(PicSources["textColours"][col], this).LoadColors();
            var textColour = imageColours[0];

            imageColours = Images.ExtractBitmap(PicSources["flags"][col], this).LoadColors();
            var lightColour = imageColours[3 * Images.ExtractBitmap(PicSources["flags"][col], this).Width + 8];

            imageColours = Images.ExtractBitmap(PicSources["flags"][9 + col], this).LoadColors();
            var darkColour = imageColours[3 * Images.ExtractBitmap(PicSources["flags"][9 + col], this).Width + 5];
            Image.UnloadColors(imageColours);

            playerColours[col] = new PlayerColour
            {
                Image = PicSources["flags"][col],
                TextColour = textColour,
                LightColour = lightColour,
                DarkColour = darkColour
            };
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
        HPbarColours = [new Color(243, 0, 0, 255), new Color(255, 223, 79, 255), new Color(87, 171, 39, 255)],
        HPbarSizeForColours = [3, 8],
        OrderOffset = new(Images.ExtractBitmap(PicSources["backShield1"][0], this).Width / 2f, 7),
        OrderTextHeight = Images.ExtractBitmap(PicSources["backShield1"][0], this).Height - 7,
    };

    /// <summary>
    /// Draw outer border wallpaper around panel
    /// </summary>
    /// <param name="wallpaper">Wallpaper image to tile onto the border</param>
    /// <param name="destination">the Image we're rendering to</param>
    /// <param name="height">final image Height</param>
    /// <param name="width">final image Width</param>
    /// <param name="padding">padding of borders</param>
    /// <param name="statusPanel">is this status panel?</param>
    public override void DrawBorderWallpaper(Wallpaper wallpaper, ref Image destination, int height, int width, Padding padding, bool statusPanel)
    {
        var rows = height / wallpaper.Outer.Height + 1;
        var columns = width / wallpaper.Outer.Width + 1;
        var headerSourceRec = new Rectangle { Height = padding.Top, Width = wallpaper.Outer.Width };
        for (var col = 0; col < columns; col++)
        {
            destination.Draw(wallpaper.Outer, headerSourceRec,
                new Rectangle(col * wallpaper.Outer.Width, 0, wallpaper.Outer.Width, padding.Top), Color.White);
        }
        var leftSide = new Rectangle { Height = wallpaper.Outer.Height, Width = DialogPadding.Left };

        var rightEdge = width - DialogPadding.Right;
        var rightOffset = width % wallpaper.Outer.Width;
        var rightSide = new Rectangle { X = rightOffset, Height = wallpaper.Outer.Height, Width = DialogPadding.Right };

        for (var row = 0; row < rows; row++)
        {
            destination.Draw(wallpaper.Outer, leftSide,
                new Rectangle(0, row * wallpaper.Outer.Height, DialogPadding.Left, wallpaper.Outer.Height), Color.White);
            destination.Draw(wallpaper.Outer, rightSide,
                new Rectangle(rightEdge, row * wallpaper.Outer.Height, DialogPadding.Right, wallpaper.Outer.Height), Color.White);
        }

        var bottomEdge = height - padding.Bottom;
        var bottomOffset = height % wallpaper.Outer.Height;
        var bottomSource = new Rectangle { Y = bottomOffset, Height = padding.Bottom, Width = wallpaper.Outer.Width };
        for (var col = 0; col < columns; col++)
        {
            destination.Draw(wallpaper.Outer, bottomSource,
                new Rectangle(col * wallpaper.Outer.Width, bottomEdge, wallpaper.Outer.Width, padding.Bottom), Color.White);
        }

        if (statusPanel)
        {
            columns = (width - padding.Left - padding.Right) / wallpaper.Outer.Width + 1;
            var sourceRec = new Rectangle { Height = 4, Width = wallpaper.Outer.Width };
            for (var col = 0; col < columns; col++)
            {
                destination.Draw(wallpaper.Outer, sourceRec,
                    new Rectangle(col * wallpaper.Outer.Width, padding.Top + 62, wallpaper.Outer.Width, 4), Color.White);
            }
        }
    }

    public override void DrawBorderLines(ref Image destination, int height, int width, Padding padding, bool statusPanel)
    {
        // Outer border
        var pen1 = new Color(227, 227, 227, 255);
        var pen2 = new Color(105, 105, 105, 255);
        var pen3 = new Color(255, 255, 255, 255);
        var pen4 = new Color(160, 160, 160, 255);
        var pen5 = new Color(240, 240, 240, 255);
        var pen6 = new Color(223, 223, 223, 255);
        var pen7 = new Color(67, 67, 67, 255);
        destination.DrawLine(0, 0, width - 2, 0, pen1); // 1st layer of border
        destination.DrawLine(0, 0, width - 2, 0, pen1);
        destination.DrawLine(0, 0, 0, height - 2, pen1);
        destination.DrawLine(width - 1, 0, width - 1, height - 1, pen2);
        destination.DrawLine(0, height - 1, width - 1, height - 1, pen2);
        destination.DrawLine(1, 1, width - 3, 1, pen3); // 2nd layer of border
        destination.DrawLine(1, 1, 1, height - 3, pen3);
        destination.DrawLine(width - 2, 1, width - 2, height - 2, pen4);
        destination.DrawLine(1, height - 2, width - 2, height - 2, pen4);
        destination.DrawLine(2, 2, width - 4, 2, pen5); // 3rd layer of border
        destination.DrawLine(2, 2, 2, height - 4, pen5);
        destination.DrawLine(width - 3, 2, width - 3, height - 3, pen5);
        destination.DrawLine(2, height - 3, width - 3, height - 3, pen5);
        destination.DrawLine(3, 3, width - 5, 3, pen6); // 4th layer of border
        destination.DrawLine(3, 3, 3, height - 5, pen6);
        destination.DrawLine(width - 4, 3, width - 4, height - 4, pen7);
        destination.DrawLine(3, height - 4, width - 4, height - 4, pen7);
        destination.DrawLine(4, 4, width - 6, 4, pen6); // 5th layer of border
        destination.DrawLine(4, 4, 4, height - 6, pen6);
        destination.DrawLine(width - 5, 4, width - 5, height - 5, pen7);
        destination.DrawLine(4, height - 5, width - 5, height - 5, pen7);

        // Inner panel
        destination.DrawLine(9, padding.Top - 1, 9 + (width - 18 - 1), padding.Top - 1, pen7); // 1st layer of border
        if (!statusPanel)
        {
            // 1st layer of border
            destination.DrawLine(10, padding.Top - 1, 10, height - padding.Bottom - 1, pen7);
            destination.DrawLine(width - 11, padding.Top - 1, width - 11, height - padding.Bottom - 1, pen6);
            destination.DrawLine(9, height - padding.Bottom, width - 9 - 1, height - padding.Bottom, pen6);
            destination.DrawLine(10, padding.Top - 2, 9 + (width - 18 - 2), padding.Top - 2, pen7); // 2nd layer of border
            destination.DrawLine(9, padding.Top - 2, 9, height - padding.Bottom, pen7);
            destination.DrawLine(width - 10, padding.Top - 2, width - 10, height - padding.Bottom, pen6);
            destination.DrawLine(9, height - padding.Bottom + 1, width - 9 - 1, height - padding.Bottom + 1, pen6);
        }
        else
        {
            // 1st layer of border
            destination.DrawLine(9, padding.Top + 67, 9 + (width - 18 - 1), padding.Top + 67, pen7);
            destination.DrawLine(10, padding.Top - 1, 10, padding.Top + 59, pen7);
            destination.DrawLine(10, padding.Top + 66, 10, height - padding.Bottom - 1, pen7);
            destination.DrawLine(width - 11, padding.Top - 1, width - 11, padding.Top + 61, pen6);
            destination.DrawLine(width - 11, padding.Top + 67, width - 11, height - padding.Bottom - 1, pen6);
            destination.DrawLine(10, height - padding.Bottom, width - 9 - 1, height - padding.Bottom, pen6);
            destination.DrawLine(10, padding.Top + 60, width - 9 - 1, padding.Top + 60, pen6);
            destination.DrawLine(10, padding.Top - 2, 9 + (width - 18 - 2), padding.Top - 2, pen7); // 2nd layer of border
            destination.DrawLine(10, padding.Top + 66, 9 + (width - 18 - 2), padding.Top + 66, pen7);
            destination.DrawLine(9, padding.Top - 2, 9, padding.Top + 60, pen7);
            destination.DrawLine(9, padding.Top + 66, 9, height - padding.Bottom, pen7);
            destination.DrawLine(width - 10, padding.Top - 2, width - 10, padding.Top + 59, pen6);
            destination.DrawLine(width - 10, padding.Top + 66, width - 10, height - padding.Bottom - 1, pen6);
            destination.DrawLine(9, height - padding.Bottom + 1, width - 9 - 1, height - padding.Bottom + 1, pen6);
            destination.DrawLine(9, padding.Top + 61, width - 9 - 1, padding.Top + 61, pen6);
        }
    }

    public override void DrawButton(Texture2D texture, int x, int y, int w, int h)
    {
        Graphics.DrawRectangleLinesEx(new Rectangle(x, y, w, h), 1.0f, new Color(100, 100, 100, 255));
        Graphics.DrawRectangleRec(new Rectangle(x + 1, y + 1, w - 2, h - 2), Color.White);
        Graphics.DrawRectangleRec(new Rectangle(x + 3, y + 3, w - 6, h - 6), new Color(192, 192, 192, 255));
        Graphics.DrawLine(x + 2, y + h - 2, x + w - 2, y + h - 2, new Color(128, 128, 128, 255));
        Graphics.DrawLine(x + 3, y + h - 3, x + w - 2, y + h - 3, new Color(128, 128, 128, 255));
        Graphics.DrawLine(x + w - 1, y + 2, x + w - 1, y + h - 1, new Color(128, 128, 128, 255));
        Graphics.DrawLine(x + w - 2, y + 3, x + w - 2, y + h - 1, new Color(128, 128, 128, 255));
    }
    
    protected override IEnumerable<Ruleset> GenerateRulesets(string path, string title)
    {
        var rules = FileUtilities.GetFile(path, "rules.txt");
        if (rules != null)
        {
            yield return new Ruleset(title, new Dictionary<string, string>
            {
                { "Civ2Gold", "Standard" }
            }, path);

            foreach (var subdirectory in Directory.EnumerateDirectories(path))
            {
                var scnRules = FileUtilities.GetFile(subdirectory, "rules.txt");
                if (scnRules != null)
                {

                    var game = Utils.GetFilePath("game.txt", [subdirectory]);
                    var name = "";
                    if (File.Exists(game))
                    {
                        foreach (var line in File.ReadLines(game))
                        {
                            if (!line.StartsWith("@title")) continue;
                            name = line[7..];
                            break;
                        }
                    }

                    if (string.IsNullOrWhiteSpace(name))
                    {
                        name = Path.GetFileName(subdirectory);
                    }

                    yield return new Ruleset(name, new Dictionary<string, string>
                    {

                        { "Civ2Gold", "Scenario-" + name }
                    }, subdirectory, path);
                }
            }
        }
    }
}