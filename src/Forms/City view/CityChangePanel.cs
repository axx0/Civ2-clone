using civ2.Bitmaps;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace civ2.Forms
{
    public class CityChangePanel : Civ2panel
    {
        private Game Game => Game.Instance;
        private readonly CityPanel _parent;
        private readonly City _city;
        private readonly VScrollBar _verticalBar;
        private readonly DoubleBufferedPanel _choicePanel;
        private int _totalNoUnits, _totalNoImprov, _barValue;

        public CityChangePanel(CityPanel parent, City city) : base(686, 389, $"What shall we build in {city.Name}?", 38, 46)
        {
            _parent = parent;
            _city = city;

            // Initial states
            _totalNoUnits = 62; // TO-DO: Calculate total number of units+improvements for CityChangePanel
            _totalNoImprov = 66;
            // BarValue should always be so that the chosen item is in the center. But the BarValue should be corrected once you are at the edges.
            _barValue = Math.Max(0, _city.ItemInProduction - 8);  // Correction for the lower value
            _barValue = Math.Min(_totalNoUnits + _totalNoImprov - 16, _barValue);   // Correction for the upper value

            // Add DrawPanel from base control
            Controls.Add(DrawPanel);
            DrawPanel.Paint += DrawPanel_Paint;

            // Choice panel
            _choicePanel = new DoubleBufferedPanel
            {
                Location = new Point(2, 2),
                Size = new Size(DrawPanel.Width - 4, DrawPanel.Height - 4),
                BackColor = Color.FromArgb(207, 207, 207),
                BorderStyle = BorderStyle.None
            };
            DrawPanel.Controls.Add(_choicePanel);
            _choicePanel.Paint += ChoicePanel_Paint;
            _choicePanel.MouseDown += ChoicePanel_MouseDown;

            // Vertical bar for choosing production
            _verticalBar = new VScrollBar()
            {
                Location = new Point(643, 0),
                Size = new Size(18, 301),
                Maximum = _totalNoUnits + _totalNoImprov - 7    // 16 can be shown
            };
            _choicePanel.Controls.Add(_verticalBar);
            _verticalBar.ValueChanged += VerticalBarValueChanged;

            // Auto button
            var _autoButton = new Civ2button
            {
                Location = new Point(9, 347),
                Size = new Size(165, 36),
                Text = "Auto"
            };
            Controls.Add(_autoButton);
            _autoButton.Click += AutoButton_Click;

            // Help button
            var _helpButton = new Civ2button
            {
                Location = new Point(177, 347),
                Size = new Size(165, 36),
                Text = "Help"
            };
            Controls.Add(_helpButton);
            _helpButton.Click += HelpButton_Click;

            // Cheat! button
            var _cheatButton = new Civ2button
            {
                Location = new Point(344, 347),
                Size = new Size(165, 36),
                Text = "Cheat!"
            };
            Controls.Add(_cheatButton);
            _cheatButton.Click += CheatButton_Click;

            // OK button
            var _OKButton = new Civ2button
            {
                Location = new Point(512, 347),
                Size = new Size(165, 36),
                Text = "OK"
            };
            Controls.Add(_OKButton);
            _OKButton.Click += OKButton_Click;
        }

        private void DrawPanel_Paint(object sender, PaintEventArgs e) { }

        private void ChoicePanel_MouseDown(object sender, MouseEventArgs e)
        {
            _city.ItemInProduction = _barValue + e.Location.Y / 23;
            _choicePanel.Invalidate();  // Redraw the panel 
        }

        private void ChoicePanel_Paint(object sender, PaintEventArgs e)
        {
            // Entries
            using var sf = new StringFormat();
            sf.Alignment = StringAlignment.Far;
            Color textColor;
            FontStyle fontstyle;
            for (int row = 0; row < 16; row++)
            {
                // Draw selection rectangle & set font of text in it
                if (_barValue + row == _city.ItemInProduction)
                {
                    e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(107, 107, 107)), new Rectangle(85, 2 + row * 23, 556, 21));
                    e.Graphics.DrawLine(new Pen(Color.FromArgb(223, 223, 223)), 84, 1 + row * 23, 84 + 556 + 1, 1 + row * 23);  // Border line
                    e.Graphics.DrawLine(new Pen(Color.FromArgb(223, 223, 223)), 84, 1 + row * 23, 84, 1 + row * 23 + 21);       // Border line
                    e.Graphics.DrawLine(new Pen(Color.FromArgb(67, 67, 67)), 84, 1 + row * 23 + 22, 84 + 556 + 1, 1 + row * 23 + 22);  // Border line
                    e.Graphics.DrawLine(new Pen(Color.FromArgb(67, 67, 67)), 641, 1 + row * 23, 641, 1 + row * 23 + 21);       // Border line
                    textColor = Color.White;
                    fontstyle = FontStyle.Bold;
                }
                else
                {
                    textColor = Color.Black;
                    fontstyle = FontStyle.Regular;
                }

                // Draw units
                if (_barValue + row < _totalNoUnits)
                {
                    if (_barValue + row == _city.ItemInProduction)   // Draw shadow of text for chosen line
                    {
                        e.Graphics.DrawString(Game.Rules.UnitName[_barValue + row], new Font("Times New Roman", 16, FontStyle.Bold), new SolidBrush(Color.Black),
                            new Point(85 + 1, row * 23 + 1));
                        e.Graphics.DrawString($"(20 Turns, ADM: {Game.Rules.UnitAttack[_barValue + row]} / {Game.Rules.UnitDefense[_barValue + row]} /" +
                            $" {Game.Rules.UnitMove[_barValue + row]} HP: { Game.Rules.UnitHitp[_barValue + row]} / {Game.Rules.UnitFirepwr[_barValue + row]} )",
                            new Font("Times New Roman", 16, FontStyle.Bold), new SolidBrush(Color.Black), new Point(_choicePanel.Width - _verticalBar.Width, row * 23 + 1), sf);
                    }
                    e.Graphics.DrawImage(ModifyImage.Resize(Images.Units[_barValue + row], -4), new Point(1 + ((_barValue + row) % 2) * 38, 3 + row * 23 - 8));  // big=0.75-times the normal size of units
                    e.Graphics.DrawString(Game.Rules.UnitName[_barValue + row], new Font("Times New Roman", 16, fontstyle), new SolidBrush(textColor), new Point(85, row * 23));
                    e.Graphics.DrawString($"(20 Turns, ADM: {Game.Rules.UnitAttack[_barValue + row]} / {Game.Rules.UnitDefense[_barValue + row]} / " +
                        $"{Game.Rules.UnitMove[_barValue + row]}  HP: {Game.Rules.UnitHitp[_barValue + row]} / {Game.Rules.UnitFirepwr[_barValue + row]} )",
                        new Font("Times New Roman", 16, fontstyle), new SolidBrush(textColor), new Point(_choicePanel.Width - _verticalBar.Width - 1, row * 23), sf);
                }
                // Draw improvements
                else
                {
                    int improvNo = _barValue + row - _totalNoUnits + 1;
                    if (_barValue + row == _city.ItemInProduction)   // Draw shadow of text for chosen line
                    {
                        e.Graphics.DrawString(Game.Rules.ImprovementName[improvNo], new Font("Times New Roman", 16, FontStyle.Bold), new SolidBrush(Color.Black),
                            new Point(85 + 1, row * 23 + 1));
                        e.Graphics.DrawString("(20 Turns)", new Font("Times New Roman", 16, FontStyle.Bold), new SolidBrush(Color.Black),
                            new Point(_choicePanel.Width - _verticalBar.Width, row * 23 + 1), sf);
                    }
                    e.Graphics.DrawImage(Images.Improvements[improvNo], new Point(1 + ((_barValue + row) % 2) * 38, 3 + row * 23));
                    e.Graphics.DrawString(Game.Rules.ImprovementName[improvNo], new Font("Times New Roman", 16, fontstyle), new SolidBrush(textColor), new Point(85, row * 23));
                    e.Graphics.DrawString("(20 Turns)", new Font("Times New Roman", 16, fontstyle), new SolidBrush(textColor), new Point(_choicePanel.Width - _verticalBar.Width - 1,
                        row * 23), sf);
                }
            }
        }

        // This is so that arrow keys are detected
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Down:
                    if (_city.ItemInProduction < _totalNoUnits + _totalNoImprov - 1) _city.ItemInProduction++;
                    break;
                case Keys.Up:
                    if (_city.ItemInProduction > 0) _city.ItemInProduction--;
                    break;
                case Keys.PageDown:
                    _city.ItemInProduction = Math.Min(_city.ItemInProduction + 16, _totalNoUnits + _totalNoImprov - 1);
                    break;
                case Keys.PageUp:
                    _city.ItemInProduction = Math.Max(_city.ItemInProduction - 16, 0);
                    break;
            }

            // Update relations between chosen value & bar value
            if (_city.ItemInProduction > _barValue + 15) _barValue = _city.ItemInProduction - 15;
            else if (_city.ItemInProduction < _barValue) _barValue = _city.ItemInProduction;
            _verticalBar.Value = _barValue;   // Also update the bar value of control

            _choicePanel.Invalidate();  // Redraw the panel

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void AutoButton_Click(object sender, EventArgs e) { }

        private void HelpButton_Click(object sender, EventArgs e) { }

        private void CheatButton_Click(object sender, EventArgs e) { }

        private void OKButton_Click(object sender, EventArgs e)
        {
            _city.ItemInProduction = _barValue; // TODO: correct selection
            _parent.Invalidate();
            _parent.Enabled = true;
            this.Visible = false;
            this.Dispose();
        }

        // Once slider value changes --> redraw list
        private void VerticalBarValueChanged(object sender, EventArgs e)
        {
            _barValue = _verticalBar.Value;
            _choicePanel.Invalidate();
        }
    }
}
