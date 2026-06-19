using Raylib_CSharp.Colors;
using Raylib_CSharp.Rendering;
using RaylibUI.BasicTypes.Controls;
using RaylibUI;

namespace RaylibUI.RunGame.GameControls.Civilopedia;

public class PediaLinkLabel : LabelControl
{
    private readonly bool _red;

    public PediaLinkLabel(IControlLayout layout, string text, int x, int y, bool red = false) :
        base(layout, text, false, fontSize: 23,
            colorFront: red ? new Color(210, 0, 0, 255) : new Color(32, 49, 142, 255),
            colorShadow: new Color(255, 255, 245, 170),
            shadowOffset: new System.Numerics.Vector2(-1, -1))
    {
        _red = red;
        Location = new(x, y);
    }

    public override void Draw(bool pulse)
    {
        var color = _red ? new Color(210, 0, 0, 255) : new Color(32, 49, 142, 255);
        Graphics.DrawLine((int)Bounds.X, (int)Bounds.Y + Height, (int)Bounds.X + Width, (int)Bounds.Y + Height, color);
        base.Draw(pulse);
    }
}
