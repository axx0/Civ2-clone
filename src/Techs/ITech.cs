using civ2.Enums;

namespace civ2.Techs
{
    public interface ITech
    {
        //From RULES.TXT
        string Name { get; set; }
        int AIvalue { get; set; }
        int Modifier { get; set; }
        TechType Prereq1 { get; set; }
        TechType Prereq2 { get; set; }
        int Epoch { get; set; }
        int Category { get; set; }

        int Id { get; }
        TechType Type { get; set; }
    }
}
