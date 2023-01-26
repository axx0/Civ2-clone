using System.Drawing;
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
}