using civ2.Bitmaps;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace civ2.Forms
{
    public class CityBuyPanel : Civ2panel
    {
        private Game Game => Game.Instance;
        private readonly CityPanel _parent;
        private readonly City _city;
        private readonly Civ2radioBtn _completeitButton, _nevermindButton;

        public CityBuyPanel(CityPanel parent, City city) : base(814, 212, "", 38, 46)
        {
            _parent = parent;
            _city = city;

            this.Paint += CityBuyPanel_Paint;

            // Add DrawPanel from base control
            Controls.Add(DrawPanel);
            DrawPanel.Paint += DrawPanel_Paint;

            // OK button
            var _OKButton = new Civ2button
            {
                Location = new Point(9, 170),
                Size = new Size(796, 36),
                Text = "OK"
            };
            Controls.Add(_OKButton);
            _OKButton.Click += OKButton_Click;

            // Radio button 1
            _completeitButton = new Civ2radioBtn
            {
                Text = "Complete it.",
                Location = new Point(140, 69),
            };
            DrawPanel.Controls.Add(_completeitButton);

            // Radio button 2
            _nevermindButton = new Civ2radioBtn
            {
                Text = "Never mind.",
                Location = new Point(140, 98),
            };
            DrawPanel.Controls.Add(_nevermindButton);
            _nevermindButton.Checked = true;
        }

        private void CityBuyPanel_Paint(object sender, PaintEventArgs e)
        {
            // Title
            string text = _city.ItemInProduction < 62 ? Game.Rules.UnitName[_city.ItemInProduction] : Game.Rules.ImprovementName[_city.ItemInProduction - 62 + 1];
            using var _font = new Font("Times New Roman", 17);
            Draw.Text(e.Graphics, "Buy " + text, _font, StringAlignment.Center, StringAlignment.Center, Color.FromArgb(135, 135, 135), new Point(this.Width / 2, 20), Color.Black, 1, 1);
        }

        private void DrawPanel_Paint(object sender, PaintEventArgs e)
        {
            using var _font = new Font("Times New Roman", 18);

            // Draw icons and text
            int itemNo = _city.ItemInProduction;
            // It's a unit
            if (itemNo < 62)
            {
                _completeitButton.Location = new Point(140, 69);
                _nevermindButton.Location = new Point(140, 98);

                using var _unitPic = ModifyImage.Resize(Images.Units[itemNo], 4);
                e.Graphics.DrawImage(_unitPic, 4, 4); // 2-times larger
                int _costToComplete = 10 * Game.Rules.UnitCost[itemNo] - _city.ShieldsProgress;
                string _itemName = Game.Rules.UnitName[itemNo];
                Draw.Text(e.Graphics, $"Cost to complete {_itemName} : {_costToComplete} gold.", _font, StringAlignment.Near, StringAlignment.Near, Color.FromArgb(51, 51, 51), new Point(120, 8));
                Draw.Text(e.Graphics, $"Treasury: {Game.ActiveCiv.Money} gold.", _font, StringAlignment.Near, StringAlignment.Near, Color.FromArgb(51, 51, 51), new Point(120, 35));
            }
            // It's an improvement
            else
            {
                _completeitButton.Location = new Point(85, 69);
                _nevermindButton.Location = new Point(85, 98);

                string _itemName = Game.Rules.ImprovementName[itemNo - 62 + 1];
                using var _improvPic = ModifyImage.Resize(Images.Improvements[itemNo - 62 + 1], 1);
                e.Graphics.DrawImage(_improvPic, 4, 4);
                int _costToComplete = 10 * Game.Rules.ImprovementCost[itemNo - 62 + 1] - _city.ShieldsProgress;
                Draw.Text(e.Graphics, $"Cost to complete {_itemName} : {_costToComplete} gold.", _font, StringAlignment.Near, StringAlignment.Near, Color.FromArgb(51, 51, 51), new Point(80, 8));
                Draw.Text(e.Graphics, $"Treasury: {Game.ActiveCiv.Money} gold.", _font, StringAlignment.Near, StringAlignment.Near, Color.FromArgb(51, 51, 51), new Point(80, 35));
            }
        }

        private void OKButton_Click(object sender, EventArgs e)
        {
            int cost = _city.ItemInProduction < 62 ? Game.Rules.UnitCost[_city.ItemInProduction] : Game.Rules.ImprovementCost[_city.ItemInProduction - 62 + 1];
            Game.ActiveCiv.Money -= 10 * cost - _city.ShieldsProgress;
            _city.ShieldsProgress = 10 * cost;
            _parent.Invalidate();
            _parent.Enabled = true;
            this.Visible = false;
            this.Dispose();
        }
    }
}
