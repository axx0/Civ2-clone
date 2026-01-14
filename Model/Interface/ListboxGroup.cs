namespace Model.Interface;

/// <summary>
/// Groups of texts and images in listbox.
/// </summary>
public class ListboxGroup
{
    public List<ListboxGroupElement> Elements { get; set; }
    public int? Height { get; set; } = null;
    public int? Width { get; set; } = null;
    
    public ListboxGroup() { }

    public ListboxGroup(string entry) 
    {
        Elements = [new ListboxGroupElement { Text = entry }];
    }
}
