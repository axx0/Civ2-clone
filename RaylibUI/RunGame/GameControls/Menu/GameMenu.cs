using Model;
using Raylib_cs;
using RaylibUI.BasicTypes.Controls;

namespace RaylibUI.RunGame.GameControls.Menu;

public class GameMenu : ControlGroup
{
    private readonly GameScreen _gameScreen;
    private DropdownMenu? _dropdownMenu;
    private readonly List<MenuLabel> _labels;
    public DropdownMenu Dropdown => _dropdownMenu ??= new DropdownMenu(_gameScreen);

    public GameMenu(GameScreen gameScreen, IList<DropdownMenuContents> menus) : base(gameScreen, flexElement: NoFlex, spacing: 12)
    {
        _gameScreen = gameScreen;
        var look = gameScreen.MainWindow.ActiveInterface.Look;

        var textHeight = (int)Raylib.MeasureTextEx(look.MenuFont, "sample", look.MenuFontSize, 0f).Y;
        _labels = menus.Select((menu, index) =>
                              {
                                  var menuLabel = new MenuLabel(gameScreen, look, this, menu, index, textHeight);
                  
                                  return menuLabel;
                              })
                              .ToList();

        Children = _labels.Cast<IControl>().ToList();
    }

    public void Activate(int index)
    {
        if (index == -1)
        {
            _labels[^1].Activate();
        }
        else if (index >= _labels.Count)
        {
            _labels[0].Activate();
        }
        else
        {
            _labels[index].Activate();
        }
    }

    public bool Activate(int index, KeyboardKey hotkey)
    {
        for (int i = index + 1; i != index; i++)
        {
            if (i >= _labels.Count)
            {
                i = -1;
                continue;
            }

            if (_labels[i].Hotkey != hotkey)
            {
                continue;
            }
            
            _labels[i].Activate();
            return true;
        }
        return false;
    }

    public override void Draw(bool pulse)
    {
        Raylib.DrawRectangleRec(Bounds, Color.WHITE);
        foreach (var control in Children)
        {
            if (control == Controller.Focused)
            {
                Raylib.DrawRectangleRec(control.Bounds, new Color(204, 232, 255, 255));
                Raylib.DrawRectangleLinesEx(control.Bounds, 1.0f, new Color(153, 209, 255, 255));
            }
            else if (control == Controller.Hovered)
            {
                Raylib.DrawRectangleRec(control.Bounds, new Color(229, 243, 255, 255));
                Raylib.DrawRectangleLinesEx(control.Bounds, 1.0f, new Color(204, 232, 255, 255));
            }
        }
        base.Draw(pulse);
    }
}