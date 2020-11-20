using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Techs
{
    internal class Railroad : BaseTech
    {
        public Railroad() : base(6, 0, TechType.SteamEngine, TechType.BridgeBuild, 2, 1)
        {
            Type = TechType.Railroad;
            Name = "Railroad";
        }
    }
}
