using Model.Menu;
using Raylib_CSharp.Interact;

namespace Model;

public class DropdownMenuContents
{
    public string Title { get; set; }
    
    public KeyboardKey HotKey { get; set; }
    
    public IList<MenuCommand> Commands { get; set; }
}