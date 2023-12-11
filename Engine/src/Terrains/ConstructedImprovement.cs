using System;

namespace Civ2engine.Terrains
{
    public class ConstructedImprovement
    {
        public int Improvement { get; set; }
        
        public int Level { get; set; }
        
        public int Group { get; set; }

        public bool IsMatch(ConstructedImprovement other)
        {
            return Improvement == other.Improvement && Level == other.Level;
        }
    }
}