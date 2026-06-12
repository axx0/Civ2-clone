using Civ2engine;
using Civ2engine.Enums;
using Civ2engine.Events;
using Model;
using RaylibUI.BasicTypes.Controls;
using System.Globalization;
using Model.Core;
using System.Numerics;
using Civ2engine.IO;
using Raylib_CSharp.Transformations;
using Raylib_CSharp.Textures;
using Raylib_CSharp.Rendering;
using Raylib_CSharp.Colors;
using Model.Controls;
using RaylibUI.Controls;

namespace RaylibUI.RunGame.GameControls;

public class StatusPanel : BaseControl
{
    private readonly GameScreen _gameScreen;
    private readonly IGame _game;
    private readonly IUserInterface _active;
    private readonly HeaderLabel _headerLabel;
    private readonly Button _endTurnButton;
    private Texture2D? _backgroundImage;
    private Rectangle _infoPanelRect, _unitPanelRect;
    private Padding _padding;

    public StatusPanel(GameScreen gameScreen, IGame game) : base(gameScreen)
    {
        _gameScreen = gameScreen;
        _game = game;
        _active = gameScreen.MainWindow.ActiveInterface;

        _headerLabel = new HeaderLabel(gameScreen, _active.Look, Labels.For(LabelIndex.Status), fontSize: _active.Look.HeaderLabelFontSizeNormal);
        _endTurnButton = new Button(gameScreen, Labels.For(LabelIndex.Turn), _active.Look.ButtonFont, 15);
        _endTurnButton.Click += EndTurnButtonClicked;
        _padding = _active.GetPadding(_headerLabel.TextSize.Y, false);
        Click += OnClick;
    }

    public void Update()
    {
        var statusFontSize = _gameScreen.ToTPanelLayout ? 15 : 16;
        var statusPaddingTop = 0;
        var yOffset = _gameScreen.ToTPanelLayout ? 2 : 2;
        var labelHeight = Math.Max(18, statusFontSize + 3);
        var infoInset = _gameScreen.ToTPanelLayout ? 1 : 2;
        var infoWidth = Math.Max(1, (int)_infoPanelRect.Width - 2 * infoInset);

        var populText = _game.GetPlayerCiv.Cities.Count == 0 ? "0 " + Labels.For(LabelIndex.People) : 
            _game.GetPlayerCiv.Cities.Sum(c=>c.GetPopulation()).ToString("###,###,###", 
            new NumberFormatInfo() { NumberDecimalSeparator = "," }) + " " + Labels.For(LabelIndex.People);
        
        var populLabel = new StatusLabel(_gameScreen, populText, alignment: HorizontalAlignment.Right, fontSize: statusFontSize);
        populLabel.Padding = new(0, statusPaddingTop, 0, 0);
        populLabel.Location = new(_infoPanelRect.X + infoInset, _infoPanelRect.Y + yOffset);
        populLabel.Width = infoWidth;
        populLabel.Height = populLabel.GetPreferredHeight();

        var yearLabel = new StatusLabel(_gameScreen, _game.Date.GameYearString(_game.TurnNumber), fontSize: statusFontSize);
        yearLabel.Padding = new(0, statusPaddingTop, 0, 0);
        yearLabel.Location = new(_infoPanelRect.X + infoInset, _infoPanelRect.Y + yOffset + labelHeight);
        yearLabel.Width = (int)(infoWidth * 0.55f);
        yearLabel.Height = yearLabel.GetPreferredHeight();

        var goldLabel = new StatusLabel(_gameScreen, $"{_game.GetPlayerCiv.Money} {Labels.For(LabelIndex.Gold)}  {_game.GetPlayerCiv.TaxRate / 10}.{_game.GetPlayerCiv.LuxRate / 10}.{_game.GetPlayerCiv.ScienceRate / 10}", fontSize: statusFontSize);
        goldLabel.Padding = new (0, statusPaddingTop, 0, 0);
        goldLabel.Location = new(_infoPanelRect.X + infoInset, _infoPanelRect.Y + yOffset + 2 * labelHeight);
        goldLabel.Width = (int)(infoWidth * 0.58f);
        goldLabel.Height = goldLabel.GetPreferredHeight();

        var turnsLabel = new StatusLabel(_gameScreen, $"{Labels.For(LabelIndex.Turn)} {_game.TurnNumber}", HorizontalAlignment.Right, fontSize: statusFontSize);
        turnsLabel.Padding = new (0, statusPaddingTop, 0, 0);
        turnsLabel.Location = new(_infoPanelRect.X + infoInset, _infoPanelRect.Y + yOffset + 2 * labelHeight);
        turnsLabel.Width = infoWidth;
        turnsLabel.Height = turnsLabel.GetPreferredHeight();

        var iconNo = 0; // TODO: determine one of 4 icons based on current research progress (0...25%, 25...50%, 50...75%, 75...100%)
        var iconScale = _gameScreen.ToTPanelLayout ? 1.0f : 1.0f;
        var researchIconLoc = new Vector2(
            _infoPanelRect.X + _infoPanelRect.Width - 58,
            _infoPanelRect.Y + yOffset + labelHeight - 1);
        if (_gameScreen.ToTPanelLayout)
            researchIconLoc = new Vector2(_infoPanelRect.X + 7, _infoPanelRect.Y + 72);
        var researchIcon = new TextureDisplay(_gameScreen, TextureCache.GetImage(_active.PicSources["researchProgress"][iconNo]), researchIconLoc, scale: iconScale);

        _endTurnButton.Visible = _game.GetPlayerCiv == _game.GetActiveCiv;
        Controls = [_headerLabel, populLabel, yearLabel, goldLabel, turnsLabel, researchIcon, _endTurnButton];

        // TODO: find out when global warming icon is shown (it's based on no of skull icons on map)
        if (true)
        {
            iconNo = 0;
            var warmingIconLoc = new Vector2(researchIconLoc.X + 24, researchIconLoc.Y);
            var warmingIcon = new TextureDisplay(_gameScreen, TextureCache.GetImage(_active.PicSources["globalWarming"][iconNo]), warmingIconLoc, scale: iconScale);
            Controls.Add(warmingIcon);
        }

        if (_game.GetPlayerCiv == _game.GetActiveCiv)
        {
            foreach (var c in _gameScreen.ActiveMode.GetSidePanelContents(_unitPanelRect))
            {
                Controls.Add(c);
            }
        }
    }

    private void OnClick(object? sender, MouseEventArgs e)
    {
        _gameScreen.ActiveMode.PanelClick();
    }

    private void EndTurnButtonClicked(object? sender, MouseEventArgs e)
    {
        if (_game.ProcessEndOfTurn())
        {
            _game.ChoseNextCiv();
        }
    }

    public override void OnResize()
    {
        _headerLabel.Visible = !_gameScreen.ToTPanelLayout;

        if (_gameScreen.ToTPanelLayout)
        {
            _padding = _active.GetPadding(0, false);

        }
        else
        {
            _padding = _active.GetPadding(_headerLabel.TextSize.Y, false);
            _headerLabel.Location = new(0, 0);
            _headerLabel.Width = Width;
            _headerLabel.Height = _padding.Top;
        }

        _backgroundImage = ImageUtils.PaintDialogBase(_active, Width, Height, _padding, statusPanel: true, ToTStatusPanelLayout: _gameScreen.ToTPanelLayout);

        if (_gameScreen.ToTPanelLayout)
        {
            _infoPanelRect = new Rectangle(_padding.Left, _padding.Top, 0.25f * Width - 1 - _padding.Left - _padding.Right, Height - _padding.Top - _padding.Bottom);
            _unitPanelRect = new Rectangle(_padding.Left + 0.25f * Width - 15, _padding.Top, 0.75f * Width + 14 - _padding.Left - _padding.Right, Height - _padding.Top - _padding.Bottom);
        }
        else
        {
            _infoPanelRect = new Rectangle(_padding.Left, _padding.Top, Width - _padding.Left - _padding.Right, 60);
            _unitPanelRect = new Rectangle(_padding.Left, _padding.Top + 68, Width - _padding.Left - _padding.Right, Height - 68 - _padding.Top - _padding.Bottom);
        }

        _endTurnButton.Width = 78;
        _endTurnButton.Height = 30;
        _endTurnButton.Location = new(Width - _padding.Right - _endTurnButton.Width - 6, Height - _padding.Bottom - _endTurnButton.Height - 6);

        base.OnResize();
        Update();
    }

    public override void Draw(bool pulse)
    {
        Graphics.DrawRectangle((int)Location.X, (int)Location.Y, Width, Height, Color.Black);
        if (_backgroundImage.HasValue)
        {
            Graphics.DrawTexture(_backgroundImage.Value, (int)Location.X, (int)Location.Y, Color.White);
        }

        base.Draw(pulse);

        // AI turn civ indicator
        if (_game.GetPlayerCiv != _game.GetActiveCiv)
            Graphics.DrawRectangleRec(new Rectangle(_unitPanelRect.X + _unitPanelRect.Width - 8, _unitPanelRect.Y + _unitPanelRect.Height - 6, 8, 6), _active.PlayerColours[_game.GetActiveCiv.Id].LightColour);
    }
}
