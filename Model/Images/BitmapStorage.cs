using Raylib_cs;

namespace Model.Images;

public class BitmapStorage : IImageSource
{
    public ImageStorage Type => ImageStorage.Bitmap;
    public string Key => $"{Filename}-{Location}";
    
    public string Filename { get; }
    
    public Rectangle Location { get; }
    public string[] Extension { get; }

    public BitmapStorage(string file, Rectangle location)
    {
        if (Path.HasExtension(file))
        {
            Extension = new[] { Path.GetExtension(file).Remove(0,1) };
            Filename = Path.GetFileNameWithoutExtension(file);
        }
        else
        {
            Filename = file;
            Extension = new[] { "gif", "bmp" };
        }

        Location = location;
        
    }

    public BitmapStorage(string file, int x, int y, int w, int h = -1) : this(file,
        new Rectangle(x, y, w, h == -1 ? w : h))
    {
    }
}