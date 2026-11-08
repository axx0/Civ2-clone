using Model.Controls;
using Raylib_CSharp.Colors;
using RaylibUI.BasicTypes;

namespace RaylibUI.RunGame.GameControls.CityControls;

public class UnitSupportBox : Listbox
{
    private readonly CityWindow _cityWindow;
    private readonly CityLabel _label;
    private float _oldScale = 0f;

    /// <param name="label">The "Units supported" label; shown only when the units fit in one row.</param>
    public UnitSupportBox(CityWindow cityWindow, CityLabel label) : base(cityWindow)
    {
        _cityWindow = cityWindow;
        _label = label;

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
            FontSize = _cityWindow.ScaledFontSize,
            TextColorFront = Color.Black,
            TextColorShadow = Color.Gray
        };

        if (_oldScale != _cityWindow.Scale)
        {
            Groups = MakeEntries(_cityWindow);
            _oldScale = _cityWindow.Scale;
        }

        _cityWindow.Place(this, properties.Box);

        // All units fit in one row: half the height, centred vertically in the box, and the label shows
        _label.Visible = Groups.Count <= Columns;
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
                // Rounded down, so all the rows fit in the box's height
                Height = (int)(properties.Box.Height / properties.Rows * cityWindow.Scale)
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