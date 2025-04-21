using System.Collections;
using System.Numerics;
using Civ2engine.MapObjects;
using Raylib_CSharp.Textures;

namespace RaylibUI.RunGame.GameControls.Mapping.Views;

public interface IGameView : IDisposable
{
    Tile Location { get; }
    Texture2D BaseImage { get; }
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
    void Preserve();
    void Next();
}