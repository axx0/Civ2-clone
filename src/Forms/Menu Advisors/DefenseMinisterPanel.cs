using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using civ2.Bitmaps;
using civ2.Units;

namespace civ2.Forms
{
    public class DefenseMinisterPanel : Civ2panel
    {
        private Game _game => Game.Instance;

        private readonly Main _main;
        private readonly Civ2button _casualtiesButton;
        private readonly VScrollBar _verticalBar;
        private readonly int[] _activeUnitCount, _unitInProductionCount;
        private bool _showStatistics;  // true=statistics are shown, false=casualties are shown
        private int _barValue;       // Starting value of view of horizontal bar

        public DefenseMinisterPanel(Main parent, int _width, int _height) : base(_width, _height, null, 11, 10)
        {
            _main = parent;

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
                Text = "Casualties"
            };
            DrawPanel.Controls.Add(_casualtiesButton);
            _casualtiesButton.Click += CasualtiesButton_Click;

            // Close button
            var _closeButton = new Civ2button
            {
                Location = new Point(301, 373),
                Size = new Size(297, 24),
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
                // TODO: determine maximum value of Vscrollbar
            };
            DrawPanel.Controls.Add(_verticalBar);
            _verticalBar.ValueChanged += VerticalBarValueChanged;

            // Count active units, units in production
            _activeUnitCount = new int[62];
            _unitInProductionCount = new int[62];
            for (int i = 0; i < 62; i++)
            {
                // Count active units
                foreach (IUnit unit in _game.GetUnits.Where(n => n.Owner == _game.ActiveCiv))
                {
                    if (unit.Name == _game.Rules.UnitName[i]) _activeUnitCount[i]++;
                }
                // Count units in production
                foreach (City city in _game.GetCities.Where(n => n.Owner == _game.ActiveCiv))
                {
                    if (city.ItemInProduction == i) _unitInProductionCount[i]++;
                }
            }
        }

        private void DrawPanel_Paint(object sender, PaintEventArgs e)
        {
            // Draw text
            string statText = _showStatistics ? "Statistics" : "Casualties";
            string bcad = (_game.GameYear < 0) ? "B.C." : "A.D.";

            using var font1 = new Font("Times New Roman", 14);
            Draw.Text(e.Graphics, "DEFENSE MINISTER: " + statText, font1, StringAlignment.Center, StringAlignment.Near, Color.FromArgb(223, 223, 223), new Point(302, 3), Color.FromArgb(67, 67, 67), 2, 1);
            Draw.Text(e.Graphics, $"Kingdom of the {_game.ActiveCiv.TribeName}", font1, StringAlignment.Center, StringAlignment.Near, Color.FromArgb(223, 223, 223), new Point(302, 24), Color.FromArgb(67, 67, 67), 2, 1);
            Draw.Text(e.Graphics, $"King {_game.ActiveCiv.LeaderName} : {Math.Abs(_game.GameYear)} {bcad}", font1, StringAlignment.Center, StringAlignment.Near, Color.FromArgb(223, 223, 223), new Point(302, 45), Color.FromArgb(67, 67, 67), 2, 1);

            // Units
            if (_showStatistics)
            {
                int count = 0;
                for (int i = 0; i < 62; i++)
                {
                    if (_activeUnitCount[i] > 0)
                    {
                        // Image of unit
                        using var unitPic = Draw.UnitType(i, _game.ActiveCiv.Id);
                        e.Graphics.DrawImage(unitPic, new Point(4 + 64 * ((count + 1) % 2), 69 + 24 * count));

                        // Unit name
                        using var font2 = new Font("Times New Roman", 11, FontStyle.Bold);
                        Draw.Text(e.Graphics, _game.Rules.UnitName[i], font2, StringAlignment.Near, StringAlignment.Near, Color.FromArgb(223, 223, 223), new Point(142, 85 + 24 * count), Color.FromArgb(67, 67, 67), 1, 1);

                        // Unit attack/defense/movement
                        Draw.Text(e.Graphics, $"{_game.Rules.UnitAttack[i]} / {_game.Rules.UnitDefense[i]} / {_game.Rules.UnitMove[i]}", font2, StringAlignment.Center, StringAlignment.Near, Color.FromArgb(223, 223, 223), new Point(255, 85 + 24 * count), Color.FromArgb(67, 67, 67), 1, 1);

                        // Hitpoints/firepower
                        Draw.Text(e.Graphics, $"{_game.Rules.UnitHitp[i]} / {_game.Rules.UnitFirepwr[i]}", font2, StringAlignment.Center, StringAlignment.Near, Color.FromArgb(223, 223, 223), new Point(300, 85 + 24 * count), Color.FromArgb(67, 67, 67), 1, 1);

                        // No of active units
                        Draw.Text(e.Graphics, $"{_activeUnitCount[i]} active", font2, StringAlignment.Near, StringAlignment.Near, Color.FromArgb(255, 223, 79), new Point(325, 85 + 24 * count), Color.Black, 1, 1);
                        
                        // No of units in production
                        if (_unitInProductionCount[i] > 0)
                        {
                            Draw.Text(e.Graphics, $"{_unitInProductionCount[i]} in prod", font2, StringAlignment.Near, StringAlignment.Near, Color.FromArgb(63, 187, 199), new Point(390, 85 + 24 * count), Color.Black, 1, 1);
                        }
                        count++;
                    }
                }
            }
            else
            {
                // TO-DO: show casualties
            }
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
