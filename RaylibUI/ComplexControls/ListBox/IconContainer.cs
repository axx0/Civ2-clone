using Model.Images;
using Raylib_cs;

namespace RaylibUI;

public class IconContainer : BaseControl
{
    // TODO: read this from Active interface somehow it's tied to tileset
    private const int IconWidth = 36;
    private readonly int _isOdd;
    private readonly Texture2D? _texture;

    public IconContainer(IControlLayout controller, IImageSource? imageSource, int index) : base(controller, true)
    {
        if (imageSource != null)
        {
            _texture = TextureCache.GetImage(imageSource);
        }
        _isOdd = index % 2;
    }

    public override void Draw(bool pulse)
    {
        if (_texture != null)
        {
            Raylib.DrawTexture(_texture.Value, (int)Location.X + IconWidth * _isOdd, (int)Location.Y, Color.White);
        }
        base.Draw(pulse);
    }

    public override int GetPreferredWidth()
    {
        return IconWidth * 2;
    }
}