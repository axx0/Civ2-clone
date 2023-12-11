namespace Model.Images;

public class ComputedImage : IImageSource
{
    public ImageStorage Type => ImageStorage.Computed;
    public string GetKey(int ownerId = -1)
    {
        throw new NotImplementedException();
    }
}