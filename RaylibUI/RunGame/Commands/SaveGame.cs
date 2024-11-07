using Civ2engine;
using Model;
using Model.Dialog;
using Model.Menu;
using Raylib_CSharp.Interact;

namespace RaylibUI.RunGame.Commands;

public class SaveGame : IGameCommand
{
    private readonly GameScreen _gameScreen;

    public SaveGame(GameScreen gameScreen)
    {
        _gameScreen = gameScreen;
    }
    
    public string Id => CommandIds.SaveGame;
    public Shortcut[] ActivationKeys { get; set; } = [new Shortcut(KeyboardKey.S, ctrl: true)];
    public CommandStatus Status => CommandStatus.Normal;
    public bool Update()
    {
        return true;
    }

    public void Action()
    {
        var saveDialog = new FileDialog(_gameScreen.Main, Labels.For(LabelIndex.SaveFiles),
            _gameScreen.Game.GamePaths.First(), IsValidSelectionCallback, OnSelectionCallback);

       

        _gameScreen.ShowDialog(saveDialog, true);
    }
 bool OnSelectionCallback(string? arg)
        {
            throw new NotImplementedException();
        }
    private bool IsValidSelectionCallback(string path)
    {
        return path.EndsWith(".sav");
    }

    public MenuCommand? Command { get; set; }
    public string ErrorDialog { get; }
    public DialogImageElements? ErrorImage { get; }
    public string? Name { get; }
}