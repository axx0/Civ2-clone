using Civ2engine;
using Civ2engine.Production;
using Model;
using Model.Controls;
using Model.Core.Units;
using Raylib_CSharp.Colors;
using Raylib_CSharp.Interact;
using RaylibUI.BasicTypes;
using RaylibUI.BasicTypes.Controls;
using RaylibUI.Controls;
using RaylibUI.RunGame.GameControls.CityControls;
using RaylibUtils;

namespace RaylibUI.RunGame.GameControls;

public class DefenseMinisterWindow : BaseDialog
{
    private readonly IUserInterface _active;
    private readonly int _width, _height;
    private readonly GameScreen _gameScreen;
    private readonly Civilization _civ;
    private readonly AdvisorsHeaderLabel _headerLabel, _tribeLabel, _titleLabel;
    private readonly Listbox _liveListbox, _deadListbox;
    private readonly Button _casualtBtn;

    public DefenseMinisterWindow(GameScreen gameScreen) : base(gameScreen.Main)
    {
        _gameScreen = gameScreen;
        _active = gameScreen.MainWindow.ActiveInterface;
        var game = gameScreen.Game;
        _civ = game.GetPlayerCiv;

        LayoutPadding = _active.GetPadding(0, false);

        var back = _active.PicSources["defenseMinister"][0];
        _width = Images.GetImageWidth(back, _active) + PaddingSide;
        _height = Images.GetImageHeight(back, _active) + LayoutPadding.Top + LayoutPadding.Bottom;

        BackgroundImage = ImageUtils.PaintDialogBase(_active, _width, _height, LayoutPadding,
            Images.ExtractBitmap(back, _active));

        _headerLabel = new AdvisorsHeaderLabel(this, $"{Labels.For(LabelIndex.DEFENSEMINISTER)}: {Labels.For(LabelIndex.Statistics)}")
        { Location = new(LayoutPadding.Left, LayoutPadding.Top + 2), Width = _width - PaddingSide };
        _tribeLabel = new AdvisorsHeaderLabel(this, $"{game.GetRealmName(_civ.Government)} {Labels.For(LabelIndex.of)}" +
            $" {LabelIndex.the} {_civ.TribeName}")
        { Location = new(LayoutPadding.Left, LayoutPadding.Top + 4 + _headerLabel.Height), Width = _width - PaddingSide };
        _titleLabel = new AdvisorsHeaderLabel(this, $"{_civ.LeaderTitle} {_civ.LeaderName}: {game.Date.GameYearString(game.TurnNumber)}")
        { Location = new(LayoutPadding.Left, LayoutPadding.Top + 6 + _headerLabel.Height + _tribeLabel.Height), Width = _width - PaddingSide };
        Controls.Add(_headerLabel);
        Controls.Add(_tribeLabel);
        Controls.Add(_titleLabel);

        var frontColorActive = new Color(255, 223, 79, 255);
        var shadowColorActive = Color.Black;
        var frontColorInProd = new Color(63, 187, 199, 255);
        var shadowColorInProd = Color.Black;

        List<ListboxGroup> liveGroups = [];
        List<ListboxGroup> deadGroups = [];
        var count = 0;
        for (var i = 0; i < game.Rules.UnitTypes.Length; i++)
        {
            var typeDef = game.Rules.UnitTypes[i];
            var image = _active.PicSources["unit"][i];
            var imgWidth = Images.GetImageWidth(image, _active);

            var liveUnits = _civ.Units.Where(u => !u.Dead).Count(u => u.Type == typeDef.Type);
            var unitsInProduction = _civ.Cities.Count(x => x.ItemInProduction.Title == typeDef.Name);

            var deadUnits = new int[8];

            if (liveUnits > 0 || unitsInProduction > 0)
            {
                var group = new ListboxGroup()
                {
                    Elements = [
                        new ListboxGroupElement { Icon = image, Xoffset = (count % 2 == 0) ? imgWidth : 0 },
                        new ListboxGroupElement { Text = typeDef.Name, Xoffset = 138, VerticalAlignment = VerticalAlignment.Center },
                        new ListboxGroupElement { Text = $"{typeDef.Attack}/{typeDef.Defense}/{typeDef.Move / 3}", Xoffset = 234,
                            VerticalAlignment = VerticalAlignment.Center },
                        new ListboxGroupElement { Text = $"{typeDef.Hitp / 10}/{typeDef.Firepwr}", Xoffset = 289, 
                            VerticalAlignment = VerticalAlignment.Center },
                        new ListboxGroupElement { Text = liveUnits > 0 ? $"{liveUnits} {Labels.For(LabelIndex.active)}" : "",
                            Xoffset = 321, VerticalAlignment = VerticalAlignment.Center, FrontColorOverride = frontColorActive, 
                            ShadowColorOverride = shadowColorActive },
                        new ListboxGroupElement { Text = unitsInProduction > 0 ? $"{unitsInProduction} {Labels.For(LabelIndex.inprod)}" : "",
                            Xoffset = 382, VerticalAlignment = VerticalAlignment.Center, FrontColorOverride = frontColorInProd,
                            ShadowColorOverride = shadowColorInProd },
                                ],
                    Height = 24
                };
                liveGroups.Add(group);
                count++;
            }
        }

        var defLive = new ListboxDefinition()
        {
            Rows = 12,
            Selectable = false,
            Looks = new ListboxLooks()
            {
                FontSize = 16,
                TextColorFront = new Color(223, 223, 223, 255),
                TextColorShadow = new Color(67, 67, 67, 255)
            },
            Groups = liveGroups
        };

        var defDead = new ListboxDefinition()
        {
            Rows = 12,
            Selectable = false,
            Looks = new ListboxLooks()
            {
                FontSize = 16,
                TextColorFront = new Color(223, 223, 223, 255),
                TextColorShadow = new Color(67, 67, 67, 255)
            },
            Groups = []
        };

        _liveListbox = new Listbox(this, defLive)
        {
            Width = 595,
            Height = 305,
            Location = new(LayoutPadding.Left + 2, LayoutPadding.Top + 80)
        };
        Controls.Add(_liveListbox);

        _deadListbox = new Listbox(this, defDead)
        {
            Width = 595,
            Height = 305,
            Location = new(LayoutPadding.Left + 2, LayoutPadding.Top + 80)
        };

        var btnWidth = (_width - PaddingSide - 6) / 2;
        _casualtBtn = new Button(this, Labels.For(LabelIndex.Casualties), _active.Look.ButtonFont, 18)
        {
            Location = new(LayoutPadding.Left + 2, _height - LayoutPadding.Bottom - 2 - 28),
            Width = btnWidth,
            Height = 28
        };
        _casualtBtn.Click += SwitchPanel;

        var okBtn = new Button(this, Labels.For(LabelIndex.OK), _active.Look.ButtonFont, 18)
        {
            Location = new(LayoutPadding.Left + 4 + btnWidth, _height - LayoutPadding.Bottom - 2 - 28),
            Width = btnWidth,
            Height = 28
        };
        okBtn.Click += (_, _) => _gameScreen.CloseDialog(this);

        Controls.Add(_casualtBtn);
        Controls.Add(okBtn);
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

    private void SwitchPanel(object? sender, MouseEventArgs e)
    {
        if (_casualtBtn.Text == Labels.For(LabelIndex.Casualties))
        {
            _casualtBtn.Text = Labels.For(LabelIndex.Statistics);
            _headerLabel.Text = $"{Labels.For(LabelIndex.DEFENSEMINISTER)}: {Labels.For(LabelIndex.Casualties)}";
            var index = Controls.IndexOf(_liveListbox);
            Controls[index] = _deadListbox;
        }
        else
        {
            _casualtBtn.Text = Labels.For(LabelIndex.Casualties);
            _headerLabel.Text = $"{Labels.For(LabelIndex.DEFENSEMINISTER)}: {Labels.For(LabelIndex.Statistics)}";
            var index = Controls.IndexOf(_deadListbox);
            Controls[index] = _liveListbox;
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
}
