using Model;
using Model.Controls;
using Raylib_CSharp.Colors;
using RaylibUI.BasicTypes;

namespace RaylibUI.RunGame.GameControls.CityControls;

public class UnitSupportBox : Listbox
{
    private readonly CityWindow _cityWindow;
    private float _oldScale = 0f;

    public UnitSupportBox(CityWindow cityWindow) : base(cityWindow)
    {
        _cityWindow = cityWindow;
        ItemSelected += OpenPopup;
    }

    public override void OnResize()
    {
        if (_oldScale != _cityWindow.Scale)
        {
            Definition = MakeListbox(_cityWindow);
            _oldScale = _cityWindow.Scale;
        }

        var pos = _cityWindow.CityWindowProps.UnitSupport.Box;
        Location = new(_cityWindow.LayoutPadding.Left + pos.X * _cityWindow.Scale,
            _cityWindow.LayoutPadding.Top + pos.Y * _cityWindow.Scale);
        Width = (int)(pos.Width * _cityWindow.Scale);
        Height = (int)(pos.Height * _cityWindow.Scale);

        base.OnResize();
    }

    static ListboxDefinition MakeListbox(CityWindow cityWindow)
    {
        var units = cityWindow.City.SupportedUnits;
        var active = cityWindow.MainWindow.ActiveInterface;
        var properties = cityWindow.CityWindowProps.UnitSupport;

        List<ListboxGroup> groups = [];
        foreach (var unit in units)
        {
            var group = new ListboxGroup()
            {
                Elements = [new ListboxGroupElement { Unit = unit, Game = cityWindow.CurrentGameScreen.Game, ScaleIcon = ImageUtils.ZoomScale(0) * 0.82f}],
                Height = (int)Math.Ceiling(properties.Box.Height / properties.Rows * cityWindow.Scale)
            };
            groups.Add(group);
        }

        return new ListboxDefinition()
        {
            Rows = properties.Rows,
            Columns = properties.Columns,
            HorizontalStacking = true,
            Selectable = false,
            Looks = new ListboxLooks()
            {
                Font = active.Look.CityWindowFont,
                FontSize = Math.Max(10, (int)Math.Round(active.Look.CityWindowFontSize * cityWindow.Scale * 0.72f)),
                TextColorFront = Color.Black,
                TextColorShadow = Color.Gray
            },
            Groups = groups
        };
    }

    private void OpenPopup(object? sender, ListboxSelectionEventArgs args)
    {
        var units = _cityWindow.City.SupportedUnits;
        if (args.Index < 0 || args.Index >= units.Count)
        {
            return;
        }

        CityUnitMenu.Show(_cityWindow, units[args.Index]);
    }
}
