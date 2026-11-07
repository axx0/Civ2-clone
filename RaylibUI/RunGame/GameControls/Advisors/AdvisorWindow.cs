using Civ2engine;
using Civ2engine.IO;
using Model;
using Model.Core;
using Raylib_CSharp.Interact;
using RaylibUI.Controls;
using RaylibUtils;

namespace RaylibUI.RunGame.GameControls.Advisors;

/// <summary>
/// Fixed-size window painted on a background picture (advisors):
/// - size = the picture plus the dialog padding,
/// </summary>
public abstract class AdvisorWindow : BaseDialog
{
    private const int ButtonHeight = 28;
    private const int ButtonGap = 2;

    private readonly int _width, _height;
    private readonly List<Button> _buttons = [];

    protected readonly GameScreen GameScreen;
    protected readonly IUserInterface Active;
    protected readonly IGame Game;
    protected readonly Civilization Civ;

    /// <summary>
    /// First header line (the title), or null if the window has no header.
    /// </summary>
    protected AdvisorsHeaderLabel? HeaderLabel { get; }

    /// <summary>
    /// Last header line, or null if the window has no header.
    /// </summary>
    protected AdvisorsHeaderLabel? LastHeaderLabel { get; }

    /// <summary>
    /// Width inside the dialog's side padding (= the picture's width).
    /// </summary>
    protected int InnerWidth => _width - PaddingSide;

    /// <param name="pictureKey">Key of the background picture in PicSources.</param>
    /// <param name="title">First header line; null = no header.</param>
    protected AdvisorWindow(GameScreen gameScreen, string pictureKey, string? title) : base(gameScreen.Main)
    {
        GameScreen = gameScreen;
        Active = gameScreen.MainWindow.ActiveInterface;
        Game = gameScreen.Game;
        Civ = Game.GetPlayerCiv;

        LayoutPadding = Active.GetPadding(0, false);

        var back = Active.PicSources[pictureKey][0];
        _width = Images.GetImageWidth(back, Active) + PaddingSide;
        _height = Images.GetImageHeight(back, Active) + LayoutPadding.Top + LayoutPadding.Bottom;

        BackgroundImage = ImageUtils.PaintDialogBase(Active, _width, _height, LayoutPadding,
            Images.ExtractBitmap(back, Active));

        if (title != null)
        {
            HeaderLabel = AddHeaderLine(title);
            AddHeaderLine($"{Game.GetRealmName(Civ.Government)} {Labels.For(LabelIndex.of)} " +
                $"{Labels.For(LabelIndex.the)} {Civ.TribeName}");
            LastHeaderLabel = AddHeaderLine($"{Civ.LeaderTitle} {Civ.LeaderName}: {Game.Date.GameYearString(Game.TurnNumber)}");
        }
    }

    // Header lines are stacked from the top, 2 px apart
    private int _headerBottom;
    private AdvisorsHeaderLabel AddHeaderLine(string text)
    {
        _headerBottom += 2;
        var label = new AdvisorsHeaderLabel(this, text)
        {
            Location = new(LayoutPadding.Left, LayoutPadding.Top + _headerBottom),
            Width = InnerWidth
        };
        _headerBottom += label.Height;
        Controls.Add(label);
        return label;
    }

    /// <summary>
    /// Add a button to the bottom row. Without an action the button closes the window.
    /// The last button added gets the focus.
    /// </summary>
    protected Button AddButton(string text, Action? onClick = null)
    {
        var button = new Button(this, text, Active.Look.ButtonFont, 18) { Height = ButtonHeight };
        button.Click += (_, _) =>
        {
            if (onClick != null) onClick();
            else Close();
        };
        _buttons.Add(button);
        Controls.Add(button);
        LayoutButtons();

        Focused = button;
        return button;
    }

    /// <summary>
    /// Buttons share the width inside the picture frame evenly, 2 px apart.
    /// </summary>
    private void LayoutButtons()
    {
        if (_buttons.Count == 0) return;

        var left = LayoutPadding.Left + 2;
        var available = InnerWidth - 4 - (_buttons.Count - 1) * ButtonGap;
        var width = available / _buttons.Count;
        for (var i = 0; i < _buttons.Count; i++)
        {
            _buttons[i].Location = new(left + i * (width + ButtonGap), _height - LayoutPadding.Bottom - 2 - ButtonHeight);
            // The last button takes the rounding remainder, so the row ends at the frame
            _buttons[i].Width = i == _buttons.Count - 1 ? available - width * (_buttons.Count - 1) : width;
            _buttons[i].Height = ButtonHeight;
        }
    }

    protected void Close() => GameScreen.CloseDialog(this);

    public override int Width => _width;
    public override int Height => _height;

    public override void Resize(int width, int height)
    {
        SetLocation(width, Width, height, Height);
        LayoutButtons();

        foreach (var control in Controls)
        {
            control.OnResize();
        }
    }

    public override void OnKeyPress(KeyboardKey key)
    {
        if (key is KeyboardKey.Escape or KeyboardKey.Enter or KeyboardKey.KpEnter)
        {
            Close();
            return;
        }
        base.OnKeyPress(key);
    }
}
