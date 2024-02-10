using System.Numerics;
using Raylib_cs;

namespace RaylibUI.RunGame.GameControls;

public class TextureDisplay : BaseControl
{
    private readonly Vector2 _location;
    private readonly Texture2D _texture;
    private readonly float _scale;

    public TextureDisplay(IControlLayout controller, Texture2D texture, Vector2 location, float scale = 1f) : base(controller)
    {
        _texture = texture;
        _location = location;
        _scale = scale;
    }

    public override void Draw(bool pulse)
    {
        Raylib.DrawTextureEx(_texture, _location, 0f, _scale, Color.White);
        base.Draw(pulse);
    }
}