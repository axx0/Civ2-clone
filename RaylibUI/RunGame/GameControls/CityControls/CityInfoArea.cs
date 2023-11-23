using Raylib_cs;

namespace RaylibUI.RunGame.GameControls.CityControls;

public class CityInfoArea : BaseControl
{
    public CityInfoArea(IControlLayout controller, bool eventTransparent = false) : base(controller, eventTransparent)
    {
        this.Mode = DisplayMode.Info;
    }

    public override void Draw(bool pulse)
    {
        Raylib.DrawRectangleLines((int)Bounds.x, (int)Bounds.y, Width,Height,Color.MAGENTA);
        Raylib.DrawTextEx(Fonts.DefaultFont,  Mode.ToString(), Location, 20,1,Color.MAGENTA );
    }

    public DisplayMode Mode { get; set; }
}

public enum DisplayMode
{
    Info,
    SupportMap,
    Happiness
}