using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Techs
{
    internal class Electronics : BaseTech
    {
        public Electronics() : base(4, 1, TechType.Electricity, TechType.Corporat, 3, 4)
        {
            Type = TechType.Electronics;
            Name = "Electronics";
        }
    }
}
