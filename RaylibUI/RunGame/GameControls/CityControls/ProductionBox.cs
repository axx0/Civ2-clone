using Civ2engine;
using Civ2engine.Production;
using Model;
using Model.CityWindowModel;
using Model.Images;
using Model.Interface;
using Raylib_CSharp.Colors;
using Raylib_CSharp.Fonts;
using Raylib_CSharp.Rendering;
using Raylib_CSharp.Textures;
using Raylib_CSharp.Transformations;
using RaylibUI.BasicTypes.Controls;
using RaylibUI.Controls;
using System.Numerics;

namespace RaylibUI.RunGame.GameControls.CityControls;

public class ProductionBox : BaseControl
{
    private readonly CityWindow _cityWindow;
    private readonly Texture2D _shieldIcon;
    private readonly IUserInterface _active;
    private int _totalCost;
    private readonly int _shieldBoxRows;
    private readonly Color _pen1, _pen2, _penTitle;
    private readonly LabelControl _label;
    private readonly ShieldProduction _properties;
    private readonly CityButton _buyButton, _changeButton;
    private float _shieldWidth, _shieldHeight;
    private readonly City _city;
    private readonly IList<IProductionOrder> _canProduce;
    private readonly ImageBox _iconControl;
    private readonly IImageSource _icon;

    public ProductionBox(CityWindow cityWindow) : base(cityWindow, eventTransparent: false)
    {
        _penTitle = new Color(63, 79, 167, 255);
        _pen1 = new Color(83, 103, 191, 255);
        _pen2 = new Color(0, 0, 95, 255);
        _shieldBoxRows = cityWindow.CurrentGameScreen.Game.Rules.Cosmic.RowsShieldBox;
        _cityWindow = cityWindow;
        _city = cityWindow.City;
        _canProduce = ProductionPossibilities.GetAllowedProductionOrders(_city);
        _active = cityWindow.MainWindow.ActiveInterface;
        _properties = _cityWindow.CityWindowProps.Production;
        _shieldIcon = TextureCache.GetImage(_active.ResourceImages
            .First(r => r.Name == "Shields")
            .LargeImage);

        _label = new CityLabel(cityWindow, "", _active.Look.CityWindowFont, _active.Look.CityWindowFontSize, _penTitle, Color.Black)
        {
            AbsolutePosition = new Rectangle(_properties.Box.X, _properties.Box.Y, _properties.Box.Width, 19)
        };

        _buyButton = new CityButton(cityWindow, Labels.For(LabelIndex.Buy), _active.Look.CityWindowFont, _active.Look.CityWindowFontSize)
        {
            AbsolutePosition = _properties.BuyButtonBounds
        };
        _changeButton = new CityButton(cityWindow, Labels.For(LabelIndex.Change), _active.Look.CityWindowFont, _active.Look.CityWindowFontSize)
        {
            AbsolutePosition = _properties.ChangeButtonBounds
        };
        _changeButton.Click += (_, _) =>
        {
            cityWindow.CurrentGameScreen.ShowPopup(
                "PRODUCTION", handleButtonClick: BuildDialogClosed, replaceStrings: [_city.Name], listBox: new ListBoxDefinition
                {
                    Vertical = true,
                    Entries = _canProduce.Select(p => p.GetBuildListEntry(_active)).ToList(),
                    InitialSelection = _canProduce.IndexOf(_city.ItemInProduction)
                });
        };

        _iconControl = new ImageBox(_cityWindow, _city.ItemInProduction.GetIcon(_active));
        Children = [_label, _iconControl, _buyButton, _changeButton];

        ChangeProductionDisplay();
    }

    public override void OnResize()
    {
        AbsolutePosition = _properties.Box.ScaleAll(_cityWindow.Scale);
        base.OnResize();

        _shieldWidth = _shieldIcon.Width * _cityWindow.Scale;
        _shieldHeight = _shieldIcon.Height * _cityWindow.Scale;

        if (_city.ItemInProduction.Type == ItemType.Unit)
        {
            _label.Text = "";
            _iconControl.Scale = ImageUtils.ZoomScale(-1 + (int)(4 * (_cityWindow.Scale - 1)));
            var iconW = _iconControl.GetPreferredWidth();
            var iconH = _iconControl.GetPreferredHeight();
            _iconControl.Bounds = new Rectangle(Location.X + _properties.IconLocation.X * _cityWindow.Scale - iconW / 2,
                Location.Y, iconW, iconH);
        }
        else
        {
            _label.Text = _city.ItemInProduction.Title;
            _iconControl.Scale = 1f;
            var iconW = _iconControl.GetPreferredWidth();
            var iconH = _iconControl.GetPreferredHeight();
            _iconControl.Bounds = new Rectangle(Location.X + _properties.IconLocation.X * _cityWindow.Scale - iconW / 2,
                Location.Y + _properties.IconLocation.Y * _cityWindow.Scale, iconW, iconH);
        }

        foreach (var child in Children)
        {
            child.OnResize();
        }
    }

    private void ChangeProductionDisplay()
    {
        _iconControl.Image = [_city.ItemInProduction.GetIcon(_active)];

        OnResize();

        if (_properties.Type == "Box")
        {
            UpdateData(_city.ItemInProduction);
        }
    }

    private void BuildDialogClosed(string button, int selectedIndex, IList<bool>? chx, IDictionary<string, string>? txt)
    {
        if (button == Labels.Ok && selectedIndex != -1 && _city.ItemInProduction != _canProduce[selectedIndex])
        {
            _city.ItemInProduction = _canProduce[selectedIndex];

            ChangeProductionDisplay();
        }
    }

    public override void Draw(bool pulse)
    {
        base.Draw(pulse);

        //Graphics.DrawRectangleLinesEx(Bounds, 1f, Color.Yellow);

        var scale = _cityWindow.Scale;

        int lines = (int)((Height - 48 * scale) / _shieldHeight);
        int requiredLines = _city.ItemInProduction.Cost;
        int shieldsPerRow = _shieldBoxRows;
        if (lines > requiredLines)
        {
            lines = requiredLines;
        }
        else
        {
            shieldsPerRow = (int)Math.Ceiling(_totalCost / (decimal)lines);
        }

        var posX = Location.X + 5 * scale;
        var posY = Location.Y + 42 * scale;

        var drawWidth = Width - 42 * scale;
        var spacing = (int)_shieldWidth;
        var requiredWidth = shieldsPerRow * spacing;

        Graphics.DrawLineEx(new Vector2(posX, posY), new Vector2(posX + Width - 15 * scale, posY), 1f, _pen1);
        // 2nd horizontal line
        var lineHeight = 6 * scale + lines * _shieldHeight;
        posY = Location.Y + 42 * scale + lineHeight;
        Graphics.DrawLineEx(new Vector2(posX, posY), new Vector2(posX + Width - 15 * scale, posY), 1f, _pen2);
        // 1st vertical line
        posY = Location.Y + 42 * scale;
        Graphics.DrawLineEx(new Vector2(posX, posY), new Vector2(posX, posY + lineHeight), 1f, _pen1);

        // 2nd vertical line
        Graphics.DrawLineEx(new Vector2(posX + Width - 15 * scale, posY), new Vector2(posX + Width - 15 * scale, posY + lineHeight), 1f, _pen2);

        if (requiredWidth < drawWidth)
        {
            posX += (drawWidth - requiredWidth) / 2f;
        }
        else
        {
            spacing = (int)(drawWidth - _shieldWidth) / shieldsPerRow;
        }
        var shields = _city.ShieldsProgress;
        int count = 0;
        posX += 3 * scale;
        for (int row = 0; row < 10 && count < shields; row++)
        {
            for (int col = 0; col < shieldsPerRow && count < shields; col++)
            {
                Graphics.DrawTextureEx(_shieldIcon, 
                    new Vector2((int)posX + spacing * col, (int)(Location.Y + 45 * scale + _shieldHeight * row)),
                    0f, scale, Color.White);
                count++;
            }
        }
    }

    public void UpdateData(IProductionOrder itemInProduction)
    {
        _totalCost = itemInProduction.Cost * _shieldBoxRows;
    }
}