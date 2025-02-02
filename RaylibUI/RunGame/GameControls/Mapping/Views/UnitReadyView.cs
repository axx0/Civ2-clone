using Civ2engine.Units;
using RaylibUI.RunGame.GameControls.Mapping.Views.ViewElements;

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
        ImageUtils.GetUnitTextures(unit, activeInterface, gameScreen.Game, elements, ActivePos with{ Y = ActivePos.Y + Dimensions.TileHeight - activeInterface.UnitImages.UnitRectangle.Height.ZoomScale(gameScreen.Zoom)});

        SetAnimation(elements);

        SetAnimation([]);
    }

    public Unit Unit { get; set; }
}