using Civ2engine;
using Model;
using Model.Interface;
using Raylib_cs;
using System.Numerics;

namespace RaylibUI;

public class Dialog
{
    private int _selectedRadio = 0;
    private readonly string[] _textBoxValues;
    
    private readonly Action<string, int, IDictionary<string,string>?>[] _buttonHandlers;
    private readonly List<string> _text;
    private readonly string _title;
    private readonly Size _size, _innerSize;
    private readonly Padding _padding;
    private readonly IList<string> _options;
    private readonly List<TextBoxDefinition>? _textBoxes;
    private readonly IList<string> _buttonTexts;
    private readonly int _textHeight, _optionsColumns, _optionsRows, _btnWidth;
    private readonly Image[] _icons;
    private readonly Image _image;
    private readonly Button[] _buttons;
    private int _dialogPosX, _dialogPosY;

    public Dialog(PopupBox popupBox, Point dialogPos, Action<string, int, IDictionary<string, string>?>[] buttonHandlers,List<TextBoxDefinition>? textBoxes = null, int optionsCols = 1, Image[] icons = null, Image image = new Image())
    {
        _dialogPosX = dialogPos.X;
        _dialogPosY = dialogPos.Y;
        _padding = new Padding(11, 11, 38, 46);
        _title = popupBox.Title;
        _buttonHandlers = buttonHandlers;
        var text = popupBox.Text?.ToList() ?? new List<string>();
        
        if (textBoxes is not null)
        {
            if (popupBox.Options?.Count > 0)
            {
                SetTextBoxText(textBoxes, popupBox.Options);
            }
            else
            {
                SetTextBoxText(textBoxes, text);
                text = text.Take(textBoxes[0].index - 1).ToList();
            }

            _textBoxValues = textBoxes.Select(t => t.InitialValue).ToArray();
            _textBoxes = textBoxes;
        }
        else
        {
            _options = popupBox.Options;
        }

        _text = text;
        _buttonTexts = popupBox.Button;
        //_listbox = listbox;
        _image = image;
        _icons = icons ?? Array.Empty<Image>();
        _optionsColumns = optionsCols < 1 ? 1 : optionsCols;
        //if (checkboxOptionState is not null) CheckboxReturnStates = new List<bool>(checkboxOptionState); // Initialize return checkbox states

        // Format title & adjust dialog width to fit the title
        if (!string.IsNullOrWhiteSpace(_title))
        {
            //_fTitle = GetFormattedTitle(popupBox.Title, replaceStrings, replaceNumbers);
            _innerSize.width = Raylib.MeasureText(_title, 18) - _padding.L - _padding.R;
        }

        // Determine size of text and based on that determine dialog size (TODO)
        _textHeight = 0;
        if (_text?.Count > 0)
        {
        //    _fTexts = GetFormattedTexts(_text, popupBox.LineStyles.ToList(), replaceStrings, replaceNumbers);
        //    _innerSize.Width = Math.Max(_innerSize.Width, GetInnerPanelWidthFromText(_fTexts, popupBox));
        //    foreach (var fText in _fTexts)
        //    {
        //        fText.MaximumWidth = _innerSize.Width; // Adjust text width to inner panel width
        //    }

        //    var totalHeight = _fTexts.Sum(fText => fText.Measure().Height / 28.0) * 28;
        //    _textHeight = (int)Math.Round(totalHeight);
        //    foreach (var fText in _fTexts)
        //    {
        //        _innerSize.Height += (int)Math.Round(fText.Measure().Height / 28.0) * 28;
        //    }
        }

        // Correction of inner panel size for options
        _optionsRows = GetOptionsRows(popupBox.Options?.Count, _optionsColumns);
        var iconsHeight = _icons.Length == 0 ? 0 : _icons.Sum(i => i.height) + 4 * (_icons.Length - 1);
        _innerSize = new Size(Math.Max(_size.width, GetInnerPanelWidthFromOptions(popupBox, _optionsRows, _optionsColumns, _icons, textBoxes)),
            (_optionsRows - _icons.Length) * 32 + iconsHeight + _textHeight);

        // Correction of inner panel size for image
        //_innerSize = new Size(Math.Max(_innerSize.width, _image.width ?? 0), Math.Max(_innerSize.height, _image.height ?? 0));

        // Correction of inner panel size for textbox
        _innerSize = new Size(_innerSize.width, _innerSize.height + (30 * textBoxes?.Count ?? 0));

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

        // Detect mouse click on option
        if (Raylib.IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT) && Raylib.CheckCollisionPointRec(mousePos, new Rectangle(x + _padding.L + 8, y + _padding.T + 5, w - _padding.L - _padding.R - 8, 32 * _options.Count)))
        {
            _selectedRadio = ((int)mousePos.Y - y - _padding.T - 5) / 32;
        }

        // Drag/move dialog
        if (Raylib.IsMouseButtonDown(MouseButton.MOUSE_BUTTON_LEFT) && Raylib.CheckCollisionPointRec(mousePos, new Rectangle(x, y, w, _padding.T)))
        {
            delta = Raylib.GetMouseDelta();
            _dialogPosX += (int)delta.X;
            _dialogPosY += (int)delta.Y;
        }

        ImageUtils.PaintDialogBase(x, y, w, h, _padding);

        // Dialog text
        var textSize = Raylib.MeasureTextEx(Raylib.GetFontDefault(), _title, 20, 1.0f);
        Raylib.DrawTextEx(Raylib.GetFontDefault(), _title, new Vector2(x + w / 2 - (int)textSize.X / 2, y + _padding.T / 2 - (int)textSize.Y / 2), 20, 1.0f, Color.BLACK);

        // Options
        for (int i = 0; i < _options.Count; i++)
        {
            ImageUtils.PaintRadioButton(x + _padding.L + 10, y + _padding.T + 9 + 32 * i, _selectedRadio == i);
            Raylib.DrawText(_options[i], x + _padding.L + 40, y + _padding.T + 10 + 32 * i, 20, Color.BLACK);

            if (_selectedRadio == i)
            {
                Raylib.DrawRectangleLines(x + _padding.L + 34, y + _padding.T + 5 + 32 * i, w - _padding.L - _padding.R - 34 - 2, 26, new Color(64, 64, 64, 255));
            }
        }

        // Buttons
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

    private IDictionary<string,string>? FormatTextBoxReturn()
    {
        return _textBoxes?.Select((k, i) => new { k.Name, Value = _textBoxValues[i] })
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
                //var widthCandidate = textWidthCandidate + imageWidth + (_icons.Length > index ? _icons[index].Width : 40 * optionsColumns); // Count in width of radio button
                var widthCandidate = textWidthCandidate + (icons.Length > index ? icons[index].width : 40 * optionsColumns); // Count in width of radio button
                if (widthCandidate > width) width = widthCandidate;
            }
        }

        if (popupBox.Width != 0)
            return (int)Math.Ceiling(Math.Max(width, popupBox.Width * 1.5));
        else
            return (int)Math.Ceiling(Math.Max(width, 660.0));    // 660=440*1.5
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