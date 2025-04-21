using System.Numerics;
using Model;
using Raylib_CSharp.Interact;
using RaylibUI.BasicTypes.Controls;

namespace RaylibUI.RunGame.GameControls.Menu;

public class MenuLabel : LabelControl
{
    private readonly GameMenu _gameMenu;
    private bool _active;
    private readonly DropdownMenuContents _menuElements;

    public override bool CanFocus => true;

    public MenuLabel(IControlLayout controller, InterfaceStyle look, GameMenu gameMenu, DropdownMenuContents contents, int index, int textHeight) :
        base(controller, contents.Title.Replace("&", ""), false, defaultHeight: textHeight, font: look.MenuFont, fontSize: look.MenuFontSize)
    {
        Hotkey = contents.HotKey;
        _menuElements = contents;
        Index = index;
        _gameMenu = gameMenu;
        Click += (_, _) => Activate();
    }

    public KeyboardKey Hotkey { get; set; }

    public override void OnFocus()
    {
        base.OnFocus();
        Activate();
    }

    public override void OnMouseEnter()
    {
        base.OnMouseEnter();
        if (Input.IsMouseButtonDown(MouseButton.Left))
        {
            Activate();
        }
    }

    public void Activate()
    {
        if (_gameMenu.Dropdown.Current == Index) return;

        Controller.Focused = this;
        Controller.Hovered = this;
        _gameMenu.Dropdown.Show(Location with { Y = Location.Y + Height }, Index,
            _menuElements.Commands, _menuElements.SeparatorRows);
    }

    public int Index { get; }

    public override void OnMouseMove(Vector2 moveAmount)
    {
        base.OnMouseMove(moveAmount);
        if (Input.IsMouseButtonDown(MouseButton.Left))
        {
            //Activate();
        }
    }

    public override bool OnKeyPressed(KeyboardKey key)
    {
        if (_gameMenu.Children != null)
        {
            switch (key)
            {
                case KeyboardKey.Left:
                    Controller.Focused = this.Index > 0 ? _gameMenu.Children[Index - 1] : _gameMenu.Children[^1];
                    return true;
                case KeyboardKey.Right:
                    Controller.Focused = Index < _gameMenu.Children.Count - 1
                        ? _gameMenu.Children[Index + 1]
                        : _gameMenu.Children[0];
                    return true;
                case KeyboardKey.Escape:
                    _gameMenu.Dropdown.Hide();
                    return true;
                case KeyboardKey.Enter:
                    Activate();
                    return true;
                case KeyboardKey.Up:
                case KeyboardKey.Down:
                    Activate();
                    Controller.Focused = _gameMenu.Dropdown.Controls[key == KeyboardKey.Up ? ^1 : 0];
                    return true;
            }

            for (int i = Index + 1; i != Index; i++)
            {
                if (i >= _gameMenu.Children.Count)
                {
                    i = -1;
                    continue;
                }

                if (_gameMenu.Children[i] is MenuLabel label && label.Hotkey == key)
                {
                    Controller.Focused = label;
                    return true;
                }
            }
        }

        return base.OnKeyPressed(key);
    }

    public override void Draw(bool pulse)
    {
        base.Draw(pulse);
        //if (Controller.Focused == this)
        //{
        //    Graphics.DrawRectangleLinesEx(new Rectangle(Location.X + 1, Location.Y + 1, Width - 2, Height - 2), 0.5f, Color.Black);
        //}
    }
}