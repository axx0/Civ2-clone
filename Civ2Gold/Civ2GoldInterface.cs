using Civ2;
using JetBrains.Annotations;
using Model;
using Model.Images;

namespace Civ2Gold;

[UsedImplicitly]
public class Civ2GoldInterface : Civ2Interface
{
    public override string Title => "Civilization II Multiplayer Gold";

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
}