using System;
using System.Collections.Generic;
using System.Linq;
using Eto.Forms;
using Eto.Drawing;
using Civ2engine;

namespace EtoFormsUI
{
    public sealed class Civ2dialogV2 : Dialog
    {
        public int SelectedIndex;
        public string SelectedButton;
        public List<bool> CheckboxReturnStates;
        public IDictionary<string, string> TextValues = new Dictionary<string, string>();

        
        private readonly int _paddingTop, _paddingBtm;
        private readonly RadioButtonList _radioBtnList;
        private readonly CheckBox[] _checkBox;
        private readonly FormattedText[] _formattedOptionsTexts;

        private readonly List<string> _options;
        private readonly List<string> _text;
        private readonly string _popupTitle;

        private readonly bool _hasCheckBoxes;
        private readonly List<TextStyles> _TextStyles;

        private int _textBoxAlignment;

        /// <summary>
        /// Show a popup box (dialog).
        /// </summary>
        /// <param name="parent">Main window.</param>
        /// <param name="popupBox">Popupbox object read from Game.txt. Determines properties of a popup box.</param>
        /// <param name="replaceStrings">A list of strings to replace %STRING0, %STRING1, %STRING2, etc.</param>
        /// <param name="checkboxOptionState">A list of boolean values representing states of checkbox options.</param>
        public Civ2dialogV2(Main parent, PopupBox popupBox, List<string> replaceStrings = null, IList<bool> checkboxOptionState = null, List<TextBoxDefinition> textBoxes = null)
        {
            if (checkboxOptionState != null) CheckboxReturnStates = new List<bool>(checkboxOptionState); // Initialize return checkbox states

            foreach (var item in parent.Menu.Items) item.Enabled = false;

            _paddingTop = 38;
            _paddingBtm = 46;

            WindowStyle = WindowStyle.None;
            MovableByWindowBackground = true;

            // Replace %STRING in texts
            _popupTitle = ReplaceString(popupBox.Title, replaceStrings);
            if (popupBox.Text != null)
            {
                _text = popupBox.Text.Select(t => ReplaceString(t, replaceStrings)).ToList();
                _TextStyles = popupBox.LineStyles;
            }

            _hasCheckBoxes = popupBox.Checkbox;

            // Determine size of inner panel
            var optionRows = popupBox.Options?.Count ?? 0;
            var textRows = popupBox.Text?.Count ?? 0;
            var innerSize = new Size(2 * 2 + MaxWidth(popupBox, textBoxes), 2 * 2 + optionRows * 32 + textRows * 30);
            Size = new Size(innerSize.Width + 2 * 11, innerSize.Height + _paddingTop + _paddingBtm);
            
            // Center the dialog on screen by default
            Location = new Point(parent.Width / 2 - Width / 2, parent.Height / 2 - Height / 2);

            var layout = new PixelLayout() { Size = new Size(Width, Height) };

            // Options (if they exist) <- either checkbox or radio btn.
            if (popupBox.Options != null)
            {
                _options = popupBox.Options;
                // Texts
                _formattedOptionsTexts = new FormattedText[optionRows];
                for (var i = 0; i < optionRows; i++)
                {
                    _formattedOptionsTexts[i] = new FormattedText()
                    {
                        Font = new Font("Times New Roman", 18),
                        ForegroundBrush = new SolidBrush(Color.FromArgb(51, 51, 51)), Text = _options[i]
                    };
                }

                // Checkboxes
                if (popupBox.Checkbox)
                {
                    _checkBox = new CheckBox[_options.Count];
                    for (var row = 0; row < _options.Count; row++)
                    {
                        _checkBox[row] = new CheckBox
                        {
                            Text = _options[row],
                            Font = new Font("Times New Roman", 18),
                            TextColor = Color.FromArgb(51, 51, 51),
                            BackgroundColor = Colors.Transparent,
                            Checked = checkboxOptionState[row]
                        };
                        _checkBox[row].CheckedChanged += (_, _) => Invalidate();
                        _checkBox[row].GotFocus += (_, _) => Invalidate();
                        layout.Add(_checkBox[row], 11 + 10, 40 + 32 * row);
                    }
                }
                // Radio buttons.
                else
                {
                    _radioBtnList = new RadioButtonList() { DataStore = _options, Orientation = Orientation.Vertical };
                    _radioBtnList.SelectedIndexChanged += (_, _) => Invalidate();
                    _radioBtnList.GotFocus += (_, _) => Invalidate();
                    layout.Add(_radioBtnList, 11 + 10, 40);
                }
            }

            // Drawable surface
            var surface = new Drawable() { Size = new Size(Width, Height), CanFocus = false };
            surface.Paint += Surface_Paint;
            layout.Add(surface, 0, 0);

            // Buttons
            var buttonTitles = popupBox.Button;
            var buttons = new Civ2button[buttonTitles.Count];
            var buttonWidth = (Width - 2 * 9 - 3 * (buttonTitles.Count - 1)) / buttonTitles.Count;
            for (var i = 0; i < buttonTitles.Count; i++)
            {
                var text = buttonTitles[i];
                buttons[i] = new Civ2button(text, buttonWidth, 36, new Font("Times new roman", 11));
                layout.Add(buttons[i], 9 + buttonWidth * i + 3 * i, Height - 46);
                buttons[i].Click += (sender, _) => SelectedButton = ((Civ2button) sender)?.Text;

                switch (text)
                {
                    // Define abort button so that is also called with Esc
                    case "Cancel":
                        AbortButton = buttons[i];
                        AbortButton.Click += (sender, e) =>
                        {
                            foreach (MenuItem item in parent.Menu.Items) item.Enabled = true;
                            SelectedIndex = int.MinValue;
                            if (popupBox.Name == "MAINMENU") 
                            { 
                                Application.Instance.Quit(); 
                            }
                            else
                            {
                                Close();
                            }
                        };
                        break;
                    // Define default button so that it is also called with return key
                    case "OK":
                    {
                        foreach (var item in parent.Menu.Items) item.Enabled = true;

                        DefaultButton = buttons[i];
                        DefaultButton.Click += (_, _) =>
                        {
                            if (popupBox.Checkbox)
                            {
                                for (int row = 0; row < _options.Count; row++)
                                {
                                    CheckboxReturnStates[row] = _checkBox[row].Checked == true;
                                }
                            }
                            else if (_radioBtnList != null) 
                            {
                                SelectedIndex = _radioBtnList.SelectedIndex;
                            }

                            Close(); 
                        };
                        break;
                    }
                    default:
                    {
                        buttons[i].Click += (sender, args) =>
                        {
                            foreach (MenuItem item in parent.Menu.Items) item.Enabled = true;
                            SelectedButton = text;
                            Close();
                        };
                        break;
                    }
                }
            }

            // Update checkbox/choose radiobtn. with mouse click
            surface.MouseDown += (_, e) =>
            {
                // Select radio button if clicked
                if (_options == null) return;

                var yOffset = _text?.Count * 30 ?? 0;

                // Update checkbox
                if (_hasCheckBoxes)
                {
                    for (var row = 0; row < _options.Count; row++)
                    {
                        // Update if checkbox is clicked
                        if (e.Location.X > 14 && e.Location.X < 47 + (int)_formattedOptionsTexts[row].Measure().Width && e.Location.Y > _paddingTop + yOffset + 5 + 32 * row && e.Location.Y < _paddingTop + yOffset + 5 + 32 * (row + 1))
                        {
                            _checkBox[row].Checked = !_checkBox[row].Checked;
                            _checkBox[row].Focus();
                            Invalidate();
                        }
                    }
                }
                // Update radio btn
                else
                {
                    for (var row = 0; row < _options.Count; row++)
                    {
                        if (e.Location.X > 14 && e.Location.X < Width - 14 && e.Location.Y > _paddingTop + yOffset + 5 + 32 * row && e.Location.Y < _paddingTop + yOffset + 5 + 32 * (row + 1))
                        {
                            _radioBtnList.SelectedIndex = row;
                            Invalidate();
                        }
                    }
                }
            };
            
            
            if (textBoxes != null)
            {
                foreach (var textBox in textBoxes)
                {
                    TextValues[textBox.Name] = textBox.InitialValue;
                    var box = new TextBox
                    {
                        Text = textBox.InitialValue,
                        Width = innerSize.Width - 20 - _textBoxAlignment,
                    };
                    if (textBox.MinValue.HasValue)
                    {
                        box.TextChanged += (_, _) =>
                        {
                            var cleaned = new string(box.Text.Where(char.IsNumber).ToArray());
                            if (cleaned.Length == 0)
                            {
                                box.Text = textBox.InitialValue;
                            }else if (!int.TryParse(cleaned, out var result) || !(result >= textBox.MinValue))
                            {
                                box.Text = textBox.MinValue.ToString();
                            }
                            else if(box.Text.Length > cleaned.Length)
                            {
                                box.Text = cleaned;
                            }

                            TextValues[textBox.Name] = box.Text;
                        };
                    }
                    else
                    {
                        box.TextChanged += (_, _) =>
                        {
                            if (box.Text.Length < 0 && textBox.InitialValue.Length > 0)
                            {
                                box.Text = textBox.InitialValue;
                            }

                            TextValues[textBox.Name] = box.Text;
                        };
                    }

                    layout.Add(box, _textBoxAlignment + 10, 40 + 32 * textBox.index);
                }
            }

            Content = layout;
        }


        private void Surface_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.AntiAlias = false;

            // Paint outer wallpaper
            var imgSize = Images.PanelOuterWallpaper.Size;
            for (int row = 0; row < Height / imgSize.Height + 1; row++)
            {
                for (int col = 0; col < Width / imgSize.Width + 1; col++)
                {
                    e.Graphics.DrawImage(Images.PanelOuterWallpaper, col * imgSize.Width, row * imgSize.Height);
                }
            }

            // Paint panel borders
            // Outer border
            using var pen1 = new Pen(Color.FromArgb(227, 227, 227));
            using var pen2 = new Pen(Color.FromArgb(105, 105, 105));
            using var pen3 = new Pen(Color.FromArgb(255, 255, 255));
            using var pen4 = new Pen(Color.FromArgb(160, 160, 160));
            using var pen5 = new Pen(Color.FromArgb(240, 240, 240));
            using var pen6 = new Pen(Color.FromArgb(223, 223, 223));
            using var pen7 = new Pen(Color.FromArgb(67, 67, 67));
            e.Graphics.DrawLine(pen1, 0, 0, Width - 2, 0);   // 1st layer of border
            e.Graphics.DrawLine(pen1, 0, 0, 0, Height - 2);
            e.Graphics.DrawLine(pen2, Width - 1, 0, Width - 1, Height - 1);
            e.Graphics.DrawLine(pen2, 0, Height - 1, Width - 1, Height - 1);
            e.Graphics.DrawLine(pen3, 1, 1, Width - 3, 1);   // 2nd layer of border
            e.Graphics.DrawLine(pen3, 1, 1, 1, Height - 3);
            e.Graphics.DrawLine(pen4, Width - 2, 1, Width - 2, Height - 2);
            e.Graphics.DrawLine(pen4, 1, Height - 2, Width - 2, Height - 2);
            e.Graphics.DrawLine(pen5, 2, 2, Width - 4, 2);   // 3rd layer of border
            e.Graphics.DrawLine(pen5, 2, 2, 2, Height - 4);
            e.Graphics.DrawLine(pen5, Width - 3, 2, Width - 3, Height - 3);
            e.Graphics.DrawLine(pen5, 2, Height - 3, Width - 3, Height - 3);
            e.Graphics.DrawLine(pen6, 3, 3, Width - 5, 3);   // 4th layer of border
            e.Graphics.DrawLine(pen6, 3, 3, 3, Height - 5);
            e.Graphics.DrawLine(pen7, Width - 4, 3, Width - 4, Height - 4);
            e.Graphics.DrawLine(pen7, 3, Height - 4, Width - 4, Height - 4);
            e.Graphics.DrawLine(pen6, 4, 4, Width - 6, 4);   // 5th layer of border
            e.Graphics.DrawLine(pen6, 4, 4, 4, Height - 6);
            e.Graphics.DrawLine(pen7, Width - 5, 4, Width - 5, Height - 5);
            e.Graphics.DrawLine(pen7, 4, Height - 5, Width - 5, Height - 5);

            // Inner border
            e.Graphics.DrawLine(pen7, 9, _paddingTop - 1, 9 + (Width - 18 - 1), _paddingTop - 1);   // 1st layer of border
            e.Graphics.DrawLine(pen7, 10, _paddingTop - 1, 10, Height - _paddingBtm - 1);
            e.Graphics.DrawLine(pen6, Width - 11, _paddingTop - 1, Width - 11, Height - _paddingBtm - 1);
            e.Graphics.DrawLine(pen6, 9, Height - _paddingBtm, Width - 9 - 1, Height - _paddingBtm);
            e.Graphics.DrawLine(pen7, 10, _paddingTop - 2, 9 + (Width - 18 - 2), _paddingTop - 2);   // 2nd layer of border
            e.Graphics.DrawLine(pen7, 9, _paddingTop - 2, 9, Height - _paddingBtm);
            e.Graphics.DrawLine(pen6, Width - 10, _paddingTop - 2, Width - 10, Height - _paddingBtm);
            e.Graphics.DrawLine(pen6, 9, Height - _paddingBtm + 1, Width - 9 - 1, Height - _paddingBtm + 1);

            // Paint inner wallpaper
            imgSize = Images.PanelInnerWallpaper.Size;
            for (var row = 0; row < (Height - _paddingTop - _paddingBtm) / imgSize.Height + 1; row++)
            {
                for (var col = 0; col < (Width - 2 * 11) / imgSize.Width + 1; col++)
                {
                    var rectS = new Rectangle(0, 0, Math.Min(Width - 2 * 11 - col * imgSize.Width, imgSize.Width), Math.Min(Height - _paddingBtm - _paddingTop - row * imgSize.Height, imgSize.Height));
                    e.Graphics.DrawImage(Images.PanelInnerWallpaper, rectS, new Point(col * imgSize.Width + 11, row * imgSize.Height + _paddingTop));
                }
            }

            // Title
            Draw.Text(e.Graphics, _popupTitle, new Font("Times new roman", 17, FontStyle.Bold), Color.FromArgb(135, 135, 135), new Point(Width / 2, _paddingTop / 2), true, true, Colors.Black, 1, 1);

            var yOffset = 0;

            // Centered text
           
            if (_text != null)
            {

                for (var i = 0; i < _text.Count; i++)
                {
                    var centered = _TextStyles[i] == TextStyles.Centered;
                    Draw.Text(e.Graphics, _text[i], new Font("Times new roman", 18),
                        Color.FromArgb(51, 51, 51), 
                        new Point(centered ? Width / 2 : 10, _paddingTop + 5 + yOffset),
                        centered, false,
                        Color.FromArgb(191, 191, 191), 1, 1);

                    yOffset += 30;
                }
            }

            // Options (if they exist) <- either checkbox or radio btn.
            if (_options != null)
            {
                var rowCount = 0;
                foreach (var option in _options)
                {
                    // Draw checkboxes
                    if (_hasCheckBoxes)
                    {
                        Draw.Checkbox(e.Graphics, _checkBox[rowCount].Checked == true, new Point(21, _paddingTop + 9 + yOffset));

                        e.Graphics.DrawText(_formattedOptionsTexts[rowCount], new Point(47, _paddingTop + 5 + yOffset));

                        using var pen = new Pen(Color.FromArgb(64, 64, 64));
                        if (_checkBox[rowCount].HasFocus) e.Graphics.DrawRectangle(pen, new Rectangle(45, _paddingTop + 5 + yOffset, (int)_formattedOptionsTexts[rowCount].Measure().Width, 26));
                    }
                    // Draw radio buttons
                    else
                    {
                        Draw.RadioBtn(e.Graphics, _radioBtnList.SelectedIndex == rowCount, new Point(21, _paddingTop + 9 + yOffset));

                        e.Graphics.DrawText(_formattedOptionsTexts[rowCount], new Point(47, _paddingTop + 5 + yOffset));

                        using var pen = new Pen(Color.FromArgb(64, 64, 64));
                        if (_radioBtnList.SelectedIndex == rowCount) e.Graphics.DrawRectangle(pen, new Rectangle(45, _paddingTop + 5 + yOffset, Width - 45 - 14, 26));
                    }
                    yOffset += 32;
                    rowCount++;
                }
            }
        }

        /// <summary>
        /// Determine max width of a popup box.
        /// </summary>
        private int MaxWidth(PopupBox popupBox, List<TextBoxDefinition> textBoxDefinitions)
        { 
            // 1) Input from Game.txt
            int width = (int)(popupBox.Width * 1.5);

            // 2) Max length of strings
            // First title
            var titleText = new FormattedText { Text = popupBox.Title, Font = new Font("Times new roman", 17, FontStyle.Bold) };
            int titleWidth = (int)titleText.Measure().Width;
            if (titleWidth > width)
            {
                width = titleWidth;
            }
            
            // Then options strings
            foreach (var optionsText in popupBox.Options ?? Enumerable.Empty<string>())
            {
                var textWidthCandidate = (int)(new FormattedText { Text = optionsText, Font = new Font("Times new roman", 18) }.Measure().Width);
                var widthCandidate = textWidthCandidate + 32;   // Count in width of radio button
                if (widthCandidate > width) width = widthCandidate;
            }

            int minTextBox = 0;
            
            // Then other text
            for (int i = 0; i < _text?.Count  ; i++)
            {
                var box = textBoxDefinitions?.FirstOrDefault(b => b.index == i);
                var text = _text[i];
                if (box != null)
                {
                    var textWidthCandidate = (int)(new FormattedText { Text = text, Font = new Font("Times new roman", 18) }.Measure().Width);
                    if (textWidthCandidate > minTextBox)
                    {
                        minTextBox = textWidthCandidate;
                    }
                    var widthCandidate = textWidthCandidate + 50;   // Count in width of text box
                    if (widthCandidate > width) width = widthCandidate;
                }else if (_TextStyles[i] == TextStyles.Centered)
                {
                    var textWidthCandidate = (int)(new FormattedText { Text = text, Font = new Font("Times new roman", 18) }.Measure().Width);
                    if (textWidthCandidate > width) width = textWidthCandidate;              
                }
            }

            _textBoxAlignment = minTextBox;

            return width;
        }


        /// <summary>
        /// Find occurences of %STRING in text and replace it with other strings.
        /// </summary>
        /// <param name="text">Text where replacement takes place.</param>
        /// <param name="replacementStrings">A list of strings to replace %STRING0, %STRING1, %STRING2, etc.</param>
        private static string ReplaceString(string text, List<string> replacementStrings)
        {
            var index = text.IndexOf("%STRING", StringComparison.Ordinal);
            while (index != -1)
            {
                var numericChar = text[index + 7];
                text = text.Replace("%STRING" + numericChar, replacementStrings[(int)char.GetNumericValue(numericChar)]);
                index = text.IndexOf("%STRING", StringComparison.Ordinal);
            }
            return text;
        }
    }
}
