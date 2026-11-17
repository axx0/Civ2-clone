using System.Diagnostics;
using System.Linq;
using System.Numerics;
using Civ2engine.Events;
using Civ2engine.MapObjects;

namespace RaylibUI.RunGame.GameControls.Mapping.Views;

/// <summary>
/// A unit sliding from one tile to the next. Its position is worked out from the time since the slide started,
/// so it moves smoothly at any frame rate and always takes the same time.
/// </summary>
internal class MoveAnimation : BaseGameView
{
    /// <summary>
    /// Length of the slide in the original 8 steps of 30 ms
    /// </summary>
    private const double SlideDuration = 8 * 30;

    /// <summary>
    /// Units standing on the tile the unit leaves and the tile it enters (they don't move)
    /// </summary>
    private readonly List<IViewElement> _standingUnits;

    /// <summary>
    /// The moving unit at its start position
    /// </summary>
    private readonly List<IViewElement> _movingUnit;

    /// <summary>
    /// Distance of the whole slide (px, at the current zoom)
    /// </summary>
    private readonly Vector2 _slide;

    /// <summary>
    /// A unit entering a city disappears into it for the last eighth of the slide (it isn't drawn over the city)
    /// </summary>
    private readonly bool _entersCity;

    public MoveAnimation(GameScreen gameScreen, MovementEventArgs moveEvent, IGameView? previousView, int viewHeight,
        int viewWidth, bool forceRedraw) : base(gameScreen, moveEvent.Location.First(), previousView, viewHeight, viewWidth, false, 30, moveEvent.Location, forceRedraw)
    {
        var activeInterface = gameScreen.Main.ActiveInterface;
        var activeUnit = moveEvent.Unit;
        var map = activeUnit.CurrentLocation.Map;
        float[] unitDrawOffset = { activeUnit.X - activeUnit.PrevXy[0], activeUnit.Y - activeUnit.PrevXy[1] };
        if (!map.Flat && Math.Abs(unitDrawOffset[0]) >= map.XDimMax - 2)
        {
            if (unitDrawOffset[0] < 0)
            {
                unitDrawOffset[0] += map.XDimMax;
            }
            else
            {
                unitDrawOffset[0] -= map.XDimMax;
            }
        }

        // Map coordinates are in half tiles (x is doubled), so one step is half a tile on screen
        _slide = new Vector2(unitDrawOffset[0] * Dimensions.HalfWidth, unitDrawOffset[1] * Dimensions.HalfHeight);
        var unitPos = ActivePos with { Y = ActivePos.Y - activeInterface.UnitImages.UnitRectangle.Height.ZoomScale(gameScreen.Zoom) + Dimensions.TileHeight };

        // Units on previous tile of moving unit
        _standingUnits = new List<IViewElement>();
        var prevTileUnit = map.TileC2(activeUnit.PrevXy[0], activeUnit.PrevXy[1]).UnitsHere.FirstOrDefault();
        if (prevTileUnit != null)
        {
            ImageUtils.GetUnitTextures(prevTileUnit, activeInterface, gameScreen.Game, _standingUnits, unitPos);
        }

        // Units on next tile of moving unit
        var nextTileUnit = activeUnit.CurrentLocation.UnitsHere.Where(u => u != activeUnit && !activeUnit.CarriedUnits.Contains(u)).FirstOrDefault();
        if (nextTileUnit != null)
        {
            ImageUtils.GetUnitTextures(nextTileUnit, activeInterface, gameScreen.Game, _standingUnits, unitPos + _slide);
        }

        // Moving unit
        _movingUnit = new List<IViewElement>();
        ImageUtils.GetUnitTextures(activeUnit, activeInterface, gameScreen.Game, _movingUnit, unitPos, true);

        _entersCity = activeUnit.CurrentLocation.CityHere != null;
    }

    public override double Duration => SlideDuration;

    public override IEnumerable<IViewElement> GetAnimations(double elapsedMs)
    {
        var progress = Math.Clamp(elapsedMs / SlideDuration, 0, 1);
        if (_entersCity && progress >= 7 / 8.0)
        {
            return [];
        }

        // Whole pixels, so the unit stays sharp with smooth map scaling
        var offset = new Vector2(MathF.Round(_slide.X * (float)progress), MathF.Round(_slide.Y * (float)progress));
        return _standingUnits.Concat(_movingUnit.Select(ve => ve.CloneForLocation(ve.Location + offset)));
    }
}
