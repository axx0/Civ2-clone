using Civ2engine;
using Model;
using Model.Interface;
using Raylib_cs;
using RaylibUI.BasicTypes.Controls;
using RaylibUI.Controls;
using RaylibUI.Forms;
using Button = RaylibUI.Controls.Button;

namespace RaylibUI;

public class CivDialog : BaseDialog
{
    private OptionControl _selectedOption = null;

    private IList<bool>? _checkboxes = null;

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
        Action<string, int, IList<bool>?, 
            IDictionary<string, string>?> handleButtonClick, 
        IList<string>? replaceStrings = null, 
        IList<int>? replaceNumbers = null, IList<bool>? checkboxStates = null, List<TextBoxDefinition>? textBoxDefs = null, int optionsCols = 1, Image[]? icons = null, Image image = new Image(), Forms.ListBox? listbox = null) : 
        base(
            popupBox.Title, new Point(5, 5)) //actionMenuElement.DialogPos)
    {
        if (popupBox.Text?.Count > 0)
        {
            var ftext = Dialog.GetFormattedTexts(popupBox.Text, popupBox.LineStyles, replaceStrings, replaceNumbers);
            foreach (var text in ftext)
            {
                
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

            var optionAction = popupBox.Checkbox ? (Action<OptionControl>) TogggleCheckBox : SetSelectedOption;
            var images = ImageUtils.GetOptionImages(popupBox.Checkbox);

            var optionControls = popupBox.Options.Select((o, i) =>
                new OptionControl(this, o, i, optionAction, checkboxStates?[i] ?? false, images)).ToList();

            if (!popupBox.Checkbox)
            {
                optionControls[0].Checked = true;
                SetSelectedOption(optionControls[0]);
            }
            if (optionsCols < 2)
            {
                optionControls.ForEach(Controls.Add);
            }
            else
            {
                var optionsCount = optionControls.Count;
                var rows = optionsCount % optionsCols == 0
                    ? optionsCount / optionsCols
                    : optionsCount / optionsCols + 1;

                for (var i = 0; i < rows; i++)
                {
                    var optionGroup = new ControlGroup(this);
                    for (var j = 0; j < optionsCols; j++)
                    {
                        var optionIndex = i + j * rows;
                        if (optionIndex < optionControls.Count)
                        {
                            optionGroup.AddChild(optionControls[optionIndex]);
                        }
                    }
                    Controls.Add(optionGroup);
                }
            }
        }

        var menuBar = new ControlGroup(this);
        foreach (var button in popupBox.Button)
        {
            menuBar.AddChild(new Button(this, button,
                () => handleButtonClick(button, _selectedOption.Index, _checkboxes , 
                    FormatTextBoxReturn())));
        }

        Controls.Add(menuBar);
        SetButtons(menuBar);
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