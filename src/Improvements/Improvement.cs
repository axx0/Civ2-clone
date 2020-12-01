using System;
using civ2.Enums;

namespace civ2.Improvements
{
    internal class Improvement : BaseInstance, IImprovement
    {
        public int Id => (int)Type;
        public ImprovementType Type { get; set; }

        //From RULES.TXT
        public string Name => Game.Rules.ImprovementName[(int)Type];
        public int Cost => 10 * Game.Rules.ImprovementCost[(int)Type];
        public int Upkeep => Game.Rules.ImprovementUpkeep[(int)Type];
        public AdvanceType? Prerequisite
        {
            get
            {
                if (Game.Rules.ImprovementPrereq[(int)Type] == "nil")
                    return null;
                else
                    return (AdvanceType)Array.IndexOf(Game.Rules.AdvanceShortName, Game.Rules.ImprovementPrereq[(int)Type]);
            }
        }
        public AdvanceType? Expiration
        {
            get
            {
                if (Game.Rules.ImprovementAdvanceExpiration[(int)Type] == "nil")
                    return null;
                else
                    return (AdvanceType)Array.IndexOf(Game.Rules.AdvanceShortName, Game.Rules.ImprovementAdvanceExpiration[(int)Type]);
            }
        }

        public Improvement(ImprovementType type)
        {
            Type = type;
        }
    }
}
