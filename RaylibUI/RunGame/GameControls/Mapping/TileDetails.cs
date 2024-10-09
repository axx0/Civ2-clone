using Civ2engine.Enums;
using Model.Images;
using Raylib_CSharp.Images;

namespace RaylibUI.RunGame.GameControls.Mapping;

public class TileDetails
{
    public Image Image { get; set; }
    public ForegroundImprovement? ForegroundElement { get; set; }
}

public class ForegroundImprovement
{
    public IImageSource Image { get; set; }
}

public class UnitHidingImprovement : ForegroundImprovement
{
    public IImageSource UnitImage { get; set; }
    
    public UnitGas UnitDomain { get; set; }
}