using Raylib_CSharp;
using Raylib_CSharp.Collision;
using Raylib_CSharp.Colors;
using Raylib_CSharp.Interact;
using Raylib_CSharp.Rendering;
using Raylib_CSharp.Transformations;
using Raylib_CSharp.Windowing;
using System.Numerics;

namespace RaylibUI.Forms;

public class MenuBar
{
    public List<MenuBarItem> Items { get; set; }
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
                    new MenuStripItem { Text = "Zoom Out", KeyShortcut = "X" },
                },
            },
        };

        // Determine bounds of items
        int fontSize = 14;
        int offsetX = 0;
        for (int col = 0; col < Items.Count; col++)
        {
            // Bounds of bar items
            var textSize = new Vector2(0, 0);// Raylib.MeasureTextEx(Fonts.DefaultFont, Items[col].Text, fontSize, 1.0f);
            Items[col].Bounds = new Rectangle(offsetX, 0, textSize.X + 10, textSize.Y);
            offsetX += (int)textSize.X + 10;

            // Bounds of strip items
            // ! 230 is hardcoded Width of stip !
            for (int row = 0; row < Items[col].Items.Length; row++)
            {
                Items[col].Items[row].Bounds = new Rectangle(Items[col].Bounds.X, Items[col].Bounds.Height + 22 * row, 230, 22);
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
        Vector2 mousePos = Input.GetMousePosition();

        // Determine if mouse is over items
        bool[] mouseOverBarItems = new bool[Items.Count];
        bool[][] mouseOverStipItems = new bool[Items.Count][];
        for (int i = 0; i < Items.Count; i++)
        {
            mouseOverBarItems[i] = ShapeHelper.CheckCollisionPointRec(mousePos, Items[i].Bounds);

            bool[] rowArray = new bool[Items[i].Items.Length];
            for (int j = 0; j < Items[i].Items.Length; j++)
            {
                rowArray[j] = ShapeHelper.CheckCollisionPointRec(mousePos, Items[i].Items[j].Bounds);
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
                if (Input.IsMouseButtonPressed(MouseButton.Left))
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
            else if (Input.IsMouseButtonPressed(MouseButton.Left))
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
        Graphics.DrawRectangle(0, 0, Window.GetScreenWidth(), 15, Color.White);
        for (int col = 0; col < Items.Count; col++)
        {
            if (Items[col].Activated)
            {
                Graphics.DrawRectangleRec(Items[col].Bounds, new Color(204, 232, 255, 255));
                Graphics.DrawRectangleLinesEx(Items[col].Bounds, 1.0f, new Color(153, 209, 255, 255));

                // Draw strip
                Graphics.DrawRectangleRec(new Rectangle(Items[col].Bounds.X, Items[col].Bounds.Height, 230, Items[col].Items.Length * 22), new Color(242, 242, 242, 255));
                Graphics.DrawRectangleLinesEx(new Rectangle(Items[col].Bounds.X, Items[col].Bounds.Height, 230, Items[col].Items.Length * 22), 1.0f, new Color(204, 204, 204, 255));
                for (int row = 0; row < Items[col].Items.Length; row++)
                {
                    if (mouseOverStipItems[col][row])
                    {
                        var rect = Items[col].Items[row].Bounds;
                        rect.X += 2;
                        rect.Y += 2;
                        rect.Width -= 4;
                        rect.Height -= 4;

                        var color = Items[col].Items[row].Enabled ? new Color(145, 201, 247, 255) : new Color(230, 230, 230, 255);

                        Graphics.DrawRectangleRec(rect, color);
                    }

                    var textStripColor = Items[col].Items[row].Enabled ? Color.Black : Color.Gray;

                    Graphics.DrawText(Items[col].Items[row].Text, (int)Items[col].Bounds.X + 5, (int)Items[col].Bounds.Height + 22 * row + 5, 14, textStripColor);
                    Graphics.DrawText(Items[col].Items[row].KeyShortcut, (int)Items[col].Bounds.X + 160, (int)Items[col].Bounds.Height + 22 * row + 5, 14, textStripColor);
                }
            }
            // Hover
            else if (mouseOverBarItems[col] && Items[col].Enabled)
            {
                Graphics.DrawRectangleRec(Items[col].Bounds, new Color(229, 243, 255, 255));
                Graphics.DrawRectangleLinesEx(Items[col].Bounds, 1.0f, new Color(204, 232, 255, 255));
            }

            var textColor = Items[col].Enabled ? Color.Black : Color.Gray;
            Graphics.DrawText(Items[col].Text, (int)Items[col].Bounds.X + 5, 0, 14, textColor);
        }
    }
}
