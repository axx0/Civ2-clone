using Raylib_cs;
using System.Numerics;

namespace RaylibUI.Forms;

public class MenuBar
{
    public List<MenuBarItem> Items { get; set; }
    public int ActiveMenuId { get; set; } = 0;
    public bool IsMenuStripVisible { get; set; } = false;
    //public bool Enabled { get; set; } = true;

    public MenuBar()
    {
        Items = new List<MenuBarItem>()
        {
            new MenuBarItem
            {
                Text = "Game",
                Items = new MenuStripItem[]
                {
                    new MenuStripItem { Text = "Game Options", KeyShortcut = "Ctrl+O" },
                    new MenuStripItem { Text = "Graphic Options", KeyShortcut = "Ctrl+P" },
                    new MenuStripItem { Text = "City Report Options", KeyShortcut = "Ctrl+E" },
                    new MenuStripItem { Text = "Multiplayer Options", KeyShortcut = "Ctrl+Y", Enabled = false },
                    new MenuStripItem { Text = "Game Profile", Enabled = false },
                    new MenuStripItem { Text = "Quit", KeyShortcut = "Ctrl+Q"},
                },
            },
            new MenuBarItem()
            {
                Text = "Kingdom",
                Items = new MenuStripItem[]
                {
                    new MenuStripItem { Text = "Tax Rate", KeyShortcut = "Shift+T" },
                    new MenuStripItem { Text = "View Throne Room", KeyShortcut = "Shift+H" },
                },
            },
            new MenuBarItem()
            {
                Text = "View",
                Items = new MenuStripItem[]
                {
                    new MenuStripItem { Text = "Move Pieces", KeyShortcut = "v" },
                    new MenuStripItem { Text = "View Pieces", KeyShortcut = "v" },
                    new MenuStripItem { Text = "Zoom In", KeyShortcut = "z" },
                    new MenuStripItem { Text = "Zoom Out", KeyShortcut = "x" },
                },
            },
        };

        // Determine bounds of items
        int fontSize = 14;
        int offsetX = 0;
        for (int col = 0; col < Items.Count; col++)
        {
            // Bounds of bar items
            var textSize = Raylib.MeasureTextEx(Raylib.GetFontDefault(), Items[col].Text, fontSize, 1.0f);
            Items[col].Bounds = new Rectangle(offsetX, 0, textSize.X + 10, textSize.Y);
            offsetX += (int)textSize.X + 10;

            // Bounds of strip items
            // ! 230 is hardcoded width of stip !
            for (int row = 0; row < Items[col].Items.Length; row++)
            {
                Items[col].Items[row].Bounds = new Rectangle(Items[col].Bounds.x, Items[col].Bounds.height + 22 * row, 230, 22);
            }
        }
    }

    public void Disable()
    {
        foreach (var item in Items)
        {
            item.Enabled = false;
        }
    }

    public void Enable()
    {
        foreach (var item in Items)
        {
            item.Enabled = true;
        }
    }

    public void Draw()
    {
        Vector2 mousePos = Raylib.GetMousePosition();

        // Determine if mouse is over items
        bool[] mouseOverBarItems = new bool[Items.Count];
        bool[][] mouseOverStipItems = new bool[Items.Count][];
        for (int i = 0; i < Items.Count; i++)
        {
            mouseOverBarItems[i] = Raylib.CheckCollisionPointRec(mousePos, Items[i].Bounds);

            bool[] rowArray = new bool[Items[i].Items.Length];
            for (int j = 0; j < Items[i].Items.Length; j++)
            {
                rowArray[j] = Raylib.CheckCollisionPointRec(mousePos, Items[i].Items[j].Bounds);
            }
            mouseOverStipItems[i] = rowArray;
        }

        for (int i = 0; i < Items.Count; i++)
        {
            // First deactivate item if it is disabled
            if (!Items[i].Enabled)
                Items[i].Activated = false;

            if (mouseOverBarItems[i] && Items[i].Enabled)
            {
                if (Raylib.IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT))
                {
                    if (Items[i].Activated)
                    {
                        foreach (var item in Items)
                            item.Activated = false;
                    }
                    else
                    {
                        Items[i].Activated = true;

                        // Deactivate all other items
                        for (int j = 0; j < Items.Count; j++)
                        {
                            if (j != i)
                            {
                                Items[j].Activated = false;
                            }
                        }
                    }
                }
                else
                {
                    // Activate item of any other is already activated
                    for (int j = 0; j < Items.Count; j++)
                    {
                        if (Items[j].Activated && j != i)
                        {
                            Items[j].Activated = false;
                            Items[i].Activated = true;
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
                    for (int j = 0; j < Items.Count; j++)
                        Items[j].Activated = false;
                }
            }
        }

        // Draw
        Raylib.DrawRectangle(0, 0, Raylib.GetScreenWidth(), 15, Color.WHITE);
        for (int col = 0; col < Items.Count; col++)
        {
            if (Items[col].Activated)
            {
                Raylib.DrawRectangleRec(Items[col].Bounds, new Color(204, 232, 255, 255));
                Raylib.DrawRectangleLinesEx(Items[col].Bounds, 1.0f, new Color(153, 209, 255, 255));

                // Draw strip
                Raylib.DrawRectangleRec(new Rectangle(Items[col].Bounds.x, Items[col].Bounds.height, 230, Items[col].Items.Length * 22), new Color(242, 242, 242, 255));
                Raylib.DrawRectangleLinesEx(new Rectangle(Items[col].Bounds.x, Items[col].Bounds.height, 230, Items[col].Items.Length * 22), 1.0f, new Color(204, 204, 204, 255));
                for (int row = 0; row < Items[col].Items.Length; row++)
                {
                    if (mouseOverStipItems[col][row])
                    {
                        var rect = Items[col].Items[row].Bounds;
                        rect.x += 2;
                        rect.y += 2;
                        rect.width -= 4;
                        rect.height -= 4;

                        var color = Items[col].Items[row].Enabled ? new Color(145, 201, 247, 255) : new Color(230, 230, 230, 255);

                        Raylib.DrawRectangleRec(rect, color);
                    }

                    var textStripColor = Items[col].Items[row].Enabled ? Color.BLACK : Color.GRAY;

                    Raylib.DrawText(Items[col].Items[row].Text, (int)Items[col].Bounds.x + 5, (int)Items[col].Bounds.height + 22 * row + 5, 14, textStripColor);
                    Raylib.DrawText(Items[col].Items[row].KeyShortcut, (int)Items[col].Bounds.x + 160, (int)Items[col].Bounds.height + 22 * row + 5, 14, textStripColor);
                }
            }
            // Hover
            else if (mouseOverBarItems[col] && Items[col].Enabled)
            {
                Raylib.DrawRectangleRec(Items[col].Bounds, new Color(229, 243, 255, 255));
                Raylib.DrawRectangleLinesEx(Items[col].Bounds, 1.0f, new Color(204, 232, 255, 255));
            }

            var textColor = Items[col].Enabled ? Color.BLACK : Color.GRAY;
            Raylib.DrawText(Items[col].Text, (int)Items[col].Bounds.x + 5, 0, 14, textColor);
        }
    }
}
