namespace RaylibUI;

public class ScrollBoxSelectionEventArgs
{
    public string Text { get; }
    public int Index { get; }
    
    /// <summary>
    /// Selection by soft means don't make final selection based on this
    /// </summary>
    public bool Soft { get; }

    public ScrollBoxSelectionEventArgs(string text, int index, bool soft)
    {
        Text = text;
        Index = index;
        Soft = soft;
    }
}