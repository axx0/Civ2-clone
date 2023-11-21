using Raylib_cs;

namespace Model;

public class CityWindowLayout
{
    public Image Image { get; init; }
    public int Height { get; init; }
    public int Width { get; init; }
    public IDictionary<string, Rectangle> Buttons { get; } = new Dictionary<string, Rectangle>();
    public Rectangle InfoPanel { get; set; }
}