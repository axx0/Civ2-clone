using civ2.Enums;

namespace civ2.Techs
{
    internal class Tech : ITech
    {
        //From RULES.TXT
        public string Name { get; set; }
        public int AIvalue { get; set; }
        public int Modifier { get; set; }
        public TechType Prereq1 { get; set; }
        public TechType Prereq2 { get; set; }
        public int Epoch { get; set; }
        public int Category { get; set; }

        public TechType Type { get; set; }
        public int Id => (int)Type;

        //When making a new tech
        public Tech(TechType type)
        {
            Name = Rules.TechName[(int)(type)];
            AIvalue = Rules.TechAIvalue[(int)(type)];
            Modifier = Rules.TechModifier[(int)(type)];
            //find correct Prereq1 TO-DO
            //find correct Prereq2 TO-DO
            Epoch = Rules.TechEpoch[(int)(type)];
            Category = Rules.TechCategory[(int)(type)];
        }
    }
}
