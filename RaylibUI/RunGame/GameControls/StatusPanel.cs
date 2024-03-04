using Civ2engine;
using Civ2engine.Enums;
using Civ2engine.Events;
using Civ2engine.Terrains;
using Model;
using Model.Interface;
using Raylib_cs;
using RaylibUI.BasicTypes;
using RaylibUI.BasicTypes.Controls;
using RaylibUI.Controls;
using RaylibUI.Forms;
using System.Globalization;

namespace RaylibUI.RunGame.GameControls;

public class StatusPanel : BaseControl
{
    private readonly GameScreen _gameScreen;
    private readonly Game _game;
    private readonly IUserInterface _active;
    private readonly HeaderLabel _headerLabel;
    private Texture2D? _backgroundImage;
    private Rectangle _internalBounds;
    private Padding _padding;

    public StatusPanel(GameScreen gameScreen, Game game) : base(gameScreen)
    {
        _gameScreen = gameScreen;
        _game = game;
        _active = gameScreen.MainWindow.ActiveInterface;

        _headerLabel = new HeaderLabel(gameScreen, _active.Look, Labels.For(LabelIndex.Status), fontSize: _active.Look.HeaderLabelFontSizeNormal);
        _padding = _active.GetPadding(_headerLabel?.TextSize.Y ?? 0, false);
        Click += OnClick;
        _game.OnPlayerEvent += PlayerEventTriggered;
    }



    public void Update()
    {
        var yOffset = 1;

        var populText = _game.GetPlayerCiv.Population.ToString("###,###,###",
                    new NumberFormatInfo() { NumberDecimalSeparator = "," }) + " " + Labels.For(LabelIndex.People);
        var populLabel = new StatusLabel(_gameScreen, populText);
        populLabel.Bounds = _internalBounds with { Y = _internalBounds.Y + yOffset, Height = populLabel.GetPreferredHeight() };

        var labelHeight = 18;

        var yearLabel = new StatusLabel(_gameScreen, _game.Date.GameYearString(_game.TurnNumber));
        yearLabel.Bounds = _internalBounds with { Y = _internalBounds.Y + yOffset + labelHeight, Height = yearLabel.GetPreferredHeight() };

        var goldLabel = new StatusLabel(_gameScreen, $"{_game.GetPlayerCiv.Money} {Labels.For(LabelIndex.Gold)}  {_game.GetPlayerCiv.TaxRate / 10}.{_game.GetPlayerCiv.LuxRate / 10}.{_game.GetPlayerCiv.ScienceRate / 10}");
        goldLabel.Bounds = _internalBounds with { Y = _internalBounds.Y + yOffset + 2 * labelHeight, Height = goldLabel.GetPreferredHeight() };

        var turnsLabel = new StatusLabel(_gameScreen, $"{Labels.For(LabelIndex.Turn)} {_game.TurnNumber}", TextAlignment.Right);
        turnsLabel.Bounds = _internalBounds with { Y = _internalBounds.Y + yOffset + 2 * labelHeight, Height = turnsLabel.GetPreferredHeight() };

        var iconNo = 0; // TODO: determine one of 4 icons based on current research progress (0...25%, 25...50%, 50...75%, 75...100%)
        var researchIcon = new TextureDisplay(_gameScreen, _active.MiscImages.ResearchProgressIcon[iconNo], new System.Numerics.Vector2(_internalBounds.X + 119, _internalBounds.Y + 20), scale: 1.5f);

        Children = new List<IControl>() { populLabel, yearLabel, goldLabel, turnsLabel, researchIcon };

        // TODO: find out when global warming icon is shown (it's based on no of skull icons on map)
        if (true)
        {
            iconNo = 0;
            var warmingIcon = new TextureDisplay(_gameScreen, _active.MiscImages.GlobalWarmingIcon[iconNo], new System.Numerics.Vector2(_internalBounds.X + 150, _internalBounds.Y + 20), scale: 1.5f);
            Children.Add(warmingIcon);
        }

        if (_game.GetPlayerCiv == _game.GetActiveCiv)
        {
            foreach (var c in _gameScreen.ActiveMode.GetSidePanelContents(_internalBounds))
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
        _backgroundImage = ImageUtils.PaintDialogBase(_active, Width, Height, _padding, statusPanel: true);
        
        _headerLabel.Bounds = new Rectangle((int)Location.X, (int)Location.Y, Width, _padding.Top);
        _headerLabel.OnResize();
        _internalBounds = new Rectangle(Location.X + _padding.Left, Location.Y + _padding.Top, Width - _padding.Left - _padding.Right, Height - _padding.Top - _padding.Bottom);
        base.OnResize();
        Update();
    }

    public override void Draw(bool pulse)
    {
        Raylib.DrawTexture(_backgroundImage.Value,(int)Location.X, (int)Location.Y, Color.White);
        _headerLabel.Draw(pulse);
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
            Raylib.DrawRectangleRec(new Rectangle(_internalBounds.X + _internalBounds.Width - 8, _internalBounds.Y + _internalBounds.Height - 6, 8, 6), _active.PlayerColours[_game.GetActiveCiv.Id].LightColour);
    }

    private void PlayerEventTriggered(object sender, PlayerEventArgs e)
    {
        switch (e.EventType)
        {
            case PlayerEventType.NewTurn:
                {
                    Update();
                    break;
                }
            default: break;
        }
    }
}

public class StatusLabel : LabelControl
{
    public StatusLabel(IControlLayout layout, string text, TextAlignment alignment = TextAlignment.Left, Color[]? switchColors = null, int switchTime = 0) : base(layout, text, true, fontSize: layout.MainWindow.ActiveInterface.Look.StatusPanelLabelFontSize, colorFront: layout.MainWindow.ActiveInterface.Look.StatusPanelLabelColor, colorShadow: layout.MainWindow.ActiveInterface.Look.StatusPanelLabelColorShadow, shadowOffset: new System.Numerics.Vector2(1,1), spacing: 0f, alignment: alignment, offset: 8, defaultHeight: 18, font: layout.MainWindow.ActiveInterface.Look.StatusPanelLabelFont, switchColors: switchColors, switchTime: switchTime)
    {

    }
}