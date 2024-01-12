using Civ2engine;
using Model;
using Model.Interface;
using Raylib_cs;
using RaylibUI.BasicTypes.Controls;
using RaylibUI.Controls;
using RaylibUI.Dialogs;
using RaylibUI.Forms;
using Button = RaylibUI.Controls.Button;

namespace RaylibUI;

public class CivDialog : DynamicSizingDialog
{
    private OptionControl? _selectedOption;

    private IList<bool>? _checkboxes;

    private readonly IList<LabeledTextBox> _textBoxes;
    private readonly List<OptionControl>? _optionControls;
    private readonly Action<string, int, IList<bool>?, IDictionary<string, string>?> _handleButtonClick;
    private readonly int _optionsCols;

    private void SetSelectedOption(OptionControl newSelection)
    {
        if (_selectedOption == newSelection) return;
        newSelection.Checked = true;
        _selectedOption?.Clear();
        _selectedOption = newSelection;
        this.Focused = newSelection;
    }

    private void TogggleCheckBox(OptionControl checkBox)
    {
        checkBox.Checked = !checkBox.Checked;
        if (_checkboxes == null || _checkboxes.Count < checkBox.Index)
        {
            var old = _checkboxes ?? new List<bool>();
            _checkboxes = old.Concat(Enumerable.Repeat(false, checkBox.Index + 1)).ToList();
        }

        _checkboxes[checkBox.Index] = checkBox.Checked;
    }

    public CivDialog(Main host,
        PopupBox popupBox,
        Point relatDialogPos,
        Action<string, int, IList<bool>?, IDictionary<string, string>?> handleButtonClick,
        IList<string>? replaceStrings = null,
        IList<int>? replaceNumbers = null,
        IList<bool>? checkboxStates = null,
        List<TextBoxDefinition>? textBoxDefs = null,
        int optionsCols = 1,
        Image[]? icons = null) :
        base(host,
            DialogHelper.ReplacePlaceholders(popupBox.Title, replaceStrings, replaceNumbers),
             relatDialogPos
            , requestedWidth: popupBox.Width == 0 ? host.ActiveInterface.DefaultDialogWidth : popupBox.Width)
    {
        _handleButtonClick = handleButtonClick;
        _optionsCols = optionsCols;
        _managedTextures = new List<Texture2D>();
        
        if (popupBox.Text?.Count > 0)
        {
            var ftext = DialogHelper.GetFormattedTexts(popupBox.Text, popupBox.LineStyles, replaceStrings, replaceNumbers);
            foreach (var text in ftext)
            {
                Controls.Add(new LabelControl(this, text.Text, false,
                    alignment: text.HorizontalAlignment == HorizontalAlignment.Center
                        ? TextAlignment.Center
                        : TextAlignment.Left, wrapText: text.HorizontalAlignment == HorizontalAlignment.Left));
            }
        }

        var options = popupBox.Options;

        if (textBoxDefs is { Count: > 0 })
        {
            
            _textBoxes = new List<LabeledTextBox>();
            List<string> textBoxLabels;
            if(textBoxDefs.Any(t=>string.IsNullOrWhiteSpace(t.Description)) && popupBox.Options != null && popupBox.Options.Count == textBoxDefs.Count)
            {
                textBoxLabels = new List<string>(popupBox.Options);
                options = null;
            }
            else
            {
                textBoxLabels = textBoxDefs.Select(t =>
                    DialogHelper.ReplacePlaceholders(t.Description, replaceStrings, replaceNumbers)).ToList();
            }

            var labelSize = (int)textBoxLabels.Max(l => Raylib.MeasureTextEx(Fonts.DefaultFont, l, 20, 1.0f).X) +24;

            for (int i = 0; i < textBoxDefs.Count; i++)
            {
                var labeledBox = new LabeledTextBox(this, textBoxDefs[i],textBoxLabels[i], labelSize);
                Controls.Add(labeledBox);
                _textBoxes.Add(labeledBox);
            }
        }

        _checkboxes = checkboxStates;
        if (options is not null)
        {
            for (int i = 0; i < options.Count; i++)
            {
                options[i] =
                    Forms.DialogHelper.ReplacePlaceholders(options[i], replaceStrings, replaceNumbers);
            }

            var optionAction = popupBox.Checkbox ? (Action<OptionControl>)TogggleCheckBox : SetSelectedOption;

            var iconTextures =
                icons?.Select(Raylib.LoadTextureFromImage).ToArray()
                ?? Array.Empty<Texture2D>();
            _managedTextures.AddRange(iconTextures);

            var images = ImageUtils.GetOptionImages(popupBox.Checkbox);

            _optionControls = options.Select((o, i) =>
                new OptionControl(this, o, i, checkboxStates?[i] ?? false,
                    i < iconTextures.Length ? new[] { iconTextures[i] } : images)).ToList();
            _optionControls.ForEach(c=>c.Click += (_,_) =>optionAction(c));
            if (!popupBox.Checkbox)
            {
                _optionControls[popupBox.Default].Checked = true;
                SetSelectedOption(_optionControls[popupBox.Default]);
            }

            if (optionsCols < 2)
            {
                _optionControls.ForEach(Controls.Add);
            }
            else
            {
                var optionsCount = _optionControls.Count;
                var rows = optionsCount % optionsCols == 0
                    ? optionsCount / optionsCols
                    : optionsCount / optionsCols + 1;

                for (var i = 0; i < rows; i++)
                {
                    var optionGroup = new ControlGroup(this);
                    for (var j = 0; j < optionsCols; j++)
                    {
                        var optionIndex = i + j * rows;
                        if (optionIndex < _optionControls.Count)
                        {
                            optionGroup.AddChild(_optionControls[optionIndex]);
                        }
                    }

                    Controls.Add(optionGroup);
                }
            }
        }

        var menuBar = new ControlGroup(this);
        foreach (var button in popupBox.Button)
        {
            var actionButton = new Button(this, button);

            actionButton.Click += OnActionButtonOnClick;
            menuBar.AddChild(actionButton);
        }

        Controls.Add(menuBar);
        SetButtons(menuBar);
    }
    
    private void OnActionButtonOnClick(object? sender, MouseEventArgs mouseEventArgs)
    {
        if (sender is not Button button) return;

        CloseDialog(button.Text);
    }

    private void CloseDialog(string buttonText)
    {
        _managedTextures.ForEach(Raylib.UnloadTexture);
        _handleButtonClick(buttonText, _selectedOption?.Index ?? -1, _checkboxes, FormatTextBoxReturn());
    }

    private readonly KeyboardKey[] _navKeys = {
        KeyboardKey.KEY_UP, KeyboardKey.KEY_DOWN, KeyboardKey.KEY_LEFT,  KeyboardKey.KEY_RIGHT,
        KeyboardKey.KEY_KP_8, KeyboardKey.KEY_KP_2, KeyboardKey.KEY_KP_4, KeyboardKey.KEY_KP_6,
    };

    private readonly List<Texture2D> _managedTextures;

    public override void OnKeyPress(KeyboardKey key)
    {
        switch (key)
        {
            case KeyboardKey.KEY_ENTER when ButtonExists(Labels.Ok):
                CloseDialog(Labels.Ok);
                return;
            case KeyboardKey.KEY_ESCAPE when ButtonExists(Labels.Cancel):
                CloseDialog(Labels.Cancel);
                return;
        }

        if (_optionControls?.Count > 0)
        {
            var keyIndex = Array.IndexOf(_navKeys, key);
            if (keyIndex != -1)
            {
                var dir = keyIndex % (_optionsCols > 1 ? 4 : 2);
                switch (dir)
                {
                    case 0:
                        if (_selectedOption != null && _selectedOption.Index != 0)
                        {
                            SetSelectedOption(_optionControls[_selectedOption.Index -1]);
                        }
                        else
                        {
                            SetSelectedOption(_optionControls[^1]);
                        }
                        break;
                    case 1:
                        if (_selectedOption != null &&_selectedOption.Index < _optionControls.Count -1)
                        {
                            SetSelectedOption(_optionControls[_selectedOption.Index + 1]);
                        }
                        else
                        {
                            SetSelectedOption(_optionControls[0]);
                        }
                        break;
                    case 3:
                        if (_selectedOption != null)
                        {
                            var rows = GetRows();
                            var newIndex = _selectedOption.Index + rows;
                            if (newIndex >= _optionControls.Count)
                            {
                                newIndex -= _optionControls.Count;
                            }

                            SetSelectedOption(_optionControls[newIndex]);
                        }
                        else
                        {
                            SetSelectedOption(_optionControls[0]);
                        }
                        break;
                    case 2:
                        if (_selectedOption != null)
                        {
                            var rows = GetRows();
                            var newIndex = _selectedOption.Index - rows;
                            if (newIndex < 0)
                            {
                                newIndex += _optionControls.Count;
                            }

                            SetSelectedOption(_optionControls[newIndex]);
                        }
                        else
                        {
                            SetSelectedOption(_optionControls[^1]);
                        }
                        break;


                }
            }
        }

       

        base.OnKeyPress(key);
    }

 

    private int GetRows()
    {
        var rows = Math.DivRem(_optionControls.Count, _optionsCols, out var rem);
        if (rem != 0)
        {
            rows++;
        }
        return rows;
    }

    private IDictionary<string, string>? FormatTextBoxReturn()
    {
        return _textBoxes?.Select(box => new { box.Name, Value = box.Text })
            .ToDictionary(k => k.Name, v => v.Value);
    }
}