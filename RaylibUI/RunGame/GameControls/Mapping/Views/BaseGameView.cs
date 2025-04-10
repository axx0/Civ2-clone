using System.Diagnostics;
using System.Numerics;
using System.Xml.Serialization;
using Civ2engine;
using Civ2engine.Enums;
using Civ2engine.Events;
using Civ2engine.MapObjects;
using Model;
using Model.ImageSets;
using Model.Interface;
using Raylib_CSharp.Colors;
using Raylib_CSharp.Fonts;
using Raylib_CSharp.Images;
using Raylib_CSharp.Rendering;
using Raylib_CSharp.Textures;
using Raylib_CSharp.Transformations;
using RaylibUI.RunGame.GameControls.Mapping.Views.ViewElements;
using RaylibUtils;
using ExtensionMethods;

namespace RaylibUI.RunGame.GameControls.Mapping.Views;

public abstract class BaseGameView : IGameView
{
    private readonly GameScreen _gameScreen;
    private readonly IList<Tile> _actionTiles;
    private int _currentIndex;

    private IList<IList<IViewElement>> _animations = new List<IList<IViewElement>>();
    private bool _preserve;
    
    protected readonly Vector2 ActivePos;
    protected MapDimensions Dimensions;

    /// <summary>
    /// Offset of view area from map start (in px)
    /// <0 ... view area shows whole map
    /// >0 ... a fraction of the map shown
    /// </summary>
    private Vector2 _offsets = Vector2.Zero;
    public Vector2 Offsets => _offsets;

    /// <summary>
    /// Shift of map start in x-direction (in tiles, =0 for flat)
    /// </summary>
    private int _xShift;
    public int Xshift => _xShift;

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

        var map = location.Map;
        Dimensions = _gameScreen.TileCache.GetDimensions(map, gameScreen.Zoom);
        
        var activeInterface = _gameScreen.Main.ActiveInterface;
        
        var cities = activeInterface.CityImages;
        var civilizationId = _gameScreen.VisibleCivId;
        // Force redraw should be checked last as IsSameArea will set offsets 
        if (previousView != null && IsInSameArea(previousView, location, Dimensions, forceRedraw) && !forceRedraw)
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
                CalculateElementsAtTile(gameScreen, tile, newElements, activeInterface,cities,pos,tileDetails, civilizationId);
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

            var image = Image.GenColor(ViewWidth, ViewHeight, Color.Black);
            var dim = _gameScreen.TileCache.GetDimensions(map, gameScreen.Zoom);
            var ypos = -_offsets.Y;

            for (var row = 0; row < map.YDim; row++)
            {
                if (ypos >= -dim.TileHeight)
                {
                    var xpos = -_offsets.X + (row % 2 * dim.HalfWidth);
                    if (!map.Flat && xpos + dim.TotalWidth < ViewWidth)
                    {
                        if (ViewWidth >= dim.TotalWidth)
                        {
                            xpos = -_offsets.X + (row % 2 * dim.HalfWidth);
                        }
                        else
                        {
                            xpos += dim.TotalWidth;
                        }
                    }

                    for (var col = _xShift / 2; col < _xShift / 2 + map.XDim; col++)
                    {
                        if (xpos >= -dim.TileWidth)
                        {
                            if (xpos >= ViewWidth)
                            {
                                if (!map.Flat)
                                {
                                    xpos -= dim.TotalWidth;
                                }
                                else
                                {
                                    break;
                                }
                            }

                            var tile = map.Tile[col % map.XDim, row];
                            if (tile == location)
                            {
                                ActivePos = new Vector2(xpos, ypos);
                            }

                            if (tile.IsVisible(civilizationId) || map.MapRevealed)
                            {
                                var tileDetails = _gameScreen.TileCache.GetTileDetails(tile, civilizationId);
                                if (gameScreen.Zoom == 0)
                                {
                                    image.Draw(tileDetails.Image, MapImage.TileRec,
                                        new Rectangle(xpos, ypos, dim.TileWidth, dim.TileHeight),
                                        Color.White);
                                }
                                else
                                {
                                    var resizedImg = tileDetails.Image.Copy();
                                    if (Settings.TextureFilter == 0)
                                    {
                                        resizedImg.ResizeNN(resizedImg.Width.ZoomScale(gameScreen.Zoom),
                                            resizedImg.Height.ZoomScale(gameScreen.Zoom));
                                    }
                                    else
                                    {
                                        resizedImg.Resize(resizedImg.Width.ZoomScale(gameScreen.Zoom),
                                            resizedImg.Height.ZoomScale(gameScreen.Zoom));
                                    }
                                    image.Draw(resizedImg, MapImage.TileRec.ZoomScale(gameScreen.Zoom),
                                        new Rectangle(xpos, ypos, dim.TileWidth, dim.TileHeight),
                                        Color.White);
                                }

                                if (_gameScreen.ShowGrid)
                                {
                                    image.Draw(Images.ExtractBitmap(activeInterface.PicSources["gridlines"][0]), MapImage.TileRec.ZoomScale(gameScreen.Zoom), new Rectangle(xpos, ypos, dim.TileWidth, dim.TileHeight),
                                        Color.White);

                                    // Show coords for debugging
                                    var text = $"({2 * col % (2 * map.XDim) + row % 2},{row})";
                                    var meas = TextManager.MeasureTextEx(Font.GetDefault(), text, 10, 0f);
                                    image.DrawText(text, (int)xpos + dim.TileWidth / 2 - (int)meas.X / 2, (int)ypos + dim.TileHeight / 2 - (int)meas.Y / 2, 10, Color.Yellow);
                                }

                                var posVector = new Vector2(xpos, ypos);
                                CalculateElementsAtTile(gameScreen, tile, elements, activeInterface, cities, posVector, tileDetails, civilizationId);
                            }
                        }

                        xpos += dim.TileWidth;
                    }
                }

                ypos += dim.HalfHeight;
                if (ypos > ViewHeight)
                {
                    break;
                }
            }

            this.BaseImage = Texture2D.LoadFromImage(image);
            this.Elements = elements.ToArray();

            image.Unload();

            gameScreen.TriggerMapEvent(new MapEventArgs(MapEventType.MapViewChanged)
            {
                MapStartXy = [Utils.WrapNumber(Math.Max(0, (int)_offsets.X / dim.HalfWidth) + _xShift, 2 * map.XDim), 
                    (int)_offsets.Y / dim.HalfHeight],
                MapDrawSq = [ViewWidth / dim.HalfWidth, ViewHeight / dim.HalfHeight],
                Xshift = _xShift
            });
        }
    }


    private void CalculateElementsAtTile(GameScreen gameScreen, Tile tile, List<IViewElement> elements,
        IUserInterface activeInterface,
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
            if (tile.CityHere.Owner.Epoch == (int)Civ2engine.Enums.EpochType.Industrial)
            {
                cityStyleIndex = 4;
            }
            else if (tile.CityHere.Owner.Epoch == (int)Civ2engine.Enums.EpochType.Modern)
            {
                cityStyleIndex = 5;
            }

            var sizeIncrement =
                _gameScreen.Main.ActiveInterface.GetCityIndexForStyle(cityStyleIndex,
                    tile.CityHere, playerKnowledge.CityHere.Size);
            
            var cityImage = cities.Sets[cityStyleIndex][sizeIncrement];
            var cityPos = posVector with{ Y = posVector.Y + Dimensions.TileHeight - TextureCache.GetImage(cityImage.Image).Height.ZoomScale(gameScreen.Zoom) };
            elements.Add(new CityData(
                color: activeInterface.PlayerColours[playerKnowledge.CityHere.OwnerId],
                name: playerKnowledge.CityHere.Name,
                size: playerKnowledge.CityHere.Size,
                sizeRectLoc: cityImage.SizeLoc,
                texture: TextureCache.GetImage(cityImage.Image),
                location: cityPos, tile: tile));
            if (tile.UnitsHere.Count > 0)
            {
                var flagTexture = TextureCache.GetImage(activeInterface.PlayerColours[playerKnowledge.CityHere.OwnerId].Image);
                var flagOffset = cityImage.FlagLoc - new Vector2(0, flagTexture.Height - 5);
                elements.Add(new TextureElement(texture: flagTexture,
                    tile: tile, location: cityPos, offset: flagOffset)
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
                        texture: impImage, location: posVector with { Y = posVector.Y + Dimensions.TileHeight - impImage.Height.ZoomScale(gameScreen.Zoom) }, tile: tile, isTerrain: true));
                }
                else
                {
                    var impImage = ImageUtils.GetImpImage(activeInterface, unitImp.Image,
                        tile.Owner);
                    elements.Add(new TextureElement(
                        texture: impImage,
                        location: posVector with { Y = posVector.Y + Dimensions.TileHeight - impImage.Height.ZoomScale(gameScreen.Zoom) },
                        tile: tile, isTerrain: true));

                    
                    ImageUtils.GetUnitTextures(unit, activeInterface, gameScreen.Game, elements,
                        posVector with
                        {
                            Y = posVector.Y + Dimensions.TileHeight -
                                activeInterface.UnitImages.UnitRectangle.Height.ZoomScale(gameScreen.Zoom)
                        });
                }
            }
            else
            {
                ImageUtils.GetUnitTextures(unit, activeInterface, gameScreen.Game, elements,
                    posVector with
                    {
                        Y = posVector.Y + Dimensions.TileHeight -
                            activeInterface.UnitImages.UnitRectangle.Height.ZoomScale(gameScreen.Zoom)
                    });
                if (tileDetails.ForegroundElement != null)
                {
                    var impImage = ImageUtils.GetImpImage(activeInterface,
                        tileDetails.ForegroundElement.Image, tile.Owner);
                    elements.Add(new TextureElement(
                        texture: impImage,
                        location: posVector with{ Y = posVector.Y + Dimensions.TileHeight - impImage.Height.ZoomScale(gameScreen.Zoom)
                        }, tile: tile, isTerrain: true));
                }
            }
        }
        else if (tileDetails.ForegroundElement != null)
        {
            var impImage = ImageUtils.GetImpImage(activeInterface,
                tileDetails.ForegroundElement.Image, tile.Owner);
            elements.Add(new TextureElement(
                texture: impImage,
                location: posVector with { Y = posVector.Y + Dimensions.TileHeight - impImage.Height.ZoomScale(gameScreen.Zoom) },
                tile: tile, isTerrain: true));
        }
    }

    protected Vector2 GetPosForTile(Tile tile)
    {
        return new Vector2(Utils.WrapNumber(tile.XIndex - _xShift / 2, Location.Map.XDim) * Dimensions.TileWidth + tile.Odd * Dimensions.HalfWidth,
                   tile.Y * Dimensions.HalfHeight) - _offsets;
    }

    public Texture2D BaseImage { get; set; }

    private bool IsInSameArea(IGameView previousView, Tile location, MapDimensions dimensions, bool force = false)
    {
        if (previousView.Location.Map != location.Map) return false;
        if (previousView.ViewHeight != ViewHeight || previousView.ViewWidth != ViewWidth) return false;

        return !CalculateOffsets(previousView, location, dimensions, force);
    }

    private bool CalculateOffsets(IGameView? previousView, Tile location, MapDimensions dimensions, bool force = false)
    {
        if (previousView != null)
        {
            _offsets = previousView.Offsets;
            _xShift = previousView.Xshift;
        }
        bool setOffsetX, setOffsetY;
        int offsetX, offsetY, xShift;

        // Whole map in y-dir shown
        if (ViewHeight >= dimensions.TotalHeight)
        {
            offsetY = (dimensions.TotalHeight - ViewHeight) /2;
            setOffsetY = offsetY != (int)_offsets.Y;
        }
        // Part of map in y-dir shown
        else
        {
            // Get candidates for new offsets
            var tileTop = location.Y * dimensions.HalfHeight;    // map start to active tile
            offsetY = tileTop + dimensions.HalfHeight - ViewHeight / 2;  // center active tile on screen
            offsetY = (int)(Math.Round((double)offsetY / dimensions.HalfHeight, 0) * dimensions.HalfHeight);  // round offset to a half tile value

            var currentOffsetYPos = tileTop - _offsets.Y;   // Distance between active tile and start of view area in y-dir

            // Set offset if active tile is beyond view area
            setOffsetY = currentOffsetYPos < 0 || currentOffsetYPos + dimensions.TileHeight > ViewHeight;

            // Limit view to edges of map (if active tile is beyond view or when starting game or when moving with mouse)
            if (offsetY < 0)
            {
                offsetY = 0;
                setOffsetY = offsetY != (int)_offsets.Y;
            }
            else if (offsetY + ViewHeight > dimensions.TotalHeight)
            {
                offsetY = dimensions.TotalHeight - ViewHeight;
                setOffsetY = offsetY != (int)_offsets.Y;
            }
        }

        xShift = location.Map.Flat ? 0 : Utils.WrapNumber(location.X - 2 * location.Map.XDim / 2, 2 * location.Map.XDim);
        if (previousView == null)
        {
            _xShift = xShift;
        }

        // Whole map in x-dir shown
        if (ViewWidth >= dimensions.TotalWidth)
        {
            offsetX = (dimensions.TotalWidth - ViewWidth) /2;
            setOffsetX = offsetX != (int)_offsets.X;
        }
        // Part of map in x-dir shown
        else
        {
            // Get candidates for new offsets
            var tileLeft = Utils.WrapNumber(location.X - _xShift, 2 * location.Map.XDim) * dimensions.HalfWidth;    // map start to active tile
            offsetX = tileLeft + dimensions.HalfWidth - ViewWidth / 2;  // center active tile on screen
            offsetX = (int)(Math.Round((double)offsetX / dimensions.HalfWidth, 0) * dimensions.HalfWidth);  // round offset to a half tile value

            var currentOffsetXPos = tileLeft - _offsets.X;  // Distance between active tile and start of view area in x-dir
            setOffsetX = currentOffsetXPos < 0 || currentOffsetXPos + dimensions.TileWidth > ViewWidth;   // Active tile is beyond view area

            if (location.Map.Flat)
            {
                if (offsetX < 0)
                {
                    offsetX = 0;
                    setOffsetX = offsetX != (int)_offsets.X;
                }
                else if (offsetX + ViewWidth > dimensions.TotalWidth)
                {
                    offsetX = dimensions.TotalWidth - ViewWidth;
                    setOffsetX = offsetX != (int)_offsets.X;
                }
            }
            else
            {
                if (setOffsetY || setOffsetX || force)
                {
                    // Recalculate offset with new xShift
                    tileLeft = Utils.WrapNumber(location.X - xShift, 2 * location.Map.XDim) * dimensions.HalfWidth;    // map start to active tile
                    offsetX = tileLeft + dimensions.HalfWidth - ViewWidth / 2;  // center active tile on screen
                    offsetX = (int)(Math.Round((double)offsetX / dimensions.HalfWidth, 0) * dimensions.HalfWidth);  // round offset to a half tile value
                    setOffsetX = true;
                }
            }
        }

        if (force || setOffsetX || setOffsetY)
        {
            _offsets.X = offsetX;
            _offsets.Y = offsetY;
            _xShift = xShift;
        }

        return setOffsetX || setOffsetY;
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
            BaseImage.Unload();
        }
    }

    protected void SetAnimation(IList<IViewElement> frameSet)
    {
        _animations.Add(frameSet);
    }
}