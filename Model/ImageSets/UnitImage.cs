using System.Numerics;
using Model.Images;

namespace Model.ImageSets;

public class UnitImage
{
    public IImageSource Image { get; set; }
    public Vector2 FlagLoc { get; set; }
}