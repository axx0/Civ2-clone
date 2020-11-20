using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Techs
{
    internal class GuerillaWar : BaseTech
    {
        public GuerillaWar() : base(4, 1, TechType.Communism, TechType.Tactics, 3, 0)
        {
            Type = TechType.GuerillaWar;
            Name = "Guerilla Warfare";
        }
    }
}
