using Civ2engine;
using Civ2engine.Enums;
using Civ2engine.Events;
using Raylib_cs;
using RaylibUI.BasicTypes;
using RaylibUI.Controls;
using System.Numerics;

namespace RaylibUI.RunGame.GameControls;

public class MinimapPanel : BaseControl
{
    private readonly Game _game;
    private readonly GameScreen _gameScreen; 
    private Texture2D? _backgroundImage;
    private const int PaddingSide = 11;
    private const int Top = 38;
    private const int PaddingBtm = 11;
    private HeaderLabel _headerLabel;

    public MinimapPanel(GameScreen controller, Game game) : base(controller)
    {
        _gameScreen = controller;
        _game = game;

        _headerLabel = new HeaderLabel(controller, "World");

        controller.OnMapEvent += MapEventTriggered;
        Click += OnClick;
    }
    
    public override void OnResize()
    {
        _backgroundImage = ImageUtils.PaintDialogBase(Width, Height, Top, PaddingBtm, PaddingSide);
        
        _headerLabel.Bounds = new Rectangle((int)Location.X, (int)Location.Y, Width, Top);
        _headerLabel.OnResize();
        
        _offset = new[] { ( Width - 2 * _game.CurrentMap.XDim) / 2, 
            Top + ( Height - Top - PaddingBtm - _game.CurrentMap.YDim) / 2 };
        base.OnResize();
    }

    public void OnClick(object? sender, MouseEventArgs mouseEventArgs)
    {
        _gameScreen.Focused = this;
        var clickPosition = GetRelativeMousePosition();
        if (clickPosition.X < _offset[0] || clickPosition.X > Width - _offset[0] 
            || clickPosition.Y < _offset[1] || clickPosition.Y > Height - _offset[1])
        {
            return;
        }

        var clickedTilePosition = clickPosition - new Vector2(_offset[0],_offset[1]) + new Vector2(GetCenterShift(), 0);
        clickedTilePosition.X = WrapNumber((int)clickedTilePosition.X, 2 * _game.CurrentMap.XDim);
        _gameScreen.Game.ActiveTile =
            _gameScreen.Game.CurrentMap.Tile[(int)clickedTilePosition.X / 2, (int)clickedTilePosition.Y];
        _gameScreen.TriggerMapEvent(new MapEventArgs(MapEventType.MinimapViewChanged,
                new[] { (int)clickedTilePosition.X / 2, (int)clickedTilePosition.Y }));
    }

    private void MapEventTriggered(object sender, MapEventArgs e)
    {
        switch (e.EventType)
        {
            case MapEventType.MapViewChanged:
                {
                    _mapStartXy = e.MapStartXY;
                    _mapDrawSq = e.MapDrawSq;
                    break;
                }
            default: break;
        }
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
        Raylib.DrawRectangle((int)Location.X + PaddingSide, (int)Location.Y + Top, Width - 2 * PaddingSide, Height - Top - PaddingBtm, Color.BLACK);
        var map = _game.CurrentMap;
        // Draw map
        for (var row = 0; row < map.YDim; row++)
        {
            for (var col = 0; col < map.XDim; col++)
            {
                var tileX = WrapNumber(2 * col + GetCenterShift(), 2 * map.XDim) / 2;

                var tile = map.Tile[tileX, row];
                if (!map.MapRevealed && !tile.IsVisible(map.WhichCivsMapShown)) continue;

                var drawColor = tile.CityHere is not null
                    ? Controller.MainWindow.ActiveInterface.PlayerColours[tile.CityHere.Owner.Id].TextColour
                    : tile.Type == TerrainType.Ocean
                        ? OceanColor
                        : LandColor;

                Raylib.DrawRectangle((int)Location.X + _offset[0] + 2 * col + (row % 2),
                    (int)Location.Y + _offset[1] + row, 2, 1, drawColor);
            }
        }

        // Draw current view rectangle
        Raylib.DrawRectangleLines((int)Location.X + _offset[0] + _mapStartXy[0] - GetCenterShift(),
            (int)Location.Y + _offset[1] + _mapStartXy[1],
            _mapDrawSq[0], _mapDrawSq[1], Color.WHITE);

        _headerLabel.Draw(pulse);
        
        base.Draw(pulse);
    }

    private int GetCenterShift()
    {
        return _game.CurrentMap.Flat ? 0 : _mapStartXy[0] + _mapDrawSq[0] / 2 - _game.CurrentMap.XDim;
    }

    private int WrapNumber(int number, int range)
    {
        return (number % range + range) % range;
    }
}