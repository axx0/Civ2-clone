namespace Civ2engine.Terrains
{
    public class ConstructedImprovement
    {
        public ConstructedImprovement()
        {
            
        }

        public ConstructedImprovement(ConstructedImprovement other)
        {
            Improvement = other.Improvement;
            Group = other.Group;
            Level = other.Level;
        }
        public int Improvement { get; set; }
        
        public int Level { get; set; }
        
        public int Group { get; set; }
    }
}