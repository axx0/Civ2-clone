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

    public override IImageSource BackgroundImage => new BinaryStorage
        { FileName = "Tiles.dll", DataStart = 0xF7454, Length = 0x1389D };

}