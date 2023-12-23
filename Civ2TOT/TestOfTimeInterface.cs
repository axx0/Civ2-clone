using System.Numerics;
using System.Runtime.Intrinsics.X86;
using Civ2;
using Civ2.Dialogs;
using Civ2engine;
using JetBrains.Annotations;
using Model;
using Model.Images;
using Model.ImageSets;
using Raylib_cs;
using RayLibUtils;

namespace TOT;

/// <summary>
/// This class is dynamically instantiated when ToT files are detected
/// </summary>
[UsedImplicitly]
public class TestOfTimeInterface : Civ2Interface
{
    public override string Title => "Test of Time";

    public override string InitialMenu => "STARTMENU";

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
}