namespace Model.Core.Mapping
{
    public class ActiveEffect(TerrainImprovementAction action, int source, int levelNo = 0)
    {
        public int Target { get; set; } = action.Target;
        public int Action { get; set; } = action.Action;
        public int Value { get; set; } = action.Value;
        public int Source { get; set; } = source;

        public int Level { get; set; } = levelNo;
    }
}