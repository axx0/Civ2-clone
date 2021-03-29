using System.Collections.Generic;
using Eto.Forms;
using Eto.Drawing;

namespace EtoFormsUI
{
    public class CheckboxPanel : Civ2dialog
    {
        protected Civ2button[] Button;
        public CheckBox[] CheckBox;
        private readonly Drawable innerPanel;

        public CheckboxPanel(int width, int height, string title, string[] checkboxNames, string[] buttonTexts) : base(width, height, 38, 46, title)
        {
            // Define formatted texts
            var _font1 = new Font("Times New Roman", 18);
            var _brush1 = new SolidBrush(Color.FromArgb(51, 51, 51));
            FormattedText[] formattedText = new FormattedText[checkboxNames.Length];
            for (int i = 0; i < checkboxNames.Length; i++)
            {
                formattedText[i] = new FormattedText()
                {
                    Font = _font1,
                    ForegroundBrush = _brush1,
                    Text = checkboxNames[i]
                };
            }

            CheckBox = new CheckBox[checkboxNames.Length];
            for (int row = 0; row < checkboxNames.Length; row++)
            {
                CheckBox[row] = new CheckBox() { Text = checkboxNames[row], Font = new Font("Times New Roman", 18), TextColor = Color.FromArgb(51, 51, 51), BackgroundColor = Colors.Transparent };
                CheckBox[row].CheckedChanged += (sender, e) => innerPanel.Invalidate();
                CheckBox[row].GotFocus += (sender, e) => innerPanel.Invalidate();
                Layout.Add(CheckBox[row], 11 + 10, 40 + 32 * row);
            }

            innerPanel = new Drawable() { Size = new Size(width - 2 * 11, height - 38 - 46), BackgroundColor = Colors.Black };
            innerPanel.Paint += (sender, e) =>
            {
                // Background
                var imgSize = Images.PanelInnerWallpaper.Size;
                for (int row = 0; row < this.Height / imgSize.Height + 1; row++)
                {
                    for (int col = 0; col < this.Width / imgSize.Width + 1; col++)
                    {
                        e.Graphics.DrawImage(Images.PanelInnerWallpaper, col * imgSize.Width, row * imgSize.Height);
                    }
                }

                // Draw checkbox, text, text outline
                for (int row = 0; row < checkboxNames.Length; row++)
                {
                    Draw.Checkbox(e.Graphics, CheckBox[row].Checked == true, new Point(9, 5 + 32 * row));

                    e.Graphics.DrawText(formattedText[row], new Point(36, 32 * row + 4));

                    using var _pen = new Pen(Color.FromArgb(64, 64, 64));
                    var textSize = formattedText[row].Measure();
                    if (CheckBox[row].HasFocus) e.Graphics.DrawRectangle(_pen, new Rectangle(36, 32 * row + 4, (int)textSize.Width, (int)textSize.Height));
                }
            };
            innerPanel.MouseDown += (sender, e) =>
            {
                for (int row = 0; row < checkboxNames.Length; row++)
                {
                    // Update if checkbox is clicked
                    if (e.Location.X > 7 && e.Location.X < 33 && e.Location.Y > 3 + 32 * row && e.Location.Y < 28 + 32 * row)
                    {
                        CheckBox[row].Checked = !CheckBox[row].Checked;
                        innerPanel.Invalidate();
                    }

                    // Update if text is clicked
                    if (e.Location.X > 36 && e.Location.X < 36 + (int)formattedText[row].Measure().Width && e.Location.Y > 32 * row + 4 && e.Location.Y < 32 * row + 4 + (int)formattedText[row].Measure().Height)
                    {
                        CheckBox[row].Checked = !CheckBox[row].Checked;
                        CheckBox[row].Focus();
                        innerPanel.Invalidate();
                    }
                }
            };
            Layout.Add(innerPanel, 11, 38);

            // Buttons
            Button = new Civ2button[buttonTexts.Length];
            int btnW = (this.Width - 2 * 9 - 3 * (buttonTexts.Length - 1)) / buttonTexts.Length;  // Determine width of one button
            for (int i = 0; i < buttonTexts.Length; i++)
            {
                Button[i] = new Civ2button(buttonTexts[i], btnW, 36, new Font("Times new roman", 11));
                Layout.Add(Button[i], 9 + btnW * i + 3 * i, Height - 46);
            }

            Content = Layout;
        }
    }
}
