using Civ2engine;
using Civ2engine.IO;
using Model;
using Model.Controls;
using Model.Core;
using Model.Core.Cities;
using Raylib_CSharp.Colors;
using Raylib_CSharp.Interact;
using RaylibUI.BasicTypes;
using RaylibUI.Controls;
using RaylibUtils;

namespace RaylibUI.RunGame.GameControls.Advisors;

public class TradeAdvisorWindow : AdvisorWindow
{
    public TradeAdvisorWindow(GameScreen gameScreen)
        : base(gameScreen, "tradeAdvisor", Labels.For(LabelIndex.TRADEADVISOR))
    {

        // Get no of cities which have improvements with upkeep
        List<(Improvement impr, int noCities)> upkeepList = [];
        var totalCost = 0;
        foreach (var improvement in Game.Rules.Improvements)
        {
            var citiesWithImprovement = Civ.Cities.Count(c => c.Improvements.Any(i => i == improvement));
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

        var cityStyleIndex = Active.GetCityStyleIndexFromEpoch(Civ.CityStyle, Civ.Epoch);
        for (var i = 0; i < Math.Max(upkeepList.Count + 2, Civ.Cities.Count); i++)
        {
            List<ListboxGroupElement> elements = [];

            if (i < Civ.Cities.Count)
            {
                var city = Civ.Cities[i];
                var sizeIncrement = Active.GetCityIndexForStyle(cityStyleIndex, city, city.Size);
                var cityImage = Active.CityImages.Sets[cityStyleIndex][sizeIncrement];
                var cityImgWidth = Images.GetImageWidth(cityImage.Image, Active);

                elements.Add(new() { Icon = cityImage.Image, Xoffset = (i % 2 == 0) ? cityImgWidth : 0 });
                elements.Add(new() { Text = city.Name, Xoffset = 138, VerticalAlignment = VerticalAlignment.Center });
                elements.Add(new() { Text = city.GetTax().ToString(), Xoffset = 243, VerticalAlignment = VerticalAlignment.Center });
                elements.Add(new() { Icon = Active.PicSources["gold,large"][0] });
                elements.Add(new() { Text = $" {city.GetScience()}", VerticalAlignment = VerticalAlignment.Center });
                elements.Add(new() { Icon = Active.PicSources["science,large"][0] });
            }
            
            if (i < upkeepList.Count)
            {
                var (impr, noCities) = upkeepList[i];
                elements.Add(new() { Text = $"{noCities} {impr.Name} ({Labels.For(LabelIndex.Cost)}: {impr.Upkeep * noCities}", 
                    Xoffset = 333, VerticalAlignment = VerticalAlignment.Center, FrontColorOverride = new Color(255, 223, 79, 255),
                    ShadowColorOverride = Color.Black});
                elements.Add(new() { Icon = Active.PicSources["gold,large"][0] });
                elements.Add(new() { Text = ")", VerticalAlignment = VerticalAlignment.Center, 
                    FrontColorOverride = new Color(255, 223, 79, 255), ShadowColorOverride = Color.Black });
            }

            if (i == upkeepList.Count + 1)
            {

                elements.Add(new() { Text = $"{Labels.For(LabelIndex.TotalCost)}: {totalCost}", Xoffset = 333, 
                    VerticalAlignment = VerticalAlignment.Center, FrontColorOverride = new Color(255, 223, 79, 255), 
                    ShadowColorOverride = Color.Black });
                elements.Add(new() { Icon = Active.PicSources["gold,large"][0] });
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
                    new() { Icon = Active.PicSources["gold,large"][0] }],
            Height = 24
        });
        var totalIncome = Civ.Cities.Sum(c => c.GetTax());
        groups.Add(new ListboxGroup()
        {
            Elements = [new() { Text = $"{Labels.For(LabelIndex.TotalIncome)}: {totalIncome}", Xoffset = 138,
                    VerticalAlignment = VerticalAlignment.Center, FrontColorOverride = Color.White, ShadowColorOverride = Color.Black },
                    new() { Icon = Active.PicSources["gold,large"][0] }],
            Height = 24
        });
        var totalScience = Civ.Cities.Sum(c => c.GetScience());
        groups.Add(new ListboxGroup()
        {
            Elements = [new() { Text = $"{Labels.For(LabelIndex.TotalScience)}: {totalScience}", Xoffset = 138,
                    VerticalAlignment = VerticalAlignment.Center, FrontColorOverride = Color.White, ShadowColorOverride = Color.Black },
                    new() { Icon = Active.PicSources["science,large"][0] }],
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

        var listbox = new Listbox(this)
        {
            Width = 595,
            Height = 305,
            Location = new(LayoutPadding.Left + 2, LayoutPadding.Top + 80),
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
        Controls.Add(listbox);

        AddButton(Labels.For(LabelIndex.SupplyandDemand), () => { });
        AddButton(Labels.For(LabelIndex.OK));
    }
}
