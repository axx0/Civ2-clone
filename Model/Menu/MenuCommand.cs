using Raylib_cs;

namespace Model.Menu;

public class MenuCommand
{
    public MenuCommand(string text, KeyboardKey hotKey, Shortcut shortcut, IGameCommand? gameCommand)
    {
        HotKey = hotKey;
        _defaultText = text;
        _menuText = _defaultText;
        GameCommand = gameCommand;
        
        if (gameCommand == null) return;
        
        gameCommand.KeyCombo = shortcut;
        gameCommand.Command = this;
    }
    
    private readonly string _defaultText;
    private string _menuText;

    public string MenuText
    {
        get => _menuText;
        set => _menuText = string.IsNullOrWhiteSpace(value) ? _defaultText : value;
    }

    public bool Enabled { get; set; }
    
    public IGameCommand? GameCommand { get; }
    public KeyboardKey HotKey { get; set; }
}