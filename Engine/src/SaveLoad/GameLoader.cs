using System;
using System.IO;
using Civ2engine.IO;
using Civ2engine.SaveLoad.SavFile;
using Model;
using Model.Core;
using Model.InterfaceActions;

namespace Civ2engine.SaveLoad;

public interface IGameLoader
{
    IInterfaceAction LoadGame(IGame game, IUserInterface activeInterface);
}

public class GameLoader : IGameLoader
{
    private string path;
    private string savDirectory; // Only needed for when dealing with scenario loading, not used for normal files.
    private Rules rules;
    private Ruleset activeRuleSet;
    SavFileBase savFile;

    public GameLoader(string path, string savDirectory, Rules rules, Ruleset activeRuleset, SavFileBase savFile)
    {
        this.path = path;
        this.savDirectory = savDirectory;
        this.rules = rules;
        this.activeRuleSet = activeRuleset;
        this.savFile = savFile;
    }

    public IInterfaceAction LoadGame(IGame game, IUserInterface activeInterface)
    {
        // TODO: This was only possible for a classic sav file. Was this intentional?
        if (string.Equals(Path.GetExtension(path), ".scn", StringComparison.OrdinalIgnoreCase))
        {
            var scnName = Path.GetFileName(path);
            return activeInterface.HandleLoadScenario(game, scnName, savDirectory);
        }
        return activeInterface.HandleLoadGame(game, rules, activeRuleSet, savFile.ViewData!);
    }
}
