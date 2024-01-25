using Model.Menu;
using Raylib_cs;

namespace Model;

public interface IGameCommand
{
    string Id { get; }
    Shortcut KeyCombo { get; set; }
    CommandStatus Status { get; }
    void Update();
    void Action();
    
    MenuCommand? Command { get; set; } 
    
    string ErrorDialog { get; }
    string? Name { get; }
}

public enum CommandStatus
{
    Priority,
    Normal,
    Default,
    Disabled,
    Invalid
}