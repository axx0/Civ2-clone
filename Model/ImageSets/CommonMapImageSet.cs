using Model.Images;

namespace Model.ImageSets;

public class CommonMapImageSet
{
    public IImageSource ViewPiece { get; set; }
    public IImageSource GridLinesVisible { get; set; }
    public IImageSource GridLines { get; set; }
}