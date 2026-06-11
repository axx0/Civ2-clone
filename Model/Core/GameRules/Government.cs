namespace Model.Core.GameRules
{
    public class Government : IGovernmentTitles
    {
        public string Name { get; set; } = string.Empty;
        public string TitleMale { get; set; } = string.Empty;
        public string TitleFemale { get; set; } = string.Empty;
        public int Level { get; set; }
        public int NumberOfFreeUnitsPerCity { get; set; }
        public int[] UnitTypesAlwaysFree { get; set; } = [];
        public int Distance { get; set; }
        public int SettlersConsumption { get; set; }

        public Dictionary<string, int> MaxRates { get; set; } = new();
        
        public Dictionary<string, int> GlobalResourceWastage { get; set; } = new();
    }
}
