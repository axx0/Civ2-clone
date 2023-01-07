using Raylib_cs;
using System.Numerics;

namespace RaylibUI
{
    public partial class Main
    {
        Vector2 mousePosition;
        Vector2 mouseMapViewIncrement;

        private void MousePressedAction()
        {
            mousePosition = Raylib.GetMousePosition();
            mouseMapViewIncrement = new Vector2(0, 0);
            
            if (Raylib.IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT))
            {
                mouseMapViewIncrement = new Vector2((int)((mousePosition.X - Raylib.GetScreenWidth() / 2) / MapTileTextureC2(0, 0).width),
                                                    (int)((mousePosition.Y - Raylib.GetScreenHeight() / 2) / MapTileTextureC2(0, 0).height));
            }
        }
    }
}
