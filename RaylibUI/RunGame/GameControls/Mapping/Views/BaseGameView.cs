using System.Numerics;
using Civ2engine.MapObjects;
using Model;
using Model.ImageSets;
using Raylib_cs;
using RaylibUI.RunGame.GameControls.Mapping.Views.ViewElements;

namespace RaylibUI.RunGame.GameControls.Mapping.Views;

public abstract class BaseGameView : IGameView
{
    private readonly GameScreen _gameScreen;
    private readonly IList<Tile> _actionTiles;
    private int _currentIndex;

    private IList<IList<IViewElement>> _animations = new List<IList<IViewElement>>();
    private bool _preserve;
    
    private Vector2 _offsets = Vector2.Zero;
    
    protected readonly Vector2 ActivePos;
    protected MapDimensions Dimensions;

    public Vector2 Offsets => _offsets;

    public bool IsDefault { get; }
    public int Interval { get; }
    public IList<Tile> ActionTiles => _actionTiles;

    protected BaseGameView(GameScreen gameScreen, Tile location, IGameView? previousView, int viewHeight, int viewWidth, bool isDefault, int interval, IList<Tile> actionTiles, bool forceRedraw)
    {
        IsDefault = isDefault;
        Interval = interval;
        _gameScreen = gameScreen;
        _actionTiles = actionTiles;

        Location = location;
        ViewWidth = viewWidth;
        ViewHeight = viewHeight;

        Dimensions = _gameScreen.TileCache.GetDimensions(location.Map);
        
        
        var activeInterface = _gameScreen.Main.ActiveInterface;
        
        var cities = activeInterface.CityImages;
        var civilizationId = _gameScreen.Player.Civilization.Id;
        // Force redraw should be checked last as IsSameArea will set offsets 
        if (previousView != null && IsInSameArea(previousView, location, Dimensions) && !forceRedraw)
        {
            ActivePos = GetPosForTile(location);
            BaseImage = previousView.BaseImage;
            
            //Update elements where action just happened if any
            var previousAction = previousView.ActionTiles.Where(t=>!_actionTiles.Contains(t)).ToList();
            var newElements = new List<IViewElement>();
            foreach (var tile in previousAction)
            {
                var pos = GetPosForTile(tile);
                
                var tileDetails = _gameScreen.TileCache.GetTileDetails(tile, civilizationId);
                CalculateElementsAtTile(tile, newElements, activeInterface,cities,pos,tileDetails, civilizationId);
            }
            Elements = previousView.Elements.Where(e=> !previousAction.Contains(e.Tile)).Concat(newElements).ToArray();
            
            previousView.Preserve();
        }
        else
        {
            var elements = new List<IViewElement>();
            if (_offsets == Vector2.Zero)
            {
                CalculateOffsets(null, location, Dimensions, force: true);
            }


            var imageWidth = ViewWidth;
            var imageHeight = ViewHeight;
            var image = ImageUtils.NewImage(imageWidth, imageHeight);
            var map = location.Map;
            var dim = _gameScreen.TileCache.GetDimensions(map);

            Raylib.ImageDrawRectangle(ref image, 0, 0, imageWidth, imageHeight, Color.BLACK);

            var ypos = -_offsets.Y;

            var maxWidth = map.XDim * dim.TileWidth;

            for (var row = 0; row < map.YDim; row++)
            {
                if (ypos >= -dim.TileHeight)
                {
                    var xpos = -_offsets.X + (row % 2 * dim.HalfWidth);
                    if (!map.Flat && xpos + maxWidth < ViewWidth)
                    {
                        xpos += maxWidth;
                    }

                    for (var col = 0; col < map.XDim; col++)
                    {
                        if (xpos >= -dim.TileWidth)
                        {
                            if (xpos >= imageWidth)
                            {
                                if (!map.Flat)
                                {
                                    xpos -= maxWidth;
                                }
                                else
                                {
                                    break;
                                }
                            }

                            var tile = map.Tile[col, row];
                            if (tile == location)
                            {
                                ActivePos = new Vector2(xpos, ypos);
                            }

                            if (tile.IsVisible(civilizationId))
                            {
                                var tileDetails = _gameScreen.TileCache.GetTileDetails(tile, civilizationId);
                                Raylib.ImageDraw(ref image, tileDetails.Image,
                                    MapImage.TileRec,
                                    new Rectangle(xpos, ypos, dim.TileWidth, dim.TileHeight), Color.WHITE);

                                var posVector = new Vector2(xpos, ypos);
                                CalculateElementsAtTile(tile, elements, activeInterface, cities, posVector, tileDetails, civilizationId);
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
            this.Elements = elements.ToArray();

            Raylib.UnloadImage(image);
        }
    }


    private void CalculateElementsAtTile(Tile tile, List<IViewElement> elements, IUserInterface activeInterface,
        CityImageSet cities,
        Vector2 posVector, TileDetails tileDetails, int civilizationId)
    {
        if (tile.PlayerKnowledge == null || tile.PlayerKnowledge.Length < civilizationId ||
            tile.PlayerKnowledge[civilizationId] == null)
        {
            return; //We know nothing of this tile 
        }

        var playerKnowledge = tile.PlayerKnowledge[civilizationId];
        if (playerKnowledge.CityHere != null)
        {
            var cityStyleIndex = _gameScreen.Game.Players[playerKnowledge.CityHere.OwnerId].Civilization.CityStyle;
            if (tile.CityHere.Owner.Epoch == Civ2engine.Enums.EpochType.Industrial)
            {
                cityStyleIndex = 4;
            }
            else if (tile.CityHere.Owner.Epoch == Civ2engine.Enums.EpochType.Modern)
            {
                cityStyleIndex = 5;
            }

            var sizeIncrement =
                _gameScreen.Main.ActiveInterface.GetCityIndexForStyle(cityStyleIndex,
                    tile.CityHere, playerKnowledge.CityHere.Size);
            var cityImage = cities.Sets[cityStyleIndex][sizeIncrement];
            var cityPos = posVector with{ Y = posVector.Y + Dimensions.TileHeight - cityImage.Texture.Height};
            elements.Add(new CityData(
                color: activeInterface.PlayerColours[playerKnowledge.CityHere.OwnerId],
                name: playerKnowledge.CityHere.Name,
                texture: cityImage.Texture,
                location: cityPos, tile: tile));
            if (tile.UnitsHere.Count > 0)
            {
                var flagTexture = activeInterface.PlayerColours[playerKnowledge.CityHere.OwnerId].FlagTexture;
                var flagOffset = cityImage.FlagLoc - new Vector2(0, flagTexture.Height - 5);
                elements.Add(new TextureElement(texture: flagTexture,
                    tile: tile, location: cityPos + flagOffset, offset: flagOffset)
                );
            }
        }
        else if ((tile.Map.MapRevealed || tile.Map.IsCurrentlyVisible(tile, civilizationId)) && tile.UnitsHere.Count > 0)
        {
            var unit = tile.GetTopUnit();

            if (tileDetails.ForegroundElement is UnitHidingImprovement unitImp)
            {
                if (unitImp.UnitDomain == unit.Domain)
                {
                    var impImage = ImageUtils.GetImpImage(activeInterface, unitImp.UnitImage,
                        tile.Owner);
                    elements.Add(new TextureElement(
                        texture: impImage, location: posVector with{ Y = posVector.Y + Dimensions.TileHeight}, tile: tile, isTerrain: true));
                }
                else
                {
                    var impImage = ImageUtils.GetImpImage(activeInterface, unitImp.Image,
                        tile.Owner);
                    elements.Add(new TextureElement(
                        texture: impImage,
                        location: posVector with { Y = posVector.Y + Dimensions.TileHeight - impImage.Height },
                        tile: tile, isTerrain: true));

                    
                    ImageUtils.GetUnitTextures(unit, activeInterface, elements,
                        posVector with
                        {
                            Y = posVector.Y + Dimensions.TileHeight -
                                activeInterface.UnitImages.UnitRectangle.Height
                        });
                }
            }
            else
            {
                ImageUtils.GetUnitTextures(unit, activeInterface, elements,
                    posVector with
                    {
                        Y = posVector.Y + Dimensions.TileHeight -
                            activeInterface.UnitImages.UnitRectangle.Height
                    });
                if (tileDetails.ForegroundElement != null)
                {
                    var impImage = ImageUtils.GetImpImage(activeInterface,
                        tileDetails.ForegroundElement.Image, tile.Owner);
                    elements.Add(new TextureElement(
                        texture: impImage,
                        location: posVector with{ Y = posVector.Y + Dimensions.TileHeight - impImage.Height}, tile: tile, isTerrain: true));
                }
            }
        }
        else if (tileDetails.ForegroundElement != null)
        {
            var impImage = ImageUtils.GetImpImage(activeInterface,
                tileDetails.ForegroundElement.Image, tile.Owner);
            elements.Add(new TextureElement(
                texture: impImage,
                location: posVector with { Y = posVector.Y + Dimensions.TileHeight - impImage.Height },
                tile: tile, isTerrain: true));
        }
    }

    protected Vector2 GetPosForTile(Tile tile)
    {
        return new Vector2(tile.XIndex * Dimensions.TileWidth + tile.Odd * Dimensions.HalfWidth,
                   tile.Y * Dimensions.HalfHeight) -
               Offsets;
    }

    public Texture2D BaseImage { get; set; }

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

    public IViewElement[] Elements { get; }
    public IEnumerable<IViewElement> CurrentAnimations => _animations[_currentIndex];
    public int ViewHeight { get; }
    public int ViewWidth { get; set; }

    public bool Finished()
    {
        return _currentIndex == _animations.Count - 1;
    }

    public void Reset()
    {
        _currentIndex = 0;
    }

    public void Next()
    {
        _currentIndex++;
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
        }
    }

    protected void SetAnimation(IList<IViewElement> frameSet)
    {
        _animations.Add(frameSet);
    }
}