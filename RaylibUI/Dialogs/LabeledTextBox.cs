using Model.Interface;
using RaylibUI.BasicTypes.Controls;
using RaylibUI.Controls;

namespace RaylibUI.Dialogs;

internal class LabeledTextBox : ControlGroup
{
    private readonly TextBox _textBox;
    public string Name { get; set; }
    public string Text => _textBox.Text;

    public LabeledTextBox(IControlLayout controller, TextBoxDefinition textBoxDef, string textBoxLabel, int labelSize) : base(controller, flexElement: NoFlex)
    {
        Name = textBoxDef.Name;
        var label = new LabelControl(controller, textBoxLabel, minWidth: labelSize);
        Children.Add(label);
        _textBox = new TextBox(controller, textBoxDef.InitialValue, textBoxDef.Width);
        Children.Add(_textBox);
    }
}