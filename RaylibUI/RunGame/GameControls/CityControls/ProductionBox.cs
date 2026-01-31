using Civ2engine;
using Civ2engine.Production;
using Model;
using Model.Controls;
using Model.Images;
using Raylib_CSharp.Audio;
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
    private readonly Color _pen1, _pen2;
    private readonly LabelControl _label;
    private readonly ShieldProduction _properties;
    private readonly CityButton _buyButton, _changeButton;
    private float _shieldWidth, _shieldHeight;
    private readonly City _city;
    private readonly IList<IProductionOrder> _canProduce;
    private readonly ImageBox _icon;

    public ProductionBox(CityWindow cityWindow) : base(cityWindow, eventTransparent: false)
    {
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

        _label = new CityLabel(_cityWindow, _cityWindow.CityWindowProps.Labels["ItemInProduction"]);

        _buyButton = new CityButton(cityWindow, "Buy");
        _changeButton = new CityButton(cityWindow, "Change");
        _changeButton.Click += (_, _) =>
        {
            cityWindow.CurrentGameScreen.ShowPopup(
                "PRODUCTION", handleButtonClick: BuildDialogClosed, replaceStrings: [_city.Name], listBox: new ListboxDefinition
                {
                    Type = ListboxType.Default,
                    ImageShift = true,
                    Rows = 13,
                    Groups = _canProduce.Select(p => p.GetBuildListEntry(_active, _city)).ToList(),
                    SelectedId = _canProduce.IndexOf(_city.ItemInProduction)
                });
        };

        _icon = new ImageBox(_cityWindow, _city.ItemInProduction.GetIcon(_active));
        Controls = [_label, _icon, _buyButton, _changeButton];

        ChangeProductionDisplay();
    }

    public override void OnResize()
    {
        Location = new(_cityWindow.LayoutPadding.Left + _properties.Box.X * _cityWindow.Scale,
            _cityWindow.LayoutPadding.Top + _properties.Box.Y * _cityWindow.Scale);
        Width = (int)(_properties.Box.Width * _cityWindow.Scale);
        Height = (int)(_properties.Box.Height * _cityWindow.Scale);
        base.OnResize();

        _shieldWidth = _shieldIcon.Width * _cityWindow.Scale;
        _shieldHeight = _shieldIcon.Height * _cityWindow.Scale;

        if (_city.ItemInProduction.Type == ItemType.Unit)
        {
            _label.Visible = false;
            _icon.Scale = ImageUtils.ZoomScale(-1 + (int)(4 * (_cityWindow.Scale - 1)));
            _icon.Location = new(_properties.IconLocation.X * _cityWindow.Scale - _icon.Width / 2, 0);
        }
        else
        {
            _label.Visible = true;
            _label.Text = _city.ItemInProduction.Title;
            _icon.Scale = _cityWindow.Scale;
            _icon.Location = new(_properties.IconLocation.X * _cityWindow.Scale - _icon.Width / 2, 
                _properties.IconLocation.Y * _cityWindow.Scale);
        }

        foreach (var child in Controls)
        {
            child.OnResize();
        }
    }

    private void ChangeProductionDisplay()
    {
        _icon.Image = [_city.ItemInProduction.GetIcon(_active)];

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

        var posX = Bounds.X + 5 * scale;
        var posY = Bounds.Y + 42 * scale;

        var drawWidth = Width - 42 * scale;
        var spacing = (int)_shieldWidth;
        var requiredWidth = shieldsPerRow * spacing;

        Graphics.DrawLineEx(new Vector2(posX, posY), new Vector2(posX + Width - 15 * scale, posY), 1f, _pen1);
        // 2nd horizontal line
        var lineHeight = 6 * scale + lines * _shieldHeight;
        posY = Bounds.Y + 42 * scale + lineHeight;
        Graphics.DrawLineEx(new Vector2(posX, posY), new Vector2(posX + Width - 15 * scale, posY), 1f, _pen2);
        // 1st vertical line
        posY = Bounds.Y + 42 * scale;
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
                    new Vector2((int)posX + spacing * col, (int)(Bounds.Y + 45 * scale + _shieldHeight * row)),
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