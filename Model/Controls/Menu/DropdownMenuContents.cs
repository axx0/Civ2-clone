namespace Model.Controls;
using Model.Input;

public class DropdownMenuContents
{
    public string Title { get; set; }
    
    public Key HotKey { get; set; }
    
    public required IList<MenuCommand> Commands { get; init; }

    public required int[] SeparatorRows { get; init; }
}