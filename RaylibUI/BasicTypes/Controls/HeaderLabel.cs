using System.Numerics;
using Model;
using Model.Interface;
using Raylib_cs;

namespace RaylibUI.BasicTypes.Controls;

public class HeaderLabel : LabelControl
{

    public HeaderLabel(IControlLayout controller, InterfaceStyle look, string title, int fontSize = 0) : 
        base(controller, title, eventTransparent: false, offset: 0, alignment: TextAlignment.Center, font: look.HeaderLabelFont, fontSize: fontSize, spacing: 0.0f, colorFront: look.HeaderLabelColour, colorShadow: Color.Black, shadowOffset: look.HeaderLabelShadow ? new Vector2(1, 1) : new Vector2(0, 0))
    {
    }

    public override void OnMouseMove(Vector2 moveAmount)
    {
        if (Raylib.IsMouseButtonDown(MouseButton.Left))
        {
            Controller.Move(moveAmount);
        }
    }

    public override int GetPreferredHeight()
    {
        return Controller.LayoutPadding.Top;
    }
}