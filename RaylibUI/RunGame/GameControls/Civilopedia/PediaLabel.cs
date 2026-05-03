using RaylibUI.BasicTypes.Controls;

namespace RaylibUI.RunGame.GameControls.Civilopedia;

public class PediaLabel : LabelControl
{
    public PediaLabel(IControlLayout layout, string text, int x, int y) : base(layout, text, true, fontSize: 22)
    {
        Location = new(x, y);
    }
}