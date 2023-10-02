using System.Numerics;
using Raylib_cs;
using RaylibUI.BasicTypes.Controls;

namespace RaylibUI.Controls;

public class HeaderLabel : LabelControl
{

    public HeaderLabel(IControlLayout controller, string title) : base(controller, title, eventTransparent: false,alignment: TextAlignment.Center)
    {
    }

    public override void OnMouseMove(Vector2 moveAmount)
    {
        if (Raylib.IsMouseButtonDown(MouseButton.MOUSE_BUTTON_LEFT))
        {
            Controller.Move(moveAmount);
        }
    }
}