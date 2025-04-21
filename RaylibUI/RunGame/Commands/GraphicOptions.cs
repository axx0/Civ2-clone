using Civ2engine;
using Model;
using Model.Dialog;
using Model.Images;
using Model.Menu;
using Raylib_CSharp.Interact;

namespace RaylibUI.RunGame.Commands;

public class GraphicOptions : IGameCommand
{
    private readonly GameScreen _gameScreen;
    private readonly Options _options;

    public GraphicOptions(GameScreen gameScreen)
    {
        _gameScreen = gameScreen;
        _options = gameScreen.Game.Options;
    }
    
    public string Id => CommandIds.GraphicOptions;
    public Shortcut[] ActivationKeys { get; set; } = { new(KeyboardKey.P, ctrl: true) };
    public CommandStatus Status => CommandStatus.Normal;

    public bool Update()
    {
        return true;
    }

    public void Action()
    {
        // ReSharper disable once StringLiteralTypo
        _gameScreen.ShowPopup("GRAPHICOPTIONS", DialogClick, 
            checkboxStates: new List<bool> { _options.ThroneRoomGraphics, _options.DiplomacyScreenGraphics, _options.AnimatedHeralds, _options.CivilopediaForAdvances, _options.HighCouncil, _options.WonderMovies });
    }

    private void DialogClick(string button, int _, IList<bool>? checkboxes, IDictionary<string, string>? _2)
    {
        if (button == Labels.Ok)
        {
            _options.ThroneRoomGraphics = checkboxes[0];
            _options.DiplomacyScreenGraphics = checkboxes[1];
            _options.AnimatedHeralds = checkboxes[2];
            _options.CivilopediaForAdvances = checkboxes[3];
            _options.HighCouncil = checkboxes[4];
            _options.WonderMovies = checkboxes[5];
        }
    }

    public bool Checked => false;
    public MenuCommand? Command { get; set; }
    public string ErrorDialog { get; } = string.Empty;
    public DialogImageElements? ErrorImage { get; } = null;
    public string? Name { get; }
}