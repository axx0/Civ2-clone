using System;
using Civ2engine.Enums;

namespace Civ2engine.Improvements
{
    public class Improvement
    {
        public int Id => (int)Type;
        public ImprovementType Type { get; set; }

        //From RULES.TXT
        public string Name { get; set; }

        public int Cost { get; set; }
        public int Upkeep { get; set; }
        public int Prerequisite { get; set; }

        public int ExpiresAt { get; set; } = -1;
    }
}
