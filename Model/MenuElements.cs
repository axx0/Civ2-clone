using Civ2engine;
using Model.Images;
using Model.Interface;

namespace Model;

public class MenuElements
{
    public List<Decoration> Decorations { get; } = new();
    public PopupBox Dialog { get; init; }
    public Point DialogPos { get; init; }
    public List<TextBoxDefinition>? TextBoxes { get; set; }
    public IList<int>? ReplaceNumbers { get; set; }
    public IList<bool>? CheckboxStates { get; set; }
    public int OptionsCols { get; set; } = 1;
}