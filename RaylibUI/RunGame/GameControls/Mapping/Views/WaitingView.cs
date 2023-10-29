using Civ2engine.MapObjects;
using Civ2engine.Units;
using Raylib_cs;

namespace RaylibUI.RunGame.GameControls.Mapping.Views;

public class WaitingView : BaseGameView
{
    public WaitingView(GameScreen gameScreen, IGameView? currentView, int viewHeight,
        int viewWidth) : base(gameScreen, gameScreen.Game.ActiveTile,
        currentView, viewHeight, viewWidth, true, 200, Array.Empty<Tile>())
    {
        var activeInterface = gameScreen.Main.ActiveInterface;

            SetAnimation(new[]
            {
                new ViewElement
                {
                    Image = Raylib.LoadTextureFromImage(activeInterface.MapImages.ViewPiece),
                    X = (int)ActivePos.X,
                    Y = (int)ActivePos.Y
                }
            });
     

        SetAnimation(Array.Empty<ViewElement>());
    }
}