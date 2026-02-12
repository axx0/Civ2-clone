using Civ2engine;
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

public class CityStatusWindow : BaseDialog
{
    private readonly IUserInterface _active;
    private readonly int _width, _height;
    private readonly GameScreen _gameScreen;
    private readonly Civilization _civ;

    public CityStatusWindow(GameScreen gameScreen) : base(gameScreen.Main)
    {
        _gameScreen = gameScreen;
        _active = gameScreen.MainWindow.ActiveInterface;
        var game = gameScreen.Game;
        _civ = game.GetPlayerCiv;

        LayoutPadding = _active.GetPadding(0, false);

        var back = _active.PicSources["cityReport"][0];
        _width = Images.GetImageWidth(back, _active) + PaddingSide;
        _height = Images.GetImageHeight(back, _active) + LayoutPadding.Top + LayoutPadding.Bottom;

        BackgroundImage = ImageUtils.PaintDialogBase(_active, _width, _height, LayoutPadding,
            Images.ExtractBitmap(back, _active));

        var headerLabel = new AdvisorsHeaderLabel(this, Labels.For(LabelIndex.CITYSTATUS))
        { Location = new(LayoutPadding.Left, LayoutPadding.Top + 2), Width = _width - PaddingSide };
        var tribeLabel = new AdvisorsHeaderLabel(this, $"{game.GetRealmName(_civ.Government)} {Labels.For(LabelIndex.of)}" +
            $" {LabelIndex.the} {_civ.TribeName}")
        { Location = new(LayoutPadding.Left, LayoutPadding.Top + 4 + headerLabel.Height), Width = _width - PaddingSide };
        var titleLabel = new AdvisorsHeaderLabel(this, $"{_civ.LeaderTitle} {_civ.LeaderName}: {game.Date.GameYearString(game.TurnNumber)}")
        { Location = new(LayoutPadding.Left, LayoutPadding.Top + 6 + headerLabel.Height + tribeLabel.Height), Width = _width - PaddingSide };
        Controls.Add(headerLabel);
        Controls.Add(tribeLabel);
        Controls.Add(titleLabel);

        var cityStyleIndex = _active.GetCityStyleIndexFromEpoch(_civ.CityStyle, _civ.Epoch);

        List<ListboxGroup> groups = [];
        for (var i = 0; i < _civ.Cities.Count; i++)
        {
            var city = _civ.Cities[i];
            var sizeIncrement = _active.GetCityIndexForStyle(cityStyleIndex, city, city.Size);
            var cityImage = _active.CityImages.Sets[cityStyleIndex][sizeIncrement];
            var cityImgWidth = Images.GetImageWidth(cityImage.Image, _active);
            Color? frontColor = null;
            Color? shadowColor = null;
            if (city.ItemInProduction.Type == ItemType.Unit)
            {
                frontColor = new Color(63, 187, 199, 255);
                shadowColor = Color.Black;
            }
            // TODO: wonders have seperate color

            var group = new ListboxGroup()
            {
                Elements = [new ListboxGroupElement { Icon = cityImage.Image, Xoffset = (i % 2 == 0) ? cityImgWidth : 0 },
                            new ListboxGroupElement { Text = city.Name, Xoffset = 138, VerticalAlignment = VerticalAlignment.Center },
                            new ListboxGroupElement { Text = city.Food.ToString(), Xoffset = 243, VerticalAlignment = VerticalAlignment.Center },
                            new ListboxGroupElement { Icon = _active.ResourceImages.First(i => i.Name == "Food").LargeImage, Xoffset = 259 },
                            new ListboxGroupElement { Text = city.Production.ToString(), VerticalAlignment = VerticalAlignment.Center },
                            new ListboxGroupElement { Icon = _active.ResourceImages.First(i => i.Name == "Shields").LargeImage },
                            new ListboxGroupElement { Text = city.Trade.ToString(), VerticalAlignment = VerticalAlignment.Center },
                            new ListboxGroupElement { Icon = _active.ResourceImages.First(i => i.Name == "Trade").LargeImage },
                            new ListboxGroupElement { Text = city.ItemInProduction.Title, Xoffset = 363, 
                                VerticalAlignment = VerticalAlignment.Center, FrontColorOverride = frontColor, 
                                ShadowColorOverride = shadowColor },
                            new ListboxGroupElement { Text = $"   ({city.ShieldsProgress}/{10 * city.ItemInProduction.Cost})", 
                                VerticalAlignment = VerticalAlignment.Center, FrontColorOverride = new Color(191, 191, 191, 255) }
                            ],
                Height = 24
            };
            groups.Add(group);
        }

        var def = new ListboxDefinition()
        {
            Rows = 12,
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
            Width = 595,
            Height = 305,
            Location = new(LayoutPadding.Left + 2, LayoutPadding.Top + 80)
        };
        listbox.ItemSelected += (_, i) => _gameScreen.ShowDialog(new CityWindow(_gameScreen, _civ.Cities[i.Index]));
        Controls.Add(listbox);

        var btn = new Button(this, Labels.For(LabelIndex.OK), _active.Look.ButtonFont, 18)
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
