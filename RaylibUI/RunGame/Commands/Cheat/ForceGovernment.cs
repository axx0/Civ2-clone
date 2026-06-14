using System.Diagnostics;
using Model.Input;
using Civ2engine.IO;
using Model.Core;
using Model.Controls;
using Model.Core.GameRules;

namespace RaylibUI.RunGame.Commands.Cheat;

// TODO:
// This functionally changes the player's government.
// But doesn't include the Tax Rate or "$LEADER proclaimed $TITLE of ..." newspaper dialog.
// After we implement the Revolution / standard government changes in normal gameplay, this cheat should reuse those dialogs.
public class ForceGovernment(GameScreen gameScreen) : AlwaysOnCommand(gameScreen, CommandIds.CheatForceGovernment, [new Shortcut(Key.F7, shift: true)])
{
    private CivDialog? _selectTribeDialog;
    private CivDialog? _chooseGovtDialog;
    private Civilization? _targetCiv;
    private Government[] Governments => GameScreen.Game.Rules.Governments;

    public override void Action()
    {
        // TODO: Refactor "Pick A Player" selector as a common helper (also used by ChangeMoney, Edit King, etc.)
        _selectTribeDialog = new CivDialog(GameScreen.Main, new DialogElements(new PopupBox
        {
            Title = "Pick A Player",
            Options = GameScreen.Game.AllCivilizations
                .FindAll(c => c.Alive && c.PlayerType != PlayerType.Barbarians)
                .Select(c => c.Adjective)
                .ToArray(),
            Button = [Labels.Ok, Labels.Cancel]
        }), HandleClickSelectTribe);

        GameScreen.ShowDialog(_selectTribeDialog);
    }

    private void HandleClickSelectTribe(string button, int selection, IList<bool>? arg3, IDictionary<string, string>? arg4)
    {
        if (button == Labels.Ok)
        {
            var civId = selection + 1;
            _targetCiv = gameScreen.Game.AllCivilizations.Find(civ => civ.Id == civId && civ.PlayerType != PlayerType.Barbarians);
            Debug.Assert(_targetCiv != null, nameof(_targetCiv) + " != null");

            _chooseGovtDialog = BuildChooseGovtDialog(_targetCiv.Government);
            GameScreen.CloseDialog(_selectTribeDialog);
            GameScreen.ShowDialog(_chooseGovtDialog);
        }
        else
        {
            GameScreen.CloseDialog(_selectTribeDialog);
        }
    }

    private CivDialog BuildChooseGovtDialog(int currentGovernment)
    {
        var governments = new PopupBox
        {
            Title = "Select Type of Government",
            Options = Governments.Select(g => g.Name).ToArray(),
            Default = currentGovernment,
            Button = [Labels.Ok, Labels.Cancel]
        };
        
        return new CivDialog(GameScreen.Main, new DialogElements(governments), HandleChooseGovt);
    }

    private void HandleChooseGovt(string button, int selectedIndex, IList<bool>? check,
        IDictionary<string, string>? textBoxes)
    {
        if (button == Labels.Ok && null != _targetCiv)
        {
            var newGovt = Governments[selectedIndex];
            _targetCiv.Government = selectedIndex;
            _targetCiv.LeaderTitle = _targetCiv.LeaderGender == 0 ? newGovt.TitleMale : newGovt.TitleFemale;
            // TODO: Pop-up to reset tax rates
            // TODO: "$LEADER proclaimed $TITLE of ..." newspaper dialog
        }
        GameScreen.CloseDialog(_chooseGovtDialog);
    }
}