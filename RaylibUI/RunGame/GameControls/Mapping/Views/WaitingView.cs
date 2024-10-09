using Civ2engine.MapObjects;
using Civ2engine.Units;
using RaylibUI.RunGame.GameControls.Mapping.Views.ViewElements;

namespace RaylibUI.RunGame.GameControls.Mapping.Views;

public class WaitingView : BaseGameView
{
    public WaitingView(GameScreen gameScreen, IGameView? currentView, int viewHeight,
        int viewWidth, bool forceRedraw) : base(gameScreen, gameScreen.Game.ActivePlayer.ActiveTile,
        currentView, viewHeight, viewWidth, true, 200, Array.Empty<Tile>(), forceRedraw)
    {
        var activeInterface = gameScreen.Main.ActiveInterface;

        SetAnimation(new[]
        {
            new TextureElement(texture: TextureCache.GetImage(activeInterface.MapImages.ViewPiece),
                location: ActivePos, gameScreen.Game.ActivePlayer.ActiveTile)
        });


        SetAnimation(Array.Empty<TextureElement>());
    }
}