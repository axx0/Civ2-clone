using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using civ2.Bitmaps;

namespace civ2.Forms
{
    public partial class ScienceAdvisorPanel : Civ2panel
    {
        Game Game => Game.Instance;

        private readonly Main Main;
        private readonly HScrollBar _horizontalBar;
        private readonly List<int> _discoveredAdvances;
        private readonly int _techsPerColumn;
        private int _barValue;       // Starting value of view of horizontal bar

        public ScienceAdvisorPanel(Main parent, int _width, int _height) : base(_width, _height, null, 11, 10)
        {
            Main = parent;

            // Add DrawPanel from base control
            Controls.Add(DrawPanel);
            DrawPanel.BackgroundImage = Images.ScienceAdvWallpaper;
            DrawPanel.Paint += DrawPanel_Paint;

            _barValue = 0;

            // Goal button
            Civ2button _goalButton = new Civ2button
            {
                Location = new Point(2, 373),
                Size = new Size(297, 24),
                Font = new Font("Times New Roman", 11),
                Text = "Goal"
            };
            DrawPanel.Controls.Add(_goalButton);
            _goalButton.Click += new EventHandler(GoalButton_Click);

            // Close button
            Civ2button _closeButton = new Civ2button
            {
                Location = new Point(301, 373),
                Size = new Size(297, 24),
                Font = new Font("Times New Roman", 11),
                Text = "Close"
            };
            DrawPanel.Controls.Add(_closeButton);
            _closeButton.Click += new EventHandler(CloseButton_Click);

            // Horizontal bar
            _techsPerColumn = (Game.ActiveCiv.ReseachingTech == 255) ? 11 : 8;  // 11 techs per column = you discovered all advances. Otherwise 8.
            _horizontalBar = new HScrollBar()
            {
                Location = new Point(2, 353),
                Size = new Size(596, 17),
                LargeChange = 1,
                Maximum = Game.ActiveCiv.Techs.Sum(x => x ? 1 : 0) / _techsPerColumn  // Maximum=0 if no of techs <= 8, maximum=1 for techs=9...16, etc. 
                                                                                      // (slider cannot move if maximum=0, it can move 1 move if maximum=1, it can move 2 moves if maximum=2, ...)
            };
            DrawPanel.Controls.Add(_horizontalBar);
            _horizontalBar.ValueChanged += new EventHandler(HorizontalBarValueChanged);

            // Create a list of discovered advances
            _discoveredAdvances = new List<int>();
            for (int i = 0; i < 89; i++)    // Browse through all advances
            {
                if (Game.ActiveCiv.Techs[i]) _discoveredAdvances.Add(i);
            }
        }

        private void DrawPanel_Paint(object sender, PaintEventArgs e)
        {
            // Draw text
            string bcad = (Game.GameYear < 0) ? "B.C." : "A.D.";
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            e.Graphics.DrawString("SCIENCE ADVISOR", new Font("Times New Roman", 14), new SolidBrush(Color.FromArgb(67, 67, 67)), new Point(302 + 2, 3 + 1), sf);
            e.Graphics.DrawString("SCIENCE ADVISOR", new Font("Times New Roman", 14), new SolidBrush(Color.FromArgb(223, 223, 223)), new Point(302, 3), sf);
            e.Graphics.DrawString($"Kingdom of the {Game.ActiveCiv.TribeName}", new Font("Times New Roman", 14), new SolidBrush(Color.FromArgb(67, 67, 67)), new Point(302 + 2, 24 + 1), sf);
            e.Graphics.DrawString($"Kingdom of the {Game.ActiveCiv.TribeName}", new Font("Times New Roman", 14), new SolidBrush(Color.FromArgb(223, 223, 223)), new Point(302, 24), sf);
            e.Graphics.DrawString($"King {Game.ActiveCiv.LeaderName} : {Math.Abs(Game.GameYear)} {bcad}", new Font("Times New Roman", 14), new SolidBrush(Color.FromArgb(67, 67, 67)), new Point(302 + 2, 45 + 1), sf);
            e.Graphics.DrawString($"King {Game.ActiveCiv.LeaderName} : {Math.Abs(Game.GameYear)} {bcad}", new Font("Times New Roman", 14), new SolidBrush(Color.FromArgb(223, 223, 223)), new Point(302, 45), sf);
            // If civ discovered all techs
            if (Game.ActiveCiv.ReseachingTech == 255)
            {
                e.Graphics.DrawString("Discoveries Every 80 Turns", new Font("Times New Roman", 14), new SolidBrush(Color.FromArgb(67, 67, 67)), new Point(302 + 2, 71 + 1), sf);
                e.Graphics.DrawString("Discoveries Every 80 Turns", new Font("Times New Roman", 14), new SolidBrush(Color.FromArgb(223, 223, 223)), new Point(302, 71), sf);
            }
            // If civ is still researching
            else
            {
                e.Graphics.DrawRectangle(new Pen(Color.White), new Rectangle(2, 73, 595, 53));  // White rectangle
                e.Graphics.DrawString($"Researching: {Game.Rules.AdvanceName[Game.ActiveCiv.ReseachingTech]}", new Font("Times New Roman", 14), new SolidBrush(Color.FromArgb(67, 67, 67)), new Point(302 + 2, 74 + 1), sf);
                e.Graphics.DrawString($"Researching: {Game.Rules.AdvanceName[Game.ActiveCiv.ReseachingTech]}", new Font("Times New Roman", 14), new SolidBrush(Color.FromArgb(223, 223, 223)), new Point(302, 74), sf);
                e.Graphics.DrawString("Discoveries Every 80 Turns", new Font("Times New Roman", 14), new SolidBrush(Color.FromArgb(67, 67, 67)), new Point(302 + 2, 130 + 1), sf);
                e.Graphics.DrawString("Discoveries Every 80 Turns", new Font("Times New Roman", 14), new SolidBrush(Color.FromArgb(223, 223, 223)), new Point(302, 130), sf);
            }
            sf.Dispose();
            // Write discovered advances
            int count = 0;
            int startingOffset = (Game.ActiveCiv.ReseachingTech == 255) ? 100 : 160;
            for (int i = _barValue * _techsPerColumn; i < _discoveredAdvances.Count(); i++)
            {
                int x = 198 * (count / _techsPerColumn);
                int y = 22 * (count % _techsPerColumn);
                e.Graphics.DrawImage(Images.ResearchIcons[Game.Rules.AdvanceCategory[_discoveredAdvances[i]], Game.Rules.AdvanceEpoch[_discoveredAdvances[i]]], new Point(4 + x, startingOffset - 1 + y));
                e.Graphics.DrawString(Game.Rules.AdvanceName[_discoveredAdvances[i]], new Font("Times New Roman", 11, FontStyle.Bold), new SolidBrush(Color.FromArgb(67, 67, 67)), new Point(x + 42 + 2, y + startingOffset + 1));
                e.Graphics.DrawString(Game.Rules.AdvanceName[_discoveredAdvances[i]], new Font("Times New Roman", 11, FontStyle.Bold), new SolidBrush(Color.FromArgb(63, 187, 199)), new Point(x + 42, y + startingOffset));
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
