using Model.Input;

namespace Model.Menu;

public class DropdownMenuContents
{
    public string Title { get; set; }
    
    public Key HotKey { get; set; }
    
    public required IList<MenuCommand> Commands { get; init; }

    public required int[] SeparatorRows { get; init; }
}