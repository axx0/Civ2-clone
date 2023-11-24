using System.Numerics;
using Model.Images;
using Raylib_cs;

namespace Model;

public class CityWindowLayout
{
    public CityWindowLayout(IImageSource image)
    {
        Image = image;
    }

    public IImageSource Image { get; init; }
    public int Height { get; init; }
    public int Width { get; init; }
    public IDictionary<string, Rectangle> Buttons { get; } = new Dictionary<string, Rectangle>();
    public Rectangle InfoPanel { get; set; }
    public Rectangle?TileMap { get; set; }
    
    public ResourceProduction Resources { get; set; }
}

public class ResourceProduction
{
    public Vector2 TitlePosition { get; set; }
    public IList<ResourceArea> Resources { get; set; }
}