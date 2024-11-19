using System.Numerics;
using Model;
using Model.Menu;
using Raylib_CSharp.Colors;
using Raylib_CSharp.Fonts;
using Raylib_CSharp.Interact;
using Raylib_CSharp.Rendering;
using Raylib_CSharp.Transformations;
using RaylibUI.BasicTypes.Controls;

namespace RaylibUI.RunGame.GameControls.Menu;

internal class DropDownItem : ControlGroup
{
    public const int DropdownSpacing = 15;
    private readonly MenuCommand _command;
    private readonly DropdownMenu _dropdownMenu;
    private readonly int _index;
    public override bool CanFocus => true;

    public KeyboardKey HotKey => _command.HotKey;

    public DropDownItem(DropdownMenu dropdownMenu, InterfaceStyle look, MenuCommand command, int index) : base(dropdownMenu,
        DropdownSpacing, NoFlex, eventTransparent: false)
    {
        _command = command;
        _dropdownMenu = dropdownMenu;
        _index = index;
        var texts = command.MenuText.Split("|");
        var textHeight = (int)TextManager.MeasureTextEx(look.MenuFont, texts[0], look.MenuFontSize, 0f).Y;
        Children.Add(new LabelControl(dropdownMenu, texts[0].Replace("&", ""), true, defaultHeight: textHeight, font: look.MenuFont, fontSize: look.MenuFontSize));
        Children.Add(new LabelControl(dropdownMenu, texts.Length > 1 ? texts[1] : string.Empty, true, defaultHeight: textHeight, font: look.MenuFont, fontSize: look.MenuFontSize));
        Click += (_, _) => Activate();
    }

    public override bool OnKeyPressed(KeyboardKey key)
    {
        switch (key)
        {
            case KeyboardKey.Escape:
                _dropdownMenu.Hide();
                return true;
            case KeyboardKey.Enter:
                Activate();
                return true;
        }
        return base.OnKeyPressed(key);
    }

    private void Activate()
    {
        if (_command is { Enabled: true, GameCommand: not null })
        {
            _command.GameCommand?.Action();
            _dropdownMenu.Hide();
        }
    }

    public override void Draw(bool pulse)
    {
        base.Draw(pulse);
        if (Controller.Hovered == this)
        {
            Graphics.DrawRectangleLinesEx(new Rectangle(Location.X +1, Location.Y +1,Width - 2, Height - 2), 0.5f, Color.Black);
        }
    }
}