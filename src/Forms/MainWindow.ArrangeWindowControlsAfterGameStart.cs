using System.Windows.Forms;
using System.Drawing;

namespace civ2.Forms
{
    public partial class MainWindow : Form
    {
        public void ArrangeWindowControlsAfterGameStart()
        {
            ChoiceMenu.Visible = false;
            ChoiceMenu.Dispose();
            ChoiceMenu = null;
            MainMenuStrip.Enabled = true;

            SinaiPanel.Dispose();
            SinaiPanel = null;

            MapPanel = new MapPanel(ClientSize.Width - 262, ClientSize.Height - MainMenuStrip.Height);
            MapPanel.Location = new Point(0, MainMenuStrip.Height);
            Controls.Add(MapPanel);
            MapPanel.BringToFront();

            //MapPanel = new MapPanel(instance, ClientSize.Width - 262, ClientSize.Height - MainMenuStrip.Height);
            //MapPanel.Location = new Point(0, MainMenuStrip.Height);
            //Controls.Add(MapPanel);
            //ZoomInItem.Click += MapPanel.ZoomINclicked;
            //ZoomOutItem.Click += MapPanel.ZoomOUTclicked;
            //MaxZoomInItem.Click += MapPanel.MaxZoomINclicked;
            //MaxZoomOutItem.Click += MapPanel.MaxZoomOUTclicked;
            //StandardZoomItem.Click += MapPanel.StandardZOOMclicked;
            //MediumZoomOutItem.Click += MapPanel.MediumZoomOUTclicked;
            //StatusPanel.OnMapEvent += MapEventHappened;
            //MapPanel.OnMapEvent += MapEventHappened;

            //MinimapPanel = new MinimapPanel(262, 149);
            //MinimapPanel.Location = new Point(ClientSize.Width - 262, MainMenuStrip.Height);
            //Controls.Add(MinimapPanel);

            //StatusPanel = new StatusPanel(262, ClientSize.Height - MainMenuStrip.Height - 148);
            //StatusPanel.Location = new Point(ClientSize.Width - 262, MainMenuStrip.Height + 148);
            //Controls.Add(StatusPanel);
        }
    }
}
