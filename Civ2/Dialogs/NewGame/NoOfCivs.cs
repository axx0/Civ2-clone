using Civ2.Rules;
using Civ2engine;
using Model.Dialog;
using Model.InterfaceActions;

namespace Civ2.Dialogs.NewGame;

public class NoOfCivs : SimpleSettingsDialog
{
    public const string Title = "ENEMIES";
    
    public NoOfCivs() : base(Title, -0.085, -0.03)
    {
    }

    public override IInterfaceAction Show(Civ2Interface activeInterface)
    {
        var possibleCivs = activeInterface.PlayerColours.Length - 1;
        if(Dialog.Dialog.Options == null || Dialog.Dialog.Options.Count +2 != possibleCivs)
        {
            var suffix = Dialog.Dialog.Options?[0].Split(" ", 2,
                StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)[1] ?? "Civilizations";
            Dialog.Dialog.Options = Enumerable.Range(0, possibleCivs - 2)
                .Select(v => $"{(possibleCivs - v)} {suffix}").ToArray();
        }
        return base.Show(activeInterface);
    }

    protected override string SetConfigValue(DialogResult result, PopupBox? popupBox)
    {
        Initialization.ConfigObject.NumberOfCivs = popupBox.Options.Count + 2 - result.SelectedIndex;
        return Barbarity.Title;
    }
}