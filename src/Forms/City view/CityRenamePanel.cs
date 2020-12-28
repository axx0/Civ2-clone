using System;
using System.Drawing;
using System.Windows.Forms;

namespace civ2.Forms
{
    public class CityRenamePanel : Civ2panel
    {
        private readonly CityPanel _parent;
        private readonly City _city;
        private readonly TextBox _renameTextBox;

        public CityRenamePanel(CityPanel parent, City city) : base(686, 126, "What Shall We Rename This City?", 38, 46)
        {
            _parent = parent;
            _city = city;

            // Add DrawPanel from base control
            Controls.Add(DrawPanel);
            DrawPanel.Paint += DrawPanel_Paint;

            //Textbox for renaming city
            _renameTextBox = new TextBox
            {
                Location = new Point(163, 2),
                Size = new Size(225, 30),
                Text = _city.Name,
                Font = new Font("Times New Roman", 11)
            };
            DrawPanel.Controls.Add(_renameTextBox);

            // OK button
            var _OKButton = new Civ2button
            {
                Location = new Point(9, 84),
                Size = new Size(333, 36),
                Font = new Font("Times New Roman", 12),
                Text = "OK"
            };
            Controls.Add(_OKButton);
            _OKButton.Click += OKButton_Click;

            // Cancel button
            var _cancelButton = new Civ2button
            {
                Location = new Point(344, 84),
                Size = new Size(333, 36),
                Font = new Font("Times New Roman", 12),
                Text = "Cancel"
            };
            Controls.Add(_cancelButton);
            _cancelButton.Click += CancelButton_Click;
        }

        private void DrawPanel_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;
            e.Graphics.DrawString("New City Name:", new Font("Arial", 10, FontStyle.Bold), new SolidBrush(Color.FromArgb(51, 51, 51)), new Point(3, 10));
        }

        // If OK is pressed --> rename city & close
        private void OKButton_Click(object sender, EventArgs e)
        {
            _city.Name = _renameTextBox.Text;
            _parent.Invalidate();
            _parent.DrawPanel.Invalidate();
            _parent.Enabled = true;
            this.Visible = false;
            this.Dispose();
        }

        // If cancel is pressed --> unfreeze city panel & close
        private void CancelButton_Click(object sender, EventArgs e)
        {
            _parent.Enabled = true;
            this.Visible = false;
            this.Dispose();
        }
    }
}
