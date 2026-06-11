using Model.Images;

namespace Model.ImageSets;

public class CommonMapImageSet
{
    public IImageSource ViewPiece { get; set; } = null!;
    public IImageSource GridLinesVisible { get; set; } = null!;
    public IImageSource GridLines { get; set; } = null!;
}
