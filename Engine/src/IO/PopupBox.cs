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
        public bool Listbox { get; set; }
        public int ListboxLines { get; set; } = 0;
        public IList<string> Text { get; set; }
        
        public IList<TextStyles> LineStyles { get;set;}
    }

    public enum TextStyles
    {
        Centered,
        LeftOwnLine,
        Left
    }
}
