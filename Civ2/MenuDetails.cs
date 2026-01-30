using Model;
using Model.Controls;

namespace Civ2;

public class MenuDetails
{
    public string Key { get; init; }
    public IList<MenuElement> Defaults { get;init; }
    public int[] SeparatorRows { get; init; }
}