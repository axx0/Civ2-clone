namespace RaylibUI;

public class ListBoxSelectionEventArgs : EventArgs
{
    public string Text { get; }

    public ListBoxSelectionEventArgs(string text)
    {
        Text = text;
    }
}