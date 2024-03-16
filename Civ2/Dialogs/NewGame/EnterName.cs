using Civ2.Rules;
using Civ2engine;
using Model;
using Model.Interface;
using Model.InterfaceActions;

namespace Civ2.Dialogs.NewGame;

public class EnterName : BaseDialogHandler
{
    public const string Title = "NAME";

    public EnterName() : base(Title, 0, -0.03)
    {
    }

    public override IInterfaceAction Show(Civ2Interface activeInterface)
    {
        if (Dialog.TextBoxes == null || Dialog.TextBoxes.Count == 0)
        {
            Dialog.TextBoxes = new List<TextBoxDefinition>
            {
                new()
                {
                    Index = 0,
                    Name = "Name",
                    InitialValue = Initialization.ConfigObject.IsScenario ?
                        Game.Instance.GetPlayerCiv.LeaderName : 
                        Initialization.ConfigObject.PlayerCiv.LeaderName,
                    Width = 400,
                    Description = Dialog.Dialog.Options?[0] ?? string.Empty
                },
            };
            Dialog.Dialog.Options = null;
        }
        else
        {
            Dialog.TextBoxes[0].InitialValue = Initialization.ConfigObject.PlayerCiv.LeaderName;
        }

        return base.Show(activeInterface);
    }

    public override IInterfaceAction HandleDialogResult(DialogResult result,
        Dictionary<string, ICivDialogHandler> civDialogHandlers, Civ2Interface civ2Interface)
    {
        if (result.SelectedButton == Labels.Cancel)
        {
            return Initialization.ConfigObject.IsScenario ?
                civDialogHandlers[Difficulty.Title].Show(civ2Interface) :
                civDialogHandlers[SelectGender.Title].Show(civ2Interface);
        }

        if (result.TextValues != null)
        {
            if (Initialization.ConfigObject.IsScenario)
            {
                Game.Instance.AllCivilizations.Find(c => c.PlayerType == PlayerType.Local).LeaderName = result.TextValues["Name"];
            }
            else
            {
                Initialization.ConfigObject.PlayerCiv.LeaderName = result.TextValues["Name"];
            }
        }
        
        return Initialization.ConfigObject.IsScenario ?
            new StartGame(Initialization.GameInstance) :
            civDialogHandlers[SelectCityStyle.Title].Show(civ2Interface);
    }
}