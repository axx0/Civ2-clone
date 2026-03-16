using Civ2engine;
using Model.Controls;
using Raylib_CSharp.Fonts;
using RaylibUI.BasicTypes;
using RaylibUI.BasicTypes.Controls;

namespace RaylibUI.RunGame.GameControls;

public class CivilopediaDescription : Listbox
{
    public CivilopediaDescription(CivilopediaWindow window, GameScreen gameScreen, Civilopedia pedia, int id) : base(window)
    {
        Width = window.Width - window.LayoutPadding.Left - window.LayoutPadding.Right - 4;
        Height = window.Height - window.LayoutPadding.Top - window.LayoutPadding.Bottom - 4;
        Location = new System.Numerics.Vector2(window.LayoutPadding.Left + 2, window.LayoutPadding.Top + 2);
        
        var active = gameScreen.MainWindow.ActiveInterface;

        string text = CivilopediaLoader.GetDescription(pedia, id);
        var wrappedTexts = DialogUtils.GetWrappedTexts(text, Width, active.Look.LabelFont, active.Look.CivilopediaFontSize);
        var textHeight = (int)TextManager.MeasureTextEx(active.Look.LabelFont, text, active.Look.CivilopediaFontSize, 0.0f).Y;
        var rows = Height / textHeight;
        if (wrappedTexts.Count > rows)
        {
            wrappedTexts = DialogUtils.GetWrappedTexts(text, Width - ScrollBar.ScrollbarDimDefault, 
                active.Look.LabelFont, active.Look.CivilopediaFontSize);
        }

        List<ListboxGroup> groups = [];
        foreach (var txt in wrappedTexts)
        {
            groups.Add(new ListboxGroup(txt));
        }

        Definition = new ListboxDefinition
        {
            Groups = groups,
            Rows = rows,
            Selectable = false,
            Type = ListboxType.Civilopedia
        };
    }
}
