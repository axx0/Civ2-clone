using System;
using civ2.Enums;

namespace civ2.Advances
{
    public class Advance : BaseInstance, IAdvance
    {
        public AdvanceType Type { get; set; }

        // From RULES.TXT
        public string Name => Game.Rules.TechName[(int)Type];
        public int AIvalue => Game.Rules.TechAIvalue[(int)Type];
        public int Modifier => Game.Rules.TechModifier[(int)Type];
        public AdvanceType Prereq1 => (AdvanceType)Array.IndexOf(Game.Rules.TechShortName, Game.Rules.TechPrereq1[(int)Type]);
        public AdvanceType Prereq2 => (AdvanceType)Array.IndexOf(Game.Rules.TechShortName, Game.Rules.TechPrereq2[(int)Type]);
        public EpochType TechEpoch => (EpochType)Game.Rules.TechEpoch[(int)Type];
        public KnowledgeType TechCategory => (KnowledgeType)Game.Rules.TechCategory[(int)Type];
    }
}
