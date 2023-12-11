using Civ2engine.Units;
using Raylib_cs;

namespace RaylibUI.RunGame.GameControls.Mapping.Views;

public class UnitReadyView : BaseGameView
{
    public UnitReadyView(GameScreen gameScreen, IGameView? currentView, int viewHeight,
        int viewWidth, Unit unit, bool forceRedraw) : base(gameScreen, unit.CurrentLocation,
        currentView, viewHeight, viewWidth, true, 150, new []{ unit.CurrentLocation }, forceRedraw)
    {
        this.Unit = unit;
        var activeInterface = gameScreen.Main.ActiveInterface;

        var elements = new List<IViewElement>();
        ImageUtils.GetUnitTextures(unit, activeInterface, elements, ActivePos);

        SetAnimation(elements);

        SetAnimation(Array.Empty<TextureElement>());
    }

    public Unit Unit { get; set; }
}