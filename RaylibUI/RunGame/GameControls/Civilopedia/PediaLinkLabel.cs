using Raylib_CSharp.Colors;
using Raylib_CSharp.Rendering;
using RaylibUI.BasicTypes.Controls;

namespace RaylibUI.RunGame.GameControls;

public class PediaLinkLabel : LabelControl
{
    private readonly bool _red;

    public PediaLinkLabel(IControlLayout layout, string text, int x, int y, bool red = false) : 
        base(layout, text, false, fontSize: 22, colorFront: red ? new Color(243, 0, 0, 255) : new Color(55, 71, 159, 255),
        colorShadow: Color.Black, shadowOffset: new System.Numerics.Vector2(1, 0))
    {
        _red = red;
        Location = new(x, y);
    }

    public override void Draw(bool pulse)
    {
        Graphics.DrawLine((int)Bounds.X, (int)Bounds.Y + Height, (int)Bounds.X + Width, (int)Bounds.Y + Height, 
            _red ? new Color(243, 0, 0, 255) : new Color(55, 71, 159, 255));

        base.Draw(pulse);
    }
}