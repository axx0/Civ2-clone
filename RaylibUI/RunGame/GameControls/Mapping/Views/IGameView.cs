using System.Collections;
using System.Numerics;
using Civ2engine.MapObjects;
using Raylib_cs;

namespace RaylibUI.RunGame.GameControls.Mapping.Views;

public interface IGameView : IDisposable
{
    Tile Location { get; }
    Texture2D BaseImage { get; }
    ViewElement[] Elements { get; }
    IEnumerable<ViewElement> CurrentAnimations { get; }
    int ViewHeight { get; }
    int ViewWidth { get; set; }
    Vector2 Offsets { get; }
    bool IsDefault { get;  }
    int Interval { get; }
    bool Finished();

    void Reset();
    void Preserve();
    void Next();
}