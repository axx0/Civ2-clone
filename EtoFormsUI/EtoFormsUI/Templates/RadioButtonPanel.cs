using Eto.Forms;
using Eto.Drawing;

namespace EtoFormsUI
{
    public class RadioButtonPanel : Civ2panel
    {
        public Civ2button[] Button;
        public RadioButton[] RadioBtn;

        public RadioButtonPanel(int width, int height, string title, string[] choiceNames, string[] buttonTexts) : base(width, height, 38, 46, title)
        {
            // Buttons
            Button = new Civ2button[buttonTexts.Length];
            int btnW = (this.Width - 2 * 9 - 3 * (buttonTexts.Length - 1)) / buttonTexts.Length;  // Determine width of one button
            for (int i = 0; i < buttonTexts.Length; i++)
            {
                Button[i] = new Civ2button(buttonTexts[i], btnW, 36, new Font("Times new roman", 11));
                MainPanelLayout.Add(Button[i], 9 + btnW * i + 3 * i, Height - 46);
            }

            // Radio buttons
            var layout2 = new PixelLayout();
            var controller = new RadioButton();
            RadioBtn = new RadioButton[choiceNames.Length];
            for (int row = 0; row < choiceNames.Length; row++)
            {
                RadioBtn[row] = new RadioButton(controller) { Text = choiceNames[row], Font = new Font("Times new roman", 18), Size = new Size(InnerPanel.Width, 33), TextColor = Color.FromArgb(51, 51, 51) };
                layout2.Add(RadioBtn[row], 10, row * 33);
            }
            layout2.Size = new Size(InnerPanel.Width, InnerPanel.Height);
            InnerPanel.Content = layout2;

            MainPanel.Content = MainPanelLayout;
        }
    }
}
