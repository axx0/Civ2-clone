using System.Diagnostics;
using System.Numerics;
using Civ2engine.Events;
using Civ2engine.MapObjects;
using Raylib_cs;

namespace RaylibUI.RunGame.GameControls.Mapping.Views;

internal class MoveAnimation : BaseGameView
{
    public MoveAnimation(GameScreen gameScreen, MovementEventArgs moveEvent, IGameView? previousView, int viewHeight,
        int viewWidth, bool forceRedraw) : base(gameScreen, moveEvent.Location.First(), previousView, viewHeight, viewWidth, false, 70, moveEvent.Location, forceRedraw)
    {
        var activeInterface = gameScreen.Main.ActiveInterface;
        var activeUnit = moveEvent.Unit;
        var noFramesForOneMove = 8;
        float[] unitDrawOffset = { activeUnit.X - activeUnit.PrevXy[0], activeUnit.Y - activeUnit.PrevXy[1] };
        var map = activeUnit.CurrentLocation.Map;
        var viewElements = new List<IViewElement>();
        ImageUtils.GetUnitTextures(activeUnit, activeInterface, viewElements,
            ActivePos with { Y = ActivePos.Y - activeInterface.UnitImages.UnitRectangle.Height.ZoomScale(map.Zoom) + Dimensions.TileHeight }, true);
        SetAnimation(viewElements);
        var totalFrames = activeUnit.CurrentLocation.CityHere == null ? noFramesForOneMove : noFramesForOneMove - 1;
        for (var frame = 1; frame < totalFrames; frame++)
        {
            var offsetVector = new Vector2(unitDrawOffset[0] * map.Xpx / noFramesForOneMove * frame,
                +unitDrawOffset[1] * map.Ypx / noFramesForOneMove * frame);
            SetAnimation(viewElements.Select(ve => ve.CloneForLocation(ve.Location + offsetVector)).ToList());
        }

        if (totalFrames != noFramesForOneMove)
        {
            SetAnimation(Array.Empty<IViewElement>());
        }
    }
}