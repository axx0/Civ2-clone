using System.Numerics;
using Raylib_CSharp.Transformations;

namespace Model.Controls;

public class ShieldProduction
{
    public string Type { get; set; } = string.Empty;
    public Rectangle Box { get; set; }
    
    public Vector2 IconLocation;
}
