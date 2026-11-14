using Model.Input;
using Civ2engine;
using Civ2engine.Enums;
using Civ2engine.IO;
using JetBrains.Annotations;
using Model;
using Model.Controls;
using Model.Core;

namespace RaylibUI.RunGame.Commands;

[UsedImplicitly]
public class GraphicOptions(GameScreen gameScreen) : IGameCommand
{
    private readonly Options _options = gameScreen.Game.Options;

    public string Id => CommandIds.GraphicOptions;
    public Shortcut[] ActivationKeys { get; set; } = { new(Key.P, ctrl: true) };
    public CommandStatus Status => CommandStatus.Normal;

    public bool Update()
    {
        return true;
    }

    public void Action()
    {
        // ReSharper disable once StringLiteralTypo
        gameScreen.ShowPopup("GRAPHICOPTIONS", DialogClick,
            checkboxStates: new List<bool>
            {
                _options.ThroneRoomGraphics, _options.DiplomacyScreenGraphics, _options.AnimatedHeralds,
                _options.CivilopediaForAdvances, _options.HighCouncil, _options.WonderMovies,
                Settings.MapScaling == MapScaling.Smooth
            },
            extraOptions: [SmoothMapScalingText]);
    }

    /// <summary>
    /// Our own option (not in the game's text files)
    /// </summary>
    private const string SmoothMapScalingText = "Smooth Map Scaling";

    private void DialogClick(string button, int _, IList<bool>? checkboxes, IDictionary<string, string>? _2)
    {
        if (button != Labels.Ok) return;
        
        _options.ThroneRoomGraphics = checkboxes![0];
        _options.DiplomacyScreenGraphics = checkboxes[1];
        _options.AnimatedHeralds = checkboxes[2];
        _options.CivilopediaForAdvances = checkboxes[3];
        _options.HighCouncil = checkboxes[4];
        _options.WonderMovies = checkboxes[5];

        if (checkboxes.Count > 6)
        {
            var scaling = checkboxes[6] ? MapScaling.Smooth : MapScaling.Pixelated;
            if (scaling != Settings.MapScaling)
            {
                Settings.SetMapScaling(scaling);
                TextureCache.ApplyMapScaling();
                gameScreen.TileCache.ApplyMapScaling();
            }
        }
    }

    public bool Checked => false;
    public MenuCommand? Command { get; set; }
    public string ErrorDialog => string.Empty;
    public DialogImageElements? ErrorImage => null;
    public string? Name => "Graphic Options";
}