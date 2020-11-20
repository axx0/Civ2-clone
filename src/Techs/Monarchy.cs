using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Techs
{
    internal class Monarchy : BaseTech
    {
        public Monarchy() : base(5, 1, TechType.CeremBurial, TechType.CodeLaws, 0, 2)
        {
            Type = TechType.Monarchy;
            Name = "Monarchy";
        }
    }
}
