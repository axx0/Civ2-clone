namespace Model.Core.Advances
{
    public class AdvanceResearch
    {
        public bool Discovered => DiscoveredBy != -1;

        public int DiscoveredBy { get; set; } = -1;
    }
}