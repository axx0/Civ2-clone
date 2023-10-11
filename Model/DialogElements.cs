using Civ2engine;
using Model.Images;
using Model.ImageSets;
using Model.Interface;
using Raylib_cs;

namespace Model;

public class DialogElements
{
    public List<Decoration> Decorations { get; } = new();
    public PopupBox Dialog { get; init; }
    public Point DialogPos { get; init; }
    public List<TextBoxDefinition>? TextBoxes { get; set; }
    public ListBoxDefinition? ListBox { get; set; }
    public IList<int>? ReplaceNumbers { get; set; }
    public IList<bool>? CheckboxStates { get; set; }
    public int OptionsCols { get; set; } = 1;
    public List<string>? ReplaceStrings { get; set; }
    public Image[]? OptionsImages { get; set; }
}