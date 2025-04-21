using Civ2engine;
using Model;
using Model.Dialog;
using Model.Images;
using Model.Menu;
using Raylib_CSharp.Interact;

namespace RaylibUI.RunGame.Commands;

public class GameOptions : IGameCommand
{
    private readonly GameScreen _gameScreen;
    private readonly Options _options;

    public GameOptions(GameScreen gameScreen)
    {
        _gameScreen = gameScreen;
        _options = gameScreen.Game.Options;
    }
    
    public string Id => CommandIds.GameOptions;
    public Shortcut[] ActivationKeys { get; set; } = { new(KeyboardKey.O, ctrl: true) };
    public CommandStatus Status => CommandStatus.Normal;

    public bool Update()
    {
        return true;
    }

    public void Action()
    {
        // ReSharper disable once StringLiteralTypo
        _gameScreen.ShowPopup("GAMEOPTIONS", DialogClick, 
            checkboxStates: new List<bool> { _options.SoundEffects, _options.Music, _options.AlwaysWaitAtEndOfTurn, _options.AutosaveEachTurn, _options.ShowEnemyMoves, _options.NoPauseAfterEnemyMoves, _options.FastPieceSlide, _options.InstantAdvice, _options.TutorialHelp, _options.MoveUnitsWithoutMouse, _options.EnterClosestCityScreen });
    }

    private void DialogClick(string button, int _, IList<bool>? checkboxes, IDictionary<string, string>? _2)
    {
        if (button == Labels.Ok)
        {
            _options.SoundEffects = checkboxes[0];
            _options.Music = checkboxes[1];
            _options.AlwaysWaitAtEndOfTurn = checkboxes[2];
            _options.AutosaveEachTurn = checkboxes[3];
            _options.ShowEnemyMoves = checkboxes[4];
            _options.NoPauseAfterEnemyMoves = checkboxes[5];
            _options.FastPieceSlide = checkboxes[6];
            _options.InstantAdvice = checkboxes[7];
            _options.TutorialHelp = checkboxes[8];
            _options.MoveUnitsWithoutMouse = checkboxes[9];
            _options.EnterClosestCityScreen = checkboxes[10];
        }
    }

    public bool Checked => false;
    public MenuCommand? Command { get; set; }
    public string ErrorDialog { get; } = string.Empty;
    public DialogImageElements? ErrorImage { get; } = null;
    public string? Name { get; }
}