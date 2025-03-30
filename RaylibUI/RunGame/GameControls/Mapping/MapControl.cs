using System.Numerics;
using Civ2engine;
using Civ2engine.Enums;
using Civ2engine.Events;
using Civ2engine.MapObjects;
using Raylib_CSharp.Transformations;
using Raylib_CSharp.Textures;
using RaylibUI.BasicTypes.Controls;
using RaylibUI.RunGame.GameControls.Mapping.Views;
using Model;
using Model.Core;
using Model.Interface;
using Raylib_CSharp.Rendering;
using Raylib_CSharp.Colors;
using Raylib_CSharp.Fonts;
using Raylib_CSharp.Interact;
using RaylibUI.Controls;
using Raylib_CSharp.Collision;

namespace RaylibUI.RunGame.GameControls.Mapping;

public class MapControl : BaseControl
{
    public override bool CanFocus => true;
    private readonly GameScreen _gameScreen;
    private readonly IGame _game;
    private Texture2D? _backgroundImage;
    private int _viewWidth,_viewHeight;
    private Padding _padding;
    private HeaderLabel? _headerLabel;
    private IUserInterface _active;
    private Button _zoomInButton, _zoomOutButton;
    private float _zoomBtnScale;

    private readonly Queue<IGameView> _animationQueue = new();
    private IGameView _currentView;
    
    public MapControl(GameScreen gameScreen, IGame game, Rectangle initialBounds) : base(gameScreen)
    {
        Bounds = initialBounds;
        _currentBounds = initialBounds;
        _gameScreen = gameScreen;
        _game = game;
        _active = gameScreen.MainWindow.ActiveInterface;
        
        _headerLabel = new HeaderLabel(gameScreen, _active.Look, $"{_game.GetPlayerCiv.Adjective} {Labels.For(LabelIndex.Map)}", fontSize: _active.Look.HeaderLabelFontSizeNormal);
        _padding = _active.GetPadding(_headerLabel?.TextSize.Y ?? 0, false);
        _zoomBtnScale = _padding.Top > 30 ? 1.4f : 1.0f;   // MGE=1.4f, ToT=1.0f
        _zoomInButton = new Button(Controller, String.Empty, backgroundImage: _active.PicSources["zoomIn"][0], imageScale: _zoomBtnScale);
        _zoomOutButton = new Button(Controller, String.Empty, backgroundImage: _active.PicSources["zoomOut"][0], imageScale: _zoomBtnScale);
        SetDimensions();

        _currentView =
            _gameScreen.ActiveMode.GetDefaultView(gameScreen, null, _viewHeight, _viewWidth, ForceRedraw);

        gameScreen.OnMapEvent += MapEventTriggered;
        _game.OnUnitEvent += UnitEventTriggered;
        Click += OnClick;
        MouseDown += OnMouseDown;

    }

    private void OnMouseDown(object? sender, MouseEventArgs e)
    {
        var tile = GetTileAtMousePosition();
        if(tile == null) return;
        _gameScreen.ActiveMode.MouseDown(tile);
    }

    private void UnitEventTriggered(object sender, UnitEventArgs e)
    {
        switch (e.EventType)
        {
            // Unit movement animation event was raised
            case UnitEventType.MoveCommand:
            {
                if (e is MovementEventArgs mo)
                {
                    _animationQueue.Enqueue(new MoveAnimation(_gameScreen, mo, _animationQueue.LastOrDefault(_currentView), _viewHeight, _viewWidth, ForceRedraw));
                }

                break;
            }
            case UnitEventType.Attack:
            {
                if (e is CombatEventArgs combatEventArgs)
                {
                    _animationQueue.Enqueue(new AttackAnimation(_gameScreen, combatEventArgs, _animationQueue.LastOrDefault(_currentView), _viewHeight, _viewWidth, ForceRedraw));
                }
                break;
            }
            case UnitEventType.NewUnitActivated:
            {
                //animType = AnimationType.Waiting;
                //if (IsActiveSquareOutsideMapView) MapViewChange(Map.ActiveXY);
                //UpdateMap();
                break;
            }
        }
    }

    public override void OnResize()
    {
        if (Bounds.Equals(_currentBounds)) return;
        _currentBounds = Bounds;
        base.OnResize();

        SetDimensions();
        //ShowTile(_selectedTile);
    }

    private void SetDimensions()
    {
        if (_gameScreen.ToTPanelLayout)
        {
            _padding = _active.GetPadding(0, false);
        }
        else
        {
            _padding = _active.GetPadding(_headerLabel.TextSize.Y, false);
        }

        if (_backgroundImage != null)
        {
            _backgroundImage.Value.Unload();
        }
        _backgroundImage = ImageUtils.PaintDialogBase(_active, Width, Height, _padding, noWallpaper:true);

        if (!_gameScreen.ToTPanelLayout)
        {
            _headerLabel.Bounds = new Rectangle((int)Location.X, (int)Location.Y, Width, _padding.Top);
            _zoomInButton.Bounds = new Rectangle((int)Location.X + 11, (int)Location.Y + 7, _zoomInButton.GetPreferredWidth(), _zoomInButton.GetPreferredHeight());
            _zoomOutButton.Bounds = new Rectangle((int)Location.X + 11 + _zoomInButton.GetPreferredWidth() + 2, (int)Location.Y + 7, _zoomOutButton.GetPreferredWidth(), _zoomOutButton.GetPreferredHeight());
            _headerLabel.OnResize();
            _zoomInButton.OnResize();
            _zoomOutButton.OnResize();
        }

        _viewWidth = Width - _padding.Left - _padding.Right;
        _viewHeight = Height - _padding.Top - _padding.Bottom;
    }

    

    private void OnClick(object? sender, MouseEventArgs mouseEventArgs)
    {
        try
        {
            _gameScreen.Focused = this;
            var tile = GetTileAtMousePosition();
            if (tile == null)
            {
                var clickPosition = GetRelativeMousePosition();
                clickPosition.Y += Location.Y;
                if (!_gameScreen.ToTPanelLayout && ShapeHelper.CheckCollisionPointRec(clickPosition, _zoomInButton.Bounds))
                {
                    if (_gameScreen.Zoom < 8)
                        _gameScreen.TriggerMapEvent(new MapEventArgs(MapEventType.ZoomChange) { Zoom = _gameScreen.Zoom + 1 });
                }
                else if (!_gameScreen.ToTPanelLayout && ShapeHelper.CheckCollisionPointRec(clickPosition, _zoomOutButton.Bounds))
                {
                    if (_gameScreen.Zoom > -7)
                        _gameScreen.TriggerMapEvent(new MapEventArgs(MapEventType.ZoomChange) { Zoom = _gameScreen.Zoom - 1 });
                }
                return;
            }

            if (_gameScreen.ActiveMode.MapClicked(tile, mouseEventArgs.Button))
            {
                _gameScreen.ForceRedraw();
                MapViewChange(tile);
            }
        }
        finally
        {
            _gameScreen.ActiveMode.MouseClear();
        }
    }

    private Tile? GetTileAtMousePosition()
    {
        var clickPosition = GetRelativeMousePosition();
        if (clickPosition.X < _padding.Left + _padding.Right || clickPosition.X > _viewWidth + _padding.Left + _padding.Right || clickPosition.Y < _padding.Top || clickPosition.Y > _padding.Top + _viewHeight)
        {
            return null;
        }

        var map = _gameScreen.CurrentMap;
        var dim = _gameScreen.TileCache.GetDimensions(map, _gameScreen.Zoom);
        var clickedTilePosition = clickPosition - new Vector2(_padding.Left, _padding.Top) + _currentView.Offsets;
        var y = Math.DivRem((int)clickedTilePosition.Y, dim.HalfHeight, out var yRemainder);
        var odd = y % 2 == 1;
        var clickX = (int)(odd ? clickedTilePosition.X - dim.HalfWidth : clickedTilePosition.X);
        if (clickX < 0)
        {
            if (map.Flat)
            {
                clickX = 0;
            }
            else
            {
                clickX += dim.TotalWidth;
            }
        }
        else if (clickX > dim.TotalWidth)
        {
            if (map.Flat)
            {
                clickX = dim.TotalWidth - 1;
            }
            else
            {
                clickX -= dim.TotalWidth;
            }
        }

        var x = Math.DivRem(clickX, dim.TileWidth, out var xRemainder);

        if (xRemainder < dim.HalfWidth && y > 0)
        {
            if (yRemainder *  dim.HalfWidth + xRemainder *  dim.HalfHeight < dim.DiagonalCut)
            {
                y -= 1;
                if (!odd)
                {
                    x -= 1;
                    if (x < 0)
                    {
                        x = map.Flat ? 0 : map.Tile.GetLength(0) - 1;
                    }
                }
            }
        }
        else if (xRemainder > dim.HalfWidth)
        {
            if ((dim.TileWidth - xRemainder) *  dim.HalfHeight + yRemainder *  dim.HalfWidth < dim.DiagonalCut)
            {
                y -= 1;
                if (odd)
                {
                    x += 1;
                    if (x == map.Tile.GetLength(0))
                    {
                        if (map.Flat)
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

        if (0 <= y && y < map.Tile.GetLength(1))
        {
            x = Utils.WrapNumber(2 * x + _currentView.Xshift, 2 * map.XDim) / 2;
            return map.Tile[x, y];
        }

        return null;
    }

    private void MapViewChange(Tile tile)
    {
        if(_currentView.IsDefault && _currentView.Location != tile)
        {
            NextView();
        }
    }

    private void MapEventTriggered(object sender, MapEventArgs e)
    {
        switch (e.EventType)
        {
            case MapEventType.MinimapViewChanged:
                {
                    ForceRedraw = true;
                    if (_currentView.IsDefault)
                    {
                        if (_gameScreen.ActiveMode != _gameScreen.ViewPiece)
                        {
                            _gameScreen.ActiveMode = _gameScreen.ViewPiece;
                        }

                        if (_gameScreen.Player.ActiveTile != _currentView.Location)
                        {
                            MapViewChange(_gameScreen.Player.ActiveTile);
                        }
                    }

                    break;
                }
            case MapEventType.ZoomChange:
                {
                    _gameScreen.Zoom = e.Zoom;
                    _gameScreen.ForceRedraw();
                    NextView();
                }
                break;
            default: break;
        }
    }

    private Rectangle _currentBounds;

    private DateTime _animationStart;
    public override void Draw(bool pulse)
    {
        if (_animationStart.AddMilliseconds(_currentView.Interval) < DateTime.Now)
        {
            if (_currentView.Finished())
            {
                NextView();
            }
            else
            {
                _currentView.Next();
            }

            _animationStart = DateTime.Now;
        }

        var paddedLoc = new Vector2(Location.X + _padding.Left, Location.Y + _padding.Top);
        Graphics.DrawTextureEx(_currentView.BaseImage, paddedLoc, 0f, 1f,
            Color.White);

        var cityDetails = new List<CityData>();

        var zoom = _gameScreen.Zoom;
        foreach (var element in _currentView.Elements)
        {
            if (element is CityData data)
            {
                element.Draw(element.Location + paddedLoc, scale: ImageUtils.ZoomScale(zoom));
                cityDetails.Add(data);

                var size = data.Size.ToString();
                var fontSize = 14.ZoomScale(zoom);
                var textSize = TextManager.MeasureTextEx(Fonts.TnRbold, size, fontSize, 0);
                var citySizeRectLoc = paddedLoc + data.Location + data.SizeRectLoc.ZoomScale(zoom);
                var textPosition = citySizeRectLoc;
                Graphics.DrawRectangle((int)citySizeRectLoc.X, (int)citySizeRectLoc.Y, (int)textSize.X, (int)textSize.Y, data.Color.TextColour);
                Graphics.DrawRectangleLines((int)citySizeRectLoc.X - 1, (int)citySizeRectLoc.Y, (int)textSize.X + 2, (int)textSize.Y, Color.Black);
                Graphics.DrawTextEx(Fonts.TnRbold, size, textPosition, fontSize, 0, Color.Black);
            }
            else if (element.IsTerrain || !_currentView.ActionTiles.Contains(element.Tile) || element.Tile.IsCityPresent)
            {
                element.Draw(element.Location + paddedLoc, isShaded: element.IsShaded, scale: ImageUtils.ZoomScale(zoom));
            }
        }

        foreach (var cityData in cityDetails)
        {
            var name = cityData.Name;
            var fontSize = 20.ZoomScale(zoom);
            var textSize = TextManager.MeasureTextEx(_active.Look.DefaultFont, name, fontSize, 1);
            var textPosition = paddedLoc + cityData.Location + new Vector2(cityData.Texture.Width.ZoomScale(zoom) / 2f , cityData.Texture.Height.ZoomScale(zoom)) - textSize /2f;

            Graphics.DrawTextEx(_active.Look.DefaultFont, name, textPosition + new Vector2(1,1), fontSize, 1, Color.Black);
            Graphics.DrawTextEx(_active.Look.DefaultFont, name, textPosition, fontSize, 1, cityData.Color.TextColour);
        }

        foreach (var animation in _currentView.CurrentAnimations)
        {
            animation.Draw(animation.Location + paddedLoc, scale: ImageUtils.ZoomScale(zoom));
        }

        if (_backgroundImage != null)
            Graphics.DrawTextureEx(_backgroundImage.Value, Location, 0f, 1f, Color.White);

        if (!_gameScreen.ToTPanelLayout)
        {
            _headerLabel.Draw(pulse);
            _zoomInButton?.Draw(pulse);
            _zoomOutButton?.Draw(pulse);
        }
    }

    private void NextView()
    {
        var nextView = _animationQueue.Count > 0
            ? _animationQueue.Dequeue()
            : _gameScreen.ActiveMode.GetDefaultView(_gameScreen, _currentView, _viewHeight, _viewWidth, ForceRedraw);
        if (nextView != _currentView)
        {
            _currentView.Dispose();
            _currentView = nextView;
        }
    }

    private bool _forceRedraw;

    internal bool ForceRedraw
    {
        private get
        {
            var s = _forceRedraw;
            _forceRedraw = false;
            return s;
        }
        set => _forceRedraw = value;
    }
}