using Civ2engine.MapObjects;
using Civ2engine.Units;
using Raylib_cs;

namespace RaylibUI.RunGame.GameControls.Mapping.Views;

public class WaitingView : BaseGameView
{
    public WaitingView(GameScreen gameScreen, IGameView? currentView, int viewHeight,
        int viewWidth, bool forceRedraw) : base(gameScreen, gameScreen.Game.ActiveTile,
        currentView, viewHeight, viewWidth, true, 200, Array.Empty<Tile>(), forceRedraw)
    {
        var activeInterface = gameScreen.Main.ActiveInterface;

        SetAnimation(new[]
        {
            new TextureElement(texture: activeInterface.MapImages.ViewPieceTexture,
                location: ActivePos, gameScreen.Game.ActiveTile)
        });


        SetAnimation(Array.Empty<TextureElement>());
    }
}