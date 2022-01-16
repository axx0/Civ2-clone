using Eto.Forms;
using Eto.Drawing;

namespace EtoFormsUI
{
    public class RadiobuttonPanel : Civ2customDialog
    {
        protected Civ2button[] Button;
        protected RadioButtonList RadioBtnList;
        protected Drawable innerPanel;

        public RadiobuttonPanel(Main parent, int width, int height, string title, string[] checkboxNames, string[] buttonTexts) : base(parent, width, height, 38, 46, title)
        {
            RadioBtnList = new RadioButtonList()
            {
                DataStore = checkboxNames,
                Orientation = Orientation.Vertical
            };
            RadioBtnList.SelectedIndexChanged += (sender, e) => innerPanel.Invalidate();
            RadioBtnList.GotFocus += (sender, e) => innerPanel.Invalidate(); 
            Layout.Add(RadioBtnList, 11 + 10, 40);

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

                // Draw radio btn, text, text outline
                for (int row = 0; row < checkboxNames.Length; row++)
                {
                    Draw.RadioBtn(e.Graphics, RadioBtnList.SelectedIndex == row, new Point(10, 9 + 32 * row));

                    e.Graphics.DrawText(formattedText[row], new Point(36, 32 * row + 4));

                    using var _pen = new Pen(Color.FromArgb(64, 64, 64));
                    if (RadioBtnList.SelectedIndex == row) e.Graphics.DrawRectangle(_pen, new Rectangle(34, 5 + 32 * row, innerPanel.Width - 37, 26));
                }
            };
            innerPanel.MouseDown += (sender, e) =>
            {
                for (int row = 0; row < checkboxNames.Length; row++)
                {
                    // Update if row is clicked
                    if (e.Location.X > 7 && e.Location.X < innerPanel.Width - 2 && e.Location.Y > 5 + 32 * row && e.Location.Y < 33 + 32 * row)
                    {
                        RadioBtnList.SelectedIndex = row;
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
