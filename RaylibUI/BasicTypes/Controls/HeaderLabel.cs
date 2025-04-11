using System.Numerics;
using Model;
using Model.Interface;
using Raylib_CSharp.Colors;
using Raylib_CSharp.Interact;

namespace RaylibUI.BasicTypes.Controls;

public class HeaderLabel : LabelControl
{

    public HeaderLabel(IControlLayout controller, InterfaceStyle look, string title, int fontSize = 0) : 
        base(controller, title, eventTransparent: false, alignment: TextAlignment.Center, font: look.HeaderLabelFont, fontSize: fontSize, spacing: 0.0f, colorFront: look.HeaderLabelColour, colorShadow: Color.Black, shadowOffset: look.HeaderLabelShadow ? new Vector2(1, 1) : new Vector2(0, 0))
    {
    }

    public HeaderLabel(IControlLayout controller, string title, int fontSize = 0) :
        base(controller, title, eventTransparent: false, alignment: TextAlignment.Center, font: Fonts.TnRbold, fontSize: fontSize)
    {
    }

    public override void OnMouseMove(Vector2 moveAmount)
    {
        if (Input.IsMouseButtonDown(MouseButton.Left))
        {
            Controller.Move(moveAmount);
        }
    }

    public override int GetPreferredHeight()
    {
        return Controller.LayoutPadding.Top;
    }
}