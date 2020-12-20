using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using civ2.Bitmaps;

namespace civ2.Forms
{
    public partial class AttitudeAdvisorPanel : Civ2panel
    {
        Game Game => Game.Instance;

        private readonly Main Main;
        private readonly Button _closeButton;
        private readonly VScrollBar _verticalBar;
        private int _barValue;       // Starting value of view of horizontal bar

        public AttitudeAdvisorPanel(Main parent, int _width, int _height) : base(_width, _height, null, 11, 10)
        {
            Main = parent;

            // Add DrawPanel from base control
            Controls.Add(DrawPanel);
            DrawPanel.BackgroundImage = Images.AttitudeAdvWallpaper;
            DrawPanel.Paint += DrawPanel_Paint;

            // Close button
            _closeButton = new Civ2button
            {
                Location = new Point(2, 373),
                Size = new Size(596, 24),
                Font = new Font("Times New Roman", 11),
                Text = "Close"
            };
            DrawPanel.Controls.Add(_closeButton);
            _closeButton.Click += new EventHandler(CloseButton_Click);

            // Vertical bar
            _verticalBar = new VScrollBar()
            {
                Location = new Point(581, 66),
                Size = new Size(17, 305),
                LargeChange = 1
                //Maximum = TO-DO...
            };
            DrawPanel.Controls.Add(_verticalBar);
            _verticalBar.ValueChanged += new EventHandler(VerticalBarValueChanged);
        }

        private void DrawPanel_Paint(object sender, PaintEventArgs e)
        {
            // Text
            string bcad = (Game.GameYear < 0) ? "B.C." : "A.D.";
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            e.Graphics.DrawString("ATTITUDE ADVISOR", new Font("Times New Roman", 14), new SolidBrush(Color.FromArgb(67, 67, 67)), new Point(302 + 2, 3 + 1), sf);
            e.Graphics.DrawString("ATTITUDE ADVISOR", new Font("Times New Roman", 14), new SolidBrush(Color.FromArgb(223, 223, 223)), new Point(302, 3), sf);
            e.Graphics.DrawString($"Kingdom of the {Game.ActiveCiv.TribeName}", new Font("Times New Roman", 14), new SolidBrush(Color.FromArgb(67, 67, 67)), new Point(302 + 2, 24 + 1), sf);
            e.Graphics.DrawString($"Kingdom of the {Game.ActiveCiv.TribeName}", new Font("Times New Roman", 14), new SolidBrush(Color.FromArgb(223, 223, 223)), new Point(302, 24), sf);
            e.Graphics.DrawString($"King {Game.ActiveCiv.LeaderName} : {Math.Abs(Game.GameYear)} {bcad}", new Font("Times New Roman", 14), new SolidBrush(Color.FromArgb(67, 67, 67)), new Point(302 + 2, 45 + 1), sf);
            e.Graphics.DrawString($"King {Game.ActiveCiv.LeaderName} : {Math.Abs(Game.GameYear)} {bcad}", new Font("Times New Roman", 14), new SolidBrush(Color.FromArgb(223, 223, 223)), new Point(302, 45), sf);
            // Cities
            int count = 0;
            foreach (City city in Game.GetCities.Where(n => n.Owner == Game.ActiveCiv))
            {
                // City image
                e.Graphics.DrawImage(Draw.City(city, true, 0), new Point(4 + 64 * ((count + 1) % 2), 69 + 32 * count));
                // City name
                e.Graphics.DrawString(city.Name, new Font("Times New Roman", 11, FontStyle.Bold), new SolidBrush(Color.FromArgb(67, 67, 67)), new Point(142 + 1, 82 + 32 * count + 1));
                e.Graphics.DrawString(city.Name, new Font("Times New Roman", 11, FontStyle.Bold), new SolidBrush(Color.FromArgb(223, 223, 223)), new Point(142, 82 + 32 * count));
                // Faces
                //e.Graphics.DrawImage(Draw.DrawFaces(city, 1), new Point(220, 69 + 32 * count));
                count++;
            }
            sf.Dispose();
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
