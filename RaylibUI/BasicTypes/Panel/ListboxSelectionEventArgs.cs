namespace RaylibUI;

public class ListboxSelectionEventArgs
{
    public int Index { get; }

    /// <summary>
    /// Selection by soft means don't make final selection based on this
    /// </summary>
    public bool Soft { get; }

    public ListboxSelectionEventArgs(int index, bool soft)
    {
        Index = index;
        Soft = soft;
    }
}