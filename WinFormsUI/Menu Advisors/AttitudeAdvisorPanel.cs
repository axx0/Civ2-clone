using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Civ2engine;

namespace WinFormsUI
{
    public class AttitudeAdvisorPanel : Civ2panel
    {
        private Game _game => Game.Instance;

        private readonly Main _main;
        private readonly VScrollBar _verticalBar;
        private int _barValue;       // Starting value of view of horizontal bar

        public AttitudeAdvisorPanel(Main parent, int _width, int _height) : base(_width, _height, null, 11, 10)
        {
            _main = parent;

            // Add DrawPanel from base control
            Controls.Add(DrawPanel);
            DrawPanel.BackgroundImage = Images.AttitudeAdvWallpaper;
            DrawPanel.Paint += DrawPanel_Paint;

            // Close button
            var _closeButton = new Civ2button
            {
                Location = new Point(2, 373),
                Size = new Size(596, 24),
                Text = "Close"
            };
            DrawPanel.Controls.Add(_closeButton);
            _closeButton.Click += CloseButton_Click;

            // Vertical bar
            _verticalBar = new VScrollBar()
            {
                Location = new Point(581, 66),
                Size = new Size(17, 305),
                LargeChange = 1
                //Maximum = TO-DO...
            };
            DrawPanel.Controls.Add(_verticalBar);
            _verticalBar.ValueChanged += VerticalBarValueChanged;
        }

        private void DrawPanel_Paint(object sender, PaintEventArgs e)
        {
            // Text
            string bcad = (_game.GameYear < 0) ? "B.C." : "A.D.";
            using var font1 = new Font("Times New Roman", 14);
            Draw.Text(e.Graphics, "ATTITUDE ADVISOR", font1, StringAlignment.Center, StringAlignment.Near, Color.FromArgb(223, 223, 223), new Point(302, 3), Color.FromArgb(67, 67, 67), 2, 1);
            Draw.Text(e.Graphics, $"Kingdom of the {_game.ActiveCiv.TribeName}", font1, StringAlignment.Center, StringAlignment.Near, Color.FromArgb(223, 223, 223), new Point(302, 24), Color.FromArgb(67, 67, 67), 2, 1);
            Draw.Text(e.Graphics, $"King {_game.ActiveCiv.LeaderName} : {Math.Abs(_game.GameYear)} {bcad}", font1, StringAlignment.Center, StringAlignment.Near, Color.FromArgb(223, 223, 223), new Point(302, 45), Color.FromArgb(67, 67, 67), 2, 1);

            // Cities
            int count = 0;
            foreach (City city in _game.GetCities.Where(n => n.Owner == _game.ActiveCiv))
            {
                // City image
                //e.Graphics.DrawImage(Draw.City(city, true, 0), new Point(4 + 64 * ((count + 1) % 2), 69 + 32 * count));

                // City name
                using var font2 = new Font("Times New Roman", 11, FontStyle.Bold);
                Draw.Text(e.Graphics, city.Name, font2, StringAlignment.Near, StringAlignment.Near, Color.FromArgb(223, 223, 223), new Point(142, 82 + 32 * count), Color.FromArgb(67, 67, 67), 1, 1);

                // Faces
                //e.Graphics.DrawImage(Draw.DrawFaces(city, 1), new Point(220, 69 + 32 * count));
                count++;
            }
        }

        private void CloseButton_Click(object sender, EventArgs e)
        {
            this.Visible = false;
            this.Dispose();
        }

        // Once slider value changes --> redraw list
        private void VerticalBarValueChanged(object sender, EventArgs e)
        {
            _barValue = _verticalBar.Value;
            DrawPanel.Invalidate();
        }
    }
}
