using Civ2engine;
using Model;
using Model.Interface;
using Raylib_cs;
using System.Numerics;

namespace RaylibUI.Forms;

public class Dialog : Form, IForm
{
    //public IDictionary<string, string> TextboxValues = new Dictionary<string, string>();

    private readonly Action<string, int, IList<bool>, IDictionary<string, string>?>[] _buttonHandlers;
    private readonly IList<string> _text;
    private readonly IList<FormattedText> _fTexts;
    private readonly Size _innerSize;
    private readonly List<Textbox> _textBoxes;
    private readonly FormattedText[] _formattedTextboxTexts;
    private readonly IList<string> _buttonTexts;
    private readonly int _textHeight, _optionsColumns, _optionsRows, _btnWidth;
    private readonly Image[] _icons;
    private readonly Image _image;
    private readonly Button[] _buttons;
    private readonly RadioButtonList _options;
    private readonly CheckBoxList _checkboxes;

    private Texture2D texture;

    public Dialog(PopupBox popupBox, Point relatDialogPos, Action<string, int, IList<bool>, IDictionary<string, string>?>[] buttonHandlers, IList<string>? replaceStrings = null, IList<int>? replaceNumbers = null, IList<bool>? checkboxStates = null, List<TextBoxDefinition>? textBoxDefs = null, int optionsCols = 1, Image[]? icons = null, Image image = new Image())
    {
        Padding = new Padding(11, 11, 38, 46);
        _buttonHandlers = buttonHandlers;
        _text = popupBox.Text?.ToList() ?? new List<string>();
        //_listbox = listbox;

        // TEST !!!!
        _image = image;
        //int _frames = 0;
        //_image = Raylib.LoadImageAnim(@"C://Program Files (x86)\Civ2\UNITS.GIF", out _frames);
        //Raylib.ImageCrop(ref _image, new Rectangle(1, 1, 64, 48));
        texture = Raylib.LoadTextureFromImage(_image);

        _icons = icons ?? Array.Empty<Image>();
        _optionsColumns = optionsCols < 1 ? 1 : optionsCols;

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
            _textBoxes.ForEach(tbox => Controls.Add(tbox));
        }
        else
        {
            if (popupBox.Options is not null)
            {
                for (int i = 0; i < popupBox.Options.Count; i++) 
                {
                    popupBox.Options[i] = Replace(popupBox.Options[i], replaceStrings, replaceNumbers);
                }
                
                if (popupBox.Checkbox)
                {
                    _checkboxes = new() { Texts = popupBox.Options, Checked = checkboxStates };
                    Controls.Add(_checkboxes);
                }
                else
                {
                    _options = new(popupBox.Options, _optionsColumns);
                    Controls.Add(_options);
                }
            }
        }

        _buttonTexts = popupBox.Button;

        // Format title & adjust dialog width to fit the title
        if (!string.IsNullOrWhiteSpace(popupBox.Title))
        {
            Title = new FormattedText
            {
                Text = Replace(popupBox.Title, replaceStrings, replaceNumbers),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            _innerSize.width = Title.MeasureWidth() - Padding.L - Padding.R;
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
        _innerSize = new Size(Math.Max(Size.width, GetInnerPanelWidthFromOptions(popupBox, _optionsRows, _optionsColumns, _icons, textBoxDefs)),
            (_optionsRows - _icons.Length) * 32 + iconsHeight + _textHeight);

        // Correction of inner panel size for image
        _innerSize = new Size(_innerSize.width + _image.width, Math.Max(_innerSize.height, _image.height));

        // Correction of inner panel size for textbox
        if (popupBox.Options == null)
        {
            _innerSize = new Size(_innerSize.width, _innerSize.height + (32 * textBoxDefs?.Count ?? 0));
        }

        //_listboxShownLines = popupBox.ListboxLines;
        //_listboxHeight = _listboxShownLines * 23 + 2;
        //_innerSize.Height += _listboxHeight;
        Size = new Size(_innerSize.width + 2 * 2 + Padding.L + Padding.R, _innerSize.height + 2 * 2 + Padding.T + Padding.B);

        // Create buttons
        _buttons = new Button[_buttonTexts.Count];
        _btnWidth = (Size.width - Padding.L - Padding.R - 3 * (_buttons.Length - 1)) / _buttons.Length;
        for (int i = 0; i < _buttons.Length; i++)
        {
            _buttons[i] = new Button
            {
                Width = _btnWidth,
                Height = 36,
                Text = _buttonTexts[i]
            };
            Controls.Add(_buttons[i]);
        }

        // Initial dialog position on screen
        if (relatDialogPos.X < 0) // offset from right
        {
            _formPosX = (int)((1 + relatDialogPos.X) * Raylib.GetScreenWidth()) - Size.width;
        }
        else if (relatDialogPos.X > 0)
        {
            _formPosX = (int)(relatDialogPos.X * Raylib.GetScreenWidth());
        }
        else // =0 (center on screen)
        {
            _formPosX = (int)(Raylib.GetScreenWidth() * 0.5 - Size.width * 0.5);
        }

        if (relatDialogPos.Y < 0) // offset from bottom
        {
            _formPosY = (int)((1 + relatDialogPos.Y) * Raylib.GetScreenHeight()) - Size.height;
        }
        else if (relatDialogPos.Y > 0)
        {
            _formPosY = (int)(relatDialogPos.Y * Raylib.GetScreenHeight());
        }
        else // =0 (center on screen)
        {
            _formPosY = (int)(Raylib.GetScreenHeight() * 0.5 - Size.height * 0.5);
        }

        Raylib.UnloadImage(_image);
    }

    private void SetTextBoxText(List<TextBoxDefinition> textBoxes, IList<string> text)
    {
        foreach (var textBox in textBoxes)
            textBox.Text = text[textBox.index];
    }

    public void Draw()
    {
        base.Draw();

        Vector2 mousePos = Raylib.GetMousePosition();

        // Draw image
        int x_offset = _formPosX + Padding.L + 2;
        int y_offset = _formPosY + Padding.T + 2;
        Raylib.DrawTexture(texture, x_offset, y_offset, Color.WHITE);
        if (texture.width > 0)
            x_offset += texture.width + 2;

        // Draw body text
        _fTexts?.ToList().ForEach(t => t.Draw(x_offset, y_offset));
        var ok = _fTexts?.Sum(t => t.MeasureHeight());
        y_offset += Math.Max(texture.height, _fTexts?.Sum(t => t.MeasureHeight()) ?? 0);


        // Draw buttons
        int selectedButton = -1;
        for (int i = 0; i < _buttons.Length; i++)
        {
            _buttons[i].Draw(_formPosX + Padding.L + _btnWidth * i + 3 * i - 2, _formPosY + Size.height - Padding.B + 4);
            if (_buttons[i].Pressed)
            {
                selectedButton = i;
            }
        }

        // Draw options
        _options?.Draw(x_offset, y_offset, _innerSize.width);

        // Draw checkboxes
        _checkboxes?.Draw(x_offset, y_offset, _innerSize);

        // Draw textboxes
        if (_textBoxes is not null)
        {
            for (int i = 0; i < _textBoxes.Count; i++)
            {
                _formattedTextboxTexts[i].Draw(x_offset, y_offset + 5 + 32 * i);
                if (_textBoxes[i].Draw(x_offset + _formattedTextboxTexts.Max(box => box.MeasureWidth()) + 24,
                  y_offset + 2 + 32 * i))
                    _textBoxes[i].EditMode = !_textBoxes[i].EditMode;
            }
        }


        if (selectedButton != -1)
        {
            _buttonHandlers[selectedButton >= _buttonHandlers.Length ? 0 : selectedButton](
                _buttonTexts[selectedButton], _options?.Selected ?? 0, _checkboxes?.Checked ?? null, FormatTextBoxReturn());
        }
    }

    private IDictionary<string, string>? FormatTextBoxReturn()
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
                Text = text == "" && styles[i] == TextStyles.LeftOwnLine ? " " : text,    // Add space if ^ is the only character
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
}