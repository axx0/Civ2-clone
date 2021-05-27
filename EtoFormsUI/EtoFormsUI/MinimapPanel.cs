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
        private readonly int[] Offset;
        private int[] CentrXY, CentrOffset, ActiveOffset, PanelMap_offset, MapPanel_offset;
        private Rectangle MapRect1, MapRect2;
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
            Offset = new int[] { (drawPanel.Width - 2 * Map.XDim) / 2, (drawPanel.Height - Map.YDim) / 2 };

            CentrXY = new int[] { 0, 0 };
            CentrOffset = new int[] { 0, 0 };
            ActiveOffset = new int[] { 0, 0 };
            PanelMap_offset = new int[] { 0, 0 };
            MapPanel_offset = new int[] { 0, 0 };
        }


        private void DrawPanel_Paint(object sender, PaintEventArgs e)
        {
            // Draw map
            for (var row = 0; row < Map.YDim; row++)
                for (var col = 0; col < Map.XDim; col++)
                    if (Map.WhichCivsMapShown == 8 || Map.Visibility[col, row][Map.WhichCivsMapShown])
                    {
                        var drawColor = (Map.Tile[col, row].Type == TerrainType.Ocean) ? Color.FromArgb(0, 0, 95) : Color.FromArgb(55, 123, 23);
                        e.Graphics.FillRectangle(new SolidBrush(drawColor), Offset[0] + 2 * col + (row % 2), Offset[1] + row, 2, 1);
                    }

            // Draw cities
            foreach (City city in Game.GetCities)
            {
                if (Map.WhichCivsMapShown == 8 || Map.IsTileVisibleC2(city.X, city.Y, Map.WhichCivsMapShown))
                {
                    e.Graphics.FillRectangle(new SolidBrush(MapImages.TextColours[city.Owner.Id]), Offset[0] + city.X, Offset[1] + city.Y, 2, 1);
                }
            }

            // Draw current view rectangle
            //e.Graphics.DrawRectangle(new Pen(Color.White), Offset[0] + StartingSqXY[0], Offset[1] + StartingSqXY[1], DrawingSqXY[0], DrawingSqXY[1]); // TODO: correct this
        }
        // TODO: Make sure minimap rectangle is correct immediately after game loading

        private void DrawPanel_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Buttons == MouseButtons.Primary)
            {
                int clickedX = (int)e.Location.X;
                int clickedY = (int)e.Location.Y;
                // Determine if you clicked within the drawn minimap
                if (clickedX >= Offset[0] && clickedX < Offset[0] + 2 * Map.XDim && clickedY >= Offset[1] && clickedY < Offset[1] + Map.YDim)
                {
                    OnMapEvent?.Invoke(null, new MapEventArgs(MapEventType.MapViewChanged, new int[] { clickedX - Offset[0], clickedY - Offset[1] }));
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
                        CentrXY = e.CentrXY;
                        CentrOffset = e.CentrOffset;
                        ActiveOffset = e.ActiveOffset;
                        PanelMap_offset = e.PanelMap_offset;
                        MapPanel_offset = e.MapPanel_offset;
                        //MapRect1 = e.MapRect1;
                        //MapRect2 = e.MapRect2;
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
