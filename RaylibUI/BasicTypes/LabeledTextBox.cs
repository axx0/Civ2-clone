using Model.Controls;
using RaylibUI.BasicTypes.Controls;
using RaylibUI.Controls;

namespace RaylibUI.Dialogs;

internal class LabeledTextBox : ControlGroup
{
    private readonly TextBox _textBox;
    private readonly LabelControl _label;
    public string Name { get; set; }
    public string Text => _textBox.Text;

    public LabeledTextBox(IControlLayout controller, TextBoxDefinition textBoxDef, string textBoxLabel, int labelSize) : base(controller, flexElement: NoFlex, eventTransparent: false)
    {
        Name = textBoxDef.Name;
        _label = new LabelControl(controller, textBoxLabel, eventTransparent: true, minWidth: labelSize,
            verticalAlignment: VerticalAlignment.Center, defaultHeight: 38);
        Controls.Add(_label);

        _textBox = new TextBox(controller, textBoxDef.InitialValue, textBoxDef.Width)
        {
            Location = new(labelSize, 0)
        };
        Controls.Add(_textBox);
        Click += OnClick;
    }

    public override int GetPreferredHeight()
    {
        return Math.Max(38, Math.Max(_label.GetPreferredHeight(), _textBox.GetPreferredHeight()));
    }

    public override int GetPreferredWidth()
    {
        return _label.Width + _textBox.Width;
    }

    public override void OnResize()
    {
        var height = GetPreferredHeight();
        _label.Height = height;
        _label.Width = _label.GetPreferredWidth();
        _textBox.Location = new(_label.Width, 0);
        _textBox.Height = height;
    }

    public void OnClick(object? sender, MouseEventArgs args)
    {
        Controller.Focused = _textBox;
    }
}
