using Civ2engine;
using Model;
using Model.Controls;
using Model.Core;
using Raylib_CSharp.Colors;
using Raylib_CSharp.Fonts;
using Raylib_CSharp.Rendering;
using Raylib_CSharp.Transformations;
using RaylibUI.BasicTypes.Controls;
using RaylibUtils;
using System.Numerics;
using Civ2engine.IO;
using Model.Core.Cities;
using Model.Core.Mapping;

namespace RaylibUI.RunGame.GameControls.CityControls;

public class CityInfoArea : BaseControl
{
    private readonly CityWindow _cityWindow;
    private CityDisplayMode _mode;
    private IUserInterface _active;
    private readonly CityLabel _label, _suppliesLabel, _demandsLabel;
    private readonly CityLabel[] _tradeLabels;
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
        _label = new CityLabel(controller, _props.Labels["UnitsPresent"]);
        Controls.Add(_label);

        var suppliesProp = _props.Labels["Supplies"];
        var text = $"{suppliesProp.Text}: ";
        if (_city.CommoditySupplied == null)
        {
            text = String.Empty;
        }
        for (var i = 0; i < _city.CommoditySupplied?.Length; i++)
        {
            text += _city.CommoditySupplied[i].Name;
            if (i < _city.CommoditySupplied?.Length - 1)
            {
                text += ", ";
            }
        }
        _suppliesLabel = new CityLabel(controller, suppliesProp)
        {
            Text = text,
            HorizontalAlignment = HorizontalAlignment.Left,
        };
        Controls.Add(_suppliesLabel);

        var demandsProp = _props.Labels["Demands"];
        text = $"{demandsProp.Text}: ";
        if (_city.CommodityDemanded == null)
        {
            text = String.Empty;
        }
        for (var i = 0; i < _city.CommodityDemanded?.Length; i++)
        {
            text += _city.CommodityDemanded[i].Name;
            if (i < _city.CommodityDemanded?.Length - 1)
            {
                text += ", ";
            }
        }
        _demandsLabel = new CityLabel(controller, demandsProp)
        {
            Text = text,
            HorizontalAlignment = HorizontalAlignment.Left,
        };
        Controls.Add(_demandsLabel);

        var noRoutes = _city.TradeRoutes?.Length ?? 0;
        if (noRoutes > 0) 
        {
            _tradeLabels = new CityLabel[noRoutes];
        }
        for (var i = 0; i < noRoutes; i++)
        {
            var route = _city.TradeRoutes[i];
            var tradeCity = _game.AllCities.ElementAt(route.Destination);
            var box = new Rectangle(demandsProp.Box.X, demandsProp.Box.Y + 15 + 13 * i, demandsProp.Box.Width, demandsProp.Box.Height);
            _tradeLabels[i] = new CityLabel(controller, new CityLabelProperties($"{tradeCity.Name} {route.Commodity.Name} +xx", 
                box, demandsProp.Color, demandsProp.ColorShadow, HorizontalAlignment.Left, new Vector2(1, 1)));
            Controls.Add(_tradeLabels[i]);
        }

        Controls.Add(new UnitsPresentBox(controller, this));
    }

    public override void OnResize()
    {
        _cityWindow.Place(this, _props.InfoPanel.Box);
        base.OnResize();

        _label.Text = _mode switch
        {
            CityDisplayMode.SupportMap => Labels.For(LabelIndex.SupportMap),
            CityDisplayMode.Happiness => Labels.For(LabelIndex.HappinessAnalysis),
            _ => Labels.For(LabelIndex.UnitsPresent),
        };

        // In Info mode the label shows only while the units present fit in one row (as for the units supported box)
        _label.Visible = !(_mode == CityDisplayMode.Info && _city.UnitsInCity.Count > _props.InfoPanel.UnitsPresent.Columns);
        _suppliesLabel.Visible = _mode == CityDisplayMode.Info;
        _demandsLabel.Visible = _mode == CityDisplayMode.Info;
        if (_tradeLabels != null)
        {
            for (var i = 0; i < _tradeLabels.Length; i++)
            {
                _tradeLabels[i].Visible = _mode == CityDisplayMode.Info;
            }
        }

        foreach (var child in Controls)
        {
            child.OnResize();
        }
    }

    public override void Draw(bool pulse)
    {
        base.Draw(pulse);

        if (_mode == CityDisplayMode.Info && _tradeLabels != null)
        {
            for (var i = 0; i < _tradeLabels.Length; i++)
            {
                Graphics.DrawTextureEx(TextureCache.GetImage(_active.PicSources["trade,small"][0]),
                    new(_tradeLabels[i].Bounds.X + _tradeLabels[i].TextSize.X + 5, _tradeLabels[i].Bounds.Y), 
                    0, _cityWindow.Scale, Color.White);
            }
                
        }
        else if (_mode == CityDisplayMode.SupportMap)
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
}