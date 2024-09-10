using Civ2engine;
using Civ2engine.Enums;
using Civ2engine.Events;
using Raylib_cs;
using RaylibUI.BasicTypes;
using RaylibUI.Controls;
using System.Numerics;
using RaylibUI.BasicTypes.Controls;
using Model;
using Model.Core;

namespace RaylibUI.RunGame.GameControls;

public class MinimapPanel : BaseControl
{
    private readonly IGame _game;
    private readonly GameScreen _gameScreen;
    private readonly IUserInterface _active;
    private readonly HeaderLabel _headerLabel;
    private Texture2D? _backgroundImage;
    private Padding _padding;

    public MinimapPanel(GameScreen controller, IGame game) : base(controller)
    {
        _gameScreen = controller;
        _game = game;
        _active = controller.MainWindow.ActiveInterface;

        _headerLabel = new HeaderLabel(controller, _active.Look, Labels.For(LabelIndex.World), fontSize: _active.Look.HeaderLabelFontSizeNormal);
        _padding = controller.Main.ActiveInterface.GetPadding(_headerLabel?.TextSize.Y ?? 0, false);

        controller.OnMapEvent += MapEventTriggered;
        Click += OnClick;
    }
    
    public override void OnResize()
    {
        _backgroundImage = ImageUtils.PaintDialogBase(_active, Width, Height, _padding);
        
        _headerLabel.Bounds = new Rectangle((int)Location.X, (int)Location.Y, Width, _padding.Top);
        _headerLabel.OnResize();
        
        _offset = new[] { ( Width - 2 * _game.CurrentMap.XDim) / 2,
            _padding.Top + ( Height - _padding.Top - _padding.Bottom - _game.CurrentMap.YDim) / 2 };
        base.OnResize();
    }

    public void OnClick(object? sender, MouseEventArgs mouseEventArgs)
    {
        var clickPosition = GetRelativeMousePosition();
        if (clickPosition.X < _offset[0] || clickPosition.X > Width + _offset[0] 
            || clickPosition.Y < _offset[1] || clickPosition.Y > Height + _offset[1])
        {
            return;
        }

        var clickedTilePosition = clickPosition - new Vector2(_offset[0],_offset[1]) + new Vector2(GetCenterShift(), 0);
        clickedTilePosition.X = WrapNumber((int)clickedTilePosition.X, 2 * _game.CurrentMap.XDim);
        if (clickPosition.Y > _gameScreen.Game.CurrentMap.YDim)
        {
            clickPosition.Y = _gameScreen.Game.CurrentMap.YDim-1;
        }
        _gameScreen.Game.ActivePlayer.ActiveTile =
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
                    _mapStartXy = e.MapStartXy;
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
        Raylib.DrawTexture(_backgroundImage.Value,(int)Location.X, (int)Location.Y, Color.White);
        Raylib.DrawRectangle((int)Location.X + _padding.Left, (int)Location.Y + _padding.Top, Width - _padding.Left - _padding.Right, Height - _padding.Top - _padding.Bottom, Color.Black);
        var map = _game.CurrentMap;
        // Draw map
        for (var row = 0; row < map.YDim; row++)
        {
            for (var col = 0; col < map.XDim; col++)
            {
                var tileX = WrapNumber(2 * col + GetCenterShift(), 2 * map.XDim) / 2;

                var tile = map.Tile[tileX, row];
                if (!map.MapRevealed && !tile.IsVisible(_game.GetActiveCiv.Id)) continue;

                var drawColor = tile.CityHere is not null
                    ? _active.PlayerColours[tile.CityHere.Owner.Id].TextColour
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
            _mapDrawSq[0], _mapDrawSq[1], Color.White);

        _headerLabel.Draw(pulse);
        
        base.Draw(pulse);
    }

    private int GetCenterShift() => _game.CurrentMap.Flat ? 0 : _mapStartXy[0] + _mapDrawSq[0] / 2 - _game.CurrentMap.XDim;

    private static int WrapNumber(int number, int range) => (number % range + range) % range;
}