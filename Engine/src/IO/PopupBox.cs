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
        public int X { get; set; }
        public int Y { get; set; }
        public List<string> Button { get; set; }
        public List<string> Options { get; set; }
        public bool Checkbox { get; set; }
        public List<string> Text { get; set; }
        
        public List<TextStyles> LineStyles { get;set;}
    }

    public enum TextStyles
    {
        Centered,
        Left,
        Smaller
    }
}
