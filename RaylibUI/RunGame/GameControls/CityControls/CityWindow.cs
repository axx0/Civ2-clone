using System.Numerics;
using Civ2engine;
using Model;
using Raylib_cs;
using RaylibUI.BasicTypes.Controls;
using RaylibUI.Controls;

namespace RaylibUI.RunGame.GameControls.CityControls;

public class CityWindow : BaseDialog
{
    public GameScreen CurrentGameScreen { get; }
    private readonly CityWindowLayout _cityWindowProps;
    private readonly HeaderLabel _headerLabel;

    public CityWindow(GameScreen gameScreen, City city) : base(gameScreen.Main)
    {
        CurrentGameScreen = gameScreen;
        City = city;

        _cityWindowProps = gameScreen.Main.ActiveInterface.GetCityWindowDefinition();

        _headerLabel = new HeaderLabel(this, City.Name);

        LayoutPadding = new Padding(LayoutPadding, _headerLabel.GetPreferredHeight());

        DialogWidth = _cityWindowProps.Width + PaddingSide;
        DialogHeight = _cityWindowProps.Height + LayoutPadding.Top + LayoutPadding.Bottom;
        BackgroundImage = ImageUtils.PaintDialogBase(DialogWidth, DialogHeight, LayoutPadding.Top, LayoutPadding.Bottom,
            LayoutPadding.Left, Images.ExtractBitmap(_cityWindowProps.Image));

        Controls.Add(_headerLabel);

        var infoArea = new CityInfoArea(this)
        {
            AbsolutePosition = _cityWindowProps.InfoPanel
        };
        Controls.Add(infoArea);

        var buyButton = new Button(this, Labels.For(LabelIndex.Buy))
        {
            AbsolutePosition = _cityWindowProps.Buttons["Buy"]
        };
        Controls.Add(buyButton);

        var changeButton = new Button(this, Labels.For(LabelIndex.Change))
        {
            AbsolutePosition = _cityWindowProps.Buttons["Change"]
        };
        Controls.Add(changeButton);
        var infoButton = new Button(this, Labels.For(LabelIndex.Info))
        {
            AbsolutePosition = _cityWindowProps.Buttons["Info"]
        };
        infoButton.Click += (_, _) => infoArea.Mode = DisplayMode.Info;
        Controls.Add(infoButton);

        // Map button
        var mapButton = new Button(this, Labels.For(LabelIndex.Map))
        {
            AbsolutePosition = _cityWindowProps.Buttons["Map"]
        };
        mapButton.Click += (_, _) => infoArea.Mode = DisplayMode.SupportMap;
        Controls.Add(mapButton);


        // Rename button
        var renameButton = new Button(this, Labels.For(LabelIndex.Rename))
        {
            AbsolutePosition = _cityWindowProps.Buttons["Rename"]
        };
        Controls.Add(renameButton);

        // Happy button
        var happyButton = new Button(this, Labels.For(LabelIndex.Happy))
        {
            AbsolutePosition = _cityWindowProps.Buttons["Happy"]
        };
        happyButton.Click += (_, _) => infoArea.Mode = DisplayMode.Happiness;
        Controls.Add(happyButton);

        // View button
        var viewButton = new Button(this, Labels.For(LabelIndex.View))
        {
            AbsolutePosition = _cityWindowProps.Buttons["View"]
        };
        Controls.Add(viewButton);

        // Exit button
        var exitButton = new Button(this, Labels.For(LabelIndex.Close))
        {
            AbsolutePosition = _cityWindowProps.Buttons["Exit"]
        };
        exitButton.Click += CloseButtonOnClick;
        Controls.Add(exitButton);

        var tileMap = new CityTileMap(this)
        {
            AbsolutePosition = _cityWindowProps.TileMap
        };
        Controls.Add(tileMap);

        var titlePosition = _cityWindowProps.Resources.TitlePosition;
        if (titlePosition != Vector2.Zero)
        {
            var resourceTitle = Labels.For(LabelIndex.CityResources);
            var resourceTitleSize = Raylib.MeasureTextEx(Fonts.AlternativeFont, resourceTitle, 16, 1);
            Controls.Add(new LabelControl(this, resourceTitle, eventTransparent:true, alignment: TextAlignment.Center, colorFront: Color.GOLD, font: Fonts.AlternativeFont, fontSize: 16)
            {
                AbsolutePosition = new Rectangle(titlePosition.X - resourceTitleSize.X / 2 - 10,titlePosition.Y, resourceTitleSize.X + 20, resourceTitleSize.Y )
            });
        }
        foreach (var resource in _cityWindowProps.Resources.Resources)
        {
            Controls.Add(new ResourceProductionBar(this, resource));
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

    public void UpdateProduction()
    {
        City.CalculateOutput(City.Owner.Government, CurrentGameScreen.Game);
        ResourceProductionChanged(this, EventArgs.Empty);
    }

    public event EventHandler ResourceProductionChanged;
}