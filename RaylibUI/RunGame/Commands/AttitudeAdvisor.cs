using JetBrains.Annotations;
using Model.Controls;
using Model.Input;
using RaylibUI.RunGame.GameControls;

namespace RaylibUI.RunGame.Commands;

[UsedImplicitly]
public class AttitudeAdvisor(GameScreen gameScreen)
    : AlwaysOnCommand(gameScreen, CommandIds.AttitudeAdvisor, [new Shortcut(Key.F4)])
{
    public override void Action()
    {
        // ReSharper disable once StringLiteralTypo
        GameScreen.ShowDialog(new AttitudeAdvisorWindow(gameScreen));
    }
}