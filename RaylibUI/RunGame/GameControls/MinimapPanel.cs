using Civ2engine;
using Civ2engine.Enums;
using Raylib_cs;
using RaylibUI.BasicTypes;

namespace RaylibUI.RunGame.GameControls;

public class MinimapPanel : BaseControl
{
    private readonly Game _game;
    private Texture2D? _backgroundImage;

    public MinimapPanel(GameScreen controller, Game game) : base(controller)
    {
        _game = game;
        
    }

    public override Size GetPreferredSize(int width, int height)
    {
        return new Size(GameScreen.MiniMapWidth, GameScreen.MiniMapHeight);
    }
    
    public override void OnResize()
    {
        _backgroundImage = ImageUtils.PaintDialogBase(Width, Height, 11,11,11);
        _offset = new[] { ( Width - 2 * _game.CurrentMap.XDim) / 2, ( Height - _game.CurrentMap.YDim) / 2 };
        base.OnResize();
    }
    
    public void Update(int[] startXy, int[] visibleTiles)
    {
        _mapStartXy = startXy;
        _mapDrawSq = visibleTiles;
    }
    
    
    private static readonly Color OceanColor = new (0, 0, 95,255);
    private static readonly Color LandColor = new (55, 123, 23,255);
    private int[] _offset;
    private int[] _mapStartXy = { 0, 0 };
    private int[] _mapDrawSq = { 0, 0 };

    public override void Draw(bool pulse)
    {
        Raylib.DrawTexture(_backgroundImage.Value,(int)Location.X, (int)Location.Y, Color.WHITE);
        Raylib.DrawRectangle((int)Location.X + 11, (int)Location.Y + 11, Width - 2 * 11, Height - 22, Color.BLACK);
        var map = _game.CurrentMap;
        // Draw map
        for (var row = 0; row < map.YDim; row++)
        {
            for (var col = 0; col < map.XDim; col++)
            {
                var tile = map.Tile[col, row];
                if (!map.MapRevealed && !tile.IsVisible(map.WhichCivsMapShown)) continue;

                var drawColor = tile.CityHere is not null
                    ? MapImages.PlayerColours[tile.CityHere.Owner.Id].TextColour
                    : tile.Type == TerrainType.Ocean
                        ? OceanColor
                        : LandColor;

                Raylib.DrawRectangle((int)Location.X + _offset[0] + 2 * col + (row % 2),
                    (int)Location.Y + _offset[1] + row, 2, 1, drawColor);
            }
        }

        // Draw current view rectangle
        Raylib.DrawRectangle(_offset[0] + _mapStartXy[0], _offset[1] + _mapStartXy[1],
            _mapDrawSq[0], _mapDrawSq[1], Color.WHITE);
        base.Draw(pulse);
    }
}