using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Techs
{
    internal class MobileWarf : BaseTech
    {
        public MobileWarf() : base(8, -1, TechType.Automobile, TechType.Tactics, 3, 0)
        {
            Type = TechType.MobileWarf;
            Name = "Mobile Warfare";
        }
    }
}
