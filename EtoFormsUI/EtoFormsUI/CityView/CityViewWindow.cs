using Civ2engine;
using Eto.Forms;
using Eto.Drawing;

namespace EtoFormsUI
{
    public class CityViewWindow : Form
    {
        private readonly Drawable drawPanel;
        private readonly City _city;

        public CityViewWindow(City city)
        {
            _city = city;

            WindowState = WindowState.Maximized;
            ShowInTaskbar = false;
            WindowStyle = WindowStyle.None;
            this.Shown += (sender, _) => drawPanel.Size = this.Size;

            var Layout = new PixelLayout();
            drawPanel = new Drawable() { BackgroundColor = Colors.Black };
            drawPanel.MouseUp += (sender, e) => Close();
            drawPanel.Paint += PaintPanel;
            Layout.Add(drawPanel, new Point(0, 0));
            Content = Layout;
        }

        private void PaintPanel(object sender, PaintEventArgs e)
        {
            if (_city.IsNextToOcean)
                e.Graphics.DrawImage(Images.CityViewOcean, new Point(0, 0));
            else if (_city.IsNextToRiver)
                e.Graphics.DrawImage(Images.CityViewRiver, new Point(0, 0));
            else
                e.Graphics.DrawImage(Images.CityViewLand, new Point(0, 0));
        }
    }
}
