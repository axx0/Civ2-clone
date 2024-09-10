namespace RaylibUI;

public class ScrollBoxSelectionEventArgs
{
    private readonly MouseEventArgs _args;

    public ScrollBoxSelectionEventArgs(MouseEventArgs args)
    {
        _args = args;
    }
}