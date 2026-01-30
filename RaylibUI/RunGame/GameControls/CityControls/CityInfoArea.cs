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
            var tradeCity = _game.AllCities[route.Destination];
            var box = new Rectangle(demandsProp.Box.X, demandsProp.Box.Y + 15 + 13 * i, demandsProp.Box.Width, demandsProp.Box.Height);
            _tradeLabels[i] = new CityLabel(controller, new CityLabelProperties($"{tradeCity.Name} {route.Commodity.Name} +xx", 
                box, demandsProp.Color, demandsProp.ColorShadow, HorizontalAlignment.Left, new Vector2(1, 1)));
            Controls.Add(_tradeLabels[i]);
        }

        Controls.Add(new UnitsPresentBox(controller, this));
    }

    public override void OnResize()
    {
        var pos = _props.InfoPanel.Box.ScaleAll(_cityWindow.Scale);
        Location = new(_cityWindow.LayoutPadding.Left + pos.X, _cityWindow.LayoutPadding.Top + pos.Y);
        Width = (int)pos.Width;
        Height = (int)pos.Height;
        base.OnResize();

        _label.Text = _mode switch
        {
            CityDisplayMode.SupportMap => Labels.For(LabelIndex.SupportMap),
            CityDisplayMode.Happiness => Labels.For(LabelIndex.HappinessAnalysis),
            _ => Labels.For(LabelIndex.UnitsPresent),
        };

        var activPos = new Vector2(Location.X, Location.Y + _label.TextSize.Y);

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


        if (!(_mode == CityDisplayMode.Info && _city.UnitsInCity.Count > 5))
        {
            //_label.Visible = false;
        }
        else
        {
            //_label.Visible = true;
            activPos = new Vector2(Location.X, Location.Y);
        }

        if (_mode == CityDisplayMode.Info)
        {
            //Controls.Add(new UnitsPresentBox(_cityWindow));

            //float unitScale = ImageUtils.ZoomScale((int)(6 * _cityWindow.Scale - 8));    // zoom=-5/-2/+1
            //int row = 0;
            //foreach (var unit in _city.UnitsInCity.AsEnumerable().Reverse())
            //{
            //    var unitDisplay = new UnitDisplay(_cityWindow, unit, _game, activPos, _active, unitScale);
            //    _fontSize = 13 + 16 * (_cityWindow.Scale - 1);
            //    var unitLabel = new LabelControl(_cityWindow, ShortCityName(_city), true, font: _active.Look.CityWindowFont,
            //        alignment: TextAlignment.Center, colorShadow: new Color(135, 135, 135, 255), shadowOffset: new Vector2(1, 1),
            //        fontSize: (int)_fontSize);
            //    var textDim = TextManager.MeasureTextEx(_active.Look.CityWindowFont, unitLabel.Text, _fontSize, 1);
            //    unitLabel.Location = new(activPos.X, activPos.Y + unitDisplay.Height);
            //    Width = unitDisplay.Width;
            //    Height = (int)textDim.Y;
            //    Controls.Add(unitDisplay);
            //    Controls.Add(unitLabel);

            //    activPos = activPos with { X = activPos.X + unitDisplay.Width };
            //    if (activPos.X + unitDisplay.Width > Location.X + Width)
            //    {
            //        row++;
            //        if (row == 1)
            //        {
            //            activPos = new Vector2(Location.X, activPos.Y + unitDisplay.Height + 3 * _cityWindow.Scale);
            //        }
            //        else if (row == 2)
            //        {
            //            activPos = new Vector2(Location.X + unitDisplay.Width / 2, Location.Y + unitDisplay.Height / 2);
            //        }
            //        else if (row == 3)
            //        {
            //            activPos = new Vector2(Location.X + unitDisplay.Width / 2, Location.Y + 1.5f * unitDisplay.Height + 3 * _cityWindow.Scale);
            //        }
            //        else
            //        {
            //            break;
            //        }
            //    }
            //}

            //Controls.Add(
            //    new CityLabel(_cityWindow, $"{Labels.For(LabelIndex.Supplies)}: " +
            //    $"{string.Join(", ", _city.CommoditySupplied?.Select(c => _game.Rules.CaravanCommoditie[c.Id].Name) ?? 
            //    Array.Empty<string>())}", new Color(227, 83, 15, 255), new Color(67, 67, 67, 255), TextAlignment.Left)
            //    {
            //        Location = new(_props.InfoPanel.X + 5, _props.InfoPanel.Y + 170),
            //        Width = 0,
            //        Height = 0
            //    });
            //Controls.Add(
            //    new CityLabel(_cityWindow, $"{Labels.For(LabelIndex.Demands)}: " +
            //    $"{string.Join(", ", _city.CommodityDemanded?.Select(c => _game.Rules.CaravanCommoditie[c.Id].Name) ?? 
            //    Array.Empty<string>())}", new Color(227, 83, 15, 255), new Color(67, 67, 67, 255), TextAlignment.Left)
            //    {
            //        Location = new(_props.InfoPanel.X + 5, _props.InfoPanel.Y + 183),
            //        Width = 0,
            //        Height = 0
            //    });
        }

        //Controls = Controls;

        foreach (var child in Controls)
        {
            child.OnResize();
        }
    }

    public override void Draw(bool pulse)
    {
        base.Draw(pulse);

        //Graphics.DrawRectangleLinesEx(Bounds, 1f, Color.Magenta);

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