using Raylib_CSharp.Transformations;

namespace Model.Images;

public record BinaryStorage : IImageSource
{
    public ImageStorage Type => ImageStorage.Binary;
    public string GetKey(int ownerId = -1)
    {
        return Key;
    }

    public string Key => $"Binary-{Filename}-{DataStart}-{Location}";
    public string Filename { get; }
    public int DataStart { get; }
    public int Length { get; }
    public Rectangle Location { get; }

    public BinaryStorage(string file, int dataStart, int length, Rectangle location)
    {
        Filename = Path.HasExtension(file) ? file : Path.GetFileName(file);
        DataStart = dataStart;
        Length = length;
        Location = location;
    }

    public BinaryStorage(string file, int dataStart, int length) : this(file, dataStart, length, new Rectangle(0, 0, 0, 0))
    {
    }
}