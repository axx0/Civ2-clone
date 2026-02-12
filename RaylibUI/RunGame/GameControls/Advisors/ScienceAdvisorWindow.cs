using Civ2engine;
using Model;
using Model.Controls;
using Raylib_CSharp.Colors;
using Raylib_CSharp.Interact;
using Raylib_CSharp.Rendering;
using RaylibUI.BasicTypes;
using RaylibUI.Controls;
using RaylibUtils;

namespace RaylibUI.RunGame.GameControls;

public class ScienceAdvisorWindow : BaseDialog
{
    private readonly IUserInterface _active;
    private readonly int _width, _height;
    private readonly GameScreen _gameScreen;
    private readonly Civilization _civ;

    public ScienceAdvisorWindow(GameScreen gameScreen) : base(gameScreen.Main)
    {
        _gameScreen = gameScreen;
        _active = gameScreen.MainWindow.ActiveInterface;
        var game = gameScreen.Game;
        _civ = game.GetPlayerCiv;

        LayoutPadding = _active.GetPadding(0, false);

        var back = _active.PicSources["scienceAdvisor"][0];
        _width = Images.GetImageWidth(back, _active) + PaddingSide;
        _height = Images.GetImageHeight(back, _active) + LayoutPadding.Top + LayoutPadding.Bottom;

        BackgroundImage = ImageUtils.PaintDialogBase(_active, _width, _height, LayoutPadding,
            Images.ExtractBitmap(back, _active));

        var headerLabel = new AdvisorsHeaderLabel(this, Labels.For(LabelIndex.SCIENCEADVISOR))
        { Location = new(LayoutPadding.Left, LayoutPadding.Top + 2), Width = _width - PaddingSide };
        var tribeLabel = new AdvisorsHeaderLabel(this, $"{game.GetRealmName(_civ.Government)} {Labels.For(LabelIndex.of)}" +
            $" {LabelIndex.the} {_civ.TribeName}")
        { Location = new(LayoutPadding.Left, LayoutPadding.Top + 4 + headerLabel.Height), Width = _width - PaddingSide };
        var titleLabel = new AdvisorsHeaderLabel(this, $"{_civ.LeaderTitle} {_civ.LeaderName}: {game.Date.GameYearString(game.TurnNumber)}")
        { Location = new(LayoutPadding.Left, LayoutPadding.Top + 6 + headerLabel.Height + tribeLabel.Height), Width = _width - PaddingSide };
        Controls.Add(headerLabel);
        Controls.Add(tribeLabel);
        Controls.Add(titleLabel);

        if (_civ.ReseachingAdvance != -1)
        {
            var advance = game.Rules.Advances[_civ.ReseachingAdvance];
            Controls.Add(new AdvisorsHeaderLabel(this, $"{Labels.For(LabelIndex.Researching)}: {advance.Name}")
            { Location = new(LayoutPadding.Left, titleLabel.Location.Y + 27), Width = _width - PaddingSide });
        }

        List<ListboxGroup> groups = [];
        var allAdvances = game.Rules.Advances;
        for (var i = 0; i < allAdvances.Length; i++)
        {
            if (!_civ.Advances[i]) continue;

            var advance = allAdvances[i];
            var icon = _active.PicSources["advanceCategories"][5 * advance.Epoch + advance.KnowledgeCategory];
            var group = new ListboxGroup()
            {
                Elements = [new ListboxGroupElement { Icon = icon },
                            new ListboxGroupElement { Text = advance.Name, VerticalAlignment = VerticalAlignment.Center },
                            ],
                Height = 22
            };
            groups.Add(group);
        }

        var def = new ListboxDefinition()
        {
            Rows = 10,
            Columns = 3,
            VerticalScrollbar = false,
            Selectable = false,
            Looks = new ListboxLooks()
            {
                FontSize = 16,
                TextColorFront = Color.White,
                TextColorShadow = new Color(67, 67, 67, 255)
            },
            Groups = groups
        };

        var listbox = new Listbox(this, def)
        {
            Width = 596,
            Height = 240,
            Location = new(LayoutPadding.Left + 2, LayoutPadding.Top + 130)
        };
        listbox.ItemSelected += (_, i) => { };
        Controls.Add(listbox);

        var btnWidth = (_width - PaddingSide - 6) / 2;
        var goalBtn = new Button(this, Labels.For(LabelIndex.Goal), _active.Look.ButtonFont, 18)
        {
            Location = new(LayoutPadding.Left + 2, _height - LayoutPadding.Bottom - 2 - 28),
            Width = btnWidth,
            Height = 28
        };
        goalBtn.Click += (_, _) => { };

        var closeBtn = new Button(this, Labels.For(LabelIndex.Close), _active.Look.ButtonFont, 18)
        {
            Location = new(LayoutPadding.Left + 4 + btnWidth, _height - LayoutPadding.Bottom - 2 - 28),
            Width = btnWidth,
            Height = 28
        };
        closeBtn.Click += (_, _) => _gameScreen.CloseDialog(this);

        Controls.Add(goalBtn);
        Controls.Add(closeBtn);
    }

    public override int Width => _width;
    public override int Height => _height;

    public override void Resize(int width, int height)
    {
        SetLocation(width, Width, height, Height);

        foreach (var control in Controls)
        {
            control.OnResize();
        }
    }

    public override void OnKeyPress(KeyboardKey key)
    {
        if (key == KeyboardKey.Escape || key == KeyboardKey.Enter || key == KeyboardKey.KpEnter)
        {
            _gameScreen.CloseDialog(this);
        }
        base.OnKeyPress(key);
    }

    public override void Draw(bool pulse)
    {
        base.Draw(pulse);

        Graphics.DrawRectangleLinesEx(new(Location.X + 2 + LayoutPadding.Left, Location.Y + 71 + LayoutPadding.Top,
            Width - 4 - PaddingSide, 51), 2f, Color.White);
    }
}
