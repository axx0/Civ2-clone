using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Techs
{
    internal class Chivalry : BaseTech
    {
        public Chivalry() : base(4, -2, TechType.Feudalism, TechType.HorsebackRid, 1, 0)
        {
            Type = TechType.Chivalry;
            Name = "Chivalry";
        }
    }
}
