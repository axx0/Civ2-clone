namespace Model.Controls;

/// <summary>
/// Use this for customizing listbox in dialogs from game.txt.
/// </summary>
public class ListboxDefinition
{
    public List<ListboxGroup>? Groups { get; set; }

    public bool? VerticalScrollbar { get; set; }

    /// <summary>
    /// No of rows visible in the box.
    /// </summary>
    public int? Rows { get; set; }

    /// <summary>
    /// Max no of columns that will be visible in the box.
    /// </summary>
    public int? Columns { get; set; }

    /// <summary>
    /// If true, controls are stacked left-to-right.
    /// </summary>
    public bool? HorizontalStacking { get; set; }

    /// <summary>
    /// Selected control id.
    /// </summary>
    public int? SelectedId { get; set; }

    /// <summary>
    /// Meaning you can also move through the controls with keys.
    /// </summary>
    public bool? Selectable { get; set; }

    /// <summary>
    /// Defines looks of listbox.
    /// </summary>
    public ListboxType? Type { get; set; }

    public ListboxLooks? Looks { get; set; }

    public void Update(IList<string> initialEntries)
    {
        Groups = [];
        foreach (var entry in initialEntries)
        {
            Groups.Add(new ListboxGroup(entry));
        }
    }
}