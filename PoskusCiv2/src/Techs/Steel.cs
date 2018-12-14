using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Techs
{
    internal class Steel : BaseTech
    {
        public Steel() : base(4, -1, TechType.Electricity, TechType.Industrializ, 2, 4)
        {
            Type = TechType.Steel;
            Name = "Steel";
        }
    }
}
