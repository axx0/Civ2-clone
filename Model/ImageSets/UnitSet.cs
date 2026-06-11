using Model.Images;
using Raylib_CSharp.Transformations;

namespace Model.ImageSets;

public class UnitSet
{
    public Rectangle UnitRectangle { get; set; }
    public UnitImage[] Units { get; set; } = [];
    public IImageSource ShieldsSource { get; set; } = null!;
    public IImageSource Shields { get; set; } = null!;
    public IImageSource ShieldBack { get; set; } = null!;
    public IImageSource ShieldShadow { get; set; } = null!;
    public IImageSource[] BattleAnim { get; set; } = [];
    public IImageSource Fortify { get; set; } = null!;
}
