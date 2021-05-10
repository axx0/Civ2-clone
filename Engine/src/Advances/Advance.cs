using System;
using Civ2engine.Enums;

namespace Civ2engine.Advances
{
    public class Advance
    {
        public string Name { get; set; }
        public int AIvalue { get; set; }
        public int Modifier { get; set; }
        public int Prereq1 { get; set; }
        public int Prereq2{ get; set; }
        public EpochType Epoch { get; set; }
        public KnowledgeType KnowledgeCategory { get; set; }
    }
}
