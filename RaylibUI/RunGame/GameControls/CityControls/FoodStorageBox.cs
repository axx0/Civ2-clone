using System.Numerics;
using Civ2engine;
using Civ2engine.Improvements;
using Model.Images;
using Raylib_cs;

namespace RaylibUI.RunGame.GameControls.CityControls;

public class FoodStorageBox : BaseControl
{
    private readonly CityWindow _cityWindow;
    private readonly Color _pen1;
    private readonly Color _pen2;
    private readonly Texture2D _foodIcon;
    private readonly string _text;
    private readonly Vector2 _textDim;

    public FoodStorageBox(CityWindow cityWindow) : base(cityWindow, true)
    {
        _cityWindow = cityWindow;
        _pen1 = new Color(75, 155, 35, 255);
        _pen2 = new Color(0, 51, 0, 255);
        _foodIcon = TextureCache.GetImage(_cityWindow.CurrentGameScreen.Main.ActiveInterface.ResourceImages
            .First(r => r.Name == "Food")
            .LargeImage);
        
        _text = Labels.For(LabelIndex.FoodStorage);
        _textDim = Raylib.MeasureTextEx(Fonts.AlternativeFont, _text, 16, 1);
    }
    

    public override void Draw(bool pulse)
    {
        base.Draw(pulse);
    

    var foodPerRow = _cityWindow.City.Size + 1;
        var spacing = _foodIcon.width;
        var boxWidth = _foodIcon.width * foodPerRow + 6;
        
        if (boxWidth > Width - 10)
        {
            spacing = (Width - 10 - _foodIcon.width) / (foodPerRow - 1);
            boxWidth = 10 + foodPerRow  * spacing + _foodIcon.width;
        }
        var posX = Location.X + Width / 2f - boxWidth / 2f;
        var posY = Location.Y + 16;
        Raylib.DrawLineEx(new Vector2( posX, posY),new Vector2(posX + boxWidth, posY), 1f, _pen1);
        // 2nd horizontal line
        posY = Location.Y + 160;
        Raylib.DrawLineEx(new Vector2( posX, posY),new Vector2(posX + boxWidth, posY), 1f, _pen2);
        // 1st vertical line
        posY = Location.Y + 15;
        int lineHeight = 144;
        Raylib.DrawLineEx(new Vector2( posX, posY),new Vector2(posX , posY + lineHeight), 1f, _pen1);
            ;
        // 2nd vertical line
        Raylib.DrawLineEx(new Vector2( posX + boxWidth, posY),new Vector2(posX + boxWidth, posY + lineHeight), 1f, _pen2);

        var storage = _cityWindow.City.GetFoodStorage();
        if (storage > 0)
        {
            var line_width = boxWidth- 10;
            var starting_x = posX + 5;
            var starting_y = posY + lineHeight * storage / 100f;
            Raylib.DrawLineEx(new Vector2( starting_x, starting_y),new Vector2(starting_x + line_width, starting_y), 1f, _pen1);
        }
        
        var foodStore = _cityWindow.City.FoodInStorage;
        
        int count = 0;
        posX += 3;
        for (int row = 0; row < 10 && count < foodStore; row++)
        {
            for (int col = 0; col < foodPerRow && count < foodStore; col++)
            {
                Raylib.DrawTexture(_foodIcon, (int)posX + spacing * col, (int)Location.Y +  15 + 3 + _foodIcon.height * row,Color.WHITE);
                count++;
            }
        }
        
        Raylib.DrawTextEx(Fonts.AlternativeFont, _text,
            new Vector2(Location.X + Width / 2f - _textDim.X / 2, Location.Y), 16, 1,
            new Color(70,127,47,255));
    }
}