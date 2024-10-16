using Civ2engine;
using Model.Images;
using Model.ImageSets;
using Model.Interface;
using Raylib_CSharp;

namespace Model.Dialog;

public class DialogElements
{
    public List<Decoration> Decorations { get; } = new();
    public PopupBox? Dialog { get; init; }
    public Point DialogPos { get; init; }
    public List<TextBoxDefinition>? TextBoxes { get; set; }
    
    public ListBoxDefinition? ListBox { get; set; }
    public IList<int>? ReplaceNumbers { get; set; }
    public IList<bool>? CheckboxStates { get; set; }
    public int OptionsCols { get; set; } = 1;
    public int SelectedOption { get; set; } = 0;
    public List<string>? ReplaceStrings { get; set; }
    public IImageSource[]? OptionsIcons { get; set; }
    public DialogImageElements? Image { get; set; }
}