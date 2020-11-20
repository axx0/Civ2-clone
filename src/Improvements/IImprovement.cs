using civ2.Enums;

namespace civ2.Improvements
{
    public interface IImprovement
    {
        //From RULES.TXT
        string Name { get; set; }
        int Cost { get; set; }
        int Upkeep { get; set; }
        TechType Prerequisite { get; set; }
        TechType AdvanceExpiration { get; set; }

        int Id { get; }
        ImprovementType Type { get; set; }
    }
}
