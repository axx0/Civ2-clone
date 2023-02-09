using Civ;
using JetBrains.Annotations;
using Model;
using Model.Images;

namespace Civ2;

[UsedImplicitly]
public class Civ2Interface : CivInterface
{
    public override string Title => "Civilization II Multiplayer Gold";

    public override void Initialize()
    {
        base.Initialize();
        
        DialogHandlers["MAINMENU"].Dialog.Decorations.Add(new Decoration(SinaiPic,new Point(160,76)));
    }

    private static readonly IImageSource SinaiPic = new BinaryStorage
        { FileName = "Intro.dll", DataStart = 0x1E630, Length = 0x9F78 };

    public override IImageSource BackgroundImage => new BinaryStorage
        { FileName = "Tiles.dll", DataStart = 0xF7454, Length = 0x1389D };

}