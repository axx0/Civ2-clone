using Raylib_cs;
using System.Numerics;

namespace RaylibControls
{
    public static class UI
    {
        public static Texture2D tileTextureOuter, tileTextureInner;
        public static Vector2 dialogPos;
        public static int selectedRadioOption;

        // Button returns true when clicked
        public static bool Button(Rectangle bounds, string text)
        {
            bool pressed = false;
            Vector2 mousePos = Raylib.GetMousePosition();

            int x = (int)bounds.x;
            int y = (int)bounds.y;
            int w = (int)bounds.width;
            int h = (int)bounds.height;

            Raylib.DrawRectangleLinesEx(bounds, 1.0f, new Color(100, 100, 100, 255));
            Raylib.DrawRectangleRec(new Rectangle(x + 1, y + 1, w - 2, h - 2), Color.WHITE);
            Raylib.DrawRectangleRec(new Rectangle(x + 3, y + 3, w - 6, h - 6), new Color(192, 192, 192, 255));
            Raylib.DrawLine(x + 2, y + h - 2, x + w - 2, y + h - 2, new Color(128, 128, 128, 255));
            Raylib.DrawLine(x + 3, y + h - 3, x + w - 2, y + h - 3, new Color(128, 128, 128, 255));
            Raylib.DrawLine(x + w - 1, y + 2, x + w - 1, y + h - 1, new Color(128, 128, 128, 255));
            Raylib.DrawLine(x + w - 2, y + 3, x + w - 2, y + h - 1, new Color(128, 128, 128, 255));

            var textSize = Raylib.MeasureTextEx(Raylib.GetFontDefault(), text, 18, 1.0f);
            Raylib.DrawText(text, x + w / 2 - (int)textSize.X / 2, y + h / 2 - (int)textSize.Y / 2, 18, Color.BLACK);

            if (Raylib.IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT) && Raylib.CheckCollisionPointRec(mousePos, bounds))
            {
                pressed = true;
            }

            return pressed;
        }

        public static void Dialog(Vector2 size, string text, int paddingTop = 38, int paddingBtm = 46)
        {
            int paddingL = 11, paddingR = 11;
            string[] options = new[] { "Start a New Game", "Start on Premade World", "Customize World", "Begin Scenario", "Load a Game", "Multiplayer Game", "View Hall of Fame", "View Credits" };

            int x = (int)dialogPos.X;
            int y = (int)dialogPos.Y;
            int w = (int)size.X;
            int h = (int)size.Y;

            Vector2 mousePos = Raylib.GetMousePosition();
            Vector2 delta;

            // Detect mouse click on option
            if (Raylib.IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT) && Raylib.CheckCollisionPointRec(mousePos, new Rectangle(x + paddingL + 8, y + paddingTop + 5, w - paddingL - paddingR - 8, 32 * options.Length)))
            {
                selectedRadioOption = ((int)mousePos.Y - y - paddingTop - 5) / 32;
            }

            // Move dialog
            if (Raylib.IsMouseButtonDown(MouseButton.MOUSE_BUTTON_LEFT) && Raylib.CheckCollisionPointRec(mousePos, new Rectangle(x, y, w, paddingTop)))
            {
                delta = Raylib.GetMouseDelta();
                dialogPos.X += (int)delta.X;
                dialogPos.Y += (int)delta.Y;
            }

            // Outer wallpaper
            Raylib.DrawTextureTiled(tileTextureOuter, new Rectangle(0, 0, tileTextureOuter.width, tileTextureOuter.height), new Rectangle(x, y, w, h), new Vector2(0, 0), 0.0f, 1.0f, Color.WHITE);

            // Outer border
            var color1 = new Color(227, 227, 227, 255);
            var color2 = new Color(105, 105, 105, 255);
            var color3 = new Color(255, 255, 255, 255);
            var color4 = new Color(160, 160, 160, 255);
            var color5 = new Color(240, 240, 240, 255);
            var color6 = new Color(223, 223, 223, 255);
            var color7 = new Color(63, 63, 63, 255);
            Raylib.DrawLine(x, y, x + w - 1, y, color1);    // 1st layer of border
            Raylib.DrawLine(x + 1, y + 1, x + 1, y + h - 1, color1);
            Raylib.DrawLine(x + w, y, x + w, y + h - 1, color2);
            Raylib.DrawLine(x, y + h - 1, x + w, y + h - 1, color2);
            Raylib.DrawLine(x + 1, y + 1, x + w - 2, y + 1, color3);    // 2nd layer of border
            Raylib.DrawLine(x + 2, y + 1, x + 2, y + h - 2, color3);
            Raylib.DrawLine(x + w - 1, y + 1, x + w - 1, y + h - 1, color4);
            Raylib.DrawLine(x + 1, y + h - 2, x + w - 1, y + h - 2, color4);
            Raylib.DrawLine(x + 2, y + 2, x + w - 3, y + 2, color5);    // 3rd layer of border
            Raylib.DrawLine(x + 3, y + 2, x + 3, y + h - 3, color5);
            Raylib.DrawLine(x + w - 2, y + 2, x + w - 2, y + h - 2, color5);
            Raylib.DrawLine(x + 2, y + h - 3, x + w - 2, y + h - 3, color5);
            Raylib.DrawLine(x + 3, y + 3, x + w - 4, y + 3, color6);    // 4th layer of border
            Raylib.DrawLine(x + 4, y + 3, x + 4, y + h - 3, color6);
            Raylib.DrawLine(x + w - 3, y + 3, x + w - 3, y + h - 3, color7);
            Raylib.DrawLine(x + 4, y + h - 4, x + w - 4, y + h - 4, color7);
            Raylib.DrawLine(x + 4, y + 4, x + w - 6, y + 4, color6);    // 5th layer of border
            Raylib.DrawLine(x + 5, y + 4, x + 5, y + h - 4, color6);
            Raylib.DrawLine(x + w - 4, y + 4, x + w - 4, y + h - 4, color7);

            // Inner border
            Raylib.DrawLine(x + paddingL - 2, y + paddingTop - 2, x + w - paddingR + 1, y + paddingTop - 2, color7);    // 1st layer of border
            Raylib.DrawLine(x + paddingL - 1, y + paddingTop - 1, x + paddingL - 1, y + h - paddingBtm + 1, color7);
            Raylib.DrawLine(x + w - paddingR + 2, y + paddingTop - 2, x + w - paddingR + 2, y + h - paddingBtm + 1, color6);
            Raylib.DrawLine(x + paddingL - 2, y + h - paddingBtm + 1, x + w - paddingR + 2, y + h - paddingBtm + 1, color6);
            Raylib.DrawLine(x + paddingL - 2, y + paddingTop - 1, x + w - paddingR, y + paddingTop - 1, color7);    // 2nd layer of border
            Raylib.DrawLine(x + paddingL, y + paddingTop - 1, x + paddingL, y + h - paddingBtm, color7);
            Raylib.DrawLine(x + w - paddingR + 1, y + paddingTop - 1, x + w - paddingR + 1, y + h - paddingBtm + 1, color6);
            Raylib.DrawLine(x + paddingL - 1, y + h - paddingBtm, x + w - paddingR + 2, y + h - paddingBtm, color6);

            // Iner wallpaper
            Raylib.DrawTextureTiled(tileTextureInner, new Rectangle(0, 0, tileTextureInner.width, tileTextureInner.height), new Rectangle(x + paddingR, y + paddingTop, w - paddingR - paddingL, h - paddingTop - paddingBtm), new Vector2(0, 0), 0.0f, 1.0f, Color.WHITE);

            // Dialog text
            var textSize = Raylib.MeasureTextEx(Raylib.GetFontDefault(), text, 20, 1.0f);
            Raylib.DrawText(text, x + w / 2 - (int)textSize.X / 2, y + paddingTop / 2 - (int)textSize.Y / 2, 20, Color.BLACK);

            // Options
            for (int i = 0; i < options.Length; i++)
            {
                DrawRadioButton(x + paddingL + 10, y + paddingTop + 9 + 32 * i, selectedRadioOption == i);
                Raylib.DrawText(options[i], x + paddingL + 40, y + paddingTop + 10 + 32 * i, 20, Color.BLACK);

                if (selectedRadioOption == i)
                {
                    Raylib.DrawRectangleLines(x + paddingL + 34, y + paddingTop + 5 + 32 * i, w - paddingL - paddingR - 34 - 2, 26, new Color(64, 64, 64, 255));
                }
            }

            // Buttons
            if (Button(new Rectangle(x + paddingL - 2, y + h - paddingBtm + 4, 156, 36), "OK"))
            {
                Raylib.DrawText("OK PRESSED", x, y - 20, 18, Color.BLACK);
            }
            if (Button(new Rectangle(x + w - paddingR + 2 - 156, y + h - paddingBtm + 4, 156, 36), "Cancel"))
            {
                Raylib.DrawText("Cancel PRESSED", x, y - 20, 18, Color.RED);
            }
        }

        private static void DrawRadioButton(int x, int y, bool isSelected)
        {
            Raylib.DrawCircle(x + 8, y + 8, 8.0f, new Color(128, 128, 128, 255));
            Raylib.DrawCircleLines(x + 8 + 1, y + 8 + 1, 8.0f, Color.BLACK);
            Raylib.DrawRectangle(x + 1, y + 4, 2, 3, Color.BLACK);
            Raylib.DrawRectangle(x + 3, y + 2, 2, 2, Color.BLACK);
            Raylib.DrawRectangle(x + 6, y + 1, 1, 1, Color.BLACK);
            Raylib.DrawRectangle(x + 11, y + 15, 3, 2, Color.BLACK);
            Raylib.DrawRectangle(x + 14, y + 13, 2, 2, Color.BLACK);
            Raylib.DrawRectangle(x + 16, y + 11, 1, 1, Color.BLACK);
            Raylib.DrawCircleLines(x + 8, y + 8, 8.0f, Color.WHITE);

            if (!isSelected)
            {
                Raylib.DrawRectangle(x + 6, y + 4, 5, 9, new Color(192, 192, 192, 255));
                Raylib.DrawRectangle(x + 4, y + 6, 9, 5, new Color(192, 192, 192, 255));
                Raylib.DrawRectangle(x + 5, y + 11, 1, 1, Color.WHITE);
                Raylib.DrawRectangle(x + 4, y + 6, 1, 5, Color.WHITE);
                Raylib.DrawRectangle(x + 5, y + 5, 1, 2, Color.WHITE);
                Raylib.DrawRectangle(x + 6, y + 4, 1, 2, Color.WHITE);
                Raylib.DrawRectangle(x + 7, y + 4, 4, 1, Color.WHITE);
                Raylib.DrawRectangle(x + 11, y + 5, 1, 1, Color.WHITE);
                Raylib.DrawRectangle(x + 11, y + 11, 1, 1, new Color(192, 192, 192, 255));
                Raylib.DrawRectangle(x + 7, y + 13, 4, 1, Color.WHITE);
                Raylib.DrawRectangle(x + 11, y + 12, 1, 1, Color.WHITE);
                Raylib.DrawRectangle(x + 12, y + 11, 1, 1, Color.WHITE);
                Raylib.DrawRectangle(x + 13, y + 7, 1, 4, Color.WHITE);
            }
            else
            {
                Raylib.DrawRectangle(x + 7, y + 4, 4, 10, new Color(192, 192, 192, 255));
                Raylib.DrawRectangle(x + 4, y + 7, 10, 4, new Color(192, 192, 192, 255));
                Raylib.DrawRectangle(x + 6, y + 5, 6, 8, new Color(192, 192, 192, 255));
                Raylib.DrawRectangle(x + 5, y + 6, 8, 6, new Color(192, 192, 192, 255));
                Raylib.DrawRectangle(x + 7, y + 6, 4, 6, Color.BLACK);
                Raylib.DrawRectangle(x + 6, y + 7, 6, 4, Color.BLACK);
            }
        }
    }
}
