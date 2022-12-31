namespace Model.Images;

public interface IImageSource
{
    ImageStorage Type { get; }
    string Key { get; }
}