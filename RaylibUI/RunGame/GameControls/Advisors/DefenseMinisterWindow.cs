using Civ2engine;
using Civ2engine.IO;
using Model;
using Model.Controls;
using Raylib_CSharp.Colors;
using RaylibUI.BasicTypes;
using RaylibUI.Controls;
using RaylibUtils;

namespace RaylibUI.RunGame.GameControls.Advisors;

public class DefenseMinisterWindow : AdvisorWindow
{
    private readonly Listbox _liveListbox, _deadListbox;
    private readonly Button _casualtBtn;

    public DefenseMinisterWindow(GameScreen gameScreen)
        : base(gameScreen, "defenseMinister", $"{Labels.For(LabelIndex.DEFENSEMINISTER)}: {Labels.For(LabelIndex.Statistics)}")
    {
        var frontColorActive = new Color(255, 223, 79, 255);
        var shadowColorActive = Color.Black;
        var frontColorInProd = new Color(63, 187, 199, 255);
        var shadowColorInProd = Color.Black;

        List<ListboxGroup> liveGroups = [];
        var count = 0;
        for (var i = 0; i < Game.Rules.UnitTypes.Length; i++)
        {
            var typeDef = Game.Rules.UnitTypes[i];
            var image = Active.PicSources["unit"][i];
            var imgWidth = Images.GetImageWidth(image, Active);

            var liveUnits = Civ.Units.Where(u => !u.Dead).Count(u => u.Type == typeDef.Type);
            var unitsInProduction = Civ.Cities.Count(x => x.ItemInProduction.Title == typeDef.Name);

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

        _liveListbox = MakeListbox();
        _liveListbox.Groups = liveGroups;
        Controls.Add(_liveListbox);

        _deadListbox = MakeListbox();

        _casualtBtn = AddButton(Labels.For(LabelIndex.Casualties), SwitchPanel);
        AddButton(Labels.For(LabelIndex.OK));
    }

    private Listbox MakeListbox() => new(this)
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
        }
    };

    private void SwitchPanel()
    {
        if (_casualtBtn.Text == Labels.For(LabelIndex.Casualties))
        {
            _casualtBtn.Text = Labels.For(LabelIndex.Statistics);
            HeaderLabel!.Text = $"{Labels.For(LabelIndex.DEFENSEMINISTER)}: {Labels.For(LabelIndex.Casualties)}";
            var index = Controls.IndexOf(_liveListbox);
            Controls[index] = _deadListbox;
        }
        else
        {
            _casualtBtn.Text = Labels.For(LabelIndex.Casualties);
            HeaderLabel!.Text = $"{Labels.For(LabelIndex.DEFENSEMINISTER)}: {Labels.For(LabelIndex.Statistics)}";
            var index = Controls.IndexOf(_deadListbox);
            Controls[index] = _liveListbox;
        }
    }
}
