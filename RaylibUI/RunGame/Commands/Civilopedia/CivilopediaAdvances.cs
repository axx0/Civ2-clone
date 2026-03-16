using JetBrains.Annotations;
using Model.Controls;
using Model.Input;
using RaylibUI.RunGame.GameControls;

namespace RaylibUI.RunGame.Commands;

[UsedImplicitly]
public class CivilopediaAdvances(GameScreen gameScreen)
    : AlwaysOnCommand(gameScreen, CommandIds.CivilopediaAdvances, [])
{
    public override void Action()
    {
        // ReSharper disable once StringLiteralTypo
        GameScreen.ShowDialog(new CivilopediaWindow(gameScreen, new(CivilopediaInfoType.Advances, CivilopediaWindowType.Listbox)));
    }
}