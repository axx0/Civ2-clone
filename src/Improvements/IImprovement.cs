using civ2.Enums;

namespace civ2.Improvements
{
    public interface IImprovement
    {
        int Id { get; }
        ImprovementType Type { get; set; }

        //From RULES.TXT
        string Name { get; }
        int Cost { get; }
        int Upkeep { get; }
        AdvanceType? Prerequisite { get; }
        AdvanceType? Expiration { get; }

    }
}
