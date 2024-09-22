using System.Numerics;
using Raylib_cs;

namespace Model.CityWindowModel;

public class ShieldProduction
{
    public string Type { get; set; } = "Box";
    public Vector2 TitlePosition { get; set; } = new (534.5f, 165);
    public Rectangle? ShieldBox { get; set; } = new Rectangle(444, 210, 179, 145);
}