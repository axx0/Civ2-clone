using Civ2engine;
using Model.Controls;
using RaylibUI.BasicTypes;
using RaylibUI.BasicTypes.Controls;
using RaylibUtils;

namespace RaylibUI.RunGame.GameControls.CityControls;

public class ImprovementsBox : Listbox
{
    private readonly CityWindow _cityWindow;
    private float _oldScale = 0f;

    public ImprovementsBox(CityWindow cityWindow) : base(cityWindow)
    {
        _cityWindow = cityWindow;
        ItemSelected += SellSelectedImprovement;
    }

    public override void OnResize()
    {
        if (_oldScale != _cityWindow.Scale)
        {
            Definition = MakeListbox(_cityWindow);
            _oldScale = _cityWindow.Scale;
        }

        var pos = _cityWindow.CityWindowProps.Improvements.Box;
        Location = new(_cityWindow.LayoutPadding.Left + pos.X * _cityWindow.Scale,
            _cityWindow.LayoutPadding.Top + pos.Y * _cityWindow.Scale);
        Width = (int)(pos.Width * _cityWindow.Scale + ScrollBar.ScrollBarDim);
        Height = (int)(pos.Height * _cityWindow.Scale);

        base.OnResize();
    }

    static ListboxDefinition MakeListbox(CityWindow cityWindow)
    {
        var active = cityWindow.MainWindow.ActiveInterface;
        var improvements = cityWindow.City.Improvements;
        var properties = cityWindow.CityWindowProps.Improvements;
        var firstWonderIndex = cityWindow.CurrentGameScreen.Game.Rules.FirstWonderIndex;

        List<ListboxGroup> groups = new();
        foreach (var improvement in improvements)
        {
            var icon = improvement.Icon ?? active.GetImprovementImage(improvement, firstWonderIndex);
            var iconScale = 0.5f * cityWindow.Scale;
            var iconWidth = Images.GetImageWidth(icon, active, iconScale);

            var group = new ListboxGroup()
            {
                Elements = [new ListboxGroupElement { Icon = icon, ScaleIcon = iconScale, Xoffset = (int)(3 * cityWindow.Scale) },
                            new ListboxGroupElement { Text = improvement.Name, Xoffset = iconWidth + (int)(8 * cityWindow.Scale) },
                            new ListboxGroupElement { Icon = active.PicSources["gold,large"][0],
                                                      ScaleIcon = 12f / 14f * cityWindow.Scale, Xoffset = (int)(149 * cityWindow.Scale) }],
                Height = (int)(12 * cityWindow.Scale)
            };
            groups.Add(group);
        }

        return new ListboxDefinition()
        {
            Rows = properties.Rows,
            Selectable = false,
            Looks = new ListboxLooks()
            {
                Font = active.Look.CityWindowFont,
                FontSize = active.Look.CityWindowFontSize + (int)(12 * (cityWindow.Scale - 1)),
                TextColorFront = properties.LabelColor,
                TextColorShadow = properties.LabelColorShadow,
                TextShadowOffset = properties.ShadowOffset
            },
            Groups = groups
        };
    }

    private void SellSelectedImprovement(object? sender, ListboxSelectionEventArgs args)
    {
        var city = _cityWindow.City;
        var screen = _cityWindow.CurrentGameScreen;
        var active = _cityWindow.MainWindow.ActiveInterface;
        var improvement = city.Improvements[args.Index];

        if (city.ImprovementSold)
        {
            screen.ShowPopup("ALREADYSOLD");
        }
        else if (city.Improvements[args.Index].Type == 1)   // Can't sell palace
        {
            screen.ShowPopup("CANTHOCKTHIS", dialogImage: new([improvement.Icon ??
                    active.GetImprovementImage(improvement, screen.Game.Rules.FirstWonderIndex)]));
        }
        else
        {
            screen.ShowPopup("HOCKTHIS",
                handleButtonClick: (button, i, arg3, arg4) =>
                {
                    if (i == 0 && button == Labels.Ok)
                    {
                        city.SellImprovement(improvement);
                        Definition = MakeListbox(_cityWindow);
                        OnResize();
                    }
                },
                dialogImage: new([improvement.Icon ??
                    active.GetImprovementImage(improvement, screen.Game.Rules.FirstWonderIndex)]),
                replaceStrings: [improvement.Name],
                replaceNumbers: [improvement.Cost * 10]);
        }
    }
}
