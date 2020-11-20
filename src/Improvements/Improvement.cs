using civ2.Enums;

namespace civ2.Improvements
{
    internal class Improvement : IImprovement
    {
        //From RULES.TXT
        public string Name { get; set; }
        public int Cost { get; set; }
        public int Upkeep { get; set; }
        public TechType Prerequisite { get; set; }
        public TechType AdvanceExpiration { get; set; }

        public ImprovementType Type { get; set; }
        public int Id => (int)Type;

        //When making a new improvement
        public Improvement(ImprovementType type)
        {
            Type = type;
            Name = ReadFiles.ImprovementName[(int)(type)];
            Cost = ReadFiles.ImprovementCost[(int)(type)];
            Upkeep = ReadFiles.ImprovementUpkeep[(int)(type)];
            //find correct Prereq TO-DO
            //find correct advance expiration TO-DO
        }

    }
}
