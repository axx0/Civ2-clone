using System.Numerics;
using Civ2engine;
using Model;
using Raylib_cs;
using RaylibUI.Controls;

namespace RaylibUI.RunGame;

public class CityWindow : BaseDialog
{
    private readonly GameScreen _gameScreen;
    private readonly City _city;
    private readonly CityWindowLayout _cityWindowProps;
    private readonly HeaderLabel _headerLabel;

    public CityWindow(GameScreen gameScreen, City city) : base(gameScreen.Main)
    {
        _gameScreen = gameScreen;
        _city = city;

        _cityWindowProps = gameScreen.Main.ActiveInterface.GetCityWindowDefinition();

        DialogWidth = _cityWindowProps.Width + PaddingSide * 2;
        DialogHeight = _cityWindowProps.Height + Top + PaddingBtm;
        BackgroundImage = ImageUtils.PaintDialogBase(DialogWidth, DialogHeight, Top, PaddingBtm, PaddingSide, _cityWindowProps.Image);
        
        
        _headerLabel = new HeaderLabel(this, _city.Name);
        Controls.Add(_headerLabel);

        var closeButton = new Button(this, "Close");
        Controls.Add(closeButton);
        closeButton.Bounds = new Rectangle(875, 620, 85, 36);
        closeButton.Click += CloseButtonOnClick;
    }

    private void CloseButtonOnClick(object? sender, MouseEventArgs e)
    {
        _gameScreen.CloseDialog(this);
    }

    public int DialogWidth { get; }

    public int DialogHeight { get; }

    private const int Top = 38;

    private const int PaddingBtm = 11;

    public const int PaddingSide = 11;


    public override void Resize(int width, int height)
    {
        SetLocation(width, DialogWidth, height, DialogHeight);
        foreach (var control in Controls)
        {
            control.OnResize();
        }
    }
}