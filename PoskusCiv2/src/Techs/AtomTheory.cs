using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Techs
{
    internal class AtomTheory : BaseTech
    {
        public AtomTheory() : base(4, -1, TechType.TheoryGrav, TechType.Physics, 2, 3)
        {
            Type = TechType.AtomTheory;
            Name = "Atomic Theory";
        }
    }
}
