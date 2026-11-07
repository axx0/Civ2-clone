using System.Numerics;
using Model;
using Model.Controls;
using Raylib_CSharp.Collision;
using Raylib_CSharp.Colors;
using Raylib_CSharp.Interact;
using Raylib_CSharp.Rendering;
using Raylib_CSharp.Transformations;

namespace RaylibUI.RunGame.GameControls.Menu;

public class DropdownMenu(GameScreen gameScreen) : BaseDialog(gameScreen.Main)
{
    private bool _shown;
    private int _current = -1;
    private readonly IUserInterface _active = gameScreen.MainWindow.ActiveInterface;
    private readonly List<int> _separatorOffsets = [];
    private int _width, _height;

    /// <summary>
    /// Index of currently selected dropdown menu (-1 = no menu selected)
    /// </summary>
    public int Current => _shown ? _current : -1;

    public void Show(Vector2 location, int menuIndex, IEnumerable<MenuCommand> elements, int[] separatorRows)
    {
        _separatorOffsets.Clear();
        Location = location;
        _current = menuIndex;
        Controls.Clear();
        var childWidths = new List<int>{ 20,10};
        foreach (var command in elements)
        {
            command.Enabled = command.GameCommand?.Update() ?? false;
            var dropDownItem = new DropDownItem(this, _active.Look, command,  Controls.Count);
            Controls.Add( dropDownItem);
            
            dropDownItem.GetPreferredWidth();
            var itemWidths = dropDownItem.ChildWidths;
            if (childWidths[0] < itemWidths[0])
            {
                childWidths[0] = itemWidths[0];
            }

            if (childWidths[1] < itemWidths[1])
            {
                childWidths[1] = itemWidths[1];
            }
        }

        var dropdownWidth = childWidths.Sum() + DropDownItem.DropdownSpacing;
        var currentY = 3;
        int itemNo = 0;
        foreach (var menuItem in Controls.OfType<DropDownItem>())
        {
            var height = menuItem.GetPreferredHeight() + 8;
            menuItem.SetChildWidths(childWidths);
            menuItem.Location = new(0, currentY);
            menuItem.Width = dropdownWidth;
            menuItem.Height = height;
            menuItem.OnResize();
            currentY += height;
            if (separatorRows != null && separatorRows.Contains(itemNo))
            {
                currentY += 7;
                _separatorOffsets.Add(currentY - 8);
            }
            itemNo++;
        }

        _width = dropdownWidth;
        _height = (int)(currentY - location.Y + 10);
        // Switching menus re-uses this dialog; it must only be on the dialog stack once
        if (!_shown)
        {
            gameScreen.ShowDialog(this, true);
        }
        _shown = true;
        // A menu opens with nothing selected (the old selection belongs to the previous menu's items,
        // and ShowDialog's starting focus doesn't apply to menus)
        Focused = null;
    }

    // Mouse outside the dropdown's items (while open this dialog gets all mouse input)

    /// <summary>
    /// Mouse down on the open menu's title closes it, on another title opens that one.
    /// Anywhere else (outside the dropdown) the menu closes and the click still acts on what's underneath.
    /// </summary>
    public override bool OnMouseDownOutside(MouseEventArgs e)
    {
        var title = MenuBar.TitleIndexAt(e.ScreenLocation);
        if (title == _current)
        {
            Hide();
            return false;
        }
        if (title != -1)
        {
            MenuBar.Open(title);
            return false;
        }
        if (ShapeHelper.CheckCollisionPointRec(e.ScreenLocation, Bounds))
        {
            return false;
        }

        Hide();
        return true;
    }

    /// <summary>
    /// Moving over another menu title switches to that menu.
    /// </summary>
    public override void OnMouseMoveOutside(MouseEventArgs e)
    {
        var title = MenuBar.TitleIndexAt(e.ScreenLocation);
        if (title != -1 && title != _current)
        {
            MenuBar.Open(title);
        }
    }

    /// <summary>
    /// Releasing over a menu title keeps the menu open (normal click on a title);
    /// releasing anywhere else outside the dropdown closes it.
    /// </summary>
    public override void OnMouseUpOutside(MouseEventArgs e)
    {
        if (MenuBar.TitleIndexAt(e.ScreenLocation) != -1 ||
            ShapeHelper.CheckCollisionPointRec(e.ScreenLocation, Bounds))
        {
            return;
        }

        Hide();
    }

    public override int Height => _height;
    public override int Width => _width;


    public GameMenu MenuBar { get; } = gameScreen.MenuBar;

    public override void OnKeyPress(KeyboardKey key)
    {
        switch (key)
        {
            case KeyboardKey.Left:
                MenuBar.Activate(_current - 1);
                return;
            case KeyboardKey.Right:
                MenuBar.Activate(_current + 1);
                return;
            case KeyboardKey.Down:
            {
                // Wraps around from the last item to the first
                if (Controls.Count == 0) return;
                var idx = Focused == null ? -1 : Controls.IndexOf(Focused);
                Focused = Controls[(idx + 1) % Controls.Count];
                return;
            }
            case KeyboardKey.Up:
            {
                // Wraps around from the first item to the last
                if (Controls.Count == 0) return;
                var idx = Focused == null ? -1 : Controls.IndexOf(Focused);
                Focused = idx <= 0 ? Controls[^1] : Controls[idx - 1];
                return;
            }
        }

        // Letters do nothing in an open menu, except Alt+letter, which opens the menu with that title letter
        // (Alt has already opened the first menu by the time the letter arrives)
        if (Input.IsKeyDown(KeyboardKey.LeftAlt) || Input.IsKeyDown(KeyboardKey.RightAlt))
        {
            if (MenuBar.Activate(_current, key))
            {
                return;
            }
        }

        base.OnKeyPress(key);
    }

    public override void Resize(int width, int height)
    {
    }

    public override void Draw(bool pulse)
    {
        if (!_shown || Controls.Count == 0) return;

        Graphics.DrawRectangleRec(Bounds, new Color(242, 242, 242, 255));
        Graphics.DrawRectangleLinesEx(Bounds, 1f, new Color(204, 204, 204, 255));

        foreach (var control in Controls)
        {
            if (Focused == control)
            {
                Graphics.DrawRectangleRec(control.Bounds, new Color(145, 201, 247, 255));
            }
            control.Draw(pulse);
        }

        foreach(var offset in _separatorOffsets)
        {
            Graphics.DrawLine((int)Bounds.X, (int)Bounds.Y + offset, (int)Bounds.X + Width, (int)Bounds.Y + offset, new Color(215, 215, 215, 255));
        }
    }

    public void Hide()
    {
        _shown = false;
        gameScreen.CloseDialog(this);
        // Give the focus back to where it was before the menu opened (e.g. the map)
        gameScreen.Focused = MenuBar.FocusBeforeMenu;
        MenuBar.FocusBeforeMenu = null;
    }
}
