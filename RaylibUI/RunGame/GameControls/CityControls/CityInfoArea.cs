using Civ2engine;
using Model;
using Model.Core;
using Raylib_CSharp.Colors;
using Raylib_CSharp.Fonts;
using Raylib_CSharp.Rendering;
using Raylib_CSharp.Transformations;
using RaylibUI.BasicTypes.Controls;
using System.Numerics;

namespace RaylibUI.RunGame.GameControls.CityControls;

public class CityInfoArea : BaseControl
{
    private readonly CityWindow _cityWindow;
    private CityDisplayMode _mode;
    private IUserInterface _active;
    private readonly CityLabel _label;
    private readonly Color _pen1, _pen2;
    private float _fontSize;
    private readonly CityWindowLayout _props;
    private readonly IGame _game;
    private readonly City _city;

    public CityInfoArea(CityWindow controller) : base(controller, false)
    {
        _cityWindow = controller;
        _city = _cityWindow.City;
        _game = controller.CurrentGameScreen.Game;
        _mode = CityDisplayMode.Info;
        _active = controller.MainWindow.ActiveInterface;
        _props = _cityWindow.CityWindowProps;
        _pen1 = new Color(223, 187, 63, 255);
        _pen2 = new Color(67, 67, 67, 255);
        _label = new CityLabel(controller, Labels.For(LabelIndex.UnitsPresent), _active.Look.CityWindowFont, _active.Look.CityWindowFontSize,
            _pen1, _pen2)
        {
            AbsolutePosition = new Rectangle(_props.InfoPanel.X, _props.InfoPanel.Y, _props.InfoPanel.Width, 15)
        };
    }

    public override void OnResize()
    {
        AbsolutePosition = _props.InfoPanel.ScaleAll(_cityWindow.Scale);
        base.OnResize();

        _label.Text = _mode switch
        {
            CityDisplayMode.SupportMap => Labels.For(LabelIndex.SupportMap),
            CityDisplayMode.Happiness => Labels.For(LabelIndex.HappinessAnalysis),
            _ => Labels.For(LabelIndex.UnitsPresent),
        };

        var activPos = new Vector2(Bounds.X, Bounds.Y + _label.TextSize.Y);
        var children = new List<IControl>();
        
        if (!(_mode == CityDisplayMode.Info && _city.UnitsInCity.Count > 5))
        {
            children.Add(_label);
        }
        else
        {
            activPos = new Vector2(Bounds.X, Bounds.Y);
        }

        if (_mode == CityDisplayMode.Info)
        {
            float unitScale = ImageUtils.ZoomScale((int)(6 * _cityWindow.Scale - 8));    // zoom=-5/-2/+1
            int row = 0;
            foreach (var unit in _city.UnitsInCity.AsEnumerable().Reverse())
            {
                var unitDisplay = new UnitDisplay(_cityWindow, unit, _game, activPos, _active, unitScale);
                _fontSize = 13 + 16 * (_cityWindow.Scale - 1);
                var unitLabel = new LabelControl(_cityWindow, ShortCityName(_city), true, font: _active.Look.CityWindowFont,
                    alignment: TextAlignment.Center, colorShadow: new Color(135, 135, 135, 255), shadowOffset: new Vector2(1, 1),
                    fontSize: (int)_fontSize);
                var textDim = TextManager.MeasureTextEx(_active.Look.CityWindowFont, unitLabel.Text, _fontSize, 1);
                unitLabel.Bounds = new Rectangle(activPos.X, activPos.Y + unitDisplay.Height, unitDisplay.Width, textDim.Y);
                children.Add(unitDisplay);
                children.Add(unitLabel);

                activPos = activPos with { X = activPos.X + unitDisplay.Width };
                if (activPos.X + unitDisplay.Width > Bounds.X + Bounds.Width)
                {
                    row++;
                    if (row == 1)
                    {
                        activPos = new Vector2(Bounds.X, activPos.Y + unitDisplay.Height + 3 * _cityWindow.Scale);
                    }
                    else if (row == 2)
                    {
                        activPos = new Vector2(Bounds.X + unitDisplay.Width / 2, Bounds.Y + unitDisplay.Height / 2);
                    }
                    else if (row == 3)
                    {
                        activPos = new Vector2(Bounds.X + unitDisplay.Width / 2, Bounds.Y + 1.5f * unitDisplay.Height + 3 * _cityWindow.Scale);
                    }
                    else
                    {
                        break;
                    }
                }
            }

            children.Add(
                new CityLabel(_cityWindow, $"{Labels.For(LabelIndex.Supplies)}: {string.Join(", ", _city.CommoditySupplied?.Select(c => _game.Rules.CaravanCommoditie[c.Id].Name) ?? Array.Empty<string>())}", _active.Look.CityWindowFont, _active.Look.CityWindowFontSize,
                new Color(227, 83, 15, 255), new Color(67, 67, 67, 255), TextAlignment.Left)
                    { AbsolutePosition = new Rectangle(_props.InfoPanel.X + 5, _props.InfoPanel.Y + 170, 0, 0) });
            children.Add(
                new CityLabel(_cityWindow, $"{Labels.For(LabelIndex.Demands)}: {string.Join(", ", _city.CommodityDemanded?.Select(c => _game.Rules.CaravanCommoditie[c.Id].Name) ?? Array.Empty<string>())}", _active.Look.CityWindowFont, _active.Look.CityWindowFontSize,
                new Color(227, 83, 15, 255), new Color(67, 67, 67, 255), TextAlignment.Left)
                { AbsolutePosition = new Rectangle(_props.InfoPanel.X + 5, _props.InfoPanel.Y + 183, 0, 0) });
        }

        Children = children;

        foreach (var child in Children)
        {
            child.OnResize();
        }
    }

    public override void Draw(bool pulse)
    {
        base.Draw(pulse);

        //Graphics.DrawRectangleLines((int)Bounds.X, (int)Bounds.Y, Width,Height, Color.Magenta);

        if (_mode == CityDisplayMode.SupportMap)
        {
            var map = _cityWindow.CurrentGameScreen.CurrentMap;

            Color _color;
            var sqW = 2 * _cityWindow.Scale;
            var sqH = 1 * _cityWindow.Scale;
            var drawingOffsetX = Bounds.X + Width / 2 - map.XDim * sqW / 2;
            var drawingOffsetY = Bounds.Y + Height / 2 - map.YDim * sqH / 2;

            for (int row = 0; row < map.YDim; row++)
            {
                for (int col = 0; col < map.XDim; col++)
                {
                    _color = map.Tile[col, row].Type == TerrainType.Ocean ? new Color(0, 0, 95, 255) : new Color(55, 123, 23, 255);
                    Graphics.DrawRectangleRec(new Rectangle(drawingOffsetX + sqW * col, drawingOffsetY + sqH * row, sqW, sqH), _color);

                    // Mark this city
                    if (col == (_city.X - _city.Y % 2) / 2 && row == _city.Y)
                    {
                        Graphics.DrawRectangleRec(new Rectangle(drawingOffsetX + sqW * col, drawingOffsetY + sqH * row, sqW, sqH), Color.White);
                    }
                }
            }
            // Mark supported units (omit those in the city)
            foreach (var unit in _city.SupportedUnits.Where(u => u.X != _city.X && u.Y != _city.Y))
            {
                Graphics.DrawRectangleRec(new Rectangle(drawingOffsetX + sqW * unit.X, drawingOffsetY + sqH * unit.Y, sqW, sqH), new Color(159, 159, 159, 255));
            }
        }
    }

    public CityDisplayMode Mode => _mode;


    public void SetActiveMode(CityDisplayMode mode)
    {
        _mode = mode;
        OnResize();
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