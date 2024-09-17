namespace RaylibUI;

public class ScrollBoxSelectionEventArgs
{
    public int Index { get; }
    
    private readonly MouseEventArgs _args;

    public ScrollBoxSelectionEventArgs(MouseEventArgs args, int index)
    {
        Index = index;
        _args = args;
    }
}