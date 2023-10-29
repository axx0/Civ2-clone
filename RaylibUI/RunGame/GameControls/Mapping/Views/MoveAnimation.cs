using Civ2engine.Events;
using Civ2engine.MapObjects;
using Raylib_cs;

namespace RaylibUI.RunGame.GameControls.Mapping.Views;

internal class MoveAnimation : BaseGameView
{
    public MoveAnimation(GameScreen gameScreen, MovementEventArgs moveEvent, IGameView? previousView, int viewHeight,
        int viewWidth) : base(gameScreen, moveEvent.Location.First(), previousView, viewHeight, viewWidth, false, 70, moveEvent.Location)
    {
        
        var dimensions = gameScreen.TileCache.GetDimensions(moveEvent.Unit.CurrentLocation.Map);
        var relevantPositions = moveEvent.Location.Select(t => GetPosForTile(t, dimensions)).ToList();
        Elements = Elements.Where(e => relevantPositions.All(pos => (int)pos.X != e.X && (int)pos.Y != e.Y)).ToArray();

        var activeInterface = gameScreen.Main.ActiveInterface;
        var activeUnit = moveEvent.Unit;
        int noFramesForOneMove = 8;
        int[] unitDrawOffset = { activeUnit.X - activeUnit.PrevXY[0], activeUnit.Y - activeUnit.PrevXY[1] };
        var map = activeUnit.CurrentLocation.Map;
        for (int frame = 0; frame < noFramesForOneMove; frame++)
        {
            // Draw active unit on top of everything (except if it's city, then don't draw the unit in last frame)
            if (!(frame == noFramesForOneMove - 1 && activeUnit.CurrentLocation.CityHere != null))
            {
                SetAnimation(new[]
                {
                    new ViewElement
                    {
                        Image = Raylib.LoadTextureFromImage(activeInterface.UnitImages
                            .Units[(int)activeUnit.Type].Image),
                        X = (int)ActivePos.X + unitDrawOffset[0] * map.Xpx / noFramesForOneMove * (frame + 1),
                        Y = (int)ActivePos.Y + unitDrawOffset[1] * map.Ypx / noFramesForOneMove * (frame + 1)
                    }
                });
            }
            else
            {
                SetAnimation(Array.Empty<ViewElement>());
            }
        }
    }
}