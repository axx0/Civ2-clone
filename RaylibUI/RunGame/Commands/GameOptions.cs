using Model.Input;
using Civ2engine;
using JetBrains.Annotations;
using Model;
using Model.Dialog;
using Model.Menu;

namespace RaylibUI.RunGame.Commands;

[UsedImplicitly]
public class GameOptions(GameScreen gameScreen) : IGameCommand
{
    private readonly Options _options = gameScreen.Game.Options;

    public string Id => CommandIds.GameOptions;
    public Shortcut[] ActivationKeys { get; set; } = { new(Key.O, ctrl: true) };
    public CommandStatus Status => CommandStatus.Normal;

    public bool Update()
    {
        return true;
    }

    public void Action()
    {
        // ReSharper disable once StringLiteralTypo
        gameScreen.ShowPopup("GAMEOPTIONS", DialogClick,
            checkboxStates: (List<bool>)
            [
                _options.SoundEffects, _options.Music, _options.AlwaysWaitAtEndOfTurn, _options.AutosaveEachTurn,
                _options.ShowEnemyMoves, _options.NoPauseAfterEnemyMoves, _options.FastPieceSlide,
                _options.InstantAdvice, _options.TutorialHelp, _options.MoveUnitsWithoutMouse,
                _options.EnterClosestCityScreen
            ]);
    }

    private void DialogClick(string button, int _, IList<bool>? checkboxes, IDictionary<string, string>? _2)
    {
        if (button == Labels.Ok)
        {
            _options.SoundEffects = checkboxes![0];
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
    public string ErrorDialog => string.Empty;
    public DialogImageElements? ErrorImage => null;
    public string? Name => "Game Options";
}