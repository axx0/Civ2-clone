using Civ2engine;
using Civ2engine.Enums;
using Civ2engine.Events;
using Model;
using RaylibUI.BasicTypes.Controls;
using System.Globalization;
using Model.Core;
using System.Numerics;
using Raylib_CSharp.Transformations;
using Raylib_CSharp.Textures;
using Raylib_CSharp.Rendering;
using Raylib_CSharp.Colors;

namespace RaylibUI.RunGame.GameControls;

public class StatusPanel : BaseControl
{
    private readonly GameScreen _gameScreen;
    private readonly IGame _game;
    private readonly IUserInterface _active;
    private readonly HeaderLabel _headerLabel;
    private Texture2D? _backgroundImage;
    private Rectangle _infoPanelBounds, _unitPanelBounds;
    private Padding _padding;

    public StatusPanel(GameScreen gameScreen, IGame game) : base(gameScreen)
    {
        _gameScreen = gameScreen;
        _game = game;
        _active = gameScreen.MainWindow.ActiveInterface;

        _headerLabel = new HeaderLabel(gameScreen, _active.Look, Labels.For(LabelIndex.Status), fontSize: _active.Look.HeaderLabelFontSizeNormal);
        _padding = _active.GetPadding(_headerLabel.TextSize.Y, false);
        Click += OnClick;
    }

    public void Update()
    {
        var yOffset = 1;

        var populText = _game.GetPlayerCiv.Cities.Count == 0 ? "0 " + Labels.For(LabelIndex.People) : 
            _game.GetPlayerCiv.Cities.Sum(c=>c.GetPopulation()).ToString("###,###,###", 
            new NumberFormatInfo() { NumberDecimalSeparator = "," }) + " " + Labels.For(LabelIndex.People);
        
        var populLabel = new StatusLabel(_gameScreen, populText, alignment: TextAlignment.Right);
        populLabel.Padding = new(0, 8, 0, 0);
        populLabel.Bounds = _infoPanelBounds with { Y = _infoPanelBounds.Y + yOffset, Height = populLabel.GetPreferredHeight() };

        var labelHeight = 18;

        var yearLabel = new StatusLabel(_gameScreen, _game.Date.GameYearString(_game.TurnNumber));
        yearLabel.Padding = new(0, 8, 0, 0);
        yearLabel.Bounds = _infoPanelBounds with { Y = _infoPanelBounds.Y + yOffset + labelHeight, Height = yearLabel.GetPreferredHeight() };

        var goldLabel = new StatusLabel(_gameScreen, $"{_game.GetPlayerCiv.Money} {Labels.For(LabelIndex.Gold)}  {_game.GetPlayerCiv.TaxRate / 10}.{_game.GetPlayerCiv.LuxRate / 10}.{_game.GetPlayerCiv.ScienceRate / 10}");
        goldLabel.Padding = new (0, 8, 0, 0);
        goldLabel.Bounds = _infoPanelBounds with { Y = _infoPanelBounds.Y + yOffset + 2 * labelHeight, Height = goldLabel.GetPreferredHeight() };

        var turnsLabel = new StatusLabel(_gameScreen, $"{Labels.For(LabelIndex.Turn)} {_game.TurnNumber}", TextAlignment.Right);
        turnsLabel.Padding = new (0, 8, 0, 0);
        turnsLabel.Bounds = _infoPanelBounds with { Y = _infoPanelBounds.Y + yOffset + 2 * labelHeight, Height = turnsLabel.GetPreferredHeight() };

        var iconNo = 0; // TODO: determine one of 4 icons based on current research progress (0...25%, 25...50%, 50...75%, 75...100%)
        var researchIconLoc = new Vector2(_infoPanelBounds.X + 119, _infoPanelBounds.Y + 20);
        if (_gameScreen.ToTPanelLayout)
            researchIconLoc = new Vector2(_infoPanelBounds.X + 7, _infoPanelBounds.Y + 72);
        var researchIcon = new TextureDisplay(_gameScreen, TextureCache.GetImage(_active.PicSources["researchProgress"][iconNo]), researchIconLoc, scale: 1.5f);

        Children = new List<IControl>() { populLabel, yearLabel, goldLabel, turnsLabel, researchIcon };

        // TODO: find out when global warming icon is shown (it's based on no of skull icons on map)
        if (true)
        {
            iconNo = 0;
            var warmingIconLoc = new Vector2(researchIconLoc.X + 31, researchIconLoc.Y);
            var warmingIcon = new TextureDisplay(_gameScreen, TextureCache.GetImage(_active.PicSources["globalWarming"][iconNo]), warmingIconLoc, scale: 1.5f);
            Children.Add(warmingIcon);
        }

        if (_game.GetPlayerCiv == _game.GetActiveCiv)
        {
            foreach (var c in _gameScreen.ActiveMode.GetSidePanelContents(_unitPanelBounds))
            {
                Children.Add(c);
            }
        }
    }

    private void OnClick(object? sender, MouseEventArgs e)
    {
        _gameScreen.ActiveMode.PanelClick();
    }

    public override void OnResize()
    {
        if (_gameScreen.ToTPanelLayout)
        {
            _padding = _active.GetPadding(0, false);

        }
        else
        {
            _padding = _active.GetPadding(_headerLabel.TextSize.Y, false);
            _headerLabel.Bounds = new Rectangle((int)Location.X, (int)Location.Y, Width, _padding.Top);
            _headerLabel.OnResize();
        }

        _backgroundImage = ImageUtils.PaintDialogBase(_active, Width, Height, _padding, statusPanel: true, ToTStatusPanelLayout: _gameScreen.ToTPanelLayout);

        if (_gameScreen.ToTPanelLayout)
        {
            _infoPanelBounds = new Rectangle(Location.X + _padding.Left, Location.Y + _padding.Top, 0.25f * Width - 1 - _padding.Left - _padding.Right, Height - _padding.Top - _padding.Bottom);
            _unitPanelBounds = new Rectangle(Location.X + _padding.Left + 0.25f * Width - 15, Location.Y + _padding.Top, 0.75f * Width + 14 - _padding.Left - _padding.Right, Height - _padding.Top - _padding.Bottom);
        }
        else
        {
            _infoPanelBounds = new Rectangle(Location.X + _padding.Left, Location.Y + _padding.Top, Width - _padding.Left - _padding.Right, 60);
            _unitPanelBounds = new Rectangle(Location.X + _padding.Left, Location.Y + _padding.Top + 68, Width - _padding.Left - _padding.Right, Height - 68 - _padding.Top - _padding.Bottom);
        }

        base.OnResize();
        Update();
    }

    public override void Draw(bool pulse)
    {
        Graphics.DrawRectangle((int)Location.X, (int)Location.Y, Width, Height, Color.Black);
        Graphics.DrawTexture(_backgroundImage.Value,(int)Location.X, (int)Location.Y, Color.White);
        if (!_gameScreen.ToTPanelLayout)
        {
            _headerLabel.Draw(pulse);
        }
        base.Draw(pulse);
        if (Children != null)
        {
            foreach (var control in Children)
            {
                control.Draw(pulse);
            }
        }

        // AI turn civ indicator
        if (_game.GetPlayerCiv != _game.GetActiveCiv)
            Graphics.DrawRectangleRec(new Rectangle(_unitPanelBounds.X + _unitPanelBounds.Width - 8, _unitPanelBounds.Y + _unitPanelBounds.Height - 6, 8, 6), _active.PlayerColours[_game.GetActiveCiv.Id].LightColour);
    }
}

public class StatusLabel : LabelControl
{
    public StatusLabel(IControlLayout layout, string text, TextAlignment alignment = TextAlignment.Left, Color[]? switchColors = null, int switchTime = 0, int fontSize = 18) : base(layout, text, true, alignment: alignment, defaultHeight: 18, font: layout.MainWindow.ActiveInterface.Look.StatusPanelLabelFont, fontSize: fontSize, spacing: 0f, colorFront: layout.MainWindow.ActiveInterface.Look.StatusPanelLabelColor, colorShadow: layout.MainWindow.ActiveInterface.Look.StatusPanelLabelColorShadow, shadowOffset: new System.Numerics.Vector2(1,1), switchColors: switchColors, switchTime: switchTime)
    {

    }
}
