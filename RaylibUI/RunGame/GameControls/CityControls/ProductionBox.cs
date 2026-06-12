using Civ2engine;
using Civ2engine.Production;
using Model;
using Model.Controls;
using Raylib_CSharp.Colors;
using Raylib_CSharp.Rendering;
using Raylib_CSharp.Textures;
using RaylibUI.BasicTypes.Controls;
using System.Numerics;
using Civ2engine.IO;
using Model.Core.Cities;
using Model.Core.Production;

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
                    ImageShift = true,
                    Rows = Math.Min(9, _canProduce.Count),
                    Looks = new ListboxLooks
                    {
                        Font = _active.Look.DefaultFont,
                        FontSize = 18,
                        TextColorFront = Color.Black,
                        TextColorShadow = Color.Blank,
                        SelectedTextFont = _active.Look.DefaultFont,
                        SelectedTextBackgroundColor = new Color(107, 107, 107, 255),
                        SelectedTextColorFront = Color.White,
                        SelectedTextColorShadow = Color.Black
                    },
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

        var activeOrder = GetDisplayedOrder();
        if (activeOrder.Type == ItemType.Unit)
        {
            _label.Visible = false;
            _icon.Scale = ImageUtils.ZoomScale(1);
            _icon.Location = new(_properties.IconLocation.X * _cityWindow.Scale - _icon.Width / 2, 0);
        }
        else
        {
            _label.Visible = true;
            _label.Text = activeOrder.Title;
            _icon.Scale = Math.Min(_cityWindow.Scale, 1.25f);
            _icon.Location = new(_properties.IconLocation.X * _cityWindow.Scale - _icon.Width / 2,
                _properties.IconLocation.Y * _cityWindow.Scale);
        }

        foreach (var child in Controls)
        {
            child.OnResize();
        }
    }

    private IProductionOrder GetDisplayedOrder()
    {
        return _city.ConstructionQueue.Current?.Order ?? _city.ItemInProduction;
    }

    private int GetDisplayedProgress()
    {
        var queuedItem = _city.ConstructionQueue.Current;
        return queuedItem == null
            ? _city.ShieldsProgress
            : queuedItem.TotalCost - queuedItem.RemainingCost;
    }

    private void ChangeProductionDisplay()
    {
        var activeOrder = GetDisplayedOrder();
        _icon.Image = [activeOrder.GetIcon(_active)];

        OnResize();

        if (_properties.Type == "Box")
        {
            UpdateData(activeOrder);
        }
    }

    private void BuildDialogClosed(string button, int selectedIndex, IList<bool>? chx, IDictionary<string, string>? txt)
    {
        if (button != Labels.Ok || _canProduce.Count == 0)
        {
            return;
        }

        selectedIndex = Math.Clamp(selectedIndex, 0, _canProduce.Count - 1);
        var selectedOrder = _canProduce[selectedIndex];
        if (_city.ItemInProduction != selectedOrder)
        {
            _city.ItemInProduction = selectedOrder;
            _city.ConstructionQueue.Clear();
            _city.ConstructionQueue.Enqueue(selectedOrder, _shieldBoxRows);
            _city.ShieldsProgress = 0;

            ChangeProductionDisplay();
        }
    }

    public override void Draw(bool pulse)
    {
        base.Draw(pulse);

        var scale = _cityWindow.Scale;
        var queueItems = _city.ConstructionQueue.Items;

        var queueTextY = Bounds.Y + 45 * scale;
        var queueTextOffset = 0f;

        for (var i = 0; i < queueItems.Count; i++)
        {
            var item = queueItems[i];
            var progress = item.TotalCost <= 0
                ? 1f
                : (item.TotalCost - item.RemainingCost) / (float)item.TotalCost;
            var text = $"[{i + 1}] {item.Name}: {(int)(progress * 100)}%";
            Graphics.DrawTextEx(_active.Look.CityWindowFont, text,
                new Vector2(Bounds.X + 5 * scale, queueTextY + queueTextOffset),
                12 * scale, 0f, Color.White);
            queueTextOffset += 20 * scale;
        }

        var activeOrder = GetDisplayedOrder();
        var progressShields = Math.Min(GetDisplayedProgress(), _totalCost);
        DrawShieldProgress(activeOrder, progressShields, queueTextOffset);
    }

    private void DrawShieldProgress(IProductionOrder activeOrder, int progressShields, float queueTextOffset)
    {
        var scale = _cityWindow.Scale;
        var lines = (int)((Height - 48 * scale - queueTextOffset) / _shieldHeight);
        var requiredLines = activeOrder.Cost;
        var shieldsPerRow = _shieldBoxRows;
        if (lines <= 0)
        {
            return;
        }

        if (lines > requiredLines)
        {
            lines = requiredLines;
        }
        else
        {
            shieldsPerRow = (int)Math.Ceiling(_totalCost / (decimal)lines);
        }

        var posX = Bounds.X + 5 * scale;
        var posY = Bounds.Y + 42 * scale + queueTextOffset;
        var drawWidth = Width - 42 * scale;
        var spacing = (int)_shieldWidth;
        var requiredWidth = shieldsPerRow * spacing;

        Graphics.DrawLineEx(new Vector2(posX, posY), new Vector2(posX + Width - 15 * scale, posY), 1f, _pen1);
        var lineHeight = 6 * scale + lines * _shieldHeight;
        posY = Bounds.Y + 42 * scale + queueTextOffset + lineHeight;
        Graphics.DrawLineEx(new Vector2(posX, posY), new Vector2(posX + Width - 15 * scale, posY), 1f, _pen2);

        posY = Bounds.Y + 42 * scale + queueTextOffset;
        Graphics.DrawLineEx(new Vector2(posX, posY), new Vector2(posX, posY + lineHeight), 1f, _pen1);
        Graphics.DrawLineEx(new Vector2(posX + Width - 15 * scale, posY), new Vector2(posX + Width - 15 * scale, posY + lineHeight), 1f, _pen2);

        if (requiredWidth < drawWidth)
        {
            posX += (drawWidth - requiredWidth) / 2f;
        }
        else
        {
            spacing = (int)(drawWidth - _shieldWidth) / shieldsPerRow;
        }

        var count = 0;
        posX += 3 * scale;
        for (var row = 0; row < lines && count < progressShields; row++)
        {
            for (var col = 0; col < shieldsPerRow && count < progressShields; col++)
            {
                Graphics.DrawTextureEx(_shieldIcon,
                    new Vector2((int)posX + spacing * col, (int)(Bounds.Y + 45 * scale + queueTextOffset + _shieldHeight * row)),
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
