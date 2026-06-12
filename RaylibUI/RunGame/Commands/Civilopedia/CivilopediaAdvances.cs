using JetBrains.Annotations;
using Model.Controls;
using Model.Input;
using RaylibUI.RunGame.GameControls;
using RaylibUI.RunGame.GameControls.Civilopedia;

namespace RaylibUI.RunGame.Commands;

[UsedImplicitly]
public class CivilopediaAdvances(GameScreen gameScreen)
    : AlwaysOnCommand(gameScreen, CommandIds.CivilopediaAdvances, [])
{
    public override void Action()
    {
        // ReSharper disable once StringLiteralTypo
        GameScreen.ShowDialog(new CivilopediaWindow(GameScreen, new(CivilopediaInfoType.Advances, CivilopediaWindowType.Listbox)));
    }
}