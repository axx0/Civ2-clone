using Model.Images;
using Raylib_cs;

namespace Model.ImageSets;

public class UnitSet
{
    public Rectangle UnitRectangle { get; set; }
    public UnitImage[] Units { get; set; }
    public IImageSource Shields { get; set; }
    public IImageSource ShieldBack { get; set; }
    public IImageSource ShieldShadow { get; set; }
    public Texture2D[] BattleAnim { get; set; }
    public Texture2D Fortify { get; set; }
}