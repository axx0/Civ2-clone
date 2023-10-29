using Civ2engine.Units;
using Raylib_cs;

namespace RaylibUI.RunGame.GameControls.Mapping.Views;

public class UnitReadyView : BaseGameView
{
    public UnitReadyView(GameScreen gameScreen, IGameView? currentView, int viewHeight,
        int viewWidth, Unit unit) : base(gameScreen, unit.CurrentLocation,
        currentView, viewHeight, viewWidth, true, 200, new []{ unit.CurrentLocation })
    {
        this.Unit = unit;
        var activeInterface = gameScreen.Main.ActiveInterface;

        SetAnimation(new[]
        {
            new ViewElement
            {
                Image = Raylib.LoadTextureFromImage(activeInterface.UnitImages.Units[(int)unit.Type].Image),
                X = (int)ActivePos.X,
                Y = (int)ActivePos.Y
            }
        });


        SetAnimation(Array.Empty<ViewElement>());
    }

    public Unit Unit { get; set; }
}