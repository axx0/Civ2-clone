
using Civ2engine;
using Civ2engine.IO;
using Civ2engine.Production;
using Model;
using Model.Controls;
using Raylib_CSharp.Colors;
using Raylib_CSharp.Interact;
using RaylibUI.BasicTypes;
using RaylibUI.Controls;
using RaylibUI.RunGame.GameControls.CityControls;
using RaylibUtils;

namespace RaylibUI.RunGame.GameControls;

public class WondersWindow : BaseDialog
{
    private readonly IUserInterface _active;
    private readonly int _width, _height;
    private readonly GameScreen _gameScreen;
    private readonly Civilization _civ;

    public WondersWindow(GameScreen gameScreen) : base(gameScreen.Main)
    {
        _gameScreen = gameScreen;
        _active = gameScreen.MainWindow.ActiveInterface;
        var game = gameScreen.Game;
        _civ = game.GetPlayerCiv;

        LayoutPadding = _active.GetPadding(0, false);

        var back = _active.PicSources["worldWonders"][0];
        _width = Images.GetImageWidth(back, _active) + PaddingSide;
        _height = Images.GetImageHeight(back, _active) + LayoutPadding.Top + LayoutPadding.Bottom;

        BackgroundImage = ImageUtils.PaintDialogBase(_active, _width, _height, LayoutPadding,
            Images.ExtractBitmap(back, _active));

        var wonders = game.Rules.Improvements.Where(i => i.IsWonder).ToList();

        List<ListboxGroup> groups = [];
        for (var i = 0; i < _civ.Cities.Count; i++)
        {
            Color? frontColor = null;
            Color? shadowColor = null;

            var group = new ListboxGroup()
            {
                Elements = [],
                Height = 24
            };
            groups.Add(group);
        }

        var def = new ListboxDefinition()
        {
            Rows = 9,
            Selectable = false,
            Looks = new ListboxLooks()
            {
                FontSize = 16,
                TextColorFront = new Color(223, 223, 223, 255),
                TextColorShadow = new Color(67, 67, 67, 255)
            },
            Groups = groups
        };

        var listbox = new Listbox(this, def)
        {
            Width = _width - PaddingSide - 2 * 2,
            Height = 370,
            Location = new(LayoutPadding.Left + 2, LayoutPadding.Top + 2)
        };
        Controls.Add(listbox);

        var btn = new Button(this, Labels.For(LabelIndex.Close), _active.Look.ButtonFont, 18)
        {
            Location = new(LayoutPadding.Left + 2, _height - LayoutPadding.Bottom - 2 - 28),
            Width = _width - PaddingSide - 4,
            Height = 28
        };
        btn.Click += (_, _) => _gameScreen.CloseDialog(this); ;
        Controls.Add(btn);
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
}
