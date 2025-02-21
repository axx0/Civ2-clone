using System.Diagnostics;
using System.Linq;
using System.Numerics;
using Civ2engine.Events;
using Civ2engine.MapObjects;

namespace RaylibUI.RunGame.GameControls.Mapping.Views;

internal class MoveAnimation : BaseGameView
{
    public MoveAnimation(GameScreen gameScreen, MovementEventArgs moveEvent, IGameView? previousView, int viewHeight,
        int viewWidth, bool forceRedraw) : base(gameScreen, moveEvent.Location.First(), previousView, viewHeight, viewWidth, false, 30, moveEvent.Location, forceRedraw)
    {
        var activeInterface = gameScreen.Main.ActiveInterface;
        var activeUnit = moveEvent.Unit;
        var noFramesForOneMove = 8;
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
        
        // Get view elements of units on previous tile of moving unit
        var viewElementsPrevTileUnits = new List<IViewElement>();
        var prevTileUnit = map.TileC2(activeUnit.PrevXy[0], activeUnit.PrevXy[1]).UnitsHere.FirstOrDefault();
        if (prevTileUnit != null)
        {
            ImageUtils.GetUnitTextures(prevTileUnit, activeInterface, gameScreen.Game, viewElementsPrevTileUnits,
                ActivePos with { Y = ActivePos.Y - activeInterface.UnitImages.UnitRectangle.Height.ZoomScale(gameScreen.Zoom) + Dimensions.TileHeight });
        }

        // Get view elements of units on next tile of moving unit
        var viewElementsNextTileUnits = new List<IViewElement>();
        var nextTileUnit = activeUnit.CurrentLocation.UnitsHere.Where(u => u != activeUnit && !activeUnit.CarriedUnits.Contains(u)).FirstOrDefault();
        if (nextTileUnit != null)
        {
            ImageUtils.GetUnitTextures(nextTileUnit, activeInterface, gameScreen.Game, viewElementsNextTileUnits,
                new Vector2(unitDrawOffset[0] * (4 * (gameScreen.Zoom + 8)), unitDrawOffset[1] * (2 * (gameScreen.Zoom + 8))) + ActivePos with { Y = ActivePos.Y - activeInterface.UnitImages.UnitRectangle.Height.ZoomScale(gameScreen.Zoom) + Dimensions.TileHeight });
        }

        // Moving unit view elements
        var viewElementsActiveUnit = new List<IViewElement>();
        ImageUtils.GetUnitTextures(activeUnit, activeInterface, gameScreen.Game, viewElementsActiveUnit,
            ActivePos with { Y = ActivePos.Y - activeInterface.UnitImages.UnitRectangle.Height.ZoomScale(gameScreen.Zoom) + Dimensions.TileHeight }, true);

        SetAnimation(viewElementsPrevTileUnits.Concat(viewElementsNextTileUnits).Concat(viewElementsActiveUnit).ToList());

        var totalFrames = activeUnit.CurrentLocation.CityHere == null ? noFramesForOneMove : noFramesForOneMove - 1;
        for (var frame = 1; frame < totalFrames; frame++)
        {
            var offsetVector = new Vector2(unitDrawOffset[0] * (4 * (gameScreen.Zoom + 8)) / noFramesForOneMove * frame,
                +unitDrawOffset[1] * (2 * (gameScreen.Zoom + 8)) / noFramesForOneMove * frame);
            var animPrevUnit = viewElementsPrevTileUnits.Select(ve => ve.CloneForLocation(ve.Location));
            var animNextUnit = viewElementsNextTileUnits.Select(ve => ve.CloneForLocation(ve.Location));
            var animActiveUnit = viewElementsActiveUnit.Select(ve => ve.CloneForLocation(ve.Location + offsetVector));
            SetAnimation(animPrevUnit.Concat(animNextUnit).Concat(animActiveUnit).ToList());
        }

        if (totalFrames != noFramesForOneMove)
        {
            SetAnimation([]);
        }
    }
}