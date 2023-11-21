using Model.Images;

namespace Model;

public class ResourceImage
{
    public ResourceImage(string name, BitmapStorage largeImage, BitmapStorage smallImage, BitmapStorage lossImage)
    {
        Name = name;
        LargeImage = largeImage;
        LossImage = lossImage;
        SmallImage = smallImage;
    }

    public string Name { get; set; }
    public BitmapStorage LargeImage { get; set; }
    public BitmapStorage SmallImage { get; set; }
    public BitmapStorage LossImage { get; set; }
}