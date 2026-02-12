using Civ2engine;
using Model;
using Model.Controls;
using Raylib_CSharp.Colors;
using Raylib_CSharp.Interact;
using RaylibUI.BasicTypes;
using RaylibUI.Controls;
using RaylibUtils;

namespace RaylibUI.RunGame.GameControls;

public class TradeAdvisorWindow : BaseDialog
{
    private readonly IUserInterface _active;
    private readonly int _width, _height;
    private readonly GameScreen _gameScreen;
    private readonly Civilization _civ;

    public TradeAdvisorWindow(GameScreen gameScreen) : base(gameScreen.Main)
    {
        _gameScreen = gameScreen;
        _active = gameScreen.MainWindow.ActiveInterface;
        var game = gameScreen.Game;
        _civ = game.GetPlayerCiv;

        LayoutPadding = _active.GetPadding(0, false);

        var back = _active.PicSources["tradeAdvisor"][0];
        _width = Images.GetImageWidth(back, _active) + PaddingSide;
        _height = Images.GetImageHeight(back, _active) + LayoutPadding.Top + LayoutPadding.Bottom;

        BackgroundImage = ImageUtils.PaintDialogBase(_active, _width, _height, LayoutPadding,
            Images.ExtractBitmap(back, _active));

        var headerLabel = new AdvisorsHeaderLabel(this, Labels.For(LabelIndex.TRADEADVISOR))
        { Location = new(LayoutPadding.Left, LayoutPadding.Top + 2), Width = _width - PaddingSide };
        var tribeLabel = new AdvisorsHeaderLabel(this, $"{game.GetRealmName(_civ.Government)} {Labels.For(LabelIndex.of)}" +
            $" {LabelIndex.the} {_civ.TribeName}")
        { Location = new(LayoutPadding.Left, LayoutPadding.Top + 4 + headerLabel.Height), Width = _width - PaddingSide };
        var titleLabel = new AdvisorsHeaderLabel(this, $"{_civ.LeaderTitle} {_civ.LeaderName}: {game.Date.GameYearString(game.TurnNumber)}")
        { Location = new(LayoutPadding.Left, LayoutPadding.Top + 6 + headerLabel.Height + tribeLabel.Height), Width = _width - PaddingSide };
        Controls.Add(headerLabel);
        Controls.Add(tribeLabel);
        Controls.Add(titleLabel);


        // Get no of cities which have improvements with upkeep
        List<(Improvement impr, int noCities)> upkeepList = [];
        var totalCost = 0;
        foreach (var improvement in game.Rules.Improvements)
        {
            var citiesWithImprovement = _civ.Cities.Count(c => c.Improvements.Any(i => i == improvement));
            if (improvement.Upkeep > 0 && citiesWithImprovement > 0)
            {
                upkeepList.Add((improvement, citiesWithImprovement));
                totalCost += improvement.Upkeep * citiesWithImprovement;
            }
        }

        List<ListboxGroup> groups = [];
        
        groups.Add(new ListboxGroup()
        {
            Elements = [new ListboxGroupElement { Text = $"{Labels.For(LabelIndex.CityTrade)}", Xoffset = 138, TextSizeOverride = 18,
                VerticalAlignment = VerticalAlignment.Center, FrontColorOverride = Color.White, ShadowColorOverride = Color.Black },
            new ListboxGroupElement { Text = $"{Labels.For(LabelIndex.MaintenanceCosts)}", Xoffset = 333, TextSizeOverride = 18,
                VerticalAlignment = VerticalAlignment.Center, FrontColorOverride = Color.White, ShadowColorOverride = Color.Black }],
            Height = 24
        });

        var cityStyleIndex = _active.GetCityStyleIndexFromEpoch(_civ.CityStyle, _civ.Epoch);
        for (var i = 0; i < Math.Max(upkeepList.Count + 2, _civ.Cities.Count); i++)
        {
            List<ListboxGroupElement> elements = [];

            if (i < _civ.Cities.Count)
            {
                var city = _civ.Cities[i];
                var sizeIncrement = _active.GetCityIndexForStyle(cityStyleIndex, city, city.Size);
                var cityImage = _active.CityImages.Sets[cityStyleIndex][sizeIncrement];
                var cityImgWidth = Images.GetImageWidth(cityImage.Image, _active);

                elements.Add(new() { Icon = cityImage.Image, Xoffset = (i % 2 == 0) ? cityImgWidth : 0 });
                elements.Add(new() { Text = city.Name, Xoffset = 138, VerticalAlignment = VerticalAlignment.Center });
                elements.Add(new() { Text = city.GetTax().ToString(), Xoffset = 243, VerticalAlignment = VerticalAlignment.Center });
                elements.Add(new() { Icon = _active.PicSources["gold,large"][0] });
                elements.Add(new() { Text = $" {city.GetScience()}", VerticalAlignment = VerticalAlignment.Center });
                elements.Add(new() { Icon = _active.PicSources["science,large"][0] });
            }
            
            if (i < upkeepList.Count)
            {
                var (impr, noCities) = upkeepList[i];
                elements.Add(new() { Text = $"{noCities} {impr.Name} ({Labels.For(LabelIndex.Cost)}: {impr.Upkeep * noCities}", 
                    Xoffset = 333, VerticalAlignment = VerticalAlignment.Center, FrontColorOverride = new Color(255, 223, 79, 255),
                    ShadowColorOverride = Color.Black});
                elements.Add(new() { Icon = _active.PicSources["gold,large"][0] });
                elements.Add(new() { Text = ")", VerticalAlignment = VerticalAlignment.Center, 
                    FrontColorOverride = new Color(255, 223, 79, 255), ShadowColorOverride = Color.Black });
            }

            if (i == upkeepList.Count + 1)
            {

                elements.Add(new() { Text = $"{Labels.For(LabelIndex.TotalCost)}: {totalCost}", Xoffset = 333, 
                    VerticalAlignment = VerticalAlignment.Center, FrontColorOverride = new Color(255, 223, 79, 255), 
                    ShadowColorOverride = Color.Black });
                elements.Add(new() { Icon = _active.PicSources["gold,large"][0] });
            }

            var group = new ListboxGroup()
            {
                Elements = elements,
                Height = 24
            };
            groups.Add(group);
        }

        // Total costs, income, etc.
        groups.Add(new ListboxGroup() { Elements = [], Height = 24 });
        groups.Add(new ListboxGroup()
        {
            Elements = [new() { Text = $"{Labels.For(LabelIndex.TotalCost)}: {totalCost}", Xoffset = 138,
                    VerticalAlignment = VerticalAlignment.Center, FrontColorOverride = Color.White, ShadowColorOverride = Color.Black },
                    new() { Icon = _active.PicSources["gold,large"][0] }],
            Height = 24
        });
        var totalIncome = _civ.Cities.Sum(c => c.GetTax());
        groups.Add(new ListboxGroup()
        {
            Elements = [new() { Text = $"{Labels.For(LabelIndex.TotalIncome)}: {totalIncome}", Xoffset = 138,
                    VerticalAlignment = VerticalAlignment.Center, FrontColorOverride = Color.White, ShadowColorOverride = Color.Black },
                    new() { Icon = _active.PicSources["gold,large"][0] }],
            Height = 24
        });
        var totalScience = _civ.Cities.Sum(c => c.GetScience());
        groups.Add(new ListboxGroup()
        {
            Elements = [new() { Text = $"{Labels.For(LabelIndex.TotalScience)}: {totalScience}", Xoffset = 138,
                    VerticalAlignment = VerticalAlignment.Center, FrontColorOverride = Color.White, ShadowColorOverride = Color.Black },
                    new() { Icon = _active.PicSources["science,large"][0] }],
            Height = 24
        });
        groups.Add(new ListboxGroup()
        {
            Elements = [new() { Text = $"{Labels.For(LabelIndex.Discoveries)}: xx {Labels.For(LabelIndex.Turns)}", Xoffset = 138,
                    VerticalAlignment = VerticalAlignment.Center, FrontColorOverride = Color.White, ShadowColorOverride = Color.Black } ],
            Height = 24
        });
        groups.Add(new ListboxGroup() { Elements = [], Height = 24 });
        groups.Add(new ListboxGroup() { Elements = [], Height = 24 });


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
        Controls.Add(listbox);

        var btnWidth = (_width - PaddingSide - 6) / 2;
        var supplyBtn = new Button(this, Labels.For(LabelIndex.SupplyandDemand), _active.Look.ButtonFont, 18)
        {
            Location = new(LayoutPadding.Left + 2, _height - LayoutPadding.Bottom - 2 - 28),
            Width = btnWidth,
            Height = 28
        };
        supplyBtn.Click += (_, _) => { };

        var okBtn = new Button(this, Labels.For(LabelIndex.OK), _active.Look.ButtonFont, 18)
        {
            Location = new(LayoutPadding.Left + 4 + btnWidth, _height - LayoutPadding.Bottom - 2 - 28),
            Width = btnWidth,
            Height = 28
        };
        okBtn.Click += (_, _) => _gameScreen.CloseDialog(this);

        Controls.Add(supplyBtn);
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

    public override void OnKeyPress(KeyboardKey key)
    {
        if (key == KeyboardKey.Escape || key == KeyboardKey.Enter || key == KeyboardKey.KpEnter)
        {
            _gameScreen.CloseDialog(this);
        }
        base.OnKeyPress(key);
    }
}
