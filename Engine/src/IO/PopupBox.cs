using System.Collections.Generic;

namespace Civ2engine
{
    public class PopupBox
    {
        public string Name { get; set; }
        public int Width { get; set; }
        public List<string> LeftText { get; set; }
        public List<string> CenterText { get; set; }
        public string Title { get; set; }
        public int Default { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public List<string> Button { get; set; }
        public List<string> Options { get; set; }
    }
}
