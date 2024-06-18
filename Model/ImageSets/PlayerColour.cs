using Model.Images;
using Raylib_cs;

namespace Model.ImageSets;

public class PlayerColour
{
    public IImageSource Image { get; set; }
    public Color DarkColour { get; set; }
    public Color TextColour { get; set; }
    public Color LightColour { get; set; }
}