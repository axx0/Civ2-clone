using System.Collections.Generic;
using Eto.Forms;
using Eto.Drawing;

namespace EtoFormsUI
{
    public class CheckboxPanel : Civ2panel
    {
        public Civ2button[] Button;
        public readonly List<Panel> CheckboxFields;
        public bool[] CheckboxState;
        private readonly string[] checkboxNames;

        public CheckboxPanel(int width, int height, string title, string[] _checkboxNames, string[] buttonTexts) : base(width, height, 38, 46, title)
        {
            checkboxNames = _checkboxNames;

            // Buttons
            Button = new Civ2button[buttonTexts.Length];
            int btnW = (this.Width - 2 * 9 - 3 * (buttonTexts.Length - 1)) / buttonTexts.Length;  // Determine width of one button
            for (int i = 0; i < buttonTexts.Length; i++)
            {
                Button[i] = new Civ2button(buttonTexts[i], btnW, 36, new Font("Times new roman", 11));
                MainPanelLayout.Add(Button[i], 9 + btnW * i + 3 * i, Height - 46);
            }

            var InnerPanelLayout = new PixelLayout();
            InnerPanelLayout.Size = new Size(width, height);

            // Make checkbox fields for each option
            CheckboxFields = new List<Panel>();
            for (int i = 0; i < checkboxNames.Length; i++)
            {
                var panel = new Panel
                {
                    BackgroundColor = Colors.Transparent
                };
                InnerPanelLayout.Add(panel, 10, 32 * i + 4);
                panel.MouseDown += CheckboxFields_Click;
                CheckboxFields.Add(panel);
            }

            InnerPanel.Paint += InnerPanel_Paint;
            InnerPanel.Content = InnerPanelLayout;
        }

        private void InnerPanel_Paint(object sender, PaintEventArgs e)
        {
            // Show all options
            using var _font1 = new Font("Times New Roman", 18);
            using var _font2 = new Font("Wingdings", 18);
            using var _brush1 = new SolidBrush(Color.FromArgb(51, 51, 51));
            using var _brush2 = new SolidBrush(Colors.White);
            using var _brush3 = new SolidBrush(Color.FromArgb(128, 128, 128));
            using var _brush4 = new SolidBrush(Colors.Black);
            using var _brush5 = new SolidBrush(Color.FromArgb(192, 192, 192));
            using var _pen = new Pen(Colors.Black);
            for (int row = 0; row < checkboxNames.Length; row++)
            {
                // Text
                var formattedText = new FormattedText()
                {
                    Font = _font1,
                    ForegroundBrush = _brush1,
                    Text = checkboxNames[row]
                };
                var textSize = formattedText.Measure();
                e.Graphics.DrawText(formattedText, new Point(36, 32 * row + 4));
                CheckboxFields[row].Size = new Size(30 + (int)(textSize.Width), (int)textSize.Height);   // Set the correct size of clickable field

                // Draw checkbox
                e.Graphics.FillRectangle(_brush2, new Rectangle(13, 8 + 32 * row, 15, 17));
                e.Graphics.FillRectangle(_brush2, new Rectangle(12, 9 + 32 * row, 17, 15));
                e.Graphics.FillRectangle(_brush3, new Rectangle(14, 9 + 32 * row, 13, 15));
                e.Graphics.FillRectangle(_brush3, new Rectangle(13, 10 + 32 * row, 15, 13));
                e.Graphics.DrawLine(_pen, 14, 9 + 32 * row, 26, 9 + 32 * row);
                e.Graphics.DrawLine(_pen, 14, 25 + 32 * row, 27, 25 + 32 * row);
                e.Graphics.DrawLine(_pen, 13, 10 + 32 * row, 13, 22 + 32 * row);
                e.Graphics.DrawLine(_pen, 29, 10 + 32 * row, 29, 24 + 32 * row);
                e.Graphics.DrawLine(_pen, 13, 10 + 32 * row, 14, 10 + 32 * row);
                e.Graphics.DrawLine(_pen, 28, 24 + 32 * row, 28, 25 + 32 * row);

                // Draw check marks
                if (CheckboxState[row])
                {
                    e.Graphics.DrawText(_font2, _brush4, 10 + 1, 32 * row + 3 + 2, "ü");
                    e.Graphics.DrawText(_font2, _brush5, 10, 32 * row + 3, "ü");
                }
            }
        }

        // When an option is clicked, update the bool array of options
        private void CheckboxFields_Click(object sender, MouseEventArgs e)
        {
            Control clickedControl = (Control)sender;   // Sender gives you which control is clicked.
            int index = CheckboxFields.FindIndex(a => a == clickedControl);    // Which control is clicked in a list
            CheckboxState[index] = !CheckboxState[index];
            InnerPanel.Invalidate();
        }
    }
}
