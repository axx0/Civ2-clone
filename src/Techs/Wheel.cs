using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Techs
{
    internal class Wheel : BaseTech
    {
        public Wheel() : base(4, -1, TechType.HorsebackRid, TechType.None, 0, 4)
        {
            Type = TechType.Wheel;
            Name = "The Wheel";
        }
    }
}
