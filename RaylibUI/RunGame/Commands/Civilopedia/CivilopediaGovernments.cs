using JetBrains.Annotations;
using Model.Controls;
using Model.Input;
using RaylibUI.RunGame.GameControls;

namespace RaylibUI.RunGame.Commands;

[UsedImplicitly]
public class CivilopediaGovernments(GameScreen gameScreen)
    : AlwaysOnCommand(gameScreen, CommandIds.CivilopediaGovernments, [])
{
    public override void Action()
    {
        // ReSharper disable once StringLiteralTypo
        GameScreen.ShowDialog(new CivilopediaWindow(gameScreen, new(CivilopediaInfoType.Governments, CivilopediaWindowType.Listbox)));
    }
}