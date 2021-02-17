using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Civ2engine;

namespace WinFormsUI
{
    public class ScienceAdvisorPanel : Civ2panel
    {
        private Game _game => Game.Instance;

        private readonly Main _main;
        private readonly HScrollBar _horizontalBar;
        private readonly List<int> _discoveredAdvances;
        private readonly int _techsPerColumn;
        private int _barValue;       // Starting value of view of horizontal bar

        public ScienceAdvisorPanel(Main parent, int _width, int _height) : base(_width, _height, null, 11, 10)
        {
            _main = parent;

            // Add DrawPanel from base control
            Controls.Add(DrawPanel);
            DrawPanel.BackgroundImage = Images.ScienceAdvWallpaper;
            DrawPanel.Paint += DrawPanel_Paint;

            _barValue = 0;

            // Goal button
            var _goalButton = new Civ2button
            {
                Location = new Point(2, 373),
                Size = new Size(297, 24),
                Text = "Goal"
            };
            DrawPanel.Controls.Add(_goalButton);
            _goalButton.Click += GoalButton_Click;

            // Close button
            var _closeButton = new Civ2button
            {
                Location = new Point(301, 373),
                Size = new Size(297, 24),
                Text = "Close"
            };
            DrawPanel.Controls.Add(_closeButton);
            _closeButton.Click += CloseButton_Click;

            // Horizontal bar
            _techsPerColumn = (_game.ActiveCiv.ReseachingTech == 255) ? 11 : 8;  // 11 techs per column = you discovered all advances. Otherwise 8.
            _horizontalBar = new HScrollBar()
            {
                Location = new Point(2, 353),
                Size = new Size(596, 17),
                LargeChange = 1,
                Maximum = _game.ActiveCiv.Techs.Sum(x => x ? 1 : 0) / _techsPerColumn  // Maximum=0 if no of techs <= 8, maximum=1 for techs=9...16, etc. 
                                                                                      // (slider cannot move if maximum=0, it can move 1 move if maximum=1, it can move 2 moves if maximum=2, ...)
            };
            DrawPanel.Controls.Add(_horizontalBar);
            _horizontalBar.ValueChanged += HorizontalBarValueChanged;

            // Create a list of discovered advances
            _discoveredAdvances = new List<int>();
            for (int i = 0; i < 89; i++)    // Browse through all advances
            {
                if (_game.ActiveCiv.Techs[i]) _discoveredAdvances.Add(i);
            }
        }

        private void DrawPanel_Paint(object sender, PaintEventArgs e)
        {
            // Draw text
            string bcad = (_game.GameYear < 0) ? "B.C." : "A.D.";
            using var font1 = new Font("Times New Roman", 14);
            Draw.Text(e.Graphics, "SCIENCE ADVISOR", font1, StringAlignment.Center, StringAlignment.Near, Color.FromArgb(223, 223, 223), new Point(302, 3), Color.FromArgb(67, 67, 67), 2, 1);
            Draw.Text(e.Graphics, $"Kingdom of the {_game.ActiveCiv.TribeName}", font1, StringAlignment.Center, StringAlignment.Near, Color.FromArgb(223, 223, 223), new Point(302, 24), Color.FromArgb(67, 67, 67), 2, 1);
            Draw.Text(e.Graphics, $"King {_game.ActiveCiv.LeaderName} : {Math.Abs(_game.GameYear)} {bcad}", font1, StringAlignment.Center, StringAlignment.Near, Color.FromArgb(223, 223, 223), new Point(302, 45), Color.FromArgb(67, 67, 67), 2, 1);

            // If civ discovered all techs
            if (_game.ActiveCiv.ReseachingTech == 255)
            {
                Draw.Text(e.Graphics, "Discoveries Every 80 Turns", font1, StringAlignment.Center, StringAlignment.Near, Color.FromArgb(223, 223, 223), new Point(302, 71), Color.FromArgb(67, 67, 67), 2, 1);
            }
            // If civ is still researching
            else
            {
                using var _pen = new Pen(Color.White);
                e.Graphics.DrawRectangle(_pen, new Rectangle(2, 73, 595, 53));  // White rectangle
                Draw.Text(e.Graphics, $"Researching: {_game.Rules.AdvanceName[_game.ActiveCiv.ReseachingTech]}", font1, StringAlignment.Center, StringAlignment.Near, Color.FromArgb(223, 223, 223), new Point(302, 74), Color.FromArgb(67, 67, 67), 2, 1);
                Draw.Text(e.Graphics, "Discoveries Every 80 Turns", font1, StringAlignment.Center, StringAlignment.Near, Color.FromArgb(223, 223, 223), new Point(302, 130), Color.FromArgb(67, 67, 67), 2, 1);
            }
            // Write discovered advances
            using var font2 = new Font("Times New Roman", 11, FontStyle.Bold);
            int count = 0;
            int startingOffset = (_game.ActiveCiv.ReseachingTech == 255) ? 100 : 160;
            for (int i = _barValue * _techsPerColumn; i < _discoveredAdvances.Count; i++)
            {
                int x = 198 * (count / _techsPerColumn);
                int y = 22 * (count % _techsPerColumn);
                e.Graphics.DrawImage(Images.ResearchIcons[_game.Rules.AdvanceCategory[_discoveredAdvances[i]], _game.Rules.AdvanceEpoch[_discoveredAdvances[i]]], new Point(4 + x, startingOffset - 1 + y));
                Draw.Text(e.Graphics, _game.Rules.AdvanceName[_discoveredAdvances[i]], font1, StringAlignment.Near, StringAlignment.Near, Color.FromArgb(63, 187, 199), new Point(x + 42, y + startingOffset), Color.FromArgb(67, 67, 67), 2, 1);
                count++;
                if (count == 3 * _techsPerColumn) break; // Only 3 columns can be shown
            }
        }

        private void GoalButton_Click(object sender, EventArgs e) { }

        private void CloseButton_Click(object sender, EventArgs e)
        {
            this.Visible = false;
            this.Dispose();
        }

        // Once slider value changes --> redraw list
        private void HorizontalBarValueChanged(object sender, EventArgs e)
        {
            _barValue = _horizontalBar.Value;
            DrawPanel.Invalidate();
        }
    }
}
