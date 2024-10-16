using System.Numerics;
using Civ2engine;
using Model;
using Raylib_CSharp.Colors;
using Raylib_CSharp.Rendering;
using Raylib_CSharp.Textures;

namespace RaylibUI.RunGame.GameControls.CityControls;

public class ShieldBox : BaseControl
{
    private readonly CityWindow _cityWindow;
    private readonly Texture2D _shieldIcon;
    private readonly IUserInterface _activeInterface;
    private int _totalCost;
    private readonly int _shieldBoxRows;
    private readonly Color _pen1;
    private readonly Color _pen2;

    public ShieldBox(CityWindow cityWindow) : base(cityWindow, eventTransparent: true)
    {
        
        _pen1 = new Color(80, 89, 182, 255);
        _pen2 = new Color(0, 3, 84, 255);
        _shieldBoxRows = cityWindow.CurrentGameScreen.Game.Rules.Cosmic.RowsShieldBox;
        _cityWindow = cityWindow;
        _activeInterface = cityWindow.MainWindow.ActiveInterface;
        _shieldIcon = TextureCache.GetImage(_cityWindow.CurrentGameScreen.Main.ActiveInterface.ResourceImages
            .First(r => r.Name == "Shields")
            .LargeImage);
    }

    public override void Draw(bool pulse)
    {
        base.Draw(pulse);
        
        int lines = (Height - 6) / _shieldIcon.Height;
        int requiredLines = _cityWindow.City.ItemInProduction.Cost;
        int shieldsPerRow = _shieldBoxRows;
        if (lines > requiredLines)
        {
            lines = requiredLines;
        }
        else
        {
            shieldsPerRow = (int)Math.Ceiling(_totalCost / (decimal)lines);
        }

        var posX = Location.X;
        var posY = Location.Y;

        var drawWidth = Width - 6;
        var spacing = _shieldIcon.Width;
        var requiredWidth = shieldsPerRow * spacing;

        Graphics.DrawLineEx(new Vector2( posX, posY),new Vector2(posX + Width, posY), 1f, _pen1);
        // 2nd horizontal line
        var lineHeight = 6 + lines * _shieldIcon.Height;
        posY = Location.Y + lineHeight;
        Graphics.DrawLineEx(new Vector2( posX, posY),new Vector2(posX + Width, posY), 1f, _pen2);
        // 1st vertical line
        posY = Location.Y;
        Graphics.DrawLineEx(new Vector2( posX, posY),new Vector2(posX , posY + lineHeight), 1f, _pen1);
        
        // 2nd vertical line
        Graphics.DrawLineEx(new Vector2( posX + Width, posY),new Vector2(posX + Width, posY + lineHeight), 1f, _pen2);
        
        if (requiredWidth < drawWidth)
        {
            posX += (drawWidth - requiredWidth) / 2f;
        }
        else
        {
            spacing = (drawWidth - _shieldIcon.Width) / shieldsPerRow;
        }
        var shields = _cityWindow.City.ShieldsProgress;
        int count = 0;
        posX += 3;
        for (int row = 0; row < 10 && count < shields; row++)
        {
            for (int col = 0; col < shieldsPerRow && count < shields; col++)
            {
                Graphics.DrawTexture(_shieldIcon, (int)posX + spacing * col, (int)Location.Y + 3 + _shieldIcon.Height * row,Color.White);
                count++;
            }
        }
    }

    public void UpdateData(IProductionOrder itemInProduction)
    {
        _totalCost = itemInProduction.Cost * _shieldBoxRows ;
    }
}