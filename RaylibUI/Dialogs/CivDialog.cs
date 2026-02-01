using Civ2engine;
using Model;
using Model.Core;
using Model.Controls;
using Raylib_CSharp.Fonts;
using Raylib_CSharp.Interact;
using Raylib_CSharp.Windowing;
using RaylibUI.BasicTypes;
using RaylibUI.BasicTypes.Controls;
using RaylibUI.Controls;
using RaylibUI.Dialogs;
using System.Numerics;

namespace RaylibUI;

public class CivDialog : DynamicSizingDialog
{
    private readonly IUserInterface _active;
    private readonly TableLayoutPanel _innerPanel;
    private readonly IList<LabeledTextBox> _textBoxes;
    private readonly Action<string, int, IList<bool>?, IDictionary<string, string>?> _handleButtonClick;
    private readonly OptionsPanel? _optionsPanel;
    private readonly Listbox? _listbox;
    private int _selectedIndex = -1;
    private readonly ImageBox _imageBox;

    public CivDialog(Main host, DialogElements dialog, Action<string, int, IList<bool>?, IDictionary<string, string>?> handleButtonClick) :
        base(host, DialogUtils.ReplacePlaceholders(dialog.Title, dialog.ReplaceStrings, dialog.ReplaceNumbers),
            dialog.X != null || dialog.Y != null ? new Point(dialog.X ?? 0 / Window.GetScreenWidth(),
            dialog.Y ?? 0 / Window.GetScreenHeight()) : dialog.DialogPos, requestedWidth: dialog.Width == 0 ?
            host.ActiveInterface.DefaultDialogWidth : dialog.Width)
    {
        _active = host.ActiveInterface;
        _handleButtonClick = handleButtonClick;

        var innerLayout = new TableLayout();
        var layoutRow = 0;

        if (dialog.Image != null && dialog.Image.Image.All(n => n != null))
        {
            _imageBox = new ImageBox(this, dialog.Image);
            innerLayout.Add(_imageBox, 0, 0);
        }

        var maxTextWidth = 0;
        if (dialog.Text?.Count > 0)
        {
            var textLabels = GetTextLabels(this, dialog.Text, dialog.LineStyles, dialog.ReplaceStrings, dialog.ReplaceNumbers);
            maxTextWidth = GetInnerPanelWidthFromText(textLabels, dialog.Width ?? 0);
            for (int i = 0; i < textLabels.Count; i++)
            {
                textLabels[i].Width = maxTextWidth;
                innerLayout.Add(textLabels[i], layoutRow++, 1);
            }
        }

        if (dialog.Listbox != null)
        {
            _listbox = new Listbox(this, dialog.Listbox);
            innerLayout.Add(_listbox, layoutRow++, 1, new Padding(2, 2, 2, 2));
            _listbox.ItemSelected += ListboxOnItemSelected;
            _selectedIndex = 0;
        }

        if (dialog.TextBoxes is { Count: > 0 })
        {
            _textBoxes = new List<LabeledTextBox>();
            List<string> textBoxLabels;
            if (dialog.TextBoxes.Any(t => string.IsNullOrWhiteSpace(t.Description)) && dialog.Options != null && dialog.Options.Texts.Count == dialog.TextBoxes.Count)
            {
                textBoxLabels = new List<string>(dialog.Options.Texts);
                dialog.Options = null;
            }
            else
            {
                textBoxLabels = dialog.TextBoxes.Select(t =>
                    DialogUtils.ReplacePlaceholders(t.Description, dialog.ReplaceStrings, dialog.ReplaceNumbers)).ToList();
            }

            var labelSize = (int)textBoxLabels.Max(l => TextManager.MeasureTextEx(host.ActiveInterface.Look.DefaultFont, l, 20, 1.0f).X) + 24;

            for (int i = 0; i < dialog.TextBoxes.Count; i++)
            {
                var labeledBox = new LabeledTextBox(this, dialog.TextBoxes[i], textBoxLabels[i], labelSize);
                labeledBox.Width = labeledBox.GetPreferredWidth();
                innerLayout.Add(labeledBox, layoutRow++, 1);
                _textBoxes.Add(labeledBox);
            }
        }

        if (dialog.Options is not null)
        {
            dialog.Options.ReplacedTexts = [];
            for (int i = 0; i < dialog.Options.Texts.Count; i++)
            {
                dialog.Options.ReplacedTexts.Add(DialogUtils.ReplacePlaceholders(dialog.Options.Texts[i], dialog.ReplaceStrings, dialog.ReplaceNumbers));
            }
            _optionsPanel = new OptionsPanel(this, dialog.Options);

            innerLayout.Add(_optionsPanel, layoutRow++, 1);
        }

        _innerPanel = new TableLayoutPanel(this)
        {
            Location = new Vector2(LayoutPadding.Left, LayoutPadding.Top),
            TableLayout = innerLayout
        };
        Controls.Add(_innerPanel);

        var menuBar = new ControlGroup(this);
        foreach (var button in dialog.Button)
        {
            var actionButton = new Button(this, button);

            actionButton.Click += OnActionButtonOnClick;
            menuBar.AddChild(actionButton);
        }

        Controls.Add(menuBar);
        SetButtons(menuBar);

        // Determine which control is focused at game start
        if (_optionsPanel != null)
        {
            Focused = _optionsPanel;
        }
        else if (_listbox != null)
        {
            Focused = _listbox;
        }
    }

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

        base.OnKeyPress(key);
    }

    private void OnActionButtonOnClick(object? sender, MouseEventArgs mouseEventArgs)
    {
        if (sender is not Button button) return;

        CloseDialog(button.Text);
    }

    private void ListboxOnItemSelected(object? sender, ListboxSelectionEventArgs e)
    {
        _selectedIndex = e.Index;
    }

    private void CloseDialog(string buttonText)
    {
        if (_optionsPanel != null)
        {
            _selectedIndex = _optionsPanel?.SelectedId ?? -1;
        }
        _handleButtonClick(buttonText, _selectedIndex, _optionsPanel?.CheckboxStates ?? [], FormatTextBoxReturn());
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
                        horizontalAlignment: styles[i] == TextStyles.Centered ? HorizontalAlignment.Center : HorizontalAlignment.Left,
                        wrapText: styles[i] == TextStyles.Left,
                        font: _active.Look.LabelFont, fontSize: _active.Look.LabelFontSize, colorFront: _active.Look.LabelColour, colorShadow: _active.Look.LabelShadowColour, shadowOffset: new Vector2(1, 1)));
        }

        return labels;
    }

    private int GetInnerPanelWidthFromText(IList<LabelControl> labels, int popupboxWidth)
    {
        var centredTextMaxWidth = 0.0;
        if (labels.Where(t => t.HorizontalAlignment == HorizontalAlignment.Center || (t.HorizontalAlignment == HorizontalAlignment.Left && !t.WrapText)).Any())
            centredTextMaxWidth = (from label in labels
                                   where label.HorizontalAlignment == HorizontalAlignment.Center || (label.HorizontalAlignment == HorizontalAlignment.Left && !label.WrapText)
                                   orderby label.TextSize.X descending
                                   select label).ToList().FirstOrDefault().TextSize.X;

        if (popupboxWidth != 0)
            return (int)Math.Ceiling(Math.Max(centredTextMaxWidth, popupboxWidth));
        else
            return (int)Math.Ceiling(Math.Max(centredTextMaxWidth, _active.DefaultDialogWidth));    // 660=440*1.5
    }
}