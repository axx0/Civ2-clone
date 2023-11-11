using System.Numerics;
using Civ2engine;
using Civ2engine.MapObjects;
using Raylib_cs;

namespace RaylibUI.RunGame.GameControls.Mapping.Views;

public abstract class BaseGameView : IGameView
{
    private readonly GameScreen _gameScreen;
    private readonly IList<Tile> _actionTiles;
    private int currentIndex = 0;

    private IList<IList<ViewElement>> _animations = new List<IList<ViewElement>>();
    private bool _preserve;
    
    private Vector2 _offsets = Vector2.Zero;
    
    protected readonly Vector2 ActivePos;

    public Vector2 Offsets => _offsets;

    public bool IsDefault { get; }
    public int Interval { get; }

    protected BaseGameView(GameScreen gameScreen, Tile location, IGameView? previousView, int viewHeight, int viewWidth, bool isDefault, int interval, IList<Tile> actionTiles)
    {
        IsDefault = isDefault;
        Interval = interval;
        _gameScreen = gameScreen;
        _actionTiles = actionTiles;

        Location = location;
        ViewWidth = viewWidth;
        ViewHeight = viewHeight;

        var dimensions = _gameScreen.TileCache.GetDimensions(location.Map);
        if (previousView != null && IsInSameArea(previousView, location, dimensions))
        {
            ActivePos = GetPosForTile(location, dimensions);
            this.BaseImage = previousView.BaseImage;
            this.Cities = previousView.Cities;
            Elements = previousView.Elements.Where(e=>e.X != (int)ActivePos.X || e.Y != (int)ActivePos.Y).ToArray();

            previousView.Preserve();

        }
        else
        {

            var civilizationId = _gameScreen.Player.Civilization.Id;
            var cityData = new List<CityData>();
            var elements = new List<ViewElement>();
            if (_offsets == Vector2.Zero)
            {
                CalculateOffsets(null, location, dimensions, force: true);
            }

            var activeInterface = _gameScreen.Main.ActiveInterface;
            var cities = activeInterface.CityImages;

            var imageWidth = ViewWidth;
            var imageHeight = ViewHeight;
            var image = ImageUtils.NewImage(imageWidth, imageHeight);
            var _map = location.Map;
            var dim = _gameScreen.TileCache.GetDimensions(_map);

            Raylib.ImageDrawRectangle(ref image, 0, 0, imageWidth, imageHeight, Color.BLACK);

            var ypos = -_offsets.Y;

            var maxWidth = _map.XDim * dim.TileWidth;

            for (var row = 0; row < _map.YDim; row++)
            {
                if (ypos >= -dim.TileHeight)
                {
                    var xpos = -_offsets.X + (row % 2 * dim.HalfWidth);
                    if (!_map.Flat && xpos + maxWidth < ViewWidth)
                    {
                        xpos += maxWidth;
                    }

                    for (var col = 0; col < _map.XDim; col++)
                    {
                        if (xpos >= -dim.TileWidth)
                        {
                            if (xpos >= imageWidth)
                            {
                                if (!_map.Flat)
                                {
                                    xpos -= maxWidth;
                                }
                                else
                                {
                                    break;
                                }
                            }

                            var tile = _map.Tile[col, row];
                            if (tile == location)
                            {
                                ActivePos = new Vector2(xpos, ypos+ dim.TileHeight);
                            }

                            if (tile.Visibility[civilizationId])
                            {
                                Raylib.ImageDraw(ref image, _gameScreen.TileCache.GetTextureForTile(tile),
                                    MapImage.TileRec,
                                    new Rectangle(xpos, ypos, dim.TileWidth, dim.TileHeight), Color.WHITE);

                                if (tile.CityHere != null)
                                {
                                    var cityStyleIndex = tile.CityHere.Owner.CityStyle;
                                    var sizeIncrement =
                                        _gameScreen.Main.ActiveInterface.GetCityIndexForStyle(cityStyleIndex,
                                            tile.CityHere);
                                    cityData.Add(new CityData
                                    {
                                        Name = tile.CityHere.Name,
                                        Image = Raylib.LoadTextureFromImage(cities.Sets[cityStyleIndex][sizeIncrement]
                                            .Image),
                                        X = (int)xpos,
                                        Y = (int)ypos - dim.HalfHeight
                                    });
                                }
                                else if ( tile.UnitsHere.Count > 0 && !actionTiles.Contains(tile))
                                {
                                    var unit = tile.GetTopUnit();
                                    elements.Add(new ViewElement
                                    {
                                        Image = Raylib.LoadTextureFromImage(activeInterface.UnitImages
                                            .Units[(int)unit.Type].Image),
                                        X = (int)xpos,
                                        Y = (int)ypos + dim.TileHeight
                                    });
                                }
                            }
                        }

                        xpos += dim.TileWidth;
                    }
                }

                ypos += dim.HalfHeight;
                if (ypos > imageHeight)
                {
                    break;
                }
            }


            this.BaseImage = Raylib.LoadTextureFromImage(image);
            this.Cities = cityData.ToArray();
            this.Elements = elements.ToArray();

            Raylib.UnloadImage(image);
        }
    }

    protected Vector2 GetPosForTile(Tile location, MapDimensions dimensions)
    {
        return new Vector2(location.XIndex * dimensions.TileWidth + location.Odd * dimensions.HalfWidth, location.Y * dimensions.HalfHeight+ dimensions.TileHeight) -
               Offsets;
    }

    public Texture2D BaseImage { get; set; }
    public CityData[] Cities { get; set; }

    private bool IsInSameArea(IGameView previousView, Tile location, MapDimensions dimensions)
    {
        if (previousView.Location.Map != location.Map) return false;
        if (previousView.ViewHeight != ViewHeight || previousView.ViewWidth != ViewWidth) return false;

        return !CalculateOffsets(previousView, location, dimensions);
    }

    private bool CalculateOffsets(IGameView? previousView, Tile location, MapDimensions dimensions, bool force = false)
    {
        if (previousView != null)
        {
            _offsets = previousView.Offsets;
        }
        bool setOffsets;
        int offsetY;
        int offsetX;
        if (ViewHeight >= dimensions.TotalHeight)
        {
            offsetY = (dimensions.TotalHeight - ViewHeight) /2;
            setOffsets = offsetY != (int)_offsets.Y;
        }
        else
        {
            var tileTop = location.Y * dimensions.HalfHeight;
            offsetY = tileTop - (ViewHeight / 2);
            if (offsetY < 0)
            {
                offsetY = 0; setOffsets = offsetY != (int)_offsets.Y;
            }
            else if(offsetY > (dimensions.TotalHeight - ViewHeight))
            {
                offsetY = dimensions.TotalHeight - ViewHeight;
                setOffsets = offsetY != (int)_offsets.Y;
            }
            else
            {
                var currentOffsetYPos = tileTop - _offsets.Y;
                setOffsets = currentOffsetYPos < dimensions.TotalHeight || currentOffsetYPos + dimensions.TileHeight * 2 > ViewHeight;
            }
        }

        if (location.Map.Flat && ViewWidth >= dimensions.TotalWidth)
        {
            offsetX = (dimensions.TotalWidth - ViewWidth) /2;
            setOffsets = setOffsets || offsetX != (int)_offsets.X;
        }
        else
        {
            var tileLeft = location.XIndex * dimensions.TileWidth + location.Odd * dimensions.HalfWidth;
            offsetX = tileLeft - (ViewWidth / 2);
            if (location.Map.Flat)
            {
                if (offsetX < 0)
                {
                    offsetX = 0;
                    setOffsets = setOffsets || offsetX != (int)_offsets.X;
                }else if (offsetX > (dimensions.TotalWidth - ViewWidth + dimensions.HalfWidth))
                {
                    offsetX = dimensions.TotalWidth - ViewWidth + dimensions.HalfWidth;
                    setOffsets = setOffsets || offsetX != (int)_offsets.X;
                }
                else
                {
                    var currentOffsetXPos = tileLeft - _offsets.X;
                    setOffsets = setOffsets || currentOffsetXPos < dimensions.TileWidth ||
                                 currentOffsetXPos +  dimensions.TileWidth * 2 > ViewWidth;
                }
            }
            else
            {
                var currentOffsetXPos = tileLeft - _offsets.X;
                setOffsets = setOffsets || currentOffsetXPos < dimensions.TileWidth ||
                             currentOffsetXPos + dimensions.TileWidth * 2 > ViewWidth;
            }
        }

        if (force || setOffsets)
        {
            _offsets = new Vector2(offsetX, offsetY);
        }

        return setOffsets;
    }

    public Tile Location { get; }

    public ViewElement[] Elements { get; set; }
    public IEnumerable<ViewElement> CurrentAnimations => _animations[currentIndex];
    public int ViewHeight { get; }
    public int ViewWidth { get; set; }

    public bool Finished()
    {
        return currentIndex == _animations.Count - 1;
    }

    public void Reset()
    {
        currentIndex = 0;
    }

    public void Next()
    {
        currentIndex++;
    }

    public void Preserve()
    {
        _preserve = true;
    }

    public void Dispose()
    {
        if (!_preserve)
        {
            Raylib.UnloadTexture(BaseImage);
            foreach (var city in Cities)
            {
                Raylib.UnloadTexture(city.Image);
            }

            foreach (var element in Elements)
            {
                Raylib.UnloadTexture(element.Image);
            }
        }

        foreach (var animation in _animations)
        {
            foreach (var element in animation)
            {
                Raylib.UnloadTexture(element.Image);
            }
        }
    }

    protected void SetAnimation(IList<ViewElement> frameSet)
    {
        _animations.Add(frameSet);
    }
}