using Model.Interface;
using RaylibUI.BasicTypes.Controls;

namespace RaylibUI;

public class CivDialogListBox : ScrollBox
{
    public CivDialogListBox(IControlLayout controller, ListBoxDefinition boxDetails) : base(controller,
        boxDetails.Vertical, 1, MakeLabels(controller, boxDetails), initialSelection: boxDetails.InitialSelection)
    {
        
    }

    private static List<ScrollBoxElement> MakeLabels(IControlLayout controller, ListBoxDefinition boxDetails)
    {
        var iconWidth = -1;
        var rightText = false;
        for (var i = 0; i < boxDetails.Entries.Count; i++)
        {
            var entry = boxDetails.Entries[i];
            if (entry.Icon != null)
            {
                var width = TextureCache.GetImage(entry.Icon).Width;
                if (width > iconWidth)
                {
                    iconWidth = width;
                }
            }

            if (!string.IsNullOrWhiteSpace(entry.RightText))
            {
                rightText = true;
            }
        }

        if (iconWidth > -1)
        {
            return boxDetails.Entries.Select((entry, index) => new ScrollBoxElement(controller,
                    new IControl[]
                    {
                        new IconContainer(controller, entry.Icon, index, iconWidth),
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