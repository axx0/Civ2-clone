using System.Numerics;
using Civ2engine;
using Civ2engine.Production;
using Model;
using Model.CityWindowModel;
using Model.Interface;
using Raylib_CSharp.Colors;
using Raylib_CSharp.Fonts;
using Raylib_CSharp.Interact;
using Raylib_CSharp.Transformations;
using RaylibUI.BasicTypes.Controls;
using RaylibUI.Controls;
using RaylibUtils;

namespace RaylibUI.RunGame.GameControls.CityControls;

public class CityWindow : BaseDialog
{
    public GameScreen CurrentGameScreen { get; }
    private readonly CityWindowLayout _cityWindowProps;
    private readonly HeaderLabel _headerLabel;
    private readonly IUserInterface _active;
    private readonly IList<IProductionOrder> _canProduce;
    
    private LabelControl? _productionLabel;
    private IconContainer? _productionIcon;
    private ShieldBox? _productionBox;

    public CityWindow(GameScreen gameScreen, City city) : base(gameScreen.Main)
    {
        CurrentGameScreen = gameScreen;
        City = city;
        _canProduce = ProductionPossibilities.GetAllowedProductionOrders(city);
        _active = gameScreen.MainWindow.ActiveInterface;

        _cityWindowProps = _active.GetCityWindowDefinition();

        _headerLabel = new HeaderLabel(this, _active.Look, City.Name,
            fontSize: _active.Look.CityHeaderLabelFontSizeNormal);

        LayoutPadding = _active.GetPadding(_headerLabel?.TextSize.Y ?? 0, false);

        DialogWidth = _cityWindowProps.Width + PaddingSide;
        DialogHeight = _cityWindowProps.Height + LayoutPadding.Top + LayoutPadding.Bottom;
        BackgroundImage = ImageUtils.PaintDialogBase(_active, DialogWidth, DialogHeight, LayoutPadding,
            Images.ExtractBitmap(_cityWindowProps.Image, _active));

        Controls.Add(_headerLabel);

        //Tile map rendered first because in TOT it renders behind
        var tileMap = new CityTileMap(this, gameScreen.Game)
        {
            AbsolutePosition = _cityWindowProps.TileMap
        };
        Controls.Add(tileMap);

        var infoArea = new CityInfoArea(this, _cityWindowProps.InfoPanel);

        Controls.Add(infoArea);

        var buyButton = new Button(this, Labels.For(LabelIndex.Buy), _active.Look.CityWindowFont,
            _active.Look.CityWindowFontSize)
        {
            AbsolutePosition = _cityWindowProps.Buttons["Buy"]
        };
        Controls.Add(buyButton);

        var changeButton = new Button(this, Labels.For(LabelIndex.Change), _active.Look.CityWindowFont,
            _active.Look.CityWindowFontSize)
        {
            AbsolutePosition = _cityWindowProps.Buttons["Change"]
        };
        changeButton.Click += (_, _) =>
        {
            gameScreen.ShowPopup(
                "PRODUCTION", handleButtonClick: BuildDialogClosed, replaceStrings: new[] { city.Name }, listBox: new ListBoxDefinition
                {
                    Vertical = true,
                    Entries = _canProduce.Select(p => p.GetBuildListEntry(_active)).ToList(),
                    InitialSelection = _canProduce.IndexOf(city.ItemInProduction)
                });
        };
        Controls.Add(changeButton);

        var productionSettings = _cityWindowProps.Production;
        ChangeProductionDisplay(city, productionSettings);
      
        
        var infoButton = new Button(this, Labels.For(LabelIndex.Info), _active.Look.CityWindowFont, _active.Look.CityWindowFontSize)
        {
            AbsolutePosition = _cityWindowProps.Buttons["Info"]
        };
        infoButton.Click += (_, _) => infoArea.SetActiveMode(CityDisplayMode.Info);
        Controls.Add(infoButton);

        // Map button
        var mapButton = new Button(this, Labels.For(LabelIndex.Map), _active.Look.CityWindowFont, _active.Look.CityWindowFontSize)
        {
            AbsolutePosition = _cityWindowProps.Buttons["Map"]
        };
        mapButton.Click += (_, _) => infoArea.SetActiveMode(CityDisplayMode.SupportMap);
        Controls.Add(mapButton);


        // Rename button
        var renameButton = new Button(this, Labels.For(LabelIndex.Rename), _active.Look.CityWindowFont, _active.Look.CityWindowFontSize)
        {
            AbsolutePosition = _cityWindowProps.Buttons["Rename"]
        };
        Controls.Add(renameButton);

        // Happy button
        var happyButton = new Button(this, Labels.For(LabelIndex.Happy), _active.Look.CityWindowFont, _active.Look.CityWindowFontSize)
        {
            AbsolutePosition = _cityWindowProps.Buttons["Happy"]
        };
        happyButton.Click += (_, _) => infoArea.SetActiveMode(CityDisplayMode.Happiness);
        Controls.Add(happyButton);

        // View button
        var viewButton = new Button(this, Labels.For(LabelIndex.View), _active.Look.CityWindowFont, _active.Look.CityWindowFontSize)
        {
            AbsolutePosition = _cityWindowProps.Buttons["View"]
        };
        Controls.Add(viewButton);

        // Exit button
        var exitButton = new Button(this, Labels.For(LabelIndex.Close), _active.Look.CityWindowFont, _active.Look.CityWindowFontSize)
        {
            AbsolutePosition = _cityWindowProps.Buttons["Exit"]
        };
        exitButton.Click += CloseButtonOnClick;
        Controls.Add(exitButton);

        var titlePosition = _cityWindowProps.Resources.TitlePosition;
        if (titlePosition != Vector2.Zero)
        {
            var resourceTitle = Labels.For(LabelIndex.CityResources);
            var resourceTitleSize = TextManager.MeasureTextEx(_active.Look.CityWindowFont, resourceTitle, _active.Look.CityWindowFontSize, 1);
            Controls.Add(new LabelControl(this, resourceTitle, eventTransparent: true, alignment: TextAlignment.Center, font: _active.Look.CityWindowFont, fontSize: _active.Look.CityWindowFontSize, colorFront: Color.Gold)
            {
                AbsolutePosition = new Rectangle(titlePosition.X - resourceTitleSize.X / 2 - 10,titlePosition.Y, resourceTitleSize.X + 20, resourceTitleSize.Y )
            });
        }
        foreach (var resource in _cityWindowProps.Resources.Resources)
        {
            Controls.Add(new ResourceProductionBar(this, resource));
        }

        var foodBox = new FoodStorageBox(this)
        {
            AbsolutePosition = _cityWindowProps.FoodStorage
        };
        Controls.Add(foodBox);

        var supportBox = new UnitSupportBox(this, _cityWindowProps.UnitSupport);
        Controls.Add(supportBox);

    }

    private void ChangeProductionDisplay(City city, ShieldProduction productionSettings)
    {
        var productionTitlePosition = productionSettings.TitlePosition;
        
        var productionTitle = city.ItemInProduction.Title;
        var productionTitleSize = TextManager.MeasureTextEx(_active.Look.CityWindowFont, productionTitle, _active.Look.CityWindowFontSize, 1);
        
        var label = new LabelControl(this, productionTitle, eventTransparent: true, alignment: TextAlignment.Center, font: _active.Look.CityWindowFont, fontSize: _active.Look.CityWindowFontSize, colorFront: Color.Blue)
        {
            AbsolutePosition = new Rectangle(productionTitlePosition.X - productionTitleSize.X / 2 - 10, productionTitlePosition.Y, productionTitleSize.X + 20, productionTitleSize.Y )
        };
        if (_productionLabel != null)
        {
            Controls.Remove(_productionLabel);
            label.OnResize();
        }
        _productionLabel = label;
        Controls.Add(_productionLabel);
        
        var productionIcon = new IconContainer(this, city.ItemInProduction.GetIcon(_active), 0,
            (int)productionSettings.IconLocation.Width){
            AbsolutePosition =productionSettings.IconLocation
        };
        if (_productionIcon != null)
        {
            Controls.Remove(_productionIcon);
            productionIcon.OnResize();
        }

        _productionIcon = productionIcon;
        Controls.Add(productionIcon);
        if (productionSettings.Type == "Box")
        {
            if (_productionBox == null)
            {
                _productionBox = new ShieldBox(this)
                {
                    AbsolutePosition = productionSettings.ShieldBox
                };
                Controls.Add(_productionBox);
            }

            _productionBox.UpdateData(city.ItemInProduction);
        }
    }

    private void BuildDialogClosed(string button, int selectedIndex, IList<bool>? chx, IDictionary<string, string>? txt)
    {
        if (button == Labels.Ok && selectedIndex != -1 && City.ItemInProduction != _canProduce[selectedIndex])
        {
            City.ItemInProduction = _canProduce[selectedIndex];
            
            ChangeProductionDisplay(City, _cityWindowProps.Production);
        }
    }

    private void CloseButtonOnClick(object? sender, MouseEventArgs e)
    {
        CurrentGameScreen.CloseDialog(this);
    }

    public int DialogWidth { get; }

    public int DialogHeight { get; }
    public City City { get; }

    public override void Resize(int width, int height)
    {
        SetLocation(width, DialogWidth, height, DialogHeight);
        _headerLabel.Bounds = new Rectangle(Location.X, Location.Y, DialogWidth, LayoutPadding.Top);
        foreach (var control in Controls)
        {
            control.OnResize();
        }
    }

    public override void OnKeyPress(KeyboardKey key)
    {
        if (key == KeyboardKey.Escape)
        {
            CurrentGameScreen.CloseDialog(this);
        }
        base.OnKeyPress(key);
    }

    public void UpdateProduction()
    {
        City.CalculateOutput(City.Owner.Government, CurrentGameScreen.Game);
        ResourceProductionChanged(this, EventArgs.Empty);
    }

    public event EventHandler ResourceProductionChanged;
}