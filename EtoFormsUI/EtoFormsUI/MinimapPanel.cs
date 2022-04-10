using System;
using Eto.Forms;
using Eto.Drawing;
using Civ2engine;
using Civ2engine.Enums;
using Civ2engine.Events;
using Civ2engine.MapObjects;

namespace EtoFormsUI
{
    public class MinimapPanel : Civ2panel
    {
        Game Game => Game.Instance;
        Map Map => Game.CurrentMap;

        private Main Main;
        private int[] offset, mapStartXY, mapDrawSq;
        private readonly Drawable drawPanel;

        public static event EventHandler<MapEventArgs> OnMapEvent;

        public MinimapPanel(Main parent, int width, int height) : base(width, height, 38, 10, "World")
        {
            this.Main = parent;

            MapPanel.OnMapEvent += MapEventHappened;
            //Main.OnMapEvent += MapEventHappened;

            drawPanel = new Drawable()
            {
                Size = new Size(MainPanel.Width - 2 * 11, MainPanel.Height - 38 - 10),
                BackgroundColor = Colors.Black,
                Cursor = new Cursor(MakeCursor(), new Point(7, 7))
            };
            drawPanel.Paint += DrawPanel_Paint;
            drawPanel.MouseDown += DrawPanel_MouseClick;
            MainPanelLayout.Add(drawPanel, 11, 38);
            MainPanel.Content = MainPanelLayout;

            // Determine the offset of minimap from panel edges
            offset = new int[] { (drawPanel.Width - 2 * Map.XDim) / 2, (drawPanel.Height - Map.YDim) / 2 };
            mapDrawSq = mapStartXY = new int[] { 0, 0 };
        }


        private void DrawPanel_Paint(object sender, PaintEventArgs e)
        {
            // Draw map
            for (var row = 0; row < Map.YDim; row++)
                for (var col = 0; col < Map.XDim; col++)
                    if (Map.WhichCivsMapShown == 8 || Map.Tile[col, row].Visibility[Map.WhichCivsMapShown])
                    {
                        var drawColor = (Map.Tile[col, row].Type == TerrainType.Ocean) ? Color.FromArgb(0, 0, 95) : Color.FromArgb(55, 123, 23);
                        e.Graphics.FillRectangle(new SolidBrush(drawColor), offset[0] + 2 * col + (row % 2), offset[1] + row, 2, 1);
                    }

            // Draw cities
            foreach (City city in Game.AllCities)
            {
                if (Map.WhichCivsMapShown == 8 || Map.IsTileVisibleC2(city.X, city.Y, Map.WhichCivsMapShown))
                {
                    e.Graphics.FillRectangle(new SolidBrush(MapImages.PlayerColours[city.Owner.Id].TextColour), offset[0] + city.X, offset[1] + city.Y, 2, 1);
                }
            }

            // Draw current view rectangle
            e.Graphics.DrawRectangle(new Pen(Colors.White), offset[0] + mapStartXY[0], offset[1] + mapStartXY[1], mapDrawSq[0], mapDrawSq[1]);
        }

        private void DrawPanel_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Buttons == MouseButtons.Primary)
            {
                int clickedX = (int)e.Location.X;
                int clickedY = (int)e.Location.Y;
                // Determine if you clicked within the drawn minimap
                if (clickedX >= offset[0] && clickedX < offset[0] + Map.XDimMax && clickedY >= offset[1] && clickedY < offset[1] + Map.YDim)
                {
                    OnMapEvent?.Invoke(null, new MapEventArgs(MapEventType.MapViewChanged, new int[] { clickedX - offset[0], clickedY - offset[1] }));
                }
            }
            else
            {
                // TODO: right click logic on minimap panel
            }
        }

        private void MapEventHappened(object sender, MapEventArgs e)
        {
            switch (e.EventType)
            {
                case MapEventType.MapViewChanged:
                    {
                        mapStartXY = e.MapStartXY;
                        mapDrawSq = e.MapDrawSq;
                        drawPanel.Invalidate();
                        break;
                    }
                case MapEventType.ToggleBetweenCurrentEntireMapView:
                    {
                        drawPanel.Invalidate();
                        break;
                    }
                default: break;
            }
        }

        private static Bitmap MakeCursor()
        {
            var _cursorBmp = new Bitmap(15, 15, PixelFormat.Format32bppRgba);
            using (var g = new Graphics(_cursorBmp))
            {
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
            }
            return _cursorBmp;
        }
    }
}
