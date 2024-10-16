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

        /// <summary>
        /// Dialog offset in pixels (neg.=from right edge, 0=center)
        /// </summary>
        public int? X { get; set; }

        /// <summary>
        /// Dialog offset in pixels (neg.=from bottom edge, 0=center)
        /// </summary>
        public int? Y { get; set; }

        public IList<string>? Button { get; set; }
        public IList<string>? Options { get; set; }

        /// <summary>
        /// False = radio buttons
        /// </summary>
        public bool Checkbox { get; set; }

        public bool Listbox { get; set; }
        public int ListboxLines { get; set; }
        public IList<string>? Text { get; set; }
        public IList<TextStyles>? LineStyles { get;set;}
    }
}
