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

        HorizontalStacking = true;
        Selectable = false;

        ItemSelected += OpenPopup;
    }

    public override void OnResize()
    {
        var active = _cityWindow.MainWindow.ActiveInterface;
        var properties = _cityWindow.CityWindowProps.UnitSupport;

        Rows = properties.Rows;
        Columns = properties.Columns;
        Looks = new ListboxLooks()
        {
            Font = active.Look.CityWindowFont,
            FontSize = active.Look.CityWindowFontSize + (int)(12 * (_cityWindow.Scale - 1)),
            TextColorFront = Color.Black,
            TextColorShadow = Color.Gray
        };

        if (_oldScale != _cityWindow.Scale)
        {
            Groups = MakeEntries(_cityWindow);
            _oldScale = _cityWindow.Scale;
        }

        var pos = _cityWindow.CityWindowProps.UnitSupport.Box;
        Location = new(_cityWindow.LayoutPadding.Left + pos.X * _cityWindow.Scale,
            _cityWindow.LayoutPadding.Top + pos.Y * _cityWindow.Scale);
        Width = (int)(pos.Width * _cityWindow.Scale);
        Height = (int)(pos.Height * _cityWindow.Scale);

        if (Groups.Count <= Columns)
        {
            Height = Height / 2;
            Location = new(Location.X, Location.Y + Height / 2);
        }

        base.OnResize();
    }

    static List<ListboxGroup> MakeEntries(CityWindow cityWindow)
    {
        var units = cityWindow.City.SupportedUnits;
        var properties = cityWindow.CityWindowProps.UnitSupport;

        List<ListboxGroup> groups = [];
        foreach (var unit in units)
        {
            var group = new ListboxGroup()
            {
                Elements = [new ListboxGroupElement { Unit = unit, Game = cityWindow.CurrentGameScreen.Game, ScaleIcon = ImageUtils.ZoomScale(-3 + (int)(6 * (cityWindow.Scale - 1)))}],    // zoom = -6 / -3 / 0
                Height = (int)Math.Ceiling(properties.Box.Height / properties.Rows * cityWindow.Scale)
            };
            groups.Add(group);
        }

        return groups;
    }

    private void OpenPopup(object? sender, ListboxSelectionEventArgs args)
    {
        var city = _cityWindow.City;
        var screen = _cityWindow.CurrentGameScreen;
        var unit = city.SupportedUnits[args.Index];

        screen.ShowPopup("CHILDCLICK", 
            replaceStrings: [$"{unit.Owner.Adjective} {unit.Name}", $"({unit.CurrentLocation.X},{unit.CurrentLocation.Y})"],
            dialogImage: new(unit, screen.MainWindow.ActiveInterface));
    }
}