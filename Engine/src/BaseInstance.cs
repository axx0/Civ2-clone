namespace Civ2engine
{
    public abstract class BaseInstance
    {
        protected static Game Game => Game.Instance;
        protected static Map Map => Map.Instance;
    }
}
