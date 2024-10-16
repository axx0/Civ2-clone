using Raylib_CSharp.Images;
using Raylib_CSharp.Colors;

namespace Model.Images;

public class MemoryStorage : IImageSource
{
    private readonly string _name;

    public MemoryStorage(Image baseImage, string name, Color? replacementColour = null, bool dark = false)
    {
        Image = baseImage;
        _name = name;
        ReplacementColour = replacementColour;
        Dark = dark;
    }

    public ImageStorage Type => ImageStorage.Memory;
    public Image Image { get; }
    public Color? ReplacementColour { get; }
    public bool Dark { get; }

    public string GetKey(int ownerId = -1)
    {
        if (ReplacementColour == null || ownerId == -1)
        {
            return _name;
        }

        return _name + "-" + ownerId;
    }
}