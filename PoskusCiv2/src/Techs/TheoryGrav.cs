using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Techs
{
    internal class TheoryGrav : BaseTech
    {
        public TheoryGrav() : base(4, 0, TechType.Astronomy, TechType.University, 1, 3)
        {
            Type = TechType.TheoryGrav;
            Name = "Theory of Gravity";
        }
    }
}
