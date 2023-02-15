using Raylib_cs;
using RaylibControls;
using System.Runtime.CompilerServices;

namespace RaylibUI.Controls
{
    public class MenuBar
    {
        public List<MenuBarItem> Items;
        public int ActiveMenuId = 0;
        public bool IsMenuStripVisible = false;

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
                        new MenuStripItem { Text = "Multiplayer Options", KeyShortcut = "Ctrl+Y", IsEnabled = false },
                        new MenuStripItem { Text = "Game Profile", IsEnabled = false },
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
                item.IsEnabled = false;
            }
        }

        public void Enable()
        {
            foreach (var item in Items)
            {
                item.IsEnabled = true;
            }
        }
    }
}
