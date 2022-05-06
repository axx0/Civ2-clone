namespace Civ2engine.MapObjects
{
    public class ActiveEffect
    {
        public ActiveEffect(TerrainImprovementAction action, int source, int levelNo = 0)
        {
            Target = action.Target;
            Action = action.Action;
            Value = action.Value;
            Source = source;
            Level = levelNo;
        }

        public int Target { get; set; }
        public int Action { get; set; }
        public int Value { get; set; }
        public int Source { get; set; }
        
        public int Level { get; set; }
    }
}