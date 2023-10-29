using System.Net.Mime;
using RaylibUI.BasicTypes.Controls;

namespace RaylibUI;

public class ListBoxLabel : LabelControl
{
    public ListBoxLabel(IControlLayout controller, string text, ListBox listBox) : base(controller, text, eventTransparent: false, defaultHeight: 28)
    {
        Click += (_,_) => listBox.LabelClicked(Text);
    }
}