using Model.Constants;

namespace Model.Core.Advances
{
    public class Advance
    {
        public string Name { get; set; }
        public int AIvalue { get; set; }
        public int Modifier { get; set; }
        public int Prereq1 { get; set; }
        public int Prereq2{ get; set; }
        public int Epoch { get; set; }
        public int KnowledgeCategory { get; set; }
        
        public int AdvanceGroup { get; set; }
        public int Index { get; set; }
        public Dictionary<Effects, int> Effects { get; } = new();
    }
}
