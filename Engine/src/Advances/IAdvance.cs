using Civ2engine.Enums;

namespace Civ2engine.Advances
{
    public interface IAdvance
    {
        AdvanceType Type { get; set; }

        //From RULES.TXT
        string Name { get; }
        int AIvalue { get; }
        int Modifier { get; }
        AdvanceType? Prereq1 { get; }
        AdvanceType? Prereq2 { get; }
        EpochType Epoch { get; }
        KnowledgeType KnowledgeCategory { get; }
    }
}
