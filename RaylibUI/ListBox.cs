using System.Numerics;
using Raylib_cs;

namespace RaylibUI;

public class ListBox : BaseControl
{
    private List<string> _list;
    private int _selectedElement;
    private List<bool> _valid;
    public int ColumnWidth { get; }

    public ListBox(IControlLayout controller, int columnWidth) : base(controller)
    {
        ColumnWidth = columnWidth;
        Height = 10 * (Styles.BaseFontSize + 10);
    }

    public override Size GetPreferredSize(int width, int height)
    {
        return new Size(ColumnWidth, Height);
    }

    public void SetElements(List<string> list, List<bool> valid)
    {
        _list = list;
        _valid = valid;
        _selectedElement = -1;
    }

    public override void Draw(bool pulse)
    {
        base.Draw(pulse);

        var startX = Location.X;
        var startY = Location.Y;
        for (int i = 0; i < _list.Count; i++)
        {
            Raylib.DrawTextEx(Raylib.GetFontDefault(), _list[i], new Vector2( startX + 5, 5 + startY + 20 * (i + 0)), 20, 1f, Color.BLACK);
        }
    }

    public override void OnMouseMove(Vector2 moveAmount)
    {
        base.OnMouseMove(moveAmount);
    }
}