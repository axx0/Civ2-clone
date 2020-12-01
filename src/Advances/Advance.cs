using System;
using civ2.Enums;

namespace civ2.Advances
{
    public class Advance : BaseInstance, IAdvance
    {
        public AdvanceType Type { get; set; }

        // From RULES.TXT
        public string Name => Game.Rules.AdvanceName[(int)Type];
        public int AIvalue => Game.Rules.AdvanceAIvalue[(int)Type];
        public int Modifier => Game.Rules.AdvanceModifier[(int)Type];
        public AdvanceType? Prereq1
        {
            get
            {
                if (Game.Rules.AdvancePrereq1[(int)Type] == "nil")
                    return null;
                else
                    return (AdvanceType)Array.IndexOf(Game.Rules.AdvanceShortName, Game.Rules.AdvancePrereq1[(int)Type]);
            }
        }
        public AdvanceType? Prereq2
        {
            get
            {
                if (Game.Rules.AdvancePrereq2[(int)Type] == "nil")
                    return null;
                else
                    return (AdvanceType)Array.IndexOf(Game.Rules.AdvanceShortName, Game.Rules.AdvancePrereq2[(int)Type]);
            }
        }
        public EpochType Epoch => (EpochType)Game.Rules.AdvanceEpoch[(int)Type];
        public KnowledgeType KnowledgeCategory => (KnowledgeType)Game.Rules.AdvanceCategory[(int)Type];
    }
}
