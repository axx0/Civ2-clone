using System.Numerics;
using System.Reflection.Emit;
using Raylib_cs;
using RaylibUI.BasicTypes;
using RaylibUI.BasicTypes.Controls;

namespace RaylibUI;

public class ListBox : ScrollBox
{
    public ListBox(IControlLayout controller, bool vertical = false, int? maxColumns = null,
        IList<string>? initialEntries = null) : base(controller, vertical, maxColumns,
        MakeLabels(controller, initialEntries ?? Array.Empty<string>()))
    {
        
    }

    public void SetElements(IList<string> list, bool refresh, List<bool>? valid = null, bool scrollToEnd = false)
    {
        base.SetElements(MakeLabels(Controller, list), refresh, scrollToEnd);
    }

    private static IList<BaseControl> MakeLabels(IControlLayout controller, IList<string> entries)
    {
        return entries.Select(text => new LabelControl(controller, text,  eventTransparent: false, defaultHeight: 28)).OfType<BaseControl>().ToList();
    }
}