using JetBrains.Annotations;
using Model.Controls;
using Model.Input;
using RaylibUI.RunGame.GameControls;

namespace RaylibUI.RunGame.Commands;

[UsedImplicitly]
public class CivilopediaWonders(GameScreen gameScreen)
    : AlwaysOnCommand(gameScreen, CommandIds.CivilopediaWonders, [])
{
    public override void Action()
    {
        // ReSharper disable once StringLiteralTypo
        GameScreen.ShowDialog(new CivilopediaWindow(gameScreen, new(CivilopediaInfoType.Wonders, CivilopediaWindowType.Listbox)));
    }
}