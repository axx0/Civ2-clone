using Civ2engine;
using Civ2engine.Enums;
using Civ2engine.Events;
using RaylibUI.BasicTypes;
using RaylibUI.Controls;
using System.Numerics;
using RaylibUI.BasicTypes.Controls;
using Model;
using Model.Core;
using Raylib_CSharp.Textures;
using Raylib_CSharp.Transformations;
using Raylib_CSharp.Colors;
using Raylib_CSharp.Rendering;
using Civ2engine.MapObjects;
using ExtensionMethods;
using Raylib_CSharp.Collision;
using Raylib_CSharp.Interact;

namespace RaylibUI.RunGame.GameControls;

public class MinimapPanel : BaseControl
{
    private readonly IGame _game;
    private readonly GameScreen _gameScreen;
    private readonly IUserInterface _active;
    private readonly HeaderLabel _headerLabel;
    private Texture2D? _backgroundImage;
    private Rectangle _drawArea = new();
    private Map _map;
    private Button _nextMapButton, _prevMapButton, _closeMapButton;
    private const int _globeRotation_dt = 1000; // globe roration animation step in miliseconds
    private DateTime _rotationT;
    private int _rotationShift = 0;

    public MinimapPanel(GameScreen controller, IGame game) : base(controller)
    {
        _gameScreen = controller;
        _game = game;
        _active = controller.MainWindow.ActiveInterface;
        _map = _gameScreen.CurrentMap;

        _headerLabel = new HeaderLabel(controller, _active.Look, Labels.For(LabelIndex.World), fontSize: _active.Look.HeaderLabelFontSizeNormal);
        _closeMapButton = new Button(Controller, String.Empty, backgroundImage: _active.PicSources["close"][0]);
        _nextMapButton = new Button(Controller, String.Empty, backgroundImage: _active.PicSources["zoomIn"][0]);
        _prevMapButton = new Button(Controller, String.Empty, backgroundImage: _active.PicSources["zoomOut"][0]);

        controller.OnMapEvent += MapEventTriggered;
        Click += OnClick;
    }
    
    public override void OnResize()
    {
        var padding = _active.GetPadding(_gameScreen.MinimapGlobe ? 0 : _headerLabel.TextSize.Y, false);
        _rotationT = DateTime.Now;
        _rotationShift = 0;

        _backgroundImage = ImageUtils.PaintDialogBase(_active, Width, Height, padding);
        
        _headerLabel.Bounds = new Rectangle((int)Location.X, (int)Location.Y, Width, padding.Top);
        _headerLabel.OnResize();
        _closeMapButton.Bounds = new Rectangle((int)Location.X + 14, (int)Location.Y + 6, _closeMapButton.GetPreferredWidth(), _closeMapButton.GetPreferredHeight());
        _prevMapButton.Bounds = new Rectangle((int)Location.X + 30, (int)Location.Y + 6, _prevMapButton.GetPreferredWidth(), _prevMapButton.GetPreferredHeight());
        _nextMapButton.Bounds = new Rectangle((int)Location.X + 46, (int)Location.Y + 6, _nextMapButton.GetPreferredWidth(), _nextMapButton.GetPreferredHeight());
        _closeMapButton.OnResize();
        _prevMapButton.OnResize();
        _nextMapButton.OnResize();

        _drawArea = new(Location.X + padding.Left, Location.Y + padding.Top, Width - padding.Left - padding.Right, Height - padding.Top - padding.Bottom);
        _offset = [ ((int)_drawArea.Width - 2 * _map.XDim ) / 2, ((int)_drawArea.Height - _map.YDim ) / 2];

        // Short flat minimap is fixed, center view on long flat minimap until you reach the edges
        if (_map.Flat && (int)_drawArea.Width < 2 * _map.XDim)
        {
            _offset[0] = (int)_drawArea.Width / 2 - (_visibleStartXy[0] + _visibleDim[0] / 2);

            // Stop moving minimap on edges of flat earth
            if (_visibleStartXy[0] + _visibleDim[0] / 2 < (int)_drawArea.Width / 2)
            {
                _offset[0] = 0;
            }
            else if (_visibleStartXy[0] + _visibleDim[0] / 2 > 2 * _map.XDim - (int)_drawArea.Width / 2)
            {
                _offset[0] = -(2 * _map.XDim - (int)_drawArea.Width);
            }
        }
        base.OnResize();
    }

    public void OnClick(object? sender, MouseEventArgs mouseEventArgs)
    {
        if (_gameScreen.MinimapGlobe)
        {
            _gameScreen.RemoveGlobe();
        }
        else
        {
            // Clicked buttons
            if (_gameScreen.ToTPanelLayout && ShapeHelper.CheckCollisionPointRec(Input.GetMousePosition(), _closeMapButton.Bounds))
            {
                _gameScreen.ShowGlobe();
                return;
            }
            if (_game.Maps.Count > 1 && ShapeHelper.CheckCollisionPointRec(Input.GetMousePosition(), _nextMapButton.Bounds))
            {
                _game.ActivePlayer.ActiveTile = 
                    _game.Maps[(_gameScreen.CurrentMap.MapIndex + 1) % _game.Maps.Count].TileC2(_game.ActivePlayer.ActiveTile.X, _game.ActivePlayer.ActiveTile.Y);
                _gameScreen.TriggerMapEvent(new MapEventArgs(MapEventType.MinimapViewChanged));
                _map = _gameScreen.CurrentMap;
            }
            if (_game.Maps.Count > 1 && ShapeHelper.CheckCollisionPointRec(Input.GetMousePosition(), _prevMapButton.Bounds))
            {
                _game.ActivePlayer.ActiveTile =
                    _game.Maps[(_gameScreen.CurrentMap.MapIndex + _game.Maps.Count - 1) % _game.Maps.Count].TileC2(_game.ActivePlayer.ActiveTile.X, _game.ActivePlayer.ActiveTile.Y);
                _gameScreen.TriggerMapEvent(new MapEventArgs(MapEventType.MinimapViewChanged));
                _map = _gameScreen.CurrentMap;
            }

            var padding = _active.GetPadding(_gameScreen.MinimapGlobe ? 0 : _headerLabel.TextSize.Y, false);
            var clickPosition = GetRelativeMousePosition() - new Vector2(padding.Left, padding.Top); // click position relative to drawing area

            // Clicked outside drawing area
            if (clickPosition.X < 0 || clickPosition.X > _drawArea.Width ||
                clickPosition.Y < 0 || clickPosition.Y > _drawArea.Height)
            {
                return;
            }

            // Position relative to map
            var clickedTilePosition = clickPosition - new Vector2(_offset[0], _offset[1]);

            // Go to edges if clicked outside map bounds
            clickedTilePosition.X = Math.Max(0, clickedTilePosition.X);
            clickedTilePosition.X = Math.Min(2 * _map.XDim - 1, clickedTilePosition.X);
            clickedTilePosition.Y = Math.Max(0, clickedTilePosition.Y);
            clickedTilePosition.Y = Math.Min(_map.YDim - 1, clickedTilePosition.Y);

            clickedTilePosition.X = Utils.WrapNumber(_xShift + (int)clickedTilePosition.X, 2 * _map.XDim);

            _game.ActivePlayer.ActiveTile =
                _map.TileC2((int)clickedTilePosition.X, (int)clickedTilePosition.Y);
            _gameScreen.TriggerMapEvent(new MapEventArgs(MapEventType.MinimapViewChanged));
        }

    }

    private void MapEventTriggered(object sender, MapEventArgs e)
    {
        switch (e.EventType)
        {
            case MapEventType.MapViewChanged:
                {
                    _rotationShift = 0;

                    _visibleStartXy = e.MapStartXy;
                    _xShift = e.Xshift;
                    _visibleDim = e.MapDrawSq;

                    // Short flat minimap is fixed, center view on long flat minimap until you reach the edges
                    if (_map.Flat && (int)_drawArea.Width < 2 * _map.XDim)
                    {
                        _offset[0] = (int)_drawArea.Width / 2 - (_visibleStartXy[0] + _visibleDim[0] / 2);

                        // Stop moving minimap on edges of flat earth
                        if (_visibleStartXy[0] + _visibleDim[0] / 2 < (int)_drawArea.Width / 2)
                        {
                            _offset[0] = 0;
                        }
                        else if (_visibleStartXy[0] + _visibleDim[0] / 2 > 2 * _map.XDim - (int)_drawArea.Width / 2)
                        {
                            _offset[0] = - (2 * _map.XDim - (int)_drawArea.Width);
                        }
                    }

                    break;
                }
            default: break;
        }
    }
    
    private static readonly Color OceanColor = new (0, 0, 95,255);
    private static readonly Color LandColor = new (55, 123, 23,255);

    private int[] _offset = [0, 0];  // Offset of map on drawing area
    private int _xShift = 0;    // Map shift
    private int[] _visibleStartXy = [0, 0]; // Starting coords of visible area
    private int[] _visibleDim = [0, 0]; // Dimensions of visible area

    public override void Draw(bool pulse)
    {
        Graphics.DrawTexture(_backgroundImage.Value,(int)Location.X, (int)Location.Y, Color.White);
        Graphics.DrawRectangleRec(_drawArea, Color.Black);

        if (_gameScreen.MinimapGlobe)
        {
            if ((DateTime.Now - _rotationT).TotalMilliseconds > _globeRotation_dt)
            {
                _rotationT = DateTime.Now;
                _rotationShift++;
            }

            var radius = Math.Min(_drawArea.Width, _drawArea.Height) / 2;
            var center = new Point(Width / 2, Height / 2);
            for (int y = 0; y < 2 * radius; y++)
            {
                var xleft = center.X - Math.Sqrt(radius * radius - (y - radius) * (y - radius));
                var xright = center.X + Math.Sqrt(radius * radius - (y - radius) * (y - radius));
                
                for (int x = 0; x < (xright - xleft); x++)
                {
                    double yf = _map.YDim / (2 * radius);
                    double xf = (double)(_map.XDim / (xright - xleft));
                    var xcoord = x * xf + _game.ActivePlayer.ActiveTile.X - 2 * _rotationShift - _map.XDim / 2;
                    var ycoord = y * yf;

                    Color color = _map.TileC2((int)xcoord, (int)ycoord).Terrain.Type == TerrainType.Ocean ? Color.Blue : Color.DarkGreen;

                    Graphics.DrawPixel((int)Location.X + (int)xleft + x, (int)Location.Y + y + (int)(center.Y - radius), color);
                }
            }
        }
        else
        {
            // Draw map
            for (var row = 0; row < _map.YDim; row++)
            {
                for (var col = 0; col < _map.XDim; col++)
                {
                    var tileX = Utils.WrapNumber(2 * col + _xShift, 2 * _map.XDim) / 2;

                    var tile = _map.Tile[tileX, row];
                    if (!_map.MapRevealed && !tile.IsVisible(_gameScreen.VisibleCivId)) continue;

                    // Don't draw beyond draw area (long maps)
                    if (_offset[0] + 2 * col + (row % 2) < 0 || _offset[0] + 2 * col + (row % 2) >= (int)_drawArea.Width) continue;

                    var drawColor = tile.CityHere is not null
                        ? _active.PlayerColours[tile.CityHere.Owner.Id].TextColour
                        : tile.Type == TerrainType.Ocean
                            ? OceanColor
                            : LandColor;

                    Graphics.DrawRectangle((int)_drawArea.X + _offset[0] + 2 * col + (row % 2),
                        (int)_drawArea.Y + _offset[1] + row, 2, 1, drawColor);
                }
            }

            // Draw current view rectangle
            Graphics.DrawRectangleLines((int)_drawArea.X + Math.Max(0, _offset[0] + Utils.WrapNumber(_visibleStartXy[0] - _xShift, 2 * _map.XDim)),
                (int)_drawArea.Y + Math.Max(0, _offset[1] + _visibleStartXy[1]),
                Math.Min(_visibleDim[0], 2 * _map.XDim), Math.Min(_visibleDim[1], (int)_drawArea.Height), Color.White);
        }

        if (!_gameScreen.MinimapGlobe)
        {
            _headerLabel.Draw(pulse);

            if (_gameScreen.ToTPanelLayout)
            {
                _closeMapButton.Draw(pulse);
            }

            if (_game.Maps.Count > 1)
            {
                _prevMapButton.Draw(pulse);
                _nextMapButton.Draw(pulse);
            }
        }

        base.Draw(pulse);
    }
}