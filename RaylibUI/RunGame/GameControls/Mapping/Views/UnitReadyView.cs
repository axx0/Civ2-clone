using Civ2engine.Units;
using Raylib_cs;

namespace RaylibUI.RunGame.GameControls.Mapping.Views;

public class UnitReadyView : BaseGameView
{
    public UnitReadyView(GameScreen gameScreen, IGameView? currentView, int viewHeight,
        int viewWidth, Unit unit) : base(gameScreen, unit.CurrentLocation,
        currentView, viewHeight, viewWidth, true, 150, new []{ unit.CurrentLocation })
    {
        this.Unit = unit;
        var activeInterface = gameScreen.Main.ActiveInterface;

        var elements = new List<ViewElement>();
        elements.Add(new ViewElement
        {
            Image = Raylib.LoadTextureFromImage(ImageUtils.GetUnitImage(activeInterface, unit)),
            X = (int)ActivePos.X,
            Y = (int)ActivePos.Y
        });

        SetAnimation(elements);

        SetAnimation(Array.Empty<ViewElement>());
    }

    public Unit Unit { get; set; }
}