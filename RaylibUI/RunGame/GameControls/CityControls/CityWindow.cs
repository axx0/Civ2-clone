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
using Civ2engine.IO;
using Model.Core.Cities;

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
    private Vector2? _startLocation;    // used once, on the first layout (switching cities keeps the window in place)

    /// <param name="scale">Zoom level to open at (0.5, 1 or 1.5).</param>
    /// <param name="location">Where to open the window; null = centred.</param>
    public CityWindow(GameScreen gameScreen, City city, float scale = 1.0f, Vector2? location = null) : base(gameScreen.Main)
    {
        CurrentGameScreen = gameScreen;
        City = city;
        _scale = Math.Clamp(scale, _scaleMin, _scaleMax);
        _startLocation = location;
        _active = gameScreen.MainWindow.ActiveInterface;
        var game = CurrentGameScreen.Game;

        _cityWindowProps = _active.GetCityWindowDefinition();

        _headerLabel = new HeaderLabel(this, _active.Look, $"{Labels.For(LabelIndex.City)} {Labels.For(LabelIndex.of)} {City.Name}, " +
            $"{game.Date.GameYearString(game.TurnNumber)}, {Labels.For(LabelIndex.Population)} {city.GetPopulation()} " +
            $"({Labels.For(LabelIndex.Treasury)} {city.Owner.Money} {Labels.For(LabelIndex.Gold)})",
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
        viewButton.Click += (_, _) => gameScreen.ShowDialog(new CityView(gameScreen, city)); ;
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
        var supportLabel = new CityLabel(this, _cityWindowProps.Labels["UnitsSupported"]);
        Controls.Add(supportLabel);
        Controls.Add(new UnitSupportBox(this, supportLabel));
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

        Focused = exitButton;
    }

    private void CloseButtonOnClick(object? sender, MouseEventArgs e)
    {
        CurrentGameScreen.CloseDialog(this);
    }

    public override int Width => (int)(_cityWindowProps.Width * _scale) + PaddingSide;
    public override int Height => (int)(_cityWindowProps.Height * _scale) + LayoutPadding.Top + LayoutPadding.Bottom;
    public float Scale => _scale;
    public City City { get; }

    /// <summary>
    /// A box of the city picture at the current scale, relative to the picture.
    /// The edges are rounded, not the size, so boxes that touch at scale 1 still touch when zoomed.
    /// </summary>
    public Rectangle ScaledBox(Rectangle box)
    {
        static int R(float v) => (int)Math.Round(v, MidpointRounding.AwayFromZero);
        var x0 = R(box.X * _scale);
        var y0 = R(box.Y * _scale);
        var x1 = R((box.X + box.Width) * _scale);
        var y1 = R((box.Y + box.Height) * _scale);
        return new Rectangle(x0, y0, x1 - x0, y1 - y0);
    }

    /// <summary>
    /// Font size of the city window's texts at the current scale.
    /// </summary>
    public int ScaledFontSize => _active.Look.CityWindowFontSize + (int)(12 * (_scale - 1));

    /// <summary>
    /// Put a control on a box of the city picture. A direct child of the window is offset by the
    /// window's padding; a control inside another one (e.g. the info area) uses the box as it is,
    /// relative to its parent.
    /// </summary>
    public void Place(BaseControl control, Rectangle box)
    {
        var scaled = ScaledBox(box);
        var offset = control.Parent == this ? new Vector2(LayoutPadding.Left, LayoutPadding.Top) : Vector2.Zero;
        control.Location = new Vector2(scaled.X, scaled.Y) + offset;
        control.Width = (int)scaled.Width;
        control.Height = (int)scaled.Height;
    }

    public override void Resize(int width, int height)
    {
        _headerLabel.FontSize = Math.Max(_active.Look.CityHeaderLabelFontSizeSmall, (int)(_active.Look.CityHeaderLabelFontSizeNormal * _scale));

        LayoutPadding = _active.GetPadding(_headerLabel?.TextSize.Y ?? 0, false);

        BackgroundImage = ImageUtils.PaintDialogBase(_active, Width, Height, LayoutPadding,
            Images.ExtractBitmap(_cityWindowProps.Image, _active));

        SetLocation(width, Width, height, Height);
        if (_startLocation is { } start)
        {
            Location = new(Math.Clamp(start.X, 0, Math.Max(0, width - Width)),
                Math.Clamp(start.Y, 0, Math.Max(0, height - Height)));
            _startLocation = null;
        }
        LayoutTitleBar();

        foreach (var control in Controls)
        {
            control.OnResize();
        }
    }

    private void LayoutTitleBar()
    {
        const int left = 11, top = 5, gap = 2;

        Button[] icons = [_exitIcon, _shrinkIcon, _expandIcon];
        for (var i = 0; i < icons.Length; i++)
        {
            icons[i].Location = new(left + i * (_iconW + gap) * _scale, top);
            icons[i].Scale = _scale;
        }

        var headerOffset = (int)(left + icons.Length * (_iconW + gap) * _scale);
        _headerLabel.Location = new(headerOffset, 0);
        _headerLabel.Width = Width - 2 * headerOffset;
        _headerLabel.Height = LayoutPadding.Top;
    }

    public override void OnKeyPress(KeyboardKey key)
    {
        switch (key)
        {
            case KeyboardKey.Escape:
                CurrentGameScreen.CloseDialog(this);
                return;
            case KeyboardKey.Left:
                SwitchCity(-1);
                return;
            case KeyboardKey.Right:
                SwitchCity(1);
                return;
        }
        base.OnKeyPress(key);
    }

    private void SwitchCity(int direction)
    {
        var cities = City.Owner.Cities;
        var index = cities.IndexOf(City);
        if (cities.Count < 2 || index < 0) return;

        var next = cities[((index + direction) % cities.Count + cities.Count) % cities.Count];
        CurrentGameScreen.CloseDialog(this);
        CurrentGameScreen.ShowDialog(new CityWindow(CurrentGameScreen, next, _scale, Location), stack: true);
    }

    public void UpdateProduction()
    {
        City.CalculateOutput(City.Owner.Government, CurrentGameScreen.Game);
        ResourceProductionChanged?.Invoke(this, EventArgs.Empty);
    }

    public event EventHandler? ResourceProductionChanged;
}