using Civ2engine;
using Model;
using Model.Interface;
using Raylib_cs;
using RaylibUI.Controls;
using Button = RaylibUI.Controls.Button;

namespace RaylibUI;

public class CivDialog : BaseDialog
{
    private OptionControl _selectedOption = null;

    private IList<bool>? _checkboxes;

    private IList<LabeledTextBox> _textBoxes;

    private void SetSelectedOption(OptionControl newSelection)
    {
        if(_selectedOption == newSelection) return;
        _selectedOption?.Clear();
        _selectedOption = newSelection;
    }

    private void TogggleCheckBox(OptionControl checkBox)
    {
        if (_checkboxes == null || _checkboxes.Count < checkBox.Index)
        {
            var old = _checkboxes ?? new List<bool>();
            _checkboxes = old.Concat(Enumerable.Repeat(false, checkBox.Index+1)).ToList();
        }
        _checkboxes[checkBox.Index] = !_checkboxes[checkBox.Index];
    }

    public CivDialog(PopupBox popupBox, Point relatDialogPos, 
        Action<string, int, IList<bool>, 
            IDictionary<string, string>?> handleButtonClick, 
        IList<string>? replaceStrings = null, 
        IList<int>? replaceNumbers = null, IList<bool>? checkboxStates = null, List<TextBoxDefinition>? textBoxDefs = null, int optionsCols = 1, Image[]? icons = null, Image image = new Image(), Forms.ListBox? listbox = null) : 
        base(
            popupBox.Title, new Point(5, 5)) //actionMenuElement.DialogPos)
    {
        _checkboxes = checkboxStates;
        if (popupBox.Options is not null)
        {
            for (int i = 0; i < popupBox.Options.Count; i++)
            {
                popupBox.Options[i] =
                    RaylibUI.Forms.Dialog.ReplacePlaceholders(popupBox.Options[i], replaceStrings, replaceNumbers);
            }

            var optionAction = popupBox.Checkbox ? (Action<OptionControl>)SetSelectedOption : TogggleCheckBox;
            var images = ImageUtils.GetOptionImages(popupBox.Checkbox);

            for (int i = 0; i < popupBox.Options.Count; i++)
            {
                Controls.Add(new OptionControl(this, popupBox.Options[i], i, optionAction, checkboxStates?[i] ?? false,
                    images));
            }
        }

        var menuBar = new ControlGroup(this);
        foreach (var button in popupBox.Button)
        {
            menuBar.AddChild(new Button(this, button,
                () => handleButtonClick(button, 0,null, //_options?.Selected ?? 0, _checkboxes?.Checked ?? null,
                    FormatTextBoxReturn())));
        }

        Controls.Add(menuBar);
    }

    private IDictionary<string, string>? FormatTextBoxReturn()
    {
        return _textBoxes?.Select(box => new { box.Name, Value = box.Text })
            .ToDictionary(k => k.Name, v => v.Value);
    }
}

internal class LabeledTextBox
{
    public string Name { get; set; }
    public string Text { get; set; }
}