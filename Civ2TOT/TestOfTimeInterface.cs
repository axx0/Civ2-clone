using Civ2;
using JetBrains.Annotations;
using Model;
using Model.Images;

namespace TOT;

/// <summary>
/// This class is dynamically instantiated when ToT files are detected
/// </summary>
[UsedImplicitly]
public class TestOfTimeInterface : Civ2Interface
{
    public override string Title => "Test of Time";

    public override string InitialMenu => "MAINMENU";//"STARTMENU";

    public override void Initialize()
    {
        base.Initialize();

        //DialogHandlers["STARTMENU"].Dialog.Decorations.Add(new Decoration(ObservatoryPic, new Point(0.08, 0.09)));
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
}