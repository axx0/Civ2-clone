using JetBrains.Annotations;
using Model.Controls;
using Model.Input;
using RaylibUI.RunGame.GameControls;
using RaylibUI.RunGame.GameControls.Civilopedia;

namespace RaylibUI.RunGame.Commands;

[UsedImplicitly]
public class MasterControl(GameScreen gameScreen)
    : AlwaysOnCommand(gameScreen, CommandIds.CheatMasterControl, [])
{
    public override void Action()
    {
        // ReSharper disable once StringLiteralTypo
        GameScreen.ShowDialog(new MasterCheatDialog(gameScreen));
    }
}