using System.Numerics;
using Model.Images;
using Raylib_cs;

namespace RaylibUI;

public class IconContainer : BaseControl
{
    private readonly int _iconWidth;
    private readonly Texture2D? _texture;
    private readonly Vector2 _offset;

    public IconContainer(IControlLayout controller, IImageSource? imageSource, int index, int iconWidth) : base(controller, true)
    {
        _iconWidth = iconWidth + 2;
        if (imageSource != null)
        {
            _texture = TextureCache.GetImage(imageSource);
            _offset = new Vector2(index % 2 * _iconWidth + (_iconWidth - _texture.Value.Width) / 2f,
                (32 - _texture.Value.Height) / 2f);
        }
    }

    public override void Draw(bool pulse)
    {
        if (_texture != null)
        {
            Raylib.DrawTextureEx(_texture.Value, Location + _offset, 0f, 1f, Color.White);
        }

        base.Draw(pulse);
    }

    public override int GetPreferredWidth()
    {
        return _iconWidth * 2;
    }
}