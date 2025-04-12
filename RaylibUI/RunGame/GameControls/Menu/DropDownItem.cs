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
    public const int DropdownSpacing = 35;
    private const int _paddingLeft = 35;
    private const int _paddingRight = 20;
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
        Children.Add(new LabelControl(dropdownMenu, texts[0].Replace("&", ""), true, defaultHeight: textHeight, font: look.MenuFont, fontSize: look.MenuFontSize, padding: new Padding(0, _paddingLeft, 0, 0)));
        Children.Add(new LabelControl(dropdownMenu, texts.Length > 1 ? texts[1] : string.Empty, true, defaultHeight: textHeight, font: look.MenuFont, fontSize: look.MenuFontSize, alignment: TextAlignment.Right, padding: new Padding(0, 0, 0, _paddingRight)));
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
        if (Controller.Hovered == this)
        {
            Graphics.DrawRectangle((int)Location.X + 1, (int)Location.Y + 1, Width - 2, Height - 2, new Color(145, 201, 247, 255));
        }
        if (_command.GameCommand != null && _command.GameCommand.Checked)
        {
            Graphics.DrawRectangle((int)Location.X + 1, (int)Location.Y + 1, 22, Height - 2, new Color(86, 176, 250, 255));
            Graphics.DrawLine((int)Location.X + 8, (int)Location.Y + 11, (int)Location.X + 11, (int)Location.Y + 14, Color.Black);
            Graphics.DrawLine((int)Location.X + 11, (int)Location.Y + 14, (int)Location.X + 17, (int)Location.Y + 9, Color.Black);
        }
        base.Draw(pulse);
    }
}