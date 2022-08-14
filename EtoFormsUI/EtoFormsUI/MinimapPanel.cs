using Civ2engine;
using Civ2engine.Enums;
using Civ2engine.Events;
using Eto.Drawing;
using Eto.Forms;

namespace EtoFormsUI
{
    public class MinimapPanel : Civ2panel
    {
        private readonly Main _main;
        private readonly Game _game;
        private readonly Drawable _drawPanel;
        private readonly int[] offset;
        private int[] _mapStartXy = { 0, 0 };
        private int[] _mapDrawSq = { 0, 0 };

        private static readonly Color OceanColor = Color.FromArgb(0, 0, 95);
        private static readonly Color LandColor = Color.FromArgb(55, 123, 23);

        public MinimapPanel(Main parent, int width, int height, Game game) : base(parent.InterfaceStyle, width, height, 38, 10, "World")
        {
            _game = game;
            _main = parent;

            _drawPanel = new Drawable()
            {
                Size = new Size(MainPanel.Width - 2 * 11, MainPanel.Height - 38 - 10),
                BackgroundColor = Colors.Black,
                Cursor = new Cursor(MakeCursor(), new Point(7, 7))
            };
            _drawPanel.Paint += DrawPanel_Paint;
            _drawPanel.MouseDown += DrawPanel_MouseClick;
            MainPanelLayout.Add(_drawPanel, 11, 38);
            MainPanel.Content = MainPanelLayout;

            // Determine the offset of minimap from panel edges
            var currentMap = game.CurrentMap;
            offset = new[] { (_drawPanel.Width - 2 * currentMap.XDim) / 2, (_drawPanel.Height - currentMap.YDim) / 2 };
        }


        private void DrawPanel_Paint(object sender, PaintEventArgs e)
        {
            var map = _game.CurrentMap;
            // Draw map
            for (var row = 0; row < map.YDim; row++)
            {
                for (var col = 0; col < map.XDim; col++)
                {
                    var tile = map.Tile[col, row];
                    if (!map.MapRevealed && !tile.IsVisible(map.WhichCivsMapShown)) continue;

                    var drawColor = tile.CityHere is not null
                        ? MapImages.PlayerColours[tile.CityHere.Owner.Id].TextColour
                        : tile.Type == TerrainType.Ocean
                            ? OceanColor
                            : LandColor;

                    e.Graphics.FillRectangle(new SolidBrush(drawColor), offset[0] + 2 * col + (row % 2),
                        offset[1] + row, 2, 1);
                }
            }

            // Draw current view rectangle
            e.Graphics.DrawRectangle(new Pen(Colors.White), offset[0] + _mapStartXy[0], offset[1] + _mapStartXy[1],
                _mapDrawSq[0], _mapDrawSq[1]);
        }

        private void DrawPanel_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Buttons == MouseButtons.Primary)
            {
                int clickedX = (int)e.Location.X;
                int clickedY = (int)e.Location.Y;
                var map = _game.CurrentMap;
                // Determine if you clicked within the drawn minimap
                if (clickedX >= offset[0] && clickedX < offset[0] + map.XDimMax && clickedY >= offset[1] &&
                    clickedY < offset[1] + map.YDim)
                {
                    _main.TriggerMapEvent(new MapEventArgs(MapEventType.MapViewChanged,
                        new[] { clickedX - offset[0], clickedY - offset[1] }));
                }
            }
        }

        public void Update(int[] startXy, int[] visibleTiles)
        {
            _mapStartXy = startXy;
            _mapDrawSq = visibleTiles;
            _drawPanel.Invalidate();
        }

        private static Bitmap MakeCursor()
        {
            var cursorBmp = new Bitmap(15, 15, PixelFormat.Format32bppRgba);
            using var g = new Graphics(cursorBmp);

            g.AntiAlias = false;
            g.DrawLine(Colors.White, 0, 7, 5, 7);
            g.DrawLine(Colors.White, 9, 7, 14, 7);
            g.DrawLine(Colors.White, 7, 0, 7, 5);
            g.DrawLine(Colors.White, 7, 9, 7, 14);
            g.DrawLine(Colors.White, 5, 1, 9, 1);
            g.DrawLine(Colors.White, 10, 2, 11, 2);
            g.DrawLine(Colors.White, 12, 3, 12, 4);
            g.DrawLine(Colors.White, 13, 5, 13, 9);
            g.DrawLine(Colors.White, 12, 10, 12, 11);
            g.DrawLine(Colors.White, 10, 12, 11, 12);
            g.DrawLine(Colors.White, 5, 13, 9, 13);
            g.DrawLine(Colors.White, 3, 12, 4, 12);
            g.DrawLine(Colors.White, 2, 10, 2, 11);
            g.DrawLine(Colors.White, 1, 5, 1, 9);
            g.DrawLine(Colors.White, 2, 3, 2, 4);
            g.DrawLine(Colors.White, 3, 2, 4, 2);
            g.DrawLine(Colors.White, 6, 4, 8, 4);
            g.FillRectangle(Colors.White, 9, 5, 1, 1);
            g.DrawLine(Colors.White, 10, 6, 10, 8);
            g.FillRectangle(Colors.White, 9, 9, 1, 1);
            g.DrawLine(Colors.White, 6, 10, 8, 10);
            g.FillRectangle(Colors.White, 5, 9, 1, 1);
            g.DrawLine(Colors.White, 4, 6, 4, 8);
            g.FillRectangle(Colors.White, 5, 5, 1, 1);
            return cursorBmp;
        }
    }
}