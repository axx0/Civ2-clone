using RaylibUI.BasicTypes.Controls;
using RaylibUI;

namespace RaylibUI.RunGame.GameControls.Civilopedia;

public class PediaLabel : LabelControl
{
    public PediaLabel(IControlLayout layout, string text, int x, int y) : base(layout, text, true,
        fontSize: 23,
        colorFront: TextRendering.StrongBlack,
        colorShadow: new Raylib_CSharp.Colors.Color(255, 255, 245, 170),
        shadowOffset: new System.Numerics.Vector2(-1, -1))
    {
        Location = new(x, y);
    }
}
