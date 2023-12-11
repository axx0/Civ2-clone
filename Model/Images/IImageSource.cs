namespace Model.Images;

public interface IImageSource
{
    ImageStorage Type { get; }
    string GetKey(int ownerId = -1);
}