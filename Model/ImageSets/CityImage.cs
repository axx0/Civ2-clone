using System.Numerics;
using Model.Images;

namespace Model.ImageSets;

public class CityImage
{
    public IImageSource Image { get; set; }
    public Vector2 FlagLoc { get; set; }
    public Vector2 SizeLoc { get; set; }
}