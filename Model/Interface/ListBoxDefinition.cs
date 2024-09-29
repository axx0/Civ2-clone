namespace Model.Interface;

public class ListBoxDefinition
{
    /// <summary>
    /// List of entries for the listbox to display
    /// </summary>
    public List<ListBoxEntry> Entries { get; set; }
    
    
    public bool Vertical { get; set; }
    public int InitialSelection { get; set; }
}