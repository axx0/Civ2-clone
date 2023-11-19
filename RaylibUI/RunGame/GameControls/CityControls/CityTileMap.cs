using System.Numerics;
using Civ2engine;
using Civ2engine.MapObjects;
using Raylib_cs;
using RaylibUI.RunGame.GameControls.Mapping;

namespace RaylibUI.RunGame.GameControls;

public class CityTileMap : BaseControl
{
    private readonly CityWindow _cityWindow;
    private Texture2D? _texture;
    private float _scaleFactor;
    private Vector2 _offset;

    public CityTileMap(CityWindow cityWindow) : base(cityWindow)
    {
        _cityWindow = cityWindow;
        Click += OnClick;
    }

    private void OnClick(object? sender, MouseEventArgs e)
    {
        throw new NotImplementedException();
    }

    public override void Draw(bool pulse)
    {
        Raylib.DrawTextureEx(_texture.Value, Location + _offset,0,_scaleFactor,Color.WHITE);
        Raylib.DrawRectangleLines((int)Bounds.x, (int)Bounds.y, Width,Height,Color.MAGENTA);
        Raylib.DrawTextEx(Fonts.DefaultFont,  "Tile Map", Location, 20,1,Color.MAGENTA );
    }

    public override void OnResize()
    {
        base.OnResize();
        var city = _cityWindow.City;
        var gameScreen = _cityWindow.CurrentGameScreen;
        
        var cities = gameScreen.Main.ActiveInterface.CityImages;
        var tileCache = gameScreen.TileCache;
        
        var dim = tileCache.GetDimensions(city.Location.Map);
        var width = dim.TileWidth * 4;
        var height = dim.TileHeight * 4;
        var xcentre = width / 2 - dim.HalfWidth;
        var ycentre = height / 2 - dim.HalfHeight;
        var image = ImageUtils.NewImage(width, height);
        
        var cityData = new List<CityLoc>();
        foreach (var tile in city.Location.CityRadius())
        {
            if (tile.Visibility[city.Owner.Id])
            {
                var tileImage = tileCache.GetTextureForTile(tile);
                var locationX = xcentre + (tile.X - city.Location.X) * dim.HalfWidth;
                var locationY = ycentre + (tile.Y - city.Location.Y) * dim.HalfHeight;
                Raylib.ImageDraw(ref image, tileImage,
                    MapImage.TileRec,
                    new Rectangle(locationX,
                        locationY, dim.TileWidth, dim.TileHeight),
                    Color.WHITE);
                if (tile.CityHere != null)
                {
                    var cityStyleIndex = tile.CityHere.Owner.CityStyle;
                    var sizeIncrement =
                        gameScreen.Main.ActiveInterface.GetCityIndexForStyle(cityStyleIndex,
                            tile.CityHere);
                    cityData.Add(new CityLoc
                    {
                        Image = cities.Sets[cityStyleIndex][sizeIncrement]
                            .Image,
                        X = locationX,
                        Y = locationY - dim.HalfHeight
                    });
                }
            }
        }
        
        //Cities must be drawn after terrain since they sometimes overdraw onto later tiles
        foreach (var cityDetails in cityData)
        {
            Raylib.ImageDraw(ref image, cityDetails.Image, cities.CityRectangle,
                cities.CityRectangle with { x = cityDetails.X, y = cityDetails.Y },
                Color.WHITE);
        }

        if (_texture.HasValue)
        {
            Raylib.UnloadTexture(_texture.Value);
        }

        _texture = Raylib.LoadTextureFromImage(image);
        _scaleFactor = Width / (float)_texture.Value.width;
        _offset = new Vector2(0,(Height - height * _scaleFactor) / 2f);
        Raylib.UnloadImage(image);
    }
}

public struct CityLoc
{
    public Image Image { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
}