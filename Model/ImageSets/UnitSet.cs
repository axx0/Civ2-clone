using Model.Images;
using Raylib_CSharp.Transformations;

namespace Model.ImageSets;

public class UnitSet
{
    public Rectangle UnitRectangle { get; set; }
    public UnitImage[] Units { get; set; }
    public IImageSource ShieldsSource { get; set; }
    public IImageSource Shields { get; set; }
    public IImageSource ShieldBack { get; set; }
    public IImageSource ShieldShadow { get; set; }
    public IImageSource[] BattleAnim { get; set; }
    public IImageSource Fortify { get; set; }
}