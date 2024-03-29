using Model.Interface;
using RaylibUI.BasicTypes.Controls;
using RaylibUI.Controls;

namespace RaylibUI.Dialogs;

internal class LabeledTextBox : ControlGroup
{
    private readonly TextBox _textBox;
    public string Name { get; set; }
    public string Text => _textBox.Text;

    public LabeledTextBox(IControlLayout controller, TextBoxDefinition textBoxDef, string textBoxLabel, int labelSize) : base(controller, flexElement: NoFlex, eventTransparent: false)
    {
        Name = textBoxDef.Name;
        var label = new LabelControl(controller, textBoxLabel, eventTransparent: true, minWidth: labelSize);
        Children.Add(label);
        _textBox = new TextBox(controller, textBoxDef.InitialValue, textBoxDef.Width);
        Children.Add(_textBox);
        Click += OnClick;
    }

    public void OnClick(object? sender, MouseEventArgs args)
    {
        Controller.Focused = _textBox;
    }
}