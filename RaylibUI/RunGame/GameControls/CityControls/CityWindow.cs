using Civ2engine;
using Civ2engine.Production;
using Model;
using Model.Controls;
using Model.Interface;
using Raylib_CSharp.Colors;
using Raylib_CSharp.Fonts;
using Raylib_CSharp.Interact;
using Raylib_CSharp.Transformations;
using Raylib_CSharp.Windowing;
using RaylibUI.BasicTypes.Controls;
using RaylibUI.Controls;
using RaylibUtils;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Threading;

namespace RaylibUI.RunGame.GameControls.CityControls;

public class CityWindow : BaseDialog
{
    public GameScreen CurrentGameScreen { get; }
    public CityWindowLayout CityWindowProps => _cityWindowProps;
    private readonly CityWindowLayout _cityWindowProps;
    private readonly HeaderLabel _headerLabel;
    private readonly Button _shrinkIcon, _expandIcon, _exitIcon;
    private readonly IUserInterface _active;
    private readonly int _iconW, _iconH;
    private float _scale = 1.0f;  // scale city window size (0.5=small, 1=normal, 1.5=large)
    private const float _scaleMax = 1.5f;
    private const float _scaleMin = 0.5f;
    private const float _scaleDelta = 0.5f;
    private readonly UnitSupportBox _unitSupportBox;
    private readonly CityLabel _supportLabel;

    public CityWindow(GameScreen gameScreen, City city) : base(gameScreen.Main)
    {
        CurrentGameScreen = gameScreen;
        City = city;
        _active = gameScreen.MainWindow.ActiveInterface;
        var game = CurrentGameScreen.Game;

        _cityWindowProps = _active.GetCityWindowDefinition();

        _headerLabel = new HeaderLabel(this, _active.Look, $"City of {City.Name}, {game.Date.GameYearString(game.TurnNumber)}, " +
            $"Population {city.GetPopulation()} (Treasury {city.Owner.Money} Gold)",
            fontSize: _active.Look.CityHeaderLabelFontSizeNormal);

        Controls.Add(_headerLabel);

        //Tile map rendered first because in TOT it renders behind
        var tileMap = new CityTileMap(this, gameScreen.Game);
        Controls.Add(tileMap);

        var infoArea = new CityInfoArea(this);
        Controls.Add(infoArea);

        var infoButton = new CityButton(this, "Info");
        infoButton.Click += (_, _) => infoArea.SetActiveMode(CityDisplayMode.Info);
        Controls.Add(infoButton);

        var mapButton = new CityButton(this, "Map");
        mapButton.Click += (_, _) => infoArea.SetActiveMode(CityDisplayMode.SupportMap);
        Controls.Add(mapButton);

        var renameButton = new CityButton(this, "Rename");
        renameButton.Click += (_, _) => { };
        Controls.Add(renameButton);

        var happyButton = new CityButton(this, "Happy");
        happyButton.Click += (_, _) => infoArea.SetActiveMode(CityDisplayMode.Happiness);
        Controls.Add(happyButton);

        var viewButton = new CityButton(this, "View");
        viewButton.Click += (_, _) => { };
        Controls.Add(viewButton);

        var exitButton = new CityButton(this, "Exit");
        exitButton.Click += CloseButtonOnClick;
        Controls.Add(exitButton);

        var resourceTitle = Labels.For(LabelIndex.CityResources);
        //bounds = _cityWindowProps.Resources.TitlePosition;
        //Controls.Add(new CityLabel(this, Labels.For(LabelIndex.CityResources), new Color(223, 187, 63, 255), new Color(67, 67, 67, 255))
        //{
        //    Location = new(bounds.X, bounds.Y),
        //    Width = (int)bounds.Width,
        //    Height = (int)bounds.Height,
        //});
        foreach (var resource in _cityWindowProps.Resources.Resources)
        {
            Controls.Add(new ResourceProductionBar(this, resource));
        }

        Controls.Add(new CityLabel(this, _cityWindowProps.Labels["FoodStorage"]));
        Controls.Add(new FoodStorageBox(this));
        Controls.Add(new ProductionBox(this));
        _supportLabel = new CityLabel(this, _cityWindowProps.Labels["UnitsSupported"]);
        Controls.Add(_supportLabel);
        _unitSupportBox = new UnitSupportBox(this);
        Controls.Add(_unitSupportBox);
        Controls.Add(new CityLabel(this, _cityWindowProps.Labels["CityImprovements"]));
        Controls.Add(new ImprovementsBox(this));
        Controls.Add(new CityLabel(this, _cityWindowProps.Labels["Citizens"]));
        Controls.Add(new CityCitizensBox(this));

        _iconW = Images.GetImageWidth(_active.PicSources["zoomIn"][0], _active);
        _iconH = Images.GetImageHeight(_active.PicSources["zoomIn"][0], _active);
        _exitIcon = new Button(this, String.Empty, backgroundImage: _active.PicSources["close"][0]);
        _shrinkIcon = new Button(this, String.Empty, backgroundImage: _active.PicSources["zoomIn"][0]);
        _expandIcon = new Button(this, String.Empty, backgroundImage: _active.PicSources["zoomOut"][0]);
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

    public override int Width => (int)(_cityWindowProps.Width * _scale) + PaddingSide;
    public override int Height => (int)(_cityWindowProps.Height * _scale) + LayoutPadding.Top + LayoutPadding.Bottom;
    public float Scale => _scale;
    public City City { get; }

    public override void Resize(int width, int height)
    {
        _headerLabel.FontSize = Math.Max(_active.Look.CityHeaderLabelFontSizeSmall, (int)(_active.Look.CityHeaderLabelFontSizeNormal * _scale));

        LayoutPadding = _active.GetPadding(_headerLabel?.TextSize.Y ?? 0, false);

        BackgroundImage = ImageUtils.PaintDialogBase(_active, Width, Height, LayoutPadding,
            Images.ExtractBitmap(_cityWindowProps.Image, _active));

        _exitIcon.Location = new(11, 5);
        _shrinkIcon.Location = new(11 + (_iconW + 2) * _scale, 5);
        _expandIcon.Location = new(11 + (2 * _iconW + 2 * 2) * _scale, 5);
        _exitIcon.Scale = _scale;
        _shrinkIcon.Scale = _scale;
        _expandIcon.Scale = _scale;
        
        SetLocation(width, Width, height, Height);
        var headerOffset = 11 + (3 * _iconW + 3 * 2) * _scale;
        _headerLabel.Location = new(headerOffset, 0);
        _headerLabel.Width = Width - 2 * (int)headerOffset;
        _headerLabel.Height = LayoutPadding.Top;


        foreach (var control in Controls)
        {
            control.OnResize();
        }

        _supportLabel.Visible = _unitSupportBox.Definition.Groups.Count <= _unitSupportBox.Definition.Columns;

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