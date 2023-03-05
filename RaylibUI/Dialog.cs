using Civ2engine;
using Model;
using Model.Interface;
using Raylib_cs;
using System.Numerics;

namespace RaylibUI;

public class Dialog
{
    private int _selectedRadio = 0;
    //public IDictionary<string, string> TextboxValues = new Dictionary<string, string>();

    private readonly Action<string, int, IDictionary<string,string>?>[] _buttonHandlers;
    private readonly IList<string> _text;
    private readonly string _title;
    private readonly IList<FormattedText> _fTexts;
    private readonly FormattedText _fTitle;
    private readonly Size _size, _innerSize;
    private readonly Padding _padding;
    private readonly IList<string> _options;
    private readonly List<Textbox> _textBoxes;
    private readonly FormattedText[] _formattedTextboxTexts;
    private readonly IList<string> _buttonTexts;
    private readonly int _textHeight, _optionsColumns, _optionsRows, _btnWidth;
    private readonly Image[] _icons;
    private readonly Image _image;
    private readonly Button[] _buttons;
    private int _dialogPosX, _dialogPosY;
    private bool dragging = false;

    public Dialog(PopupBox popupBox, Point relatDialogPos, Action<string, int, IDictionary<string, string>?>[] buttonHandlers, IList<string> replaceStrings = null, IList<int> replaceNumbers = null, List<TextBoxDefinition>? textBoxDefs = null, int optionsCols = 1, Image[] icons = null, Image image = new Image())
    {

        _padding = new Padding(11, 11, 38, 46);
        _title = popupBox.Title;
        _buttonHandlers = buttonHandlers;
        _text = popupBox.Text?.ToList() ?? new List<string>();
        //_listbox = listbox;
        _image = image;
        _icons = icons ?? Array.Empty<Image>();
        _optionsColumns = optionsCols < 1 ? 1 : optionsCols;
        //if (checkboxOptionState is not null) CheckboxReturnStates = new List<bool>(checkboxOptionState); // Initialize return checkbox states

        // Define textboxes
        if (textBoxDefs is not null)
        {
            // Fill text into textboxes & then remove it from popubox Text
            if (popupBox.Options?.Count > 0)
            {
                SetTextBoxText(textBoxDefs, popupBox.Options);
            }
            else
            {
                SetTextBoxText(textBoxDefs, _text);
                _text = _text.Take(textBoxDefs[0].index - 1).ToList();
            }

            // Texts next to textbox
            _formattedTextboxTexts = textBoxDefs.Select(def => new FormattedText
            {
                //Font = new Font("Arial", 10, FontStyle.Bold),
                Color = new Color(51, 51, 51, 255),
                Text = def.Text
            }
            ).ToArray();
            var maxWidth = _formattedTextboxTexts.Max(box => box.MeasureWidth());

            _textBoxes = new List<Textbox>();
            var i = 0;
            foreach (var boxDef in textBoxDefs)
            {
                _textBoxes.Add(new Textbox
                {
                    Name = boxDef.Name,
                    //Font = new Font("Times new roman", 12),
                    Text = boxDef.InitialValue,
                    MinValue = boxDef.MinValue,
                    Width = boxDef.Width,
                    CharLimit = boxDef.CharLimit
                });

                i++;
            }
        }
        else
        {
            _options = popupBox.Options;
        }

        _buttonTexts = popupBox.Button;

        // Format title & adjust dialog width to fit the title
        if (!string.IsNullOrWhiteSpace(_title))
        {
            _fTitle = GetFormattedTitle(popupBox.Title, replaceStrings, replaceNumbers);
            _innerSize.width = _fTitle.MeasureWidth() - _padding.L - _padding.R;
        }

        // Determine size of text and based on that determine dialog size (TODO)
        _textHeight = 0;
        if (_text?.Count > 0)
        {
            _fTexts = GetFormattedTexts(_text, popupBox.LineStyles.ToList(), replaceStrings, replaceNumbers);
            _innerSize.width = Math.Max(_innerSize.width, GetInnerPanelWidthFromText(_fTexts, popupBox));
            foreach (var fText in _fTexts)
            {
                fText.MaxWidth = _innerSize.width; // Adjust text width to inner panel width
            }

            var totalHeight = _fTexts.Sum(fText => fText.MeasureHeight() / 28.0) * 28;
            _textHeight = (int)Math.Round(totalHeight);
            foreach (var fText in _fTexts)
            {
                _innerSize.height += (int)Math.Round(fText.MeasureHeight() / 28.0) * 28;
            }
        }

        // Correction of inner panel size for options
        _optionsRows = GetOptionsRows(popupBox.Options?.Count, _optionsColumns);
        var iconsHeight = _icons.Length == 0 ? 0 : _icons.Sum(i => i.height) + 4 * (_icons.Length - 1);
        _innerSize = new Size(Math.Max(_size.width, GetInnerPanelWidthFromOptions(popupBox, _optionsRows, _optionsColumns, _icons, textBoxDefs)),
            (_optionsRows - _icons.Length) * 32 + iconsHeight + _textHeight);

        // Correction of inner panel size for image
        //_innerSize = new Size(Math.Max(_innerSize.width, _image.width ?? 0), Math.Max(_innerSize.height, _image.height ?? 0));

        // Correction of inner panel size for textbox
        _innerSize = new Size(_innerSize.width, _innerSize.height + (30 * textBoxDefs?.Count ?? 0));

        //_listboxShownLines = popupBox.ListboxLines;
        //_listboxHeight = _listboxShownLines * 23 + 2;
        //_innerSize.Height += _listboxHeight;
        _size = new Size(_innerSize.width + 2 * 2 + _padding.L + _padding.R, _innerSize.height + 2 * 2 + _padding.T + _padding.B);

        // Create buttons
        _buttons = new Button[_buttonTexts.Count];
        _btnWidth = (_size.width - _padding.L - _padding.R - 3 * (_buttons.Length - 1)) / _buttons.Length;
        for (int i = 0; i < _buttons.Length; i++)
        {
            _buttons[i] = new Button(_btnWidth, 36, _buttonTexts[i]);
        }

        // Initial dialog position on screen
        if (relatDialogPos.X < 0) // offset from right
        {
            _dialogPosX = (int)((1 + relatDialogPos.X) * Raylib.GetScreenWidth()) - _size.width;
        }
        else if (relatDialogPos.X > 0)
        {
            _dialogPosX = (int)(relatDialogPos.X * Raylib.GetScreenWidth());
        }
        else // =0 (center on screen)
        {
            _dialogPosX = (int)(Raylib.GetScreenWidth() * 0.5 - _size.width * 0.5);
        }

        if (relatDialogPos.Y < 0) // offset from bottom
        {
            _dialogPosY = (int)((1 + relatDialogPos.Y) * Raylib.GetScreenHeight()) - _size.height;
        }
        else if (relatDialogPos.Y > 0)
        {
            _dialogPosY = (int)(relatDialogPos.Y * Raylib.GetScreenHeight());
        }
        else // =0 (center on screen)
        {
            _dialogPosY = (int)(Raylib.GetScreenHeight() * 0.5 - _size.height * 0.5);
        }
    }

    private void SetTextBoxText(List<TextBoxDefinition> textBoxes, IList<string> text)
    {
        foreach (var textBox in textBoxes)
            textBox.Text = text[textBox.index];
    }

    public void Draw()
    {
        int x = _dialogPosX;
        int y = _dialogPosY;
        int w = _size.width;
        int h = _size.height;

        Vector2 mousePos = Raylib.GetMousePosition();
        Vector2 delta;

        // Drag/move dialog
        if (Raylib.IsMouseButtonDown(MouseButton.MOUSE_BUTTON_LEFT) && Raylib.CheckCollisionPointRec(mousePos, new Rectangle(x, y, w, _padding.T)))
        {
            dragging = true;
        }

        if (dragging && Raylib.IsMouseButtonUp(MouseButton.MOUSE_BUTTON_LEFT))
        {
            dragging = false;
        }

        if (dragging)
        {
            delta = Raylib.GetMouseDelta();
            _dialogPosX += (int)delta.X;
            _dialogPosY += (int)delta.Y;
        }

        ImageUtils.PaintDialogBase(x, y, w, h, _padding);

        // Draw title text
        var textSize = Raylib.MeasureTextEx(Raylib.GetFontDefault(), _title, 20, 1.0f);
        Raylib.DrawTextEx(Raylib.GetFontDefault(), _title, new Vector2(x + w / 2 - (int)textSize.X / 2, y + _padding.T / 2 - (int)textSize.Y / 2), 20, 1.0f, Color.BLACK);

        // Draw body text
        if (_fTexts is not null)
        {
            h = 0;
            foreach (var fText in _fTexts)
            {
                fText.Draw(x + _padding.L + 2, y + _padding.T + 2 + h);
                h += fText.MeasureHeight();
            }
        }

        // Draw options
        if (_options is not null)
        {
            // Detect mouse click on option
            if (Raylib.IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT) && Raylib.CheckCollisionPointRec(mousePos, new Rectangle(x + _padding.L + 8, y + _padding.T + 5, w - _padding.L - _padding.R - 8, 32 * _options.Count)))
            {
                _selectedRadio = ((int)mousePos.Y - y - _padding.T - 5) / 32;
            }

            for (int i = 0; i < _options.Count; i++)
            {
                ImageUtils.PaintRadioButton(x + _padding.L + 10, y + _padding.T + 9 + 32 * i, _selectedRadio == i);
                Raylib.DrawText(_options[i], x + _padding.L + 40, y + _padding.T + 10 + 32 * i, 20, Color.BLACK);

                if (_selectedRadio == i)
                {
                    Raylib.DrawRectangleLines(x + _padding.L + 34, y + _padding.T + 5 + 32 * i, w - _padding.L - _padding.R - 34 - 2, 26, new Color(64, 64, 64, 255));
                }
            }
        }

        // Draw textboxes
        if (_textBoxes is not null)
        {
            for (int i = 0; i < _textBoxes.Count; i++)
            {
                _formattedTextboxTexts[i].Draw(x + _padding.L, y + _padding.T + _fTexts.Sum(t => t.MeasureHeight()) + 14 + 40 * i);
                if (_textBoxes[i].Draw(x + _padding.L + _formattedTextboxTexts.Max(box => box.MeasureWidth()) + 4,
                y + _padding.T + 2 + _fTexts.Sum(w => w.MeasureHeight()) + 2 + 40 * i))
                    _textBoxes[i].EditMode = !_textBoxes[i].EditMode;
            }
        }

        // Draw buttons
        int selectedButton = -1;
        for (int i = 0; i < _buttons.Length; i++)
        {
            if (_buttons[i].Draw(x + _padding.L + _btnWidth * i + 3 * i - 2, y + _size.height - _padding.B + 4))
            {
                selectedButton = i;
            }
        }

        if (selectedButton != -1)
        {
            _buttonHandlers[selectedButton >= _buttonHandlers.Length ? 0 : selectedButton](
                _buttonTexts[selectedButton], _selectedRadio, FormatTextBoxReturn());
        }
    }

    private static FormattedText GetFormattedTitle(string title, IList<string> replaceStrings, IList<int> replaceNumbers)
    {
        var fTitle = new FormattedText
        {
            Text = Replace(title, replaceStrings, replaceNumbers),
            HorizontalAlignment = HorizontalAlignment.Center
        };
        return fTitle;
    }

    private IDictionary<string,string>? FormatTextBoxReturn()
    {
        //return _textBoxes?.Select((k, i) => new { k.Name, Value = TextBoxValues[i] })
        //    .ToDictionary(k => k.Name, v => v.Value);        
        return _textBoxes?.Select(box => new { box.Name, Value = box.Text })
            .ToDictionary(k => k.Name, v => v.Value);
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

    /// <summary>
    /// Determine max width of a popup box from options.
    /// </summary>
    private static int GetInnerPanelWidthFromOptions(PopupBox popupBox, int optionsRows, int optionsColumns, Image[] icons, List<TextBoxDefinition> textBoxDefinitions)
    {
        int width = 0;
        if (optionsRows > 0)
        {
            for (var index = 0; index < optionsRows; index++)
            {
                var textWidthCandidate = Enumerable.Range(0, optionsColumns).Select(n => n * optionsRows + index)
                    .Where(n => n < popupBox.Options.Count).Select(n => Raylib.MeasureText(popupBox.Options[n], 18))
                    .Sum();
                var widthCandidate = textWidthCandidate + (icons.Length > index ? icons[index].width : 40 * optionsColumns); // Count in width of radio button
                if (widthCandidate > width) width = widthCandidate;
            }
        }

        if (popupBox.Width != 0)
            return (int)Math.Ceiling(Math.Max(width, popupBox.Width * 1.5));
        else
            return (int)Math.Ceiling(Math.Max(width, 660.0));    // 660=440*1.5
    }

    private static IList<FormattedText> GetFormattedTexts(IList<string> texts, IList<TextStyles> styles, IList<string> replaceStrings, IList<int> replaceNumbers)
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
                //Font = new Font("Times New Roman", 18),
                HorizontalAlignment = styles[i] == TextStyles.Centered ? HorizontalAlignment.Center : HorizontalAlignment.Left
            });

            i++;
        }
        return formattedTexts;
    }

    private static int GetInnerPanelWidthFromText(IList<FormattedText> fTexts, PopupBox popupbox)
    {
        var centredTextMaxWidth = 0.0;
        if (fTexts.Where(t => t.HorizontalAlignment == HorizontalAlignment.Center).Any())
            centredTextMaxWidth = (from text in fTexts
                                   where text.HorizontalAlignment == HorizontalAlignment.Center
                                   orderby text.MeasureWidth() descending
                                   select text).ToList().FirstOrDefault().MeasureWidth();

        if (popupbox.Width != 0)
            return (int)Math.Ceiling(Math.Max(centredTextMaxWidth, popupbox.Width * 1.5));
        else
            return (int)Math.Ceiling(Math.Max(centredTextMaxWidth, 660.0));    // 660=440*1.5
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

    private struct Size
    {
        public int width;
        public int height;

        public Size(int width, int height)
        {
            this.width = width;
            this.height = height;
        }
    }
}