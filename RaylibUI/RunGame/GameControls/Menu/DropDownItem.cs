using System.Numerics;
using Model;
using Model.Menu;
using Raylib_cs;
using RaylibUI.BasicTypes.Controls;

namespace RaylibUI.RunGame.GameControls.Menu;

internal class DropDownItem : LabelControl
{
    private readonly MenuCommand _command;
    private readonly DropdownMenu _dropdownMenu;
    private readonly int _index;
    public override bool CanFocus => true;

    public KeyboardKey HotKey => _command.HotKey;

    public DropDownItem(DropdownMenu dropdownMenu, MenuCommand command, int index) : base(dropdownMenu,
        command.MenuText, false)
    {
        _command = command;
        _dropdownMenu = dropdownMenu;
        _index = index;
    }

    public override bool OnKeyPressed(KeyboardKey key)
    {
        var gameMenu = _dropdownMenu.MenuBar;
        switch (key)
        {
            case KeyboardKey.KEY_ESCAPE:
                gameMenu.Dropdown.Hide();
                return true;
            case KeyboardKey.KEY_ENTER:
                if (_command is { Enabled: true, GameCommand: not null })
                {
                    _command.GameCommand?.Action();
                    
                }
                return true;
        }
        return base.OnKeyPressed(key);
    }
    
    public override void Draw(bool pulse)
    {
        base.Draw(pulse);
        if (Controller.Focused == this)
        {
            Raylib.DrawRectangleLinesEx(new Rectangle(Location.X +1, Location.Y +1,Width - 2, Height - 2), 0.5f, Color.BLACK);
        }
    }
}