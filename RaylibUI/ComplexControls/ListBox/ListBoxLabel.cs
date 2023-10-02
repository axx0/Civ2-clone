using RaylibUI.BasicTypes.Controls;

namespace RaylibUI;

public class ListBoxLabel : LabelControl
{
    private readonly ListBox _listBox;

    public ListBoxLabel(IControlLayout controller, string text, ListBox listBox) : base(controller, text, eventTransparent: false, defaultHeight: 28)
    {
        _listBox = listBox;
    }

    public override void OnClick()
    {
        _listBox.LabelClicked(_text);
    }
}