using System.Collections.Generic;
using Civ2engine.Enums;

namespace Civ2engine.Advances
{
    public class Advance
    {
        public string Name { get; internal set; }
        public int AIvalue { get; internal set; }
        public int Modifier { get;internal  set; }
        public int Prereq1 { get; internal set; }
        public int Prereq2{ get;internal set; }
        public EpochType Epoch { get; internal set; }
        public KnowledgeType KnowledgeCategory { get; internal set; }
        
        public int AdvanceGroup { get; internal set; }
        public int Index { get; internal set; }
        public List<ConstructionAbility> ImprovementsEnabled { get; } = new();
    }
}
