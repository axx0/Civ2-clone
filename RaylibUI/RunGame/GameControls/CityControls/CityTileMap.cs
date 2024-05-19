using System.Numerics;
using Civ2engine;
using Civ2engine.Enums;
using Civ2engine.MapObjects;
using Model;
using Raylib_cs;
using RaylibUI.RunGame.GameControls.Mapping;
using RaylibUtils;

namespace RaylibUI.RunGame.GameControls.CityControls;

public class CityTileMap : BaseControl
{
    private readonly CityWindow _cityWindow;
    private Texture2D? _texture;
    private float _scaleFactor;
    private Vector2 _offset;
    private readonly Vector2 _textDim;
    private readonly string _text;
    private readonly IUserInterface _active;

    public CityTileMap(CityWindow cityWindow) : base(cityWindow)
    {
        _cityWindow = cityWindow;
        _active = cityWindow.MainWindow.ActiveInterface;
        Click += OnClick;
        _text = Labels.For(LabelIndex.ResourceMap);
        _textDim = Raylib.MeasureTextEx(_active.Look.CityWindowFont, _text, _active.Look.CityWindowFontSize, 1);
    }

    private void OnClick(object? sender, MouseEventArgs e)
    {
        var originalClickPos = GetRelativeMousePosition();
        var removeOffset = originalClickPos - _offset;
        var restoreScale = removeOffset / _scaleFactor;
        
        var city = _cityWindow.City;
        var gameScreen = _cityWindow.CurrentGameScreen;

        var tileCache = gameScreen.TileCache;

        var dim = tileCache.GetDimensions(city.Location.Map);

        var zeroY = city.Location.Y - 3;
        var (ydim, yrem )= Math.DivRem((int)restoreScale.Y, dim.HalfHeight);
        var y = zeroY + ydim;
        
        var odd = y % 2 == city.Location.Odd;

        if (odd)
        {
            restoreScale.X -= dim.HalfWidth;
        }


        var zeroX = city.Location.XIndex - 1;

        var (xdim, xrem )= Math.DivRem((int)restoreScale.X, dim.TileWidth);
        var x = zeroX + xdim;
        
        if (x < 0)
        {
            if (city.Location.Map.Flat)
            {
                x = 0;
            }
            else
            {
                x += dim.TotalWidth;
            }
        }
        else if (x > dim.TotalWidth)
        {
            if (city.Location.Map.Flat)
            {
                x = dim.TotalWidth - 1;
            }
            else
            {
                x -= dim.TotalWidth;
            }
        }

        if (xrem < dim.HalfWidth && y > 0)
        {
            if (yrem *  dim.HalfWidth + xrem *  dim.HalfHeight < dim.DiagonalCut)
            {
                y -= 1;
                if (!odd)
                {
                    x -= 1;
                    if (x < 0)
                    {
                        x = city.Location.Map.Flat ? 0 : city.Location.Map.Tile.GetLength(0) - 1;
                    }
                }
            }
        }
        else if (xrem > dim.HalfWidth)
        {
            if ((dim.TileWidth - xrem) *  dim.HalfHeight + yrem *  dim.HalfWidth < dim.DiagonalCut)
            {
                y -= 1;
                if (odd)
                {
                    x += 1;
                    if (x == city.Location.Map.Tile.GetLength(0))
                    {
                        if (city.Location.Map.Flat)
                        {
                            x -= 1;
                        }
                        else
                        {
                            x = 0;
                        }
                    }
                }
            }
        }

        if (city.Location.Odd == 0 && y % 2 == 1)
        {
            //I don't know why this adjustment is needed, there's probably a bug earlier in the function
            x -= 1;
        }

        //If we don't have a valid tile return 
        if (0 > y || y >= city.Location.Map.Tile.GetLength(1)) return;
        
        var tile = city.Location.Map.Tile[x, y];
        // if the tile is outside the city radius do nothing
        if (!city.Location.CityRadius().Contains(tile)) return;

        // if there are foreign units here can't use this square
        if (tile.UnitsHere.Any(u => u.Owner != city.Owner))
        {
            // play bad action sound??
            return;
        }
        if (tile.CityHere != null)
        {
            if (tile.CityHere != city)
            {
                //play bad action sound?
                return;
            }

            foreach (var wt in city.WorkedTiles.ToArray())
            {
                wt.WorkedBy = null;
            }
            city.AutoAddDistributionWorkers();
                    
        }else if (tile.WorkedBy != null)
        {
            if (tile.WorkedBy != city)
            {
                //Play bad action sound?
                return;
            }
            tile.WorkedBy = null;
        }
        else
        {
            // if equal we still allow since worked tiles include the city centre
            if (city.WorkedTiles.Count > city.Size)
            {
                // Play bad action?
                return;
            }
            tile.WorkedBy = city;
        }

        _cityWindow.UpdateProduction();
        Redraw();
    }

    public override void Draw(bool pulse)
    {
        Raylib.DrawTextureEx(_texture.Value, Location + _offset, 0, _scaleFactor, Color.White);

        Raylib.DrawTextEx(_active.Look.CityWindowFont, _text,
            new Vector2(Location.X + Width / 2f - _textDim.X / 2, Location.Y + Height - _textDim.Y), 
            _active.Look.CityWindowFontSize, 1, Color.Gold);
    }

    public override void OnResize()
    {
        base.OnResize();
        Redraw();
    }

    private void Redraw()
    {
        var city = _cityWindow.City;
        var gameScreen = _cityWindow.CurrentGameScreen;

        var cities = gameScreen.Main.ActiveInterface.CityImages;
        var unitsSet = gameScreen.Main.ActiveInterface.UnitImages;
        var tileCache = gameScreen.TileCache;

        var dim = tileCache.GetDimensions(city.Location.Map);
        var width = dim.TileWidth * 4;
        var height = dim.TileHeight * 4;
        var xcentre = width / 2 - dim.HalfWidth;
        var ycentre = height / 2 - dim.HalfHeight;
        var image = Raylib.GenImageColor(width, height, new Color(0, 0, 0, 0));

        var cityData = new List<Element>();
        var units = new List<Element>();
        var activeCiv = _cityWindow.CurrentGameScreen.Player.Civilization;
        foreach (var tile in city.Location.CityRadius())
        {
            // TODO: city.owner should be replaced by the current active user when viewing other peoples cities
            if (tile.IsVisible(activeCiv.Id))
            {
                var tileImage = tileCache.GetTileDetails(tile, city.Owner.Id);
                var locationX = xcentre + (tile.X - city.Location.X) * dim.HalfWidth;
                var locationY = ycentre + (tile.Y - city.Location.Y) * dim.HalfHeight;
                Raylib.ImageDraw(ref image, tileImage.Image,
                    MapImage.TileRec,
                    new Rectangle(locationX,
                        locationY, dim.TileWidth, dim.TileHeight),
                    Color.White);
                if (tile.CityHere != null)
                {
                    var cityStyleIndex = tile.CityHere.Owner.CityStyle;
                    if (tile.CityHere.Owner.Epoch == EpochType.Industrial)
                    {
                        cityStyleIndex = 4;
                    }
                    else if (tile.CityHere.Owner.Epoch == EpochType.Modern)
                    {
                        cityStyleIndex = 5;
                    }
                    var sizeIncrement =
                        gameScreen.Main.ActiveInterface.GetCityIndexForStyle(cityStyleIndex,
                            tile.CityHere, tile.CityHere.Size);
                    cityData.Add(new Element
                    {
                        Image = Images.ExtractBitmap(cities.Sets[cityStyleIndex][sizeIncrement]
                            .Image),
                        X = locationX,
                        Y = locationY - dim.HalfHeight
                    });
                }
                else if (tile.UnitsHere.Any(u => u.Owner != city.Owner))
                {
                    // units.Add(new Element()
                    // {
                    //     Image = ImageUtils.GetUnitImage(gameScreen.Main.ActiveInterface, tile.GetTopUnit()),
                    //     X = locationX,
                    //     Y = locationY - (int)unitsSet.UnitRectangle.Height + dim.TileHeight
                    // });
                }

                if (tile.WorkedBy != null && tile.WorkedBy != city)
                {
                    Raylib.ImageDraw(ref image, Images.ExtractBitmap(gameScreen.Main.ActiveInterface.MapImages.ViewPiece), MapImage.TileRec,
                        MapImage.TileRec with { X = locationX, Y = locationY }, Color.Red);
                }
            }
        }

        //Cities must be drawn after terrain since they sometimes overdraw onto later tiles
        foreach (var cityDetails in cityData)
        {
            Raylib.ImageDraw(ref image, cityDetails.Image, cities.CityRectangle,
                cities.CityRectangle with { X = cityDetails.X, Y = cityDetails.Y },
                Color.White);
        }

        foreach (var unitDetails in units)
        {
            Raylib.ImageDraw(ref image, unitDetails.Image, unitsSet.UnitRectangle,
                unitsSet.UnitRectangle with { X = unitDetails.X, Y = unitDetails.Y },
                Color.White);
        }

        var resources =
            gameScreen.Main.ActiveInterface.ResourceImages.ToDictionary(k => k.Name,
                v => Images.ExtractBitmap(v.SmallImage));

        var lowOrganisation = city.Owner.Government <= GovernmentType.Despotism;
        var totalDrawWidth = dim.TileWidth - 20;
        var resourceXOffset = 10;
        var resourceWidth = resources.First().Value.Height;
        var resourceYOffset = dim.HalfHeight - resourceWidth / 2;
        var resourceRect = new Rectangle(0, 0, resourceWidth, resourceWidth);
        foreach (var workedTile in city.WorkedTiles)
        {
            var food = workedTile.GetFood(lowOrganisation);
            var shields = workedTile.GetShields(lowOrganisation);
            var trade = workedTile.GetTrade(city.OrganizationLevel);

            var totalResources = food + shields + trade;
            if (totalResources > 0)
            {
                var locationX = xcentre + (workedTile.X - city.Location.X) * dim.HalfWidth + resourceXOffset;
                var locationY = ycentre + (workedTile.Y - city.Location.Y) * dim.HalfHeight + resourceYOffset;
                var spacing = Math.Min(resourceWidth + 1, Math.Max(totalDrawWidth / totalResources, 1));
                var destRect = resourceRect with { X = locationX, Y = locationY };
                for (var i = 0; i < food; i++)
                {
                    Raylib.ImageDraw(ref image, resources["Food"], resourceRect, destRect, Color.White);
                    destRect.X += spacing;
                }
                for (var i = 0; i < shields; i++)
                {
                    Raylib.ImageDraw(ref image, resources["Shields"], resourceRect, destRect, Color.White);
                    destRect.X += spacing;
                }
                for (var i = 0; i < trade; i++)
                {
                    Raylib.ImageDraw(ref image, resources["Trade"], resourceRect, destRect, Color.White);
                    destRect.X += spacing;
                }
            }
        }

        if (_texture.HasValue)
        {
            Raylib.UnloadTexture(_texture.Value);
        }

        _texture = Raylib.LoadTextureFromImage(image);
        _scaleFactor = Width / (float)_texture.Value.Width;
        _offset = new Vector2(0, (height - height * _scaleFactor) / 2f);
        Raylib.UnloadImage(image);
    }
}

public struct Element
{
    public Image Image { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
}