using System.Numerics;
using Raylib_cs;

namespace Model.ImageSets;

public class CityImage
{
    public Image Image { get; set; }
    public Vector2 FlagLoc { get; set; }
    public Vector2 SizeLoc { get; set; }
    public Texture2D Texture { get; set; }
}