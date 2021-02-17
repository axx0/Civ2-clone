using System.Windows.Forms;
using System.Drawing;
using Civ2engine;

namespace WinFormsUI
{
    public partial class Main : Form
    {
        public void LoadGameInitialization(string directoryPath, string SAVname)
        {
            Game.LoadGame(directoryPath, SAVname);
            Images.LoadGraphicsAssetsFromFiles(directoryPath);

            // Make graphics for all tiles
            for (int col = 0; col < Map.Xdim; col++)
            {
                for (int row = 0; row < Map.Ydim; row++)
                {
                    Map.Tile[col, row].Graphic = Draw.MakeTileGraphic(Map.Tile[col, row], col, row, Game.Options.FlatEarth);
                }
            }

            ViewPieceMode = Game.ActiveUnit == null;

            _choiceMenu.Visible = false;
            _choiceMenu.Dispose();
            _choiceMenu = null;
            MainMenuStrip.Enabled = true;

            _sinaiPanel.Dispose();
            _sinaiPanel = null;

            _mapPanel = new MapPanel(this, ClientSize.Width - 262, ClientSize.Height - MainMenuStrip.Height)
            {
                Location = new Point(0, MainMenuStrip.Height)
            };
            Controls.Add(_mapPanel);
            _mapPanel.BringToFront();

            _minimapPanel = new _MinimapPanel(this, 262, 149)
            {
                Location = new Point(ClientSize.Width - 262, MainMenuStrip.Height)
            };
            Controls.Add(_minimapPanel);

            _statusPanel = new StatusPanel(this,262, ClientSize.Height - MainMenuStrip.Height - 148)
            {
                Location = new Point(ClientSize.Width - 262, MainMenuStrip.Height + 148)
            };
            Controls.Add(_statusPanel);

            //ZoomInItem.Click += MapPanel.ZoomINclicked;
            //ZoomOutItem.Click += MapPanel.ZoomOUTclicked;
            //MaxZoomInItem.Click += MapPanel.MaxZoomINclicked;
            //MaxZoomOutItem.Click += MapPanel.MaxZoomOUTclicked;
            //StandardZoomItem.Click += MapPanel.StandardZOOMclicked;
            //MediumZoomOutItem.Click += MapPanel.MediumZoomOUTclicked;
            StatusPanel.OnMapEvent += MapEventHappened;
            //MapPanel.OnMapEvent += MapEventHappened;
        }
    }
}
