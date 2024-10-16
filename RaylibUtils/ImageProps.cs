using Raylib_CSharp.Images;
using Raylib_CSharp.Transformations;
using System.Numerics;

namespace RaylibUtils;

public class ImageProps
{
    public Image Image { get; set; }
    public Rectangle Rect { get; set; }
    public Vector2 Flag1 { get; set; }
    public Vector2 Flag2 { get; set; }
}
