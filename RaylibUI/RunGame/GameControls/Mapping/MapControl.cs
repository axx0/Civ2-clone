using System.Numerics;
using System.Security.Cryptography.X509Certificates;
using Civ2engine;
using Civ2engine.Enums;
using Civ2engine.Events;
using Civ2engine.MapObjects;
using Raylib_cs;
using RaylibUI.BasicTypes.Controls;
using RaylibUI.RunGame.GameControls.Mapping.Views;
using RaylibUI.Controls;

namespace RaylibUI.RunGame.GameControls.Mapping;

public class MapControl : BaseControl
{
    public override bool CanFocus => true;
    private readonly GameScreen _gameScreen;
    private readonly Game _game;
    private Texture2D? _backgroundImage;
    private int _viewWidth;
    private int _viewHeight;
    private const int PaddingSide = 11;
    private const int Top = 38;
    private const int PaddingBtm = 11;
    private HeaderLabel _headerLabel;
    
    private readonly Queue<IGameView> _animationQueue = new();
    private IGameView _currentView;

    public MapControl(GameScreen gameScreen, Game game, Rectangle initialBounds) : base(gameScreen)
    {
        Bounds = initialBounds;
        _currentBounds = initialBounds;
        _gameScreen = gameScreen;
        _game = game;
        
        _headerLabel = new HeaderLabel(gameScreen, $"{_game.GetPlayerCiv.Adjective} Map");
        SetDimensions();

        _currentView =
            _gameScreen.ActiveMode.GetDefaultView(gameScreen, null, _viewHeight, _viewWidth, ForceRedraw);
        
        gameScreen.OnMapEvent += MapEventTriggered;
        _game.OnUnitEvent += UnitEventHappened;
        Click += OnClick;
        MouseDown += OnMouseDown;

        _clickTimer = new Timer(_ => _longHold = true);
    }
    
    private bool _longHold;
    private readonly Timer _clickTimer;

    private void OnMouseDown(object? sender, MouseEventArgs e)
    {
        var tile = GetTileAtMousePosition();
        if(tile == null) return;
        _longHold = false;
        _clickTimer.Change(500, -1);
    }

    private void UnitEventHappened(object sender, UnitEventArgs e)
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


                // animationFrames = GetAnimationFrames.UnitAttack(e);
                // StartAnimation(AnimationType.Attack);
                break;
            }
            // case UnitEventType.StatusUpdate:
            //     {
            //         animType = AnimationType.Waiting;
            //         if (IsActiveSquareOutsideMapView) MapViewChange(Map.ActiveXY);
            //         UpdateMap();
            //         break;
            //     }
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
        if (_backgroundImage != null)
        {
            Raylib.UnloadTexture(_backgroundImage.Value);
        }
        _backgroundImage = ImageUtils.PaintDialogBase(Width, Height, Top, PaddingBtm, PaddingSide, noWallpaper:true);

        _headerLabel.Bounds = new Rectangle((int)Location.X, (int)Location.Y, Width, Top);
        _headerLabel.OnResize();

        _viewWidth = Width - 2 * PaddingSide;
        _viewHeight = Height - Top - PaddingBtm;
    }

    

    private void OnClick(object? sender, MouseEventArgs mouseEventArgs)
    {
        try
        {
            _clickTimer.Change(-1, -1);
            _gameScreen.Focused = this;
            var tile = GetTileAtMousePosition();
            if (tile == null)
            {
                return;
            }

            if (_gameScreen.ActiveMode.MapClicked(tile, mouseEventArgs.Button, _longHold))
            {
                MapViewChange(tile);
            }
        }
        finally
        {
            _longHold = false;
        }
    }

    private Tile? GetTileAtMousePosition()
    {
        var clickPosition = GetRelativeMousePosition();
        if (clickPosition.X < PaddingSide || clickPosition.X > _viewWidth + PaddingSide || clickPosition.Y < Top ||
            clickPosition.Y > Top + _viewHeight)
        {
            return null;
        }

        var dim = _gameScreen.TileCache.GetDimensions(_game.CurrentMap);
        var clickedTilePosition = clickPosition - new Vector2(PaddingSide, Top) + _currentView.Offsets;
        var y = Math.DivRem((int)(clickedTilePosition.Y), dim.HalfHeight, out var yRemainder);
        var odd = y % 2 == 1;
        var clickX = (int)(odd ? clickedTilePosition.X - dim.HalfWidth : clickedTilePosition.X);
        if (clickX < 0)
        {
            if (_game.CurrentMap.Flat)
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
            if (_game.CurrentMap.Flat)
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
                        x = _game.CurrentMap.Flat ? 0 : _game.CurrentMap.Tile.GetLength(0) - 1;
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
                    if (x == _game.CurrentMap.Tile.GetLength(0))
                    {
                        if (_game.CurrentMap.Flat)
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

        if (0 <= y && y < _game.CurrentMap.Tile.GetLength(1))
        {
            return _game.CurrentMap.Tile[x, y];
        }

        return null;
    }

    private void MapViewChange(Tile tile)
    {
        if(_currentView.IsDefault && _currentView.Location != tile)
        {
            NextView();
        }
        var dim = _gameScreen.TileCache.GetDimensions(_game.CurrentMap);
        _gameScreen.TriggerMapEvent(new MapEventArgs(MapEventType.MapViewChanged,
            new[] { (int)_currentView.Offsets.X / dim.HalfWidth, (int)_currentView.Offsets.Y / dim.HalfHeight },
            new[] { _viewWidth / dim.HalfWidth, _viewHeight / dim.HalfHeight }));   
    }

    private void MapEventTriggered(object sender, MapEventArgs e)
    {
        switch (e.EventType)
        {
            case MapEventType.MinimapViewChanged:
                {
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

        var paddedLoc = new Vector2(Location.X + PaddingSide, Location.Y + Top);
        Raylib.DrawTextureEx(_currentView.BaseImage, paddedLoc, 0f,1f,
            Color.WHITE);

        var cityDetails = new List<CityData>();

        foreach (var element in _currentView.Elements)
        {
            if (element is CityData data)
            {
                element.Draw(element.Location + paddedLoc);
                cityDetails.Add(data);
            }
            else if (element.IsTerrain || !_currentView.ActionTiles.Contains(element.Tile))
            {
                element.Draw(element.Location + paddedLoc);
            }
        }

        foreach (var cityData in cityDetails)
        {
            var name = cityData.Name;
            var textSize = Raylib.MeasureTextEx(Fonts.DefaultFont, name, 20, 1);
            var textPosition = paddedLoc + cityData.Location + new Vector2(cityData.Texture.width /2f , 0) - textSize /2f;

            Raylib.DrawTextEx(Fonts.DefaultFont, name, textPosition + new Vector2(1,1), 20, 1, Color.BLACK);
            Raylib.DrawTextEx(Fonts.DefaultFont, name, textPosition, 20, 1, cityData.Color.TextColour);
        }

        foreach (var animation in _currentView.CurrentAnimations)
        {
            animation.Draw(animation.Location + paddedLoc);
        }

        if (_backgroundImage != null)
            Raylib.DrawTextureEx(_backgroundImage.Value, Location, 0f, 1f, Color.WHITE);
        _headerLabel.Draw(pulse);
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