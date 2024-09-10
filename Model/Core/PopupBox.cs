using System.Collections.Generic;
using Model.Core;

namespace Civ2engine
{
    public class PopupBox
    {
        public string? Name { get; set; }
        public int Width { get; set; }
        public string? Title { get; set; }
        public int Default { get; set; }
        public int? X { get; set; }
        public int? Y { get; set; }
        public IList<string>? Button { get; set; }
        public IList<string>? Options { get; set; }
        public bool Checkbox { get; set; }
        public bool Listbox { get; set; }
        public int ListboxLines { get; set; }
        public IList<string>? Text { get; set; }
        public IList<TextStyles>? LineStyles { get;set;}
    }
}
