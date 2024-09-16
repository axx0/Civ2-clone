using Civ2.Rules;
using Civ2engine;
using Model.Dialog;
using Model.InterfaceActions;

namespace Civ2.Dialogs.NewGame;

public class SelectOpponent : BaseDialogHandler
{
    private List<LeaderDefaults> _tribes;
    private int _index;
    public const string Title = "OPPONENT";
    public override IInterfaceAction HandleDialogResult(DialogResult result, Dictionary<string, ICivDialogHandler> civDialogHandlers, Civ2Interface activeInterface)
    {              
        if (result.SelectedIndex == int.MinValue)
        {
            return civDialogHandlers[SelectCityStyle.Title].Show(activeInterface);
        }

        var config = Initialization.ConfigObject;
        Initialization.ConfigObject.Civilizations.Add(Initialization.MakeCivilization(config,
            _tribes[
                result.SelectedIndex == 0
                    ? config.Random.Next(_tribes.Count)
                    : result.SelectedIndex - 1], false, _index));
        if (Initialization.ConfigObject.Civilizations.Count >= Initialization.ConfigObject.NumberOfCivs)
        {
            return civDialogHandlers[Init.Title].Show(activeInterface);
        }

        return Show(activeInterface);
    }

    public override IInterfaceAction Show(Civ2Interface activeInterface)
    {
        var opponentNumber = Initialization.ConfigObject.Civilizations.Count - 1;
        _index = opponentNumber < Initialization.ConfigObject.PlayerCiv.Id ? opponentNumber : opponentNumber + 1;

        _tribes = Initialization.ConfigObject.GroupedTribes.Contains(_index)
            ? Initialization.ConfigObject.GroupedTribes[_index].ToList()
            : Initialization.ConfigObject.Rules.Leaders
                .Where(leader =>
                    Initialization.ConfigObject.Civilizations.All(civ => civ.Adjective != leader.Adjective)).ToList();

        Dialog.Dialog.Options =
            new[] { Dialog.Dialog.Options[0] }.Concat(_tribes.Select(leader =>
                $"{leader.Plural} ({(leader.Female ? leader.NameFemale : leader.NameMale)})")).ToList();
        return base.Show(activeInterface);
    }

    public SelectOpponent() : base(Title)
    {
    }
}