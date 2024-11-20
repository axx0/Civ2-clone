using Civ2engine;
using Model.Menu;
using Raylib_CSharp.Interact;

namespace RaylibUI.RunGame.Commands;

public class LoadGame(GameScreen gameScreen) : AlwaysOnCommand(gameScreen, CommandIds.LoadGame, [new Shortcut(KeyboardKey.L, ctrl: true)])
{
    private FileDialog _loadDialog;

    public override void Action()
    {
        _loadDialog = new FileDialog(GameScreen.Main, Labels.For(LabelIndex.SelectGameToLoad),
            GameScreen.Main.ActiveRuleSet.FolderPath, IsValidSelectionCallback, OnSelectionCallback);
        GameScreen.ShowDialog(_loadDialog, true);
    }

    private bool OnSelectionCallback(string? arg)
    {
        if (arg == null)
        {
            GameScreen.CloseDialog(_loadDialog);
            return false;
        }
        
        Civ2engine.SaveLoad.LoadGame.LoadFrom(arg, GameScreen.Main);
        
        GameScreen.CloseDialog(_loadDialog);
        return true;
    }

    private bool IsValidSelectionCallback(string path)
    {
        return path.EndsWith(".sav", StringComparison.InvariantCultureIgnoreCase) && File.Exists(path);
    }
}