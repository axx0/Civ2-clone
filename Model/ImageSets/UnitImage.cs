using System.Numerics;
using Model.Images;

namespace Model.ImageSets;

public class UnitImage
{
    public IImageSource Image { get; set; } = null!;
    public Vector2 FlagLoc { get; set; }
}
