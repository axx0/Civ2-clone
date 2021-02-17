using Civ2engine.Enums;

namespace Civ2engine.Improvements
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
