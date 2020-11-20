using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Techs
{
    internal class Mysticism : BaseTech
    {
        public Mysticism() : base(4, 0, TechType.CeremBurial, TechType.None, 0, 2)
        {
            Type = TechType.Mysticism;
            Name = "Mysticism";
        }
    }
}
