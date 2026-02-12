using Civ2engine;
using JetBrains.Annotations;
using Model;
using Model.Controls;
using Model.Input;
using RaylibUI.RunGame.GameControls;

namespace RaylibUI.RunGame.Commands;

[UsedImplicitly]
public class ChangeTaxRate(GameScreen gameScreen)
    : AlwaysOnCommand(gameScreen, CommandIds.ChangeTaxRate, [new Shortcut(Key.T, true)])
{
    public override void Action()
    {
        // ReSharper disable once StringLiteralTypo
        GameScreen.ShowDialog(new TaxRateWindow(gameScreen));
    }
}