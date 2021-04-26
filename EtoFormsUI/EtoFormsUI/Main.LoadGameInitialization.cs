using Eto.Drawing;
using Eto.Forms;
using Civ2engine;
using System.Diagnostics;

namespace EtoFormsUI
{
    public partial class Main : Form
    {
        public void LoadGameInitialization(string directoryPath, string SAVname)
        {
            Game.LoadGame(directoryPath, SAVname);
            Images.LoadGraphicsAssetsFromFiles(directoryPath);

            // Generate map tile graphics
            Images.MapTileGraphic = new Bitmap[Map.Xdim, Map.Ydim];
            for (int col = 0; col < Map.Xdim; col++)
            {
                for (int row = 0; row < Map.Ydim; row++)
                {
                    Images.MapTileGraphic[col, row] = Draw.MakeTileGraphic(Map.Tile[col, row], col, row, Game.Options.FlatEarth);
                }
            }

            //ViewPieceMode = Game.ActiveUnit == null;

            sinaiPanel.Dispose();
            sinaiPanel = null;
            foreach (MenuItem item in this.Menu.Items) item.Enabled = true;

            mapPanel = new MapPanel(this, ClientSize.Width - 262, ClientSize.Height);
            layout.Add(mapPanel, 0, 0);

            minimapPanel = new MinimapPanel(this, 262, 149);
            layout.Add(minimapPanel, ClientSize.Width - 262, 0);

            statusPanel = new StatusPanel(this, 262, ClientSize.Height - 148);
            layout.Add(statusPanel, ClientSize.Width - 262, 148);

            Content = layout;

            BringToFront();

            ////ZoomInItem.Click += MapPanel.ZoomINclicked;
            ////ZoomOutItem.Click += MapPanel.ZoomOUTclicked;
            ////MaxZoomInItem.Click += MapPanel.MaxZoomINclicked;
            ////MaxZoomOutItem.Click += MapPanel.MaxZoomOUTclicked;
            ////StandardZoomItem.Click += MapPanel.StandardZOOMclicked;
            ////MediumZoomOutItem.Click += MapPanel.MediumZoomOUTclicked;
            //StatusPanel.OnMapEvent += MapEventHappened;
            ////MapPanel.OnMapEvent += MapEventHappened;
        }
    }
}
