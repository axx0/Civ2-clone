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

public class CivDialog : BaseDialog
{
    private OptionControl? _selectedOption = null;

    private IList<bool>? _checkboxes = null;

    private IList<LabeledTextBox> _textBoxes;
    private readonly List<OptionControl>? _optionControls;
    private readonly int _optionsCols = 1;

    private void SetSelectedOption(OptionControl newSelection)
    {
        if (_selectedOption == newSelection) return;
        _selectedOption?.Clear();
        _selectedOption = newSelection;
        this.Focused = newSelection;
    }

    private void TogggleCheckBox(OptionControl checkBox)
    {
        if (_checkboxes == null || _checkboxes.Count < checkBox.Index)
        {
            var old = _checkboxes ?? new List<bool>();
            _checkboxes = old.Concat(Enumerable.Repeat(false, checkBox.Index + 1)).ToList();
        }

        _checkboxes[checkBox.Index] = !_checkboxes[checkBox.Index];
    }

    public CivDialog(Main host,
        PopupBox popupBox,
        Point relatDialogPos,
        Action<string, int, IList<bool>?,
            IDictionary<string, string>?> handleButtonClick,
        IList<string>? replaceStrings = null,
        IList<int>? replaceNumbers = null,
        IList<bool>? checkboxStates = null,
        List<TextBoxDefinition>? textBoxDefs = null,
        int optionsCols = 1,
        Image[]? icons = null) :
        base(host,
            Dialog.ReplacePlaceholders(popupBox.Title, replaceStrings, replaceNumbers),
            new Point(5, 5) // relatDialogPos
            , requestedWidth: popupBox.Width == 0 ? host.ActiveInterface.DefaultDialogWidth : popupBox.Width)
    {
        _optionsCols = optionsCols;
        List<Texture2D> managedTextures = new List<Texture2D>();
        if (popupBox.Text?.Count > 0)
        {
            var ftext = Dialog.GetFormattedTexts(popupBox.Text, popupBox.LineStyles, replaceStrings, replaceNumbers);
            foreach (var text in ftext)
            {
                Controls.Add(new LabelControl(this, text.Text, false,
                    alignment: text.HorizontalAlignment == HorizontalAlignment.Center
                        ? TextAlignment.Center
                        : TextAlignment.Left, wrapText: text.HorizontalAlignment == HorizontalAlignment.Left));
            }
        }

        if (textBoxDefs is { Count: > 0 })
        {
            _textBoxes = new List<LabeledTextBox>();
            var textBoxLabels = textBoxDefs.Select(t =>
                Dialog.ReplacePlaceholders(t.Description, replaceStrings, replaceNumbers)).ToList();

            var labelSize = (int)textBoxLabels.Max(l => Raylib.MeasureTextEx(Fonts.DefaultFont, l, 20, 1.0f).X) +24;

            for (int i = 0; i < textBoxDefs.Count; i++)
            {
                var labeledBox = new LabeledTextBox(this, textBoxDefs[i],textBoxLabels[i], labelSize);
                Controls.Add(labeledBox);
                _textBoxes.Add(labeledBox);
            }
        }

        _checkboxes = checkboxStates;
        if (popupBox.Options is not null)
        {
            for (int i = 0; i < popupBox.Options.Count; i++)
            {
                popupBox.Options[i] =
                    Forms.Dialog.ReplacePlaceholders(popupBox.Options[i], replaceStrings, replaceNumbers);
            }

            var optionAction = popupBox.Checkbox ? (Action<OptionControl>)TogggleCheckBox : SetSelectedOption;

            var iconTextures =
                icons?.Select(Raylib.LoadTextureFromImage).ToArray()
                ?? Array.Empty<Texture2D>();
            managedTextures.AddRange(iconTextures);

            var images = ImageUtils.GetOptionImages(popupBox.Checkbox);

            _optionControls = popupBox.Options.Select((o, i) =>
                new OptionControl(this, o, i, optionAction, checkboxStates?[i] ?? false,
                    i < iconTextures.Length ? new[] { iconTextures[i] } : images)).ToList();

            if (!popupBox.Checkbox)
            {
                _optionControls[0].Checked = true;
                SetSelectedOption(_optionControls[0]);
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
            actionButton.Click += (_,_) =>
                {
                    managedTextures.ForEach(Raylib.UnloadTexture);
                    handleButtonClick(button, _selectedOption?.Index ?? -1, _checkboxes,
                        FormatTextBoxReturn());
                };
            menuBar.AddChild(actionButton);
        }

        Controls.Add(menuBar);
        SetButtons(menuBar);
    }

    private KeyboardKey[] _navKays = new[]
    {
        KeyboardKey.KEY_UP, KeyboardKey.KEY_DOWN, KeyboardKey.KEY_LEFT,  KeyboardKey.KEY_RIGHT,
        KeyboardKey.KEY_KP_8, KeyboardKey.KEY_KP_2, KeyboardKey.KEY_KP_4, KeyboardKey.KEY_KP_6,
    };

    public override void OnKeyPress(KeyboardKey key)
    {
        if (_optionControls?.Count > 0)
        {
            var keyIndex = Array.IndexOf(_navKays, key);
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