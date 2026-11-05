using System.Numerics;
using Model;
using Model.Controls;
using Model.Interface;
using Raylib_CSharp.Colors;
using Raylib_CSharp.Interact;

namespace RaylibUI.BasicTypes.Controls;

public class HeaderLabel : LabelControl
{

    public HeaderLabel(IControlLayout controller, InterfaceStyle look, string title, int fontSize = 0) : 
        base(controller, title, eventTransparent: false, horizontalAlignment: HorizontalAlignment.Center,
        verticalAlignment: VerticalAlignment.Center, font: look.HeaderLabelFont, fontSize: fontSize, 
        colorFront: look.HeaderLabelColour, colorShadow: Color.Black, 
        shadowOffset: look.HeaderLabelShadow ? new Vector2(1, 1) : new Vector2(0, 0))
    {
    }

    public HeaderLabel(IControlLayout controller, string title, int fontSize = 0) :
        base(controller, title, eventTransparent: false, horizontalAlignment: HorizontalAlignment.Center, 
             font: Fonts.TnRbold, fontSize: fontSize)
    {
    }

    // Drag the dialog by its title bar (the mouse is captured while the button is held)
    public override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);
        if (e.Button == MouseButton.Left)
        {
            Controller.Move(e.Movement);
        }
    }

    public override int GetPreferredHeight()
    {
        return Controller.LayoutPadding.Top;
    }
}