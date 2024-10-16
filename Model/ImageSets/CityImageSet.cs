using Raylib_CSharp.Transformations;

namespace Model.ImageSets;

public class CityImageSet
{
    public Rectangle CityRectangle { get; set; }
    public List<CityImage[]> Sets { get; } = new();
}