using System;
using Eto.Forms;
using Eto.Drawing;
using Civ2engine;
using Civ2engine.Enums;
using Civ2engine.Events;

namespace EtoFormsUI
{
    public class MinimapPanel : Civ2panel
    {
        Game Game => Game.Instance;
        Map Map => Map.Instance;

        private Main Main;
        private readonly Cursor MinimapCursor;
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
                BackgroundColor = Colors.Black
            };
            drawPanel.Paint += DrawPanel_Paint;
            drawPanel.MouseEnter += DrawPanel_MouseHover;
            drawPanel.MouseDown += DrawPanel_MouseClick;
            MainPanelLayout.Add(drawPanel, 11, 38);
            MainPanel.Content = MainPanelLayout;

            //MinimapCursor = new Cursor(new MemoryStream(Properties.Resources.MinimapCursor));

            // Determine the offset of minimap from panel edges
            offset = new int[] { (drawPanel.Width - 2 * Map.XDim) / 2, (drawPanel.Height - Map.YDim) / 2 };

            mapStartXY = new int[] { 0, 0 };
            mapDrawSq = new int[] { 0, 0 };
        }


        private void DrawPanel_Paint(object sender, PaintEventArgs e)
        {
            // Draw map
            for (var row = 0; row < Map.YDim; row++)
                for (var col = 0; col < Map.XDim; col++)
                    if (Map.WhichCivsMapShown == 8 || Map.Visibility[col, row][Map.WhichCivsMapShown])
                    {
                        var drawColor = (Map.Tile[col, row].Type == TerrainType.Ocean) ? Color.FromArgb(0, 0, 95) : Color.FromArgb(55, 123, 23);
                        e.Graphics.FillRectangle(new SolidBrush(drawColor), offset[0] + 2 * col + (row % 2), offset[1] + row, 2, 1);
                    }

            // Draw cities
            foreach (City city in Game.GetCities)
            {
                if (Map.WhichCivsMapShown == 8 || Map.IsTileVisibleC2(city.X, city.Y, Map.WhichCivsMapShown))
                {
                    e.Graphics.FillRectangle(new SolidBrush(MapImages.TextColours[city.Owner.Id]), offset[0] + city.X, offset[1] + city.Y, 2, 1);
                }
            }

            // Draw current view rectangle
            e.Graphics.DrawRectangle(new Pen(Colors.White), offset[0] + mapStartXY[0], offset[1] + mapStartXY[1], mapDrawSq[0], mapDrawSq[1]);
        }
        // TODO: Make sure minimap rectangle is correct immediately after game loading

        private void DrawPanel_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Buttons == MouseButtons.Primary)
            {
                int clickedX = (int)e.Location.X;
                int clickedY = (int)e.Location.Y;
                // Determine if you clicked within the drawn minimap
                if (clickedX >= offset[0] && clickedX < offset[0] + 2 * Map.XDim && clickedY >= offset[1] && clickedY < offset[1] + Map.YDim)
                {
                    OnMapEvent?.Invoke(null, new MapEventArgs(MapEventType.MapViewChanged, new int[] { clickedX - offset[0], clickedY - offset[1] }));
                }
            }
            else
            {
                // TODO: right click logic on minimap panel
            }
        }

        private void DrawPanel_MouseHover(object sender, EventArgs e)
        {
            if (drawPanel.Cursor != Cursors.Crosshair) drawPanel.Cursor = MinimapCursor;
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
    }
}
