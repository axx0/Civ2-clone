using Eto.Drawing;
using System.Collections.Generic;

namespace EtoFormsUI
{
    public class ListboxDefinition
    {
        /// <summary>
        /// List of left-aligned texts
        /// </summary>
        public List<string> LeftText { get; set; }

        /// <summary>
        /// List of right-aligned texts
        /// </summary>
        public List<string> RightText { get; set; }
     
        /// <summary>
        /// List of icons left of text
        /// </summary>
        public List<Bitmap> Icons { get; set; }
    }
}