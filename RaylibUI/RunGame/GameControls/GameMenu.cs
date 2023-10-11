using Raylib_cs;
using RaylibUI.BasicTypes.Controls;

namespace RaylibUI.RunGame.GameControls;

public class GameMenu : ControlGroup
{
    public GameMenu(GameScreen controller) : base(controller, flexElement: NoFlex)
    {
        var items = controller.MainWindow.ActiveInterface.GetMenuItems();
        this.Children = items.Select(l => (IControl)new LabelControl(controller, l, false, font: Fonts.AlternativeFont)).ToList();
    }

    public override void Draw(bool pulse)
    {
        Raylib.DrawRectangleRec(Bounds, Color.GRAY);
        base.Draw(pulse);
    }
}