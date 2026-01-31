using Model.Images;
using Raylib_CSharp.Transformations;
using System.Numerics;

namespace Model.Controls;

public class CityWindowLayout
{
    public CityWindowLayout(IImageSource image)
    {
        Image = image;
    }

    public IImageSource Image { get; init; }
    public int Height { get; init; }
    public int Width { get; init; }
    public IDictionary<string, CityLabelProperties> Labels { get; } = new Dictionary<string, CityLabelProperties>();
    public IDictionary<string, CityButtonProperties> Buttons { get; } = new Dictionary<string, CityButtonProperties>();
    public InfoPanel InfoPanel { get; set; }
    public Rectangle TileMap { get; set; }
    public ResourceProduction Resources { get; set; }
    public ShieldProduction Production { get; init; }
    public UnitBox UnitSupport { get; init; }
    public ImprovementsBox Improvements { get; init; }
    public Rectangle FoodStorage { get; set; }
    public Rectangle CitizensBox { get; set; }
}