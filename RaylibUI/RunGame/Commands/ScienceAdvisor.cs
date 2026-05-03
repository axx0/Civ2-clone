using JetBrains.Annotations;
using Model.Controls;
using Model.Input;
using RaylibUI.RunGame.GameControls.Advisors;

namespace RaylibUI.RunGame.Commands;

[UsedImplicitly]
public class ScienceAdvisor(GameScreen gameScreen)
    : AlwaysOnCommand(gameScreen, CommandIds.ScienceAdvisor, [new Shortcut(Key.F6)])
{
    public override void Action()
    {
        GameScreen.ShowDialog(new ScienceAdvisorWindow(GameScreen));
    }
}