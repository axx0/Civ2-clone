using Raylib_CSharp.Colors;
using Raylib_CSharp.Transformations;

namespace Model.Controls;

public class CityButtonProperties
{
    public string Text { get; init; }
    public Rectangle Box { get; init; }

    public CityButtonProperties(string text, Rectangle box)
    {
        Text = text;
        Box = box;
    }
}
