using Model.CityWindowModel;
using Model.Images;
using Raylib_CSharp.Transformations;
using System.Numerics;

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
    public Rectangle TileMap { get; set; }
    public ResourceProduction Resources { get; set; }

    //public ShieldProduction Production { get; set; } = new();
    public ShieldProduction Production { get; init; }
    public UnitSupport UnitSupport { get; init; }
    public Rectangle FoodStorage { get; set; }
}