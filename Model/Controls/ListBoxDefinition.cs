namespace Model.Controls;

public class ListboxDefinition
{
    public List<ListboxGroup> Groups { get; set; } = [];

    public bool VerticalScrollbar { get; set; } = true;

    /// <summary>
    /// No of rows visible in the box.
    /// </summary>
    public int Rows { get; set; } = 10;

    /// <summary>
    /// Max no of columns that will be visible in the box.
    /// </summary>
    public int Columns { get; set; } = 1;

    /// <summary>
    /// If true, controls are stacked left-to-right.
    /// </summary>
    public bool HorizontalStacking { get; set; } = false;

    /// <summary>
    /// Selected control id.
    /// </summary>
    public int SelectedId { get; set; } = 0;

    /// <summary>
    /// Meaning you can also move through the controls with keys.
    /// </summary>
    public bool Selectable { get; set; } = true;

    /// <summary>
    /// Shift icons left<->right.
    /// </summary>
    public bool ImageShift { get; set; } = false;

    /// <summary>
    /// Defines looks of listbox.
    /// </summary>
    public ListboxType? Type { get; set; } = null;

    public ListboxLooks Looks { get; set; } = new();

    public void Update(IList<string> initialEntries)
    {
        Groups = [];
        foreach (var entry in initialEntries)
        {
            Groups.Add(new ListboxGroup(entry));
        }
    }
}