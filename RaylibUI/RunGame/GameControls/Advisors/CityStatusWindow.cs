using Civ2engine;
using Civ2engine.IO;
using Model;
using Model.Controls;
using Model.Core.Production;
using Raylib_CSharp.Colors;
using RaylibUI.BasicTypes;
using RaylibUI.RunGame.GameControls.CityControls;
using RaylibUtils;

namespace RaylibUI.RunGame.GameControls.Advisors;

public class CityStatusWindow : AdvisorWindow
{
    public CityStatusWindow(GameScreen gameScreen)
        : base(gameScreen, "cityReport", Labels.For(LabelIndex.CITYSTATUS))
    {
        var cityStyleIndex = Active.GetCityStyleIndexFromEpoch(Civ.CityStyle, Civ.Epoch);

        List<ListboxGroup> groups = [];
        for (var i = 0; i < Civ.Cities.Count; i++)
        {
            var city = Civ.Cities[i];
            var sizeIncrement = Active.GetCityIndexForStyle(cityStyleIndex, city, city.Size);
            var cityImage = Active.CityImages.Sets[cityStyleIndex][sizeIncrement];
            var cityImgWidth = Images.GetImageWidth(cityImage.Image, Active);
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
                            new ListboxGroupElement { Icon = Active.ResourceImages.First(i => i.Name == "Food").LargeImage, Xoffset = 259 },
                            new ListboxGroupElement { Text = city.Production.ToString(), VerticalAlignment = VerticalAlignment.Center },
                            new ListboxGroupElement { Icon = Active.ResourceImages.First(i => i.Name == "Shields").LargeImage },
                            new ListboxGroupElement { Text = city.Trade.ToString(), VerticalAlignment = VerticalAlignment.Center },
                            new ListboxGroupElement { Icon = Active.ResourceImages.First(i => i.Name == "Trade").LargeImage },
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
        listbox.ItemSelected += (_, i) => GameScreen.ShowDialog(new CityWindow(GameScreen, Civ.Cities[i.Index]));
        Controls.Add(listbox);

        AddButton(Labels.For(LabelIndex.OK));
    }
}
