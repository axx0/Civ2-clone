using Raylib_cs;

namespace Model.Images;

public class BitmapStorage : IImageSource
{
    public ImageStorage Type => ImageStorage.Bitmap;
    public string Key => $"{Filename}-{Location}";
    
    public string Filename { get; }
    
    public Rectangle Location { get; }

    public BitmapStorage(string file, Rectangle location)
    {
        Filename = file;
        Location = location;
    }
}