using System;
using System.Collections.Generic;
using System.IO;
using Civ2engine.IO;
using Model;
using Model.Core;
using Model.InterfaceActions;
using Civ2engine.src.SaveLoad.SavFile;
using System.Formats.Tar;

namespace Civ2engine.src.SaveLoad;

public class GameLoader
{
    private string path;
    private string savDirectory;
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

    public IInterfaceAction LoadGame(IGame game, string savDirectory, IUserInterface activeInterface)
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
