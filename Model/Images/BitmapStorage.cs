using Raylib_CSharp.Transformations;

namespace Model.Images;

public class BitmapStorage : IImageSource
{
    public ImageStorage Type => ImageStorage.Bitmap;
    public string Key => $"{Filename}-{Location}";

    public string GetKey(int ownerId = -1)
    {
        return $"{Filename}-{Location}";
    }

    public string Filename { get; }
    
    public Rectangle Location { get; }
    public string[] Extension { get; }

    /// <summary>
    /// Does this image have an extra transparency colour
    /// defined in upper-left corner pixel?
    /// </summary>
    public bool TransparencyPixel { get; }

    /// <summary>
    /// Search for position of shield (units)/city size flag&box?
    /// </summary>
    public bool SearchFlagLoc { get; }

    public BitmapStorage(string file, Rectangle location, bool transparencyPixel = false, bool searchFlagLoc = false)
    {
        if (Path.HasExtension(file))
        {
            Extension = new[] { Path.GetExtension(file).Remove(0,1) };
            Filename = Path.GetFileNameWithoutExtension(file);
        }
        else
        {
            Filename = file;
            Extension = new[] { "gif", "bmp", "png" };
        }

        Location = location;
        TransparencyPixel = transparencyPixel;
        SearchFlagLoc = searchFlagLoc;
    }

    public BitmapStorage(string file, int x, int y, int w, int h = -1, bool transparencyPixel = false, bool searchFlagLoc = false) : this(file, new Rectangle(x, y, w, h == -1 ? w : h), transparencyPixel, searchFlagLoc)
    {
    }

    public BitmapStorage(string file) : this(file, new Rectangle(0,0,0,0))
    {
        
    }
}