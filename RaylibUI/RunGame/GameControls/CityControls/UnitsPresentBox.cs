using Civ2engine;
using Model.Controls;
using Model.Core.Cities;
using RaylibUI.BasicTypes;
using System;
using System.Drawing;
using System.Text.RegularExpressions;

namespace RaylibUI.RunGame.GameControls.CityControls;

public class UnitsPresentBox : Listbox
{
    private readonly CityWindow _cityWindow;
    private readonly CityInfoArea _infoArea;
    private float _oldScale = 0f;

    public UnitsPresentBox(CityWindow cityWindow, CityInfoArea infoArea) : base(cityWindow)
    {
        _cityWindow = cityWindow;
        _infoArea = infoArea;

        HorizontalStacking = true;
        Selectable = false;

        ItemSelected += OpenPopup;
    }

    public override void OnResize()
    {
        var active = _cityWindow.MainWindow.ActiveInterface;
        var properties = _cityWindow.CityWindowProps.InfoPanel.UnitsPresent;

        Rows = properties.Rows;
        Columns = properties.Columns;
        Looks = new()
        {
            Font = active.Look.CityWindowFont,
            FontSize = active.Look.CityWindowFontSize + (int)(12 * (_cityWindow.Scale - 1)),
            TextColorFront = Raylib_CSharp.Colors.Color.Black,
            TextColorShadow = new Raylib_CSharp.Colors.Color(135, 135, 135, 255)
        };

        if (_oldScale != _cityWindow.Scale)
        {
            Groups = MakeEntries(_cityWindow);
            _oldScale = _cityWindow.Scale;
        }

        var pos = _cityWindow.CityWindowProps.InfoPanel.UnitsPresent.Box;
        Location = new(pos.X * _cityWindow.Scale, pos.Y * _cityWindow.Scale);
        Width = (int)(pos.Width * _cityWindow.Scale);
        Height = (int)(pos.Height * _cityWindow.Scale);

        if (Groups.Count <= Columns)
        {
            Height /= 2;
            Location = new(Location.X, Location.Y + Height / 2);
        }

        Visible = _infoArea.Mode == CityDisplayMode.Info;

        base.OnResize();
    }

    static List<ListboxGroup> MakeEntries(CityWindow cityWindow)
    {
        var units = cityWindow.City.UnitsInCity;
        var properties = cityWindow.CityWindowProps.InfoPanel.UnitsPresent;

        List<ListboxGroup> groups = [];
        foreach (var unit in units)
        {
            var group = new ListboxGroup()
            {
                Elements = [
                    new ListboxGroupElement { Unit = unit, Game = cityWindow.CurrentGameScreen.Game,
                        ScaleIcon = ImageUtils.ZoomScale(-2 + (int)(6 * (cityWindow.Scale - 1)))},    // zoom = -5 / -2 / 1
                    new ListboxGroupElement { Text = ShortCityName(cityWindow.City), Xoffset = 0, 
                        HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Bottom }],
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
        var unit = city.UnitsInCity[args.Index];

        screen.ShowPopup("UNITOPTIONS", 
            replaceStrings: [$"{unit.Owner.Adjective} {unit.Name}", "", $"{unit.HomeCity?.Name ?? "NONE"}"],
            dialogImage: new(unit, screen.MainWindow.ActiveInterface));
    }


    /// <summary>
    /// Returns 3 character city name
    /// </summary>
    /// <param name="city"></param>
    /// <returns></returns>
    private static string ShortCityName(City city)
    {
        return city == null ? "NON" : city.Name.Length < 3 ? city.Name : city.Name[..3];
    }
}