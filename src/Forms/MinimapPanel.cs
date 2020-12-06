using System;
using System.Windows.Forms;
using System.IO;
using System.Drawing;
using civ2.Enums;
using civ2.Bitmaps;
using civ2.Events;

namespace civ2.Forms
{
    public partial class MinimapPanel : Civ2panel
    {
        Game Game => Game.Instance;
        Map Map => Map.Instance;

        private readonly Cursor MinimapCursor;
        private readonly int[] Offset;

        public static event EventHandler<MapEventArgs> OnMapEvent;

        public MinimapPanel(int _width, int _height) : base(_width, _height, "World", false)
        {
            MapPanel.OnMapEvent += MapEventHappened;
            MainWindow.OnMapEvent += MapEventHappened;

            Controls.Add(DrawPanel);
            DrawPanel.Paint += new PaintEventHandler(DrawPanel_Paint);
            DrawPanel.MouseClick += DrawPanel_MouseClick;
            DrawPanel.MouseHover += DrawPanel_MouseHover;

            MinimapCursor = new Cursor(new MemoryStream(Properties.Resources.MinimapCursor));

            // Determine the offset of minimap from panel edges
            Offset = new int[] { (DrawPanel.Width - 2 * Map.Xdim) / 2, (DrawPanel.Height - Map.Ydim) / 2 };
        }

        private void DrawPanel_Paint(object sender, PaintEventArgs e)
        {
            // Draw map
            Color drawColor;
            for (int row = 0; row < Map.Ydim; row++)
                for (int col = 0; col < Map.Xdim; col++)
                {
                    if (Game.WhichCivsMapShown == 8 || Map.Visibility[col, row][Game.WhichCivsMapShown])
                    {
                        drawColor = (Map.Tile[col, row].Type == TerrainType.Ocean) ? Color.FromArgb(0, 0, 95) : Color.FromArgb(55, 123, 23);
                        e.Graphics.FillRectangle(new SolidBrush(drawColor), Offset[0] + 2 * col + (row % 2), Offset[1] + row, 2, 1);
                    }
                }

            // Draw cities
            foreach (City city in Game.GetCities)
            {
                if (Game.WhichCivsMapShown == 8 || Map.Visibility[(city.X - city.Y % 2) / 2, city.Y][Game.WhichCivsMapShown])
                {
                    e.Graphics.FillRectangle(new SolidBrush(CivColors.CityTextColor[city.Owner.Id]), Offset[0] + city.X, Offset[1] + city.Y, 2, 1);
                }
            }

            // Draw current view rectangle
            // TODO: supply StartingSqXY from MapPanel to here
            //e.Graphics.DrawRectangle(new Pen(Color.White), Offset[0] + MapPanel.StartingSqXY[0], Offset[1] + MapPanel.StartingSqXY[1], MapPanel.DrawingSqXY[0], MapPanel.DrawingSqXY[1]);
            e.Dispose();
        }
        // TODO: Make sure minimap rectangle is correct immediately after game loading

        private void DrawPanel_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                int clickedX = e.Location.X;
                int clickedY = e.Location.Y;
                // Determine if you clicked within the drawn minimap
                if (clickedX >= Offset[0] && clickedX < Offset[0] + 2 * Map.Xdim && clickedY >= Offset[1] && clickedY < Offset[1] + Map.Ydim)
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
            if (DrawPanel.Cursor != Cursors.Cross) DrawPanel.Cursor = MinimapCursor;
        }

        private void MapEventHappened(object sender, MapEventArgs e)
        {
            switch (e.EventType)
            {
                case MapEventType.MapViewChanged:
                    {
                        DrawPanel.Refresh();
                        break;
                    }
                case MapEventType.ToggleBetweenCurrentEntireMapView:
                    {
                        DrawPanel.Refresh();
                        break;
                    }
                default: break;
            }
        }

    }
}
