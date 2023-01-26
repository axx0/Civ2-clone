using System;
using System.Collections.Generic;
using System.Linq;
using Eto.Forms;
using Eto.Drawing;
using Civ2engine;
using System.Diagnostics;
using EtoFormsUI.Menu;
using Model;
using Model.Interface;

namespace EtoFormsUI
{
    public sealed class Civ2dialog : Dialog
    {
        public int SelectedIndex;
        public string SelectedButton;
        public List<bool> CheckboxReturnStates;
        public IDictionary<string, string> TextValues = new Dictionary<string, string>();

        private readonly int _paddingTop, _paddingBtm;
        private readonly FormattedText _fTitle;
        private readonly IList<FormattedText> _fTexts;
        private readonly FormattedText[] _formattedOptionsTexts, _formattedTextboxTexts;
        private readonly CheckBox[] _checkBox;
        private readonly ListboxDefinition _listbox;
        private readonly IList<string> _options, _text;
        private readonly VScrollBar _listboxBar;

        private readonly bool _hasCheckBoxes;

        private bool dragging;
        private PointF dragCursorPoint, dragFormPoint, dif;
        private readonly int _optionsColumns, _optionsRows;
        private readonly Drawable _surface, _listboxSurface;
        private readonly int _listboxShownLines, _listboxHeight;
        private int _listboxShownLine0;
        private readonly Bitmap[] _icons;
        private readonly Bitmap _image;
        private readonly Size _innerSize;
        private readonly int _textHeight;

        /// <summary>
        /// Show a popup box (dialog).
        /// </summary>
        /// <param name="parent">Main window.</param>
        /// <param name="popupBox">Popupbox object read from Game.txt. Determines properties of a popup box.</param>
        /// <param name="replaceStrings">A list of strings to replace %STRING0, %STRING1, %STRING2, etc.</param>
        /// <param name="replaceNumbers">A list of numbers to replace %NUMBER0, %NUMBER1, %NUMBER2, etc.</param>
        /// <param name="checkboxOptionState">A list of boolean values representing states of checkbox options.</param>
        /// <param name="textBoxes">Definitions for any text input on the dialog</param>
        /// <param name="optionsCols">The number of columns to break options into</param>
        /// <param name="icons">Icons to show next to options</param>
        /// <param name="image">Image shown</param>
        /// <param name="listbox">Listbox shown</param>
        public Civ2dialog(Main parent, PopupBox popupBox, IList<string> replaceStrings = null, IList<int> replaceNumbers = null, IList<bool> checkboxOptionState = null, List<TextBoxDefinition> textBoxes = null, int optionsCols = 1, Bitmap[] icons = null, Bitmap image = null, ListboxDefinition listbox = null)
        {
            foreach (var item in parent.Menu.Items) item.Enabled = false;

            _paddingTop = 38;
            _paddingBtm = 46;

            WindowStyle = WindowStyle.None;
            Interface = parent.ActiveInterface.Look;

            // Drag window
            this.MouseDown += (_, e) =>
            {
                if (e.Location.Y < _paddingTop)  // Enable dragging only on top of window
                {
                    dragging = true;
                    dragCursorPoint = this.Location + e.Location;
                    dragFormPoint = this.Location;
                }
            };

            this.MouseMove += (_, e) =>
            {
                if (dragging)
                {
                    dif = this.Location + e.Location - dragCursorPoint;
                    this.Location = (Point)(dragFormPoint + dif);
                }
            };

            this.MouseUp += (_, _) => dragging = false;

            var layout = new PixelLayout() { Size = new Size(Width, Height) };

            if (popupBox.Text is not null)
            {
                _text = popupBox.Text.ToList();
            }
            _listbox = listbox;
            _image = image;
            _icons = icons ?? Array.Empty<Bitmap>();
            _optionsColumns = optionsCols < 1 ? 1 : optionsCols;
            if (checkboxOptionState is not null) CheckboxReturnStates = new List<bool>(checkboxOptionState); // Initialize return checkbox states

            // Fill text into textboxes & then remove it from popubox Text
            if (textBoxes is not null)
            {
                if (popupBox.Options?.Count > 0)
                {
                    SetTextBoxText(textBoxes, popupBox.Options);
                    popupBox.Options = null;
                }
                else
                {
                    SetTextBoxText(textBoxes, _text);
                    _text = _text.Take(textBoxes[0].index - 1).ToList();
                }

            }

            // Format title & adjust inner panel width to fit the title
            if (!string.IsNullOrWhiteSpace(popupBox.Title))
            {
                _fTitle = GetFormattedTitle(popupBox.Title, replaceStrings, replaceNumbers);
                _innerSize.Width = (int)_fTitle.Measure().Width - 2 * 11;
            }

            // Determine size of text and based on that determine inner panel size
            _textHeight = 0;
            if (_text?.Count > 0)
            {
                _fTexts = GetFormattedTexts(_text, popupBox.LineStyles.ToList(), replaceStrings, replaceNumbers);
                _innerSize.Width = Math.Max(_innerSize.Width, GetInnerPanelWidthFromText(_fTexts, popupBox));
                foreach (var fText in _fTexts)
                {
                    fText.MaximumWidth = _innerSize.Width; // Adjust text width to inner panel width
                }

                var totalHeight = _fTexts.Sum(fText => fText.Measure().Height / 28.0) * 28;
                _textHeight = (int)Math.Round(totalHeight);
                foreach (var fText in _fTexts)
                {
                    _innerSize.Height += (int)Math.Round(fText.Measure().Height / 28.0) * 28;
                }
            }

            // Correction of inner panel size for options
            _optionsRows = GetOptionsRows(popupBox.Options?.Count, _optionsColumns);
            var iconsHeight = _icons.Length == 0 ? 0 : _icons.Sum(i => i.Height) + 4 * (_icons.Length - 1);
            _innerSize = new Size(Math.Max(_innerSize.Width, GetInnerPanelWidthFromOptions(popupBox, _optionsRows, _optionsColumns, _icons, textBoxes)), 
                (_optionsRows - _icons.Length) * 32 + iconsHeight + _textHeight);

            // Correction of inner panel size for image
            _innerSize = new Size(Math.Max(_innerSize.Width, _image?.Width ?? 0), Math.Max(_innerSize.Height, _image?.Height ?? 0));

            // Correction of inner panel size for textbox
            _innerSize = new Size(_innerSize.Width, _innerSize.Height + (30 * textBoxes?.Count ?? 0));

            _listboxShownLines = popupBox.ListboxLines;
            _listboxHeight = _listboxShownLines * 23 + 2;
            _innerSize.Height += _listboxHeight;
            Size = new Size(_innerSize.Width + 2 * 2 + 2 * 11, _innerSize.Height + 2 * 2 + _paddingTop + _paddingBtm);

            // Dialog location on screen (center by default)
            int locX, locY;
            if (popupBox.X is not null && popupBox.X < 0)
                locX = (int)(popupBox.X.HasValue ? popupBox.X + (int)Screen.WorkingArea.Width - Width : parent.Width / 2 - Width / 2);
            else
                locX = (int)(popupBox.X.HasValue ? popupBox.X : parent.Width / 2 - Width / 2);
            if (popupBox.Y is not null && popupBox.Y < 0)
                locY = (int)(popupBox.Y.HasValue ? popupBox.Y + (int)Screen.WorkingArea.Height - Height : parent.Height / 2 - Height / 2);
            else
                locY = (int)(popupBox.Y.HasValue ? popupBox.Y : parent.Height / 2 - Height / 2);
            Location = new Point(locX, locY);

            // Options (if they exist) <- either checkbox or radio btn.
            _hasCheckBoxes = popupBox.Checkbox;
            if (popupBox.Options is not null)
            {
                // Texts
                _options = popupBox.Options.Select(o => Replace(o, replaceStrings, replaceNumbers)).ToList();
                _formattedOptionsTexts = _options.Select(option => new FormattedText
                {
                    Font = new Font("Times New Roman", 18),
                    ForegroundBrush = new SolidBrush(Color.FromArgb(51, 51, 51)),
                    Text = option
                }
                ).ToArray();

                // Checkboxes
                if (_hasCheckBoxes)
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
            layout.Size = Size;
            layout.Add(_surface, 0, 0);

            if (popupBox.Listbox)
            {
                _listboxSurface = new Drawable() { Size = new Size(_surface.Width - 2 * 13, _listboxHeight), BackgroundColor = Color.FromArgb(207, 207, 207) };
                _listboxSurface.Paint += ListboxSurface_Paint;
                layout.Add(_listboxSurface, 13, _paddingTop + _innerSize.Height - _listboxHeight + 2);

                if (_listbox.LeftText.Count > popupBox.ListboxLines)
                {
                    _listboxBar = new VScrollBar() { Height = _listboxSurface.Height, Value = 0, Maximum = _listbox.LeftText.Count };
                    _listboxBar.ValueChanged += (_, _) =>
                    {
                        _listboxShownLine0 = _listboxBar.Value;
                        _listboxSurface.Invalidate();
                    };
                    layout.Add(_listboxBar, 13 + _listboxSurface.Width - 10, _paddingTop + _innerSize.Height - _listboxHeight + 2);
                }
                _listboxShownLine0 = _listboxBar is null ? 0 : _listboxBar.Value;
            }

            // Insert textboxes
            if (textBoxes is not null)
            {
                // Texts next to textbox
                _formattedTextboxTexts = textBoxes.Select(box => new FormattedText
                {
                    Font = new Font("Arial", 10, FontStyle.Bold),
                    ForegroundBrush = new SolidBrush(Color.FromArgb(51, 51, 51)),
                    Text = box.Text
                }
                ).ToArray();
                var maxWidth = (int)_formattedTextboxTexts.Max(box => box.Measure().Width);

                var i = 0;
                foreach (var textBox in textBoxes)
                {
                    TextValues[textBox.Name] = textBox.InitialValue;
                    var box = new TextBox
                    {
                        Font = new Font("Times new roman", 12),
                        Text = textBox.InitialValue,
                        //Width = _innerSize.Width - 20 - _textBoxAlignment,
                        Width = textBox.Width,
                        Height = 30
                    };

                    if (textBox.MinValue.HasValue)
                    {
                        box.TextChanged += (_, _) =>
                        {
                            var cleaned = new string(box.Text.Where(char.IsNumber).ToArray());
                            if (cleaned.Length == 0)
                            {
                                box.Text = textBox.InitialValue;
                            }
                            else if (!int.TryParse(cleaned, out var result) || !(result >= textBox.MinValue))
                            {
                                box.Text = textBox.MinValue.ToString();
                            }
                            else if (box.Text.Length > cleaned.Length)
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
                            if (string.IsNullOrWhiteSpace(box.Text) && textBox.InitialValue.Length > 0)
                            {
                                box.Text = textBox.InitialValue;
                            }

                            TextValues[textBox.Name] = box.Text;
                        };
                    }

                    layout.Add(box, 13 + maxWidth + 10, _paddingTop + 4 + _innerSize.Height - 30 * textBoxes.Count() + 30 * i);
                    i++;
                }
            }

            // Buttons
            var buttonTitles = popupBox.Button;
            var buttons = new Civ2button[buttonTitles.Count];
            var buttonWidth = (Width - 2 * 9 - 3 * (buttonTitles.Count - 1)) / buttonTitles.Count;
            for (var i = 0; i < buttonTitles.Count; i++)
            {
                var text = buttonTitles[i];
                buttons[i] = new Civ2button(text, buttonWidth, 36, new Font("Times new roman", 11));
                layout.Add(buttons[i], 9 + buttonWidth * i + 3 * i, Height - 42);
                buttons[i].Click += (sender, _) => SelectedButton = ((Civ2button)sender)?.Text;

                switch (text)
                {
                    // Define abort button so that is also called with Esc
                    case "Cancel":
                        AbortButton = buttons[i];
                        AbortButton.Click += (_, _) =>
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
                                if (_hasCheckBoxes)
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

            if (_options is not null && !_hasCheckBoxes)
            {
                KeyUp += (sender, args) =>
                {
                    if (args.Key is not (Keys.Up or Keys.Down or Keys.Left or Keys.Right)) return;

                    var newIndex = SelectedIndex;
                    if (optionsCols > 1 && args.Key is Keys.Left or Keys.Right)
                    {
                        if (args.Key is Keys.Right)
                        {
                            newIndex += _optionsRows;
                        }
                        else
                        {
                            newIndex -= _optionsRows;
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
                    buttons.FirstOrDefault(n => n.Text == "OK")?.Focus();
                    _surface.Invalidate();
                    args.Handled = true;
                };
            }

            if (_listbox is not null)
            {
                KeyDown += (sender, args) =>
                {
                    if (args.Key is not (Keys.Up or Keys.Down or Keys.Left or Keys.Right)) return;

                    var newIndex = SelectedIndex;
                    newIndex += (args.Key is Keys.Down or Keys.Right ? 1 : -1);
                    if (newIndex < 0)
                    {
                        newIndex = listbox.LeftText.Count - 1;
                    }
                    else if (newIndex >= listbox.LeftText.Count)
                    {
                        newIndex = 0;
                    }

                    SelectedIndex = newIndex;
                    if (SelectedIndex < _listboxShownLine0)
                    {
                        _listboxShownLine0 = SelectedIndex;
                        _listboxBar.Value = SelectedIndex;
                    }
                    else if (SelectedIndex > _listboxShownLine0 + _listboxShownLines - 1)
                    {
                        _listboxShownLine0 = SelectedIndex - _listboxShownLines + 1;
                        _listboxBar.Value = _listboxShownLine0;
                    }
                    _listboxSurface.Invalidate();
                    args.Handled = true;
                };
            }

            // Update checkbox/choose radiobtn. with mouse click
            _surface.MouseDown += (_, e) =>
            {
                // Select radio button if clicked
                if (_options is null) return;

                var yOffset = _textHeight;

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
                    for (var row = 0; row < _optionsRows; row++)
                    {
                        if (e.Location.X > 14 && e.Location.X < Width - 14 &&
                            e.Location.Y > _paddingTop + yOffset + 5 + rowHeight * row &&
                            e.Location.Y < _paddingTop + yOffset + 5 + rowHeight * (row + 1))
                        {
                            var selectedIndex = row;
                            if (_optionsColumns > 1)
                            {
                                var which = Width / _optionsColumns;
                                selectedIndex += ((int)e.Location.X / which) * _optionsRows;
                            }

                            if (selectedIndex < 0) selectedIndex = 0;
                            else if (selectedIndex > _options.Count) selectedIndex = _options.Count;
                            SelectedIndex = selectedIndex;
                            _surface.Invalidate();
                        }
                    }
                }
            };

            Content = layout;
        }

        public InterfaceStyle Interface { get; set; }

        private void SetTextBoxText(List<TextBoxDefinition> textBoxes, IList<string> text)
        {
            foreach (var textBox in textBoxes)
                textBox.Text = text[textBox.index];
        }
    
        private FormattedText GetFormattedTitle(string title, IList<string> replaceStrings, IList<int> replaceNumbers)
        {
            var fTitle = new FormattedText
            {
                Text = Replace(title, replaceStrings, replaceNumbers),
                Font = new Font("Times new roman", 18, FontStyle.Bold),
                Alignment = FormattedTextAlignment.Center
            };
            return fTitle;
        }

        private static List<FormattedText> GetFormattedTexts(IList<string> texts, IList<TextStyles> styles, IList<string> replaceStrings, IList<int> replaceNumbers)
        {
            // Group left-aligned texts
            int j = 0;
            while (j < texts.Count - 1)
            {
                j++;
                if (styles[j - 1] == TextStyles.Left && styles[j] == TextStyles.Left)
                {
                    texts[j] = texts[j - 1] + " " + texts[j];
                    texts.RemoveAt(j - 1);
                    styles.RemoveAt(j - 1);
                    j = 0;
                }
            }

            // Replace %STRING, %NUMBER
            texts = texts.Select(t => Replace(t, replaceStrings, replaceNumbers)).ToList();
            texts = texts.Select(t => t.Replace("_", " ")).ToList();

            // Format texts
            var formattedTexts = new List<FormattedText>();
            int i = 0;
            foreach (var text in texts)
            {
                formattedTexts.Add(new FormattedText
                {
                    Text = (text == "" && styles[i] == TextStyles.LeftOwnLine) ? " " : text,    // Add space if ^ is the only character
                    Font = new Font("Times New Roman", 18),
                    Alignment = styles[i] == TextStyles.Centered ? FormattedTextAlignment.Center : FormattedTextAlignment.Left
                });

                i++;
            }
            return formattedTexts;
        }

        private static int GetInnerPanelWidthFromText(IList<FormattedText> fTexts, PopupBox popupbox)
        {
            var centredTextMaxWidth = 0.0;
            if (fTexts.Where(t => t.Alignment == FormattedTextAlignment.Center).Any())
                centredTextMaxWidth = (from text in fTexts
                                       where text.Alignment == FormattedTextAlignment.Center
                                       orderby text.Measure().Width descending
                                       select text).ToList().FirstOrDefault().Measure().Width;

            if (popupbox.Width != 0)
                return (int)Math.Ceiling(Math.Max(centredTextMaxWidth, popupbox.Width * 1.5));
            else
                return (int)Math.Ceiling(Math.Max(centredTextMaxWidth, 660.0));    // 660=440*1.5
        }


        /// <summary>
        /// Determine max width of a popup box from options.
        /// </summary>
        private static int GetInnerPanelWidthFromOptions(PopupBox popupBox, int optionsRows, int optionsColumns, Bitmap[] icons, List<TextBoxDefinition> textBoxDefinitions)
        {
            int width = 0;
            if (optionsRows > 0)
            {
                for (var index = 0; index < optionsRows; index++)
                {
                    var textWidthCandidate = Enumerable.Range(0, optionsColumns).Select(n => n * optionsRows + index)
                        .Where(n => n < popupBox.Options.Count).Select(n => (int)new FormattedText
                        { Text = popupBox.Options[n], Font = new Font("Times new roman", 18) }.Measure().Width)
                        .Sum();
                    //var widthCandidate = textWidthCandidate + imageWidth + (_icons.Length > index ? _icons[index].Width : 40 * optionsColumns); // Count in width of radio button
                    var widthCandidate = textWidthCandidate + (icons.Length > index ? icons[index].Width : 40 * optionsColumns); // Count in width of radio button
                    if (widthCandidate > width) width = widthCandidate;
                }
            }

            if (popupBox.Width != 0)
                return (int)Math.Ceiling(Math.Max(width, popupBox.Width * 1.5));
            else
                return (int)Math.Ceiling(Math.Max(width, 660.0));    // 660=440*1.5
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
            InterfaceUtils.DrawOuterWallpaper(e.Graphics,Height, Width);

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
            //e.Graphics.DrawLine(pen7, 4, Height - 5, Width - 5, Height - 5);

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
            InterfaceUtils.DrawInnerWallpaper(e.Graphics,Height, Width,_paddingTop,_paddingBtm,11);

            // Title
            if (_fTitle is not null)
            {
                for (int i = 0; i < 2; i++) // Draw front + shadow
                {
                    _fTitle.ForegroundBrush =
                        (i == 0) ? new SolidBrush(Colors.Black) : new SolidBrush(Color.FromArgb(135, 135, 135));
                    _fTitle.MaximumWidth = _innerSize.Width + 2 * 2 + 2 * 11;
                    e.Graphics.DrawText(_fTitle,
                        new Point(1 - i, (int)(_paddingTop / 2 - _fTitle.Measure().Height / 2) + 1 - i));
                }
            }

            // Image
            if (_image is not null) e.Graphics.DrawImage(_image, new Point(11 + 2, _paddingTop + 2));

            var xOffset = _image == null ? 0 : _image.Width;
            var yOffset = _textHeight;

            // Text
            if (_fTexts is not null)
            {
                var h = 0;
                foreach (var fText in _fTexts)
                {
                    for (int i = 0; i < 2; i++) // Draw front + shadow
                    {
                        fText.ForegroundBrush = (i == 0) ? new SolidBrush(Color.FromArgb(191, 191, 191)) : new SolidBrush(Color.FromArgb(51, 51, 51));
                        e.Graphics.DrawText(fText, new Point(11 + 2 + 1 - i, _paddingTop + 2 + 1 - i + h));
                    }
                    h += (int)fText.Measure().Height;
                }
            }

            // Options (if they exist) <- either checkbox or radio btn.
            if (_options is not null)
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
                            new Point(widthOffset - 2, _paddingTop + 4 + yOffset));

                        e.Graphics.DrawText(_formattedOptionsTexts[rowCount], new Point(widthOffset + 24, _paddingTop + 2 + yOffset));

                        if (_checkBox[rowCount].HasFocus)
                            e.Graphics.DrawRectangle(pen,
                                new Rectangle(45, _paddingTop + 2 + yOffset,
                                    (int)_formattedOptionsTexts[rowCount].Measure().Width, 26));
                    }
                    // Draw radio buttons/icons
                    else
                    {
                        // Draw icons
                        if (rowCount < _icons.Length)
                        {
                            e.Graphics.DrawImage(_icons[rowCount], new Point(13, _paddingTop + 2 + yOffset));
                            
                            for (int i = 0; i < 2; i++) // Draw front + shadow text
                            {
                                _formattedOptionsTexts[rowCount].ForegroundBrush = (i == 0) ? new SolidBrush(Color.FromArgb(191, 191, 191)) : new SolidBrush(Color.FromArgb(51, 51, 51));
                                e.Graphics.DrawText(_formattedOptionsTexts[rowCount],
                                    new Point(_icons[rowCount].Width + 15 + 1 - i, _paddingTop + 12 + yOffset + 1 - i));
                            }
                            if (SelectedIndex == rowCount)
                            {
                                e.Graphics.DrawRectangle(new Pen(Color.FromArgb(223, 223, 223)),
                                    new Rectangle(12, _paddingTop + 1 + yOffset,
                                        _icons[rowCount].Width, _icons[rowCount].Height));
                            }

                            yOffset += _icons[rowCount].Height + 4 - 32;
                        }
                        // Draw radio buttons
                        else
                        {
                            Draw.RadioBtn(e.Graphics, SelectedIndex == rowCount,
                                new Point(widthOffset, _paddingTop + 9 + yOffset));

                            e.Graphics.DrawText(_formattedOptionsTexts[rowCount],
                                new Point(widthOffset + 26, _paddingTop + 5 + yOffset));

                            if (SelectedIndex == rowCount)
                                e.Graphics.DrawRectangle(pen,
                                    new Rectangle(widthOffset + 24, _paddingTop + 5 + yOffset,
                                        column == _optionsColumns
                                            ? Width / _optionsColumns - 45 - 14
                                            : Width / _optionsColumns - 25, 26));
                        }
                    }

                    if (rowCount % _optionsRows == _optionsRows - 1)
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

            // Textboxes text
            if (_formattedTextboxTexts is not null)
            {
                for (int i = 0; i < _formattedTextboxTexts.Length; i++)
                {
                    e.Graphics.DrawText(_formattedTextboxTexts[i],
                        new Point(13, _paddingTop + 9 + _innerSize.Height - 30 * _formattedTextboxTexts.Length + i * 30));
                }
            }
        }

        private void ListboxSurface_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.AntiAlias = false;

            e.Graphics.DrawRectangle(Color.FromArgb(67, 67, 67), new Rectangle(0, 0, _listboxSurface.Width - 1, _listboxSurface.Height - 1));

            var rowmax = _listboxBar is null ? Math.Min(_listboxShownLines, _listbox.LeftText.Count) : Math.Min(_listboxShownLines, _listbox.LeftText.Count - _listboxShownLine0);
            var hasIcons = _listbox.Icons is { Count: > 0 };
            var offsetX = hasIcons ? 76 : 1;
            for (int row = 0; row < rowmax; row++)
            {
                if (_listboxShownLine0 + row == SelectedIndex)
                {
                    e.Graphics.FillRectangle(Color.FromArgb(107, 107, 107), offsetX + 1, 2 + 23 * row, _listboxSurface.Width - offsetX - 3, 21);
                    e.Graphics.DrawLine(Color.FromArgb(223, 223, 223), offsetX, 1 + 23 * row, offsetX, 1 + 23 * row + 21);
                    e.Graphics.DrawLine(Color.FromArgb(223, 223, 223), offsetX, 1 + 23 * row, _listboxSurface.Width - 3, 1 + 23 * row);
                    e.Graphics.DrawLine(Color.FromArgb(67, 67, 67), _listboxSurface.Width - 2, 1 + 23 * row, _listboxSurface.Width - 2, 1 + 23 * row + 21);
                    e.Graphics.DrawLine(Color.FromArgb(67, 67, 67), offsetX, 23 * (row + 1), _listboxSurface.Width - 2, 23 * (row + 1));
                    Draw.Text(e.Graphics, _listbox.LeftText[SelectedIndex], new Font("Times new Roman", 15, FontStyle.Bold), Colors.White, new Point(offsetX + 2, row * 23), false, false, Colors.Black, 1, 1);
                }
                else
                {
                    Draw.Text(e.Graphics, _listbox.LeftText[_listboxShownLine0 + row], new Font("Times new Roman", 15), Colors.Black, new Point(offsetX + 2, row * 23 + 2), false, false);
                }

                if (hasIcons)
                {
                    e.Graphics.DrawImage(_listbox.Icons[row], 1 + (_listboxShownLine0 + row) % 2 * 38, 2 + 23 * row);
                }
            }                
        }

        /// <summary>
        /// Find occurences of %STRING and %NUMBER in text and replace it with other strings/numbers.
        /// </summary>
        /// <param name="text">Text where replacement takes place.</param>
        /// <param name="replacementStrings">A list of strings to replace %STRING0, %STRING1, %STRING2, etc.</param>
        /// <param name="replacementNumbers">A list of integers to replace %NUMBER0, %NUMBER1, %NUMBER2, etc.</param>
        private static string Replace(string text, IList<string> replacementStrings, IList<int> replacementNumbers)
        {
            var index = text.IndexOf("%STRING", StringComparison.Ordinal);
            while (index != -1)
            {
                var numericChar = text[index + 7];
                text = text.Replace("%STRING" + numericChar, replacementStrings[(int)char.GetNumericValue(numericChar)]);
                index = text.IndexOf("%STRING", StringComparison.Ordinal);
            }

            index = text.IndexOf("%NUMBER", StringComparison.Ordinal);
            while (index != -1)
            {
                var numericChar = text[index + 7];
                text = text.Replace("%NUMBER" + numericChar, replacementNumbers[(int)char.GetNumericValue(numericChar)].ToString());
                index = text.IndexOf("%NUMBER", StringComparison.Ordinal);
            }

            return text;
        }
    }
}
