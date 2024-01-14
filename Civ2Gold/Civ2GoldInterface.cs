using System;
using System.Numerics;
using Civ2;
using Civ2engine;
using JetBrains.Annotations;
using Model;
using Model.Images;
using Model.ImageSets;
using Model.Interface;
using Raylib_cs;
using RayLibUtils;

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
        CityWindowFontSize = 16
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
        Raylib.ImageDrawRectangle(ref shieldFront, 0, 0, shieldFront.width, 7, Color.BLACK);

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
                var lightColour = imageColours[3 * CitiesPICprops["flags"][col].Image.width + 8];

                imageColours = Raylib.LoadImageColors(CitiesPICprops["flags"][9 + col].Image);
                var darkColour = imageColours[3 * CitiesPICprops["flags"][9 + col].Image.width + 5];
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
        StackingOffset = new(UnitImages.Units[unitType].FlagLoc.X < UnitImages.UnitRectangle.width / 2 ? -4 : 4, 0),
        ShadowOffset = new(UnitImages.Units[unitType].FlagLoc.X < UnitImages.UnitRectangle.width / 2 ? -1 : 1, 1),
        DrawShadow = true,
        HPbarOffset = new(0, 2),
        HPbarSize = new(12, 3),
        HPbarColours = new[] { new Color(243, 0, 0, 255), new Color(255, 223, 79, 255), new Color(87, 171, 39, 255) },
        HPbarSizeForColours = new[] { 3, 8 },
        OrderOffset = new(UnitPICprops["backShield1"][0].Image.width / 2f, 7),
        OrderTextHeight = UnitPICprops["backShield1"][0].Image.height - 7,
    };

    /// <summary>
    /// Draw outer border wallpaper around panel
    /// </summary>
    /// <param name="wallpaper">Wallpaper image to tile onto the border</param>
    /// <param name="destination">the Image we're rendering to</param>
    /// <param name="height">final image height</param>
    /// <param name="width">final image width</param>
    /// <param name="topWidth">width of top border</param>
    /// <param name="footerWidth">width of the footer</param>
    public override void DrawBorderWallpaper(Wallpaper wallpaper, ref Image destination, int height, int width, Padding padding)
    {
        int rows = height / wallpaper.Outer.height + 1;
        var columns = width / wallpaper.Outer.width + 1;
        var headerSourceRec = new Rectangle { height = padding.Top, width = wallpaper.Outer.width };
        for (int col = 0; col < columns; col++)
        {
            Raylib.ImageDraw(ref destination, wallpaper.Outer, headerSourceRec,
                new Rectangle(col * wallpaper.Outer.width, 0, wallpaper.Outer.width, padding.Top),
                Color.WHITE);
        }
        var leftSide = new Rectangle { height = wallpaper.Outer.height, width = DialogPadding.Left };

        var rightEdge = width - DialogPadding.Right;
        var rightOffset = width % wallpaper.Outer.width;
        var rightSide = new Rectangle { x = rightOffset, height = wallpaper.Outer.height, width = DialogPadding.Right };

        for (int row = 0; row < rows; row++)
        {
            Raylib.ImageDraw(ref destination, wallpaper.Outer, leftSide,
                new Rectangle(0, row * wallpaper.Outer.height, DialogPadding.Left, wallpaper.Outer.height),
                Color.WHITE);
            Raylib.ImageDraw(ref destination, wallpaper.Outer, rightSide,
                new Rectangle(rightEdge, row * wallpaper.Outer.height, DialogPadding.Right, wallpaper.Outer.height),
                Color.WHITE);
        }

        var bottomEdge = height - padding.Bottom;
        var bottomOffset = height % wallpaper.Outer.height;
        var bottomSource = new Rectangle { y = bottomOffset, height = padding.Bottom, width = wallpaper.Outer.width };
        for (int col = 0; col < columns; col++)
        {
            Raylib.ImageDraw(ref destination, wallpaper.Outer, bottomSource,
                new Rectangle(col * wallpaper.Outer.width, bottomEdge, wallpaper.Outer.width, padding.Bottom),
                Color.WHITE);
        }
    }

    public override void DrawBorderLines(ref Image destination, int height, int width, Padding padding)
    {
        // Outer border
        var pen1 = new Color(227, 227, 227, 255);
        var pen2 = new Color(105, 105, 105, 255);
        var pen3 = new Color(255, 255, 255, 255);
        var pen4 = new Color(160, 160, 160, 255);
        var pen5 = new Color(240, 240, 240, 255);
        var pen6 = new Color(223, 223, 223, 255);
        var pen7 = new Color(67, 67, 67, 255);
        Raylib.ImageDrawLine(ref destination, 0, 0, width - 2, 0, pen1); // 1st layer of border
        Raylib.ImageDrawLine(ref destination, 0, 0, width - 2, 0, pen1);
        Raylib.ImageDrawLine(ref destination, 0, 0, 0, height - 2, pen1);
        Raylib.ImageDrawLine(ref destination, width - 1, 0, width - 1, height - 1, pen2);
        Raylib.ImageDrawLine(ref destination, 0, height - 1, width - 1, height - 1, pen2);
        Raylib.ImageDrawLine(ref destination, 1, 1, width - 3, 1, pen3); // 2nd layer of border
        Raylib.ImageDrawLine(ref destination, 1, 1, 1, height - 3, pen3);
        Raylib.ImageDrawLine(ref destination, width - 2, 1, width - 2, height - 2, pen4);
        Raylib.ImageDrawLine(ref destination, 1, height - 2, width - 2, height - 2, pen4);
        Raylib.ImageDrawLine(ref destination, 2, 2, width - 4, 2, pen5); // 3rd layer of border
        Raylib.ImageDrawLine(ref destination, 2, 2, 2, height - 4, pen5);
        Raylib.ImageDrawLine(ref destination, width - 3, 2, width - 3, height - 3, pen5);
        Raylib.ImageDrawLine(ref destination, 2, height - 3, width - 3, height - 3, pen5);
        Raylib.ImageDrawLine(ref destination, 3, 3, width - 5, 3, pen6); // 4th layer of border
        Raylib.ImageDrawLine(ref destination, 3, 3, 3, height - 5, pen6);
        Raylib.ImageDrawLine(ref destination, width - 4, 3, width - 4, height - 4, pen7);
        Raylib.ImageDrawLine(ref destination, 3, height - 4, width - 4, height - 4, pen7);
        Raylib.ImageDrawLine(ref destination, 4, 4, width - 6, 4, pen6); // 5th layer of border
        Raylib.ImageDrawLine(ref destination, 4, 4, 4, height - 6, pen6);
        Raylib.ImageDrawLine(ref destination, width - 5, 4, width - 5, height - 5, pen7);
        Raylib.ImageDrawLine(ref destination, 4, height - 5, width - 5, height - 5, pen7);

        // Inner panel
        Raylib.ImageDrawLine(ref destination, 9, padding.Top - 1, 9 + (width - 18 - 1), padding.Top - 1, pen7); // 1st layer of border
        Raylib.ImageDrawLine(ref destination, 10, padding.Top - 1, 10, height - padding.Bottom - 1, pen7);
        Raylib.ImageDrawLine(ref destination, width - 11, padding.Top - 1, width - 11, height - padding.Bottom - 1, pen6);
        Raylib.ImageDrawLine(ref destination, 9, height - padding.Bottom, width - 9 - 1, height - padding.Bottom, pen6);
        Raylib.ImageDrawLine(ref destination, 10, padding.Top - 2, 9 + (width - 18 - 2), padding.Top - 2, pen7); // 2nd layer of border
        Raylib.ImageDrawLine(ref destination, 9, padding.Top - 2, 9, height - padding.Bottom, pen7);
        Raylib.ImageDrawLine(ref destination, width - 10, padding.Top - 2, width - 10, height - padding.Bottom, pen6);
        Raylib.ImageDrawLine(ref destination, 9, height - padding.Bottom + 1, width - 9 - 1, height - padding.Bottom + 1, pen6);
    }
}