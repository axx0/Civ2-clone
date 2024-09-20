namespace Civ2engine
{
    public class Government : IGovernmentTitles
    {
        public string Name { get; set; }
        public string TitleMale { get; set; }
        public string TitleFemale { get; set; }
        public int Level { get; set; }
        
        public int NumberOfFreeUnitsPerCity { get; set; }
        
        public int[] UnitTypesAlwaysFree { get; set; }
        public int Distance { get; set; }
        public int SettlersConsumption { get; set; }
    }
}