using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Techs
{
    internal class MassProd : BaseTech
    {
        public MassProd() : base(5, 0, TechType.Automobile, TechType.Corporat, 3, 4)
        {
            Type = TechType.MassProd;
            Name = "Mass Production";
        }
    }
}
