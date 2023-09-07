using System.Numerics;
using Raylib_cs;
using RaylibUI.BasicTypes.Controls;

namespace RaylibUI.Controls;

public class HeaderLabel : LabelControl
{

    public HeaderLabel(IControlLayout controller, string title) : base(controller, title,alignment: TextAlignment.Center)
    {
    }

    public override void OnMouseMove(Vector2 moveAmount)
    {
        if (Raylib.IsMouseButtonDown(MouseButton.MOUSE_BUTTON_LEFT))
        {
            GameScreen.Move(moveAmount);
        }
    }
}