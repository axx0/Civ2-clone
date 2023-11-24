using Model;

namespace RaylibUI.RunGame.GameControls.CityControls;

public class ResourceProductionBar : BaseControl
{
    private readonly CityWindow _cityWindow;
    private readonly ResourceArea _resource;

    public ResourceProductionBar(CityWindow cityWindow, ResourceArea resource) : base(cityWindow)
    {
        _cityWindow = cityWindow;
        _resource = resource;
        AbsolutePosition = resource.Bounds;
    }
}