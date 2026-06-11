namespace Model.Core.GameRules
{
    public class LeaderTitle : IGovernmentTitles
    {
        public int Gov { get; set; }
        public string TitleMale { get; set; } = string.Empty;
        public string TitleFemale { get; set; } = string.Empty;
    }
}
