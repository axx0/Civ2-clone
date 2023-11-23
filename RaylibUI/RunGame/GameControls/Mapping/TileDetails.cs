using Civ2engine.Enums;
using Raylib_cs;

namespace RaylibUI.RunGame.GameControls.Mapping;

public class TileDetails
{
    public Image Image { get; set; }
    public ForegroundImprovement? ForegroundElement { get; set; }
}

public class ForegroundImprovement
{
    public Image Image { get; set; }
    public int OwnerId { get; set; }
    
    public Color? PlayerReplacementColor { get; set; }
}

public class UnitHidingImprovement : ForegroundImprovement
{
    public Image UnitImage { get; set; }
    
    public UnitGAS UnitDomain { get; set; }
}