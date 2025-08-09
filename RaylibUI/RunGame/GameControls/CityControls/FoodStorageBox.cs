using System.Numerics;
using Civ2engine;
using Model;
using Model.Images;
using Raylib_CSharp.Colors;
using Raylib_CSharp.Fonts;
using Raylib_CSharp.Rendering;
using Raylib_CSharp.Textures;
using RaylibUI.BasicTypes.Controls;
using Raylib_CSharp.Transformations;
using Model.CityWindowModel;

namespace RaylibUI.RunGame.GameControls.CityControls;

public class FoodStorageBox : BaseControl
{
    private readonly CityWindow _cityWindow;
    private readonly Color _pen1;
    private readonly Color _pen2;
    private readonly Texture2D _foodIcon;
    private readonly string _text;
    private Vector2 _textDim;
    private readonly IUserInterface _active;
    private readonly City _city;
    private float _fontSize, _iconWidth, _iconHeight;
    private readonly int _boxRows;
    private readonly LabelControl _label;
    private readonly Rectangle _panelBounds;

    public FoodStorageBox(CityWindow cityWindow) : base(cityWindow, true)
    {
        _cityWindow = cityWindow;
        _city = _cityWindow.City;
        _active = cityWindow.MainWindow.ActiveInterface;
        _panelBounds = _cityWindow.CityWindowProps.FoodStorage;
        _pen1 = new Color(75, 155, 35, 255);
        _pen2 = new Color(0, 51, 0, 255);
        _foodIcon = TextureCache.GetImage(_active.ResourceImages
            .First(r => r.Name == "Food")
            .LargeImage);
        _text = Labels.For(LabelIndex.FoodStorage);
        _boxRows = cityWindow.CurrentGameScreen.Game.Rules.Cosmic.RowsFoodBox;

        Children = [new CityLabel(cityWindow, _text, _active.Look.CityWindowFont, _active.Look.CityWindowFontSize, _pen1, Color.Black)
        {
            AbsolutePosition = new Rectangle(_panelBounds.X, _panelBounds.Y, _panelBounds.Width, 15)
        }];
    }

    public override void OnResize()
    {
        AbsolutePosition = _panelBounds.ScaleAll(_cityWindow.Scale);
        base.OnResize();

        _iconWidth = _foodIcon.Width * _cityWindow.Scale;
        _iconHeight = _foodIcon.Height * _cityWindow.Scale;

        foreach (var child in Children)
        {
            child.OnResize();
        }
    }

    public override void Draw(bool pulse)
    {
        base.Draw(pulse);

        //Graphics.DrawRectangleLinesEx(Bounds, 1f, Color.Yellow);

        var foodPerRow = _city.Size + 1;
        var wheat_spacing = _city.Size switch
        {
            int n when (n <= 9) => 17,
            int n when (n == 10) => 16,
            int n when (n == 11) => 13,
            int n when (n == 12) => 12,
            int n when (n == 13) => 11,
            int n when (n == 14) => 10,
            int n when (n == 15 || n == 16) => 9,
            int n when (n == 17) => 8,
            int n when (n >= 18 && n <= 20) => 7,
            int n when (n == 21 || n == 22) => 6,
            int n when (n >= 23 && n <= 26) => 5,
            int n when (n >= 27 && n <= 33) => 4,
            int n when (n >= 34 && n <= 40) => 3,
            int n when (n >= 41 && n <= 80) => 2,
            int n when (n >= 81) => 1,
            _ => 17,
        };
        wheat_spacing = (int)(wheat_spacing * _cityWindow.Scale);
        var boxWidth = _city.Size * wheat_spacing + _iconWidth + 7 * _cityWindow.Scale;

        // 1st horizontal line
        var posX = Location.X + Width / 2f - boxWidth / 2f;
        var posY = Location.Y + 15 * _cityWindow.Scale;
        Graphics.DrawLineEx(new Vector2(posX, posY), new Vector2(posX + boxWidth, posY), 1f, _pen1);
        // 2nd horizontal line
        posY = Location.Y + 160 * _cityWindow.Scale;
        Graphics.DrawLineEx(new Vector2(posX, posY), new Vector2(posX + boxWidth, posY), 1f, _pen2);
        // 1st vertical line
        posY = Location.Y + 15 * _cityWindow.Scale;
        int lineHeight = (int)(144 * _cityWindow.Scale);
        Graphics.DrawLineEx(new Vector2(posX, posY),new Vector2(posX , posY + lineHeight), 1f, _pen1);
        // 2nd vertical line
        Graphics.DrawLineEx(new Vector2(posX + boxWidth, posY),new Vector2(posX + boxWidth, posY + lineHeight), 1f, _pen2);

        // Draw wheat icons
        var foodStore = _city.FoodInStorage;
        int count = 0;
        posX += 3 * _cityWindow.Scale;
        for (int row = 0; row < _boxRows && count < foodStore; row++)
        {
            for (int col = 0; col < foodPerRow && count < foodStore; col++)
            {
                Graphics.DrawTextureEx(_foodIcon,
                    new Vector2((int)posX + wheat_spacing * col, Location.Y + 15 * _cityWindow.Scale + 3 + _iconHeight * row),
                    0f, _cityWindow.Scale, Color.White);
                count++;
            }
        }

        // 3rd horizontal line (granary effect)
        var storage = _city.GetFoodStorage();
        if (storage > 0)
        {
            var lineWidth = boxWidth - 10 * _cityWindow.Scale;
            var startingX = posX + 2 * _cityWindow.Scale;
            var startingY = Location.Y + 87 * _cityWindow.Scale;
            Graphics.DrawLineEx(new Vector2(startingX, startingY), new Vector2(startingX + lineWidth, startingY), 1f, _pen1);
        }
    }
}