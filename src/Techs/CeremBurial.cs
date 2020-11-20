using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Techs
{
    internal class CeremBurial : BaseTech
    {
        public CeremBurial() : base(5, 0, TechType.None, TechType.None, 0, 2)
        {
            Type = TechType.CeremBurial;
            Name = "Ceremonial Burial";
        }
    }
}
