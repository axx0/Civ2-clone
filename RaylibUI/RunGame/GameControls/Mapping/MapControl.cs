using Civ2engine;
using Civ2engine.MapObjects;
using Raylib_cs;
using RaylibUI.BasicTypes;

namespace RaylibUI.RunGame.GameControls.Mapping;

public class MapControl : BaseControl
{
    private readonly Game _game;
    private Texture2D? _backgroundImage;
    private readonly Texture2D[,] _mapTileTexture;
    private readonly int _tileWidth;
    private readonly int _tileHeight;
    private int[] _offsets;
    private readonly Map _map;
    private readonly int _halfHeight;
    private int _xStart;
    private int _yStart;

    public MapControl(IControlLayout controller, Game game) : base(controller)
    {
        _game = game;
        _map = game.CurrentMap;
        var map = _map;

        _mapTileTexture = new Texture2D[map.XDim, map.YDim];
        for (var col = 0; col < map.XDim; col++)
        {
            for (var row = 0; row < map.YDim; row++)
            {
                var image = MapImage.MakeTileGraphic(map.Tile[col, row], col, row, map, MapImages.Terrains[map.MapIndex], game);
                _mapTileTexture[col, row] = Raylib.LoadTextureFromImage(image);
                Raylib.UnloadImage(image);
            }
        }

        _tileWidth = _mapTileTexture[0, 0].width;
        _tileHeight = _mapTileTexture[0, 0].height;
        _halfHeight = _tileHeight / 2;
    }


    public override Size GetPreferredSize(int width, int height)
    {
        return new Size(width - GameScreen.MiniMapWidth, height);
    }

    public override void OnResize()
    {
        _backgroundImage = ImageUtils.PaintDialogBase(Width, Height, 11, 11, 11);
        var activeTile = _game.ActiveTile;
        _offsets = new[] { activeTile.XIndex * _tileWidth + Width/2 - _tileWidth/2, Height/2 - (activeTile.Y * _tileHeight) /2 };
        _xStart = (activeTile.XIndex * _tileWidth - Width / 2)/_tileWidth;
        _yStart = (activeTile.Y * _halfHeight - Height / 2)/_halfHeight;
        base.OnResize();
    }

    public override void Draw(bool pulse)
    {
        Raylib.DrawTexture(_backgroundImage.Value, (int)Location.X, (int)Location.Y, Color.WHITE);

        var activeTile = _game.ActiveTile;
        var initialPos = _xStart * _tileWidth - _offsets[0];

        var ypos = (int)Location.Y + 11;
        var maxX = (int)Location.X + Width - 22;
        var maxY = (int)Location.Y + Height -22;

            var y = Math.Max(_yStart, 0);
            var xStart = Math.Max(_xStart, 0);
        while (ypos < maxY)
        {
            var xpos = (int)Location.X + 11;
            if (y % 2 == 1)
            {
                xpos += _tileWidth / 2;
            }
                var x = xStart;
            while (xpos < maxX)
            {

                Raylib.DrawTexture(_mapTileTexture[x, y], xpos, ypos, Color.WHITE);
                xpos += _tileWidth;
                x++;
            }

            y++;
            ypos += _halfHeight;
        }
    
        Raylib.DrawTexture(_mapTileTexture[activeTile.XIndex, activeTile.Y],
            (int)Location.X + Width / 2 - _tileWidth / 2, (int)Location.Y + Height / 2 - _tileHeight / 2, Color.WHITE);
    // for (int y = -2; y < Height + 2; y++)
        // {
        //     for (int x = -2; x < Width; x++)
        //     {
        //         // Determine column index in civ2-style coords
        //         int col = 2 * x + (y % 2);
        //         int row = y;
        //
        //         // Don't draw beyond borders
        //         var xC2 = _offsets[0] + col;
        //         var yC2 = _offsets[1] + row;
        //         if (xC2 < 0 || yC2 < 0 || xC2 >= _map.XDimMax || yC2 >= _map.YDim) continue;
        //
        //         Raylib.DrawTexture(_mapTileTexture[xC2, yC2], 32 * col, 16 * row, Color.WHITE);
        //
        //         base.Draw(pulse);
        //     }
        // }
    }
}