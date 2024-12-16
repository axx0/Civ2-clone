using Civ2.Dialogs.NewGame;
using Civ2.Dialogs.NewGame.PremadeWorld;
using Civ2.Rules;
using Civ2engine;
using Civ2engine.IO;
using Model;
using Model.InterfaceActions;

namespace Civ2.Dialogs.FileDialogs;

public class LoadMap : FileDialogHandler
{
    public const string DialogTitle = "File-LoadMap";

    public LoadMap() : base(DialogTitle, ".mp")
    {
    }

    public override ICivDialogHandler UpdatePopupData(Dictionary<string, PopupBox?> popup)
    {
        this.Title = Labels.For(LabelIndex.SelectMapToLoad);
        return this;
    }

    protected override IInterfaceAction HandleFileSelection(string fileName, Dictionary<string, ICivDialogHandler> civDialogHandlers, Civ2Interface civ2Interface)
    {
        
        var mapDirectory = Path.GetDirectoryName(fileName);
        var root = Settings.SearchPaths.FirstOrDefault(p => mapDirectory.StartsWith(p)) ?? Settings.SearchPaths[0];
        
        civ2Interface.MainApp.SetActiveRulesetFromFile(root, mapDirectory, new Dictionary<string, string>());

        var config = Initialization.ConfigObject;
        try
        {
            var mapData = MapReader.Read(fileName);
            config.ResourceSeed = mapData.ResourceSeed;
            config.FlatWorld = mapData.FlatWorld;
            config.WorldSize = [mapData.Width /2, mapData.Height];
            config.TerrainData = mapData.TerrainData;
            if (mapData.StartPositions is { Length: > 0 } && mapData.StartPositions.Any(p=> p.First != -1 && p.Second != -1))
            {
                config.StartPositions = mapData.StartPositions.Select(p => new int[] { p.First, p.Second }).ToArray();
            }

            config.ResourceSeed = mapData.ResourceSeed % 64;
        }
        catch
        {
            return civDialogHandlers[Failed.Title].Show(civ2Interface);
        }
        
        Initialization.LoadGraphicsAssets(civ2Interface);
        
        if (config.ResourceSeed > 0)
        {
            return civDialogHandlers[RandomiseResourceSeed.Title].Show(civ2Interface);
        }
        
        if (config.StartPositions is { Length: > 0 })
        {
            return civDialogHandlers[StartLoc.StartLocKey].Show(civ2Interface);
        }
        
        return civDialogHandlers[DifficultyHandler.Title].Show(civ2Interface);
    }
}