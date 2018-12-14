using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Techs
{
    internal class Refining : BaseTech
    {
        public Refining() : base(4, 0, TechType.Chemistry, TechType.Corporat, 2, 4)
        {
            Type = TechType.Refining;
            Name = "Refining";
        }
    }
}
