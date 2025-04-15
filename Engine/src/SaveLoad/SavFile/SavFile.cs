using System.Collections.Generic;
using System.Text.Json;
using Civ2engine.IO;
using Civ2engine.OriginalSaves;
using Model.Core;

namespace Civ2engine.SaveLoad.SavFile;

public interface ISavFile
{
    public IGame LoadGame(byte[] fileData, Ruleset activeRuleSet, Rules rules);
}

public abstract class SavFileBase : ISavFile
{    
    protected Dictionary<string, string> viewData = new Dictionary<string, string>();
    public Dictionary<string, string> ViewData => viewData;

    protected Dictionary<string, string> extendedMetadata = new Dictionary<string, string>();
    public abstract IGame LoadGame(byte[] fileData, Ruleset activeRuleSet, Rules rules);
}

public class ClassicSavFile : SavFileBase
{
    public override IGame LoadGame(byte[] fileData, Ruleset activeRuleSet, Rules rules)
    {
        var scnNames = new string[] { "Original", "SciFi", "Fantasy" };
        if (fileData[10] > 44)
        {
            extendedMetadata.Add("TOT-Scenario", scnNames[fileData[982]]);
        }

        return Read.ClassicSav(fileData, activeRuleSet, rules, viewData);
    }
}

public class JsonSavFile : SavFileBase
{
    public override IGame LoadGame(byte[] fileData, Ruleset activeRuleSet, Rules rules)
    {
         var jsonDocument = JsonDocument.Parse(fileData);

        var metaData = jsonDocument.RootElement.GetProperty("extendedMetadata");
        foreach (var meta in metaData.EnumerateObject())
        {
            extendedMetadata[meta.Name] = meta.Value.GetString() ?? string.Empty;
        }

        if (jsonDocument.RootElement.TryGetProperty("viewData", out var viewDataElement))
        {
            foreach (var prop in viewDataElement.EnumerateObject())
            {
                viewData[prop.Name] = prop.Value.GetString();
            }
        }
        return GameSerializer.Read(jsonDocument.RootElement.GetProperty("game"), activeRuleSet, rules);
    }
}