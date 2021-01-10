using System.Windows.Forms;
using System.Drawing;

namespace civ2.Forms
{
    public partial class Main : Form
    {
        public void LoadPanelsAfterGameStart()
        {
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
