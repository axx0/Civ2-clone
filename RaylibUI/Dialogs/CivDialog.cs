using Civ2engine;
using Model;
using Model.Dialog;
using Model.Images;
using Model.Interface;
using Raylib_CSharp.Windowing;
using Raylib_CSharp.Interact;
using RaylibUI.BasicTypes.Controls;
using RaylibUI.Controls;
using RaylibUI.Dialogs;
using RaylibUI.Forms;
using RaylibUtils;
using System.Collections.Generic;
using System.Numerics;
using System.Reflection.Emit;
using Model.Core;
using Button = RaylibUI.Controls.Button;
using Raylib_CSharp.Rendering;
using Raylib_CSharp.Fonts;

namespace RaylibUI;

public class CivDialog : DynamicSizingDialog
{
    private OptionControl? _selectedOption;

    private IList<bool>? _checkboxes;

    private readonly IList<LabeledTextBox> _textBoxes;
    private readonly List<OptionControl>? _optionControls;
    private readonly Action<string, int, IList<bool>?, IDictionary<string, string>?> _handleButtonClick;
    private readonly int _optionsCols, _initSelectedOption;
    private readonly IUserInterface _active;
    private readonly Vector2 _innerSize;

    private void SetSelectedOption(OptionControl newSelection)
    {
        if (_selectedOption == newSelection) return;
        newSelection.Checked = true;
        _selectedOption?.Clear();
        _selectedOption = newSelection;
        _selectedIndex = newSelection.Index;
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
        int initSelectedOption = 0,
        IImageSource[]? optionsIcons = null,
        DialogImageElements? image = null,
        ListBoxDefinition? listBox = null) :
        base(host, 
            DialogUtils.ReplacePlaceholders(popupBox.Title, replaceStrings, replaceNumbers),
            popupBox.X != null || popupBox.Y != null ? new Point(popupBox.X ?? 0 / Window.GetScreenWidth(), popupBox.Y ?? 0 / Window.GetScreenHeight()) : relatDialogPos,
            requestedWidth: popupBox.Width == 0 ? host.ActiveInterface.DefaultDialogWidth: popupBox.Width)
    {
        _active = host.ActiveInterface;
        _handleButtonClick = handleButtonClick;
        _optionsCols = optionsCols;
        _initSelectedOption = popupBox.Default != 0 ? popupBox.Default : initSelectedOption;

        if (image != null && image.Image.All(n => n != null))
        {
            Controls.Add(new ImageBox(this, image));
        }

        if (popupBox.Text?.Count > 0)
        {
            var textLabels = GetTextLabels(this, popupBox.Text, popupBox.LineStyles, replaceStrings, replaceNumbers);
            var maxWidth = GetInnerPanelWidthFromText(textLabels, popupBox);
            foreach (var label in textLabels)
            {
                label.Width = maxWidth;
                Controls.Add(label);
            }
        }

        var options = popupBox.Options;

        if (listBox is not null)
        {
            var civDialogListBox = new CivDialogListBox(this, listBox);
            Controls.Add(civDialogListBox);
            civDialogListBox.ItemSelected += ListBoxOnItemSelected;
        }

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
                    DialogUtils.ReplacePlaceholders(t.Description, replaceStrings, replaceNumbers)).ToList();
            }

            var labelSize = (int)textBoxLabels.Max(l => TextManager.MeasureTextEx(host.ActiveInterface.Look.DefaultFont, l, 20, 1.0f).X) +24;

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
                    DialogUtils.ReplacePlaceholders(options[i], replaceStrings, replaceNumbers);
            }

            var optionAction = popupBox.Checkbox ? (Action<OptionControl>)TogggleCheckBox : SetSelectedOption;

            var images = popupBox.Checkbox ? _active.Look.CheckBoxes : _active.Look.RadioButtons;

            _optionControls = options.Select((o, i) =>
                new OptionControl(this, o, i, checkboxStates?[i] ?? false,
                    i < (optionsIcons?.Length ?? 0) ? new[] { optionsIcons[i] } : images)).ToList();
            _optionControls.ForEach(c=>c.Click += (_,_) =>optionAction(c));
            if (!popupBox.Checkbox)
            {
                _optionControls[_initSelectedOption].Checked = true;
                SetSelectedOption(_optionControls[_initSelectedOption]);
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

    private void ListBoxOnItemSelected(object? sender, ScrollBoxSelectionEventArgs e)
    {
        _selectedIndex = e.Index;
    }

    private void OnActionButtonOnClick(object? sender, MouseEventArgs mouseEventArgs)
    {
        if (sender is not Button button) return;

        CloseDialog(button.Text);
    }

    private void CloseDialog(string buttonText)
    {
        _handleButtonClick(buttonText, _selectedIndex, _checkboxes, FormatTextBoxReturn());
    }

    private readonly KeyboardKey[] _navKeys = {
        KeyboardKey.Up, KeyboardKey.Down, KeyboardKey.Left,  KeyboardKey.Right,
        KeyboardKey.Kp8, KeyboardKey.Kp2, KeyboardKey.Kp4, KeyboardKey.Kp6,
    };

     private int _selectedIndex = -1;

    public override void OnKeyPress(KeyboardKey key)
    {
        switch (key)
        {
            case KeyboardKey.Enter when ButtonExists(Labels.Ok):
                CloseDialog(Labels.Ok);
                return;
            case KeyboardKey.Escape when ButtonExists(Labels.Cancel):
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

    private List<LabelControl> GetTextLabels(IControlLayout controller, IList<string>? texts, IList<TextStyles>? styles, IList<string> replaceStrings, IList<int> replaceNumbers)
    {
        // Group left-aligned texts
        int j = 0;
        while (j < texts.Count - 1)
        {
            j++;
            if (styles[j - 1] == TextStyles.Left && styles[j] == TextStyles.Left)
            {
                texts[j] = $"{texts[j - 1]} {texts[j]}";
                texts.RemoveAt(j - 1);
                styles.RemoveAt(j - 1);
                j = 0;
            }
        }

        // Replace %STRING, %NUMBER
        texts = texts.Select(t => DialogUtils.ReplacePlaceholders(t, replaceStrings, replaceNumbers)).ToList();
        texts = texts.Select(t => t.Replace("_", " ")).ToList();

        // Make labels
        var labels = new List<LabelControl>();
        for (int i = 0; i < texts.Count; i++)
        {
            labels.Add(new LabelControl(controller,
                        string.IsNullOrEmpty(texts[i]) && styles[i] == TextStyles.LeftOwnLine ? " " : texts[i],    // Add space if ^ is the only character 
                        false,
                        alignment: styles[i] == TextStyles.Centered ? TextAlignment.Center : TextAlignment.Left,
                        wrapText: styles[i] == TextStyles.Left,
                        font: _active.Look.LabelFont, fontSize: _active.Look.LabelFontSize, colorFront: _active.Look.LabelColour, colorShadow: _active.Look.LabelShadowColour, shadowOffset: new Vector2(1, 1)));
        }

        return labels;
    }

    private static int GetInnerPanelWidthFromText(IList<LabelControl> labels, PopupBox popupbox)
    {
        var centredTextMaxWidth = 0.0;
        if (labels.Where(t => t.Alignment == TextAlignment.Center || (t.Alignment == TextAlignment.Left && !t.WrapText)).Any())
            centredTextMaxWidth = (from label in labels
                                   where label.Alignment == TextAlignment.Center || (label.Alignment == TextAlignment.Left && !label.WrapText)
                                   orderby label.TextSize.X descending
                                   select label).ToList().FirstOrDefault().TextSize.X;

        if (popupbox.Width != 0)
            return (int)Math.Ceiling(Math.Max(centredTextMaxWidth, popupbox.Width));
        else
            return (int)Math.Ceiling(Math.Max(centredTextMaxWidth, 660.0));    // 660=440*1.5
    }
}