using Model.Dialog;
using Model.Images;
using Model.Menu;
using Raylib_CSharp;

namespace Model;

public interface IGameCommand
{
    string Id { get; }
    Shortcut[] ActivationKeys { get; set; }
    CommandStatus Status { get; }
    bool Update();
    void Action();
    bool Checked { get; }
    
    MenuCommand? Command { get; set; } 
    
    string ErrorDialog { get; }
    DialogImageElements? ErrorImage { get; }
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