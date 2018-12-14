using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Techs
{
    internal class Banking : BaseTech
    {
        public Banking() : base(4, 1, TechType.Trade, TechType.Republic, 1, 1)
        {
            Type = TechType.Banking;
            Name = "Banking";
        }
    }
}
