using Raylib_CSharp.Colors;
using Raylib_CSharp.Transformations;
using System.Numerics;

namespace Model.Controls;

public class ImprovementsBox
{
    public Rectangle Box { get; set; }
    public int Rows { get; set; }
    public Color LabelColor { get; set; }
    public Color LabelColorShadow { get; set; }
    public Vector2 ShadowOffset { get; set; }
}