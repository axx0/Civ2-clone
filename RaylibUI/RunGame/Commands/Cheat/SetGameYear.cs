using Model.Input;
using Civ2engine.IO;
using Model.Core;
using Model.Controls;
using Raylib_CSharp.Logging;

namespace RaylibUI.RunGame.Commands.Cheat;

public class SetGameYear(GameScreen gameScreen) : AlwaysOnCommand(gameScreen, CommandIds.CheatSetGameYear, [new Shortcut(Key.F4, shift: true)])
{
    private CivDialog? _setGameYearDialog;
    private const string NewTurnNumber = "New Turn Number";

    public override void Action()
    {
        _setGameYearDialog = BuildSetGameYearDialog(GameScreen.Game.TurnNumber);
        GameScreen.ShowDialog(_setGameYearDialog);
    }

    private CivDialog BuildSetGameYearDialog(int currentTurn)
    {
        String dialogTitle = $"Change # Of Turns Elapsed";
        String subtext = $"# Turns Elapsed: {currentTurn}.";

        TextBoxDefinition textBox = new TextBoxDefinition
        {
            Index = 0,
            InitialValue = currentTurn.ToString(),
            Description = "New # turns:",
            MinValue = int.MinValue,
            CharLimit = 11,
            Name = NewTurnNumber,
            Width = 225
        };

        PopupBox popupBox = new PopupBox
        {
            Title = dialogTitle,
            Button = [Labels.Ok, Labels.Cancel],
            Text = new List<string> { subtext },
            LineStyles = new List<TextStyles> { TextStyles.LeftOwnLine }
        };

        return new CivDialog(
            GameScreen.Main, new DialogElements(popupBox) { TextBoxes = new List<TextBoxDefinition> { textBox } }, 
            DoChangeYear);
    }

    private void DoChangeYear(string button, int selection, IList<bool>? arg3, IDictionary<string, string>? textBoxes)
    {
        {
            if (textBoxes != null && button == Labels.Ok &&
                textBoxes.TryGetValue(NewTurnNumber, out string? newTurnNumber))
            {
                if (int.TryParse(newTurnNumber, out int parsedTurnNumber))
                {
                    GameScreen.Game.TurnNumber = parsedTurnNumber;
                }
            }
            GameScreen.CloseDialog(_setGameYearDialog);
            GameScreen.StatusPanel.Update();
        }
    }
}