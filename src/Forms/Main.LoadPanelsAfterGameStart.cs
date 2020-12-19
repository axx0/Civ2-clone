using System.Windows.Forms;
using System.Drawing;

namespace civ2.Forms
{
    public partial class Main : Form
    {
        public void LoadPanelsAfterGameStart()
        {
            ChoiceMenu.Visible = false;
            ChoiceMenu.Dispose();
            ChoiceMenu = null;
            MainMenuStrip.Enabled = true;

            SinaiPanel.Dispose();
            SinaiPanel = null;

            // Initialize some variables
            ViewPieceMode = Game.ActiveUnit == null;    // If no unit is active at start (all units ended turn or none exist) go to View piece mode

            MapPanel = new MapPanel(this, ClientSize.Width - 262, ClientSize.Height - MainMenuStrip.Height)
            {
                Location = new Point(0, MainMenuStrip.Height)
            };
            Controls.Add(MapPanel);
            MapPanel.BringToFront();

            MinimapPanel = new _MinimapPanel(this, 262, 149)
            {
                Location = new Point(ClientSize.Width - 262, MainMenuStrip.Height)
            };
            Controls.Add(MinimapPanel);

            StatusPanel = new _StatusPanel(this,262, ClientSize.Height - MainMenuStrip.Height - 148)
            {
                Location = new Point(ClientSize.Width - 262, MainMenuStrip.Height + 148)
            };
            Controls.Add(StatusPanel);

            //ZoomInItem.Click += MapPanel.ZoomINclicked;
            //ZoomOutItem.Click += MapPanel.ZoomOUTclicked;
            //MaxZoomInItem.Click += MapPanel.MaxZoomINclicked;
            //MaxZoomOutItem.Click += MapPanel.MaxZoomOUTclicked;
            //StandardZoomItem.Click += MapPanel.StandardZOOMclicked;
            //MediumZoomOutItem.Click += MapPanel.MediumZoomOUTclicked;
            _StatusPanel.OnMapEvent += MapEventHappened;
            //MapPanel.OnMapEvent += MapEventHappened;
        }
    }
}
