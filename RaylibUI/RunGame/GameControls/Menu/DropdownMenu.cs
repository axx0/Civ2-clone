using System.Numerics;
using Civ2engine.IO;
using Model;
using Model.Menu;
using Raylib_cs;
using RaylibUI.BasicTypes.Controls;

namespace RaylibUI.RunGame.GameControls.Menu;

public class DropdownMenu :  BaseDialog
{
    private bool _shown;
    private readonly GameScreen _gameScreen;
    private int _current = -1;
    private IUserInterface _active;

    public DropdownMenu(GameScreen gameScreen) : base(gameScreen.Main)
    {
        _gameScreen = gameScreen;
        _active = gameScreen.MainWindow.ActiveInterface;
        MenuBar = gameScreen.MenuBar;
    }

    public int Current => _shown ? _current : -1;

    public void Show(Vector2 location, int menuIndex, IEnumerable<MenuCommand> elements)
    {
        Location = location;
        _current = menuIndex;
        Controls.Clear();
        var width = new List<int>{ 20,10};
        foreach (var command in elements)
        {
            command.GameCommand?.Update();
            var dropDownItem = new DropDownItem(this, _active.Look, command,  Controls.Count);
            Controls.Add( dropDownItem);
            
            dropDownItem.GetPreferredWidth();
            var itemWidths = dropDownItem.ChildWidths;
            if (width[0] < itemWidths[0])
            {
                width[0] = itemWidths[0];
            }

            if (width[1] < itemWidths[1])
            {
                width[1] = itemWidths[1];
            }
        }

        var dropdownWidth = width.Sum() + DropDownItem.DropdownSpacing;
        var currentY = location.Y + 3;
        foreach (var menuItem in Controls.OfType<DropDownItem>())
        {
            var height = menuItem.GetPreferredHeight() + 12;
            menuItem.SetChildWidths(width);
            menuItem.Bounds = new Rectangle(location.X + 3, currentY, dropdownWidth, height);
            menuItem.OnResize();
            currentY += height;
        }

        Width = dropdownWidth + 6;
        Height = currentY - location.Y + 3;
        _gameScreen.ShowDialog(this,true);
        _shown = true;
    }

    public float Height { get; set; }

    public int Width { get; set; }
    public GameMenu MenuBar { get; }

    public override void OnKeyPress(KeyboardKey key)
    {
        switch (key)
        {
            case KeyboardKey.KEY_LEFT:
                MenuBar.Activate(_current - 1);
                return;
            case KeyboardKey.KEY_RIGHT:
                MenuBar.Activate(_current + 1);
                return;
            case KeyboardKey.KEY_DOWN:
                if (Focused == null)
                {
                    Focused = Controls[0];
                }
                else if (Focused == Controls[^1])
                {
                    Focused = null;
                }
                else
                {
                    var idx = Controls.IndexOf(Focused);
                    Focused = Controls[idx + 1];
                }

                return;
            case KeyboardKey.KEY_UP:
                if (Focused == null)
                {
                    Focused = Controls[^1];
                }
                else if (Focused == Controls[0])
                {
                    Focused = null;
                }
                else
                {
                    var idx = Controls.IndexOf(Focused);
                    Focused = Controls[idx - 1];
                }

                return;
        }




        if (Focused == null)
        {
            var hotControl = Controls.FirstOrDefault(c => c is DropDownItem dd && dd.HotKey == key);
            if (hotControl != null)
            {
                Focused = hotControl;
                return;
            }
        }
        else
        {
            var idx = Controls.IndexOf(Focused);
            for (int i = idx +1; i != idx; i++)
            {
                if (i >= Controls.Count)
                {
                    i = -1;
                    continue;
                }

                if (Controls[i] is DropDownItem dd && dd.HotKey == key)
                {
                    Focused = Controls[i];
                    return;
                }
            }

            if (Controls[idx] is DropDownItem cdd && cdd.HotKey == key)
            {
                //Do nothing as they pressed the hotkey for this element and there is no conflict
                return;
            }
        }

        if (MenuBar.Activate(_current, key))
        {
            return;
        }   
        base.OnKeyPress(key);
    }

    public override void Resize(int width, int height)
    {
    }

    public override void Draw(bool pulse)
    {
        if (!_shown || Controls.Count == 0) return;

        Raylib.DrawRectangleV(Location, new Vector2(Width, Height), new Color(242, 242, 242, 255));
        Raylib.DrawRectangleLines((int)Location.X, (int)Location.Y, Width, (int)Height, new Color(204, 204, 204, 255));
        
        foreach (var control in Controls)
        {
            if (Focused == control)
            {
                Raylib.DrawRectangleRec(new Rectangle(control.Location.X, control.Location.Y, control.Width, control.Height), new Color(145, 201, 247, 255));
            }
            control.Draw(pulse);
        }
    }

    public void Hide()
    {
        _shown = false;
        _gameScreen.CloseDialog(this);
    }
}