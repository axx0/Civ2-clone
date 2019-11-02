using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RTciv2.Enums;

namespace RTciv2.Techs
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
