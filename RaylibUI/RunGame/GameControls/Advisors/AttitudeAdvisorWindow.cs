using Civ2engine;
using Civ2engine.IO;
using Model;
using Model.Controls;
using Raylib_CSharp.Colors;
using RaylibUI.BasicTypes;
using RaylibUI.RunGame.GameControls.CityControls;
using RaylibUtils;

namespace RaylibUI.RunGame.GameControls.Advisors;

public class AttitudeAdvisorWindow : AdvisorWindow
{
    public AttitudeAdvisorWindow(GameScreen gameScreen)
        : base(gameScreen, "attitudeAdvisor", Labels.For(LabelIndex.ATTITUDEADVISOR))
    {
        var cityStyleIndex = Active.GetCityStyleIndexFromEpoch(Civ.CityStyle, Civ.Epoch);

        List<ListboxGroup> groups = [];
        for (var i = 0; i < Civ.Cities.Count; i++)
        {
            var city = Civ.Cities[i];
            var sizeIncrement = Active.GetCityIndexForStyle(cityStyleIndex, city, city.Size);
            var cityImage = Active.CityImages.Sets[cityStyleIndex][sizeIncrement];
            var cityImgWidth = Images.GetImageWidth(cityImage.Image, Active);

            List<ListboxGroupElement> lists =
            [
                new ListboxGroupElement { Icon = cityImage.Image, Xoffset = (i % 2 == 0) ? cityImgWidth : 0 },
                new ListboxGroupElement { Text = city.Name, Xoffset = 138, VerticalAlignment = VerticalAlignment.Center },
            ];
            for (var j = 0; j < city.Size; j++)
            {
                lists.Add(new ListboxGroupElement { Icon = Active.PicSources["people"][0], Xoffset = 248 + 5 * j });
            }

            groups.Add(new ListboxGroup()
            {
                Elements = lists,
                Height = 32
            });
        }

        var listbox = new Listbox(this)
        {
            Width = 595,
            Height = 305,
            Location = new(LayoutPadding.Left + 2, LayoutPadding.Top + 80),
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
        listbox.ItemSelected += (_, i) => GameScreen.ShowDialog(new CityWindow(GameScreen, Civ.Cities[i.Index]));
        Controls.Add(listbox);

        AddButton(Labels.For(LabelIndex.OK));
    }
}
