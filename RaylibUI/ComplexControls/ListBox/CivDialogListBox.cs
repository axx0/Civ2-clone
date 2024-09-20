using Model.Interface;
using RaylibUI.BasicTypes.Controls;

namespace RaylibUI;

public class CivDialogListBox : ScrollBox
{
    public CivDialogListBox(IControlLayout controller, ListBoxDefinition boxDetails) : base(controller,
        boxDetails.Vertical, 1, MakeLabels(controller, boxDetails))
    {
    }

    private static IList<BaseControl>? MakeLabels(IControlLayout controller, ListBoxDefinition boxDetails)
    {
        var hasIcons = false;
        var rightText = false;
        for (var i = 0; i < boxDetails.Entries.Count && (!hasIcons || !rightText); i++)
        {
            var entry = boxDetails.Entries[i];
            if (entry.Icon != null)
            {
                hasIcons = true;
            }

            if (!string.IsNullOrWhiteSpace(entry.RightText))
            {
                rightText = true;
            }
        }

        if (hasIcons)
        {
            return boxDetails.Entries.Select((entry, index) => (BaseControl)new DialogLabel(controller, entry, index))
                .ToList();
        }

        if (rightText)
        {

        }

        return boxDetails.Entries
            .Select(e => (BaseControl)new LabelControl(controller, e.LeftText ?? string.Empty, false)).ToList();
    }
}

internal class DialogLabel : ControlGroup
{
    public DialogLabel(IControlLayout controller, ListBoxEntry entry, int index) : base(controller, flexElement: 1, eventTransparent: false)
    {
        AddChild(new IconContainer(controller, entry.Icon, index));
        AddChild(new LabelControl(controller, entry.LeftText, true));
    }
}