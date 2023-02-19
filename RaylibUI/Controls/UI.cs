using Raylib_cs;
using RaylibUI;
using RaylibUI.Controls;
using System.Numerics;

namespace RaylibControls
{
    public static class UI
    {
        public static MenuBar menuBar;

        public static void MenuBar()
        {
            Vector2 mousePos = Raylib.GetMousePosition();

            // Determine if mouse is over items
            bool[] mouseOverBarItems = new bool[menuBar.Items.Count];
            bool[][] mouseOverStipItems = new bool[menuBar.Items.Count][];
            for (int i = 0; i < menuBar.Items.Count; i++)
            {
                mouseOverBarItems[i] = Raylib.CheckCollisionPointRec(mousePos, menuBar.Items[i].Bounds);

                bool[] rowArray = new bool[menuBar.Items[i].Items.Length];
                for (int j = 0; j < menuBar.Items[i].Items.Length; j++)
                {
                    rowArray[j] = Raylib.CheckCollisionPointRec(mousePos, menuBar.Items[i].Items[j].Bounds);
                }
                mouseOverStipItems[i] = rowArray;
            }

            for (int i = 0; i < menuBar.Items.Count; i++)
            {
                if (mouseOverBarItems[i])
                {
                    if (Raylib.IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT))
                    {
                        if (menuBar.Items[i].IsActivated)
                        {
                            foreach (var item in menuBar.Items)
                                item.IsActivated = false;
                        }
                        else
                        {
                            menuBar.Items[i].IsActivated = true;

                            // Deactivate all other items
                            for (int j = 0; j < menuBar.Items.Count; j++)
                            {
                                if (j != i)
                                {
                                    menuBar.Items[j].IsActivated = false;
                                }
                            }
                        }
                    }
                    else
                    {
                        // Activate item of any other is already activated
                        for (int j = 0; j < menuBar.Items.Count; j++)
                        {
                            if (menuBar.Items[j].IsActivated && j != i)
                            {
                                menuBar.Items[j].IsActivated = false;
                                menuBar.Items[i].IsActivated = true;
                            }
                        }
                    }
                }
                // TODO: item in strip menu is clicked
                //...
                // outside clicked
                else if (Raylib.IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT))
                {
                    // Make sure no item is clicked
                    bool clickedOutside = true;
                    for (int j = 0; j < mouseOverBarItems.Length; j++)
                    {
                        if (mouseOverBarItems[j])
                        {
                            clickedOutside = false;
                        }
                    }

                    if (clickedOutside)
                    {
                        for (int j = 0; j < menuBar.Items.Count; j++)
                            menuBar.Items[j].IsActivated = false;
                    }
                }
            }

            // Draw
            Raylib.DrawRectangle(0, 0, Raylib.GetScreenWidth(), 15, Color.WHITE);
            for (int col = 0; col < menuBar.Items.Count; col++)
            {
                if (menuBar.Items[col].IsActivated)
                {
                    Raylib.DrawRectangleRec(menuBar.Items[col].Bounds, new Color(204, 232, 255, 255));
                    Raylib.DrawRectangleLinesEx(menuBar.Items[col].Bounds, 1.0f, new Color(153, 209, 255, 255));

                    // Draw strip
                    Raylib.DrawRectangleRec(new Rectangle(menuBar.Items[col].Bounds.x, menuBar.Items[col].Bounds.height, 230, menuBar.Items[col].Items.Length * 22), new Color(242, 242, 242, 255));
                    Raylib.DrawRectangleLinesEx(new Rectangle(menuBar.Items[col].Bounds.x, menuBar.Items[col].Bounds.height, 230, menuBar.Items[col].Items.Length * 22), 1.0f, new Color(204, 204, 204, 255));
                    for (int row = 0; row < menuBar.Items[col].Items.Length; row++)
                    {
                        if (mouseOverStipItems[col][row])
                        {
                            var rect = menuBar.Items[col].Items[row].Bounds;
                            rect.x += 2;
                            rect.y += 2;
                            rect.width -= 4;
                            rect.height -= 4;

                            var color = menuBar.Items[col].Items[row].IsEnabled ? new Color(145, 201, 247, 255) : new Color(230, 230, 230, 255);

                            Raylib.DrawRectangleRec(rect, color);
                        }

                        var textColor = menuBar.Items[col].Items[row].IsEnabled ? Color.BLACK : Color.GRAY;

                        Raylib.DrawText(menuBar.Items[col].Items[row].Text, (int)menuBar.Items[col].Bounds.x + 5, (int)menuBar.Items[col].Bounds.height + 22 * row + 5, 14, textColor);
                        Raylib.DrawText(menuBar.Items[col].Items[row].KeyShortcut, (int)menuBar.Items[col].Bounds.x + 160, (int)menuBar.Items[col].Bounds.height + 22 * row + 5, 14, textColor);
                    }
                }
                // Hover
                else if (mouseOverBarItems[col])
                {
                    Raylib.DrawRectangleRec(menuBar.Items[col].Bounds, new Color(229, 243, 255, 255));
                    Raylib.DrawRectangleLinesEx(menuBar.Items[col].Bounds, 1.0f, new Color(204, 232, 255, 255));
                }

                Raylib.DrawText(menuBar.Items[col].Text, (int)menuBar.Items[col].Bounds.x + 5, 0, 14, Color.BLACK);
            }
        }
    }
}
