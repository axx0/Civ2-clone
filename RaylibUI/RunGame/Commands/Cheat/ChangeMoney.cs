using Model.Input;
using System.Diagnostics;
using Civ2engine;
using Microsoft.VisualBasic.CompilerServices;
using Model.Core;
using Model.Interface;
using Model.Menu;
using Raylib_CSharp.Interact;
using Point = Model.Point;

namespace RaylibUI.RunGame.Commands.Cheat;

public class ChangeMoney(GameScreen gameScreen) : AlwaysOnCommand(gameScreen, CommandIds.CheatChangeMoneyCommand, [new Shortcut(Key.F9, shift: true)])
{
    private CivDialog? _selectTribeDialog;
    private CivDialog? _enterNewMoneyDialog;
    private const string NewTreasury = "New Treasury";
    private Civilization? targetCiv;

    public override void Action()
    {
        var allLabel = Labels.For(LabelIndex.EntireMap);
        var noSpecial = Labels.For(LabelIndex.NoSpecialView);
        
        _selectTribeDialog = new CivDialog(GameScreen.Main, new PopupBox
        {
            Title = "Pick A Player",
            Options = GameScreen.Game.AllCivilizations
                .FindAll(c=>c.PlayerType != PlayerType.Barbarians)
                .Select(c=>c.Adjective)
                .ToArray(),
            Button = [Labels.Ok, Labels.Cancel]
        }, new Point(0,0), HandleClickSelectTribe );
        
        GameScreen.ShowDialog(_selectTribeDialog);
    }

    private void HandleClickSelectTribe(string button, int selection, IList<bool>? arg3, IDictionary<string, string>? arg4)
    {
        if (button == Labels.Ok)
        {
            var civId = selection + 1;
            targetCiv = gameScreen.Game.AllCivilizations.Find(civ => civ.Id == civId && civ.PlayerType != PlayerType.Barbarians);
            Debug.Assert(targetCiv != null, nameof(targetCiv) + " != null");

            _enterNewMoneyDialog = BuildChangeMoneyDialog(targetCiv.Money, targetCiv.Adjective);
            GameScreen.CloseDialog(_selectTribeDialog);
            GameScreen.ShowDialog(_enterNewMoneyDialog);
        }
        else
        {
            GameScreen.CloseDialog(_selectTribeDialog);
        }
    }

    private CivDialog BuildChangeMoneyDialog(int currentMoney, String adjective)
    {
        String dialogTitle = $"Change {adjective} Treasury";
        String subtitle = $"{adjective} treasury currently {currentMoney} Gold";
        
        TextBoxDefinition textBox = new TextBoxDefinition
        {
            Index = 0,
            InitialValue = currentMoney.ToString(),
            Description = "New treasury:",
            MinValue = 0,
            Name = NewTreasury,
            Width = 225
        };

        PopupBox popupBox = new PopupBox
        {
            Title = dialogTitle,
            Button = [Labels.Ok, Labels.Cancel],
            Text = new List<string> { subtitle },
            LineStyles = new List<TextStyles> { TextStyles.LeftOwnLine }
        };

        return new CivDialog(
            GameScreen.Main, popupBox, new Point(0, 0), DoChangeMoney,
            textBoxDefs: new List<TextBoxDefinition> { textBox });
    }

    private void DoChangeMoney(string button, int selectedIndex, IList<bool>? check,
        IDictionary<string, string>? textBoxes)
    {
        if (textBoxes != null && button == Labels.Ok && textBoxes.TryGetValue(NewTreasury, out var newMoney) &&
            targetCiv != null)
        {
            targetCiv.Money = IntegerType.FromString(newMoney);
        }
        GameScreen.CloseDialog(_enterNewMoneyDialog);
        GameScreen.StatusPanel.Update();
    }
}