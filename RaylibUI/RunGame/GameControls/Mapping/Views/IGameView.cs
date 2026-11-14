using System.Collections;
using System.Numerics;
using Civ2engine.MapObjects;
using Model.Core.Mapping;

namespace RaylibUI.RunGame.GameControls.Mapping.Views;

/// <summary>
/// A terrain tile to draw in the map view
/// </summary>
/// <param name="Tile">Map tile</param>
/// <param name="Location">Top-left corner of the tile, relative to the map view (px)</param>
/// <param name="GridLabel">Coordinates shown when the grid is on (null when off)</param>
public readonly record struct MapTileDraw(Tile Tile, Vector2 Location, string? GridLabel);

public interface IGameView
{
    Tile Location { get; }

    /// <summary>
    /// Visible terrain tiles of this view, drawn every frame from the tile cache
    /// </summary>
    IReadOnlyList<MapTileDraw> Tiles { get; }

    /// <summary>
    /// Civilization whose view of the map is shown
    /// </summary>
    int CivilizationId { get; }

    IViewElement[] Elements { get; }
    IEnumerable<IViewElement> CurrentAnimations { get; }
    int ViewHeight { get; }
    int ViewWidth { get; set; }
    Vector2 Offsets { get; }
    int Xshift { get; }
    bool IsDefault { get;  }
    int Interval { get; }
    IList<Tile> ActionTiles { get; }
    bool Finished();

    void Reset();
    void Next();
}
