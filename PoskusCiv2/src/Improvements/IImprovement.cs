using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RTciv2.Enums;

namespace RTciv2.Improvements
{
    public interface IImprovement
    {
        //From RULES.TXT
        string Name { get; set; }
        int Cost { get; set; }
        int Upkeep { get; set; }
        TechType Prerequisite { get; set; }
        TechType AdvanceExpiration { get; set; }

        int Id { get; }
        ImprovementType Type { get; set; }
    }
}
