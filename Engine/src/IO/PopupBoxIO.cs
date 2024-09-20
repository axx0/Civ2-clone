using System.Collections.Generic;
using Model.Core;

namespace Civ2engine.IO;

public static class PopupBoxIO
{
    public static void AddText(this PopupBox box, string line)
    {
        if (line.StartsWith("^^"))
        {
            (box.LineStyles ??= new List<TextStyles>()).Add(TextStyles.Centered);
            (box.Text ??= new List<string>()).Add(line[2..]);
        }
        else if (line.StartsWith("^"))
        {
            (box.LineStyles ??= new List<TextStyles>()).Add(TextStyles.LeftOwnLine);
            (box.Text ??= new List<string>()).Add(line[1..]);
        }
        else
        {
            (box.LineStyles ??= new List<TextStyles>()).Add(TextStyles.Left);
            (box.Text ??= new List<string>()).Add(line);
        }
    }
}