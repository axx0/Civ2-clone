using System.Collections.Generic;
using Model.Core;

namespace Civ2engine
{
    /// <summary>
    /// Varios dialog properties read from GAME.TXT.
    /// </summary>
    public class PopupBox
    {
        public string? Name { get; set; }

        /// <summary>
        /// Requested width of popup.
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Header text.
        /// </summary>
        public string? Title { get; set; }

        /// <summary>
        /// Default initial selected option.
        /// </summary>
        public int? Default { get; set; }

        /// <summary>
        /// Popup offset in pixels (neg.=from right edge, 0=center)
        /// </summary>
        public int? X { get; set; }

        /// <summary>
        /// Popup offset in pixels (neg.=from bottom edge, 0=center)
        /// </summary>
        public int? Y { get; set; }

        /// <summary>
        /// Text in buttons.
        /// </summary>
        public IList<string>? Button { get; set; }

        /// <summary>
        /// Text of options.
        /// </summary>
        public IList<string>? Options { get; set; }

        /// <summary>
        /// False = radio buttons
        /// </summary>
        public bool Checkbox { get; set; }

        /// <summary>
        /// Indicates whether popup has listbox.
        /// </summary>
        public bool Listbox { get; set; } = false;

        /// <summary>
        /// No of lines the listbox has.
        /// </summary>
        public int? ListboxLines { get; set; }

        public IList<string>? Text { get; set; }
        public IList<TextStyles>? LineStyles { get; set;}
    }
}
