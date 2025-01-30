using System.Numerics;
using Civ2engine;
using Civ2engine.Enums;
using Civ2engine.MapObjects;
using Model;
using Model.Core;
using Raylib_CSharp.Colors;
using Raylib_CSharp.Fonts;
using Raylib_CSharp.Images;
using Raylib_CSharp.Rendering;
using Raylib_CSharp.Textures;
using Raylib_CSharp.Transformations;
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
    private readonly int _organizationLevel;

    private IList<IViewElement> _viewElements;

    public CityTileMap(CityWindow cityWindow, IGame game) : base(cityWindow)
    {
        _cityWindow = cityWindow;
        _active = cityWindow.MainWindow.ActiveInterface;
        Click += OnClick;
        _text = Labels.For(LabelIndex.ResourceMap);
        _textDim = TextManager.MeasureTextEx(_active.Look.CityWindowFont, _text, _active.Look.CityWindowFontSize, 1);
        _organizationLevel = cityWindow.City.GetOrganizationLevel(game.Rules);
    }

    private void OnClick(object? sender, MouseEventArgs e)
    {
        var originalClickPos = GetRelativeMousePosition();
        var removeOffset = originalClickPos - _offset;
        var restoreScale = removeOffset / _scaleFactor;
        
        var city = _cityWindow.City;
        var gameScreen = _cityWindow.CurrentGameScreen;

        var tileCache = gameScreen.TileCache;

        Map map = city.Location.Map;
        var dim = tileCache.GetDimensions(map, _cityWindow.CurrentGameScreen.Zoom);

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
            city.AutoAddDistributionWorkers(gameScreen.Game.Rules);
                    
        }
        else if (tile.WorkedBy != null)
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
        var adjustedLocation = Location + _offset;
        Graphics.DrawTextureEx(_texture.Value, adjustedLocation, 0, _scaleFactor, Color.White);

        foreach (var element in _viewElements)
        {
            element.Draw(element.Location * _scaleFactor + adjustedLocation, _scaleFactor);
        }

        Graphics.DrawTextEx(_active.Look.CityWindowFont, _text,
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

        Map map = city.Location.Map;
        var dim = tileCache.GetDimensions(map, gameScreen.Zoom);
        var width = dim.TileWidth * 4;
        var height = dim.TileHeight * 4;
        var xcentre = width / 2 - dim.HalfWidth;
        var ycentre = height / 2 - dim.HalfHeight;
        var image = Image.GenColor(width, height, new Color(0, 0, 0, 0));

        var elements = new List<IViewElement>();
        var cityData = new List<Element>();
        var activeCiv = _cityWindow.CurrentGameScreen.Player.Civilization;
        foreach (var tile in city.Location.CityRadius())
        {
            // We use the active civ here not the city owner as we may be viewing other players cities
            if (tile.IsVisible(activeCiv.Id))
            {
                var tileImage = tileCache.GetTileDetails(tile, city.Owner.Id);
                var locationX = xcentre + (tile.X - city.Location.X) * dim.HalfWidth;
                var locationY = ycentre + (tile.Y - city.Location.Y) * dim.HalfHeight;
                var dstRec = new Rectangle(locationX,
                    locationY, dim.TileWidth, dim.TileHeight);
                
                image.Draw(tileImage.Image,
                    MapImage.TileRec,
                    dstRec,
                    Color.White);
                if (tile.CityHere != null)
                {
                    var cityStyleIndex = tile.CityHere.Owner.CityStyle;
                    if (tile.CityHere.Owner.Epoch == (int)EpochType.Industrial)
                    {
                        cityStyleIndex = 4;
                    }
                    else if (tile.CityHere.Owner.Epoch == (int)EpochType.Modern)
                    {
                        cityStyleIndex = 5;
                    }
                    var sizeIncrement =
                        gameScreen.Main.ActiveInterface.GetCityIndexForStyle(cityStyleIndex,
                            tile.CityHere, tile.CityHere.Size);
                    cityData.Add(new Element
                    {
                        Image = Images.ExtractBitmap(cities.Sets[cityStyleIndex][sizeIncrement]
                            .Image, gameScreen.Main.ActiveInterface),
                        DestRec = dstRec
                    });
                }
                else if (tile.UnitsHere.Any(u => u.Owner != city.Owner))
                {
                    ImageUtils.GetUnitTextures(tile.GetTopUnit(), _active, _cityWindow.CurrentGameScreen.Game, elements,
                        new Vector2(locationX, locationY - (int)unitsSet.UnitRectangle.Height + dim.TileHeight), true);
                    // units.Add(new Element()
                    // {
                    //     Image = ImageUtils.GetUnitImage(gameScreen.Main.ActiveInterface, tile.GetTopUnit()),
                    //     X = locationX,
                    //     Y = locationY - (int)unitsSet.UnitRectangle.Height + dim.TileHeight
                    // });
                }

                if (tile.WorkedBy != null && tile.WorkedBy != city)
                {
                    image.Draw(Images.ExtractBitmap(gameScreen.Main.ActiveInterface.MapImages.ViewPiece, _active),
                        MapImage.TileRec, dstRec, Color.Red);
                }
            }
        }

        //Cities must be drawn after terrain since they sometimes overdraw onto later tiles
        foreach (var cityDetails in cityData)
        {
            image.Draw(cityDetails.Image, cities.CityRectangle,
                cityDetails.DestRec,
                Color.White);
        }


        var resources =
            gameScreen.Main.ActiveInterface.ResourceImages.ToDictionary(k => k.Name,
                v => Images.ExtractBitmap(v.SmallImage, gameScreen.Main.ActiveInterface));

        var lowOrganisation = _organizationLevel == 0;
        var totalDrawWidth = dim.TileWidth - 20;
        var resourceXOffset = 10;
        var resourceWidth = resources.First().Value.Width;
        var resourceHeight = resources.First().Value.Height;
        var resourceYOffset = dim.HalfHeight - resourceHeight / 2;
        var resourceRect = new Rectangle(0, 0, resourceWidth, resourceHeight);
        foreach (var workedTile in city.WorkedTiles)
        {
            var food = workedTile.GetFood(lowOrganisation);
            var shields = workedTile.GetShields(lowOrganisation);
            var trade = workedTile.GetTrade(_organizationLevel);

            var totalResources = food + shields + trade;
            if (totalResources > 0)
            {
                var locationX = xcentre + (workedTile.X - city.Location.X) * dim.HalfWidth + resourceXOffset;
                var locationY = ycentre + (workedTile.Y - city.Location.Y) * dim.HalfHeight + resourceYOffset;
                var spacing = Math.Min(resourceWidth + 1, Math.Max(totalDrawWidth / totalResources, 1));
                var destRect = resourceRect with { X = locationX, Y = locationY };
                for (var i = 0; i < food; i++)
                {
                    image.Draw(resources["Food"], resourceRect, destRect, Color.White);
                    destRect.X += spacing;
                }
                for (var i = 0; i < shields; i++)
                {
                    image.Draw(resources["Shields"], resourceRect, destRect, Color.White);
                    destRect.X += spacing;
                }
                for (var i = 0; i < trade; i++)
                {
                    image.Draw(resources["Trade"], resourceRect, destRect, Color.White);
                    destRect.X += spacing;
                }
            }
        }

        if (_texture.HasValue)
        {
            _texture.Value.Unload();
        }

        _viewElements = elements;

        _texture = Texture2D.LoadFromImage(image);
        _scaleFactor = Width / (float)_texture.Value.Width;
        
        _offset = new Vector2(0, (Height - height * _scaleFactor) / 2f);
        image.Unload();
    }
}

public struct Element
{
    public Image Image { get; init; }
    public Rectangle DestRec { get; init; }
}