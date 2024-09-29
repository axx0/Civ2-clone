using Model.Interface;
using RaylibUI.BasicTypes.Controls;

namespace RaylibUI;

public class CivDialogListBox : ScrollBox
{
    public CivDialogListBox(IControlLayout controller, ListBoxDefinition boxDetails) : base(controller,
        boxDetails.Vertical, 1, MakeLabels(controller, boxDetails))
    {
    }

    private static List<ScrollBoxElement> MakeLabels(IControlLayout controller, ListBoxDefinition boxDetails)
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
            return boxDetails.Entries.Select((entry, index) => new ScrollBoxElement(controller,
                    new IControl[]
                    {
                        new IconContainer(controller, entry.Icon, index),
                        new LabelControl(controller, entry.LeftText ?? "", true)
                    }, flexElement: 1))
                .ToList();
        }

        if (rightText)
        {

        }

        return boxDetails.Entries.Select((entry, index) => new ScrollBoxElement(controller,
                new IControl[]
                {
                    new LabelControl(controller, entry.LeftText ?? "", true)
                } ))
            .ToList();
    }
}