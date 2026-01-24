using Model.Input;
using Civ2engine;
using JetBrains.Annotations;
using Model;
using Model.Dialog;
using Model.Menu;

namespace RaylibUI.RunGame.Commands;

[UsedImplicitly]
public class GraphicOptions(GameScreen gameScreen) : IGameCommand
{
    private readonly Options _options = gameScreen.Game.Options;

    public string Id => CommandIds.GraphicOptions;
    public Shortcut[] ActivationKeys { get; set; } = { new(Key.P, ctrl: true) };
    public CommandStatus Status => CommandStatus.Normal;

    public bool Update()
    {
        return true;
    }

    public void Action()
    {
        // ReSharper disable once StringLiteralTypo
        gameScreen.ShowPopup("GRAPHICOPTIONS", DialogClick,
            checkboxStates: new List<bool>
            {
                _options.ThroneRoomGraphics, _options.DiplomacyScreenGraphics, _options.AnimatedHeralds,
                _options.CivilopediaForAdvances, _options.HighCouncil, _options.WonderMovies
            });
    }

    private void DialogClick(string button, int _, IList<bool>? checkboxes, IDictionary<string, string>? _2)
    {
        if (button != Labels.Ok) return;
        
        _options.ThroneRoomGraphics = checkboxes![0];
        _options.DiplomacyScreenGraphics = checkboxes[1];
        _options.AnimatedHeralds = checkboxes[2];
        _options.CivilopediaForAdvances = checkboxes[3];
        _options.HighCouncil = checkboxes[4];
        _options.WonderMovies = checkboxes[5];
    }

    public bool Checked => false;
    public MenuCommand? Command { get; set; }
    public string ErrorDialog => string.Empty;
    public DialogImageElements? ErrorImage => null;
    public string? Name => "Graphic Options";
}