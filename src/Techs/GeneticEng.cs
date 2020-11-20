using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Techs
{
    internal class GeneticEng : BaseTech
    {
        public GeneticEng() : base(3, 2, TechType.Medicine, TechType.Corporat, 3, 3)
        {
            Type = TechType.GeneticEng;
            Name = "Genetic Engineering";
        }
    }
}
