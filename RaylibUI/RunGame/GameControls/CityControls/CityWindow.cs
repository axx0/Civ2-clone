using System.Numerics;
using System.Runtime.CompilerServices;
using Civ2engine;
using Civ2engine.Production;
using Model;
using Model.CityWindowModel;
using Model.Interface;
using Raylib_CSharp.Colors;
using Raylib_CSharp.Fonts;
using Raylib_CSharp.Interact;
using Raylib_CSharp.Transformations;
using Raylib_CSharp.Windowing;
using RaylibUI.BasicTypes.Controls;
using RaylibUI.Controls;
using RaylibUtils;

namespace RaylibUI.RunGame.GameControls.CityControls;

public class CityWindow : BaseDialog
{
    public GameScreen CurrentGameScreen { get; }
    public CityWindowLayout CityWindowProps => _cityWindowProps;
    private readonly CityWindowLayout _cityWindowProps;
    private readonly HeaderLabel _headerLabel;
    private readonly CityButton _shrinkIcon, _expandIcon, _exitIcon, _infoButton, _mapButton, _renameButton, _happyButton, _viewButton, _exitButton;
    private readonly IUserInterface _active;
    private float _scale = 1.0f;  // scale city window size (0.5=small, 1=normal, 1.5=large)
    private const float _scaleMax = 1.5f;
    private const float _scaleMin = 0.5f;
    private const float _scaleDelta = 0.5f;

    public CityWindow(GameScreen gameScreen, City city) : base(gameScreen.Main)
    {
        CurrentGameScreen = gameScreen;
        City = city;
        _active = gameScreen.MainWindow.ActiveInterface;

        _cityWindowProps = _active.GetCityWindowDefinition();

        _headerLabel = new HeaderLabel(this, _active.Look, City.Name,
            fontSize: _active.Look.CityHeaderLabelFontSizeNormal);

        LayoutPadding = _active.GetPadding(_headerLabel?.TextSize.Y ?? 0, false);

        Controls.Add(_headerLabel);

        //Tile map rendered first because in TOT it renders behind
        var tileMap = new CityTileMap(this, gameScreen.Game);
        Controls.Add(tileMap);

        var infoArea = new CityInfoArea(this);
        Controls.Add(infoArea);

        _infoButton = new CityButton(this, Labels.For(LabelIndex.Info), _active.Look.CityWindowFont, _active.Look.CityWindowFontSize)
        {
            AbsolutePosition = _cityWindowProps.Buttons["Info"]
        };
        _infoButton.Click += (_, _) => infoArea.SetActiveMode(CityDisplayMode.Info);
        Controls.Add(_infoButton);

        // Map button
        _mapButton = new CityButton(this, Labels.For(LabelIndex.Map), _active.Look.CityWindowFont, _active.Look.CityWindowFontSize)
        {
            AbsolutePosition = _cityWindowProps.Buttons["Map"]
        };
        _mapButton.Click += (_, _) => infoArea.SetActiveMode(CityDisplayMode.SupportMap);
        Controls.Add(_mapButton);

        // Rename button
        _renameButton = new CityButton(this, Labels.For(LabelIndex.Rename), _active.Look.CityWindowFont, _active.Look.CityWindowFontSize)
        {
            AbsolutePosition = _cityWindowProps.Buttons["Rename"]
        };
        Controls.Add(_renameButton);

        // Happy button
        _happyButton = new CityButton(this, Labels.For(LabelIndex.Happy), _active.Look.CityWindowFont, _active.Look.CityWindowFontSize)
        {
            AbsolutePosition = _cityWindowProps.Buttons["Happy"]
        };
        _happyButton.Click += (_, _) => infoArea.SetActiveMode(CityDisplayMode.Happiness);
        Controls.Add(_happyButton);

        // View button
        _viewButton = new CityButton(this, Labels.For(LabelIndex.View), _active.Look.CityWindowFont, _active.Look.CityWindowFontSize)
        {
            AbsolutePosition = _cityWindowProps.Buttons["View"]
        };
        Controls.Add(_viewButton);

        // Exit button
        _exitButton = new CityButton(this, Labels.For(LabelIndex.Close), _active.Look.CityWindowFont, _active.Look.CityWindowFontSize)
        {
            AbsolutePosition = _cityWindowProps.Buttons["Exit"]
        };
        _exitButton.Click += CloseButtonOnClick;
        Controls.Add(_exitButton);

        var resourceTitle = Labels.For(LabelIndex.CityResources);
        Controls.Add(new CityLabel(this, Labels.For(LabelIndex.CityResources), _active.Look.CityWindowFont, _active.Look.CityWindowFontSize,
            new Color(223, 187, 63, 255), new Color(67, 67, 67, 255))
        {
            AbsolutePosition = _cityWindowProps.Resources.TitlePosition
        });
        foreach (var resource in _cityWindowProps.Resources.Resources)
        {
            Controls.Add(new ResourceProductionBar(this, resource));
        }

        Controls.Add(new FoodStorageBox(this));
        Controls.Add(new ProductionBox(this));
        Controls.Add(new UnitSupportBox(this));

        var imageW = Images.GetImageWidth(_active.PicSources["zoomIn"][0], _active);
        var imageH = Images.GetImageHeight(_active.PicSources["zoomIn"][0], _active);
        _exitIcon = new CityButton(this, String.Empty, backgroundImage: _active.PicSources["close"][0])
        {
            AbsolutePositionNoPadding = new Rectangle(11, 7, imageW, imageH)
        };
        _shrinkIcon = new CityButton(this, String.Empty, backgroundImage: _active.PicSources["zoomIn"][0])
        {
            AbsolutePositionNoPadding = new Rectangle(11 + imageW + 2, 7, imageW, imageH)
        };
        _expandIcon = new CityButton(this, String.Empty, backgroundImage: _active.PicSources["zoomOut"][0])
        {
            AbsolutePositionNoPadding = new Rectangle(11 + 2 * imageW + 2 * 2, 7, imageW, imageH)
        };
        _exitIcon.Click += CloseButtonOnClick;
        _shrinkIcon.Click += (_, _) =>
        {
            _scale = Math.Max(_scale - _scaleDelta, _scaleMin);
            Resize(Window.GetScreenWidth(), Window.GetScreenHeight());
        };
        _expandIcon.Click += (_, _) =>
        {
            _scale = Math.Min(_scale + _scaleDelta, _scaleMax);
            Resize(Window.GetScreenWidth(), Window.GetScreenHeight());
        };
        Controls.Add(_shrinkIcon);
        Controls.Add(_expandIcon);
        Controls.Add(_exitIcon);
    }

    private void CloseButtonOnClick(object? sender, MouseEventArgs e)
    {
        CurrentGameScreen.CloseDialog(this);
    }

    public float Scale => _scale;

    public int DialogWidth
    {
        get => (int)(_cityWindowProps.Width * _scale) + PaddingSide;
        init { }
    }

    public int DialogHeight
    {
        get => (int)(_cityWindowProps.Height * _scale) + LayoutPadding.Top + LayoutPadding.Bottom;
        init { }
    }

    public City City { get; }

    public override void Resize(int width, int height)
    {
        _headerLabel.FontSize = Math.Max(_active.Look.CityHeaderLabelFontSizeSmall, (int)(_active.Look.CityHeaderLabelFontSizeNormal * _scale));

        LayoutPadding = _active.GetPadding(_headerLabel?.TextSize.Y ?? 0, false);

        BackgroundImage = ImageUtils.PaintDialogBase(_active, DialogWidth, DialogHeight, LayoutPadding,
            Images.ExtractBitmap(_cityWindowProps.Image, _active));
        
        SetLocation(width, DialogWidth, height, DialogHeight);
        _headerLabel.Bounds = new Rectangle(Location.X + 70, Location.Y, DialogWidth - 2 * 70, LayoutPadding.Top);
        
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