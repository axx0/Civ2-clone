using System.Collections.Generic;
using System.Dynamic;

namespace Civ2engine
{
    public class PopupBox
    {
        public string Name { get; set; }
        public int Width { get; set; }
        public string Title { get; set; }
        public int Default { get; set; }
        public int? X { get; set; }
        public int? Y { get; set; }
        public IList<string> Button { get; set; }
        public IList<string> Options { get; set; }
        public bool Checkbox { get; set; }
        public IList<string> Text { get; set; }
        
        public IList<TextStyles> LineStyles { get;set;}

        public void AddText(string line)
        {
            if (line.StartsWith("^^"))
            {
                (LineStyles ??= new List<TextStyles>()).Add(TextStyles.Centered);
                (Text ??= new List<string>()).Add(line[2..]);
            }
            else if (line.StartsWith("^"))
            {
                (LineStyles ??= new List<TextStyles>()).Add(TextStyles.LeftOwnLine);
                (Text ??= new List<string>()).Add(line[1..]);
            }
            else
            {
                (LineStyles ??= new List<TextStyles>()).Add(TextStyles.Left);
                (Text ??= new List<string>()).Add(line);
            }
        }
    }

    public enum TextStyles
    {
        Centered,
        LeftOwnLine,
        Left
    }
}
