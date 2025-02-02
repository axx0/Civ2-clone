using System.Data;
using System.Diagnostics;
using System.IO.Compression;
using System.Net;
using System.Text.Json;
using Civ2engine;
using Civ2engine.SaveLoad;
using Model;
using Model.Dialog;
using Model.Menu;
using Raylib_CSharp.Interact;

namespace RaylibUI.RunGame.Commands;

public class SaveGame(GameScreen gameScreen) : AlwaysOnCommand(gameScreen, CommandIds.SaveGame,
    [new Shortcut(KeyboardKey.S, ctrl: true)])
{
    private FileDialog? _saveDialog;

    public override void Action()
    {
        var suggestedFileName =
            $"{GameScreen.Game.ActivePlayer.Civilization.LeaderName.Substring(0, 2)}_{GameScreen.Game.Date.GameYearString(GameScreen.Game.TurnNumber, "").Replace(".", "")}.sav"
                .ToLowerInvariant();
        _saveDialog = new FileDialog(GameScreen.Main, Labels.For(LabelIndex.SaveFiles),
            GameScreen.Main.ActiveRuleSet.FolderPath, IsValidSelectionCallback, OnSelectionCallback, suggestedFileName,
            false);
        GameScreen.ShowDialog(_saveDialog, true);
    }

    bool OnSelectionCallback(string? filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            if (_saveDialog != null) GameScreen.CloseDialog(_saveDialog);
            _saveDialog = null;
            return true;
        }

        
        var serializer = new GameSerializer();
        var game = GameScreen.Game;
            
        var viewData = new Dictionary<string, string> {{ "Zoom", GameScreen.Zoom.ToString() }};
        if (File.Exists(filePath))
        {
            //TODO: prompt for overwrite?

            using var saveFile = File.Open(filePath, FileMode.Truncate);
            serializer.Write(saveFile, game, GameScreen.Main.ActiveRuleSet, viewData);
        }
        else
        {
            using var saveFile = File.Open(filePath, FileMode.CreateNew);
            serializer.Write(saveFile, game, GameScreen.Main.ActiveRuleSet, viewData);
        }

        GameScreen.ShowPopup("SAVEOK", replaceStrings: new List<string>{ game.ActivePlayer.Civilization.LeaderTitle, game.ActivePlayer.Civilization.LeaderName, game.ActivePlayer.Civilization.TribeName,Path.GetFileName(filePath)}, handleButtonClick: CloseConfirm);
        return true;
    }

    private void CloseConfirm(string arg1, int arg2, IList<bool>? arg3, IDictionary<string, string>? arg4)
    {
        GameScreen.CloseDialog(_saveDialog);
    }

    private bool IsValidSelectionCallback(string path)
    {
        return path.EndsWith(".sav", StringComparison.InvariantCultureIgnoreCase);
    }
}