using Civ2engine;
using Civ2engine.IO;
using Model;
using Model.Controls;
using Raylib_CSharp.Colors;
using Raylib_CSharp.Rendering;
using RaylibUI.BasicTypes;
using RaylibUI.RunGame.GameControls.Civilopedia;

namespace RaylibUI.RunGame.GameControls.Advisors;

public class ScienceAdvisorWindow : AdvisorWindow
{
    public ScienceAdvisorWindow(GameScreen gameScreen)
        : base(gameScreen, "scienceAdvisor", Labels.For(LabelIndex.SCIENCEADVISOR))
    {
        if (Civ.ReseachingAdvance != -1)
        {
            var advance = Game.Rules.Advances[Civ.ReseachingAdvance];
            Controls.Add(new AdvisorsHeaderLabel(this, $"{Labels.For(LabelIndex.Researching)}: {advance.Name}")
            { Location = new(LayoutPadding.Left, LastHeaderLabel!.Location.Y + 27), Width = InnerWidth });
        }

        List<ListboxGroup> groups = [];
        var allAdvances = Game.Rules.Advances;
        for (var i = 0; i < allAdvances.Length; i++)
        {
            if (!Civ.Advances[i]) continue;

            var advance = allAdvances[i];
            var icon = Active.PicSources["advanceCategories"][5 * advance.Epoch + advance.KnowledgeCategory];
            var group = new ListboxGroup()
            {
                Elements = [new ListboxGroupElement { Icon = icon },
                            new ListboxGroupElement { Text = advance.Name, VerticalAlignment = VerticalAlignment.Center },
                            ],
                Height = 22
            };
            groups.Add(group);
        }

        var listbox = new Listbox(this)
        {
            Width = 596,
            Height = 240,
            Location = new(LayoutPadding.Left + 2, LayoutPadding.Top + 130),
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
        listbox.ItemSelected += (_, i) =>
        {
            // fix this because it can chose wrong advance
            GameScreen.ShowDialog(new CivilopediaWindow(GameScreen,
                new(CivilopediaInfoType.Advances, CivilopediaWindowType.Info, i.Index)));
        };
        Controls.Add(listbox);

        AddButton(Labels.For(LabelIndex.Goal), () => { });
        AddButton(Labels.For(LabelIndex.Close));
    }

    public override void Draw(bool pulse)
    {
        base.Draw(pulse);

        Graphics.DrawRectangleLinesEx(new(Location.X + 2 + LayoutPadding.Left, Location.Y + 71 + LayoutPadding.Top,
            Width - 4 - PaddingSide, 51), 2f, Color.White);
    }
}
