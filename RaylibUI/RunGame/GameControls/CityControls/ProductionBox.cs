using Civ2engine;
using Civ2engine.IO;
using Civ2engine.Production;
using Model;
using Model.Controls;
using Model.Core.Cities;
using Model.Core.Production;
using Model.Images;
using Raylib_CSharp.Colors;
using Raylib_CSharp.Rendering;
using Raylib_CSharp.Textures;
using Raylib_CSharp.Transformations;
using RaylibUI.BasicTypes.Controls;
using RaylibUtils;
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
    private const float ShieldBoxTop = 42f;
    private const float ProductionIconSlotWidth = 34f;
    private const float ProductionIconSlotHeight = 28f;
    private const float BuyButtonX = 5f;
    private const float ChangeButtonX = 120f;
    private const float ProductionButtonY = 16f;
    private const float Padding = 5f;

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

        _buyButton = new CityButton(cityWindow, "Buy") { ManualLayout = true };
        _changeButton = new CityButton(cityWindow, "Change") { ManualLayout = true };
        _changeButton.Click += (_, _) =>
        {
            cityWindow.CurrentGameScreen.ShowPopup(
                "PRODUCTION", handleButtonClick: BuildDialogClosed, replaceStrings: [_city.Name], listBox: new ListboxDefinition
                {
                    ImageShift = false,
                    Rows = Math.Min(9, _canProduce.Count),
                    Looks = new ListboxLooks
                    {
                        Font = _active.Look.CityWindowFont,
                        FontSize = 16,
                        TextColorFront = Color.Black,
                        TextColorShadow = Color.Blank,
                        TextShadowOffset = Vector2.Zero,
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

        // Do not draw the production icon through ImageBox. The icon source can
        // be redirected by art-replacement code, and ImageBox parent discovery has
        // repeatedly allowed 1024px FOSS unit art to escape the production panel.
        // ProductionBox draws the icon itself into a fixed slot below.
        _icon.Visible = false;
        Controls = [_label, _buyButton, _changeButton];

        // CityWindow inserts this control into the tree after the constructor.
        // Do not call OnResize here; child controls can resolve the CityWindow
        // as their parent before this ProductionBox is discoverable, which makes
        // production icons/buttons draw in the wrong coordinate space.
        UpdateData(GetDisplayedOrder());
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
        _label.Visible = true;
        _label.Text = activeOrder.Title;

        foreach (var child in Controls)
        {
            child.OnResize();
        }

        LayoutProductionHeader(activeOrder);
        LayoutActionButtons();
    }

    private void LayoutProductionHeader(IProductionOrder activeOrder)
    {
        var scale = _cityWindow.Scale;

        // Match the original Civ2 production header: unit builds show only the
        // small unit icon between Buy and Change; buildings/wonders can still
        // show their title in the small top label area.
        _label.Visible = activeOrder.Type != ItemType.Unit;
        if (_label.Visible)
        {
            _label.Text = activeOrder.Title;
            _label.OnResize();
        }

        var slotWidth = Math.Max(1, (int)Math.Round(ProductionIconSlotWidth * scale));
        var slotHeight = Math.Max(1, (int)Math.Round(ProductionIconSlotHeight * scale));
        FitIconInSlot(activeOrder.GetIcon(_active), slotWidth, slotHeight);

        // IconLocation is already relative to the production box in Civ2Interface.
        _icon.Location = new(
            _properties.IconLocation.X * scale - _icon.Width / 2f,
            Math.Max(0, _properties.IconLocation.Y * scale - _icon.Height / 2f));
    }

    private void FitIconInSlot(IImageSource? source, int slotWidth, int slotHeight)
    {
        _icon.Image = [source];
        _icon.FitIntoSlot(slotWidth, slotHeight, padding: Math.Max(1, (int)Math.Round(_cityWindow.Scale)), maxScale: Math.Min(_cityWindow.Scale, 1.25f));
    }

    private void LayoutActionButtons()
    {
        var scale = _cityWindow.Scale;
        var buyProps = _cityWindow.CityWindowProps.Buttons["Buy"];
        var changeProps = _cityWindow.CityWindowProps.Buttons["Change"];

        LayoutButton(_buyButton, BuyButtonX, ProductionButtonY, buyProps.Box.Width, buyProps.Box.Height, scale);
        LayoutButton(_changeButton, ChangeButtonX, ProductionButtonY, changeProps.Box.Width, changeProps.Box.Height, scale);

        var fontSize = Math.Max(10, (int)Math.Round(_active.Look.CityWindowFontSize * scale * 0.78f));
        _buyButton.FontSize = fontSize;
        _changeButton.FontSize = fontSize;
    }

    private static void LayoutButton(CityButton button, float x, float y, float width, float height, float scale)
    {
        button.Location = new(x * scale, y * scale);
        button.Width = Math.Max(1, (int)Math.Round(width * scale));
        button.Height = Math.Max(1, (int)Math.Round(height * scale));
    }

    private IProductionOrder GetDisplayedOrder()
    {
        return _city.ConstructionQueue.Current?.Order ?? _city.ItemInProduction;
    }

    private int GetDisplayedProgress()
    {
        // GameTurn still applies production to City.ShieldsProgress.  The newer
        // queue object describes what is being displayed, but it is not yet the
        // authoritative per-turn shield accumulator.  Draw the same accumulated
        // shields the production-completion code uses so the city box updates
        // immediately for normal production and disband contributions.
        return _city.ShieldsProgress;
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
        var previousOrder = _city.ItemInProduction;
        if (previousOrder != selectedOrder)
        {
            var retainedShields = GetRetainedShieldsWhenChangingProduction(previousOrder, selectedOrder);

            _city.ItemInProduction = selectedOrder;
            _city.ShieldsProgress = retainedShields;
            _city.ConstructionQueue.Clear();
            _city.ConstructionQueue.Enqueue(selectedOrder, _shieldBoxRows);
            SynchronizeQueueProgress(selectedOrder, retainedShields);

            ChangeProductionDisplay();
        }
    }

    private int GetRetainedShieldsWhenChangingProduction(IProductionOrder previousOrder, IProductionOrder selectedOrder)
    {
        var currentProgress = Math.Max(0, _city.ShieldsProgress);
        if (previousOrder.Type == selectedOrder.Type)
        {
            return currentProgress;
        }

        return currentProgress / 2;
    }

    private void SynchronizeQueueProgress(IProductionOrder selectedOrder, int retainedShields)
    {
        var current = _city.ConstructionQueue.Current;
        if (current == null)
        {
            return;
        }

        var totalCost = selectedOrder.Cost * Math.Max(1, _shieldBoxRows);
        current.RemainingCost = Math.Max(0, totalCost - retainedShields);
        current.Status = retainedShields > 0 ? ItemStatus.InProgress : ItemStatus.Queued;
    }

    public override void Draw(bool pulse)
    {
        var activeOrder = GetDisplayedOrder();
        var progressShields = Math.Min(GetDisplayedProgress(), _totalCost);
        DrawShieldProgress(activeOrder, progressShields);
        DrawProductionIcon(activeOrder);
        base.Draw(pulse);
    }

    private void DrawProductionIcon(IProductionOrder activeOrder)
    {
        var source = activeOrder.GetIcon(_active);
        if (source == null)
        {
            return;
        }

        var texture = TextureCache.GetImage(source);
        if (texture.Width <= 0 || texture.Height <= 0)
        {
            return;
        }

        var scale = _cityWindow.Scale;
        var slotWidth = ProductionIconSlotWidth * scale;
        var slotHeight = ProductionIconSlotHeight * scale;
        var drawScale = Math.Min(slotWidth / texture.Width, slotHeight / texture.Height);
        drawScale = Math.Max(0.01f, Math.Min(0.92f * scale, drawScale));

        var drawWidth = texture.Width * drawScale;
        var drawHeight = texture.Height * drawScale;
        var center = new Vector2(
            Bounds.X + _properties.IconLocation.X * scale,
            Bounds.Y + _properties.IconLocation.Y * scale);

        Graphics.DrawTexturePro(texture,
            new Rectangle(0, 0, texture.Width, texture.Height),
            new Rectangle(center.X - drawWidth / 2f, center.Y - drawHeight / 2f, drawWidth, drawHeight),
            Vector2.Zero, 0f, Color.White);
    }

    private void DrawShieldProgress(IProductionOrder activeOrder, int progressShields)
    {
        var scale = _cityWindow.Scale;
        var shieldTop = ShieldBoxTop * scale;
        var shieldBottom = Height - Padding * scale;
        var lines = (int)((shieldBottom - shieldTop) / _shieldHeight);
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

        var posX = Bounds.X + Padding * scale;
        var posY = Bounds.Y + shieldTop;
        var drawWidth = Width - 2 * Padding * scale;
        var spacing = (int)_shieldWidth;
        var requiredWidth = shieldsPerRow * spacing;

        Graphics.DrawLineEx(new Vector2(posX, posY), new Vector2(posX + drawWidth, posY), 1f, _pen1);
        var lineHeight = 6 * scale + lines * _shieldHeight;
        posY = Bounds.Y + shieldTop + lineHeight;
        Graphics.DrawLineEx(new Vector2(posX, posY), new Vector2(posX + drawWidth, posY), 1f, _pen2);

        posY = Bounds.Y + shieldTop;
        Graphics.DrawLineEx(new Vector2(posX, posY), new Vector2(posX, posY + lineHeight), 1f, _pen1);
        Graphics.DrawLineEx(new Vector2(posX + drawWidth, posY), new Vector2(posX + drawWidth, posY + lineHeight), 1f, _pen2);

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
                    new Vector2((int)posX + spacing * col, (int)(Bounds.Y + shieldTop + 3 * scale + _shieldHeight * row)),
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
