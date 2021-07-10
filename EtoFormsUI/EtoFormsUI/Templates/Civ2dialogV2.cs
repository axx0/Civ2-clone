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
        private readonly CheckBox[] _checkBox;
        private readonly FormattedText[] _formattedOptionsTexts;

        private readonly IList<string> _options;
        private readonly IList<string> _text;
        private readonly string _popupTitle;

        private readonly bool _hasCheckBoxes;
        private readonly IList<TextStyles> _textStyles;

        private int _textBoxAlignment;
        private readonly int _optionsColumns;
        private readonly int _optionRows;
        private readonly Drawable _surface;
        private readonly Bitmap[] _icons;
        private readonly Bitmap _image;

        /// <summary>
        /// Show a popup box (dialog).
        /// </summary>
        /// <param name="parent">Main window.</param>
        /// <param name="popupBox">Popupbox object read from Game.txt. Determines properties of a popup box.</param>
        /// <param name="replaceStrings">A list of strings to replace %STRING0, %STRING1, %STRING2, etc.</param>
        /// <param name="checkboxOptionState">A list of boolean values representing states of checkbox options.</param>
        /// <param name="textBoxes">Definitions for any text input on the dialog</param>
        /// <param name="optionsCols">The number of columns to break options into</param>
        /// <param name="icons">Icons to show next to options</param>
        /// <param name="image">Image shown</param>
        public Civ2dialogV2(Main parent, PopupBox popupBox, List<string> replaceStrings = null, IList<bool> checkboxOptionState = null, List<TextBoxDefinition> textBoxes = null, int optionsCols = 1, Bitmap[] icons = null, Bitmap image = null)
        {
            _image = image;
            _icons = icons ?? Array.Empty<Bitmap>();
            _optionsColumns = optionsCols < 1 ? 1 : optionsCols;
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
                _textStyles = popupBox.LineStyles;
            }

            _hasCheckBoxes = popupBox.Checkbox;

            // Determine size of inner panel
            _optionRows = GetOptionsRows(popupBox.Options?.Count, _optionsColumns) ;
            var textRows = popupBox.Text?.Count ?? 0;
            var iconRows = _icons.Sum(i => i.Height);
            var innerSize = new Size(2 * 2 + MaxWidth(popupBox, textBoxes), 2 * 2 + Math.Max((_optionRows - _icons.Length) * 32 + textRows * 30 + iconRows, _image == null ? 0 : _image.Height));
            Size = new Size(innerSize.Width + 2 * 11, innerSize.Height + _paddingTop + _paddingBtm);
            
            // Center the dialog on screen by default
            Location = new Point(parent.Width / 2 - Width / 2, parent.Height / 2 - Height / 2);

            var layout = new PixelLayout() { Size = new Size(Width, Height) };

            // Options (if they exist) <- either checkbox or radio btn.
            if (popupBox.Options != null)
            {
                _options = popupBox.Options.Select(o=>ReplaceString(o, replaceStrings)).ToList();
                // Texts
                _formattedOptionsTexts = _options.Select(option => new FormattedText
                    {
                        Font = new Font("Times New Roman", 18),
                        ForegroundBrush = new SolidBrush(Color.FromArgb(51, 51, 51)), Text = option
                    }
                ).ToArray();

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
                        _checkBox[row].CheckedChanged += (_, _) => _surface.Invalidate();
                        _checkBox[row].GotFocus += (_, _) => _surface.Invalidate();
                        layout.Add(_checkBox[row], 11 + 10, 40 + 32 * row);
                    }
                }
            }

            // Drawable surface
            _surface = new Drawable() { Size = new Size(Width, Height), CanFocus = false };
            _surface.Paint += Surface_Paint;
            layout.Add(_surface, 0, 0);

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
                                for (var row = 0; row < _options.Count; row++)
                                {
                                    CheckboxReturnStates[row] = _checkBox[row].Checked == true;
                                }
                            }

                            Close(); 
                        };
                        break;
                    }
                    default:
                    {
                        buttons[i].Click += (sender, args) =>
                        {
                            foreach (var item in parent.Menu.Items) item.Enabled = true;
                            SelectedButton = text;
                            Close();
                        };
                        break;
                    }
                }
            };
            if (_options != null && !popupBox.Checkbox)
            {
                KeyUp += (sender, args) =>
                {
                    if (args.Key is not (Keys.Up or Keys.Down or Keys.Left or Keys.Right)) return;

                    var newIndex = SelectedIndex;
                    if (optionsCols > 1 && args.Key is Keys.Left or Keys.Right)
                    {
                        if (args.Key is Keys.Right)
                        {
                            newIndex += _optionRows;
                        }
                        else
                        {
                            newIndex -= _optionRows;
                        }
                        if (newIndex < 0)
                        {
                            newIndex += _options.Count;
                        }
                        else if (newIndex >= _options.Count)
                        {
                            newIndex -= _options.Count;
                        }
                    }
                    else
                    {
                        newIndex += (args.Key is Keys.Down or Keys.Right ? 1 : -1);
                        if (newIndex < 0)
                        {
                            newIndex = _options.Count - 1;
                        }
                        else if (newIndex >= _options.Count)
                        {
                            newIndex = 0;
                        }
                    }



                    SelectedIndex = newIndex;
                    buttons.FirstOrDefault(n=>n.Text == "OK")?.Focus();
                    _surface.Invalidate();
                    args.Handled = true;
                };
            }

            // Update checkbox/choose radiobtn. with mouse click
            _surface.MouseDown += (_, e) =>
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
                            _surface.Invalidate();
                        }
                    }
                }
                // Update radio btn
                else
                {
                    var rowHeight = _icons.Length > 0 ? _icons[0].Height : 32;
                    for (var row = 0; row < _optionRows; row++)
                    {
                        if (e.Location.X > 14 && e.Location.X < Width - 14 && e.Location.Y > _paddingTop + yOffset + 5 + rowHeight * row && e.Location.Y < _paddingTop + yOffset + 5 + rowHeight * (row + 1))
                        {
                            var selectedIndex = row;
                            if (_optionsColumns > 1)
                            {
                                var which = Width / _optionsColumns;
                                selectedIndex +=  ((int)e.Location.X / which) * _optionRows;
                            }

                            if (selectedIndex < 0) selectedIndex = 0;
                            else if (selectedIndex > _options.Count) selectedIndex = _options.Count;
                            SelectedIndex = selectedIndex;
                            _surface.Invalidate();
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
                            if(string.IsNullOrWhiteSpace(box.Text) && textBox.InitialValue.Length > 0)
                            {
                                box.Text = textBox.InitialValue;
                            }

                            TextValues[textBox.Name] = box.Text;
                        };
                    }

                    layout.Add(box, _textBoxAlignment + 10, 45 + 30 * textBox.index);
                }
            }

            Content = layout;
        }

        private static int GetOptionsRows(int? optionsCount, int optionsCols)
        {
            if (optionsCount is null or 0) return 0;
            if (optionsCols == 1) return optionsCount.Value;
            if (optionsCount.Value % optionsCols == 0)
            {
                return optionsCount.Value / optionsCols;
            }
            return optionsCount.Value / optionsCols + 1;
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

            // Image
            if (_image != null)
                e.Graphics.DrawImage(_image, new Point(11 + 2, _paddingTop + 2));

            var xOffset = _image == null ? 0 : _image.Width;
            var yOffset = 0;
            
            // Centered text
           
            if (_text != null)
            {
                for (var i = 0; i < _text.Count; i++)
                {
                    var centered = _textStyles != null && (int) _textStyles?.Count > i && _textStyles[i] == TextStyles.Centered;
                    Draw.Text(e.Graphics, _text[i], new Font("Times new roman", 18),
                        Color.FromArgb(51, 51, 51), 
                        new Point(centered ? xOffset + Width / 2 : xOffset + 10, _paddingTop + 5 + yOffset),
                        centered, false,
                        Color.FromArgb(191, 191, 191), 1, 1);

                    yOffset += 30;
                }
            }

            // Options (if they exist) <- either checkbox or radio btn.
            if (_options != null)
            {
                var initialY = yOffset;
                var widthOffset = xOffset + 21;
                var column = 1;
                
                using var pen = new Pen(Color.FromArgb(64, 64, 64));
                for (var rowCount = 0; rowCount < _options.Count; rowCount++)
                {
                    // Draw checkboxes
                    if (_hasCheckBoxes)
                    {
                        Draw.Checkbox(e.Graphics, _checkBox[rowCount].Checked == true,
                            new Point(widthOffset, _paddingTop + 9 + yOffset));

                        e.Graphics.DrawText(_formattedOptionsTexts[rowCount], new Point(widthOffset + 20, _paddingTop + 5 + yOffset));

                        if (_checkBox[rowCount].HasFocus)
                            e.Graphics.DrawRectangle(pen,
                                new Rectangle(45, _paddingTop + 5 + yOffset,
                                    (int) _formattedOptionsTexts[rowCount].Measure().Width, 26));
                    }
                    // Draw radio buttons
                    else
                    {
                        if (rowCount < _icons.Length)
                        {
                            e.Graphics.DrawImage(_icons[rowCount], new Point(widthOffset , _paddingTop + 5 + yOffset));
                            e.Graphics.DrawText(_formattedOptionsTexts[rowCount],
                                new Point(_icons[rowCount].Width + 25, _paddingTop + 5 + yOffset + _icons[rowCount].Height / 2 -18));
                            if (SelectedIndex == rowCount)
                            {
                                e.Graphics.DrawRectangle(pen,
                                    new Rectangle(widthOffset, _paddingTop + yOffset,
                                        _icons[rowCount].Width, _icons[rowCount].Height -5));
                            }

                            yOffset += _icons[rowCount].Height - 32;
                        }
                        else
                        {

                            Draw.RadioBtn(e.Graphics, SelectedIndex == rowCount,
                                new Point(widthOffset, _paddingTop + 9 + yOffset));

                            e.Graphics.DrawText(_formattedOptionsTexts[rowCount],
                                new Point(widthOffset + 20, _paddingTop + 5 + yOffset));

                            if (SelectedIndex == rowCount)
                                e.Graphics.DrawRectangle(pen,
                                    new Rectangle(widthOffset + 20, _paddingTop + 5 + yOffset,
                                        column == _optionsColumns
                                            ? Width / _optionsColumns - 45 - 14
                                            : Width / _optionsColumns - 25, 26));
                        }
                    }

                    if (rowCount % _optionRows == _optionRows -1)
                    {
                        widthOffset += Width / _optionsColumns;
                        yOffset = initialY;
                        column++;
                    }
                    else
                    {
                        yOffset += 32;
                    }
                }
            }
        }

        /// <summary>
        /// Determine max width of a popup box.
        /// </summary>
        private int MaxWidth(PopupBox popupBox, List<TextBoxDefinition> textBoxDefinitions)
        {
            var imageWidth = _image == null ? 0 : _image.Width;
            
            // 1) Width from Game.txt
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
            var rows = GetOptionsRows(popupBox.Options?.Count, _optionsColumns);
            if (rows > 0)
            {
                for (var index = 0; index < rows; index++)
                {
                    var textWidthCandidate = Enumerable.Range(0, _optionsColumns).Select(n => n * rows + index)
                        .Where(n => n < popupBox.Options.Count).Select(n => (int) (new FormattedText
                            {Text = popupBox.Options[n], Font = new Font("Times new roman", 18)}.Measure().Width))
                        .Sum();
                    var widthCandidate = textWidthCandidate + imageWidth + (_icons.Length > index ? _icons[index].Width : 40 * _optionsColumns); // Count in width of radio button
                    if (widthCandidate > width) width = widthCandidate;
                }
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
                    var widthCandidate = textWidthCandidate + imageWidth + 50;   // Count in width of text box
                    if (widthCandidate > width) width = widthCandidate + imageWidth;
                }else if ((_textStyles?.Count ?? 0 )> i && _textStyles[i] == TextStyles.Centered)
                {
                    var textWidthCandidate = (int)(new FormattedText { Text = text, Font = new Font("Times new roman", 18) }.Measure().Width) + imageWidth;
                    if (textWidthCandidate > width) width = textWidthCandidate + imageWidth; 
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
            return Replace(Replace(text, replacementStrings, "%STRING"), replacementStrings, "%NUMBER");
        }

        private static string Replace(string text, List<string> replacementStrings, string replacementKey)
        {
            var index = text.IndexOf(replacementKey, StringComparison.Ordinal);
            while (index != -1)
            {
                var numericChar = text[index + 7];
                text = text.Replace(replacementKey + numericChar, replacementStrings[(int) char.GetNumericValue(numericChar)]);
                index = text.IndexOf(replacementKey, StringComparison.Ordinal);
            }

            return text;
        }
    }
}
