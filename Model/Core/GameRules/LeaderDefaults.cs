using Model.Core.Advances;

namespace Model.Core.GameRules
{
    public class LeaderDefaults
    {
        public string NameMale { get; set; } = string.Empty;
        public string NameFemale { get; set; } = string.Empty;
        public bool Female { get; set; }
        public int Color { get; set; }
        public int CityStyle { get; set; }
        public string Plural { get; set; } = string.Empty;
        public string Adjective { get; set; } = string.Empty;
        public int Attack { get; set; }
        public int Expand { get; set; }
        public int Civilize { get; set; }

        public LeaderTitle[] Titles { get; set; } = [];
        
        public AdvanceGroupAccess[]? AdvanceGroups { get; set; }
        public int TribeId { get; set; }
    }
}
