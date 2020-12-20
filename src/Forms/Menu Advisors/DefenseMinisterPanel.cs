using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using civ2.Bitmaps;
using civ2.Units;

namespace civ2.Forms
{
    public partial class DefenseMinisterPanel : Civ2panel
    {
        Game Game => Game.Instance;

        private readonly Main Main;
        private readonly VScrollBar _verticalBar;
        private readonly Civ2button _closeButton, _casualtiesButton;
        private readonly int[] _activeUnitCount, _unitInProductionCount;
        private bool _showStatistics;  // true=statistics are shown, false=casualties are shown
        private int _barValue;       // Starting value of view of horizontal bar

        public DefenseMinisterPanel(Main parent, int _width, int _height) : base(_width, _height, null, 11, 10)
        {
            Main = parent;

            // Add DrawPanel from base control
            Controls.Add(DrawPanel);
            DrawPanel.BackgroundImage = Images.DefenseMinWallpaper;
            DrawPanel.Paint += DrawPanel_Paint;

            _showStatistics = true;

            // Casualties button
            _casualtiesButton = new Civ2button
            {
                Location = new Point(2, 373),
                Size = new Size(297, 24),
                Font = new Font("Times New Roman", 11),
                Text = "Casualties"
            };
            DrawPanel.Controls.Add(_casualtiesButton);
            _casualtiesButton.Click += new EventHandler(CasualtiesButton_Click);

            // Close button
            _closeButton = new Civ2button
            {
                Location = new Point(301, 373),
                Size = new Size(297, 24),
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
                // TODO: determine maximum value of Vscrollbar
            };
            DrawPanel.Controls.Add(_verticalBar);
            _verticalBar.ValueChanged += new EventHandler(VerticalBarValueChanged);

            // Count active units, units in production
            _activeUnitCount = new int[62];
            _unitInProductionCount = new int[62];
            for (int i = 0; i < 62; i++)
            {
                // Count active units
                foreach (IUnit unit in Game.GetUnits.Where(n => n.Owner == Game.ActiveCiv))
                {
                    if (unit.Name == Game.Rules.UnitName[i]) _activeUnitCount[i]++;
                }
                // Count units in production
                foreach (City city in Game.GetCities.Where(n => n.Owner == Game.ActiveCiv))
                {
                    if (city.ItemInProduction == i) _unitInProductionCount[i]++;
                }
            }
        }

        private void DrawPanel_Paint(object sender, PaintEventArgs e)
        {
            // Draw text
            string statText = _showStatistics ? "Statistics" : "Casualties";
            string bcad = (Game.GameYear < 0) ? "B.C." : "A.D.";
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            e.Graphics.DrawString("DEFENSE MINISTER: " + statText, new Font("Times New Roman", 14), new SolidBrush(Color.FromArgb(67, 67, 67)), new Point(302 + 2, 3 + 1), sf);
            e.Graphics.DrawString("DEFENSE MINISTER: " + statText, new Font("Times New Roman", 14), new SolidBrush(Color.FromArgb(223, 223, 223)), new Point(302, 3), sf);
            e.Graphics.DrawString($"Kingdom of the {Game.ActiveCiv.TribeName}", new Font("Times New Roman", 14), new SolidBrush(Color.FromArgb(67, 67, 67)), new Point(302 + 2, 24 + 1), sf);
            e.Graphics.DrawString($"Kingdom of the {Game.ActiveCiv.TribeName}", new Font("Times New Roman", 14), new SolidBrush(Color.FromArgb(223, 223, 223)), new Point(302, 24), sf);
            e.Graphics.DrawString($"King {Game.ActiveCiv.LeaderName} : {Math.Abs(Game.GameYear)} {bcad}", new Font("Times New Roman", 14), new SolidBrush(Color.FromArgb(67, 67, 67)), new Point(302 + 2, 45 + 1), sf);
            e.Graphics.DrawString($"King {Game.ActiveCiv.LeaderName} : {Math.Abs(Game.GameYear)} {bcad}", new Font("Times New Roman", 14), new SolidBrush(Color.FromArgb(223, 223, 223)), new Point(302, 45), sf);
            // Units
            if (_showStatistics)
            {
                int count = 0;
                for (int i = 0; i < 62; i++)
                {
                    if (_activeUnitCount[i] > 0)
                    {
                        int civId = 1;  // your civ only
                        // Image of unit
                        e.Graphics.DrawImage(Draw.UnitType(i, civId), new Point(4 + 64 * ((count + 1) % 2), 69 + 24 * count));
                        // Unit name
                        e.Graphics.DrawString(Game.Rules.UnitName[i], new Font("Times New Roman", 11, FontStyle.Bold), new SolidBrush(Color.FromArgb(67, 67, 67)), new Point(142 + 1, 85 + 24 * count + 1));
                        e.Graphics.DrawString(Game.Rules.UnitName[i], new Font("Times New Roman", 11, FontStyle.Bold), new SolidBrush(Color.FromArgb(223, 223, 223)), new Point(142, 85 + 24 * count));
                        // Unit attack/defense/movement
                        e.Graphics.DrawString($"{Game.Rules.UnitAttack[i]} / {Game.Rules.UnitDefense[i]} / {Game.Rules.UnitMove[i]}", new Font("Times New Roman", 11, FontStyle.Bold), new SolidBrush(Color.FromArgb(67, 67, 67)), new Point(255 + 1, 85 + 24 * count + 1), sf);
                        e.Graphics.DrawString($"{Game.Rules.UnitAttack[i]} / {Game.Rules.UnitDefense[i]} / {Game.Rules.UnitMove[i]}", new Font("Times New Roman", 11, FontStyle.Bold), new SolidBrush(Color.FromArgb(223, 223, 223)), new Point(255, 85 + 24 * count), sf);
                        // Hitpoints/firepower
                        e.Graphics.DrawString($"{Game.Rules.UnitHitp[i]} / {Game.Rules.UnitFirepwr[i]}", new Font("Times New Roman", 11, FontStyle.Bold), new SolidBrush(Color.FromArgb(67, 67, 67)), new Point(300 + 1, 85 + 24 * count + 1), sf);
                        e.Graphics.DrawString($"{Game.Rules.UnitHitp[i]} / {Game.Rules.UnitFirepwr[i]}", new Font("Times New Roman", 11, FontStyle.Bold), new SolidBrush(Color.FromArgb(223, 223, 223)), new Point(300, 85 + 24 * count), sf);
                        // No of active units
                        e.Graphics.DrawString($"{_activeUnitCount[i]} active", new Font("Times New Roman", 11, FontStyle.Bold), new SolidBrush(Color.Black), new Point(325 + 1, 85 + 24 * count + 1));
                        e.Graphics.DrawString($"{_activeUnitCount[i]} active", new Font("Times New Roman", 11, FontStyle.Bold), new SolidBrush(Color.FromArgb(255, 223, 79)), new Point(325, 85 + 24 * count));
                        // No of units in production
                        if (_unitInProductionCount[i] > 0)
                        {
                            e.Graphics.DrawString($"{_unitInProductionCount[i]} in prod", new Font("Times New Roman", 11, FontStyle.Bold), new SolidBrush(Color.Black), new Point(390 + 1, 85 + 24 * count + 1));
                            e.Graphics.DrawString($"{_unitInProductionCount[i]} in prod", new Font("Times New Roman", 11, FontStyle.Bold), new SolidBrush(Color.FromArgb(63, 187, 199)), new Point(390, 85 + 24 * count));
                        }
                        count++;
                    }
                }
            }
            else
            {
                // TO-DO: show casualties
            }
            sf.Dispose();
        }

        // Switch between statistics (shows active units) & casualties (shows dead units)
        private void CasualtiesButton_Click(object sender, EventArgs e)
        {
            if (_showStatistics)
            {
                _showStatistics = false;
                _casualtiesButton.Text = "Statistics";
            }
            else
            {
                _showStatistics = true;
                _casualtiesButton.Text = "Casualties";
            }
            DrawPanel.Refresh();
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
