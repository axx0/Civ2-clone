using Civ2.Dialogs.FileDialogs;
using Civ2.Dialogs.NewGame;
using Civ2.Rules;
using Civ2engine;
using Model;
using Model.Dialog;
using Model.InterfaceActions;

namespace Civ2.Dialogs.Scenario;

public class ScenChoseCiv : ICivDialogHandler
{
    public const string Title = "SCENCHOSECIV";

    public string Name { get; } = Title;
    public ICivDialogHandler UpdatePopupData(Dictionary<string, PopupBox?> popups)
    {
        Dialog = new DialogElements
        {
            Dialog = new PopupBox()
            {
                Button = new List<string> { Labels.Ok, Labels.Cancel },
                Title = " ",
                Name = Title,
            },
            DialogPos = new Point(0, 0),
        };
        return this;
    }

    public DialogElements Dialog { get; private set; }

    public IInterfaceAction HandleDialogResult(DialogResult result,
        Dictionary<string, ICivDialogHandler> civDialogHandlers, Civ2Interface civ2Interface)
    {
        if (result.SelectedButton == Labels.Cancel)
        {
            return civDialogHandlers[LoadScenario.DialogTitle].Show(civ2Interface);
        }

        var boolPos = new int[Initialization.ConfigObject.CivsInPlay.Count(c => c)];
        int count = 0;
        for (int i = 0; i < Initialization.ConfigObject.CivsInPlay.Length; i++)
        {
            if (Initialization.ConfigObject.CivsInPlay[i])
            {
                boolPos[count] = i;
                count++;
            }
        }
        Initialization.ConfigObject.ScenPlayerCivId = boolPos[result.SelectedIndex + 1];

        var difficultyDialog = civDialogHandlers[DifficultyHandler.Title];
        var popupContent = difficultyDialog.Dialog.Dialog;
        popupContent!.Options = new List<string> { 
            Labels.For(LabelIndex.Chieftan) + " (easiest)", Labels.For(LabelIndex.Warlord),
            Labels.For(LabelIndex.Prince), Labels.For(LabelIndex.King), 
            Labels.For(LabelIndex.Emperor), Labels.For(LabelIndex.Deity) + " (toughest)"};

        popupContent.Button = new List<string> { Labels.Ok, Labels.Cancel };

        popupContent.Title = $"Select {Labels.For(LabelIndex.Difficulty)} Level";

        return difficultyDialog.Show(civ2Interface);
    }

    public IInterfaceAction Show(Civ2Interface activeInterface)
    {
        //activeInterface.ScenTitleImage = 

        Dialog.Dialog.Options = Enumerable.Range(0, 7).Where(i => Initialization.ConfigObject.CivsInPlay[i + 1]).Select(i => $"{Initialization.ConfigObject.CivNames[i + 1]} ({Initialization.ConfigObject.LeaderNames[i + 1]})").ToList();
        return new MenuAction(Dialog);
    }
}