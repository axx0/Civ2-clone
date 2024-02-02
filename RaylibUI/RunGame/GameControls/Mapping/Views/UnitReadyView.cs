using Civ2engine.Units;
using Raylib_cs;
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
        ImageUtils.GetUnitTextures(unit, activeInterface, elements, ActivePos with{ Y = ActivePos.Y + Dimensions.TileHeight - activeInterface.UnitImages.UnitRectangle.Height});

        SetAnimation(elements);

        SetAnimation(Array.Empty<TextureElement>());
    }

    public Unit Unit { get; set; }
}