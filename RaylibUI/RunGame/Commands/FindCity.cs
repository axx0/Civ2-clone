using Civ2engine;
using Civ2engine.Enums;
using Civ2engine.Events;
using Model.Controls;
using Model.Input;
using RaylibUI.BasicTypes;

namespace RaylibUI.RunGame.Commands;

public class FindCity(GameScreen gameScreen)
    : AlwaysOnCommand(gameScreen, CommandIds.FindCity, [new Shortcut(Key.C, true)])
{
    public override void Action()
    {
        var listbox = new ListboxDefinition
        {
            Type = ListboxType.Default,
            Rows = 16,
        };
        listbox.Update(GameScreen.Game.AllCities.Select(c => $"{c.Name} ({c.Owner.Adjective})").ToList());

        // ReSharper disable once StringLiteralTypo
        GameScreen.ShowPopup("FINDCITY", DialogClick, listBox: listbox);
    }

    private void DialogClick(string button, int i, IList<bool>? checkboxes, IDictionary<string, string>? _)
    {
        var cities = GameScreen.Game.AllCities;

        if (button == Labels.Ok)
        {
            //GameScreen.TriggerMapEvent(new MapEventArgs(MapEventType.MapViewChanged) {  });
        }
    }
}