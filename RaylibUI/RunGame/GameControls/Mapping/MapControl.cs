using System.Diagnostics;
using System.Numerics;
using Civ2engine;
using Civ2engine.MapObjects;
using Raylib_cs;
using RaylibUI.BasicTypes;

namespace RaylibUI.RunGame.GameControls.Mapping;

public class MapControl : BaseControl
{
    public override bool CanFocus => true;
    private readonly GameScreen _gameScreen;
    private readonly Game _game;
    private Texture2D? _backgroundImage;
    private readonly Image[,] _mapTileTexture;
    private readonly int _tileWidth;
    private readonly int _tileHeight;
    private Vector2 _offsets;
    private readonly Map _map;
    private readonly int _halfHeight;
    private Texture2D _mapImage;
    private Tile _selectedTile;
    private int _viewWidth;
    private int _viewHeight;
    private readonly int _currentMapShown;
    private Vector2 _activePosition = new (0,0);
    private Texture2D? _activeImage;
    private readonly int _halfWidth;
    private readonly int _diagonalCut;
    private readonly int _totalWidth;
    private readonly int _totalHeight;
    private const int PaddingSide = 11;
    private const int Top = 11;
    private const int PaddingBtm = 11;

    public MapControl(GameScreen gameScreen, Game game) : base(gameScreen)
    {
        _gameScreen = gameScreen;
        _game = game;
        _map = game.CurrentMap;
        var map = _map;

        _mapTileTexture = new Image[map.XDim, map.YDim];
        for (var col = 0; col < map.XDim; col++)
        {
            for (var row = 0; row < map.YDim; row++)
            {
                _mapTileTexture[col, row] = MapImage.MakeTileGraphic(map.Tile[col, row], map, MapImages.Terrains[map.MapIndex], game);
            }
        }

        _currentMapShown = _game.GetPlayerCiv.Id;

        _tileWidth = _mapTileTexture[0, 0].width;
        _tileHeight = _mapTileTexture[0, 0].height;
        _halfHeight = _tileHeight / 2;
        _halfWidth = _tileWidth / 2;
        _diagonalCut = _halfHeight * _halfWidth;
        _selectedTile = game.ActiveTile;
        _totalWidth = _map.Tile.GetLength(0) * _tileWidth;
        _totalHeight = _map.Tile.GetLength(1) * _halfHeight + _halfHeight;
    }


    public override Size GetPreferredSize(int width, int height)
    {
        return new Size(width - RunGame.GameScreen.MiniMapWidth, height);
    }

    public override void OnResize()
    {
        base.OnResize();
        _backgroundImage = ImageUtils.PaintDialogBase(Width, Height, Top, PaddingBtm, PaddingSide);

        _viewWidth = Width - 2 * PaddingSide;
        _viewHeight = Height - Top - PaddingBtm;
        ShowTile(_selectedTile);
    }

    private void ShowTile(Tile tile)
    {
        bool setOffsets;
        int offsetY;
        int offsetX;
        if (_viewHeight >= _totalHeight)
        {
            offsetY = (_totalHeight - _viewHeight) /2;
            setOffsets = offsetY != (int)_offsets.Y;
        }
        else
        {
            var tileTop = tile.Y * _halfHeight;
            var currentOffsetYPos = tileTop - _offsets.Y;
            offsetY = tileTop - (_viewHeight / 2);
            if (offsetY < 0)
            {
                offsetY = 0; setOffsets = offsetY != (int)_offsets.Y;
            }
            else if(offsetY > (_totalHeight - _viewHeight))
            {
                offsetY = _totalHeight - _viewHeight;
                setOffsets = offsetY != (int)_offsets.Y;
            }
            else
            {
                setOffsets = currentOffsetYPos < _tileHeight || currentOffsetYPos + _tileHeight * 2 > _viewHeight;
            }
        }

        if (_map.Flat && _viewWidth >= _totalWidth)
        {
            offsetX = (_totalWidth - _viewWidth) /2;
            setOffsets = setOffsets || offsetX != (int)_offsets.X;
        }
        else
        {
            var tileLeft = tile.XIndex * _tileWidth + tile.Odd * _halfWidth;
            offsetX = tileLeft - (_viewWidth / 2);
            if (_map.Flat)
            {
                if (offsetX < 0)
                {
                    offsetX = 0;
                    setOffsets = setOffsets || offsetX != (int)_offsets.X;
                }else if (offsetX > (_totalWidth - _viewWidth + _halfWidth))
                {
                    offsetX = _totalWidth - _viewWidth + _halfWidth;
                    setOffsets = setOffsets || offsetX != (int)_offsets.X;
                }
                else
                {
                    var currentOffsetXPos = tileLeft - _offsets.X;
                    setOffsets = setOffsets || currentOffsetXPos < _tileWidth ||
                                 currentOffsetXPos + _tileWidth * 2 > _viewWidth;
                }
            }
            else
            {
                var currentOffsetXPos = tileLeft - _offsets.X;
                setOffsets = setOffsets || currentOffsetXPos < _tileWidth ||
                             currentOffsetXPos + _tileWidth * 2 > _viewWidth;
            }
        }

        if (setOffsets)
        {
            _offsets = new Vector2(offsetX, offsetY);
        }
        Redraw();
    }

    public override void OnClick()
    {
        _gameScreen.Focused = this;
        var clickPosition = GetRelativeMousePosition();
        if (clickPosition.X < PaddingSide || clickPosition.X > _viewWidth + PaddingSide || clickPosition.Y < Top ||
            clickPosition.Y > Top + _viewHeight)
        {
            return;
        }
        var clickedTilePosition = clickPosition - new Vector2(PaddingSide,Top) + _offsets;
        var y =Math.DivRem((int)(clickedTilePosition.Y), _halfHeight, out var yRemainder) ;
        var odd = y % 2 == 1;
        var clickX = (int)(odd ? clickedTilePosition.X - _halfWidth : clickedTilePosition.X);
        if (clickX < 0)
        {
            if (_map.Flat)
            {
                clickX = 0;
            }
            else
            {
                clickX += _totalWidth;
            }
        }else if (clickX > _totalWidth)
        {
            if (_map.Flat)
            {
                clickX = _totalWidth - 1;
            }
            else
            {
                clickX -= _totalWidth;
            }
        }
        var x = Math.DivRem(clickX, _tileWidth, out var xRemainder);
        
        if (xRemainder < _halfWidth && y > 0)
        {
            if (yRemainder * _halfWidth + xRemainder * _halfHeight < _diagonalCut)
            {
                y -= 1;
                if (!odd)
                {
                    x -= 1;
                    if (x < 0)
                    {
                        x = _map.Flat ? 0 : _map.Tile.GetLength(0) -1;
                    }
                }
            }
        }else if (xRemainder > _halfWidth)
        {
            if ((_tileWidth - xRemainder) * _halfHeight + yRemainder * _halfWidth < _diagonalCut)
            {
                y -= 1;
                if (odd)
                {
                    x += 1;
                    if (x == _map.Tile.GetLength(0))
                    {
                        if (_map.Flat)
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

        if (0 <= y && y < _map.Tile.GetLength(1))
        {
            _selectedTile = _map.Tile[x, y];
        }
        
        ShowTile(_selectedTile);
        base.OnClick();
    }

    public override bool OnKeyPressed(KeyboardKey key)
    {
        switch (key)
        {
            case KeyboardKey.KEY_UP when _selectedTile.Y > 1:
                _selectedTile = _map.Tile[_selectedTile.XIndex, _selectedTile.Y - 2];
                ShowTile(_selectedTile);
                break;
            case KeyboardKey.KEY_DOWN when _selectedTile.Y < _map.Tile.GetLength(1):
                _selectedTile = _map.Tile[_selectedTile.XIndex, _selectedTile.Y + 2];
                ShowTile(_selectedTile);
                break;
            case KeyboardKey.KEY_LEFT:
                _selectedTile =
                    _map.Tile[_selectedTile.XIndex == 0 ? _map.Tile.GetLength(0) - 1 : _selectedTile.XIndex - 1,
                        _selectedTile.Y];

                ShowTile(_selectedTile);
                break;
            case KeyboardKey.KEY_RIGHT:
                _selectedTile = _map.Tile[_selectedTile.XIndex >= _map.Tile.GetLength(0) -1 ? 0: _selectedTile.XIndex + 1, _selectedTile.Y];
                
                ShowTile(_selectedTile);
                break;

        }
        return base.OnKeyPressed(key);
    }

    private void Redraw()
    {
        var imageWidth = _viewWidth;
        var imageHeight = _viewHeight;
        var image = ImageUtils.NewImage(imageWidth, imageHeight);

        Raylib.ImageDrawRectangle(ref image, 0, 0, imageWidth, imageHeight, Color.BLACK);

        var ypos = -_offsets.Y;

        var maxWidth = _map.XDim * _tileWidth;

        Vector2? activePos = null;
        var activeUnit = _game.Players[_currentMapShown].ActiveUnit;

        for (var row = 0; row < _map.YDim; row++)
        {
            if (ypos >= -_tileHeight)
            {
                var xpos = (float)(-_offsets.X + (row % 2 * _halfWidth));
                if (!_map.Flat && xpos + maxWidth < _viewWidth)
                {
                    xpos += maxWidth;
                }

                for (var col = 0; col < _map.XDim; col++)
                {
                    if (xpos >= -_tileWidth)
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
                        if (tile == _selectedTile)
                        {
                            activePos = new Vector2(xpos, ypos);
                        }

                        if (tile.Visibility[_currentMapShown])
                        {
                            Raylib.ImageDraw(ref image, _mapTileTexture[col, row], MapImage.TileRec,
                                new Rectangle(xpos, ypos, _tileWidth, _tileHeight), Color.WHITE);

                            if (tile.CityHere != null)
                            {
                                var cityStyleIndex = tile.CityHere.Owner.CityStyle;
                                var sizeIncrement =
                                    _gameScreen.Main.ActiveInterface.GetCityIndexForStyle(cityStyleIndex,
                                        tile.CityHere);

                                var cityIndex = cityStyleIndex * 8 + sizeIncrement;
                                
                                Raylib.ImageDraw(ref image, MapImages.Cities[cityIndex].Bitmap, MapImages.CityRectangle, new Rectangle(xpos, ypos - _halfHeight, MapImages.CityRectangle.width, MapImages.CityRectangle.height), Color.WHITE);

                            }
                            else if (tile.UnitsHere.Count > 0)
                            {
                                var unit = tile.GetTopUnit();
                                if (unit != activeUnit)
                                {
                                    var unitRectangle = new Rectangle(xpos, ypos - _halfHeight, _tileWidth,
                                        _tileHeight + _halfHeight);
                                    Raylib.ImageDraw(ref image, MapImages.Units[(int)unit.Type].Image,
                                        MapImages.UnitRectangle,
                                        unitRectangle, Color.WHITE);
                                }
                            }
                        }
                    }

                    xpos += _tileWidth;
                }
            }

            ypos += _halfHeight;
            if (ypos > imageHeight)
            {
                break;
            }
        }

        if (activePos.HasValue)
        {
            // if (activeUnit != null)
            // {
            //     SetActive(activePos.Value.X + _tileHeight - MapImages.UnitRectangle.height - xCRop, activePos.Value.Y - yCrop,
            //         MapImages.Units[(int)activeUnit.Type].Image);
            // }
            // else
            // {
                //TODO: flashing tile highlight
                SetActive(activePos.Value.X, activePos.Value.Y, MapImages.ViewPiece);
            // }
        }

        _mapImage = Raylib.LoadTextureFromImage(image);
        Raylib.UnloadImage(image);
    }

    private void SetActive(float valueX, float valueY, Image image)
    {
        _activePosition = new Vector2(valueX, valueY);
        _activeImage = Raylib.LoadTextureFromImage(image);
    }

    public override void Draw(bool pulse)
    {
        Raylib.DrawTexture(_backgroundImage.Value, (int)Location.X, (int)Location.Y, Color.WHITE);

        Raylib.DrawTexture(_mapImage, (int)Location.X + PaddingSide, (int)Location.Y + Top, Color.WHITE);
        if (pulse && _activeImage.HasValue)
        {
            Raylib.DrawTexture(_activeImage.Value, (int)(Location.X + PaddingSide + _activePosition.X),
                (int)(Location.Y + Top + _activePosition.Y), Color.WHITE);
        }

        // Raylib.DrawTexture(_mapTileTexture[activeTile.XIndex, activeTile.Y],
        //     (int)Location.X + Width / 2 - _tileWidth / 2, (int)Location.Y + Height / 2 - _tileHeight / 2, Color.WHITE);

    }
}