namespace Model.Core.GameRules
{
    public class Orders
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Key { get; set; } = string.Empty;
        public int Type { get; set; }
    }
}
